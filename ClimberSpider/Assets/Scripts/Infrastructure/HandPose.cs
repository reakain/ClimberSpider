using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class HandPose
    {
        public PositionRotation transform { get; private set; }
        public List<PositionRotation[]> FingerList { get; private set; }

        public HandPose()
        {

        }

        public HandPose(GripPoints grip, Finger[] FingerList, List<int[]> PadContactList)
        {

        }
    }
}