using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class JointChainManager : MonoBehaviour
    {
        [Header("Destination")]
        public GraspRegion Destination = null;
        [Space]

        private float Delta = 0;

        protected Tree ArmTree { get; private set; }
        protected Tree GoalTree { get; private set; }
        protected SolutionList SolutionPathList { get; private set; }

        protected RobotJoint[] Joints;
        protected IKSolver ikSolver;
        protected JointChainController motionController;

        public int JointNum { get; private set; }

        protected bool doSearch { get; private set; }

        protected RobotJoint EndPoint = null;

        // Use this for initialization
        void Start()
        {
            // Initialize Trees and solutions
            ArmTree = new Tree();
            GoalTree = new Tree();
            SolutionPathList = new SolutionList();

            // Initialize distance tolerances
            Delta = Toolbox.Instance.GetConnectionDistance();

            Joints = GetComponentsInChildren<RobotJoint>();
            ikSolver = GetComponent<IKSolver>();
            motionController = GetComponent<JointChainController>();

            JointNum = Joints.Length;

            EndPoint = Joints[Joints.Length - 1];

            doSearch = true; // Placeholder until multiarm is built
        }

        public void SetNewObject(GraspRegion target)
        {
            if (Destination != null && Destination != target)
            {
                Destination.Disconnect();
                Destination = null;
            }
            if (Destination == null)
            {
                Destination = target;
                ArmTree = new Tree();
                GoalTree = new Tree();
                SolutionPathList = new SolutionList();
                //Destination.Connect(this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            /*
            if (Destination == null || EndPoint == null)
                return;
            if (!Destination.IsConnected(this))
            {
                Destination.Connect(this);
            }
            if (ArmTree.Count == 0)
            {
                var startSoln = new List<float[]>
                {
                    //GetComponent<IKSolver>().GetStartingAngles()
                    new float[Joints.Length]
                };

                var startNode = new Node(new Configuration(EndPoint), null);
                startNode.AddSolutionSteps(startSoln);
                startNode.Point.AddJointAngles(armManager.GetJointPose(startSoln));
                ArmTree.Add(startNode);
                Debug.Log("Got first point!");
            }
            if (GoalTree.Count == 0)
                return;

            if (doSearch)
            {
                RRTSearch();
            }
            */
        }

        public PositionRotation[] GetJointsPose()
        {
            var jointlist = new PositionRotation[JointNum];
            for (int i = 0; i < JointNum; i++)
            {
                jointlist[i] = new PositionRotation(Joints[i].transform.position, Joints[i].transform.rotation);
            }
            return jointlist;
        }
    }
}
