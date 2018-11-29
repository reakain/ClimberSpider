using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class ArmPlanner : MonoBehaviour
    {



        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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
                    camp = cspaceUniformSampling();
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
                indTree = whichTreetoConnect();
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
            /*
            Vector3 handPos = sampleHandPosition();
            Tobject_hand = computeTransformation(handPos);
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
                */
        }

        void VerifyGoal()
        {
            /*
             * regions = ICR(Toject_hand);
             * if (regions.length >= 2)
             * {
             *      ICRquality = computeICRqual();
             *      if (ICRquality >= minICRquality)
             *      {
             *          findCollfreeHandConfig(Toject_hands, regions);
             *          return true;
             *      }
             *      else
             *          return false;
             * }
             * else
             *      return false;
             */
        }
    }
}
