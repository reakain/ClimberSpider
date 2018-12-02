using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class ArmPlanner : MonoBehaviour
    {
        [Header("Grabber")]
        public Hand HandObject;
        [Header("Destination")]
        public GraspRegion Destination;
        [Space]

        private float Delta = 0;
        private float angleDelta = 0;


        private Tree ArmTree = new Tree();
        private Tree GoalTree = new Tree();
        private SolutionList SolutionPathList = new SolutionList();

        [HideInInspector]
        public bool doSearch;

        // Use this for initialization
        void Start()
        {
            Delta = Toolbox.Instance.GetConnectionDistance();
            angleDelta = Toolbox.Instance.GetConnectionDistance();
            var startConfig = GetStartConfiguration();
            ArmTree.Add(new Node(startConfig, null, true));
        }

        // Update is called once per frame
        void Update()
        {
            if (Destination == null)
                return;
            if (Destination.HandObject == null)
            {
                Destination.HandObject = HandObject;
            }
            if (Destination.GoalTree == null)
                return;

            GoalTree = Destination.GoalTree;
            ExpandTree();
        }

        public Configuration GetStartConfiguration()
        {
            Configuration startConfig = new Configuration(HandObject);
            return startConfig;
        }
        /*public void GraspRRT(Transform qStart, Transform po)
        {
            RRT.AddConfiguration(qstart);
            while ()
        }
        */
        public void RRTSearch()
        {
            while (doSearch)
            {
                var p = Random.value;
                if (p < 0.2) // Expand the tree
                {
                    ExpandTree();
                }
                else // Find a goal
                {
                    FindGoal();
                }
            }
        }

        public void ExpandTree()
        {
            var node = ArmTree[Random.Range(0, ArmTree.Count - 1)];
            //for (var i = 0; i < ArmTree.Count; i++)
            //{
                //var node = ArmTree[i];
                var cFree = SampleFreeSpace(node.Point);

                // Check for collisions
                if (IsCollisionFree(cFree))
                {
                    var cClose = node.Point.MoveTowards(cFree);
                    var newNode = new Node(cClose, node);

                    ArmTree.Add(newNode);
                    Debug.Log("Added node: " + cClose.transform);

                    if (SolutionPathList.AddSolutionIfExists(newNode, GoalTree))
                    {
                        Debug.Log("Found a solution!");
                        Debug.Log("Solution: " + SolutionPathList[0]);
                        GetComponent<InverseKinematics>().UpdateSolutionList(SolutionPathList);
                    }
                }
            //}
        }

        public bool IsCollisionFree(Configuration c)
        {
            return true;
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
            return new Configuration(new Hand());
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
