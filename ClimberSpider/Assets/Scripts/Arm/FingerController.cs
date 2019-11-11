using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class FingerController : JointChainController
    {

        // Use this for initialization
        void Start()
        {
            Joints = GetComponentsInChildren<FingerJoint>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}