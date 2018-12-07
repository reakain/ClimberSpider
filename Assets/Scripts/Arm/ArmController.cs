using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    //[ExecuteInEditMode]
    public class ArmController : JointChainController
    {
        void Start()
        {
            Joints = GetComponentsInChildren<ArmJoint>();

        }

        private new void Update()
        {
            base.Update();
        }
    }
}