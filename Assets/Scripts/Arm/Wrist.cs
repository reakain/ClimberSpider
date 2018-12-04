using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class Wrist : IKSolver
    {
        public Finger[] FingerList { get; private set; }
        public WristJoint[] JointList { get; private set; }

        // Use this for initialization
        void Start()
        {
            FingerList = GetComponentsInChildren<Finger>();
            JointList = GetComponentsInChildren<WristJoint>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}