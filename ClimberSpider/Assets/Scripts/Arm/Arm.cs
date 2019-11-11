using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class Arm : JointChainManager
    {
        // Use this for initialization
        void Start()
        {
            Joints = GetComponentsInChildren<ArmJoint>();
            ikSolver = GetComponent<ArmSolver>();
            motionController = GetComponent<ArmController>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
