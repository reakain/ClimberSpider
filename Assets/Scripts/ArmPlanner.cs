using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class ArmPlanner : MonoBehaviour
    {
        [Header("Grabber")]
        public Wrist HandObject;
        [Header("Destination")]
        public GraspRegion Destination;
        [Space]

        private float Delta = 0;

        public Tree ArmTree { get; private set; }
        public Tree GoalTree { get; private set; }
        public SolutionList SolutionPathList { get; private set; }
        private Collider[] ColliderList;
        private Arm ikSolver;

        public bool doSearch { get; private set; }

        // Use this for initialization
        void Start()
        {
            // Initialize Trees and solutions
            ArmTree = new Tree();
            GoalTree = new Tree();
            SolutionPathList = new SolutionList();
            ColliderList = GetComponentsInChildren<Collider>();
            ikSolver = GetComponent<Arm>();

            // Initialize distance tolerances
            Delta = Toolbox.Instance.GetConnectionDistance();

            if (HandObject == null)
            {
                HandObject = GetComponentInChildren<Wrist>();
            }
            
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
                Destination.Connect(this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Destination == null || HandObject == null)
                return;
            if (!Destination.IsConnected(this))
            {
                Destination.Connect(this);
            }
            if (ArmTree.Count == 0)
            {
                var startSoln = new List<float[]>
                {
                    GetComponent<IKSolver>().GetStartingAngles()
                };

                var startNode = new Node(new Configuration(HandObject), null);
                startNode.AddSolutionSteps(startSoln);
                startNode.Point.AddJointAngles(ikSolver.GetJointAngles(startSoln));
                ArmTree.Add(startNode);
                Debug.Log("Got first point!");
            }
            if (GoalTree.Count == 0)
                return;

            if (doSearch)
            {
                RRTSearch();
            }
        }

        public void StartSearch()
        {
            doSearch = true;
        }

        public void AddGoalNode(Node goalPoint)
        {
            GoalTree.Add(goalPoint);
        }

        public void OnApplicationQuit()
        {
            if (Destination != null)
            {
                Destination.Disconnect();
                Destination = null;
            }
        }
        public void RRTSearch()
        {
            var p = Random.value;
            //if (p < 0.5) // Expand the tree
            {
                ExpandTree(ArmTree,GoalTree);
            }
            //else // Find a goal
            {
                //ExpandTree(GoalTree,ArmTree);
            }
        }

        public bool IsSelfCollision(Vector3 point)
        {
            foreach (var collider in ColliderList)
            {
                if (collider.bounds.Contains(point))
                    return true;
            }
            return false;
        }

        public void ExpandTree(Tree expansionTree, Tree endTree)
        {
            //var node = expansionTree[Random.Range(0, expansionTree.Count - 1)];
            //for (var i = 0; i < ArmTree.Count; i++)
            //{
            //var node = ArmTree[i];
            var newNode = expansionTree.SampleFreeSpace();
            while (newNode == null)
            {
                newNode = expansionTree.SampleFreeSpace();
            }

            if (!IsSelfCollision(newNode.Point.transform))
            {
                string printsoln = "";
                foreach (var soln in newNode.ParentNode.GetSolutionPath()[0])
                {
                    printsoln += soln.ToString() + "\n";
                }
                //Debug.Log("First soln is: " + printsoln);
                //Debug.Log("Solution list is " + newNode.ParentNode.GetSolutionPath().Count + " steps long");
                var parent = newNode.ParentNode;
                var movePath = ikSolver.TestPath(parent.GetSolutionPath(), parent.Point.Joints, newNode.Point.transform);
                if (movePath != null)
                {
                    Debug.Log("Found a point");
                    newNode.AddSolutionSteps(movePath);
                    newNode.Point.AddJointAngles(ikSolver.GetJointAngles(movePath));

                    // Check for collisions
                    if (IsCollisionFree(newNode.Point))
                    {
                        expansionTree.Add(newNode);

                        //Debug.Log("Added node: " + cClose.transform);
                        var soln = SolutionPathList.AddSolutionIfExists(newNode, endTree);
                        if (soln != null)
                        {
                            Debug.Log("Found a solution!");
                            //Debug.Log("Solution: " + soln);
                            GetComponent<ArmController>().UpdateSolutionList(SolutionPathList);
                        }
                    }
                }
            }
        }

        public bool IsCollisionFree(Configuration c)
        {
            var collisionFree = true;
            return collisionFree;
        }

        public Configuration SampleFreeSpace(Configuration c)
        {

            Vector3 randPos = c.transform;
            randPos += new Vector3(Random.Range(-Delta, Delta), Random.Range(-Delta, Delta), Random.Range(-Delta, Delta));
            Quaternion randRot = Random.rotationUniform;
            //Pose newPose = new Pose(randPos, randRot);


            Configuration cNew = new Configuration(randPos,randRot,c.FingerList);
            return cNew;
        }

        public void FindGoal()
        {

        }

        public Configuration SampleHandPosition()
        {
            return new Configuration(new Wrist());
        }

        /*
        // https://core.ac.uk/download/pdf/41776685.pdf
        public void StartSearch()
        {
            
            startTree = cinit;
            while (numgoaltrees == 0)
            {
                p = randomUniform(0, 1);
                if (p < 0.2)
                {
                    csmp = cspaceUniformSampling();
                    extendTree(startTree);
                }
                else
                {
                    cgoal = FindGoal();
                    if (cgoal != null)
                    {
                        new tree;
                        tree = cgoal;
                        listGoalTrees = updateGoalTrees(tree);
                    }
                }
            }
            p = randomUniform(0, 1);
            if (p < 0.8)
            {
                indTree = WhichTreetoConnect();
                if connectTrees(startTree, listGoalTrees[indTree])
                    {
                    return path to Grasp;
                }
            }
            else
            {
                cgoal = FindGoal();
                if (cgoal != null)
                {
                    new tree;
                    tree = cgoal;
                    listGoalTrees = updateGoalTrees(tree);
                }
            }
            

        }
        

        void FindGoal()
        {
            
            Vector3 handPos = SampleHandPosition();
            Pose Tobject_hand = ComputeTransformation(handPos);
            var cgoal = IK(Tobject_hand);
            if (cgoal != null)
            {
                if VerifyGoal(Tobject_hand)
                {
                    return cgoal;
                }
                else
                    return null;
            }
            else
                return null;
                
        }

        Vector3 SampleHandPosition()
        {
            // Get x,y,z from object pca
            return new Vector3() { x = 1, y = 1, z = 1 };
        }

        Pose ComputeTransformation(Vector3 position)
        {
            // Get T of object/hand from the position in samplehandposition
            return new Pose() {position = position, rotation = new Quaternion() };
        }

        void VerifyGoal(Pose T)
        {
            
             regions = ICR(T);
             if (regions.length >= 2)
             {
                  ICRquality = ComputeICRQuality();
                  if (ICRquality >= minICRquality)
                  {
                      findCollfreeHandConfig(T, regions);
                      return true;
                  }
                  else
                      return false;
             }
             else
                  return false;
             
        }

        void ICR(Pose T)
        {
            // Find regions that are useful?
        }

        void ComputeICRQuality()
        {
            int n; // number of fingers with contact regions
            int[] m; // number of discrete points in each region ICR_i
            //QICR = Math.Prod(m_i) from i=1 to n; 
        }

        void WhichTreeToConnect()
        {
            //probabilities_i = QICR_i / Math.Sum(QCIR);
        }

        void FindCollisionFreeHandConfig(Pose T, int regions)
        {
            // Iteratively explore hand positions that work with the region
        }
    */
    }
}
