using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float maxSpeed;
    [SerializeField] float accel;

    [SerializeField] float groundDrag = 5f;
    [SerializeField] float airDrag = 0.5f;

    [SerializeField] Transform gCheck;

    private PlayerInput pInput;

    private Rigidbody rb;

    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pInput = GetComponent<PlayerInput>();
        cam = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0,cam.transform.rotation.eulerAngles.y,0);
        //Debug.Log(transform.rotation);

        if (Physics.Raycast(gCheck.position, -Vector3.up, 0.5f)){
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
        

        Vector2 moveKeyInput = pInput.currentActionMap.FindAction("Move",false).ReadValue<Vector2>() ;

        Vector3 fwdInput = cam.transform.forward * moveKeyInput.y ;
        Vector3 rsInput = cam.transform.right * moveKeyInput.x;

        Vector3 dir = fwdInput + rsInput;

        if (dir.magnitude > 0.001)
        {
            //Remove Y Value from dir to avoid fly
            dir = new Vector3(dir.x, 0, dir.z);

            Vector3 speed = dir * (accel * Time.deltaTime);
            rb.AddForce(speed, ForceMode.Impulse);
        }
        Debug.Log(rb.velocity.magnitude);
        if (rb.velocity.magnitude < 0.1) { rb.velocity = Vector3.zero; }
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }
}
