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
            if (m_Solution == null || m_Solution.IsEmpty())
            {
                return;
            }
            //if(nextPoint == null)
            else
            {
                nextPoint = m_Solution.Pop();

                /* Debug Print Array Start */
                var printSoln = "( ";
                foreach (var joint in nextPoint)
                {
                    printSoln += joint.ToString() + ", ";
                }
                printSoln += ")\n";
                Debug.Log("Move Path is: " + printSoln);
                /* Debug Print Array End */

                Debug.Log("Solution path left is: " + m_Solution.ToString());
            }

            Debug.Log("Moving!");
            for (int i = 0; i < Joints.Length - 1; i++)
            {
                Joints[i].MoveArm(nextPoint[i]);
            }
            Debug.Log("Finished move!");
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

                    /* Debug Print Array Start */
                    string printsoln = "( ";
                    foreach (var joint in addStep)
                    {
                        printsoln += joint.ToString() + ", ";
                    }
                    printsoln += ")\n";
                    Debug.Log("Solution step is: " + printsoln);
                    /* Debug Print Array End */

                    m_Solution.AddLast(addStep);
                }
                Debug.Log("Solution path to move is: " + m_Solution.ToString());
            }
        }
    }
}