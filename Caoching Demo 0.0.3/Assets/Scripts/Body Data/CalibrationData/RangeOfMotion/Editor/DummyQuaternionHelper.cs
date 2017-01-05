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

    StaticRomMB RootROM;

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

//         if (RootROM == null)
//         {
//             Debug.LogError("RootROM is null ? ");
//             RootROM = segment.transform.GetComponentInParent<StaticRomMB>();
//             RootROM.PopulateSubSegment();
//             for (int i = 0; i < RootROM.subSegment.Length; ++i)
//             {
//                 Transform t_trans = RootROM.subSegment[i];
//                 if (t_trans.name == segment.transform.name)
//                     current_index = i;
//             }
//         }
        tQuat = segment.transform.localRotation; // local copy
        if (RootROM != null)
        {
            segment.transform.localRotation = 
                RootROM.ROM.capRotation((BodyStructureMap.SegmentTypes)(current_index / 2), 
                                        (BodyStructureMap.SubSegmentTypes)current_index,
                                        segment.transform, 
                                        ref tQuat, true, false);
        }
        else
        {
            Debug.LogError("RootROM is null : no capRotation ");
        }

        //Handles.PositionHandle(segment.transform.position, segment.transform.localRotation);
        Vector3[] verts = new Vector3[4];
        verts[0] = (arrowSize*0.5f * new Vector3(1, 1, 0));
        verts[1] = (arrowSize*0.5f * new Vector3(-1, 1, 0));
        verts[2] = (arrowSize*0.5f * new Vector3(-1, -1, 0));
        verts[3] = (arrowSize*0.5f * new Vector3(1, -1, 0));
        for (int i = 0; i < 4; ++i)
            verts[i] = segment.transform.position + (tQuat * verts[i]);

        Handles.DrawSolidRectangleWithOutline(verts, sphereCol, Color.black);
    }

    int current_index = 0;
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

        RootROM = segment.transform.GetComponentInParent<StaticRomMB>();
        if(RootROM == null)
        {
            Debug.LogError("can't find StaticRomMB script in parents");
        }
        else
        {
            RootROM.PopulateSubSegment();
            for(int i = 0; i < RootROM.subSegment.Length; ++i)
            {
                Transform t_trans = RootROM.subSegment[i];
                if (t_trans.name == segment.transform.name)
                    current_index = i;
            }
        }

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

        EditorGUILayout.Separator();
        DummyQuaternion segment = target as DummyQuaternion;
        Vector4 rot;
        rot.w = segment.transform.localRotation.w;
        rot.x = segment.transform.localRotation.x;
        rot.y = segment.transform.localRotation.y;
        rot.z = segment.transform.localRotation.z;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset"))
            segment.transform.localRotation = Quaternion.identity;
        EditorGUILayout.Vector4Field("quaternion", rot);
        EditorGUILayout.EndHorizontal();
        SimpleROM tROM = RootROM.ROM.squeletteRom[current_index];
        //EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("last computed Pitch", tROM.PitchMinMax.lastCompute);
        EditorGUILayout.FloatField("last computed Yaw", tROM.YawMinMax.lastCompute);
        EditorGUILayout.FloatField("last computed Roll", tROM.RollMinMax.lastCompute);
        //EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        drawOrthoProp.boolValue = EditorGUILayout.ToggleLeft("Draw Orthogonal vector (grey)", drawOrthoProp.boolValue);
        AdditionalDebugProp.boolValue = EditorGUILayout.ToggleLeft("Additional Helper", AdditionalDebugProp.boolValue);
        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        EditorGUILayout.Separator();

        //if (squel_draw[i] = EditorGUILayout.Foldout(squel_draw[i], t.Name))
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset", GUILayout.MaxWidth(100.0f)))
        {
            RootROM.ROM.Reset((BodyStructureMap.SubSegmentTypes)current_index);
        }
        EditorGUILayout.LabelField("Angle Constraint");
        EditorGUILayout.EndHorizontal();
        {
            SimpleROM t = RootROM.ROM.squeletteRom[current_index];
            EditorGUI.indentLevel++;
            
            StaticROMHelper.OnInspectorAngleConstraint(t.PitchMinMax, "Pitch axis");
            StaticROMHelper.OnInspectorAngleConstraint(t.YawMinMax, "Yaw axis");
            StaticROMHelper.OnInspectorAngleConstraint(t.RollMinMax, "Roll axis");
            EditorGUI.indentLevel--;
        }

        

        serializedObject.ApplyModifiedProperties();
    }
}
