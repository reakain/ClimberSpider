using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class Wrist : JointChainManager
    {
        public Finger[] FingerList { get; private set; }
        // Use this for initialization
        void Start()
        {
            Joints = GetComponentsInChildren<WristJoint>();
            ikSolver = GetComponent<WristSolver>();
            motionController = GetComponent<WristController>();
            FingerList = GetComponentsInChildren<Finger>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.G))
            {
                CloseFingers();
            }
            else if (Input.GetKey(KeyCode.O))
            {
                OpenFingers();
            }
        }

        public void CloseFingers()
        {
            foreach (var finger in FingerList)
            {
                finger.StartClose();
            }
        }

        public void OpenFingers()
        {
            foreach (var finger in FingerList)
            {
                finger.StartOpen();
            }
        }
    }
}
