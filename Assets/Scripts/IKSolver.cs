using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    // A typical error function to minimise
    public delegate float ErrorFunction(PositionRotation target, float[] solution);

    //[ExecuteInEditMode]
    public class IKSolver : MonoBehaviour
    {
        public int MaximumSteps = 10;

        public int MaximumLoop = 100;

        [Header("Joints")]
        //[HideInInspector]
        [ReadOnly]
        public RobotJoint[] Joints = null;
        // The current angles
        [ReadOnly]
        public float[] Solution = null;

        [Header("Inverse Kinematics")]
        [Range(0, 1f)]
        public float DeltaGradient = 0.1f; // Used to simulate gradient (degrees)
        [Range(0, 100f)]
        public float LearningRate = 0.1f; // How much we move depending on the gradient

        [Space()]
        [Range(0, 0.25f)]
        public float StopThreshold = 0.1f; // If closer than this, it stops
        [Range(0, 10f)]
        public float SlowdownThreshold = 0.25f; // If closer than this, it linearly slows down

        public ErrorFunction ErrorFunction;

        private List<float[]> SolutionSteps = null;
        private List<PositionRotation[]> JointSteps = null;
        private PositionRotation[] JointSim = null;

        // Use this for initialization
        void Start()
        {
            if (Joints == null)
                GetJoints();

            ErrorFunction = DistanceFromTarget;
        }

        [ExposeInEditor(RuntimeOnly = false)]
        public void GetJoints()
        {
            Joints = GetComponentsInChildren<RobotJoint>();
            Solution = new float[Joints.Length];
        }

        public List<float[]> TestPath(Node startPoint, PositionRotation endPoint)
        {
            SolutionSteps = startPoint.SolutionSteps;

            JointSim = startPoint.Point.Joints;

            var target = endPoint;
            for (var i; i < MaximumLoop; i++)
            {
                if (ErrorFunction(target, Solution) > StopThreshold)
                {
                    if (ApprochTarget(target))
                    {
                        SolutionSteps.Add(Solution);
                        UpdateJointPosition();
                    }
                }
                else
                {
                    return SolutionSteps;
                }
            }
            return null;
        }

        public PositionRotation[] GetJointAngles(List<float[]> SolutionPath)
        {
            var jointStart = new PositionRotation[Joints.Length];
            for (var i = 0; i<Joints.Length; i++)
            {
                jointStart[i] = new PositionRotation(Joints[i].transform.position, Joints[i].transform.rotation);
            }
            for (var i = 0; i < SolutionPath.Count; i++)
            {
                jointStart = ForwardKinematics(jointStart,SolutionPath[i]);
            }
            return jointStart;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public float[] GetStartingAngles()
        {
            var startAngle = new float[Joints.Length];
            for (var i = Joints.Length - 1; i >= 0; i--)
                startAngle[i] = Joints[i].GetAngle();
            return startAngle;
        }

        public bool ApprochTarget(PositionRotation target)
        {
            // Starts from the end, up to the base
            // Starts from joints[end-2]
            //  so it skips the hand that doesn't move!
            for (int i = Joints.Length - 1; i >= 0; i--)
            //for (int i = 0; i < Joints.Length - 1 - 1; i++)
            {
                // FROM:    error:      [0, StopThreshold,  SlowdownThreshold]
                // TO:      slowdown:   [0, 0,              1                ]
                float error = ErrorFunction(target, Solution);
                float slowdown = Mathf.Clamp01((error - StopThreshold) / (SlowdownThreshold - StopThreshold));

                // Gradient descent
                float gradient = CalculateGradient(target, Solution, i, DeltaGradient);
                Solution[i] -= LearningRate * gradient * slowdown;
                // Clamp
                Solution[i] = Joints[i].ClampAngle(Solution[i]);

                // Early termination
                if (ErrorFunction(target, Solution) <= StopThreshold)
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
        public float CalculateGradient(PositionRotation target, float[] Solution, int i, float delta)
        {
            // Saves the angle,
            // it will be restored later
            float solutionAngle = Solution[i];

            // Gradient : [F(x+h) - F(x)] / h
            // Update   : Solution -= LearningRate * Gradient
            float f_x = ErrorFunction(target, Solution);

            Solution[i] += delta;
            float f_x_plus_h = ErrorFunction(target, Solution);

            float gradient = (f_x_plus_h - f_x) / delta;

            // Restores
            Solution[i] = solutionAngle;

            return gradient;
        }

        // Returns the distance from the target, given a solution
        public float DistanceFromTarget(PositionRotation target, float[] Solution)
        {
            PositionRotation point = ForwardKinematics(Solution);
            return Vector3.Distance(point, target);// + Quaternion.Angle(point,target);
        }


        /* Simulates the forward kinematics,
         * given a solution. */
        public PositionRotation ForwardKinematics(float[] Solution)
        {
            Vector3 prevPoint = JointSim[0];
            //Quaternion rotation = Quaternion.identity;

            // Takes object initial rotation into account
            Quaternion rotation = JointSim[0];

            for (int i = 1; i < Joints.Length; i++)
            {
                // Rotates around a new axis
                rotation *= Quaternion.AngleAxis(Solution[i - 1], Joints[i - 1].Axis);
                Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;

                if (DebugDraw)
                    Debug.DrawLine(prevPoint, nextPoint, Color.blue);

                prevPoint = nextPoint;
            }

            // The end of the effector
            return new PositionRotation(prevPoint, rotation);
        }

        public void UpdateJointPosition()
        {
            var newJointSim = new PositionRotation[JointSim.Length];

            Vector3 prevPoint = JointSim[0];
            //Quaternion rotation = Quaternion.identity;

            // Takes object initial rotation into account
            Quaternion rotation = JointSim[0];

            newJointSim[0] = new PositionRotation(prevPoint, rotation);
            for (int i = 1; i < JointSim.Length; i++)
            {
                // Rotates around a new axis
                rotation *= Quaternion.AngleAxis(Solution[i - 1], Joint[i - 1].Axis);
                Vector3 nextPoint = prevPoint + rotation * Joint[i].StartOffset;

                prevPoint = nextPoint;
                newJointSim[i] = new PositionRotation(prevPoint, rotation);
            }
            JointSim = newJointSim;
        }

        public PositionRotation[] ForwardKinematics(PositionRotation[] prevJoints, float[] newAngles)
        {
            var newJointSim = new PositionRotation[prevJoints.Length];

            Vector3 prevPoint = prevJoints[0];
            //Quaternion rotation = Quaternion.identity;

            // Takes object initial rotation into account
            Quaternion rotation = prevJoints[0];

            newJointSim[0] = new PositionRotation(prevPoint, rotation);
            for (int i = 1; i < prevJoints.Length; i++)
            {
                // Rotates around a new axis
                rotation *= Quaternion.AngleAxis(newAngles[i - 1], Joint[i - 1].Axis);
                Vector3 nextPoint = prevPoint + rotation * Joint[i].StartOffset;

                prevPoint = nextPoint;
                newJointSim[i] = new PositionRotation(prevPoint, rotation);
            }

            return newJointSim;
        }
    }
}
