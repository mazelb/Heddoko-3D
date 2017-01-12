﻿using UnityEngine;
using System.Collections;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion;
using UnityEditor;

//[RequireComponent(typeof(RenderedBody))]
//[ExecuteInEditMode]
public class StaticRomMB : MonoBehaviour
{
    public StaticROM ROM = null;// new StaticROM();
	//private RenderedBody rendbod = null;

	public Transform[] subSegment = new Transform[10];


	public void Init()
	{
        ROM = new StaticROM();
        ROM.mFlags = new BodyFlags();
        ROM.mFlags.IsAdjustingSegmentAxis = false;

        PopulateSubSegment();
	}
 
	void OnEnable()
	{
		Init();
	}

	// Use this for initialization
	void Start ()
	{
		Init();

	}
	
	// Update is called once per frame
	void Update ()
	{
		for (int i = 0; i < subSegment.Length; ++i)
		{
			Quaternion tQuat = subSegment[i].localRotation; // local copy
            subSegment[i].localRotation = ROM.capRotation((BodyStructureMap.SegmentTypes) (i/2), (BodyStructureMap.SubSegmentTypes)i, subSegment[i], ref tQuat, true);
		}
	}

	public void PopulateSubSegment()
	{
        int i = 0;
        if(subSegment[i++] == null)
		    subSegment[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine] = transform.Find("Beta:Hips/Beta:Spine/Beta:Spine1");
        if(subSegment[i++] == null)
            subSegment[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine] = transform.Find("Beta:Hips/Beta:Spine");
        if(subSegment[i++] == null)
            subSegment[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm] = transform.Find("Beta:Hips/Beta:Spine/Beta:Spine1/Beta:Spine2/Beta:RightShoulder/Beta:RightArm");
        if(subSegment[i++] == null)
            subSegment[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm] = transform.Find("Beta:Hips/Beta:Spine/Beta:Spine1/Beta:Spine2/Beta:RightShoulder/Beta:RightArm/Beta:RightForeArm");
        if(subSegment[i++] == null)
            subSegment[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm] = transform.Find("Beta:Hips/Beta:Spine/Beta:Spine1/Beta:Spine2/Beta:LeftShoulder/Beta:LeftArm");
        if (subSegment[i++] == null)
            subSegment[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm] = transform.Find("Beta:Hips/Beta:Spine/Beta:Spine1/Beta:Spine2/Beta:LeftShoulder/Beta:LeftArm/Beta:LeftForeArm");
        if(subSegment[i++] == null)
            subSegment[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh] = transform.Find("Beta:Hips/Beta:RightUpLeg");
        if(subSegment[i++] == null)
            subSegment[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf] = transform.Find("Beta:Hips/Beta:RightUpLeg/Beta:RightLeg");
        if(subSegment[i++] == null)
            subSegment[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh] = transform.Find("Beta:Hips/Beta:LeftUpLeg");
        if(subSegment[i++] == null)
            subSegment[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf] = transform.Find("Beta:Hips/Beta:LeftUpLeg/Beta:LeftLeg");
	}

    public void IsAdjustingSegmentAxis(bool val)
    {
        ROM.mFlags.IsAdjustingSegmentAxis = val;
    }
    
}