﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class WristSolver : IKSolver
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