using UnityEngine;

namespace SpiderBot
{
    public class RobotJoint : MonoBehaviour
    {

        [Header("Joint Limits")]
        // A single 1, which is the axes of movement
        public Vector3 Axis;
        public float MinAngle;
        public float MaxAngle;

        [Header("Initial position")]
        // The offset at resting position
        [ReadOnly]
        public Vector3 StartOffset;

        // The initial one
        [ReadOnly]
        public Vector3 ZeroEuler;


        [Header("Movement")]
        // It lerps the speed to zero, from this distance
        [Range(0, 1f)]
        public float SlowdownThreshold = 0.5f;
        [Range(0, 360f)]
        public float Speed = 1f; // Degrees per second




        void Awake()
        {
            ZeroEuler = transform.localEulerAngles;
            StartOffset = transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {

        }


        // Try to move the angle by delta.
        // Returns the new angle.
        public float ClampAngle(float angle, float delta = 0)
        {
            return Mathf.Clamp(angle + delta, MinAngle, MaxAngle);
        }

        // Get the current angle
        public float GetAngle()
        {
            float angle = 0;
            if (Axis.x == 1) angle = transform.localEulerAngles.x;
            else
            if (Axis.y == 1) angle = transform.localEulerAngles.y;
            else
            if (Axis.z == 1) angle = transform.localEulerAngles.z;

            return ClampAngle(angle);
        }
        public float SetAngle(float angle)
        {
            angle = ClampAngle(angle);
            if (Axis.x == 1) transform.localEulerAngles = new Vector3(angle, 0, 0);
            else
            if (Axis.y == 1) transform.localEulerAngles = new Vector3(0, angle, 0);
            else
            if (Axis.z == 1) transform.localEulerAngles = new Vector3(0, 0, angle);
            Debug.Log("Moved to axis " + Axis + " to angle " + angle);
            return angle;
        }



        // Moves the angle to reach 
        public float MoveArm(float angle)
        {
            Debug.Log("Try to move " + this.name);
            return SetAngle(angle);
        }

        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, transform.parent.position, Color.red);
        }
    }
}
