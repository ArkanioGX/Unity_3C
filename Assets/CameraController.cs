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

    public bool useCPoints;
    private CameraPoint CurrentCamPoint;

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

    public List<CameraPoint> cpList;




    void Start()
    {
        pInput = target.GetComponent<PlayerInput>();
        cam = GetComponent<Camera>();
        swapCamViaID(currentCameraID);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        checkInput();


    }

    private void Update()
    {
        if (useCPoints) { swapCam(getCurrentData()); }
        CamUpdate();
        if (tTime != 1) { transitionUpdate(); }
    }

    private void checkInput()
    {
        lookVectorInput = pInput.currentActionMap.FindAction("Look").ReadValue<Vector2>();
        lookVectorInput = cData.invertedLook ? lookVectorInput * -1 : lookVectorInput; //Invert input
    }

    private CameraData getCurrentData()
    {
        if (cpList.Count > 0)
        {
            CameraPoint currentCP = null;
            foreach (CameraPoint cp in cpList)
            {
                if (currentCP == null || Vector3.Distance(cp.transform.position,target.transform.position) < Vector3.Distance(currentCP.transform.position, target.transform.position))
                {
                    currentCP = cp;
                }
            }
            CurrentCamPoint = currentCP;
            return currentCP.data.getcData(); 
        }
        else
        {
            return currentCCD.getcData();
        }
    }
   

    public void CameraChange(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            currentCameraID = (currentCameraID + 1) % CamDatas.Length;
            swapCamViaID(currentCameraID) ;
            
        }
    }

    void swapCamViaID(int id)
    {
        oldCData = cData;
        currentCCD = CamDatas[id];
        swapCam(currentCCD.getcData());
    }

    void swapCam(CameraData cDataT)
    {
        if (cDataT != cData)
        {
            cData = cDataT;
            if (target.GetComponent<PlayerController>() != null)
            {
                target.GetComponent<PlayerController>().rotateWithCam = cData.targetUseCamRotation;
                target.GetComponent<PlayerController>().setUseForwardCamera(cData.updateFwdWhenPressed);
            }
            cam.cullingMask = cData.cullingMask;

            if (cData.typeOfRotation == RotationType.UseMouse)
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
    }

    void CamUpdate()
    {
        Quaternion baseRot = transform.rotation;
        Quaternion nextRot = Quaternion.identity;
        //Rotation
        switch (cData.typeOfRotation)
        {
            case RotationType.Static:
                nextRot = Quaternion.Euler(cData.staticRotation);
                break;
            case RotationType.UseTransformRotation:
                nextRot = cData.transformToCopyRotation.rotation;
                break;
            case RotationType.LookTarget:
                Transform trgt = target.transform;
                switch (cData.targetToLookAt)
                {
                    case TargetType.Custom:
                        trgt = cData.lookTarget;
                        break;
                    default: break;
                }
                nextRot = Quaternion.LookRotation((trgt.position - transform.position).normalized);
                //transform.LookAt(trgt);
                break;
            case RotationType.UseMouse:
                Vector3 cRotation = lookRotation;
                Vector2 lvi = lookVectorInput * Time.deltaTime;
                cRotation = new Vector3(cRotation.x + (-lvi.y * (sensitivity * cData.sensitivityMultiplier)), cRotation.y + (lvi.x * (sensitivity * cData.sensitivityMultiplier)), cRotation.z);
                //cRotation *= Time.deltaTime;
                lookRotation = cRotation;
                nextRot = Quaternion.Euler(cRotation);
                break;
            default: break;
        }

        Vector3 nextRotEuler = nextRot.eulerAngles; //Lock

        nextRotEuler.x = cData.lockRotation.x ? baseRot.eulerAngles.x : nextRotEuler.x;
        nextRotEuler.y = cData.lockRotation.y ? baseRot.eulerAngles.y : nextRotEuler.y;
        nextRotEuler.x = cData.lockRotation.z ? baseRot.eulerAngles.z : nextRotEuler.z;

        nextRot = Quaternion.Euler(nextRotEuler);
        transform.rotation = nextRot;
        //Clamp Rotation

        //Position
        switch (cData.typeOfPosition)
        {
            case PositionType.Static:
                transform.position = cData.staticPosition;
                break;
            case PositionType.FollowTransform:
                Transform trgt = target.transform;
                if (cData.targetToBeAt == TargetType.Custom)
                {
                    trgt = cData.transformToCopyPosition;
                }
                transform.position = Vector3.SmoothDamp(transform.position, trgt.transform.position + cData.positionOffset, ref vel, cData.smoothTimePosition);
                break;
            case PositionType.RotateAroundTarget:
                Vector3 tpos = target.transform.position + cData.positionOffset;
                Vector3 rPos = (transform.localRotation * Vector3.back) * cData.CamDistance;
                transform.position = Vector3.SmoothDamp(transform.position, tpos + rPos, ref vel, cData.smoothTimePosition);
                break;
            default : break;

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

    public void AddCamPointTrigger(CameraPoint cps)
    {
        cpList.Add(cps);
    }

    public void RemoveCamPointTrigger(CameraPoint cps)
    {
        while (cpList.Contains(cps))
        {
            cpList.Remove(cps);
        }
    }
}
