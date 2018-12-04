using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    //[ExecuteInEditMode]
    public class IKSolver : MonoBehaviour
    {
        public int MaximumLoop = 1000;

        [Header("Joints")]
        //[HideInInspector]
        [ReadOnly]
        public ArmJoint[] Joints = null;

        [Header("Inverse Kinematics")]
        [Range(0, 1f)]
        public float DeltaGradient = 0.01f; // Used to simulate gradient (degrees)
        [Range(0, 100f)]
        public float LearningRate = 25.0f; // How much we move depending on the gradient

        [Space()]
        [Range(0, 0.25f)]
        public float StopThreshold = 0.1f; // If closer than this, it stops
        [Range(0, 10f)]
        public float SlowdownThreshold = 0.25f; // If closer than this, it linearly slows down

        public ErrorFunction ErrorFunction;

        private PositionRotation[] JointSim = null;

        // Use this for initialization

        private void Awake()
        {
            ErrorFunction = DistanceFromTarget;
        }
        void Start()
        {
            if (Joints == null)
                GetJoints();
        }

        [ExposeInEditor(RuntimeOnly = false)]
        public void GetJoints()
        {
            Joints = GetComponentsInChildren<ArmJoint>();
        }

        public List<float[]> TestPath(List<float[]> SolutionSteps, PositionRotation[] joints, PositionRotation endPoint)
        {
            List<float[]> m_solutionSteps = new List<float[]>(SolutionSteps);
            /*float[] m_Solution = new float[Joints.Length];
            foreach (var soln in startPoint.SolutionSteps)
            {
                m_Solution = soln;
                m_solutionSteps.Add(m_Solution);
            }*/
            float[] m_Solution = new float[Joints.Length];
            m_Solution = m_solutionSteps[m_solutionSteps.Count-1];
            JointSim = new PositionRotation[Joints.Length];
            for (var i = 0; i < Joints.Length; i++)
            {
                JointSim[i] = joints[i];
            }
            //JointSim = joints;
            //Debug.Log(JointSim);
            Debug.Log(endPoint);
            var target = endPoint;
            for (var i = 0; i < MaximumLoop; i++)
            {
                if (ErrorFunction(target, m_Solution) > StopThreshold)
                {
                    if (ApprochTarget(target, ref m_Solution))
                    {
                        m_solutionSteps.Add(m_Solution);
                        var debugPrint = "";
                        foreach (var angle in m_Solution)
                        {
                            debugPrint += angle.ToString() + "\n";
                        }
                        Debug.Log("Intermediate soln " + i + ":\n" + debugPrint);
                        Debug.Log("Solution list is " + m_solutionSteps.Count + " steps long");
                        UpdateJointPosition(m_Solution);
                    }
                    else
                    {
                        m_solutionSteps.Add(m_Solution);
                        return m_solutionSteps;
                    }
                }
                else
                {
                    return m_solutionSteps;
                }
            }
            Debug.Log("No path found");
            return null;
        }

        public PositionRotation[] GetJointAngles(List<float[]> SolutionPath)
        {
            var jointStart = new PositionRotation[Joints.Length];
            for (var i = 0; i < Joints.Length; i++)
            {
                jointStart[i] = new PositionRotation(Joints[i].transform.position, Joints[i].transform.rotation);
            }
            //var jointStart = Joints;
            foreach (var soln in SolutionPath)
            {
                jointStart = ForwardKinematics(jointStart, soln);
            }

            var debugPrint = "";
            foreach (var angle in jointStart)
            {
                debugPrint += angle.ToString() + "\n";
            }
            Debug.Log(debugPrint);
            return jointStart;
        }

        public float[] GetStartingAngles()
        {
            var startAngle = new float[Joints.Length];
            for (var i = Joints.Length - 1; i >= 0; i--)
                startAngle[i] = Joints[i].GetAngle();
            return startAngle;
        }

        public bool ApprochTarget(PositionRotation target, ref float[] m_Solution)
        {
            // Starts from the end, up to the base
            // Starts from joints[end-2]
            //  so it skips the hand that doesn't move!
            for (int i = Joints.Length - 1; i >= 0; i--)
            //for (int i = 0; i < Joints.Length - 1 - 1; i++)
            {
                // FROM:    error:      [0, StopThreshold,  SlowdownThreshold]
                // TO:      slowdown:   [0, 0,              1                ]
                float error = ErrorFunction(target, m_Solution);
                float slowdown = Mathf.Clamp01((error - StopThreshold) / (SlowdownThreshold - StopThreshold));

                // Gradient descent
                float gradient = CalculateGradient(target, m_Solution, i, DeltaGradient);
                m_Solution[i] -= LearningRate * gradient * slowdown;
                // Clamp
                m_Solution[i] = Joints[i].ClampAngle(m_Solution[i]);

                // Early termination
                if (ErrorFunction(target, m_Solution) <= StopThreshold)
                    return false;
            }
            return true;
        }

        /* Calculates the gradient for the invetse kinematic.
         * It simulates the forward kinematics the i-th joint,
         * by moving it +delta and -delta.
         * It then sees which one gets closer to the target.
         * It returns the gradient (suggested changes for the i-th joint)
         * to approach the target. In range (-1,+1)
         */
        public float CalculateGradient(PositionRotation target, float[] m_Solution, int i, float delta)
        {
            // Saves the angle,
            // it will be restored later
            float solutionAngle = m_Solution[i];

            // Gradient : [F(x+h) - F(x)] / h
            // Update   : Solution -= LearningRate * Gradient
            float f_x = ErrorFunction(target, m_Solution);

            m_Solution[i] += delta;
            float f_x_plus_h = ErrorFunction(target, m_Solution);

            float gradient = (f_x_plus_h - f_x) / delta;

            // Restores
            m_Solution[i] = solutionAngle;

            return gradient;
        }

        // Returns the distance from the target, given a solution
        public float DistanceFromTarget(PositionRotation target, float[] m_Solution)
        {
            PositionRotation point = ForwardKinematics(m_Solution);
            //Debug.Log("Distance from target is: " + Vector3.Distance(point, target));
            return Vector3.Distance(point, target);// + Quaternion.Angle(point,target);
        }


        /* Simulates the forward kinematics,
         * given a solution. */
        public PositionRotation ForwardKinematics(float[] m_Solution)
        {
            Vector3 prevPoint = JointSim[0];
            //Quaternion rotation = Quaternion.identity;

            // Takes object initial rotation into account
            Quaternion rotation = JointSim[0];

            for (int i = 1; i < Joints.Length; i++)
            {
                // Rotates around a new axis
                rotation *= Quaternion.AngleAxis(m_Solution[i - 1], Joints[i - 1].Axis);
                Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;

                prevPoint = nextPoint;
            }

            // The end of the effector
            return new PositionRotation(prevPoint, rotation);
        }

        public void UpdateJointPosition(float[] m_Solution)
        {
            JointSim = ForwardKinematics(JointSim, m_Solution); ;
        }

        public PositionRotation[] ForwardKinematics(PositionRotation[] prevJoints, float[] newAngles)
        {
            //var newJointSim = new PositionRotation[prevJoints.Length];
            var newJointSim = prevJoints;
            Vector3 prevPoint = prevJoints[0];
            //Quaternion rotation = Quaternion.identity;

            // Takes object initial rotation into account
            Quaternion rotation = prevJoints[0];

            newJointSim[0] = prevJoints[0];
            for (int i = 1; i < Joints.Length; i++)
            {
                // Rotates around a new axis
                rotation *= Quaternion.AngleAxis(newAngles[i - 1], Joints[i - 1].Axis);
                Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;

                prevPoint = nextPoint;
                newJointSim[i] = new PositionRotation(prevPoint,rotation);
                //newJointSim[i].transform.position = nextPoint;
                //newJointSim[i].transform.rotation = rotation;
            }

            return newJointSim;
        }
    }
}
