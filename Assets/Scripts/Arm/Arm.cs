using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class Arm : IKSolver
    {
        public ArmJoint[] JointList { get; private set; }

        // Use this for initialization
        void Start()
        {
            JointList = GetComponentsInChildren<ArmJoint>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}