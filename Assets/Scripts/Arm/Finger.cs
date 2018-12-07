using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class Finger : JointChainManager
    {
        private bool CurlIn = false;
        private bool CurlOut = false;
        private float[] curlSolution = null;

        // Use this for initialization
        void Start()
        {
            Joints = GetComponentsInChildren<FingerJoint>();
            ikSolver = GetComponent<FingerSolver>();
            motionController = GetComponent<FingerController>();
            curlSolution = new float[Joints.Length];
        }

        // Update is called once per frame
        void Update()
        {
            CurlFinger();
        }

        void CurlFinger()
        {
            if (!CurlIn && !CurlOut) { return; }

            PositionRotation target = new PositionRotation(GetComponentInParent<WristSolver>().transform.position, GetComponentInParent<WristSolver>().transform.rotation);
            if (CurlOut)
            {
                target -= Vector3.one * Vector3.Distance(transform.position, target) * 2;
            }

            ikSolver.InitializeJointSim();

            var newSolution = ikSolver.ApproachTarget(target, curlSolution);
            for (int i = 0; i < newSolution.Length; i++)
            {
                curlSolution[i] = newSolution[i];
            }

            motionController.MoveJointChain(curlSolution);

            if (!(ikSolver.ErrorFunction(target, curlSolution) > ikSolver.StopThreshold))
            {
                CurlIn = false;
                CurlOut = false;
            }

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