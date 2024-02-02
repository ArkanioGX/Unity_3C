using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    [SerializeField]
    private int currentCameraID = 0;

    [Serializable]
    class CameraData
    {
        [Range(10f, 180f)]
        public float FOV = 90;

        public Vector3 offset;

        public float sensitivity;

        public float smoothTime;

        public Vector2 clampXRot;

        public bool rotateAroundTarget = false;

        public bool targetUseCamRot = true;

        public LayerMask cullingMask;
    }

    Camera cam;

    [SerializeField] private CameraData[] CamDatas = new CameraData[0];
    private CameraData cData;


    private Vector3 vel;

    private PlayerInput pInput;

    private Vector2 lookVectorInput;

    private Vector3 lookRotation = new Vector3();




    void Start()
    {
        pInput = target.GetComponent<PlayerInput>();
        cam = GetComponent<Camera>();
        swapCamID(currentCameraID);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        checkInput();
        CamUpdate();
    }

    private void checkInput()
    {
        lookVectorInput = pInput.currentActionMap.FindAction("Look").ReadValue<Vector2>();
    }

    public void CameraChange(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            currentCameraID = (currentCameraID + 1) % CamDatas.Length;
            swapCamID(currentCameraID) ;
            
        }
    }

    void swapCamID(int id)
    {
        cData = CamDatas[id];
        cam.fieldOfView = cData.FOV;
        if (target.GetComponent<PlayerController>() != null)
        {
            target.GetComponent<PlayerController>().rotateWithCam = cData.targetUseCamRot;
        }
        cam.cullingMask = cData.cullingMask;
        
        
    }
    void CamUpdate()
    {
        //Position
        transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + cData.offset, ref vel, cData.smoothTime);

        //Rotation
        Vector3 cRotation = lookRotation;
        cRotation =new Vector3(cRotation.x + (-lookVectorInput.y * cData.sensitivity), cRotation.y + (lookVectorInput.x * cData.sensitivity), cRotation.z);
        cRotation = new Vector3(
            Mathf.Clamp(cRotation.x,cData.clampXRot.x, cData.clampXRot.y),
            cRotation.y,
            cRotation.z);
        lookRotation = cRotation;
        transform.localRotation = Quaternion.Euler(cRotation);
    }
}
