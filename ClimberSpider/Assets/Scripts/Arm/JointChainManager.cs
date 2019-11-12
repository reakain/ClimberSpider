using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class JointChainManager : MonoBehaviour
    {
        public bool IsEndChain = false;
        public bool IsStartChain = false;
        protected JointChainManager ParentManager = null;
        protected JointChainManager ChildManager = null;
        protected GraspRegion Destination = null;

        private float Delta = 0;

        protected Tree ArmTree = null;
        protected Tree GoalTree = null;
        protected SolutionList SolutionPathList = null;

        protected RobotJoint[] Joints = null;
        protected IKSolverNew ikSolver = null;
        protected JointChainController motionController = null;

        public int JointNum = 0;

        protected bool doSearch = false;
        protected bool doMove = false;

        protected RobotJoint EndJoint = null;

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
            ikSolver = GetComponent<IKSolverNew>();
            motionController = GetComponent<JointChainController>();

            JointNum = Joints.Length;

            EndJoint = Joints[Joints.Length - 1];
            doSearch = false;
            doMove = false;
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
            if (EndJoint == null) { return; }
            
            if (IsEndChain) 
            { 
            EndChainUpdate();
            return;
            }
            if (IsStartChain)
            { 
            StartChainUpdate();
            return;
            }

            MidChainUpdate();
            

           
            
            if (doSearch)
            {
                RRTSearch();
            }
        }

        public void EndChainUpdate()
        {
            if (Destination == null) { return; }
            /*
            if (!Destination.IsConnected(this))
            {
                Destination.Connect(this);
            }
            */
            InitializeForwardTree();

            if (GoalTree.Count == 0) { return; }
        }

        public void StartChainUpdate()
        {
            if (ChildManager == null || ParentManager != null) { return; }

            InitializeForwardTree();

            if (GoalTree.Count == 0) { return; }
        }

        public void MidChainUpdate()
        {
            if (ChildManager == null || ParentManager == null) { return; }

            InitializeForwardTree();

            if (GoalTree.Count == 0) { return; }
        }

        public void InitializeForwardTree()
        {
            if (ArmTree.Count == 0)
            {
                var startSoln = new List<float[]>
                {
                    //GetComponent<IKSolver>().GetStartingAngles()
                    new float[Joints.Length]
                };
                /*
                var startNode = new Node(new Configuration(EndJoint), null);
                startNode.AddSolutionSteps(startSoln);
                startNode.Point.AddJointAngles(ikSolver.GetJointPose(startSoln));
                ArmTree.Add(startNode);
                Debug.Log("Got first point!");
                */
            }
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

        public void RRTSearch()
        {
            var p = Random.value;
            if (p < 0.5) // Expand forward tree
            {
                ExpandTree(ArmTree, GoalTree);
            }
            else // Expand backward tree
            {
                ExpandTree(GoalTree,ArmTree);
            }
        }

        public void ExpandTree(Tree expansionTree, Tree endTree)
        {
            // Get a free-ish space sample
            var newNode = expansionTree.SampleFreeSpace();
            int i = 0;
            while (newNode == null && i < 100)
            {
                newNode = expansionTree.SampleFreeSpace();
                i++;
            }
            if (newNode == null) { return; }

            // Check for collisions
            if (IsCollision(newNode.Point.transform)) { return; }

            //Debug.Log("First soln is: " + printsoln);
            //Debug.Log("Solution list is " + newNode.ParentNode.GetSolutionPath().Count + " steps long");

            var movePath = ikSolver.TestPath(newNode.ParentNode.GetSolutionPath(), newNode.ParentNode.Point.Joints, newNode.Point.transform);
            if (movePath == null) { return; }

            Debug.Log("Found a point");

            /* Debug Print Array Start */
            string printsoln = "";
            foreach (var solnstep in movePath)
            {
                printsoln += "( ";
                foreach (var joint in solnstep)
                {
                    printsoln += joint.ToString() + ", ";
                }
                printsoln += ")\n";
            }
            //Debug.Log("Move Path is: " + printsoln);
            /* Debug Print Array End */

            newNode.AddSolutionSteps(movePath);
            newNode.Point.AddJointAngles(ikSolver.GetJointPose(movePath));

            /* Debug Print Array Start */
            printsoln = "";
            foreach (var solnpoint in newNode.Point.Joints)
            {
                printsoln += solnpoint.ToString() + "\n";
            }
            //Debug.Log("Joint set is: " + printsoln);
            /* Debug Print Array End */

            expansionTree.Add(newNode.Clone());

            //Debug.Log("Added node: " + cClose.transform);
            var soln = SolutionPathList.AddSolutionIfExists(newNode, endTree);
            if (soln != null)
            {
                Debug.Log("Found a solution!");
                Debug.Log("Solution: " + soln.ToString());
                motionController.StartSolutionRun(soln);
            }
        }

        public bool IsCollision(Vector3 point)
        {
            foreach (var collider in FindObjectsOfType<Collider>())
            {
                if (collider.bounds.Contains(point))
                    return true;
            }
            return false;
        }

        public void StartSearch()
        {
            doSearch = true;
        }

        public void StopSearch()
        {
            doSearch = false;
        }

        public void StartMove()
        {
            doMove = true;
        }

        public void StopMove()
        {
            doMove = false;
        }
    }
}
