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

    [SerializeField]
    [Min(0f)]
    private float sensitivity = 1;

    [Serializable]
    class CamControlData
    {
        [Header("Use Cam Data from CamPoint ?")]
        public bool useCPoints;

        public CameraData CamData;
    }

    Camera cam;

    [SerializeField] private CamControlData[] CamDatas = new CamControlData[0];
    private CamControlData currentCCD; 
    private CameraData cData;

    [SerializeField]
    private float transitionTime = 0.2f;

    private float tTime = 1;

    private Vector3 vel;

    private PlayerInput pInput;

    private Vector2 lookVectorInput;

    private Vector3 lookRotation = new Vector3();

    private CameraData oldCData;




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
        if (currentCCD.useCPoints)
        {

        }
        else
        {
            CamUpdate();
            if (tTime != 1) { transitionUpdate(); }
        }
        
    }

    private void checkInput()
    {
        lookVectorInput = pInput.currentActionMap.FindAction("Look").ReadValue<Vector2>();
        lookVectorInput = cData.invertedLook ? lookVectorInput * -1 : lookVectorInput; //Invert input
        Debug.Log(lookVectorInput);
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
        oldCData = cData;
        currentCCD = CamDatas[id];
        cData = currentCCD.CamData;
        
        if (target.GetComponent<PlayerController>() != null)
        {
            target.GetComponent<PlayerController>().rotateWithCam = cData.targetUseCamRot;
            target.GetComponent<PlayerController>().updateFwd = cData.updateFwdWhenPressed;
        }
        cam.cullingMask = cData.cullingMask;

        if (cData.useMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
        tTime = 0;
    }
    void CamUpdate()
    {
        
        //Rotation
        Vector3 cRotation = lookRotation;
        cRotation =new Vector3(cRotation.x + (-lookVectorInput.y * (sensitivity * cData.sensitivityMultiplier)), cRotation.y + (lookVectorInput.x * (sensitivity * cData.sensitivityMultiplier)), cRotation.z);
        cRotation = new Vector3(
            Mathf.Clamp(cRotation.x,cData.clampXRot.x, cData.clampXRot.y),
            cRotation.y,
            cRotation.z);
        lookRotation = cRotation;
        transform.localRotation = Quaternion.Euler(cRotation);

        //Position
        if (cData.rotateAroundTarget)
        {
            Vector3 tpos = target.transform.position + cData.offset;
            Vector3 rPos = (transform.localRotation * Vector3.back) * cData.CamDistance;
            transform.position = Vector3.SmoothDamp(transform.position, tpos + rPos, ref vel, cData.smoothTime); 
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + cData.offset, ref vel, cData.smoothTime);
        }
    }

    private void transitionUpdate()
    {
        if (oldCData != null)
        {
            tTime = Mathf.Clamp(tTime + Time.deltaTime / transitionTime, 0, 1);
            cam.fieldOfView = Mathf.Lerp(oldCData.FOV, cData.FOV, tTime);
        }
        else
        {
            tTime = 1;
        }
    }
}
