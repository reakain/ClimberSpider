using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class Configuration
    {
        public PositionRotation transform { get; private set; }
        public Finger[] FingerList { get; private set; }
        public ArmJoint[] Joints { get; private set; }

        public Configuration(Hand Hand)
        {
            transform = new PositionRotation(Hand.transform.position, Hand.transform.rotation);
            //transform.position = Hand.transform.position;
            //transform.rotation = Hand.transform.rotation;
            FingerList = Hand.FingerList;
        }

        public Configuration(Vector3 position, Quaternion rotation, Finger[] fingerList)
        {
            transform = new PositionRotation(position, rotation);
            //transform.position = position;
            //transform.rotation = rotation;
            FingerList = fingerList;
        }

        public void AddJointAngles(ArmJoint[] joints)
        {
            Joints = joints;
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

             
            //cNew.transform = Vector3.MoveTowards(transform, c.transform, Toolbox.Instance.GetConnectionDistance());
            //cNew.transform = Quaternion.RotateTowards(transform, c.transform, Toolbox.Instance.GetConnectionAngle());

            return cNew;
        }
    }
}