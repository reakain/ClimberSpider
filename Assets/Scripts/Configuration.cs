using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class Configuration
    {
        public Transform cPose { get; private set; }
        public Finger[] FingerList { get; private set; }

        public float Delta = 10;
        public float AngleDelta = 10;

        public Configuration(Hand Hand)
        {
            cPose = Hand.transform;
            //cPose.rotation = Hand.transform.rotation;
            FingerList = Hand.FingerList;
        }

        public Configuration(Vector3 position, Quaternion rotation, Finger[] fingerList)
        {
            cPose.SetPositionAndRotation(position, rotation);
            //cPose.position = position;
            //cPose.rotation = rotation;
            FingerList = fingerList;
        }

        public float Distance(Configuration c)
        {
            return Vector3.Distance(cPose.position, c.cPose.position);
        }

        public float Angle(Configuration c)
        {
            return Quaternion.Angle(cPose.rotation, c.cPose.rotation);
        }

        public Configuration MoveTowards(Configuration c)
        {
            var cNew = c;

            cNew.cPose.position = Vector3.MoveTowards(cPose.position, c.cPose.position, Delta);
            cNew.cPose.rotation = Quaternion.RotateTowards(cPose.rotation, c.cPose.rotation, AngleDelta);

            return cNew;
        }
    }
}