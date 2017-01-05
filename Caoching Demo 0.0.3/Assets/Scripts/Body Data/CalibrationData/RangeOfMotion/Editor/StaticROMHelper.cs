﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(StaticRomMB))]
public class StaticROMHelper : Editor
{
    #region SerializedProperty 
    //SerializedProperty m_ROMProp;
    #endregion

    #region usefull member
    bool[] squel_draw = null;
    StaticRomMB staticMB;
    #endregion


    #region Editor

    void OnSceneGUI()
    {
        staticMB = target as StaticRomMB;
        for (int i = 0; i < staticMB.subSegment.Length; ++i)
        {
            Quaternion tQuat = staticMB.subSegment[i].localRotation; // local copy
            staticMB.subSegment[i].localRotation = staticMB.ROM.capRotation((BodyStructureMap.SegmentTypes)(i / 2), (BodyStructureMap.SubSegmentTypes)i, staticMB.subSegment[i], ref tQuat, true, false);
        }
    }

    static
    public void OnInspectorAngleConstraint( AngleConstraint ac, string AxisLabel)
    {
        ac.axe = EditorGUILayout.Vector3Field(AxisLabel, ac.axe);
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("min: " + ac.minAngle.ToString());
        EditorGUILayout.LabelField("max: " + ac.maxAngle.ToString());
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.MinMaxSlider(ref ac.minAngle, ref ac.maxAngle, -180, 180);
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }


    private void OnInspectorSquelette()
    {
        for (int i = 0; i < staticMB.ROM.squeletteRom.Length; ++i)
        {
            SimpleROM t = staticMB.ROM.squeletteRom[i];
            if (squel_draw[i] = EditorGUILayout.Foldout(squel_draw[i], t.Name))
            {
                EditorGUI.indentLevel++;
                OnInspectorAngleConstraint(t.PitchMinMax, "Pitch axis");
                OnInspectorAngleConstraint(t.YawMinMax, "Yaw axis");
                OnInspectorAngleConstraint(t.RollMinMax, "Roll axis");
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Separator();
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        staticMB = target as StaticRomMB;

        EditorGUILayout.ObjectField("static ROM", staticMB.ROM, typeof(StaticROM), true);
        if (GUILayout.Button("Reset All"))
        {
            staticMB.ROM.ResetAll();
        }
        EditorGUILayout.Separator();

        //OnInspectorSquelette(staticMB);
        OnInspectorSquelette();

        serializedObject.ApplyModifiedProperties();
    }

    public void OnEnable()
    {
        staticMB = target as StaticRomMB;
        //m_ROMProp = serializedObject.FindProperty("ROM");
        if (staticMB.ROM == null)
        {
            serializedObject.Update();
            staticMB.Init();
            //StaticROM t_rom = new StaticROM();
            //m_ROMProp = serializedObject.FindProperty("ROM");
            //m_ROMProp.objectReferenceValue = t_rom;

            serializedObject.ApplyModifiedProperties();
        }

        if (staticMB.subSegment[0] == null)
        {
            staticMB.PopulateSubSegment();
        }

        if(squel_draw == null)
        {
            squel_draw = new bool[staticMB.ROM.squeletteRom.Length];
            for (int i = 0; i < squel_draw.Length; ++i)
            {
                squel_draw[i] = false;
            }
        }
    }

    #endregion





}
