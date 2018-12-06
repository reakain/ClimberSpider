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
        public float[] nextPoint = null;

        private SolutionList m_SolutionList = null;
        private Solution m_Solution = null;

        // Use this for initialization
        void Start()
        {
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
            if (m_Solution == null)
                return;
            else if(nextPoint == null)
            {
                //var bestSol = m_SolutionList.ShortestPath();
                //if (bestSol.Count <= MaximumSteps)
                //{
                nextPoint = m_Solution.Pop();
                    //m_Solution.RemoveFirst();
                //}
            }
            Debug.Log("Moving!");
            for (int i = 0; i < Joints.Length - 1; i++)
            {
                Joints[i].MoveArm(nextPoint[i]);
            }
            Debug.Log("Finished move!");
            if (m_Solution != null || !m_Solution.IsEmpty())
            {
                nextPoint = m_Solution.Pop();
                //m_Solution.RemoveFirst();
            }
        }

        public void StartSolutionRun(Solution solution)
        {
            if (m_Solution == null)
            {
                m_Solution = new Solution();

                //List<float[]> m_solutionSteps = new List<float[]>();
                for (int i = 0; i < solution.Count; i++)
                {
                    var steps = solution.Pop();
                    var addStep = new float[steps.Length];
                    for (int j = 0; j < steps.Length; j++)
                    {
                        addStep[j] = steps[j];
                    }
                    m_Solution.AddLast(addStep);
                }
            }
        }
    }
}