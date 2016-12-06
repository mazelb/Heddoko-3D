using UnityEngine;
using System.Collections;
using UnityEditor;
using Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion;
using Assets.Scripts.Body_Data.View;

[CustomEditor(typeof(SimpleROMMB))]
[CanEditMultipleObjects]
public class SimpleROMHelper : Editor
{
    SerializedProperty m_staticROM_ser;

    Color m_MainCol = Color.magenta;
    Color m_orthoCol = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    Color m_sphereCol = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    Color m_angleCol = new Color(1.0f, 0.92f, 0.016f, 0.5f);
    Color m_Xcolor = new Color(1.0f, 0, 0, 0.3f);
    Color m_Ycolor = new Color(0, 1.0f, 0, 0.3f);
    Color m_Zcolor = new Color(0, 0, 1.0f, 0.3f);

    SerializedProperty m_ReferenceIsHoriz;
    SerializedProperty m_InvertReference;
    
    SerializedProperty m_ROMProp;
    SerializedProperty m_XConstraint;
    SerializedProperty m_YConstraint;
    SerializedProperty m_ZConstraint;
    SerializedProperty m_ToggleXConstraint;
    SerializedProperty m_ToggleYConstraint;
    SerializedProperty m_ToggleZConstraint;

    float m_minX;
    float m_minY;
    float m_minZ;
    float m_maxX;
    float m_maxY;
    float m_maxZ;

    SimpleROMMB m_simpleROMMB;
    float m_handleSize;
    float m_arrowSize;
    float m_offsetSize;
    float m_radiusSize;
    Vector3 m_positionGizmo;
    Quaternion m_localRotation;

    bool m_IsInBounds;

    private void DrawAngleAxis(Vector3 a_position)
    {
        float angle;
        Vector3 axis;
        m_localRotation.ToAngleAxis(out angle, out axis);
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
        Handles.color = m_angleCol;
        Handles.DrawSolidArc(a_position + axis * m_offsetSize, axis, t_orthoVec, angle, m_radiusSize);

        // axis Arrow
        Handles.color = m_MainCol;
        Quaternion t_rot = Quaternion.LookRotation(axis, t_up);
        Handles.ArrowCap(1, a_position, t_rot, m_arrowSize);
    }

    private void DrawXConstraint(Quaternion ParentRotation, Quaternion XMinQ, Quaternion XMaxQ)
    {
        Handles.color = Handles.xAxisColor;
        Handles.DrawWireArc(m_positionGizmo, ParentRotation * Vector3.right, XMinQ * Vector3.forward, m_maxX - m_minX, m_arrowSize);
        Handles.color = m_Xcolor;
        Handles.DrawSolidArc(m_positionGizmo, ParentRotation * XMinQ * Vector3.up, ParentRotation * Vector3.right, -180, m_arrowSize);
        Handles.DrawSolidArc(m_positionGizmo, ParentRotation * XMaxQ * Vector3.up, ParentRotation * Vector3.right, -180, m_arrowSize);
    }

    private void DrawYConstraint(Quaternion ParentRotation, Quaternion YMinQ, ref Vector3 YiNormal, ref Vector3 YaNormal, Quaternion YMaxQ, Quaternion YiZa, Quaternion YaZa)
    {
        if (m_ToggleZConstraint.boolValue)
        {

            Quaternion ZMedQ = Quaternion.Euler(0, 0, m_minZ + (m_maxZ - m_minZ) * 0.5f);
            Handles.color = Handles.yAxisColor;
            Handles.DrawWireArc(m_positionGizmo, ParentRotation * Vector3.up, YMinQ * ZMedQ * Vector3.right, m_maxY - m_minY, m_arrowSize);
            //Handles.DrawWireArc(m_positionGizmo, ParentRotation * Vector3.up, YMinQ * Vector3.right, maxY - minY, m_arrowSize);

            YiNormal = ParentRotation * YMinQ * Vector3.forward;
            YaNormal = ParentRotation * YMaxQ * Vector3.forward;
            Vector3 YiSart = YiZa * Vector3.right;
            Vector3 YaSart = YaZa * Vector3.right;
            Handles.color = m_Ycolor;

            Handles.DrawSolidArc(m_positionGizmo, YiNormal, YiSart, m_minZ - m_maxZ, m_arrowSize);
            Handles.DrawSolidArc(m_positionGizmo, YaNormal, YaSart, m_minZ - m_maxZ, m_arrowSize);
        }
        else
        {
            Handles.color = Handles.yAxisColor;
            Handles.DrawWireArc(m_positionGizmo, ParentRotation * Vector3.up, YMinQ * Vector3.right, m_maxY - m_minY, m_arrowSize);
            Handles.color = m_Ycolor;
            Handles.DrawSolidArc(m_positionGizmo, ParentRotation * YMinQ * Vector3.forward, ParentRotation * Vector3.up, -180, m_arrowSize);
            Handles.DrawSolidArc(m_positionGizmo, ParentRotation * YMaxQ * Vector3.forward, ParentRotation * Vector3.up, -180, m_arrowSize);
        }
    }

    private void DrawZConstraint(Quaternion ParentRotation, Quaternion ZMinQ, Quaternion YiZa, Quaternion YaZa, Quaternion YiZi, Quaternion YaZi, Quaternion ZMaxQ)
    {
        //Handles.color = Handles.zAxisColor;
        if (m_ToggleYConstraint.boolValue)
        {
            Quaternion YMedQ = Quaternion.Euler(0, m_minY + (m_maxY - m_minY) * 0.5f, 0);
            Handles.color = m_Zcolor;
            Handles.DrawWireArc(m_positionGizmo, ParentRotation * YMedQ * Vector3.forward, YMedQ * ZMinQ * Vector3.right, m_maxZ - m_minZ, m_arrowSize);


            float angleZMax, angleZMin;
            Vector3 axisZMax, axisZMin;
            Quaternion YiZaInv = Quaternion.Inverse(YiZa);
            Quaternion tempZmaxInYcoord = Quaternion.RotateTowards(YiZa, YaZa * YiZaInv, 360);
            tempZmaxInYcoord.ToAngleAxis(out angleZMax, out axisZMax);

            Quaternion YiZiInv = Quaternion.Inverse(YiZi);
            Quaternion tempZminInYcoord = Quaternion.RotateTowards(YiZi, YaZi * YiZiInv, 360);
            tempZminInYcoord.ToAngleAxis(out angleZMin, out axisZMin);

            //Handles.DrawSolidArc(m_positionGizmo, ZaNormal, YiZa * Vector3.right, Vector3.Angle(YiZaVec, YaZaVec), m_arrowSize);
            //Handles.DrawSolidArc(m_positionGizmo, ZiNormal, YiZi * Vector3.right, Vector3.Angle(YiZiVec, YaZiVec), m_arrowSize);
            Handles.DrawSolidArc(m_positionGizmo, axisZMax, YiZa * Vector3.right, angleZMax, m_arrowSize);
            Handles.DrawSolidArc(m_positionGizmo, axisZMin, YiZi * Vector3.right, angleZMin, m_arrowSize);

            //Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            //Handles.DrawSolidArc(m_positionGizmo, axisZMax, YiZa * Vector3.right, -angleZMax, m_arrowSize);
            //Handles.DrawSolidArc(m_positionGizmo, axisZMin, YiZi * Vector3.right, -angleZMin, m_arrowSize);
        }
        else
        {

            //Handles.DrawSolidDisc(m_positionGizmo + ParentRotation * ZMinQ * (m_arrowSize * Vector3.up), ParentRotation * Vector3.up, m_arrowSize);
            //Handles.DrawSolidDisc(m_positionGizmo - ParentRotation * ZMaxQ * (m_arrowSize * Vector3.up), ParentRotation * Vector3.up, m_arrowSize);

            Handles.color = m_Zcolor;
            Handles.DrawWireArc(m_positionGizmo, ParentRotation * Vector3.forward, ZMinQ * Vector3.right, m_maxZ - m_minZ, m_arrowSize);

            Handles.DrawSolidArc(m_positionGizmo, ParentRotation * ZMinQ * Vector3.up, ParentRotation * Vector3.forward, 180, m_arrowSize);
            Handles.DrawSolidArc(m_positionGizmo, ParentRotation * ZMaxQ * Vector3.up, ParentRotation * Vector3.forward, 180, m_arrowSize);
        }
    }

    private void DrawConstraints(Quaternion ParentRotation)
    {
        Quaternion XMinQ = Quaternion.Euler(m_minX, 0, 0);
        Quaternion XMaxQ = Quaternion.Euler(m_maxX, 0, 0);
        Quaternion YMinQ = Quaternion.Euler(0, m_minY, 0);
        Quaternion YMaxQ = Quaternion.Euler(0, m_maxY, 0);
        Quaternion ZMinQ = Quaternion.Euler(0, 0, m_minZ);
        Quaternion ZMaxQ = Quaternion.Euler(0, 0, m_maxZ);

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

        if (m_ToggleXConstraint.boolValue)
            DrawXConstraint( ParentRotation, XMinQ, XMaxQ);

        if (m_ToggleYConstraint.boolValue)
            DrawYConstraint( ParentRotation, YMinQ, ref YiNormal, ref YaNormal, YMaxQ, YiZa, YaZa);

        if (m_ToggleZConstraint.boolValue)
            DrawZConstraint( ParentRotation, ZMinQ, YiZa, YaZa, YiZi, YaZi, ZMaxQ);

        //Handles.color = Color.black;
        //Handles.DrawLine(m_positionGizmo, m_positionGizmo + (YiZa * Vector3.right) * m_arrowSize);
        //Handles.color = Color.red;
        //Handles.DrawLine(m_positionGizmo, m_positionGizmo + (YaZa * Vector3.right) * m_arrowSize);
    }

    private void DrawCustomGizmos()
    {
        Handles.color = m_sphereCol;
        Handles.SphereCap(4, m_positionGizmo, Quaternion.identity, m_arrowSize*2);

        DrawAngleAxis(m_simpleROMMB.transform.position);
        DrawConstraints(m_simpleROMMB.transform.parent == null ? Quaternion.identity : m_simpleROMMB.transform.parent.rotation);


        //Handles.color = Color.black;
        if (m_IsInBounds)
            Handles.color = Color.cyan; // in z bounds
        else
            Handles.color = Color.black; // not z bounds
        Handles.ArrowCap(3, m_positionGizmo, m_localRotation * Quaternion.Euler(0, 90, 0), m_arrowSize);


        Handles.color = Handles.xAxisColor;
        Handles.ArrowCap(3, m_simpleROMMB.transform.position, m_localRotation * Quaternion.Euler(0, 90, 0), m_arrowSize / 2);
        Handles.color = Handles.yAxisColor;
        Handles.ArrowCap(3, m_simpleROMMB.transform.position, m_localRotation * Quaternion.Euler(-90, 0, 0), m_arrowSize / 2);
        Handles.color = Handles.zAxisColor;
        Handles.ArrowCap(3, m_simpleROMMB.transform.position, m_localRotation * Quaternion.Euler(0, 0, 90), m_arrowSize / 2);
    }


    void OnSceneGUI()
    {
        m_simpleROMMB = target as SimpleROMMB;
        m_handleSize        = HandleUtility.GetHandleSize(m_simpleROMMB.transform.position) * 3;
        m_arrowSize         = m_handleSize * 1.2f * 0.5f;
        m_offsetSize        = m_handleSize * 1.1f * 0.5f;
        m_radiusSize        = m_handleSize * 0.5f;
        m_positionGizmo      = m_simpleROMMB.transform.position + Vector3.up * m_handleSize * 1.5f;
        m_localRotation = m_simpleROMMB.transform.localRotation;

        DrawCustomGizmos();
        ApplyConstraint();
    }

    private void ApplyConstraint()
    {
        Quaternion XMinQ = Quaternion.Euler(m_minX, 0, 0);
        Quaternion XMaxQ = Quaternion.Euler(m_maxX, 0, 0);
        Quaternion YMinQ = Quaternion.Euler(0, m_minY, 0);
        Quaternion YMaxQ = Quaternion.Euler(0, m_maxY, 0);
        Quaternion ZMinQ = Quaternion.Euler(0, 0, m_minZ);
        Quaternion ZMaxQ = Quaternion.Euler(0, 0, m_maxZ);

        Quaternion YaZa = YMaxQ * ZMaxQ;
        Quaternion YaZi = YMaxQ * ZMinQ;
        Quaternion YiZa = YMinQ * ZMaxQ;
        Quaternion YiZi = YMinQ * ZMinQ;

        Vector3 YaZaVec = (YaZa * Vector3.right);
        Vector3 YaZiVec = (YaZi * Vector3.right);
        Vector3 YiZaVec = (YiZa * Vector3.right);
        Vector3 YiZiVec = (YiZi * Vector3.right);


        Vector3 localAxe = m_localRotation * Vector3.right;
        Vector3 toYaZa = YaZaVec - localAxe;
        Vector3 toYaZi = YaZiVec - localAxe;
        Vector3 toYiZa = YiZaVec - localAxe;
        Vector3 toYiZi = YiZiVec - localAxe;
        Vector3 localEnd = m_positionGizmo + localAxe * m_arrowSize;

        // draw axes from main direction to constraint bounds
//         Handles.color = Color.gray;
//         Handles.DrawLine(localEnd, t_position + YaZaVec * m_arrowSize);
//         Handles.DrawLine(localEnd, t_position + YaZiVec * m_arrowSize);
//         Handles.DrawLine(localEnd, t_position + YiZaVec * m_arrowSize);
//         Handles.DrawLine(localEnd, t_position + YiZiVec * m_arrowSize);

        Vector3 endToYaZa = (m_positionGizmo + YaZaVec * m_arrowSize) - localEnd;
        Vector3 endToYiZa = (m_positionGizmo + YiZaVec * m_arrowSize) - localEnd;

        Vector3 endToYaZi = (m_positionGizmo + YaZiVec * m_arrowSize) - localEnd;
        Vector3 endToYiZi = (m_positionGizmo + YiZiVec * m_arrowSize) - localEnd;
        if(ApproxZConstraint(endToYiZa, endToYaZa, endToYiZi, endToYaZi, localAxe))
        {
            if (CheckYConstraint(endToYiZa, endToYaZa, endToYiZi, endToYaZi, localAxe, localEnd, m_arrowSize))
                m_IsInBounds = true;
            else
                m_IsInBounds = false;

        }
        else
            m_IsInBounds = false;

        DrawDecomposition(localAxe, m_positionGizmo, m_arrowSize);
        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        m_ReferenceIsHoriz.boolValue = EditorGUILayout.Toggle("Horiz ?", m_ReferenceIsHoriz.boolValue);
        m_InvertReference.boolValue = EditorGUILayout.Toggle("Invert ?", m_InvertReference.boolValue);
        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.

        m_minX = m_XConstraint.FindPropertyRelative("minAngle").floatValue;
        m_minY = m_YConstraint.FindPropertyRelative("minAngle").floatValue;
        m_minZ = m_ZConstraint.FindPropertyRelative("minAngle").floatValue;
        m_maxX = m_XConstraint.FindPropertyRelative("maxAngle").floatValue;
        m_maxY = m_YConstraint.FindPropertyRelative("maxAngle").floatValue;
        m_maxZ = m_ZConstraint.FindPropertyRelative("maxAngle").floatValue;

        m_ToggleXConstraint.boolValue = EditorGUILayout.BeginToggleGroup("toggle X constraint", m_ToggleXConstraint.boolValue);
        if(m_ToggleXConstraint.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("min: " + m_minX.ToString());
            EditorGUILayout.LabelField("max: " + m_maxX.ToString());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref m_minX, ref m_maxX, -180, 180);
        }
        EditorGUILayout.EndToggleGroup();

        m_ToggleYConstraint.boolValue = EditorGUILayout.BeginToggleGroup("toggle Y constraint", m_ToggleYConstraint.boolValue);
        if(m_ToggleYConstraint.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("min: " + m_minY.ToString());
            EditorGUILayout.LabelField("max: " + m_maxY.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.MinMaxSlider(ref m_minY, ref m_maxY, -180, 180);
        }
        EditorGUILayout.EndToggleGroup();


        if(m_ToggleYConstraint.boolValue && m_ToggleZConstraint.boolValue)
            m_ToggleZConstraint.boolValue = EditorGUILayout.BeginToggleGroup("toggle Z constraint", m_ToggleZConstraint.boolValue);
        else
            m_ToggleZConstraint.boolValue = EditorGUILayout.BeginToggleGroup("toggle Z constraint", m_ToggleZConstraint.boolValue);

        if (m_ToggleZConstraint.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("min: " + m_minZ.ToString());
            EditorGUILayout.LabelField("max: " + m_maxZ.ToString());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref m_minZ, ref m_maxZ, -90, 90);
        }
        EditorGUILayout.EndToggleGroup();

        m_XConstraint.FindPropertyRelative("minAngle").floatValue = m_minX;
        m_YConstraint.FindPropertyRelative("minAngle").floatValue = m_minY;
        m_ZConstraint.FindPropertyRelative("minAngle").floatValue = m_minZ;
        m_XConstraint.FindPropertyRelative("maxAngle").floatValue = m_maxX;
        m_YConstraint.FindPropertyRelative("maxAngle").floatValue = m_maxY;
        m_ZConstraint.FindPropertyRelative("maxAngle").floatValue = m_maxZ;


        serializedObject.ApplyModifiedProperties();
    }

    public void OnEnable()
    {
        SimpleROMMB simpleROMMB = target as SimpleROMMB;

        m_ReferenceIsHoriz = serializedObject.FindProperty("ReferenceIsHoriz");
        m_InvertReference = serializedObject.FindProperty("InvertReference");

        m_ROMProp = serializedObject.FindProperty("rom");

        m_XConstraint = m_ROMProp.FindPropertyRelative("XMinMax");
        m_YConstraint = m_ROMProp.FindPropertyRelative("YMinMax");
        m_ZConstraint = m_ROMProp.FindPropertyRelative("ZMinMax");

        m_ToggleXConstraint = serializedObject.FindProperty("ToggleXConstraint");
        m_ToggleYConstraint = serializedObject.FindProperty("ToggleYConstraint");
        m_ToggleZConstraint = serializedObject.FindProperty("ToggleZConstraint");


    }

    private bool CheckYConstraint(Vector3 endToYiZa, Vector3 endToYaZa, Vector3 endToYiZi, Vector3 endToYaZi, Vector3 localAxe, Vector3 localEnd, float m_arrowSize)
    {

        Vector3 normalYi = Vector3.up;
        Vector3 normalYa = Vector3.up;

        if ( (m_maxZ < 90 && m_minZ > -90) ||
             (m_maxZ > 90 && m_minZ < -90))
        {
            normalYa = Vector3.Cross(endToYiZa, endToYiZi).normalized;
            normalYi = Vector3.Cross(endToYaZa, endToYaZi).normalized;
        }
        else if (m_maxZ > 90 && m_minZ > -90)
        {
            normalYa = Vector3.Cross(endToYaZa, endToYiZi).normalized;
            normalYi = Vector3.Cross(endToYiZa, endToYaZi).normalized;
        }
        else // if (maxZ < 90 && minZ < -90)
        {
            normalYa = Vector3.Cross(endToYiZa, endToYaZi).normalized;
            normalYi = Vector3.Cross(endToYaZa, endToYiZi).normalized;
        }


        float tYa = Vector3.Dot(normalYa, localAxe);
        float tYi = Vector3.Dot(normalYi, localAxe);

        bool YisInBound = false;
        if (tYi < 0 && tYa > 0)
            YisInBound = true;
        else
            YisInBound = false;

        return YisInBound;
    }

    private bool ApproxZConstraint(Vector3 endToYiZa, Vector3 endToYaZa, Vector3 endToYiZi, Vector3 endToYaZi, Vector3 localAxe)
    {
        Vector3 normalZi;
        Vector3 normalZa;

        if (m_maxZ > 90)
            normalZa = Vector3.Cross(endToYiZa, endToYaZa).normalized;
        else
            normalZa = Vector3.Cross(endToYaZa, endToYiZa).normalized;

        if (m_minZ < -90)
            normalZi = Vector3.Cross(endToYiZi, endToYaZi).normalized;
        else
            normalZi = Vector3.Cross(endToYaZi, endToYiZi).normalized;


        float tZa = Vector3.Dot(normalZa, localAxe);
        float tZi = Vector3.Dot(normalZi, localAxe);


        bool ZisInBound = false;
        if (tZi < 0 && tZa > 0)
            ZisInBound = true;
        else
            ZisInBound = false;

        return ZisInBound;
    }

    private void DrawDecomposition(Vector3 localAxe, Vector3 t_position, float m_arrowSize)
    {
        Quaternion XMinQ = Quaternion.Euler(m_minX, 0, 0);
        Quaternion XMaxQ = Quaternion.Euler(m_maxX, 0, 0);
        Quaternion YMinQ = Quaternion.Euler(0, m_minY, 0);
        Quaternion YMaxQ = Quaternion.Euler(0, m_maxY, 0);
        Quaternion YMedQ = Quaternion.Euler(0, m_minY + (m_maxY - m_minY) * 0.5f, 0);
        Quaternion ZMinQ = Quaternion.Euler(0, 0, m_minZ);
        Quaternion ZMaxQ = Quaternion.Euler(0, 0, m_maxZ);
        Quaternion ZMedQ = Quaternion.Euler(0,0, m_minZ + (m_maxZ - m_minZ) * 0.5f);

        Vector3 Ya = YMaxQ * Vector3.right;
        Vector3 YaPerp = Vector3.Dot(Ya, localAxe) * Ya;
        Vector3 YaPar = localAxe - YaPerp;

        Vector3 Yi = YMinQ * Vector3.right;
        Vector3 YiPerp = Vector3.Dot(Yi, localAxe) * Yi;
        Vector3 YiPar = localAxe - YiPerp;

        Handles.color = Color.green * 1.3f;
        Handles.DrawLine(t_position, t_position + YaPerp * m_arrowSize);
        Handles.DrawLine(t_position + YaPerp * m_arrowSize, t_position + YaPerp * m_arrowSize + YaPar * m_arrowSize);

        Handles.color = Color.green * 0.7f;
        Handles.DrawLine(t_position, t_position + YiPerp * m_arrowSize);
        Handles.DrawLine(t_position + YiPerp * m_arrowSize, t_position + YiPerp * m_arrowSize + YiPar * m_arrowSize);




    }


}
