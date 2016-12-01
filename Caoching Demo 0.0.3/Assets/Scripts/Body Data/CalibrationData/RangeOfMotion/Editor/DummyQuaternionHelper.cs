using UnityEngine;
using System.Collections;
using UnityEditor;
using Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion;
using Assets.Scripts.Body_Data.View;

[CustomEditor(typeof(DummyQuaternion))]
[CanEditMultipleObjects]
public class DummyQuaternionHelper : Editor
{
    SerializedProperty staticROM_ser;

    Color MainCol = Color.magenta;
    Color orthoCol = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    Color sphereCol = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    Color angleCol = Color.yellow;

    public bool isArm;
    public bool isRight;

    Quaternion qCustom = Quaternion.Euler(0,30,30);

    SerializedProperty drawOrthoProp;
    SerializedProperty AdditionalDebugProp;
    void OnSceneGUI()
    {
        DummyQuaternion segment = target as DummyQuaternion;
        bool drawOrtho = drawOrthoProp.boolValue;
        bool additionalDbg = AdditionalDebugProp.boolValue;

        float handleSize = HandleUtility.GetHandleSize(segment.transform.position)* (additionalDbg ? 2 : 1);
        float arrowSize = handleSize * 1.2f;
        float offsetSize = handleSize * 1.1f;
        float radiusSize = handleSize * 0.5f;
        Vector3 t_position = segment.transform.position + (additionalDbg ? Vector3.up * handleSize*2.2f : Vector3.zero );

        Handles.color = sphereCol;

        if(additionalDbg)
        {
            Handles.SphereCap(4, t_position, Quaternion.identity, arrowSize * 2);
            qCustom = Handles.RotationHandle(qCustom, t_position);
            Handles.RotationHandle(Quaternion.identity, t_position);
        }


        Quaternion tQuat = segment.transform.localRotation;
        float angle;
        Vector3 axis;
        tQuat.ToAngleAxis(out angle, out axis);
        Vector3 t_up;
        Vector3 t_orthoVec;
        if (Mathf.Sin(Vector3.Angle(axis, Vector3.up) * Mathf.Deg2Rad) > 0.1f)
        {
            t_orthoVec = Vector3.Cross(axis, Vector3.up);
            t_up = Vector3.Cross(t_orthoVec, axis);
        }
        else
        {
            t_orthoVec = Vector3.Cross(axis, Vector3.forward);
            t_up = Vector3.Cross(axis, t_orthoVec);
        }

        //angle
        Handles.color = angleCol;
        Handles.DrawSolidArc(t_position + axis * offsetSize, axis, t_orthoVec, angle, radiusSize);

        // axis Arrow
        Handles.color = MainCol;
        Quaternion t_rot = Quaternion.LookRotation(axis, t_up);
        Handles.ArrowCap(1, t_position, t_rot, arrowSize);

        // ortho Arrow
        if (drawOrtho)
        {
            Quaternion t_orthoQ = Quaternion.LookRotation(t_orthoVec, t_up);
            Handles.color = orthoCol;
            Handles.ArrowCap(2, t_position, t_orthoQ, arrowSize);
        }

        //local segment direction
        if (isArm)
        {
            Handles.color = Handles.xAxisColor;
            if (isRight)
                Handles.ArrowCap(3, t_position, segment.transform.rotation * Quaternion.Euler(0, 90, 0), arrowSize);
            else
                Handles.ArrowCap(3, t_position, segment.transform.rotation * Quaternion.Euler(0, -90, 0), arrowSize);
        }
        else //local up
        {
            Handles.color = Handles.yAxisColor;
            Handles.ArrowCap(3, t_position, segment.transform.rotation * Quaternion.Euler(90, 0, 0), arrowSize);
        }


    }

    public void OnEnable()
    {
        DummyQuaternion segment = target as DummyQuaternion;
        if (segment.transform.name.Contains("Leg"))
            isArm = false;
        else
            isArm = true;

        if (segment.transform.name.Contains("Left"))
            isRight = false;
        else
            isRight = true;

        drawOrthoProp = serializedObject.FindProperty("drawOrtho");
        AdditionalDebugProp = serializedObject.FindProperty("AdditionalDebug");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (isArm)
        {
            if (isRight)
                EditorGUILayout.LabelField("Right Arm");
            else
                EditorGUILayout.LabelField("Left Arm");

        }
        else
            if (isRight)
            EditorGUILayout.LabelField("Right Leg");
        else
            EditorGUILayout.LabelField("Left Leg");

        drawOrthoProp.boolValue = EditorGUILayout.Toggle("Draw Orthogonal vector (grey)", drawOrthoProp.boolValue);
        AdditionalDebugProp.boolValue = EditorGUILayout.Toggle("Additional Helper", AdditionalDebugProp.boolValue);
        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
    }
}
