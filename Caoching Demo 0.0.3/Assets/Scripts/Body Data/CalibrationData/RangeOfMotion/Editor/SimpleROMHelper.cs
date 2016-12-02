using UnityEngine;
using System.Collections;
using UnityEditor;
using Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion;
using Assets.Scripts.Body_Data.View;

[CustomEditor(typeof(SimpleROMMB))]
[CanEditMultipleObjects]
public class SimpleROMHelper : Editor
{
    SerializedProperty staticROM_ser;

    Color MainCol = Color.magenta;
    Color orthoCol = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    Color sphereCol = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    Color angleCol = new Color(1.0f, 0.92f, 0.016f, 0.5f);
    Color Xcolor = new Color(1.0f, 0, 0, 0.3f);
    Color Ycolor = new Color(0, 1.0f, 0, 0.3f);
    Color Zcolor = new Color(0, 0, 1.0f, 0.3f);

    //     public bool isArm;
    //     public bool isRight;


    SerializedProperty ReferenceIsHoriz;
    SerializedProperty InvertReference;
    
    SerializedProperty ROMProp;
    SerializedProperty XConstraint;
    SerializedProperty YConstraint;
    SerializedProperty ZConstraint;
    SerializedProperty ToggleXConstraint;
    SerializedProperty ToggleYConstraint;
    SerializedProperty ToggleZConstraint;

    float minX;
    float minY;
    float minZ;
    float maxX;
    float maxY;
    float maxZ;

    SimpleROMMB simpleROMMB;
    float handleSize;
    float arrowSize;
    float offsetSize;
    float radiusSize;
    Vector3 t_position;
    Quaternion localRotation;

    private void DrawAngleAxis(ref Quaternion localRotation, Vector3 t_position, float offsetSize, float radiusSize, float arrowSize)
    {
        float angle;
        Vector3 axis;
        localRotation.ToAngleAxis(out angle, out axis);
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
    }
    private void DrawConstraints(SimpleROMMB simpleROMMB, Quaternion ParentRotation, Vector3 t_position, float offsetSize, float radiusSize, float arrowSize)
    {
        Quaternion XMinQ = Quaternion.Euler(minX, 0, 0);
        Quaternion XMaxQ = Quaternion.Euler(maxX, 0, 0);
        Quaternion YMinQ = Quaternion.Euler(0, minY, 0);
        Quaternion YMaxQ = Quaternion.Euler(0, maxY, 0);
        Quaternion ZMinQ = Quaternion.Euler(0, 0, minZ);
        Quaternion ZMaxQ = Quaternion.Euler(0, 0, maxZ);

        Quaternion YaZa = YMaxQ * ZMaxQ;
        Quaternion YaZi = YMaxQ * ZMinQ;
        Quaternion YiZa = YMinQ * ZMaxQ;
        Quaternion YiZi = YMinQ * ZMinQ;

        Vector3 YaZaVec = (YaZa * Vector3.right);
        Vector3 YaZiVec = (YaZi * Vector3.right);
        Vector3 YiZaVec = (YiZa * Vector3.right);
        Vector3 YiZiVec = (YiZi * Vector3.right);

        YaZaVec.Normalize();
        YaZiVec.Normalize();
        YiZaVec.Normalize();
        YiZiVec.Normalize();

        Vector3 ZaNormal = Vector3.Cross(YiZaVec, YaZaVec); ZaNormal.Normalize();
        Vector3 ZiNormal = Vector3.Cross(YiZiVec, YaZiVec); ZiNormal.Normalize();
        Vector3 YaNormal = Vector3.Cross(YiZaVec, YiZiVec); YaNormal.Normalize();
        Vector3 YiNormal = Vector3.Cross(YiZaVec, YiZiVec); YiNormal.Normalize();

        if (ToggleXConstraint.boolValue)
        {
            Handles.color = Handles.xAxisColor;
            Handles.DrawWireArc(t_position, ParentRotation * Vector3.right, XMinQ * Vector3.forward, maxX - minX, arrowSize);
            Handles.color = Xcolor;
            Handles.DrawSolidArc(t_position, ParentRotation *XMinQ * Vector3.up, ParentRotation * Vector3.right, -180, arrowSize);
            Handles.DrawSolidArc(t_position, ParentRotation * XMaxQ * Vector3.up, ParentRotation * Vector3.right, -180, arrowSize);
        }
        if (ToggleYConstraint.boolValue)
        {


            if (ToggleZConstraint.boolValue)
            {

                Quaternion ZMedQ = Quaternion.Euler(0, 0, minZ + (maxZ - minZ) * 0.5f);
                Handles.color = Handles.yAxisColor;
                Handles.DrawWireArc(t_position, ParentRotation * Vector3.up, YMinQ * ZMedQ * Vector3.right, maxY - minY, arrowSize);
                //Handles.DrawWireArc(t_position, ParentRotation * Vector3.up, YMinQ * Vector3.right, maxY - minY, arrowSize);

                YiNormal = ParentRotation * YMinQ * Vector3.forward;
                YaNormal = ParentRotation * YMaxQ * Vector3.forward;
                Vector3 YiSart = YiZa * Vector3.right;
                Vector3 YaSart = YaZa * Vector3.right;
                Handles.color = Ycolor;
                Handles.DrawSolidArc(t_position, YiNormal, YiSart, minZ - maxZ, arrowSize);
                Handles.DrawSolidArc(t_position, YaNormal, YaSart, minZ - maxZ, arrowSize);
            }
            else
            {
                Handles.color = Handles.yAxisColor;
                Handles.DrawWireArc(t_position, ParentRotation * Vector3.up, YMinQ * Vector3.right, maxY - minY, arrowSize);
                Handles.color = Ycolor;
                Handles.DrawSolidArc(t_position, ParentRotation * YMinQ * Vector3.forward, ParentRotation * Vector3.up, -180, arrowSize);
                Handles.DrawSolidArc(t_position, ParentRotation * YMaxQ * Vector3.forward, ParentRotation * Vector3.up, -180, arrowSize);
            }
        }

        if (ToggleZConstraint.boolValue)
        {
            //Handles.color = Handles.zAxisColor;
            if(ToggleYConstraint.boolValue)
            {
                Quaternion YMedQ = Quaternion.Euler(0, minY + ( maxY - minY) * 0.5f, 0);
                Handles.color = Zcolor;
                Handles.DrawWireArc(t_position, ParentRotation * YMedQ * Vector3.forward, YMedQ * ZMinQ * Vector3.right, maxZ - minZ, arrowSize);


                float angleZMax, angleZMin;
                Vector3 axisZMax, axisZMin;
                Quaternion YiZaInv = Quaternion.Inverse(YiZa);
                Quaternion tempZmaxInYcoord = Quaternion.RotateTowards(YiZa, YaZa * YiZaInv, 360);
                tempZmaxInYcoord.ToAngleAxis(out angleZMax, out axisZMax);

                Quaternion YiZiInv = Quaternion.Inverse(YiZi);
                Quaternion tempZminInYcoord = Quaternion.RotateTowards(YiZi, YaZi * YiZiInv, 360);
                tempZminInYcoord.ToAngleAxis(out angleZMin, out axisZMin);

                //Handles.DrawSolidArc(t_position, ZaNormal, YiZa * Vector3.right, Vector3.Angle(YiZaVec, YaZaVec), arrowSize);
                //Handles.DrawSolidArc(t_position, ZiNormal, YiZi * Vector3.right, Vector3.Angle(YiZiVec, YaZiVec), arrowSize);
                Handles.DrawSolidArc(t_position, axisZMax, YiZa * Vector3.right, angleZMax, arrowSize);
                Handles.DrawSolidArc(t_position, axisZMin, YiZi * Vector3.right, angleZMin, arrowSize);

                //Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                //Handles.DrawSolidArc(t_position, axisZMax, YiZa * Vector3.right, -angleZMax, arrowSize);
                //Handles.DrawSolidArc(t_position, axisZMin, YiZi * Vector3.right, -angleZMin, arrowSize);
            }
            else
            {

                //Handles.DrawSolidDisc(t_position + ParentRotation * ZMinQ * (arrowSize * Vector3.up), ParentRotation * Vector3.up, arrowSize);
                //Handles.DrawSolidDisc(t_position - ParentRotation * ZMaxQ * (arrowSize * Vector3.up), ParentRotation * Vector3.up, arrowSize);

                Handles.color = Zcolor;
                Handles.DrawWireArc(t_position, ParentRotation * Vector3.forward, ZMinQ * Vector3.right, maxZ - minZ, arrowSize);

                Handles.DrawSolidArc(t_position, ParentRotation * ZMinQ * Vector3.up, ParentRotation * Vector3.forward, 180, arrowSize);
                Handles.DrawSolidArc(t_position, ParentRotation * ZMaxQ * Vector3.up, ParentRotation * Vector3.forward, 180, arrowSize);
            }
        }
        //Handles.color = Color.black;
        //Handles.DrawLine(t_position, t_position + (YiZa * Vector3.right) * arrowSize);
        //Handles.color = Color.red;
        //Handles.DrawLine(t_position, t_position + (YaZa * Vector3.right) * arrowSize);
    }

    private void DrawCustomGizmos()
    {
        //SimpleROMMB simpleROMMB = target as SimpleROMMB;
        //bool drawOrtho = drawOrthoProp.boolValue;

        //float handleSize = HandleUtility.GetHandleSize(simpleROMMB.transform.position) * 2;
        //float arrowSize = handleSize * 1.2f * 0.5f;
        //float offsetSize = handleSize * 1.1f * 0.5f;
        //float radiusSize = handleSize * 0.5f;
        //Vector3 t_position = simpleROMMB.transform.position + Vector3.up * handleSize * 1.5f;

        Handles.color = sphereCol;
        Handles.SphereCap(4, t_position, Quaternion.identity, arrowSize*2);

        Quaternion localRotation = simpleROMMB.transform.localRotation;
        Quaternion globalRotation = simpleROMMB.transform.rotation;

        DrawAngleAxis(ref localRotation, simpleROMMB.transform.position, offsetSize, radiusSize, arrowSize);
        DrawConstraints(simpleROMMB, simpleROMMB.transform.parent == null ? Quaternion.identity : simpleROMMB.transform.parent.rotation, t_position, offsetSize, radiusSize, arrowSize);


        Handles.color = Color.black;
        Handles.ArrowCap(3, t_position, localRotation * Quaternion.Euler(0, 90, 0), arrowSize);


        Handles.color = Handles.xAxisColor;
        Handles.ArrowCap(3, simpleROMMB.transform.position, localRotation * Quaternion.Euler(0, 90, 0), arrowSize / 2);
        Handles.color = Handles.yAxisColor;
        Handles.ArrowCap(3, simpleROMMB.transform.position, localRotation * Quaternion.Euler(-90, 0, 0), arrowSize / 2);
        Handles.color = Handles.zAxisColor;
        Handles.ArrowCap(3, simpleROMMB.transform.position, localRotation * Quaternion.Euler(0, 0, 90), arrowSize / 2);
    }


        void OnSceneGUI()
    {
        simpleROMMB = target as SimpleROMMB;
        handleSize        = HandleUtility.GetHandleSize(simpleROMMB.transform.position) * 3;
        arrowSize         = handleSize * 1.2f * 0.5f;
        offsetSize        = handleSize * 1.1f * 0.5f;
        radiusSize        = handleSize * 0.5f;
        t_position      = simpleROMMB.transform.position + Vector3.up * handleSize * 1.5f;
        localRotation = simpleROMMB.transform.localRotation;

        DrawCustomGizmos();
        ApplyConstraint();
    }
    private void ApplyConstraint()
    {
        Quaternion XMinQ = Quaternion.Euler(minX, 0, 0);
        Quaternion XMaxQ = Quaternion.Euler(maxX, 0, 0);
        Quaternion YMinQ = Quaternion.Euler(0, minY, 0);
        Quaternion YMaxQ = Quaternion.Euler(0, maxY, 0);
        Quaternion ZMinQ = Quaternion.Euler(0, 0, minZ);
        Quaternion ZMaxQ = Quaternion.Euler(0, 0, maxZ);

        Quaternion YaZa = YMaxQ * ZMaxQ;
        Quaternion YaZi = YMaxQ * ZMinQ;
        Quaternion YiZa = YMinQ * ZMaxQ;
        Quaternion YiZi = YMinQ * ZMinQ;

        Vector3 YaZaVec = (YaZa * Vector3.right);
        Vector3 YaZiVec = (YaZi * Vector3.right);
        Vector3 YiZaVec = (YiZa * Vector3.right);
        Vector3 YiZiVec = (YiZi * Vector3.right);



        Vector3 localAxe = localRotation * Vector3.right;
        Vector3 toYaZa = YaZaVec - localAxe;
        Vector3 toYaZi = YaZiVec - localAxe;
        Vector3 toYiZa = YiZaVec - localAxe;
        Vector3 toYiZi = YiZiVec - localAxe;
        Vector3 localEnd = t_position + localAxe * arrowSize;

        Handles.color = Color.red;
        Handles.DrawLine(localEnd, t_position + YaZaVec * arrowSize);
        Handles.DrawLine(localEnd, t_position + YaZiVec * arrowSize);
        Handles.DrawLine(localEnd, t_position + YiZaVec * arrowSize);
        Handles.DrawLine(localEnd, t_position + YiZiVec * arrowSize);

        Vector3 endToYaZa = (t_position + YaZaVec * arrowSize) - localEnd;
        Vector3 endToYiZa = (t_position + YiZaVec * arrowSize) - localEnd;

        Vector3 endToYaZi = (t_position + YaZiVec * arrowSize) - localEnd;
        Vector3 endToYiZi = (t_position + YiZiVec * arrowSize) - localEnd;

        Vector3 normalZa = Vector3.Cross(endToYaZa, endToYiZa).normalized;
        Vector3 normalZi = Vector3.Cross(endToYaZi, endToYiZi).normalized;
        float tZa = Vector3.Dot(normalZa, localAxe);
        float tZi = Vector3.Dot(normalZi, localAxe);


        if (tZi < 0 && tZa > 0)
            Handles.color = Color.cyan; // in z bounds
        else
            Handles.color = Color.red; // not z bounds

        Handles.DrawLine(localEnd, localEnd + normalZa * arrowSize);
        Handles.DrawLine(localEnd, localEnd + normalZi * arrowSize);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ReferenceIsHoriz.boolValue = EditorGUILayout.Toggle("Horiz ?", ReferenceIsHoriz.boolValue);
        InvertReference.boolValue = EditorGUILayout.Toggle("Invert ?", InvertReference.boolValue);
        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.

        minX = XConstraint.FindPropertyRelative("minAngle").floatValue;
        minY = YConstraint.FindPropertyRelative("minAngle").floatValue;
        minZ = ZConstraint.FindPropertyRelative("minAngle").floatValue;
        maxX = XConstraint.FindPropertyRelative("maxAngle").floatValue;
        maxY = YConstraint.FindPropertyRelative("maxAngle").floatValue;
        maxZ = ZConstraint.FindPropertyRelative("maxAngle").floatValue;

        ToggleXConstraint.boolValue = EditorGUILayout.BeginToggleGroup("toggle X constraint", ToggleXConstraint.boolValue);
        if(ToggleXConstraint.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("min: " + minX.ToString());
            EditorGUILayout.LabelField("max: " + maxX.ToString());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref minX, ref maxX, -180, 180);
        }
        EditorGUILayout.EndToggleGroup();

        ToggleYConstraint.boolValue = EditorGUILayout.BeginToggleGroup("toggle Y constraint", ToggleYConstraint.boolValue);
        if(ToggleYConstraint.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("min: " + minY.ToString());
            EditorGUILayout.LabelField("max: " + maxY.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.MinMaxSlider(ref minY, ref maxY, -180, 180);
        }
        EditorGUILayout.EndToggleGroup();


        if(ToggleYConstraint.boolValue && ToggleZConstraint.boolValue)
            ToggleZConstraint.boolValue = EditorGUILayout.BeginToggleGroup("toggle Z constraint", ToggleZConstraint.boolValue);
        else
            ToggleZConstraint.boolValue = EditorGUILayout.BeginToggleGroup("toggle Z constraint", ToggleZConstraint.boolValue);

        if (ToggleZConstraint.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("min: " + minZ.ToString());
            EditorGUILayout.LabelField("max: " + maxZ.ToString());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref minZ, ref maxZ, -180, 180);
        }
        EditorGUILayout.EndToggleGroup();

        XConstraint.FindPropertyRelative("minAngle").floatValue = minX;
        YConstraint.FindPropertyRelative("minAngle").floatValue = minY;
        ZConstraint.FindPropertyRelative("minAngle").floatValue = minZ;
        XConstraint.FindPropertyRelative("maxAngle").floatValue = maxX;
        YConstraint.FindPropertyRelative("maxAngle").floatValue = maxY;
        ZConstraint.FindPropertyRelative("maxAngle").floatValue = maxZ;


        serializedObject.ApplyModifiedProperties();
    }

    public void OnEnable()
    {
        SimpleROMMB simpleROMMB = target as SimpleROMMB;

        ReferenceIsHoriz = serializedObject.FindProperty("ReferenceIsHoriz");
        InvertReference = serializedObject.FindProperty("InvertReference");

        ROMProp = serializedObject.FindProperty("rom");

        XConstraint = ROMProp.FindPropertyRelative("XMinMax");
        YConstraint = ROMProp.FindPropertyRelative("YMinMax");
        ZConstraint = ROMProp.FindPropertyRelative("ZMinMax");

        ToggleXConstraint = serializedObject.FindProperty("ToggleXConstraint");
        ToggleYConstraint = serializedObject.FindProperty("ToggleYConstraint");
        ToggleZConstraint = serializedObject.FindProperty("ToggleZConstraint");


    }


}
