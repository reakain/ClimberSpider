using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class MultiArmPlanner : MonoBehaviour
    {

        public ArmPlanner[] Arms = null;
        public float[] Solution = null;

        private bool makingPlan = false;
        private bool newSolution = false;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void MultiRobotRRT()
        {
            foreach (var arm in Arms)
            {
                //arm.StartSearch();
            }

            while (makingPlan)
            {
                if (newSolution)
                {

                }
            }
        }
    }
}
