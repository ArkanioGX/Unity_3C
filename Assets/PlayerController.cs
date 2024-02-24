using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float maxSpeed;
    [SerializeField] float accel;

    [SerializeField] float groundDrag = 5f;
    [SerializeField] float airDrag = 0.5f;

    [SerializeField] float angleSmooth = 0.1f;

    [SerializeField] Transform gCheck;

    private PlayerInput pInput;

    private Rigidbody rb;

    private float angleVel = 0;

    Vector2 moveKeyInput;
    Vector3 camForward, camRight;

    [HideInInspector]
    public bool rotateWithCam;
    [HideInInspector]
    public bool updateFwd;

    private Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pInput = GetComponent<PlayerInput>();
        cam = Camera.main;
        InputAction IAMove = pInput.currentActionMap.FindAction("Move", false);
        IAMove.started += context => { changeForward(); };
    }

    void changeForward()
    {
        camForward = Camera.main.transform.forward;
        camRight = Camera.main.transform.right;
    }

    private void Update()
    {
        InputAction IAMove = pInput.currentActionMap.FindAction("Move", false);
        moveKeyInput = IAMove.ReadValue<Vector2>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (updateFwd) { changeForward(); }
        Vector3 fwdInput = camForward * moveKeyInput.y;
        Vector3 rsInput = camRight * moveKeyInput.x;
        Vector3 dir = fwdInput + rsInput;

        if (rotateWithCam)
        {
            transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);
        }
        else
        {
            if (dir.magnitude > 0.001)
            {

                Quaternion velRot = Quaternion.LookRotation(dir);
                //transform.rotation = Quaternion.Euler(0, velRot.eulerAngles.y , 0);
                float moveRot = Mathf.SmoothDampAngle(rb.rotation.eulerAngles.y, velRot.eulerAngles.y , ref angleVel, angleSmooth);
                rb.MoveRotation(Quaternion.Euler(0, moveRot, 0));
            }
        }
        //Debug.Log(transform.rotation);

        if (Physics.Raycast(gCheck.position, -Vector3.up, 0.5f)){
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
        




        if (dir.magnitude > 0.1)
        {
            //Remove Y Value from dir to avoid fly
            dir = new Vector3(dir.x, 0, dir.z).normalized;

            Vector3 speed = dir * (accel * Time.fixedDeltaTime);
            rb.MovePosition(transform.position + speed);
        }
        if (rb.velocity.magnitude < 0.1) { rb.velocity = Vector3.zero; }
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CamPoints"))
        {
            cam.GetComponent<CameraController>().AddCamPointTrigger(other.GetComponent<CameraPoint>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CamPoints"))
        {
            cam.GetComponent<CameraController>().RemoveCamPointTrigger(other.GetComponent<CameraPoint>());
        }
    }
}
