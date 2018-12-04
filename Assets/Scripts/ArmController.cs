using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    //[ExecuteInEditMode]
    public class ArmController : MonoBehaviour
    {
        public int MaximumSteps = 10;

        [Header("Joints")]
        //[HideInInspector]
        [ReadOnly]
        public ArmJoint[] Joints = null;
        // The current angles
        [ReadOnly]
        public float[] Solution = null;

        private SolutionList m_SolutionList;
        private Solution1 m_Solution;

        // Use this for initialization
        void Start()
        {
            if (Joints == null)
                GetJoints();
        }

        [ExposeInEditor(RuntimeOnly = false)]
        public void GetJoints()
        {
            Joints = GetComponentsInChildren<ArmJoint>();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_SolutionList == null)
                return;
            else if(m_Solution == null && Solution == null)
            {
                var bestSol = m_SolutionList.ShortestPath();
                //if (bestSol.Count <= MaximumSteps)
                //{
                    m_Solution = m_SolutionList.ShortestPath();
                    Solution = m_Solution.First.Value;
                    m_Solution.RemoveFirst();
                //}
            }
            for (int i = 0; i < Joints.Length - 1; i++)
            {
                Joints[i].MoveArm(Solution[i]);
            }
        }

        public void UpdateSolutionList(SolutionList solutionList)
        {
            m_SolutionList = solutionList;
        }
    }
}