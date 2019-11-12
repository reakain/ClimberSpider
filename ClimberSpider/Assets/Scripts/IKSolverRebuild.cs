using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot {
    public class IKSolverRebuild : MonoBehaviour
    {
        public int MaximumLoop = 1000;

        [Header("Joints")]
        //[HideInInspector]
        [ReadOnly]
        protected RobotJoint[] Joints = null;

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

        protected Vector3[] JointSim = null;

        private void Awake()
        {
            ErrorFunction = DistanceFromTarget;
        }
        
        private void Start()
        {
            if (Joints == null) { Joints = GetComponentsInChildren<RobotJoint>(); }
        }

        private void Update()
        {
            
        }

        public List<float[]> TestPath(List<float[]> SolutionSteps, Vector3[] joints, Vector3 endPoint)
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

            JointSim = new Vector3[Joints.Length];
            string debugPrint = "";
            for (var i = 0; i < joints.Length; i++)
            {
                JointSim[i] = new Vector3(joints[i].x, joints[i].y, joints[i].z);
                debugPrint += JointSim[i] + "\n";
            }
            //JointSim = joints;
            Debug.Log(debugPrint);
            Debug.Log("Step point is: " + endPoint);
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
                    Debug.Log("Intermediate soln " + i + ":\n" + debugPrint);
                    Debug.Log("Solution list is " + m_solutionSteps.Count + " steps long");
                    /* Debug Print Array End */

                    UpdateJointPosition(m_Solution);

                    /* Debug Print Array Start */
                    debugPrint = "";
                    foreach (var angle in JointSim)
                    {
                        debugPrint += angle.ToString() + "\n";
                    }
                    Debug.Log("Joints at " + i + " are: " + debugPrint);
                    /* Debug Print Array End */
                }
                else
                {
                    return m_solutionSteps;
                }
            }
            Debug.Log("No path found");
            return null;
        }

        public Vector3[] GetJointPose(List<float[]> SolutionPath)
        {
            var jointStart = new Vector3[Joints.Length];
            for (var i = 0; i < Joints.Length; i++)
            {
                jointStart[i] = new Vector3(Joints[i].HomePose.position.x, Joints[i].HomePose.position.y, Joints[i].HomePose.position.z);
            }

            foreach (var soln in SolutionPath)
            {
                var newJoints = FKSimBuild(jointStart, soln);
                for (int i = 0; i < newJoints.Length; i++)
                {
                    jointStart[i] = new Vector3(newJoints[i].x, newJoints[i].y, newJoints[i].z);
                }
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
            JointSim = new Vector3[Joints.Length];
            for (var i = 0; i < Joints.Length; i++)
            {
                JointSim[i] = new Vector3(Joints[i].transform.position.x, Joints[i].transform.position.y, Joints[i].transform.position.z);
                //Debug.Log(JointSim[i]);
            }
        }

        public float[] GetHomeAngles()
        {
            var startAngle = new float[Joints.Length];
            for (var i = Joints.Length - 1; i >= 0; i--)
            {
                startAngle[i] = Joints[i].GetZeroAngle();
                //Debug.Log(startAngle[i]);
            }
            return startAngle;
        }

        public float[] ApproachTarget(Vector3 target, float[] m_Solution)
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
        public float CalculateGradient(Vector3 target, float[] m_Solution, int i, float delta)
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
        public float DistanceFromTarget(Vector3 target, float[] m_Solution)
        {
            Vector3 point = ForwardKinematics(m_Solution);
            //Debug.Log("Distance from target is: " + Vector3.Distance(point, target));
            return Vector3.Distance(point, target);// + Quaternion.Angle(point,target);
        }

        /* Simulates the forward kinematics,
         * given a solution. */
        public Vector3 ForwardKinematics(float[] m_Solution)
        {
            Vector3 prevPoint = new Vector3(JointSim[0].x, JointSim[0].y, JointSim[0].z);
            Quaternion rotation = Quaternion.identity;

            // Takes object initial rotation into account
            //Quaternion rotation = JointSim[0];

            for (int i = 1; i < Joints.Length; i++)
            {
                // Rotates around a new axis
                rotation *= Quaternion.AngleAxis(m_Solution[i - 1], Joints[i - 1].Axis);
                Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;
                //Debug.Log(rotation);

                prevPoint = nextPoint;
            }

            // The end of the effector
            return new Vector3(prevPoint.x, prevPoint.y,prevPoint.z);
        }

        public void UpdateJointPosition(float[] m_Solution)
        {
            var newJoints = FKSimBuild(JointSim, m_Solution);
            for (int i = 0; i < newJoints.Length; i++)
            {
                JointSim[i] = new Vector3(newJoints[i].x, newJoints[i].y, newJoints[i].z);
            }
        }

        public Vector3[] FKSimBuild(Vector3[] prevJoints, float[] newAngles)
        {
            var newJointSim = new Vector3[prevJoints.Length];
            Vector3 prevPoint = new Vector3(prevJoints[0].x, prevJoints[0].y, prevJoints[0].z);
            Quaternion rotation = Quaternion.identity;

            // Takes object initial rotation into account
            //Quaternion rotation = prevJoints[0];

            newJointSim[0] = new Vector3(prevJoints[0].x, prevJoints[0].y, prevJoints[0].z);
            for (int i = 1; i < Joints.Length; i++)
            {
                // Rotates around a new axis
                rotation *= Quaternion.AngleAxis(newAngles[i - 1], Joints[i - 1].Axis);
                Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;

                prevPoint = new Vector3(nextPoint.x, nextPoint.y, nextPoint.z);
                newJointSim[i] = new Vector3(prevPoint.x, prevPoint.y,prevPoint.z);
            }

            return newJointSim;
        }
    }
}