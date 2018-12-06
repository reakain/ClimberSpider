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
        // Use this for initialization
        void Start()
        {
            Joints = GetComponentsInChildren<FingerJoint>();
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

            float[] stepPath = GetStartingAngles();
            string debugPrint = "";
            for (var i = 0; i < stepPath.Length; i++)
            {
                debugPrint += stepPath[i] + "\n";
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

            stepPath = ApproachTarget(target, stepPath);
            debugPrint = "";
            for (var i = 0; i < stepPath.Length; i++)
            {
                debugPrint += stepPath[i] + "\n";
            }
            Debug.Log("After: " + debugPrint);
            // Call controller move step
            for (int i = 0; i < Joints.Length - 1; i++)
            {
                Joints[i].MoveArm(stepPath[i]);
            }
            if (!(ErrorFunction(target, stepPath) > StopThreshold))
                CurlIn = false;
            // Check if collision
            //CurlIn = false;
        }

        void CurlAwayFromCenter()
        {
            if (!CurlOut)
                return;

            float[] stepPath = GetStartingAngles();
            PositionRotation target = new PositionRotation(GetComponentInParent<Wrist>().transform.position, GetComponentInParent<Wrist>().transform.rotation);
            target -= Vector3.one * Vector3.Distance(transform.position, target)*2;
            InitializeJointSim();
            stepPath = ApproachTarget(target, stepPath);
            for (int i = 0; i < Joints.Length - 1; i++)
            {
                Joints[i].MoveArm(stepPath[i]);
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