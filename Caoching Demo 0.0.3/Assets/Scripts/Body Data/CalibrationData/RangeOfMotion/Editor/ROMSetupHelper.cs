using UnityEngine;
using System.Collections;
using UnityEditor;
using Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion;
using Assets.Scripts.Body_Data.View;

[CustomEditor(typeof(DummyQuaternionHelper))]
public class ROMSetupHelper : Editor
{
    SerializedProperty staticROM_ser;

    Color MainCol = Color.magenta;
    Color orthoCol = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    Color angleCol = Color.yellow;

    public bool isArm;
    public bool isRight;

    public bool drawOrtho = false;

    void OnSceneGUI()
    {
        DummyQuaternionHelper segment = target as DummyQuaternionHelper;
        float arrowSize = HandleUtility.GetHandleSize(segment.transform.position) * 1.2f;
        float offsetSize = HandleUtility.GetHandleSize(segment.transform.position) * 1.1f;
        float radiusSize = HandleUtility.GetHandleSize(segment.transform.position) * 0.5f;

        Handles.color = MainCol;
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

        // axis Arrow
        Quaternion t_rot = Quaternion.LookRotation(axis, t_up);
        Handles.ArrowCap(0, segment.transform.position, t_rot, arrowSize);

        // ortho Arrow
        if (drawOrtho)
        {
            Quaternion t_orthoQ = Quaternion.LookRotation(t_orthoVec, t_up);
            Handles.color = orthoCol;
            Handles.ArrowCap(0, segment.transform.position, t_orthoQ, arrowSize);
        }

        //local segment direction
        if (isArm)
        {
            Handles.color = Handles.xAxisColor;
            if (isRight)
                Handles.ArrowCap(0, segment.transform.position, segment.transform.rotation * Quaternion.Euler(0, 90, 0), arrowSize);
            else
                Handles.ArrowCap(0, segment.transform.position, segment.transform.rotation * Quaternion.Euler(0, -90, 0), arrowSize);
        }
        else //local up
        {
            Handles.color = Handles.yAxisColor;
            Handles.ArrowCap(0, segment.transform.position, segment.transform.rotation * Quaternion.Euler(90, 0, 0), arrowSize);
        }


        //angle
        Handles.color = angleCol;
        Handles.DrawSolidArc(segment.transform.position + axis * offsetSize, axis, t_orthoVec, angle, radiusSize);
    }

    public void OnEnable()
    {
        DummyQuaternionHelper segment = target as DummyQuaternionHelper;
        if (segment.transform.name.Contains("Leg"))
            isArm = false;
        else
            isArm = true;

        if (segment.transform.name.Contains("Left"))
            isRight = false;
        else
            isRight = true;
    }

    public override void OnInspectorGUI()
    {
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

        drawOrtho = EditorGUILayout.Toggle("Draw Orthogonal vector (grey)", drawOrtho);


    }
}
