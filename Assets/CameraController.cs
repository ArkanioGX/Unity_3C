using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    
    [SerializeField] private GameObject target;

    [SerializeField] private float sensitivity;

    [SerializeField] private float delay;

    [SerializeField] private Vector3 Offset;

    [SerializeField] private Vector2 ClampXRot;

    private Vector3 vel;

    private PlayerInput pInput;

    private Vector2 lookVectorInput;

    private Vector3 lookRotation = new Vector3();

    public enum CameraType : int { FPS = 0, TPS = 1, Isometric = 2 };
    public CameraType CType = CameraType.FPS;



    void Start()
    {
        pInput = target.GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        checkInput();
        switch (CType)
        {
            case CameraType.FPS:
                FPSUpdate();
                break;
            case CameraType.TPS:
                TPSUpdate();
                break;
            case CameraType.Isometric:
                IsometricUpdate();
                break;
            default:
                break;
        }
    }

    private void checkInput()
    {
        lookVectorInput = pInput.currentActionMap.FindAction("Look").ReadValue<Vector2>();
    }

    public void CameraChange(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CType = (CameraType)(((int)CType + 1) % Enum.GetNames(typeof(CameraType)).Length);
        }
    }
    void FPSUpdate()
    {
        //Position
        transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + Offset, ref vel, delay);

        //Rotation
        Vector3 cRotation = lookRotation;
        cRotation =new Vector3(cRotation.x + (-lookVectorInput.y * sensitivity), cRotation.y + (lookVectorInput.x * sensitivity), cRotation.z);
        cRotation = new Vector3(
            Mathf.Clamp(cRotation.x,ClampXRot.x,ClampXRot.y),
            cRotation.y,
            cRotation.z);
        lookRotation = cRotation;
        transform.localRotation = Quaternion.Euler(cRotation);
    }

    void TPSUpdate()
    {
        
    }

    void IsometricUpdate()
    {

    }
}
