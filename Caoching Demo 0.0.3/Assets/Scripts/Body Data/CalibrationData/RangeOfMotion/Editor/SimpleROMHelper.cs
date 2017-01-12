using UnityEngine;
using System.Collections;
using UnityEditor;
using Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion;
using Assets.Scripts.Body_Data.View;

[CustomEditor(typeof(SimpleROMMB))]
[CanEditMultipleObjects]
public class SimpleROMHelper : Editor
{
    #region color
    Color m_MainCol = Color.magenta;
    Color m_orthoCol = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    Color m_sphereCol = new Color(0.5f, 0.5f, 0.5f, 0.2f);
    Color m_angleCol = new Color(1.0f, 0.92f, 0.016f, 0.5f);
    Color m_Xcolor = new Color(1.0f, 0, 0, 0.3f);
    Color m_Ycolor = new Color(0, 1.0f, 0, 0.3f);
    Color m_Zcolor = new Color(0, 0, 1.0f, 0.3f);
    #endregion

    #region SerializedProperty 
    SerializedProperty m_staticROM_ser;

    SerializedProperty m_ReferenceIsHoriz;
    SerializedProperty m_InvertReference;
    
    SerializedProperty m_ROMProp;
    SerializedProperty m_XConstraint;
    SerializedProperty m_YConstraint;
    SerializedProperty m_ZConstraint;
    SerializedProperty m_ToggleXConstraint;
    SerializedProperty m_ToggleYConstraint;
    SerializedProperty m_ToggleZConstraint;

    //SerializedProperty ZMaxRadius;
    //SerializedProperty ZMinRadius;

    SimpleROMMB m_simpleROMMB;
    #endregion

    #region usefull member
    float m_minX;
    float m_minY;
    float m_minZ;
    float m_maxX;
    float m_maxY;
    float m_maxZ;

    float m_handleSize;
    float m_arrowSize;
    float m_offsetSize;
    float m_radiusSize;
    Vector3 m_positionGizmo;
    Quaternion m_localRotation;

    bool m_IsInBounds;

    bool m_drawDecomposition = false;
    bool m_drawMinMaxLinks = false;
    bool m_drawlocalAxis = false;
    bool m_drawProj = false;
    //bool m_offsetConstraintRepresentation = false;
    float m_offsetRepresentation = 0f;
    bool m_drawQuatAxis = false;
    float m_GUIhandleSizeMultiplier = 6;
    #endregion

    #region Editor

    void OnSceneGUI()
    {
        m_simpleROMMB = target as SimpleROMMB;
        m_handleSize = HandleUtility.GetHandleSize(m_simpleROMMB.transform.position) * m_GUIhandleSizeMultiplier;
        m_arrowSize = m_handleSize * 1.2f * 0.5f;
        m_offsetSize = m_handleSize * 1.1f * 0.5f;
        m_radiusSize = m_handleSize * 0.5f;

        m_positionGizmo = m_simpleROMMB.transform.position + Vector3.up * m_handleSize * m_offsetRepresentation;

        m_localRotation = m_simpleROMMB.transform.localRotation;

        DrawCustomGizmos();
        ApplyConstraint();
    }
    float y = 0;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.FloatField("Z Max Radius", m_RadiusConeMax);
        EditorGUILayout.FloatField("Z Min Radius", m_RadiusConeMin);
        EditorGUILayout.Separator();
        EditorGUILayout.FloatField("Proj Z Max Radius", m_RadiusProjOnConeMax);
        EditorGUILayout.FloatField("Proj Z Min Radius", m_RadiusProjOnConeMin);
        EditorGUILayout.Separator();
        ////y = EditorGUILayout.Knob(new Vector2(50, 50), y, -5.0f, 15.0f, "x", Color.red, Color.green, true);
        //    //EditorGUI.PrefixLabel(new Rect(0, 0, 200, 200), new GUIContent());
        //EditorGUILayout.BeginHorizontal();
        //{
        //    m_ConeCrossProdCol = EditorGUILayout.ColorField(new GUIContent(), m_ConeCrossProdCol, false, false, false, null, GUILayout.MaxWidth(20));
        //    EditorGUILayout.Vector3Field(new GUIContent("m_ConeCrossProd", "yellow"), m_ConeCrossProd);
        //}
        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //{
        //    m_crossProjMinCol = EditorGUILayout.ColorField(new GUIContent(), m_crossProjMinCol, false, false, false, null, GUILayout.MaxWidth(20));
        //    EditorGUILayout.Vector3Field(new GUIContent("m_crossProjMin ", "magenta"), m_crossProjMin);
        //}
        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //{
        //    m_crossProjMaxCol = EditorGUILayout.ColorField(new GUIContent(), m_crossProjMaxCol, false, false, false, null, GUILayout.MaxWidth(20));
        //    EditorGUILayout.Vector3Field(new GUIContent("m_crossProjMax ", "red"), m_crossProjMax);
        //}
        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //{
        //    m_crosslocalMinCol = EditorGUILayout.ColorField(new GUIContent(), m_crosslocalMinCol, false, false, false, null, GUILayout.MaxWidth(20));
        //    EditorGUILayout.Vector3Field(new GUIContent("m_crosslocalMîn", "white"), m_crosslocalMin);
        //}
        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //{
        //    m_crosslocalMaxCol = EditorGUILayout.ColorField(new GUIContent(), m_crosslocalMaxCol, false, false, false, null, GUILayout.MaxWidth(20));
        //    EditorGUILayout.Vector3Field(new GUIContent("m_crosslocalMax", "gray"), m_crosslocalMax);
        //}
        //EditorGUILayout.EndHorizontal();
        //EditorGUILayout.Separator();

        m_offsetRepresentation = EditorGUILayout.FloatField("offset constraint", m_offsetRepresentation);
        m_GUIhandleSizeMultiplier = EditorGUILayout.FloatField("handle size", m_GUIhandleSizeMultiplier);
        m_drawDecomposition = EditorGUILayout.ToggleLeft("draw decomposition", m_drawDecomposition);
        m_drawMinMaxLinks = EditorGUILayout.ToggleLeft("draw Min Max Links", m_drawMinMaxLinks);
        m_drawlocalAxis = EditorGUILayout.ToggleLeft("draw local axis", m_drawlocalAxis);
        m_drawQuatAxis = EditorGUILayout.ToggleLeft("draw quat axis", m_drawQuatAxis);
        m_drawProj = EditorGUILayout.ToggleLeft("draw proj", m_drawProj);
        EditorGUILayout.Separator();

        //         m_ReferenceIsHoriz.boolValue = EditorGUILayout.Toggle("Horiz ?", m_ReferenceIsHoriz.boolValue);
        //         m_InvertReference.boolValue = EditorGUILayout.Toggle("Invert ?", m_InvertReference.boolValue);
        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.

        m_minX = m_XConstraint.FindPropertyRelative("minAngle").floatValue;
        m_minY = m_YConstraint.FindPropertyRelative("minAngle").floatValue;
        m_minZ = m_ZConstraint.FindPropertyRelative("minAngle").floatValue;
        m_maxX = m_XConstraint.FindPropertyRelative("maxAngle").floatValue;
        m_maxY = m_YConstraint.FindPropertyRelative("maxAngle").floatValue;
        m_maxZ = m_ZConstraint.FindPropertyRelative("maxAngle").floatValue;

        m_ToggleXConstraint.boolValue = EditorGUILayout.BeginToggleGroup("toggle X constraint", m_ToggleXConstraint.boolValue);
        if (m_ToggleXConstraint.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("min: " + m_minX.ToString());
            EditorGUILayout.LabelField("max: " + m_maxX.ToString());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref m_minX, ref m_maxX, -180, 180);
        }
        EditorGUILayout.EndToggleGroup();

        m_ToggleYConstraint.boolValue = EditorGUILayout.BeginToggleGroup("toggle Y constraint", m_ToggleYConstraint.boolValue);
        if (m_ToggleYConstraint.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("min: " + m_minY.ToString());
            EditorGUILayout.LabelField("max: " + m_maxY.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.MinMaxSlider(ref m_minY, ref m_maxY, -180, 180);
        }
        EditorGUILayout.EndToggleGroup();


        if (m_ToggleYConstraint.boolValue && m_ToggleZConstraint.boolValue)
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

        m_XConstraint = m_ROMProp.FindPropertyRelative("PitchMinMax");
        m_YConstraint = m_ROMProp.FindPropertyRelative("YawMinMax");
        m_ZConstraint = m_ROMProp.FindPropertyRelative("RollMinMax");

        m_ToggleXConstraint = serializedObject.FindProperty("ToggleXConstraint");
        m_ToggleYConstraint = serializedObject.FindProperty("ToggleYConstraint");
        m_ToggleZConstraint = serializedObject.FindProperty("ToggleZConstraint");


        //ZMaxRadius = serializedObject.FindProperty("ZMaxRadius");
        //ZMinRadius = serializedObject.FindProperty("ZMinRadius");

    }

    #endregion

    #region debugDraw
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

    private void DrawMainOrientation()
    {
        //Handles.color = Color.black;
        if (m_IsInBounds)
            Handles.color = Color.cyan; // in z bounds
        else
            Handles.color = Color.black; // not z bounds
        Handles.ArrowCap(3, m_positionGizmo, m_localRotation * Quaternion.Euler(0, 90, 0), m_arrowSize);

//         float t_RollAngle = m_simpleROMMB.rom.RollMinMax.lastCompute;
//         Handles.color = m_angleCol;
//         Vector3 axis = m_localRotation * (Vector3.right * m_offsetSize);
// 
//         Vector3 t_up;
//         Vector3 t_orthoVec;
//         if (Mathf.Sin(Vector3.Angle(axis, Vector3.up) * Mathf.Deg2Rad) > 0.1f)
//         {
//             t_orthoVec = Vector3.Cross(axis, Vector3.up);
//             t_up = Vector3.Cross(t_orthoVec, axis);
//         }
//         else
//         {
//             t_orthoVec = Vector3.Cross(axis, Vector3.forward);
//             t_up = Vector3.Cross(axis, t_orthoVec);
//         }
// 
//         Handles.DrawSolidArc(m_positionGizmo + axis, axis, t_orthoVec, t_RollAngle, m_radiusSize);
    }

    private void DrawCustomGizmos()
    {
        Handles.color = m_sphereCol;
        Handles.SphereCap(4, m_positionGizmo, Quaternion.identity, m_arrowSize*2);

        if(m_drawQuatAxis)
            DrawAngleAxis(m_simpleROMMB.transform.position);

        DrawConstraints(m_simpleROMMB.transform.parent == null ? Quaternion.identity : m_simpleROMMB.transform.parent.rotation);

        DrawMainOrientation();

        if (m_drawlocalAxis)
        {
            Handles.color = Handles.xAxisColor;
            Handles.ArrowCap(3, m_simpleROMMB.transform.position, m_localRotation * Quaternion.Euler(0, 90, 0), m_arrowSize / 2);
            Handles.color = Handles.yAxisColor;
            Handles.ArrowCap(3, m_simpleROMMB.transform.position, m_localRotation * Quaternion.Euler(-90, 0, 0), m_arrowSize / 2);
            Handles.color = Handles.zAxisColor;
            Handles.ArrowCap(3, m_simpleROMMB.transform.position, m_localRotation * Quaternion.Euler(0, 0, 90), m_arrowSize / 2);
        }
    }
    
    #endregion

    #region Constraints
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
        if(m_drawMinMaxLinks)
        {
            Handles.color = Color.gray;
            Handles.DrawLine(localEnd, m_positionGizmo + YaZaVec * m_arrowSize);
            Handles.DrawLine(localEnd, m_positionGizmo + YaZiVec * m_arrowSize);
            Handles.DrawLine(localEnd, m_positionGizmo + YiZaVec * m_arrowSize);
            Handles.DrawLine(localEnd, m_positionGizmo + YiZiVec * m_arrowSize);
        }

        Vector3 endToYaZa = (m_positionGizmo + YaZaVec * m_arrowSize) - localEnd;
        Vector3 endToYiZa = (m_positionGizmo + YiZaVec * m_arrowSize) - localEnd;

        Vector3 endToYaZi = (m_positionGizmo + YaZiVec * m_arrowSize) - localEnd;
        Vector3 endToYiZi = (m_positionGizmo + YiZiVec * m_arrowSize) - localEnd;
        //if(ApproxZConstraint(endToYiZa, endToYaZa, endToYiZi, endToYaZi, localAxe))
        if(CheckHorizConstraint())
        {
            if (CheckVertConstraint(endToYiZa, endToYaZa, endToYiZi, endToYaZi, localAxe, localEnd))
                m_IsInBounds = true;
            else
                m_IsInBounds = false;
        }
        else
            m_IsInBounds = false;

        if(m_drawDecomposition)
            DrawDecomposition(localAxe);
        
    }
       
    private bool CheckVertConstraint(Vector3 endToYiZa, Vector3 endToYaZa, Vector3 endToYiZi, Vector3 endToYaZi, Vector3 localAxe, Vector3 localEnd)
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

    float m_RadiusProjOnConeMax;
    float m_RadiusProjOnConeMin;
    //float dist;
    float m_RadiusConeMax ;
    float m_RadiusConeMin;

    //Vector3 m_ConeCrossProd; Color m_ConeCrossProdCol = Color.yellow;
    //Vector3 m_crossProjMin ; Color m_crossProjMinCol  = Color.magenta;
    //Vector3 m_crossProjMax;  Color m_crossProjMaxCol  = Color.red;
    //Vector3 m_crosslocalMin; Color m_crosslocalMinCol = Color.white;
    //Vector3 m_crosslocalMax; Color m_crosslocalMaxCol = Color.gray;


    private void drawArrow(Vector3 a_vec, Color? a_col = null, float sizeoffset = 1.0f)
    {
        if (a_vec.magnitude < 0.01f)
            return;

        if(a_col.HasValue)
            Handles.color = a_col.Value;

        Quaternion tprojQuat = Quaternion.LookRotation(a_vec.normalized, m_localRotation * Vector3.up);
        Handles.ArrowCap(55, m_positionGizmo, tprojQuat, m_arrowSize * sizeoffset);
    }


    class HorizResult
    {
        public Color color = Color.clear;
        public Vector3 proj = Vector3.zero;
        public float radius = 0.0f;
        public bool ZinBound = false;
    }

    private HorizResult HorizBothPosBounds(Vector3 constraintUp, Vector3 tvert, Vector3 tFlat, Vector3 t_AxeProjOnConeMin, Vector3 t_AxeProjOnConeMax)
    {
        HorizResult tRes = new HorizResult();
        if (tvert.normalized != constraintUp)
        {
            tRes.color = Color.magenta;
            tRes.proj = -tvert + tFlat.normalized * m_RadiusProjOnConeMin;
            tRes.radius = m_RadiusProjOnConeMin;
            tRes.ZinBound = false;
        }
        else
        {

            if (m_RadiusProjOnConeMax > m_RadiusConeMax)
            {
                tRes.color = Color.red;
                tRes.proj = t_AxeProjOnConeMax;
                tRes.radius = m_RadiusProjOnConeMax;
                tRes.ZinBound = false;
            }
            else if (m_RadiusConeMin < 0 && m_RadiusProjOnConeMin < m_RadiusConeMin)
            {
                tRes.color = Color.black;
                tRes.proj = t_AxeProjOnConeMin;
                tRes.radius = m_RadiusProjOnConeMin;
                tRes.ZinBound = false;
            }
            else if (m_RadiusConeMin > 0 && m_RadiusProjOnConeMin < m_RadiusConeMin)
            {
                tRes.color = Color.grey;
                tRes.proj = t_AxeProjOnConeMin;
                tRes.radius = m_RadiusProjOnConeMin;
                tRes.ZinBound = false;
            }
            else
            {
                if (m_RadiusProjOnConeMax > 0)
                {
                    tRes.color = Color.cyan;
                    tRes.proj = t_AxeProjOnConeMax;
                    tRes.radius = m_RadiusProjOnConeMax;
                    tRes.ZinBound = true;
                }
                else if (m_RadiusProjOnConeMin < 0)
                {
                    tRes.color = Color.green;
                    tRes.proj = t_AxeProjOnConeMin;
                    tRes.radius = m_RadiusProjOnConeMin;
                    tRes.ZinBound = true;
                }
            }
        }
        return tRes;
    }

    private HorizResult HorizHalfPosBounds(Vector3 constraintUp, Vector3 tvert, Vector3 tFlat, Vector3 t_AxeProjOnConeMin, Vector3 t_AxeProjOnConeMax)
    {
        HorizResult tRes = new HorizResult();

        if (tvert.normalized != constraintUp) // bottom side
        {
            if (m_RadiusProjOnConeMin < m_RadiusConeMin)
            {
                tRes.color = Color.black;
                tRes.proj = tvert - tFlat.normalized * m_RadiusProjOnConeMin; //t_AxeProjOnConeMin;
                tRes.radius = m_RadiusProjOnConeMin;
                tRes.ZinBound = false;
            }
            else
            {
                tRes.color = Color.green;
                tRes.proj = tvert - tFlat.normalized * m_RadiusProjOnConeMin; //t_AxeProjOnConeMin;
                tRes.radius = m_RadiusProjOnConeMin;
                tRes.ZinBound = true;
            }
        }
        else // upper side
        {
            if (m_RadiusProjOnConeMax > m_RadiusConeMax)
            {
                tRes.color = Color.red;
                tRes.proj = t_AxeProjOnConeMax;
                tRes.radius = m_RadiusProjOnConeMax;
                tRes.ZinBound = false;
            }
            else
            {
                tRes.color = Color.cyan;
                tRes.proj = t_AxeProjOnConeMax;
                tRes.radius = m_RadiusProjOnConeMax;
                tRes.ZinBound = true;
            }
        }
        return tRes;
    }

    private HorizResult HorizBothNegBounds(Vector3 constraintUp, Vector3 tvert, Vector3 tFlat, Vector3 t_AxeProjOnConeMin, Vector3 t_AxeProjOnConeMax)
    {
        HorizResult tRes = new HorizResult();
        if (tvert.normalized == constraintUp) // upper side always false as both constraints are negatives
        {
            tRes.color = Color.magenta;
            tRes.proj = -t_AxeProjOnConeMax;
            tRes.radius = m_RadiusProjOnConeMin;
            tRes.ZinBound = false;
        }
        else
        {
            if (m_RadiusProjOnConeMax > m_RadiusConeMax) // negative part above max
            {
                tRes.color = Color.red;
                tRes.proj = tvert - tFlat.normalized * m_RadiusProjOnConeMax;
                tRes.radius = m_RadiusProjOnConeMax;
                tRes.ZinBound = false;
            }
            else if (m_RadiusProjOnConeMin < m_RadiusConeMin) // negative part under min
            {
                tRes.color = Color.black;
                tRes.proj = tvert - tFlat.normalized * m_RadiusProjOnConeMin;
                tRes.radius = m_RadiusProjOnConeMin;
                tRes.ZinBound = false;
            }
            else //if (m_RadiusProjOnConeMin < 0)
            {
                tRes.color = Color.green;
                tRes.proj = tvert - tFlat.normalized * m_RadiusProjOnConeMin;
                tRes.radius = m_RadiusProjOnConeMin;
                tRes.ZinBound = true;

            }
        }
        return tRes;
    }

    private bool CheckHorizConstraint()
    {
        //bool ZinBound = false;

        Vector3 localAxe = m_localRotation * Vector3.right;
        localAxe.Normalize();
        Vector3 constraintUp = Vector3.up;

        Vector3 tFlat = Vector3.ProjectOnPlane(localAxe, constraintUp);
        Vector3 tvert = localAxe - tFlat;
        Handles.color = Color.black;
        Handles.DrawLine(m_positionGizmo, m_positionGizmo + tvert * m_arrowSize);
        Handles.DrawLine(m_positionGizmo + tvert * m_arrowSize, m_positionGizmo + tvert * m_arrowSize + tFlat * m_arrowSize);

        //float currentZ = m_localRotation.eulerAngles.z;
        float med = m_minZ + (m_maxZ - m_minZ) * 0.5f;


        Vector3 ConeMax = Quaternion.Euler(0, 0, m_maxZ) * Vector3.right;
        Vector3 ConeMin = Quaternion.Euler(0, 0, m_minZ) * Vector3.right;
        //Vector3 proj = Vector3.zero;
        //float radius = 0;

        //m_ConeCrossProd = Vector3.Cross(ConeMax, ConeMin).normalized;

        m_RadiusConeMax = (m_maxZ > 0 ? 1 : -1) * Mathf.Sin((90 - m_maxZ) * Mathf.Deg2Rad);
        m_RadiusConeMin = (m_minZ > 0 ? 1 : -1) * Mathf.Sin((90 - m_minZ) * Mathf.Deg2Rad);

        m_RadiusProjOnConeMax = tvert.magnitude * Mathf.Tan((90 - m_maxZ) * Mathf.Deg2Rad);
        m_RadiusProjOnConeMin = tvert.magnitude * Mathf.Tan((90 - m_minZ) * Mathf.Deg2Rad);


        Vector3 tConeCenter = tvert;

        Vector3 t_AxeProjOnConeMax = tvert + tFlat.normalized * m_RadiusProjOnConeMax;
        Vector3 t_AxeProjOnConeMin = tvert + tFlat.normalized * m_RadiusProjOnConeMin;

        HorizResult tRes = new HorizResult();
        if (m_maxZ > 0 && m_minZ >= 0)
            tRes = HorizBothPosBounds(constraintUp, tvert, tFlat, t_AxeProjOnConeMin, t_AxeProjOnConeMax);
        else if(m_maxZ > 0 && m_minZ < 0)
            tRes = HorizHalfPosBounds(constraintUp, tvert, tFlat, t_AxeProjOnConeMin, t_AxeProjOnConeMax);
        else if(m_maxZ <= 0 && m_minZ < 0)
            tRes = HorizBothNegBounds(constraintUp, tvert, tFlat, t_AxeProjOnConeMin, t_AxeProjOnConeMax);

        Handles.color = tRes.color;

        Vector3 startLine = m_positionGizmo + localAxe * m_arrowSize;
        Vector3 startWire = m_positionGizmo;
        startWire.y += localAxe.y * m_arrowSize;


        Handles.DrawLine(startLine, m_positionGizmo + tRes.proj * m_arrowSize);
        Handles.DrawWireDisc(startWire, Vector3.up, tRes.radius * m_arrowSize);
        Handles.color = Handles.zAxisColor ;
        startWire = m_positionGizmo + (Vector3.up * Mathf.Sin(m_maxZ * Mathf.Deg2Rad)) * m_arrowSize;
        Handles.DrawWireDisc(startWire, Vector3.up, m_RadiusConeMax * m_arrowSize);
        startWire = m_positionGizmo + (Vector3.up * Mathf.Sin(m_minZ * Mathf.Deg2Rad)) * m_arrowSize;
        Handles.DrawWireDisc(startWire, Vector3.up, m_RadiusConeMin * m_arrowSize);

        if (m_drawProj && tRes.proj != Vector3.zero)
        {
            Quaternion tprojQuat = Quaternion.LookRotation(tRes.proj.normalized, m_localRotation * Vector3.up);
            Handles.color = Handles.xAxisColor;
            Handles.ArrowCap(55, m_positionGizmo + tRes.proj * m_arrowSize, tprojQuat, m_arrowSize / 2);
            Handles.ArrowCap(55, m_positionGizmo , tprojQuat, m_arrowSize);
            Handles.DrawLine(m_positionGizmo, m_positionGizmo + tRes.proj * m_arrowSize);
            Handles.color = Handles.yAxisColor;
            Handles.ArrowCap(55, m_positionGizmo + tRes.proj * m_arrowSize, tprojQuat * Quaternion.Euler(-90, 0, 0), m_arrowSize / 2);
            Handles.color = Handles.zAxisColor;
            Handles.ArrowCap(55, m_positionGizmo + tRes.proj * m_arrowSize, tprojQuat * Quaternion.Euler(0, -90, 0), m_arrowSize / 2);
        }

        return tRes.ZinBound;
    }

    private void DrawDecomposition(Vector3 localAxe)
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
        Handles.DrawLine(m_positionGizmo, m_positionGizmo + YaPerp * m_arrowSize);
        Handles.DrawLine(m_positionGizmo + YaPerp * m_arrowSize, m_positionGizmo + YaPerp * m_arrowSize + YaPar * m_arrowSize);

        //Handles.color = Color.green * 0.7f;
        Handles.DrawLine(m_positionGizmo, m_positionGizmo + YiPerp * m_arrowSize);
        Handles.DrawLine(m_positionGizmo + YiPerp * m_arrowSize, m_positionGizmo + YiPerp * m_arrowSize + YiPar * m_arrowSize);
    }

    #endregion

}
