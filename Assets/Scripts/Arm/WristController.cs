﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class WristController : JointChainController
    {

        // Use this for initialization
        void Start()
        {
            Joints = GetComponentsInChildren<WristJoint>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}