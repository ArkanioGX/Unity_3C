using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float maxSpeed;

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
        Vector2 moveKeyInput = pInput.currentActionMap.FindAction("Move",false).ReadValue<Vector2>() ;
        Vector3 speed = new Vector3(rb.velocity.x + moveKeyInput.x, rb.velocity.y, rb.velocity.z + moveKeyInput.y);
        Vector3 fwd = new Vector3(cam.transform.forward.x,0,cam.transform.forward.z);
        rb.velocity = Vector3.Project(speed, fwd);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity,maxSpeed);
    }
}
