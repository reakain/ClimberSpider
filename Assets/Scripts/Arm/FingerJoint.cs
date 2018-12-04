﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class FingerJoint : RobotJoint
    {

        public FingerJoint ParentJoint { get; private set; }
        public FingerJoint ChildJoint { get; private set; }

        // Use this for initialization
        void Start()
        {
            ParentJoint = GetComponentInParent<FingerJoint>();
            ChildJoint = GetComponentInChildren<FingerJoint>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}