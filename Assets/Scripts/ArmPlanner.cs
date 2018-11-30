using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class ArmPlanner : MonoBehaviour
    {
        public Hand HandObject;
        public bool doSearch;
        public float Delta;
        public float angleDelta;
        private Tree ArmTree;
        private Tree GoalTree;


        // Use this for initialization
        void Start()
        {
            ArmTree = new Tree();
            GoalTree = new Tree();
            var startConfig = GetStartConfiguration();
            ArmTree.Add(new Node(startConfig, null, true));
        }

        // Update is called once per frame
        void Update()
        {

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
                var p = Random.Range(0f,1f);
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
            var foundNext = false;
            while (!foundNext)
            {
                var cFree = SampleFreeSpace();
                var parentNode = ArmTree[0];
                foreach (var node in ArmTree)
                {
                    // Check if this node is closer than the last node
                    if (node.Point.Distance(cFree) <= parentNode.Point.Distance(cFree))
                    {
                        // Check for collisions
                        if (true)
                        {
                            foundNext = true;
                            parentNode = node;
                        }
                    }
                }

                // Move only a little bit
                var cClose = parentNode.Point.MoveTowards(cFree);
                ArmTree.Add(new Node(cClose, parentNode));

                if (IsReachedGoal())
                    {

                }
            }
        }

        public bool IsReachedGoal(bool forward = true)
        {
            var connectionFound = false;

            if (forward)
            {
                foreach (var node in GoalTree)
                {
                    if (node.Point.Distance(ArmTree[ArmTree.Count-1].Point) <= Delta)
                    {
                        return true;
                    }
                }
            }

            return connectionFound;
        }

        public Configuration SampleFreeSpace()
        {
            Configuration bluh = new Configuration(new Hand());
            return bluh;
        }

        // https://core.ac.uk/download/pdf/41776685.pdf
        public void StartSearch()
        {
            /*
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
            */

        }

        void FindGoal()
        {
            
            Vector3 handPos = SampleHandPosition();
            Pose Tobject_hand = ComputeTransformation(handPos);
            /*var cgoal = IK(Tobject_hand);
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
                */
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
            /*
             * regions = ICR(T);
             * if (regions.length >= 2)
             * {
             *      ICRquality = ComputeICRQuality();
             *      if (ICRquality >= minICRquality)
             *      {
             *          findCollfreeHandConfig(T, regions);
             *          return true;
             *      }
             *      else
             *          return false;
             * }
             * else
             *      return false;
             */
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
    }
}
