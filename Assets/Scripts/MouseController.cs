using UnityEngine;

namespace SpiderBot
{
    public class MouseController : MonoBehaviour
    {

        [Header("Robot Selection")]
        public InverseKinematics LeftControlRobot;
        public InverseKinematics RightControlRobot;

        [Header("Movement Tuning")]
        public float mouseSensitivity = 100.0f;
        public float clampAngle = 80.0f;
        public float speed = 5.0f;

        private float rotY = 0.0f; // rotation around the up/y axis
        private float rotX = 0.0f; // rotation around the right/x axis

        void Start()
        {
            Vector3 rot = transform.localRotation.eulerAngles;
            rotY = rot.y;
            rotX = rot.x;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Debug.Log(Input.mousePosition);
                Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // the object identified by hit.transform was clicked
                    // do whatever you want
                    //LeftControlRobot.Destination = hit.transform;
                    if (LeftControlRobot == null)
                        LeftControlRobot = hit.transform.GetComponent<InverseKinematics>();
                    else
                        LeftControlRobot.Destination = hit.transform;
                }
            }
            else if (Input.GetMouseButton(1))
            {
                Debug.Log(Input.mousePosition);
                Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // the object identified by hit.transform was clicked
                    // do whatever you want
                    //LeftControlRobot.Destination = hit.transform;
                    if (RightControlRobot == null)
                        RightControlRobot = hit.transform.GetComponent<InverseKinematics>();
                    else
                        RightControlRobot.Destination = hit.transform;
                }
            }
            else
            {
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
}
