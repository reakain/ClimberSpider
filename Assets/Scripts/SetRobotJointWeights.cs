using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpiderBot.Arm.IK
{
    public class SetRobotJointWeights : MonoBehaviour
    {

        public InverseKinematics IK;

        public Vector2 MinAngle;
        public Vector2 MaxAngle;


        public Vector3[] Axes;

        [ExposeInEditor(RuntimeOnly = false)]
        public void ChangeAngles()
        {
            for (int i = 0; i < IK.Joints.Length; i++)
            {
                float t = (float)i / (IK.Joints.Length - 1);
                IK.Joints[i].MinAngle = Mathf.Lerp(MinAngle.x, MinAngle.y, t);
                IK.Joints[i].MaxAngle = Mathf.Lerp(MaxAngle.x, MaxAngle.y, t);

                IK.Joints[i].Axis = Axes[i % Axes.Length];
            }
        }
    }
}
