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
        cData = cDataT;
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
        if (useCPoints && cData.useCustomRotation)
        {
            transform.position = CurrentCamPoint.CamPos.position;
        }
        else
        {
            if (cData.lookAtTarget == false)
            {
                Vector3 cRotation = lookRotation;
                Vector2 lvi = lookVectorInput * Time.deltaTime;
                Debug.Log(lvi);
                cRotation = new Vector3(cRotation.x + (-lvi.y * (sensitivity * cData.sensitivityMultiplier)), cRotation.y + (lvi.x * (sensitivity * cData.sensitivityMultiplier)), cRotation.z);
                cRotation = new Vector3(
                    Mathf.Clamp(cRotation.x, cData.clampXRot.x, cData.clampXRot.y),
                    cRotation.y,
                    cRotation.z);
                //cRotation *= Time.deltaTime;
                lookRotation = cRotation;
                transform.localRotation = Quaternion.Euler(cRotation);
            }
            else
            {
                transform.LookAt(target.transform);
            }
        }

        //Position
        if (useCPoints && cData.useCustomPosition)
        {
            transform.position = CurrentCamPoint.CamPos.position;
        }
        else
        {
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
