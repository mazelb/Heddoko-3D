
/** 
 * @file BodySegment.cs
 * @brief Contains the BodySegment  class
 * @author Mohammed Haider(mohamed@heddoko.com)
 * @date October 2015
 * @refactor kaltenmark J�r�mie (kaltenmark.jeremie@cdrin.com)
 * @date septembre 2016
 * Copyright Heddoko(TM) 2015, all rights reserved
 */

#if CDRIN_BODY_SEGMENTS
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Data.CalibrationData;
using Assets.Scripts.Body_Data.CalibrationData.RangeOfMotion;
using Assets.Scripts.Body_Data.view;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.Body_Pipeline.Analysis;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Torso;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BodyFlags
{
    public BodyFlags() { }
    public BodyFlags(bool setall)
    {
        IsTrackingHeight = setall;
        IsTrackingGait = setall;
        IsTrackingHips = setall;
        IsProjectingXZ = setall;
        IsProjectingXY = setall;
        IsProjectingYZ = setall;
        IsHipsEstimateForward = setall;
        IsHipsEstimateUp = setall;
        IsUsingInterpolation = setall;
        IsCalibrating = setall;
        IsAdjustingSegmentAxis = setall;
        IsFusingSubSegments = setall;
    }
	private BodyFlags(BodyFlags aBF)
	{
		IsTrackingHeight = aBF.IsTrackingHeight;
		IsTrackingGait = aBF.IsTrackingGait;
		IsTrackingHips = aBF.IsTrackingHips;
		IsProjectingXZ = aBF.IsProjectingXZ;
		IsProjectingXY = aBF.IsProjectingXY;
		IsProjectingYZ = aBF.IsProjectingYZ;
		IsHipsEstimateForward = aBF.IsHipsEstimateForward;
		IsHipsEstimateUp = aBF.IsHipsEstimateUp;
		IsUsingInterpolation = aBF.IsUsingInterpolation;
		IsCalibrating = aBF.IsCalibrating;
		IsAdjustingSegmentAxis = aBF.IsAdjustingSegmentAxis;
		IsFusingSubSegments = aBF.IsFusingSubSegments;
	}
	
	private BodyFlags(	bool aIsTrackingHeight, bool aIsTrackingGait, bool aIsTrackingHips,
						bool aIsProjectingXZ, bool aIsProjectingXY, bool aIsProjectingYZ, 
						bool aIsHipsEstimateForward, bool aIsHipsEstimateUp, bool aIsUsingInterpolation,
						bool aIsCalibrating, bool aIsAdjustingSegmentAxis, bool aIsFusingSubSegments )
	{
		IsTrackingHeight = aIsTrackingHeight;
		IsTrackingGait = aIsTrackingGait;
		IsTrackingHips = aIsTrackingHips;
		IsProjectingXZ = aIsProjectingXZ;
		IsProjectingXY = aIsProjectingXY;
		IsProjectingYZ = aIsProjectingYZ;
		IsHipsEstimateForward = aIsHipsEstimateForward;
		IsHipsEstimateUp = aIsHipsEstimateUp;
		IsUsingInterpolation = aIsUsingInterpolation;
		IsCalibrating = aIsCalibrating;
		IsAdjustingSegmentAxis = aIsAdjustingSegmentAxis;
		IsFusingSubSegments = aIsFusingSubSegments;
	}

	public bool IsTrackingHeight = true;
    public bool IsTrackingGait = false;
    public bool IsTrackingHips = false;
    public bool IsProjectingXZ = false;
    public bool IsProjectingXY = false;
    public bool IsProjectingYZ = false;
    public bool IsHipsEstimateForward = true;
    public bool IsHipsEstimateUp = false;
    public bool IsUsingInterpolation = true;
    public bool IsCalibrating = true;
    public bool IsAdjustingSegmentAxis = true;
    public bool IsFusingSubSegments = true;

    public BodyFlags clone()
    {
        return new BodyFlags(	IsTrackingHeight, IsTrackingGait, IsTrackingHips, 
        						IsProjectingXZ, IsProjectingXY, IsProjectingYZ, 
        						IsHipsEstimateForward, IsHipsEstimateUp, IsUsingInterpolation,
        						IsCalibrating, IsAdjustingSegmentAxis, IsFusingSubSegments );
    }
}


/// <summary>
/// BodySegment class: represents one abstracted reprensentation of a body segment.
/// </summary>
public class BodySegment
{
    
    public BodyFrameCalibrationContainer BodyFrameCalibrationContainer { get; internal set; }
	//Segment Type 
	public BodyStructureMap.SegmentTypes SegmentType;
	public static bool GBodyFrameUsingQuaternion = false;

	//Body SubSegments 
	public Dictionary<int, BodySubSegment> BodySubSegmentsDictionary = new Dictionary<int, BodySubSegment>();
	private BodyFrameCalibrationContainer mBodyFrameCalibrationContainer;
	public Body ParentBody;

    private StaticROM mROM;

    //Is segment tracked (based on body type) 
    public bool IsTracked = true;
    public bool IsReseting = false;
    // 	static public bool Flags.IsTrackingHeight = true;
    // 	static public bool Flags.IsTrackingGait = false;
    // 	static public bool Flags.IsTrackingHips = false;
    // 	static public bool Flags.IsProjectingXZ = false;
    // 	static public bool Flags.IsProjectingXY = false;
    // 	static public bool Flags.IsProjectingYZ = false;
    // 	static public bool Flags.IsHipsEstimateForward = true;
    // 	static public bool Flags.IsHipsEstimateUp = false;
    // 	static public bool Flags.IsUsingInterpolation =  true;
    // 	static public bool Flags.IsCalibrating = true;
    // #if SEGMENTS_DEBUG
    // 	static public bool Flags.IsAdjustingSegmentAxis = false;
    // 	static public bool IsAdjustingArms;
    // 	static public bool Flags.IsFusingSubSegments = false;
    // 	static public bool Flags.IsCalibrating = true;
    // #else
    // 	static public bool Flags.IsAdjustingSegmentAxis = true;
    // 	static public bool Flags.IsFusingSubSegments = true;
    // #endif

    static public BodyFlags Flags = new BodyFlags(false);


    public int ResetCounter = 0;
	static public float InterpolationSpeed = 0.3f;
	//Extract the delta time of the frames
	public float LastFrameTime = 0.0f;
	public float CurrentFrameTime = 0.0f;
	public float DeltaTime = 0.0f;

	//Sensor data tuples
	private List<SensorTuple> SensorsTuple = new List<SensorTuple>();

	//Associated view for the segment
	public BodySegmentView AssociatedView;
	//Analysis pipeline of the segment data
	public SegmentAnalysis mCurrentAnalysisSegment;

	//Detection of vertical Hip position
	private static Vector3 mHipDisplacement = new Vector3(0,0.95f,0);
	private static float mRightLegHeight = 0.95f;
	private static float mLeftLegHeight = 0.95f;
	private static float mInitialLegHeight = 0.95f;
	private static Vector3 mRightLegStride = Vector3.zero;
	private static Vector3 mLeftLegStride = Vector3.zero;
	private Vector3 mUACurInitRotation = Vector3.zero;
	private Vector3 mLACurInitRotation = Vector3.zero;
	private Vector3 mULCurInitRotation = Vector3.zero;
	private Vector3 mLLCurInitRotation = Vector3.zero;
	private Vector3 mUTCurInitRotation = Vector3.zero;

	static int svRightCounter = 0;
	static int svLeftCounter = 0;
	static int svStandCounter = 0;
#if SEGMENTS_DEBUG
	//*** Definitions for calibration methods
	// Filename to write to.
	private string mRecordingFileName = "";

	// File stream to write to.
	private TextWriter mRecordingFileStream;
	// Title to append to file.
	public string mRecordingTitle = "data_set";

	// Interval at which to collect data, in milliseconds. Set to zero to ignore.
	public int mRecordingFrameInterval = 0;
	//   ******** 8th try **** 
	//   ******** Soldier Pose **** 
	Vector3 vRLAAxisRightSoldier = new Vector3(0f, 0f, 0f);
	Vector3 vRLAAxisUpSoldier = new Vector3(0f, 0f, 0f);
	Vector3 vRLAAxisFwdSoldier = new Vector3(0f, 0f, 0f);
	Vector3 vRLAAxisUpSoldierSum = new Vector3(0f, 0f, 0f);
	Vector3 vRLAAxisRightSoldierSum = new Vector3(0f, 0f, 0f);
	Vector3 vRUAAxisRightSoldier = new Vector3(0f, 0f, 0f);
	Vector3 vRUAAxisUpSoldier = new Vector3(0f, 0f, 0f);
	Vector3 vRUAAxisFwdSoldier = new Vector3(0f, 0f, 0f);
	//Right lower arm
	Quaternion vRLAQuatRightsSoldier = Quaternion.identity;
	Quaternion vRLAQuatUpsSoldier = Quaternion.identity;
	Quaternion vRLAQuatforwardsSoldier = Quaternion.identity;
	//Right uper arm
	Quaternion vRUAQuatRightsSoldier = Quaternion.identity;
	Quaternion vRUAQuatUpsSoldier = Quaternion.identity;
	Quaternion vRUAQuatforwardsSoldier = Quaternion.identity;
	//Left lower arm
	Quaternion vLLAQuatRightsSoldier = Quaternion.identity;
	Quaternion vLLAQuatUpsSoldier = Quaternion.identity;
	//Left uper arm
	Quaternion vLUAQuatRightsSoldier = Quaternion.identity;
	Quaternion vLUAQuatUpsSoldier = Quaternion.identity;
	//   ******** Zombie Pose **** 
	Vector3 vRLAAxisRightTPose = new Vector3(0f, 0f, 0f);
	Vector3 vRLAAxisUpTPose = new Vector3(0f, 0f, 0f);
	Vector3 vRUAAxisRightTPose = new Vector3(0f, 0f, 0f);
	Vector3 vRUAAxisUpTPose = new Vector3(0f, 0f, 0f);
	Vector3 vRLAAxisRightZombie = new Vector3(0f, 0f, 0f);
	Vector3 vRLAAxisUpZombie = new Vector3(0f, 0f, 0f);
	Vector3 vRLAAxisUpZombieSum = new Vector3(0f, 0f, 0f);
	Vector3 vRLAAxisRightZombieSum = new Vector3(0f, 0f, 0f);
	Vector3 vRUAAxisRightZombie = new Vector3(0f, 0f, 0f);
	Vector3 vRUAAxisUpZombie = new Vector3(0f, 0f, 0f);

	Vector3 vLLAAxisRightTPose = new Vector3(0f, 0f, 0f);
	Vector3 vLLAAxisRightZombie = new Vector3(0f, 0f, 0f);
	Vector3 vLLAAxisRightZombieSum = new Vector3(0f, 0f, 0f);
	Vector3 vLLAAxisUpZombie = new Vector3(0f, 0f, 0f);
	Vector3 vLLAAxisUpZombieSum = new Vector3(0f, 0f, 0f);
	Vector3 vLUAAxisRightZombie = new Vector3(0f, 0f, 0f);
	Vector3 vLUAAxisUpZombie = new Vector3(0f, 0f, 0f);

	//Right lower arm
	Quaternion vRLAQuatRightsZombie = Quaternion.identity;
	Quaternion vRLAQuatUpsZombie = Quaternion.identity;
	//Right uper arm
	Quaternion vRUAQuatRightsZombie = Quaternion.identity;
	Quaternion vRUAQuatUpsZombie = Quaternion.identity;
	//Left lower arm
	Quaternion vLLAQuatRightsZombie = Quaternion.identity;
	Quaternion vLLAQuatUpsZombie = Quaternion.identity;
	//Left uper arm
	Quaternion vLUAQuatRightsZombie = Quaternion.identity;
	Quaternion vLUAQuatUpsZombie = Quaternion.identity;

	////////************  6th try  ////************
	public Quaternion vUpArmQuatSaved = Quaternion.identity;
	public Quaternion vLoArmQuatSaved = Quaternion.identity;
	public Quaternion vUpArmQuatSaved1 = Quaternion.identity;
	public Quaternion vLoArmQuatSaved1 = Quaternion.identity;
	public Quaternion vUpArmQuatSavedNew = Quaternion.identity;
	public Quaternion vLoArmQuatSavedNew = Quaternion.identity;
	int counterR = 0;
	int counterL = 0;
#endif

/// <summary>
	/// The function will update the sensors data with the passed in BodyFrame. Iterates through the list of sensor tuples and updates the current sensor's information
	/// </summary>
	/// <param name="vFrame"></param>
	public void UpdateSensorsData(BodyFrame vFrame)
	{
        if (vFrame == null)
            return;

		//Update the delta time
		CurrentFrameTime = vFrame.Timestamp;
		DeltaTime = CurrentFrameTime - LastFrameTime;
		LastFrameTime = CurrentFrameTime;

		//get the sensor 
		List<BodyStructureMap.SensorPositions> vSensorPos = BodyStructureMap.Instance.SegmentToSensorPosMap[SegmentType];

		//get subframes of data relevant to this body segment 
		foreach (BodyStructureMap.SensorPositions vSensorPosKey in vSensorPos)
		{
			//find a suitable sensor to update
			SensorTuple vSensTuple = SensorsTuple.First(a => a.CurrentSensor.SensorPosition == vSensorPosKey);

			//get the relevant data from vFrame 
			if (vFrame.FrameData.ContainsKey(vSensorPosKey))
			{
				BodyFrame.Vect4 vFrameData = vFrame.FrameData[vSensorPosKey];
				vSensTuple.CurrentSensor.SensorData.PositionalData = vFrameData;
			}
		}
	}

	/// <summary>
	/// UpdateSegment: Depending on the segment type, apply transformation matrices.
	/// </summary>
	/// <param name="vFilteredDictionary">Dictionnary of tracked segments and their transformations.</param>
	internal void UpdateSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vFilteredDictionary)
	{
		MapSubSegments(vFilteredDictionary);
	}

	/// <summary>
	/// UpdateInitialSensorsData: The function will update the sensors data with the passed in BodyFrame. Iterates through the list of sensor tuples and updates the initial sensor's information.
	/// </summary>
	/// <param name="vFrame">the body frame whose subframes will updates to initial sensors.</param>
	/// <param name="vBodyCalibrationSetting">optional parameter to set the current calibration setting</param>
	public void UpdateInitialSensorsData(BodyFrame vFrame)     ///*************************/////
	{
        if (vFrame == null)
            return;

		IsReseting = true;
		List<BodyStructureMap.SensorPositions> vSensorPos = BodyStructureMap.Instance.SegmentToSensorPosMap[SegmentType];
		foreach (BodyStructureMap.SensorPositions vPos in vSensorPos)
		{
			//find a suitable sensor to update
			SensorTuple vSensTuple = SensorsTuple.First(a => a.InitSensor.SensorPosition == vPos);

			//get the relevant data from vFrame 
			if (vFrame.FrameData.ContainsKey(vPos))
			{
				vSensTuple.InitSensor.SensorData.PositionalData = vFrame.FrameData[vPos];
				int vKey = (int)vSensTuple.InitSensor.SensorPosition;

				if (vSensTuple.InitSensor.SensorType == BodyStructureMap.SensorTypes.ST_Biomech)
				{
					//get the subsegment and update its  inverse initial orientation 
					if (BodySubSegmentsDictionary.ContainsKey(vKey))
					{
						BodySubSegmentsDictionary[vKey].ResetViewTransforms();
					}
				}
			}
		}
		mCurrentAnalysisSegment.ResetMetrics();
	}

	/// <summary>
	/// Resets metrics to their initial values
	/// </summary>
	public void ResetMetrics()
	{
		mCurrentAnalysisSegment.ResetMetrics();
	}


	/// <summary>
	/// Updates the vertical position of the Hips. TODO: move this to appropriate place
	/// </summary>
	internal void UpdateHipPosition(BodySubSegment vSegment)
	{
		//Update body positions
		Vector3 vDisplacement = Vector3.zero;// mHipDisplacement;
		float vHeightDiff = mRightLegHeight - mLeftLegHeight;

		if (Mathf.Abs(vHeightDiff) < 0.1f)
		{
			//Debug.Log("!!!!!  STAND !!!!!!! " + svStandCounter);
			svStandCounter++;

			//Standing position
			if(Flags.IsTrackingHeight)
			{
				vDisplacement.y = mRightLegHeight;
			}
			if(Flags.IsTrackingHips)
			{
				//vDisplacement.x = mRightLegStride.x;
				vDisplacement.z = -mRightLegStride.z;
			}
		}
		else
		{
			if(vHeightDiff > 0)
			{
				//Debug.Log("!!!!!!!!!!!!  RIGHT " + svRightCounter);
				svRightCounter++;

				//Right leg height is taller = Standing on the right leg
				if (Flags.IsTrackingHeight)
				{
					vDisplacement.y = mRightLegHeight;
				}
				if (Flags.IsTrackingHips)
				{
					//vDisplacement.x = mRightLegStride.x;
					vDisplacement.z = -mRightLegStride.z;
				}
			}
			else
			{
				//Left leg height is taller = Standing on the left leg
				//Debug.Log("LEFT !!!!!!!!!!!! " + svLeftCounter);
				svLeftCounter++;

				if (Flags.IsTrackingHeight)
				{
					vDisplacement.y = mLeftLegHeight;
				}
				if (Flags.IsTrackingHips)
				{
					//vDisplacement.x = mLeftLegStride.x;
					vDisplacement.z = -mLeftLegStride.z;
				}
			}
		}

		/*
		//Hips position is based on an circular motion
		X := originX + sin(angle)*radius;
		Y := originY + cos(angle)*radius;
		OriginX = OriginY = 0 (at the base of the feet) 
		Radius = Full Leg length
		Angle = Angle between the X coordinate vector and the new caluclated hips position
		*/
		Vector3 vNewDisplacement = Vector3.zero;
		vNewDisplacement.x = mInitialLegHeight * Mathf.Cos(Mathf.Acos(Vector3.Dot(new Vector3(vDisplacement.x, vDisplacement.y, 0), Vector3.right)));
		vNewDisplacement.y = vDisplacement.y;
		vNewDisplacement.z = mInitialLegHeight * Mathf.Cos(Mathf.Acos(Vector3.Dot(new Vector3(0, vDisplacement.y, vDisplacement.z), Vector3.forward)));

		mHipDisplacement = Vector3.Lerp(mHipDisplacement, vNewDisplacement, 0.5f);
		vSegment.UpdateSubsegmentPosition(mHipDisplacement);
	}

	/// <summary>
	/// MapTorsoSegment: Performs mapping on the torso subsegment from the available sensor data.
	/// </summary>
	/// <param name="vTransformatricies">transformation matrices mapped to sensor positions.</param>
	internal void MapTorsoSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> aTransformatricies)
	{
		TorsoAnalysis vTorsoAnalysis = (TorsoAnalysis)mCurrentAnalysisSegment;

		BodySubSegment vUSSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine];
		BodySubSegment vLSSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine];

		//This is for estimation of the hips orientation (general body orientation)
		BodySubSegment vRLLSubsegment, vRULSubSegment;
		BodySubSegment vLLLSubsegment, vLULSubSegment;
		BodySegment vRightLegSegment = ParentBody.GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes.SegmentType_RightLeg);
		BodySegment vLeftLegSegment = ParentBody.GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes.SegmentType_LeftLeg);
		vRLLSubsegment = vRightLegSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf).Value;
		vRULSubSegment = vRightLegSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh).Value;
		vLLLSubsegment = vLeftLegSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf).Value;
		vLULSubSegment = vLeftLegSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh).Value;

		////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
		BodyFrame.Vect4 vTorsoInitRawQuat = aTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitRawEuler;
		BodyFrame.Vect4 vTorsoCurRawQuat = aTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].CurrRawEuler;// * 180f / Mathf.PI;

		Quaternion vTorsoQuat = Quaternion.identity;
		Quaternion vHipQuat = Quaternion.identity;
		if (GBodyFrameUsingQuaternion)
		{
			//Upper torso
			Quaternion vTorsoInitQuat = new Quaternion(vTorsoInitRawQuat.x, vTorsoInitRawQuat.y, vTorsoInitRawQuat.z, vTorsoInitRawQuat.w);
			vTorsoQuat = new Quaternion(vTorsoCurRawQuat.x, vTorsoCurRawQuat.y, vTorsoCurRawQuat.z, vTorsoCurRawQuat.w);
			vTorsoQuat = Quaternion.Inverse(vTorsoInitQuat) * vTorsoQuat;
		}
		else
		{
			Vector3 vTorsoInitialRawEuler = new Vector3(vTorsoInitRawQuat.x, vTorsoInitRawQuat.y, vTorsoInitRawQuat.z) * 180f / Mathf.PI;//vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitRawEuler * 180f / Mathf.PI;
			Vector3 vTorsoCurrentRawEuler = new Vector3(vTorsoCurRawQuat.x, vTorsoCurRawQuat.y, vTorsoCurRawQuat.z) * 180f / Mathf.PI;
			//Upper torso
			Quaternion vTorsoInitQuat = Quaternion.Euler(0, -vTorsoInitialRawEuler.z, 0);
			Quaternion vTorsoQuatY = Quaternion.Euler(0, -vTorsoCurrentRawEuler.z, 0);
			vTorsoQuatY = Quaternion.Inverse(vTorsoInitQuat) * vTorsoQuatY;

			vTorsoInitQuat = Quaternion.Euler(-vTorsoInitialRawEuler.x, 0, 0);
			Quaternion vTorsoQuatX = Quaternion.Euler(-vTorsoCurrentRawEuler.x, 0, 0);
			vTorsoQuatX = Quaternion.Inverse(vTorsoInitQuat) * vTorsoQuatX;
			//Quaternion vTorsoQuatX = Quaternion.Euler(-Mathf.DeltaAngle(vTorsoInitialRawEuler.x, vTorsoCurrentRawEuler.x), 0, 0);

			vTorsoInitQuat = Quaternion.Euler(0, 0, vTorsoInitialRawEuler.y);
			Quaternion vTorsoQuatZ = Quaternion.Euler(0, 0, vTorsoCurrentRawEuler.y);
			vTorsoQuatZ = Quaternion.Inverse(vTorsoInitQuat) * vTorsoQuatZ;

			//vUSSubsegment.UpdateSubsegmentOrientation(Quaternion.identity, 0, true);
			//vUSSubsegment.AssociatedView.SubsegmentTransform.RotateAround(vUSSubsegment.AssociatedView.SubsegmentTransform.position, vUSSubsegment.AssociatedView.SubsegmentTransform.right, testAngle++);
			//vUSSubsegment.AssociatedView.SubsegmentTransform.RotateAround(vUSSubsegment.AssociatedView.SubsegmentTransform.position, vUSSubsegment.AssociatedView.SubsegmentTransform.right, -Mathf.DeltaAngle(vTorsoInitialRawEuler.x, vTorsoCurrentRawEuler.x));

			////////////////////////////////////////////////////////  Apply Results To Torso /////////////////////////////////////////////////////////////////////
			float vForwardAngle = 0;
			float vUpAngle = 0;


            if (Flags.IsHipsEstimateForward)
			{
				vForwardAngle = -EstimateHipsForwardAngle(vLSSubsegment.AssociatedView.SubsegmentTransform, vUSSubsegment.AssociatedView.SubsegmentTransform,
														  vRULSubSegment.AssociatedView.SubsegmentTransform, vLULSubSegment.AssociatedView.SubsegmentTransform,
														  vRLLSubsegment.AssociatedView.SubsegmentTransform, vLLLSubsegment.AssociatedView.SubsegmentTransform);
			}

			if (Flags.IsHipsEstimateUp)
			{
				vUpAngle = EstimateHipsUpAngle(vLSSubsegment.AssociatedView.SubsegmentTransform, vUSSubsegment.AssociatedView.SubsegmentTransform,
											   vRULSubSegment.AssociatedView.SubsegmentTransform, vLULSubSegment.AssociatedView.SubsegmentTransform,
											   vRLLSubsegment.AssociatedView.SubsegmentTransform, vLLLSubsegment.AssociatedView.SubsegmentTransform);
			}

            //if (Flags.IsUsingInterpolation)
            //{
            //	vHipQuat = Quaternion.Slerp(vLSSubsegment.SubsegmentOrientation, Quaternion.Euler(vUpAngle, 0, 0) * Quaternion.Euler(0, vForwardAngle, 0), InterpolationSpeed);
            //	vTorsoQuat = Quaternion.Slerp(vUSSubsegment.SubsegmentOrientation, Quaternion.Inverse(vHipQuat) * vTorsoQuatY * vTorsoQuatX * vTorsoQuatZ, InterpolationSpeed);
            //}
            //else
            //{
            vHipQuat = Quaternion.Euler(vUpAngle, 0, 0) * Quaternion.Euler(0, vForwardAngle, 0);
            vTorsoQuat = Quaternion.Inverse(vHipQuat) * vTorsoQuatY * vTorsoQuatX * vTorsoQuatZ;
			//}


            //if (Flags.IsCalibrating)
            {
                mROM.capRotation(SegmentType, BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine, vUSSubsegment, ref vTorsoQuat, true);
                mROM.capRotation(SegmentType, BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine, vLSSubsegment, ref vHipQuat);
            }

            if (Flags.IsUsingInterpolation)
            {
                vHipQuat = Quaternion.Slerp(vLSSubsegment.SubsegmentOrientation, vHipQuat, InterpolationSpeed);
                vTorsoQuat = Quaternion.Slerp(vUSSubsegment.SubsegmentOrientation, vTorsoQuat, InterpolationSpeed);
            }

        }


        //Apply results
        vUSSubsegment.UpdateSubsegmentOrientation(vTorsoQuat, 0, true);
		vLSSubsegment.UpdateSubsegmentOrientation(vHipQuat, 3, true);

		////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
		//Update the analysis inputs
		vTorsoAnalysis.TorsoTransform = vUSSubsegment.AssociatedView.SubsegmentTransform;
		vTorsoAnalysis.HipGlobalTransform = vLSSubsegment.AssociatedView.SubsegmentTransform;
		vTorsoAnalysis.DeltaTime = DeltaTime;
		vTorsoAnalysis.AngleExtraction();

		//UpdateHipPosition(vLSSubsegment);//*/
	}

	internal float EstimateHipsForwardAngle(Transform vLSTransform, Transform vUSTransform, Transform vRULTransform, Transform vLULTransform, Transform vRLLTransform, Transform vLLLTransform)
	{
		//Estimate Hips forward orientation
		Vector3 vGlobalVectorUp = Vector3.ProjectOnPlane(Vector3.up, vLSTransform.forward);
		Vector3 vHipsForwardDirection = (Vector3.ProjectOnPlane(vUSTransform.forward, vGlobalVectorUp) + Vector3.ProjectOnPlane(vRULTransform.forward, vGlobalVectorUp) +
										 Vector3.ProjectOnPlane(vLULTransform.forward, vGlobalVectorUp) + Vector3.ProjectOnPlane(vRLLTransform.forward, vGlobalVectorUp) +
										 Vector3.ProjectOnPlane(vLLLTransform.forward, vGlobalVectorUp)) / 5;

		return SegmentAnalysis.GetSignedAngle(vHipsForwardDirection, Vector3.ProjectOnPlane(Vector3.forward, vLSTransform.up), vGlobalVectorUp);
		//return SegmentAnalysis.GetSignedAngle(vHipsForwardDirection, vLSTransform.forward, vLSTransform.up);
	}

	internal float EstimateHipsUpAngle(Transform vLSTransform, Transform vUSTransform, Transform vRULTransform, Transform vLULTransform, Transform vRLLTransform, Transform vLLLTransform)
	{
		//Estimate Hips forward orientation
		Vector3 vHipsUpDirection = (Vector3.ProjectOnPlane(vUSTransform.up, Vector3.right) + Vector3.ProjectOnPlane(vRULTransform.up, Vector3.right) +
									Vector3.ProjectOnPlane(vLULTransform.up, Vector3.right) + Vector3.ProjectOnPlane(vRLLTransform.up, Vector3.right) +
									Vector3.ProjectOnPlane(vLLLTransform.up, Vector3.right)) / 5;

		return SegmentAnalysis.GetSignedAngle(vHipsUpDirection, Vector3.up, Vector3.right);
	}

	/// <summary>
	/// MapRightLegSegment: Performs mapping on the right leg subsegment from the available sensor data.
	/// </summary>
	/// <param name="vTransformatricies">transformation matrices mapped to sensor positions.</param>
	internal void MapRightLegSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
	{
		BodySubSegment vULSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh];
		BodySubSegment vLLSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf];
		BodySegment vHipsSegment = ParentBody.GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes.SegmentType_Torso);
		BodySubSegment vHipsSubsegment = vHipsSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine).Value;

		////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
		BodyFrame.Vect4 vThighInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh].InitRawEuler;
		BodyFrame.Vect4 vThighCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh].CurrRawEuler;
		BodyFrame.Vect4 vKneeInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf].InitRawEuler;
		BodyFrame.Vect4 vKneeCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf].CurrRawEuler;

		Vector3 vThighInitialRawEuler = new Vector3(vThighInit.x, vThighInit.y, vThighInit.z) * 180f / Mathf.PI;
		Vector3 vThighCurrentRawEuler = new Vector3(vThighCurr.x, vThighCurr.y, vThighCurr.z) * 180f / Mathf.PI;
		Vector3 vKneeInitialRawEuler = new Vector3(vKneeInit.x, vKneeInit.y, vKneeInit.z) * 180f / Mathf.PI;
		Vector3 vKneeCurrentRawEuler = new Vector3(vKneeCurr.x, vKneeCurr.y, vKneeCurr.z) * 180f / Mathf.PI;

        if (GBodyFrameUsingQuaternion)
		{
			Quaternion vThighInitialRawQuat = new Quaternion(vThighInit.x, vThighInit.y, vThighInit.z, vThighInit.w);
			Quaternion vThighCurrentRawQuat = new Quaternion(vThighCurr.x, vThighCurr.y, vThighCurr.z, vThighCurr.w);
			Quaternion vKneeInitialRawQuat = new Quaternion(vKneeInit.x, vKneeInit.y, vKneeInit.z, vKneeInit.w);
			Quaternion vKneeCurrentRawQuat = new Quaternion(vKneeCurr.x, vKneeCurr.y, vKneeCurr.z, vKneeCurr.w);

			MapLegsOrientationsQuat(vThighInitialRawQuat, vThighCurrentRawQuat, vKneeInitialRawQuat, vKneeCurrentRawQuat, vULSubsegment, vLLSubsegment, vHipsSubsegment);
		}
		else
		{
			MapLegsOrientations(vThighInitialRawEuler, vThighCurrentRawEuler, vKneeInitialRawEuler, vKneeCurrentRawEuler, vULSubsegment, vLLSubsegment, vHipsSubsegment);
		}
		////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
		//Update the analysis inputs
		RightLegAnalysis vRightLegAnalysis = (RightLegAnalysis)mCurrentAnalysisSegment;
		vRightLegAnalysis.ThighTransform = vULSubsegment.AssociatedView.SubsegmentTransform;
		vRightLegAnalysis.KneeTransform = vLLSubsegment.AssociatedView.SubsegmentTransform;
		vRightLegAnalysis.DeltaTime = DeltaTime;
		vRightLegAnalysis.AngleExtraction();
		mRightLegHeight = vRightLegAnalysis.LegHeight;
		mRightLegStride = vRightLegAnalysis.RightLegStride;
		//UpdateHipPosition(vHipsSubsegment);
	}

	/// <summary>
	/// MapLeftLegSegment: Performs mapping on the left leg subsegment from the available sensor data.
	/// </summary>
	/// <param name="vTransformatricies">transformation matrices mapped to sensor positions.</param>
	internal void MapLeftLegSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
	{
		BodySubSegment vULSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh];
		BodySubSegment vLLSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf];
		BodySegment vHipsSegment = ParentBody.GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes.SegmentType_Torso);
		BodySubSegment vHipsSubsegment = vHipsSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine).Value;

		////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////

		BodyFrame.Vect4 vThighInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh].InitRawEuler;
		BodyFrame.Vect4 vThighCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh].CurrRawEuler;
		BodyFrame.Vect4 vKneeInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf].InitRawEuler;
		BodyFrame.Vect4 vKneeCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf].CurrRawEuler;

		Vector3 vThighInitialRawEuler = new Vector3(vThighInit.x, vThighInit.y, vThighInit.z) * 180f / Mathf.PI; 
		Vector3 vThighCurrentRawEuler = new Vector3(vThighCurr.x, vThighCurr.y, vThighCurr.z) * 180f / Mathf.PI; 
		Vector3 vKneeInitialRawEuler = new Vector3(vKneeInit.x, vKneeInit.y, vKneeInit.z) * 180f / Mathf.PI;
		Vector3 vKneeCurrentRawEuler = new Vector3(vKneeCurr.x, vKneeCurr.y, vKneeCurr.z) * 180f / Mathf.PI;

		if (GBodyFrameUsingQuaternion)
		{
			Quaternion vThighInitialRawQuat = new Quaternion(vThighInit.x, vThighInit.y, vThighInit.z, vThighInit.w);
			Quaternion vThighCurrentRawQuat = new Quaternion(vThighCurr.x, vThighCurr.y, vThighCurr.z, vThighCurr.w);
			Quaternion vKneeInitialRawQuat = new Quaternion(vKneeInit.x, vKneeInit.y, vKneeInit.z, vKneeInit.w);
			Quaternion vKneeCurrentRawQuat = new Quaternion(vKneeCurr.x, vKneeCurr.y, vKneeCurr.z, vKneeCurr.w);

			MapLegsOrientationsQuat(vThighInitialRawQuat, vThighCurrentRawQuat, vKneeInitialRawQuat, vKneeCurrentRawQuat, vULSubsegment, vLLSubsegment, vHipsSubsegment, false);
		}
		else
		{
			MapLegsOrientations(vThighInitialRawEuler, vThighCurrentRawEuler, vKneeInitialRawEuler, vKneeCurrentRawEuler, vULSubsegment, vLLSubsegment, vHipsSubsegment, false);
		}

		////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
		//Update the analysis inputs
		LeftLegAnalysis vLeftLegAnalysis = (LeftLegAnalysis)mCurrentAnalysisSegment;
		vLeftLegAnalysis.ThighTransform = vULSubsegment.AssociatedView.SubsegmentTransform;
		vLeftLegAnalysis.KneeTransform = vLLSubsegment.AssociatedView.SubsegmentTransform;
		vLeftLegAnalysis.DeltaTime = DeltaTime;
		vLeftLegAnalysis.AngleExtraction();
		mLeftLegHeight = vLeftLegAnalysis.LegHeight;
		mLeftLegStride = vLeftLegAnalysis.LeftLegStride;
		UpdateHipPosition(vHipsSubsegment);
	}

	/// <summary>
	/// MapLegsOrientations: Updates the legs orientations from the initial and current eulerangles.
	/// </summary>
	public void MapLegsOrientationsQuat(Quaternion vULInitQuat, Quaternion vULCurQuat, Quaternion vLLInitQuat, Quaternion vLLCurQuat,
									BodySubSegment vULSubsegment, BodySubSegment vLLSubsegment, BodySubSegment vHipsSubsegment, bool vIsRight = true)
	{
		//Upper Leg
		Quaternion vThighQuat = Quaternion.Inverse(Quaternion.Euler(0, 0, 180)) * Quaternion.Inverse(vULInitQuat) * vULCurQuat * Quaternion.Euler(0, 0, 180);

		//Lower leg
		Quaternion vKneeQuat = Quaternion.Inverse(Quaternion.Euler(0, 0, 180)) * Quaternion.Inverse(vLLInitQuat) * vLLCurQuat * Quaternion.Euler(0, 0, 180);

		Quaternion vNewThighQuat = Quaternion.identity;
		Quaternion vNewKneeQuat = Quaternion.identity;

		if (Flags.IsUsingInterpolation)
		{
			vNewThighQuat = Quaternion.Slerp(vULSubsegment.SubsegmentOrientation, vThighQuat, InterpolationSpeed);
			vNewKneeQuat = Quaternion.Slerp(vLLSubsegment.SubsegmentOrientation, vKneeQuat, InterpolationSpeed);
		}
		else
		{
			vNewThighQuat = vThighQuat;
			vNewKneeQuat = vKneeQuat;
		}

		vULSubsegment.UpdateSubsegmentOrientation(vNewThighQuat, 1, true);
		vLLSubsegment.UpdateSubsegmentOrientation(vNewKneeQuat, 1, true);
	}

	/// <summary>
	/// MapLegsOrientations: Updates the legs orientations from the initial and current eulerangles.
	/// </summary>
	public void MapLegsOrientations(Vector3 vULInitEuler, Vector3 vULCurEuler, Vector3 vLLInitEuler, Vector3 vLLCurEuler,
									BodySubSegment vULSubsegment, BodySubSegment vLLSubsegment, BodySubSegment vHipsSubsegment, bool vIsRight = true)
	{
		Quaternion vThighQuat = Quaternion.identity;
		Quaternion vKneeQuat =  Quaternion.identity;

		//if(Flags.IsFusingSubSegments)
		//{
		//	//////////////////////////////////////////////////////////////////////////
		//	// upper
		//	//////////////////////////////////////////////////////////////////////////
		//	// init frame
		//	Quaternion vThighInitQuatX = Quaternion.Euler(0, -vULInitEuler.z, 0);
		//	Quaternion vThighInitQuatY = Quaternion.Euler(-vULInitEuler.x, 0, 0);
		//	Quaternion vThighInitQuatZ = Quaternion.Euler(0, 0, vULInitEuler.y);
		//	Quaternion vThighInitQuat = vThighInitQuatY * vThighInitQuatX * vThighInitQuatZ;
		//	Quaternion vThighInitInverse = Quaternion.Inverse(vThighInitQuat);

		//	// current frame
		//	Quaternion vThighQuatY = Quaternion.Euler(0, -vULCurEuler.z, 0);
		//	Quaternion vThighQuatX = Quaternion.Euler(-vULCurEuler.x, 0, 0);
		//	Quaternion vThighQuatZ = Quaternion.Euler(0, 0, vULCurEuler.y);

		//	// computing
		//	Quaternion vThighCurrentQuat = vThighQuatY * vThighQuatX * vThighQuatZ;
		//	vThighQuat = vThighInitInverse * vThighCurrentQuat;


		//	//////////////////////////////////////////////////////////////////////////
		//	// lower
		//	//////////////////////////////////////////////////////////////////////////
		//	// init frame
		//	Quaternion vKneeInitQuatX = Quaternion.Euler(0, -vLLInitEuler.z, 0);
		//	Quaternion vKneeInitQuatY = Quaternion.Euler(-vLLInitEuler.x, 0, 0);
		//	Quaternion vKneeInitQuatZ = Quaternion.Euler(0, 0, vLLInitEuler.y);
		//	Quaternion vKneeInitQuat = vKneeInitQuatY * vKneeInitQuatX * vKneeInitQuatZ;
		//	Quaternion vKneeInitInverse = Quaternion.Inverse(vKneeInitQuat);

		//	// current frame
		//	Quaternion vKneeQuatY = Quaternion.Euler(0, -vLLCurEuler.z, 0);
		//	Quaternion vKneeQuatX = Quaternion.Euler(-vLLCurEuler.x, 0, 0);
		//	Quaternion vKneeQuatZ = Quaternion.Euler(0, 0, vLLCurEuler.y);

		//	// computing
		//	Quaternion vKneeCurrentQuat = vKneeQuatY * vKneeQuatX * vKneeQuatZ;
		//	vKneeQuat = vKneeInitInverse * vKneeCurrentQuat;

		//}
		//else
		{

			//Upper Leg
			Quaternion vThighInitQuat = Quaternion.Euler(0, -vULInitEuler.z, 0);
			Quaternion vThighQuatY = Quaternion.Euler(0, -vULCurEuler.z, 0);
			vThighQuatY = Quaternion.Inverse(vThighInitQuat) * vThighQuatY;

			vThighInitQuat = Quaternion.Euler(-vULInitEuler.x, 0, 0);
			Quaternion vThighQuatX = Quaternion.Euler(-vULCurEuler.x, 0, 0);
			vThighQuatX = Quaternion.Inverse(vThighInitQuat) * vThighQuatX;

			vThighInitQuat = Quaternion.Euler(0, 0, vULInitEuler.y);
			Quaternion vThighQuatZ = Quaternion.Euler(0, 0, vULCurEuler.y);
			vThighQuatZ = Quaternion.Inverse(vThighInitQuat) * vThighQuatZ;

			//Lower leg
			Quaternion vKneeInitQuat = Quaternion.Euler(0, -vLLInitEuler.z, 0);
			Quaternion vKneeQuatY = Quaternion.Euler(0, -vLLCurEuler.z, 0);
			vKneeQuatY = Quaternion.Inverse(vKneeInitQuat) * vKneeQuatY;

			vKneeInitQuat = Quaternion.Euler(-vLLInitEuler.x, 0, 0);
			Quaternion vKneeQuatX = Quaternion.Euler(-vLLCurEuler.x, 0, 0);
			vKneeQuatX = Quaternion.Inverse(vKneeInitQuat) * vKneeQuatX;

			vKneeInitQuat = Quaternion.Euler(0, 0, vLLInitEuler.y);
			Quaternion vKneeQuatZ = Quaternion.Euler(0, 0, vLLCurEuler.y);
			vKneeQuatZ = Quaternion.Inverse(vKneeInitQuat) * vKneeQuatZ;

			//Apply results
		   vThighQuat = vThighQuatY * vThighQuatX * vThighQuatZ;
		   vKneeQuat = vKneeQuatY * vKneeQuatX * vKneeQuatZ;
		}


		//Get necessary Axis info
		Vector3 vULAxisUp, vULAxisRight, vULAxisForward;
		Vector3 vLLAxisUp, vLLAxisRight, vLLAxisForward;
		Vector3 vNewRight1 = Vector3.up;
		// ReSharper disable once UnusedVariable
		Vector3 vNewRight2 = Vector3.up;
		Vector3 vULNewForward = Vector3.forward;
		Vector3 vLLNewForward = Vector3.forward;

		vULAxisUp = vThighQuat * Vector3.up;
		vULAxisRight = vThighQuat * Vector3.right;
		vULAxisForward = vThighQuat * Vector3.forward;
		Vector3 vNewULAxisUp = vULAxisUp;

		vLLAxisUp = vKneeQuat * Vector3.up;
		vLLAxisRight = vKneeQuat * Vector3.right;
		vLLAxisForward = vKneeQuat * Vector3.forward;

		vNewRight1 = Vector3.Cross(vULAxisUp, vLLAxisUp).normalized;

		if (Mathf.Abs(Vector3.Angle(vLLAxisUp, vULAxisUp)) < 10)
		{
			vNewRight1 = vULAxisRight;
		}

		//if (Flags.IsFusingSubSegments)
		//{
		//    Vector3 vULAxisUpProjected = Vector3.ProjectOnPlane(vULAxisUp, vULAxisRight);
		//    Vector3 vLLAxisUpProjected = Vector3.ProjectOnPlane(vULAxisUp, vULAxisRight);

		//    float vULAdjustAngle = SegmentAnalysis.GetSignedAngle(vULAxisUpProjected, vLLAxisUpProjected, vULAxisRight);

		//    vULNewForward = Vector3.Cross(vNewRight1, vNewULAxisUp).normalized;
		//    vLLNewForward = Vector3.Cross(vNewRight1, vLLAxisUp).normalized;

		//    vThighQuat = Quaternion.LookRotation(vULNewForward, vNewULAxisUp);
		//    vKneeQuat = Quaternion.LookRotation(vLLNewForward, vLLAxisUp);
		//}

		Quaternion vNewThighQuat = Quaternion.identity;
		Quaternion vNewKneeQuat = Quaternion.identity;

// 		if (Flags.IsUsingInterpolation)
// 		{
// 			//if (Flags.IsAdjustingSegmentAxis)
// 			//{
// 			//    vNewThighQuat = Quaternion.Slerp(vULSubsegment.SubsegmentOrientation, vThighQuat * vThighQuatY, InterpolationSpeed);
// 			//    vNewKneeQuat = Quaternion.Slerp(vLLSubsegment.SubsegmentOrientation, vKneeQuat * vKneeQuatY, InterpolationSpeed);
// 			//}
// 			//else
// 			{
// 				vNewThighQuat = Quaternion.Slerp(vULSubsegment.SubsegmentOrientation, vThighQuat, InterpolationSpeed);
// 				vNewKneeQuat = Quaternion.Slerp(vLLSubsegment.SubsegmentOrientation, vKneeQuat, InterpolationSpeed);
// 			}
// 		}
// 		else
// 		{
// 			//if (Flags.IsAdjustingSegmentAxis)
// 			//{
// 			//    vNewThighQuat = vThighQuat * vThighQuatY;
// 			//    vNewKneeQuat = vKneeQuat * vKneeQuatY;
// 			//}
// 			//else
// 			{
		vNewThighQuat = vThighQuat;
		vNewKneeQuat = vKneeQuat;
//			}
//		}
        //if (Flags.IsCalibrating)
        {

            if (vIsRight)
            {
                mROM.capRotation(SegmentType, BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh, vULSubsegment, ref vNewThighQuat);
                mROM.capRotation(SegmentType, BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf, vLLSubsegment, ref vNewKneeQuat);
            }
            else
            {
                mROM.capRotation(SegmentType, BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh, vULSubsegment, ref vNewThighQuat);
                mROM.capRotation(SegmentType, BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf, vLLSubsegment, ref vNewKneeQuat);
            }
        }
        if (Flags.IsUsingInterpolation)
        {
            vNewThighQuat = Quaternion.Slerp(vULSubsegment.SubsegmentOrientation, vNewThighQuat, InterpolationSpeed);
            vNewKneeQuat = Quaternion.Slerp(vLLSubsegment.SubsegmentOrientation, vNewKneeQuat, InterpolationSpeed);
        }
        vULSubsegment.UpdateSubsegmentOrientation(vNewThighQuat, 1, true);
		vLLSubsegment.UpdateSubsegmentOrientation(vNewKneeQuat, 1, true);
	}

	/// <summary>
	/// MapRightArmSubsegment: Updates the right arm subsegment from the available sensor data.
	/// </summary>
	/// <param name="vTransformatricies">transformation matrices mapped to sensor positions.</param>
	internal void MapRightArmSubsegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
	{
		RightArmAnalysis vRightArmAnalysis = (RightArmAnalysis)mCurrentAnalysisSegment;

		BodySubSegment vUASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm];
		BodySubSegment vLASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm];
		BodySegment vTorsoSegment = ParentBody.GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes.SegmentType_Torso);
		BodySubSegment vTorsoSubSegment = vTorsoSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine).Value;
		BodySubSegment vHipsSubsegment = vTorsoSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine).Value; 

		////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////

		BodyFrame.Vect4 vUpArmInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].InitRawEuler;
		BodyFrame.Vect4 vUpArmCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].CurrRawEuler;
		BodyFrame.Vect4 vLoArmInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm].InitRawEuler;
		BodyFrame.Vect4 vLoArmCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm].CurrRawEuler;
/*
		BodyFrame.Vect4 vTorsoInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitRawEuler;
		BodyFrame.Vect4 vTorsoCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].CurrRawEuler;
/*/
		BodyFrame.Vect4 vTorsoInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].InitRawEuler;
		BodyFrame.Vect4 vTorsoCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].CurrRawEuler;
//*/


		Vector3 vUpArmInitialRawEuler = new Vector3(vUpArmInit.x, vUpArmInit.y, vUpArmInit.z) * Mathf.Rad2Deg;
		Vector3 vUpArmCurrentRawEuler = new Vector3(vUpArmCurr.x, vUpArmCurr.y, vUpArmCurr.z) * Mathf.Rad2Deg;
		Vector3 vLoArmInitialRawEuler = new Vector3(vLoArmInit.x, vLoArmInit.y, vLoArmInit.z) * Mathf.Rad2Deg;
		Vector3 vLoArmCurrentRawEuler = new Vector3(vLoArmCurr.x, vLoArmCurr.y, vLoArmCurr.z) * Mathf.Rad2Deg;
		Vector3 vTorsoInitialRawEuler = new Vector3(vTorsoInit.x, vTorsoInit.y, vTorsoInit.z) * Mathf.Rad2Deg;
		Vector3 vTorsoCurrentRawEuler = new Vector3(vTorsoCurr.x, vTorsoCurr.y, vTorsoCurr.z) * Mathf.Rad2Deg;

        

		if (GBodyFrameUsingQuaternion)
		{
			Quaternion vUpArmInitialRawQuat = new Quaternion(vUpArmInit.x, vUpArmInit.y, vUpArmInit.z, vUpArmInit.w);
			Quaternion vUpArmCurrentRawQuat = new Quaternion(vUpArmCurr.x, vUpArmCurr.y, vUpArmCurr.z, vUpArmCurr.w);
			Quaternion vLoArmInitialRawQuat = new Quaternion(vLoArmInit.x, vLoArmInit.y, vLoArmInit.z, vLoArmInit.w);
			Quaternion vLoArmCurrentRawQuat = new Quaternion(vLoArmCurr.x, vLoArmCurr.y, vLoArmCurr.z, vLoArmCurr.w);
			Quaternion vTorsoInitialRawQuat = new Quaternion(vTorsoInit.x, vTorsoInit.y, vTorsoInit.z, vTorsoInit.w);
			Quaternion vTorsoCurrentRawQuat = new Quaternion(vTorsoCurr.x, vTorsoCurr.y, vTorsoCurr.z, vTorsoCurr.w);

			MapArmsOrientationsQuat(vUpArmInitialRawQuat, vUpArmCurrentRawQuat, vLoArmInitialRawQuat, vLoArmCurrentRawQuat, vTorsoInitialRawQuat, vTorsoCurrentRawQuat, vUASubsegment, vLASubsegment, vTorsoSubSegment, vHipsSubsegment);
		}
		else
		{
			MapArmsOrientations(vUpArmInitialRawEuler, vUpArmCurrentRawEuler, vLoArmInitialRawEuler, vLoArmCurrentRawEuler, vTorsoInitialRawEuler, vTorsoCurrentRawEuler, vUASubsegment, vLASubsegment, vTorsoSubSegment, vHipsSubsegment);
		}



        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        vRightArmAnalysis.UpArTransform = vUASubsegment.AssociatedView.SubsegmentTransform;
		vRightArmAnalysis.LoArTransform = vLASubsegment.AssociatedView.SubsegmentTransform;
		vRightArmAnalysis.DeltaTime = DeltaTime;
		vRightArmAnalysis.ReferenceVector = Vector3.one;
		vRightArmAnalysis.AngleExtraction();//*/ 
	}

	/// <summary>
	/// MapLeftArmSubsegment: Updates the left arm subsegment from the available sensor data.
	/// </summary>
	/// <param name="vTransformsi   Q1 =  Qy *Qx *Qz  et  Q2 =  Q-1z  *Q-1x *Q-1yatricies">transformation matrices mapped to sensor positions.</param>
	internal void MapLeftArmSubsegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
	{
		BodySubSegment vUASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm];
		BodySubSegment vLASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm];
		BodySegment vTorsoSegment = ParentBody.GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes.SegmentType_Torso);
		BodySubSegment vTorsoSubSegment = vTorsoSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine).Value;
		BodySubSegment vHipsSubsegment = vTorsoSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine).Value;

		////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////


		BodyFrame.Vect4 vUpArmInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].InitRawEuler;
		BodyFrame.Vect4 vUpArmCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].CurrRawEuler;
		BodyFrame.Vect4 vLoArmInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm].InitRawEuler;
		BodyFrame.Vect4 vLoArmCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm].CurrRawEuler;
		BodyFrame.Vect4 vTorsoInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitRawEuler;
		BodyFrame.Vect4 vTorsoCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].CurrRawEuler;

		Vector3 vUpArmInitialRawEuler = new Vector3(vUpArmInit.x, vUpArmInit.y, vUpArmInit.z) * Mathf.Rad2Deg;
		Vector3 vUpArmCurrentRawEuler = new Vector3(vUpArmCurr.x, vUpArmCurr.y, vUpArmCurr.z) * Mathf.Rad2Deg;
		Vector3 vLoArmInitialRawEuler = new Vector3(vLoArmInit.x, vLoArmInit.y, vLoArmInit.z) * Mathf.Rad2Deg;
		Vector3 vLoArmCurrentRawEuler = new Vector3(vLoArmCurr.x, vLoArmCurr.y, vLoArmCurr.z) * Mathf.Rad2Deg;
		Vector3 vTorsoInitialRawEuler = new Vector3(vTorsoInit.x, vTorsoInit.y, vTorsoInit.z) * Mathf.Rad2Deg;
		Vector3 vTorsoCurrentRawEuler = new Vector3(vTorsoCurr.x, vTorsoCurr.y, vTorsoCurr.z) * Mathf.Rad2Deg;

		if (GBodyFrameUsingQuaternion)
		{
			Quaternion vUpArmInitialRawQuat = new Quaternion(vUpArmInit.x, vUpArmInit.y, vUpArmInit.z, vUpArmInit.w);
			Quaternion vUpArmCurrentRawQuat = new Quaternion(vUpArmCurr.x, vUpArmCurr.y, vUpArmCurr.z, vUpArmCurr.w);
			Quaternion vLoArmInitialRawQuat = new Quaternion(vLoArmInit.x, vLoArmInit.y, vLoArmInit.z, vLoArmInit.w);
			Quaternion vLoArmCurrentRawQuat = new Quaternion(vLoArmCurr.x, vLoArmCurr.y, vLoArmCurr.z, vLoArmCurr.w);
			Quaternion vTorsoInitialRawQuat = new Quaternion(vTorsoInit.x, vTorsoInit.y, vTorsoInit.z, vTorsoInit.w);
			Quaternion vTorsoCurrentRawQuat = new Quaternion(vTorsoCurr.x, vTorsoCurr.y, vTorsoCurr.z, vTorsoCurr.w);

			MapArmsOrientationsQuat(vUpArmInitialRawQuat, vUpArmCurrentRawQuat, vLoArmInitialRawQuat, vLoArmCurrentRawQuat, vTorsoInitialRawQuat, vTorsoCurrentRawQuat, vUASubsegment, vLASubsegment, vTorsoSubSegment, vHipsSubsegment);
		}
		else
		{
			MapArmsOrientations(vUpArmInitialRawEuler, vUpArmCurrentRawEuler, vLoArmInitialRawEuler, vLoArmCurrentRawEuler, vTorsoInitialRawEuler, vTorsoCurrentRawEuler, vUASubsegment, vLASubsegment, vTorsoSubSegment, vHipsSubsegment, false);
		}
		////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
		LeftArmAnalysis vLeftArmAnalysis = (LeftArmAnalysis)mCurrentAnalysisSegment;
		vLeftArmAnalysis.UpArTransform = vUASubsegment.AssociatedView.SubsegmentTransform;
		vLeftArmAnalysis.LoArTransform = vLASubsegment.AssociatedView.SubsegmentTransform;
		vLeftArmAnalysis.DeltaTime = DeltaTime;
		vLeftArmAnalysis.AngleExtraction();
        
	}

	public void MapArmsOrientationsQuat(Quaternion vUAInitQuat, Quaternion vUACurQuat, Quaternion vLAInitQuat, Quaternion vLACurQuat, Quaternion vTorsoInitQuat, Quaternion vTorsoCurQuat,
								BodySubSegment vUASubsegment, BodySubSegment vLASubsegment, BodySubSegment vTorsoSubSegment, BodySubSegment vHipsSubsegment, bool vIsRight = true)
	{
		Quaternion vUpArmQuat = Quaternion.Inverse(Quaternion.Euler(180, 0, 90)) * Quaternion.Inverse(vUAInitQuat) * vUACurQuat * Quaternion.Euler(180, 180, 90);
		Quaternion vLoArmQuat = Quaternion.Inverse(Quaternion.Euler(180, 0, 90)) * Quaternion.Inverse(vLAInitQuat) * vLACurQuat * Quaternion.Euler(180, 0, 90);

		Quaternion vNewUpArmQuat = Quaternion.identity;
		Quaternion vNewLoArmQuat = Quaternion.identity;

		if (Flags.IsUsingInterpolation)
		{
			vNewUpArmQuat = Quaternion.Slerp(vUASubsegment.SubsegmentOrientation, vUpArmQuat, InterpolationSpeed);
			vNewLoArmQuat = Quaternion.Slerp(vLASubsegment.SubsegmentOrientation, vLoArmQuat, InterpolationSpeed);
		}
		else
		{
			vNewUpArmQuat = vUpArmQuat;
			vNewLoArmQuat = vLoArmQuat;
		}

		vUASubsegment.UpdateSubsegmentOrientation(vNewUpArmQuat, 1, true);
		//vLASubsegment.UpdateSubsegmentOrientation(vNewLoArmQuat, 1, true);
	}


	/// <summary>
	/// MapArmsOrientations: Updates the arm orientations from the initial and current eulerangles.
	/// </summary>
	//float vCurrentAngle = 0;
	//bool vIsIncreasingAngle = true;
	public void MapArmsOrientations(Vector3 aUAInitEuler, Vector3 aUACurEuler, Vector3 aLAInitEuler, Vector3 aLACurEuler, Vector3 aTorsoInitEuler, Vector3 aTorsoCurEuler,
									BodySubSegment aUASubsegment, BodySubSegment aLASubsegment, BodySubSegment aTorsoSubSegment, BodySubSegment aHipsSubsegment, bool aIsRight = true)
	{

		//Upper arm
		Quaternion vUpArmInitQuatY = Quaternion.Euler(0, aUAInitEuler.z, 0);
		Quaternion vUpArmQuatY = Quaternion.Euler(0, aUACurEuler.z, 0);
		vUpArmQuatY = Quaternion.Inverse(vUpArmInitQuatY) * vUpArmQuatY;
		vUpArmQuatY = Quaternion.Inverse(vUpArmQuatY);

		Quaternion vUpArmInitQuatX = Quaternion.Euler(aUAInitEuler.x, 0, 0);
		Quaternion vUpArmQuatX = Quaternion.Euler(aUACurEuler.x, 0, 0);
		vUpArmQuatX = Quaternion.Inverse(vUpArmInitQuatX) * vUpArmQuatX;

		Quaternion vUpArmInitQuatZ = Quaternion.Euler(0, 0, (aUAInitEuler.y));
		Quaternion vUpArmQuatZ = Quaternion.Euler(0, 0, (aUACurEuler.y));
		vUpArmQuatZ = Quaternion.Inverse(vUpArmInitQuatZ) * vUpArmQuatZ;
		vUpArmQuatZ = Quaternion.Inverse(vUpArmQuatZ);

		//Lower arm
		Quaternion vLoArmInitQuatY = Quaternion.Euler(0, (aLAInitEuler.z), 0);
		Quaternion vLoArmQuatY = Quaternion.Euler(0, (aLACurEuler.z), 0);
		vLoArmQuatY = Quaternion.Inverse(vLoArmInitQuatY) * vLoArmQuatY;
		vLoArmQuatY = Quaternion.Inverse(vLoArmQuatY);

		Quaternion vLoArmInitQuatX = Quaternion.Euler(aLAInitEuler.x, 0, 0);
		Quaternion vLoArmQuatX = Quaternion.Euler(aLACurEuler.x, 0, 0);
		vLoArmQuatX = Quaternion.Inverse(vLoArmInitQuatX) * vLoArmQuatX;

		Quaternion vLoArmInitQuatZ = Quaternion.Euler(0, 0, (aLAInitEuler.y));
		Quaternion vLoArmQuatZ = Quaternion.Euler(0, 0, (aLACurEuler.y));
		vLoArmQuatZ = Quaternion.Inverse(vLoArmInitQuatZ) * vLoArmQuatZ;
		vLoArmQuatZ = Quaternion.Inverse(vLoArmQuatZ);

		Quaternion vUpArmQuat = vUpArmQuatY * vUpArmQuatX * vUpArmQuatZ;
		Quaternion vLoArmQuat = vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ;

		//Get necessary Axis info
		Vector3 vUAAxisUp, vUAAxisRight, vUAAxisForward;
		Vector3 vLAAxisUp, vLAAxisRight, vLAAxisForward;

		Vector3 vUANewRight = Vector3.right;
		Vector3 vLANewRight = Vector3.right;
		Vector3 vUANewUp = Vector3.up;
		Vector3 vLANewUp = Vector3.up;
		Vector3 vUANewForward = Vector3.forward;
		Vector3 vLANewForward = Vector3.forward;

		vUAAxisUp = vUpArmQuat * Vector3.up;
		vUAAxisRight = vUpArmQuat * Vector3.right;
		vUAAxisForward = vUpArmQuat * Vector3.forward;

		vLAAxisUp = vLoArmQuat * Vector3.up;
		vLAAxisRight = vLoArmQuat * Vector3.right;
		vLAAxisForward = vLoArmQuat * Vector3.forward;
#if SEGMENTS_DEBUG
        if (Flags.IsProjectingXY)
		{
			vLANewRight = Vector3.ProjectOnPlane(vLAAxisRight, vUAAxisForward);
			vLANewUp = Vector3.Cross(vLAAxisForward, vLANewRight);
			vLANewForward = Vector3.Cross(vLANewRight, vLANewUp);
			vLoArmQuat = Quaternion.LookRotation(vLANewForward, vLANewUp);
		}
		else if(Flags.IsProjectingXZ)
		{
			vLANewRight = Vector3.ProjectOnPlane(vLAAxisRight, vUAAxisUp);
			vLANewUp = Vector3.Cross(vLAAxisForward, vLANewRight);
			vLANewForward = Vector3.Cross(vLANewRight, vLANewUp);
			vLoArmQuat = Quaternion.LookRotation(vLANewForward, vLANewUp);
		}
		else if(Flags.IsProjectingYZ)
		{
			vLANewRight = Vector3.ProjectOnPlane(vLAAxisRight, vUAAxisRight);
			vLANewUp = Vector3.Cross(vLAAxisForward, vLANewRight);
			vLANewForward = Vector3.Cross(vLANewRight, vLANewUp);
			vLoArmQuat = Quaternion.LookRotation(vLANewForward, vLANewUp);
		}
#endif

#if SEGMENTS_DEBUG
		vUAAxisUp = vUpArmQuat * Vector3.up;
		vUAAxisRight = vUpArmQuat * Vector3.right;
		vUAAxisForward = vUpArmQuat * Vector3.forward;

		vLAAxisUp = vLoArmQuat * Vector3.up;
		vLAAxisRight = vLoArmQuat * Vector3.right;
		vLAAxisForward = vLoArmQuat * Vector3.forward;
#endif
		if (Flags.IsFusingSubSegments)
		{
			Vector3 vNewAxisUp = Vector3.up;
			Vector3 vTempUp = Vector3.up;

			vNewAxisUp = Vector3.Cross(vLAAxisRight, vUAAxisRight).normalized;
			vTempUp = Vector3.Cross(vUAAxisRight, vLAAxisRight).normalized;

			if (Mathf.Abs(Vector3.Angle(vLAAxisRight, vUAAxisRight)) < 10)
			{
				vNewAxisUp = vUAAxisUp;
			}
			else
			{
				if (!aIsRight)
				{
					vNewAxisUp = vTempUp;
				}
			}

			vUANewForward = Vector3.Cross(vUAAxisRight, vNewAxisUp).normalized;
			vLANewForward = Vector3.Cross(vLAAxisRight, vNewAxisUp).normalized;

			vUpArmQuat = Quaternion.LookRotation(vUANewForward, vNewAxisUp);
			vLoArmQuat = Quaternion.LookRotation(vLANewForward, vNewAxisUp);

			//Vector3 vNewUAAxisRight = vUpArmQuat * Vector3.right;
			//Vector3 vNewLAAxisRight = vLoArmQuat * Vector3.right;
			//if(aIsRight) Debug.Log(SegmentAnalysis.GetSignedAngle(vNewUAAxisRight, vNewLAAxisRight, vNewAxisUp));
			//vLoArmQuat = Quaternion.AngleAxis(SegmentAnalysis.GetSignedAngle(vNewLAAxisRight, vNewUAAxisRight, vNewAxisUp), vNewAxisUp);
		}

		Quaternion vNewUpArmQuat = Quaternion.identity;
		Quaternion vNewLoArmQuat = Quaternion.identity;

        //if (Flags.IsUsingInterpolation)
        //{
        //	if (Flags.IsAdjustingSegmentAxis)
        //	{
        //		vNewUpArmQuat = Quaternion.Slerp(aUASubsegment.SubsegmentOrientation, vUpArmQuat * Quaternion.Inverse(vUpArmQuatX), InterpolationSpeed);
        //		vNewLoArmQuat = Quaternion.Slerp(aLASubsegment.SubsegmentOrientation, vLoArmQuat * Quaternion.Inverse(vLoArmQuatX), InterpolationSpeed);
        //	}
        //	else
        //	{
        //		vNewUpArmQuat = Quaternion.Slerp(aUASubsegment.SubsegmentOrientation, vUpArmQuat, InterpolationSpeed);
        //		vNewLoArmQuat = Quaternion.Slerp(aLASubsegment.SubsegmentOrientation, vLoArmQuat, InterpolationSpeed);
        //	}
        //}
        //else
        //{
        //	if (Flags.IsAdjustingSegmentAxis)
        //	{
        //		vNewUpArmQuat = vUpArmQuat * Quaternion.Inverse(vUpArmQuatX);
        //		vNewLoArmQuat = vLoArmQuat * Quaternion.Inverse(vLoArmQuatX);
        //	}
        //	else
        //	{
        //		vNewUpArmQuat = vUpArmQuat;
        //		vNewLoArmQuat = vLoArmQuat;
        //	}
        //}

        vNewUpArmQuat = vUpArmQuat;
        vNewLoArmQuat = vLoArmQuat;
        //if(Flags.IsCalibrating)
        {
            if (aIsRight)
            {
                mROM.capRotation(SegmentType, BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm, aUASubsegment, ref vNewUpArmQuat);
                mROM.capRotation(SegmentType, BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm, aLASubsegment, ref vNewLoArmQuat);

            }
            else
            {
                mROM.capRotation(SegmentType, BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm, aUASubsegment, ref vNewUpArmQuat);
                mROM.capRotation(SegmentType, BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm, aLASubsegment, ref vNewLoArmQuat);
            }
        }

        if(Flags.IsUsingInterpolation)
        {
            vNewUpArmQuat = Quaternion.Slerp(aUASubsegment.SubsegmentOrientation, vNewUpArmQuat, InterpolationSpeed);
            vNewLoArmQuat = Quaternion.Slerp(aLASubsegment.SubsegmentOrientation, vNewLoArmQuat, InterpolationSpeed);
        }

        aUASubsegment.UpdateSubsegmentOrientation(vNewUpArmQuat, 1, true);
		aLASubsegment.UpdateSubsegmentOrientation(vNewLoArmQuat, 1, true);
	}

	//public float getAngleDelta(float vCurAngle, float vInitAngle)
	//{
	//	float vResult = 0;

	//	if (vCurAngle > 0)
	//	{
	//		if (Mathf.Abs(vCurAngle) > Mathf.Abs(vInitAngle))
	//		{
	//			vResult = Mathf.Abs(vCurAngle) - Mathf.Abs(vInitAngle);
	//		}
	//		else
	//		{
	//			vResult = Mathf.Abs(vInitAngle) - Mathf.Abs(vCurAngle);
	//		}
	//	}
	//	else
	//	{
	//		if (Mathf.Abs(vCurAngle) > Mathf.Abs(vInitAngle))
	//		{
	//			vResult = Mathf.Abs(vInitAngle) - Mathf.Abs(vCurAngle);
	//		}
	//		else
	//		{
	//			vResult = Mathf.Abs(vCurAngle) - Mathf.Abs(vInitAngle);
	//		}
	//	}

	//	return vResult;
	//}

	/// <summary>
	/// InitializeBodySegment: Initializes a new body structure's internal properties with the desired Segment Type.
	/// </summary>
	/// <param name="vSegmentType">The segment type to initialize it to.</param>
	internal void InitializeBodySegment(BodyStructureMap.SegmentTypes vSegmentType)
	{
		GameObject go = new GameObject(EnumUtil.GetName(vSegmentType));
		AssociatedView = go.AddComponent<BodySegmentView>();

		List<BodyStructureMap.SubSegmentTypes> subsegmentTypes =
		   BodyStructureMap.Instance.SegmentToSubSegmentMap[vSegmentType];

		List<BodyStructureMap.SensorPositions> sensorPositions =
			BodyStructureMap.Instance.SegmentToSensorPosMap[vSegmentType];

		foreach (var sensorPos in sensorPositions)
		{
			Sensor newSensor = new Sensor();
			newSensor.SensorBodyId = BodyStructureMap.Instance.SensorPosToSensorIDMap[sensorPos];
			newSensor.SensorType = BodyStructureMap.Instance.SensorPosToSensorTypeMap[sensorPos];
			newSensor.SensorPosition = sensorPos;
			SensorTuple tuple = new SensorTuple();
			tuple.CurrentSensor = new Sensor(newSensor);
			tuple.InitSensor = new Sensor(newSensor);
			SensorsTuple.Add(tuple);
		}

		foreach (BodyStructureMap.SubSegmentTypes sstype in subsegmentTypes)
		{
			BodySubSegment subSegment = new BodySubSegment();
			subSegment.SubsegmentType = sstype;
			subSegment.InitializeBodySubsegment(sstype);
			BodySubSegmentsDictionary.Add((int)sstype, subSegment);
			subSegment.AssociatedView.transform.parent = AssociatedView.transform;
		}

	}

 
	/// <summary>
	/// MapSubSegments: Perform mapping on the current segments and its respective subsegments.
	/// </summary>
	/// <param name="vFilteredDictionary">Dictionnary of tracked segments and their transformations.</param>
	private void MapSubSegments(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vFilteredDictionary)
	{
		if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_Torso)
		{
			//Debug.Log("TORSO");
			MapTorsoSegment(vFilteredDictionary);
		}
		if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_RightArm)
		{
			//Debug.Log("RIGHT ARM");
			MapRightArmSubsegment(vFilteredDictionary);
		}
		if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_LeftArm)
		{
			//Debug.Log("LEFT ARM");
			MapLeftArmSubsegment(vFilteredDictionary);
		}
		if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_RightLeg)
		{
			//Debug.Log("RIGHT LEG");
			MapRightLegSegment(vFilteredDictionary);
		}
		if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_LeftLeg)
		{
			//Debug.Log("LEFT LEG");
			MapLeftLegSegment(vFilteredDictionary);
		}
	}

	/// <summary>
	/// Updates subsegments with the passed in RenderedBody
	/// </summary>
	/// <param name="vRendered"></param>
	public void UpdateRenderedSegment(RenderedBody vRendered)
	{
		foreach (var vsubSegment in BodySubSegmentsDictionary)
		{
			Transform vSubSegmentTransform = vRendered.GetSubSegmentTransform((BodyStructureMap.SubSegmentTypes) vsubSegment.Key);
			vsubSegment.Value.UpdateSubSegmentTransform(vSubSegmentTransform);
		}
	}
	/// <summary>
	/// Releases 3d resources used by the BodySegment
	/// </summary>
	public void ReleaseResources()
	{
		foreach (var vBodySubSegment in BodySubSegmentsDictionary)
		{
			vBodySubSegment.Value.ReleaseResources();
		}
	}

    public void SetStaticROM(StaticROM segmentRom)
    {
        mROM = segmentRom;
        mROM.mFlags = Flags;
    }

}

#endif