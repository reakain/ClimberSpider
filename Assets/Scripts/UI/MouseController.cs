using UnityEngine;

namespace SpiderBot
{
    public class MouseController : MonoBehaviour
    {
        [Header("Movement Tuning")]
        public float mouseSensitivity = 100.0f;
        public float clampAngle = 80.0f;
        public float speed = 5.0f;

        private float rotY = 0.0f; // rotation around the up/y axis
        private float rotX = 0.0f; // rotation around the right/x axis

        private ArmPlanner m_SelectedArm;

        void Start()
        {
            Vector3 rot = transform.localRotation.eulerAngles;
            rotY = rot.y;
            rotX = rot.x;
        }
        void MouseClickEvent()
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // the object identified by hit.transform was clicked
                // do whatever you want
                //LeftControlRobot.Destination = hit.transform;
                if (hit.transform.GetComponentInParent<ArmPlanner>() != null)
                {
                    m_SelectedArm = hit.transform.GetComponentInParent<ArmPlanner>();
                    //Debug.Log("Selected Arm: " + hit.transform.name);
                }
                else if (m_SelectedArm != null && hit.transform.GetComponent<GraspRegion>() != null)
                {
                    m_SelectedArm.SetNewObject(hit.transform.GetComponent<GraspRegion>());
                    //Debug.Log("Selected Object: " + hit.transform.name);
                }
            }
        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                MouseClickEvent();
            }
            else if (Input.GetMouseButton(1))
            {
                MouseClickEvent();
            }

                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = -Input.GetAxis("Mouse Y");

                rotY += mouseX * mouseSensitivity * Time.deltaTime;
                rotX += mouseY * mouseSensitivity * Time.deltaTime;

                rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

                Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
                transform.rotation = localRotation;

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
                }
            
        }
    }
}
