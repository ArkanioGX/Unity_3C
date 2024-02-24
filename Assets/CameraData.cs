using System;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.ComponentModel;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Camera Data", menuName = "Create Camera Data", order = 1)]
public class CameraDataScriptableObject : ScriptableObject
{
    public CameraData cData;
}
public enum RotationType { Static, UseTransformRotation, LookTarget, UseMouse }
public enum PositionType { Static, FollowTransform, BasedOnRotation }

[Serializable]
public class CameraData
{
    

    [Range(10f, 180f)]
    public float FOV = 90;
    [Min(0f)]
    public float sensitivityMultiplier = 1;

    [Space(10)]
    public bool useCustomPosition = false;
    public bool useCustomRotation = false;
    public CustomPosRot CustomPositionRot;
    [Space(10)]
    public Vector3 offset;

    

    public float smoothTime = 0;

    public Vector2 clampXRot;

    public bool rotateAroundTarget = false;
    public bool lookAtTarget = false;

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

[Serializable]
public class CustomPosRot
{
    public Transform customTransform;
    public Vector3 pos;
    public Quaternion rot;

    public Vector3 getPosition()
    {
        return customTransform != null ? customTransform.position : pos;
    }

    public Quaternion getRotation()
    {
        return customTransform != null ? customTransform.rotation : rot;
    }
}

/*#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CameraData))]
public class Editor_CamData : PropertyDrawer
{

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var amountField = new PropertyField(property.FindPropertyRelative("FOV"));
        var unitField = new PropertyField(property.FindPropertyRelative("sensitivityMultiplier"));

        // Add fields to the container.
        container.Add(amountField);
        if (property.FindPropertyRelative("FOV").floatValue < 30f)
        {
            container.Add(unitField);
        }

        return container;
    }
    /*
    const int UI_POSITION_INCREMENT = 20;
    SerializedProperty SP_FOV;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect UIPosition = new Rect(position.x,position.y,position.width,position.height);

        SerializedProperty SP_FOV = property.FindPropertyRelative("FOV");
        SerializedProperty SP_Sens = property.FindPropertyRelative("sensitivityMultiplier");

        EditorGUI.PropertyField(UIPosition, SP_FOV);
        if (SP_FOV.floatValue > 30f)
        {
            UIPosition.y += UI_POSITION_INCREMENT ;
            EditorGUI.PropertyField (UIPosition, SP_Sens);
        }
    }

    public void addToRect(Rect position) {
    
    }
}
#endif*/