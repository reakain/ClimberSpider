using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot
{
    public class FingerPad : MonoBehaviour
    {
        private FingerJoint joint = null;
        // Use this for initialization
        void Start()
        {
            joint = GetComponentInParent<FingerJoint>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public PositionRotation JointPositionFromPad(VertexNormal DesiredPadPosition)
        {
            var pos = transform.localPosition;
            //var rot = joint.transform.rotation;
            var jpos = DesiredPadPosition.vertex - transform.localPosition;
            var jrot = transform.rotation;
            jrot *= Quaternion.FromToRotation(transform.position, -DesiredPadPosition.normal);
            jrot *= Quaternion.FromToRotation(transform.position, joint.transform.position);
            //var jrot = rot * jrot; 

            return new PositionRotation(jpos, jrot);
        }
    }
}