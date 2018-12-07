using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class Configuration
    {
        public PositionRotation transform { get; private set; }
        public List<PositionRotation[]> FingerList { get; private set; }
        public PositionRotation[] Joints { get; private set; }

        public Configuration(Wrist Hand)
        {
            transform = new PositionRotation(Hand.transform.position, Hand.transform.rotation);
            FingerList = new List<PositionRotation[]>();
            foreach (var finger in Hand.FingerList)
            {
                var jointlist = new PositionRotation[finger.Joints.Length];
                for(int i = 0; i < finger.Joints.Length; i++)
                {
                    jointlist[i] = new PositionRotation(finger.Joints[i].transform.position, finger.Joints[i].transform.rotation);
                }
                FingerList.Add(jointlist);
            }
        }

        public Configuration(Vector3 position, Quaternion rotation, List<PositionRotation[]> fingerList)
        {
            transform = new PositionRotation(position, rotation);
            FingerList = new List<PositionRotation[]>();
            foreach (var finger in fingerList)
            {
                var jointlist = new PositionRotation[finger.Length];
                for (int i = 0; i < finger.Length; i++)
                {
                    jointlist[i] = new PositionRotation(finger[i], finger[i]);
                }
                FingerList.Add(jointlist);
            }
        }

        public Configuration(Vector3 position, Quaternion rotation, Finger[] fingerList)
        {
            transform = new PositionRotation(position, rotation);
            FingerList = new List<PositionRotation[]>();
            foreach (var finger in fingerList)
            {
                var jointlist = new PositionRotation[finger.Joints.Length];
                for (int i = 0; i < finger.Joints.Length; i++)
                {
                    jointlist[i] = new PositionRotation(finger.Joints[i].transform.position, finger.Joints[i].transform.rotation);
                }
                FingerList.Add(jointlist);
            }
        }

        public Configuration(Configuration c)
        {
            transform = new PositionRotation(c.transform, c.transform);
            FingerList = new List<PositionRotation[]>();
            foreach (var finger in c.FingerList)
            {
                var jointlist = new PositionRotation[finger.Length];
                for (int i = 0; i < finger.Length; i++)
                {
                    jointlist[i] = new PositionRotation(finger[i], finger[i]);
                }
                FingerList.Add(jointlist);
            }

            if (c.Joints != null)
            {
                AddJointAngles(c.Joints);
            }
        }

        public Configuration Clone()
        {
            return new Configuration(this);
        }

        public void AddJointAngles(PositionRotation[] joints)
        {
            Joints = new PositionRotation[joints.Length];
            for (int i = 0; i < joints.Length; i++)
            {
                Joints[i] = joints[i];
            }
        }

        public float Distance(Configuration c)
        {
            return Vector3.Distance(transform, c.transform);
        }

        public float Angle(Configuration c)
        {
            return Quaternion.Angle(transform, c.transform);
        }

        public Configuration MoveTowards(Configuration c)
        {
            var cNew = new Configuration(Vector3.MoveTowards(transform, c.transform, Toolbox.Instance.GetConnectionDistance()),
                Quaternion.RotateTowards(transform, c.transform, Toolbox.Instance.GetConnectionAngle()), c.FingerList);

            return cNew;
        }
    }
}