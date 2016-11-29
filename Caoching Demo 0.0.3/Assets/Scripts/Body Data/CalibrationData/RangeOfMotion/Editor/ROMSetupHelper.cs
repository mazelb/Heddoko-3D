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
    void OnSceneGUI()
    {
        DummyQuaternionHelper segment = target as DummyQuaternionHelper;
        float arrowSize = HandleUtility.GetHandleSize(segment.transform.position)*1.2f;
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
        if(Mathf.Sin(Vector3.Angle(axis, Vector3.forward) * Mathf.Deg2Rad) < 0.1f) // special case if rotation axis is near +/- forward
        {

            Handles.color = orthoCol;
            Handles.ArrowCap(0, segment.transform.position, tQuat * Quaternion.Euler(0,90,0), arrowSize);
            Handles.color = angleCol;
            Handles.DrawSolidArc(segment.transform.position + axis * offsetSize, axis, Vector3.right, angle, radiusSize);
        }
        else // trivial case
        {

            Handles.color = orthoCol;
            Handles.ArrowCap(0, segment.transform.position, tQuat, arrowSize);
            Handles.color = angleCol;
            Handles.DrawSolidArc(segment.transform.position + axis * offsetSize, axis, tQuat * Vector3.forward, angle, radiusSize);
        }
    }


    void OnEnable()
    {
    }
}
