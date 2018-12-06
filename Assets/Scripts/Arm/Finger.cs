using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class Finger : IKSolver
    {

        //public FingerJoint[] JointList { get; private set; }

        private bool CurlIn = false;
        private bool CurlOut = false;
        private float[] curlSolution = null;
        // Use this for initialization
        void Start()
        {
            Joints = GetComponentsInChildren<FingerJoint>();
            curlSolution = new float[Joints.Length];
        }

        // Update is called once per frame
        void Update()
        {
            CurlTowardsCenter();
            CurlAwayFromCenter();
        }

        void CurlTowardsCenter()
        {
            if (!CurlIn)
                return;

            //float[] stepPath = GetStartingAngles();
            string debugPrint = "";
            for (var i = 0; i < curlSolution.Length; i++)
            {
                debugPrint += curlSolution[i] + "\n";
            }
            Debug.Log("Before: " + debugPrint);
            PositionRotation target = new PositionRotation(GetComponentInParent<Wrist>().transform.position, GetComponentInParent<Wrist>().transform.rotation);
            //Debug.Log(target);
            InitializeJointSim();
            debugPrint = "";
            for (var i = 0; i < JointSim.Length; i++)
            {
                debugPrint += JointSim[i] + "\n";
            }
            Debug.Log("JointSim: " + debugPrint);

            curlSolution = ApproachTarget(target, curlSolution);
            debugPrint = "";
            for (var i = 0; i < curlSolution.Length; i++)
            {
                debugPrint += curlSolution[i] + "\n";
            }
            Debug.Log("After: " + debugPrint);
            // Call controller move step
            for (int i = 0; i < Joints.Length - 1; i++)
            {
                Joints[i].MoveArm(curlSolution[i]);
            }
            if (!(ErrorFunction(target, curlSolution) > StopThreshold))
                CurlIn = false;
            // Check if collision
            //CurlIn = false;
        }

        void CurlAwayFromCenter()
        {
            if (!CurlOut)
                return;

            PositionRotation target = new PositionRotation(GetComponentInParent<Wrist>().transform.position, GetComponentInParent<Wrist>().transform.rotation);
            target -= Vector3.one * Vector3.Distance(transform.position, target)*2;
            InitializeJointSim();
            curlSolution = ApproachTarget(target, curlSolution);
            for (int i = 0; i < Joints.Length - 1; i++)
            {
                Joints[i].MoveArm(curlSolution[i]);
            }

            // Check if collision
            //CurlOut = false;
        }

        public void StartClose()
        {
            CurlIn = true;
            CurlOut = false;
        }

        public void StartOpen()
        {
            CurlOut = true;
            CurlIn = false;
        }
    }
}