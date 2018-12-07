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
        public RobotJoint[] Joints = null;

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

        public PositionRotation[] JointSim = null;

        // Use this for initialization

        private void Awake()
        {
            ErrorFunction = DistanceFromTarget;
        }
        void Start()
        {
            //GetJoints();
        }

        [ExposeInEditor(RuntimeOnly = false)]
        public void GetJoints()
        {
            Joints = GetComponentsInChildren<RobotJoint>();
        }

        public List<float[]> TestPath(List<float[]> SolutionSteps, PositionRotation[] joints, PositionRotation endPoint)
        {
            List<float[]> m_solutionSteps = new List<float[]>();
            for (int i = 0; i < SolutionSteps.Count; i++)
            {
                float[] steps = new float[SolutionSteps[i].Length];
                for (int j = 0; j < SolutionSteps[i].Length; j++)
                {
                    steps[j] = SolutionSteps[i][j];
                }
                m_solutionSteps.Add(steps);
            }
            float[] m_Solution = new float[Joints.Length];
            for (int j = 0; j < SolutionSteps[m_solutionSteps.Count - 1].Length; j++)
            {
                m_Solution[j] = m_solutionSteps[m_solutionSteps.Count - 1][j];
            }

            JointSim = new PositionRotation[Joints.Length];
            string debugPrint = "";
            for (var i = 0; i < joints.Length; i++)
            {
                JointSim[i] = joints[i];
                debugPrint += JointSim[i] + "\n";
            }
            //JointSim = joints;
            //Debug.Log(debugPrint);
            //Debug.Log("Step point is: " + endPoint);
            var target = endPoint;
            for (var i = 0; i < MaximumLoop; i++)
            {
                if (ErrorFunction(target, m_Solution) > StopThreshold)
                {
                    var newSoln = ApproachTarget(target, m_Solution);
                    float[] steps = new float[m_Solution.Length];
                    for (int j = 0; j < m_Solution.Length; j++)
                    {
                        steps[j] = m_Solution[j];
                    }
                    m_solutionSteps.Add(steps);

                    /* Debug Print Array Start */
                    debugPrint = "";
                    foreach (var angle in m_Solution)
                    {
                        debugPrint += angle.ToString() + "\n";
                    }
                    //Debug.Log("Intermediate soln " + i + ":\n" + debugPrint);
                    //Debug.Log("Solution list is " + m_solutionSteps.Count + " steps long");
                    /* Debug Print Array End */

                    UpdateJointPosition(m_Solution);

                    /* Debug Print Array Start */
                    debugPrint = "";
                    foreach (var angle in JointSim)
                    {
                        debugPrint += angle.ToString() + "\n";
                    }
                    //Debug.Log("Joints at " + i + " are: " + debugPrint);
                    /* Debug Print Array End */
                }
                else
                {
                    return m_solutionSteps;
                }
            }
            //Debug.Log("No path found");
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
                jointStart = FKSimBuild(jointStart, soln);
            }

            /* Debug Print Array Start */
            var debugPrint = "";
            foreach (var angle in jointStart)
            {
                debugPrint += angle.ToString() + "\n";
            }
            Debug.Log("Got to joint position: " + debugPrint);
            /* Debug Print Array End */

            return jointStart;
        }

        public void InitializeJointSim()
        {
            JointSim = new PositionRotation[Joints.Length];
            for (var i = 0; i < Joints.Length; i++)
            {
                JointSim[i] = new PositionRotation(Joints[i].transform.position, Joints[i].transform.rotation);
                //Debug.Log(JointSim[i]);
            }
        }

        public float[] GetStartingAngles()
        {
            var startAngle = new float[Joints.Length];
            for (var i = Joints.Length - 1; i >= 0; i--)
            {
                startAngle[i] = Joints[i].GetAngle();
                Debug.Log(startAngle[i]);
            }
            return startAngle;
        }

        public float[] ApproachTarget(PositionRotation target, float[] m_Solution)
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
                    return m_Solution;
            }
            return m_Solution;
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
            Vector3 point = ForwardKinematics(m_Solution);
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
            Quaternion rotation = JointSim[0];//.rotation;

            for (int i = 1; i < Joints.Length; i++)
            {
                // Rotates around a new axis
                rotation *= Quaternion.AngleAxis(m_Solution[i - 1], Joints[i - 1].Axis);
                Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;
                //Debug.Log(rotation);

                prevPoint = nextPoint;
            }

            // The end of the effector
            return new PositionRotation(prevPoint, rotation);
        }

        public void UpdateJointPosition(float[] m_Solution)
        {
            var newJoints = FKSimBuild(JointSim, m_Solution);
            for (int i = 0; i < newJoints.Length; i++)
            {
                JointSim[i] = newJoints[i] ;
            }
        }

        public PositionRotation[] FKSimBuild(PositionRotation[] prevJoints, float[] newAngles)
        {
            //var newJointSim = new PositionRotation[prevJoints.Length];
            var newJointSim = new PositionRotation[prevJoints.Length];
            Vector3 prevPoint = prevJoints[0];
            //Quaternion rotation = Quaternion.identity;

            // Takes object initial rotation into account
            Quaternion rotation = prevJoints[0];// transform.rotation;

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
