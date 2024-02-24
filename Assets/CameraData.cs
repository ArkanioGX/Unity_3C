using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Camera Data", menuName = "Create Camera Data", order = 1)]
public class CameraDataScriptableObject : ScriptableObject
{
    public CameraData cData;
}

[Serializable]
public class CameraData
{
    

    [Range(10f, 180f)]
    public float FOV = 90;

    public Vector3 offset;



    [Min(0f)]
    public float sensitivityMultiplier = 1;

    public float smoothTime = 0;

    public Vector2 clampXRot;

    public bool rotateAroundTarget = false;
    public bool lookAtTarget = false;
    [Tooltip("Available only for cam points")]
    public bool useCustomPosition = false;
    [Tooltip("Available only for cam points")]
    public bool useCustomRotation = false;

    public bool targetUseCamRot = true;
    public bool updateFwdWhenPressed = true;
    public float CamDistance;

    public bool invertedLook = false;

    public bool useMouse = true;

    public LayerMask cullingMask;
}

[Serializable]
public class CamControlData
{
    [Header("Camera Datas")]
    [Tooltip("If this value in null then use the value below")]
    public CameraDataScriptableObject CamDataSC;
    public CameraData CamData;

    public CameraData getcData()
    {
        return CamDataSC != null ? CamDataSC.cData : CamData;
    }
}