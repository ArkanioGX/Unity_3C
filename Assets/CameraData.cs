using System;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.ComponentModel;
using static UnityEngine.GraphicsBuffer;
#if UNITY_EDITOR
using UnityEditor;
#endif


public enum RotationType { Static = 0, UseTransformRotation = 1, LookTarget = 2, UseMouse = 3 }
public enum PositionType { Static = 0, FollowTransform = 1, RotateAroundTarget = 2 }
public enum TargetType { camTarget = 0, Custom = 1}

[Serializable]
public class CameraData
{
    [HideInInspector]
    [Range(1f, 180f)]
    public float FOV = 90;

    public bool isOrthographic = false;

    //All
    public LayerMask cullingMask;

    //// Début Rotation 
    public RotationType typeOfRotation;

    // 0 : Static
    public Vector3 staticRotation;

    // 1 : UseTransformRotation
    public Transform transformToCopyRotation;

    // 2 : Look Target
    public TargetType targetToLookAt;

    // 2.1 :
    public Transform lookTarget;
    // 2.All :
    public Vector3 offsetTarget;

    // 3 : Use Mouse to Control
    [Min(0f)]
    public float sensitivityMultiplier = 1;

    public bool invertedLook = false;
    
    
    // All 
    public minMax clampXRotation;
    public minMax clampYRotation;
    public minMax clampZRotation;

    public lockType lockRotation;

    public float smoothTimeRotation = 0;

    public bool targetUseCamRotation;
    //// Fin Rotation 

    //// Début Position
    public PositionType typeOfPosition;

    // 0 : Static
    public Vector3 staticPosition;

    // 1 : Follow Transform
    public TargetType targetToBeAt;
    // 1:1 : Follow Transform
    public Transform transformToCopyPosition;

    // 2 : Rotate Around target

    public bool updateFwdWhenPressed = true;

    public float CamDistance;

    // All :
    public Vector3 positionOffset;
    public float smoothTimePosition = 0;

    public minMax clampXPosition;
    public minMax clampYPosition;
    public minMax clampZPosition;

    public lockType lockPosition;
    //// Fin Position


}

[Serializable]
public struct minMax
{
    public bool clamp;
    public float min;
    public float max;
}

[Serializable]
public struct lockType
{
    public bool x, y, z;
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

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CameraData))]
public class EditorCamData : PropertyDrawer
{
    SerializedProperty SP_FOV;
    SerializedProperty SP_Orthographic;
    SerializedProperty SP_cullingMask;
    SerializedProperty SP_Sens;
    SerializedProperty SP_typeOfRotation;
    SerializedProperty SP_staticRotation;
    SerializedProperty SP_transformToCopyRotation;
    SerializedProperty SP_targetToLookAt;
    SerializedProperty SP_lookTarget;
    SerializedProperty SP_offsetTarget;
    SerializedProperty SP_invertedLook;
    SerializedProperty SP_clampXRotation;
    SerializedProperty SP_clampYRotation;
    SerializedProperty SP_clampZRotation;
    SerializedProperty SP_lockRotation;
    SerializedProperty SP_smoothTimeRotation;
    SerializedProperty SP_targetUseCamRotation;
    SerializedProperty SP_typeOfPosition;
    SerializedProperty SP_staticPosition;
    SerializedProperty SP_targetToBeAt;
    SerializedProperty SP_transformToCopyPosition;
    SerializedProperty SP_updateFwdWhenPressed;
    SerializedProperty SP_CamDistance;
    SerializedProperty SP_positionOffset;
    SerializedProperty SP_smoothTimePosition;
    SerializedProperty SP_clampXPosition;
    SerializedProperty SP_clampYPosition;
    SerializedProperty SP_clampZPosition;
    SerializedProperty SP_lockPosition;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Get Property
        //EditorGUI.BeginProperty(position, GUIContent.none, property);
        SP_FOV = property.FindPropertyRelative("FOV");
        SP_Orthographic = property.FindPropertyRelative("isOrthographic");
        SP_cullingMask = property.FindPropertyRelative("cullingMask");
        SP_Sens = property.FindPropertyRelative("sensitivityMultiplier");
        SP_typeOfRotation = property.FindPropertyRelative("typeOfRotation");
        SP_staticRotation = property.FindPropertyRelative("staticRotation");
        SP_transformToCopyRotation = property.FindPropertyRelative("transformToCopyRotation");
        SP_targetToLookAt = property.FindPropertyRelative("targetToLookAt");
        SP_lookTarget = property.FindPropertyRelative("lookTarget");
        SP_offsetTarget = property.FindPropertyRelative("offsetTarget");
        SP_invertedLook = property.FindPropertyRelative("invertedLook");
        SP_clampXRotation = property.FindPropertyRelative("clampXRotation");
        SP_clampYRotation = property.FindPropertyRelative("clampYRotation");
        SP_clampZRotation = property.FindPropertyRelative("clampZRotation");
        SP_lockRotation = property.FindPropertyRelative("lockRotation");
        SP_smoothTimeRotation = property.FindPropertyRelative("smoothTimeRotation");
        SP_targetUseCamRotation = property.FindPropertyRelative("targetUseCamRotation");
        SP_typeOfPosition = property.FindPropertyRelative("typeOfPosition");
        SP_staticPosition = property.FindPropertyRelative("staticPosition");
        SP_targetToBeAt = property.FindPropertyRelative("targetToBeAt");
        SP_transformToCopyPosition = property.FindPropertyRelative("transformToCopyPosition");
        SP_updateFwdWhenPressed = property.FindPropertyRelative("updateFwdWhenPressed");
        SP_CamDistance = property.FindPropertyRelative("CamDistance");
        SP_positionOffset = property.FindPropertyRelative("positionOffset");
        SP_smoothTimePosition = property.FindPropertyRelative("smoothTimePosition");
        SP_clampXPosition = property.FindPropertyRelative("clampXPosition");
        SP_clampYPosition = property.FindPropertyRelative("clampYPosition");
        SP_clampZPosition = property.FindPropertyRelative("clampZPosition");
        SP_lockPosition = property.FindPropertyRelative("lockPosition");
        //UI Editor
        EditorGUILayout.PropertyField(SP_FOV);
        EditorGUILayout.PropertyField(SP_cullingMask);
        EditorGUILayout.PropertyField(SP_Orthographic);

        EditorGUILayout.Space(10);

        EditorGUILayout.PrefixLabel("-=-=- Rotation -=-=-");

        EditorGUILayout.PropertyField(SP_typeOfRotation);

        EditorGUILayout.Space(5);

        switch (SP_typeOfRotation.enumValueIndex)
        {
            case 0:
                EditorGUILayout.PropertyField(SP_staticRotation);
                break;
            case 1:
                EditorGUILayout.PropertyField(SP_transformToCopyRotation);
                break;
            case 2:
                EditorGUILayout.PropertyField(SP_targetToLookAt);
                switch (SP_targetToLookAt.enumValueIndex)
                {
                    case 1:
                        EditorGUILayout.PropertyField(SP_lookTarget);
                        break;
                }
                EditorGUILayout.PropertyField(SP_offsetTarget);
                break;
            case 3:
                EditorGUILayout.PropertyField(SP_Sens);
                EditorGUILayout.PropertyField(SP_invertedLook);
                break;
            default: break;
        }
        EditorGUILayout.PropertyField(SP_clampXRotation);
        EditorGUILayout.PropertyField(SP_clampYRotation);
        EditorGUILayout.PropertyField(SP_clampZRotation);

        EditorGUILayout.PropertyField(SP_lockRotation);
        EditorGUILayout.PropertyField(SP_smoothTimeRotation);
        EditorGUILayout.PropertyField(SP_targetUseCamRotation);

        EditorGUILayout.Space(10);

        EditorGUILayout.PrefixLabel("-=-=- Position -=-=-");

        EditorGUILayout.PropertyField(SP_typeOfPosition);

        EditorGUILayout.Space(5);

        switch (SP_typeOfPosition.enumValueIndex)
        {
            case 0:
                EditorGUILayout.PropertyField(SP_staticPosition);
                break;
            case 1:
                EditorGUILayout.PropertyField(SP_targetToBeAt);
                switch (SP_targetToBeAt.enumValueIndex)
                {
                    case 1:
                        EditorGUILayout.PropertyField(SP_transformToCopyPosition);
                        break;
                }
                break;
            case 2:
                EditorGUILayout.PropertyField(SP_updateFwdWhenPressed);
                EditorGUILayout.PropertyField(SP_CamDistance);
                break;

            default: break;
        }

        EditorGUILayout.PropertyField(SP_positionOffset);
        EditorGUILayout.PropertyField(SP_smoothTimePosition);

        EditorGUILayout.PropertyField(SP_clampXPosition);
        EditorGUILayout.PropertyField(SP_clampYPosition);
        EditorGUILayout.PropertyField(SP_clampZPosition);

        EditorGUILayout.PropertyField(SP_lockPosition);

        //property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(CameraData)); 
        //EditorGUI.EndProperty();


    }

}

#endif