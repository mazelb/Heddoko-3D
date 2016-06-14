
/** 
* @file BodySegment.cs
* @brief Containsthe  BodySegment  class 
* @author  
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Body_Data.view;
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Data.CalibrationData;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.Body_Pipeline.Analysis;
using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Torso;
using Assets.Scripts.Utils;
//using System.Diagnostics;
using System.IO;



/// <summary>
/// BodySegment class: represents one abstracted reprensentation of a body segment.
/// </summary>
public partial class BodySegment
{

#if SEGMENTS_DEBUG
    public BodyFrameCalibrationContainer BodyFrameCalibrationContainer { get; internal set; }
    //Segment Type 
    public BodyStructureMap.SegmentTypes SegmentType;

    //Body SubSegments 
    public Dictionary<int, BodySubSegment> BodySubSegmentsDictionary = new Dictionary<int, BodySubSegment>();
    private BodyFrameCalibrationContainer mBodyFrameCalibrationContainer;
    public Body ParentBody;

    //Is segment tracked (based on body type) 
    public bool IsTracked = true;
    public bool IsReseting = false;
    public int ResetCounter = 0;
    static public bool IsTrackingHeight = true;
    static public bool IsTrackingGait = false;
    static public bool IsTrackingHips = false;
    static public bool IsAdjustingSegmentAxis = false;
    static public bool IsAdjustingArms;
    static public bool IsFusingSubSegments = false;
    static public bool IsProjectingXZ = false;
    static public bool IsProjectingXY = false;
    static public bool IsProjectingYZ = false;
    static public bool IsHipsEstimateForward = true;
    static public bool IsHipsEstimateUp = false;
    static public bool IsUsingInterpolation = true;
    static public float InterpolationSpeed = 0.3f;
    static public bool IsCalibrating = true;
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
    private static Vector3 mHipDisplacement = new Vector3(0, 0.95f, 0);
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

    *** Definitions for calibration methods
    // Filename to write to.
    private string mRecordingFileName = "";

    // File stream to write to.
    private TextWriter mRecordingFileStream;
    // Title to append to file.
    public string mRecordingTitle = "data_set";

    // Interval at which to collect data, in miliseconds. Set to zero to ignore.
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


    /// <summary>
    /// The function will update the sensors data with the passed in BodyFrame. Iterates through the list of sensor tuples and updates the current sensor's information
    /// </summary>
    /// <param name="vFrame"></param>
    public void UpdateSensorsData(BodyFrame vFrame)
    { 
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
                Vector3 vFrameData = vFrame.FrameData[vSensorPosKey];
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
         Profiler.BeginSample("MapSubSegments");
        MapSubSegments(vFilteredDictionary);
        Profiler.EndSample();
    }

    /// <summary>
    /// UpdateInitialSensorsData: The function will update the sensors data with the passed in BodyFrame. Iterates through the list of sensor tuples and updates the initial sensor's information.
    /// </summary>
    /// <param name="vFrame">the body frame whose subframes will updates to initial sensors.</param>
    /// <param name="vBodyCalibrationSetting">optional parameter to set the current calibration setting</param>
    public void UpdateInitialSensorsData(BodyFrame vFrame)     ///*************************/////
    {
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

    static int svRightCounter = 0;
    static int svLeftCounter = 0;
    static int svStandCounter = 0;

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
            if (IsTrackingHeight)
            {
                vDisplacement.y = mRightLegHeight;
            }
            if (IsTrackingHips)
            {
                //vDisplacement.x = mRightLegStride.x;
                vDisplacement.z = -mRightLegStride.z;
            }
        }
        else
        {
            if (vHeightDiff > 0)
            {
                //Debug.Log("!!!!!!!!!!!!  RIGHT " + svRightCounter);
                svRightCounter++;

                //Right leg height is taller = Standing on the right leg
                if (IsTrackingHeight)
                {
                    vDisplacement.y = mRightLegHeight;
                }
                if (IsTrackingHips)
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

                if (IsTrackingHeight)
                {
                    vDisplacement.y = mLeftLegHeight;
                }
                if (IsTrackingHips)
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
    internal void MapTorsoSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
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
        Vector3 vTorsoInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitRawEuler * 180f / Mathf.PI;
        Vector3 vTorsoCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].CurrRawEuler * 180f / Mathf.PI;

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
        Quaternion vTorsoQuat = Quaternion.identity;
        Quaternion vHipQuat = Quaternion.identity;
        float vForwardAngle = 0;
        float vUpAngle = 0;

        if (IsHipsEstimateForward)
        {
            vForwardAngle = -EstimateHipsForwardAngle(vLSSubsegment.AssociatedView.SubsegmentTransform, vUSSubsegment.AssociatedView.SubsegmentTransform,
                                                      vRULSubSegment.AssociatedView.SubsegmentTransform, vLULSubSegment.AssociatedView.SubsegmentTransform,
                                                      vRLLSubsegment.AssociatedView.SubsegmentTransform, vLLLSubsegment.AssociatedView.SubsegmentTransform);
        }

        if (IsHipsEstimateUp)
        {
            vUpAngle = EstimateHipsUpAngle(vLSSubsegment.AssociatedView.SubsegmentTransform, vUSSubsegment.AssociatedView.SubsegmentTransform,
                                           vRULSubSegment.AssociatedView.SubsegmentTransform, vLULSubSegment.AssociatedView.SubsegmentTransform,
                                           vRLLSubsegment.AssociatedView.SubsegmentTransform, vLLLSubsegment.AssociatedView.SubsegmentTransform);
        }

        if (IsUsingInterpolation)
        {
            vHipQuat = Quaternion.Slerp(vLSSubsegment.SubsegmentOrientation, Quaternion.Euler(vUpAngle, 0, 0) * Quaternion.Euler(0, vForwardAngle, 0), InterpolationSpeed);
            vTorsoQuat = Quaternion.Slerp(vUSSubsegment.SubsegmentOrientation, Quaternion.Inverse(vHipQuat) * vTorsoQuatY * vTorsoQuatX * vTorsoQuatZ, InterpolationSpeed);
        }
        else
        {
            vHipQuat = Quaternion.Euler(vUpAngle, 0, 0) * Quaternion.Euler(0, vForwardAngle, 0);
            vTorsoQuat = Quaternion.Inverse(vHipQuat) * vTorsoQuatY * vTorsoQuatX * vTorsoQuatZ;
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
    }

    internal float EstimateHipsUpAngle(Transform vLSTransform, Transform vUSTransform, Transform vRULTransform, Transform vLULTransform, Transform vRLLTransform, Transform vLLLTransform)
    {
        //Estimate Hips forward orientation
        //Vector3 vGlobalVectorRight = Vector3.ProjectOnPlane(Vector3.right, vLSTransform.forward);
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
        Vector3 vThighInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh].InitRawEuler * 180f / Mathf.PI;
        Vector3 vThighCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh].CurrRawEuler * 180f / Mathf.PI;
        Vector3 vKneeInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf].InitRawEuler * 180f / Mathf.PI;
        Vector3 vKneeCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf].CurrRawEuler * 180f / Mathf.PI;
        MapLegsOrientations(vThighInitialRawEuler, vThighCurrentRawEuler, vKneeInitialRawEuler, vKneeCurrentRawEuler, vULSubsegment, vLLSubsegment, vHipsSubsegment);

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
        Vector3 vThighInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh].InitRawEuler * 180f / Mathf.PI;
        Vector3 vThighCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh].CurrRawEuler * 180f / Mathf.PI;
        Vector3 vKneeInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf].InitRawEuler * 180f / Mathf.PI;
        Vector3 vKneeCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf].CurrRawEuler * 180f / Mathf.PI;
        MapLegsOrientations(vThighInitialRawEuler, vThighCurrentRawEuler, vKneeInitialRawEuler, vKneeCurrentRawEuler, vULSubsegment, vLLSubsegment, vHipsSubsegment, false);

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
    public void MapLegsOrientations(Vector3 vULInitEuler, Vector3 vULCurEuler, Vector3 vLLInitEuler, Vector3 vLLCurEuler,
                                    BodySubSegment vULSubsegment, BodySubSegment vLLSubsegment, BodySubSegment vHipsSubsegment, bool vIsRight = true)
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
        Quaternion vThighQuat = vThighQuatY * vThighQuatX * vThighQuatZ;
        Quaternion vKneeQuat = vKneeQuatY * vKneeQuatX * vKneeQuatZ;


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

        //if (IsFusingSubSegments)
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

        if (IsUsingInterpolation)
        {
            //if (IsAdjustingSegmentAxis)
            //{
            //    vNewThighQuat = Quaternion.Slerp(vULSubsegment.SubsegmentOrientation, vThighQuat * vThighQuatY, InterpolationSpeed);
            //    vNewKneeQuat = Quaternion.Slerp(vLLSubsegment.SubsegmentOrientation, vKneeQuat * vKneeQuatY, InterpolationSpeed);
            //}
            //else
            {
                vNewThighQuat = Quaternion.Slerp(vULSubsegment.SubsegmentOrientation, vThighQuat, InterpolationSpeed);
                vNewKneeQuat = Quaternion.Slerp(vLLSubsegment.SubsegmentOrientation, vKneeQuat, InterpolationSpeed);
            }
        }
        else
        {
            //if (IsAdjustingSegmentAxis)
            //{
            //    vNewThighQuat = vThighQuat * vThighQuatY;
            //    vNewKneeQuat = vKneeQuat * vKneeQuatY;
            //}
            //else
            {
                vNewThighQuat = vThighQuat;
                vNewKneeQuat = vKneeQuat;
            }
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
        Vector3 vUpArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].InitRawEuler * Mathf.Rad2Deg;
        Vector3 vUpArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].CurrRawEuler * Mathf.Rad2Deg;
        Vector3 vLoArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm].InitRawEuler * Mathf.Rad2Deg;
        Vector3 vLoArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm].CurrRawEuler * Mathf.Rad2Deg;
        Vector3 vTorsoInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitRawEuler * Mathf.Rad2Deg;
        Vector3 vTorsoCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].CurrRawEuler * Mathf.Rad2Deg;
        MapArmsOrientations(vUpArmInitialRawEuler, vUpArmCurrentRawEuler, vLoArmInitialRawEuler, vLoArmCurrentRawEuler, vTorsoInitialRawEuler, vTorsoCurrentRawEuler, vUASubsegment, vLASubsegment, vTorsoSubSegment, vHipsSubsegment);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        vRightArmAnalysis.UpArTransform = vUASubsegment.AssociatedView.SubsegmentTransform;
        vRightArmAnalysis.LoArTransform = vLASubsegment.AssociatedView.SubsegmentTransform;
        vRightArmAnalysis.DeltaTime = DeltaTime;
        vRightArmAnalysis.ReferenceVector = Vector3.one;
        vRightArmAnalysis.AngleExtraction();
    }

    /// <summary>
    /// MapLeftArmSubsegment: Updates the left arm subsegment from the available sensor data.
    /// </summary>
    /// <param name="vTransformatricies">transformation matrices mapped to sensor positions.</param>
    internal void MapLeftArmSubsegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
    {
        BodySubSegment vUASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm];
        BodySubSegment vLASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm];
        BodySegment vTorsoSegment = ParentBody.GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes.SegmentType_Torso);
        BodySubSegment vTorsoSubSegment = vTorsoSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine).Value;
        BodySubSegment vHipsSubsegment = vTorsoSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine).Value;

        ////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        Vector3 vUpArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].InitRawEuler * Mathf.Rad2Deg;
        Vector3 vUpArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].CurrRawEuler * Mathf.Rad2Deg;
        Vector3 vLoArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm].InitRawEuler * Mathf.Rad2Deg;
        Vector3 vLoArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm].CurrRawEuler * Mathf.Rad2Deg;
        Vector3 vTorsoInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitRawEuler * Mathf.Rad2Deg;
        Vector3 vTorsoCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].CurrRawEuler * Mathf.Rad2Deg;
        MapArmsOrientations(vUpArmInitialRawEuler, vUpArmCurrentRawEuler, vLoArmInitialRawEuler, vLoArmCurrentRawEuler, vTorsoInitialRawEuler, vTorsoCurrentRawEuler, vUASubsegment, vLASubsegment, vTorsoSubSegment, vHipsSubsegment, false);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        LeftArmAnalysis vLeftArmAnalysis = (LeftArmAnalysis)mCurrentAnalysisSegment;
        vLeftArmAnalysis.UpArTransform = vUASubsegment.AssociatedView.SubsegmentTransform;
        vLeftArmAnalysis.LoArTransform = vLASubsegment.AssociatedView.SubsegmentTransform;
        vLeftArmAnalysis.DeltaTime = DeltaTime;
        vLeftArmAnalysis.AngleExtraction();//*/
        //testing Calibration container
        /*************** Get a list of a calibration movement type ****************************/
        //List<Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure>> vItem = mBodyFrameCalibrationContainer.GetListOfCalibrationMovement(CalibrationType.Tpose);
        /*************** possible check for null references ****************************/

        //if(vItem != null)
        //{
        /*************** Get first movement frame's CurrRawEuler of the left forearm****************************/

        //    vItem[0][BodyStructureMap.SensorPositions.SP_LeftForeArm].CurrRawEuler

        //}
    }

    /// <summary>
    /// MapArmsOrientations: Updates the arm orientations from the initial and current eulerangles.
    /// </summary>
    float vCurrentAngle = 0;
    bool vIsIncreasingAngle = true;

    public void MapArmsOrientations(Vector3 vUAInitEuler, Vector3 vUACurEuler, Vector3 vLAInitEuler, Vector3 vLACurEuler, Vector3 vTorsoInitEuler, Vector3 vTorsoCurEuler,
                                    BodySubSegment vUASubsegment, BodySubSegment vLASubsegment, BodySubSegment vTorsoSubSegment, BodySubSegment vHipsSubsegment, bool vIsRight = true)
    {
        //Upper arm

        Quaternion vUpArmInitQuatY = Quaternion.Euler(0,  vUAInitEuler.z, 0);
        Quaternion vUpArmQuatY = Quaternion.Euler(0, (vIsRight) ? vUACurEuler.z : (vUACurEuler.z), 0);
        vUpArmQuatY = Quaternion.Inverse(vUpArmInitQuatY) * vUpArmQuatY;
        vUpArmQuatY = Quaternion.Inverse(vUpArmQuatY);

        Quaternion vUpArmInitQuatX = Quaternion.Euler(vUAInitEuler.x, 0, 0);
        Quaternion vUpArmQuatX = Quaternion.Euler(vUACurEuler.x, 0, 0);
        vUpArmQuatX = Quaternion.Inverse(vUpArmInitQuatX) * vUpArmQuatX;

        Quaternion vUpArmInitQuatZ = Quaternion.Euler(0, 0, vUAInitEuler.y );
        Quaternion vUpArmQuatZ = Quaternion.Euler(0, 0, vUACurEuler.y );
        vUpArmQuatZ = Quaternion.Inverse(vUpArmInitQuatZ) * vUpArmQuatZ;
        vUpArmQuatZ = Quaternion.Inverse(vUpArmQuatZ);


        if (vIsRight)
        {
            // Make sure we have a directory to write to.
            string vDataPath = "../../RecordedData";
            if (!Directory.Exists(vDataPath))
            {
                Directory.CreateDirectory(vDataPath);
            }

            // Set the file name to write to and start the file stream.
            int vFileNameIncrement = 2;
            mRecordingFileName = string.Format("{0}/{1}_{2}.csv", vDataPath, mRecordingTitle, vFileNameIncrement);

            Debug.Log("Recording data to file: " + mRecordingFileName);
            mRecordingFileStream = new StreamWriter(mRecordingFileName, true);
            mRecordingFileStream.WriteLine(vUACurEuler.x + "," + vUACurEuler.y + "," + vUACurEuler.z);
            mRecordingFileStream.Close();
        }
        //Lower arm

        Quaternion vLoArmInitQuatY = Quaternion.Euler(0, vLAInitEuler.z, 0);
        Quaternion vLoArmQuatY = Quaternion.Euler(0, vLACurEuler.z, 0);
        vLoArmQuatY = Quaternion.Inverse(vLoArmInitQuatY) * vLoArmQuatY;
        vLoArmQuatY = Quaternion.Inverse(vLoArmQuatY);

        Quaternion vLoArmInitQuatX = Quaternion.Euler(vLAInitEuler.x, 0, 0);
        Quaternion vLoArmQuatX = Quaternion.Euler(vLACurEuler.x, 0, 0);
        vLoArmQuatX = Quaternion.Inverse(vLoArmInitQuatX) * vLoArmQuatX;

        Quaternion vLoArmInitQuatZ = Quaternion.Euler(0, 0,  vLAInitEuler.y);
        Quaternion vLoArmQuatZ = Quaternion.Euler(0, 0, vLACurEuler.y);
        vLoArmQuatZ = Quaternion.Inverse(vLoArmInitQuatZ) * vLoArmQuatZ;
        vLoArmQuatZ = Quaternion.Inverse(vLoArmQuatZ);


        Quaternion vUpArmQuat = vUpArmQuatY * vUpArmQuatX * vUpArmQuatZ;
        Quaternion vLoArmQuat = vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ;

        if (vIsRight)
        {
            //Test
            Quaternion vUpArmTestInit = Quaternion.Euler(vUAInitEuler.x, -vUAInitEuler.z, -vUAInitEuler.y);
            Quaternion vUpArmTest = Quaternion.Euler(vUACurEuler.x, -vUACurEuler.z, -vUACurEuler.y);
            Quaternion vUpArmQuatTest = Quaternion.Inverse(vUpArmTestInit) * vUpArmTest;
            vUpArmQuat = vUpArmQuatTest;

            Quaternion vLoArmTestInit = Quaternion.Euler(vLAInitEuler.x, -vLAInitEuler.z, -vLAInitEuler.y);
            Quaternion vLoArmTest = Quaternion.Euler(vLACurEuler.x, -vLACurEuler.z, -vLACurEuler.y);
            Quaternion vLoArmQuatTest = Quaternion.Inverse(vLoArmTestInit) * vLoArmTest;
            vLoArmQuat = vLoArmQuatTest;

            //Convert to orientation

            Vector3 pos = new Vector3(0f, 0f, 0f);
            Vector3 s = new Vector3(1f, 1f, 1f);
            Matrix4x4 vUpArmMat = Matrix4x4.TRS(pos, vUpArmQuat, s);

            Matrix4x4 vLoArmMat=Matrix4x4.TRS(pos, vLoArmQuat,s);

            Debug.Log("vLoArmMat="+ vLoArmMat+"vUpArmQuat :" + vUpArmQuat + "-" + "vUpArmQuatTest:" + vUpArmQuatTest);
        }

        vCurrentAngle += 0.01f;

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

        **** Calibration process for arms Shiu algorithm -Right Arm*****
        if (vIsRight)
        {
            counterR++;
        }
        else
        {
            counterL++;
        }

        if (IsCalibrating)
        {
            if (vIsRight)
            {
                if (IsCalibrating)
                {

                    // ------ UA R1------------ 
                    float[,] R1 = new float[3, 3];

                    R1[0, 0] = 0.964f;
                    R1[0, 1] = -0.249f;
                    R1[0, 2] = 0.091f;

                    R1[2, 0] = -0.102f;
                    R1[2, 1] = -0.032f;
                    R1[2, 2] = 0.994f;

                    R1[1, 0] = 0.245f;
                    R1[1, 1] = 0.968f;
                    R1[1, 2] = 0.056f;

                    Quaternion UAR1Quat = MatrixTools.MatToQuat(R1);

                    //------UA R2: -----------
                    float[,] R2 = new float[3, 3];

                    //R2[0, 0] = -0.16f;
                    //R2[0, 1] = -0.273f;
                    //R2[0, 2] = 0.949f;

                    //R2[1, 0] = 0.211f;
                    //R2[1, 1] = 0.929f;
                    //R2[1, 2] = 0.303f;

                    //R2[2, 0] = -0.964f;
                    //R2[2, 1] = 0.249f;
                    R2[2, 2] = -0.091f;

                    Quaternion UAR2Quat = MatrixTools.MatToQuat(R2);

                    // ------LA R1------------ 
                     R1 = new float[3, 3];

                    R1[0, 0] = 0.971f;
                    R1[0, 1] = -0.232f;
                    R1[0, 2] = 0.058f;

                    R1[1, 0] = 0.232f;
                    R1[1, 1] = 0.973f;
                    R1[1, 2] = 0.01f;

                    R1[2, 0] = -0.058f;
                    R1[2, 1] = 0.004f;
                    R1[2, 2] = 0.998f;

                    Quaternion LAR1Quat = MatrixTools.MatToQuat(R1);

                    //------LA R2: -----------
                    R2 = new float[3, 3];

                    //R2[0, 0] = -0.16f;
                    //R2[0, 1] = -0.273f;
                    //R2[0, 2] = 0.949f;

                    //R2[1, 0] = 0.211f;
                    //R2[1, 1] = 0.929f;
                    //R2[1, 2] = 0.303f;

                    //R2[2, 0] = -0.964f;
                    //R2[2, 1] = 0.249f;
                    //R2[2, 2] = -0.091f;

                    Quaternion LAR2Quat = MatrixTools.MatToQuat(R2);


                    // if (counterR > 1000) // apply calibration from now on// how??
                    {
                        vLoArmQuat = Quaternion.Inverse(LAR1Quat) * vLoArmQuat * LAR1Quat ; *vRLAQuatUpsZombie Quaternion.Inverse(vRLAQuatUpsZombie) 
                        vUpArmQuat = Quaternion.Inverse(UAR1Quat) * vUpArmQuat * UAR1Quat;//vRUAQuatUpsZombie); //Quaternion.Inverse *vRUAQuatUpsZombie       *(vRUAQuatUpsZombie)             
                        Debug.Log("Quaternion.Inverse(UAR1Quat)=" + Quaternion.Inverse(UAR2Quat) + "R2Quat=" + UAR2Quat);
                    }
                }

            }

        }
        //******************** lisa recording- simon****************
        //if (vIsRight)
        //{
        //    counterR++;
        //}
        //else
        //{
        //    counterL++;
        //}
        //vUAAxisRight = vUpArmQuat * Vector3.right;
        //vLAAxisRight = vLoArmQuat * Vector3.right;
        //vUAAxisUp = vUpArmQuat * Vector3.up;
        //vLAAxisUp = vLoArmQuat * Vector3.up;

        //if (vIsRight)
        //{
        //    if (counterR < 200) //T-pose-horizontal 
        //    {
        //        // Lower arm
        //        vRLAAxisRightTPose = vLAAxisRight; //right direction in T-pose ,it is ideal =(1,0,0)
        //        vRLAAxisUpTPose = vLAAxisUp;
        //        // Upper arm
        //        vRUAAxisRightTPose = vUAAxisRight;
        //        vRUAAxisUpTPose = vUAAxisUp;
        //        //Debug.Log( "Right"+counterR + ";" + vUAAxisRight + ";" + vLAAxisRight);
        //    }

        //    if (counterR < 500 && counterR > 460) // zombie pose-horizontal movement (90 degree rotation frames) -TO DO : average the up axis
        //    {
        //        // Lower arm
        //        vRLAAxisRightZombie = vLAAxisRight; //right direction in zombie pose: the ideal is (0,0,1)
        //        vRLAAxisUpZombie = vLAAxisUp;  //up direction in zombie pose ideal is (1,0,0) we suppose ideal movement around up initial axis at T-pose
        //                                       // Upper arm
        //        vRUAAxisRightZombie = vUAAxisRight;
        //        vRUAAxisUpZombie = vUAAxisUp;
        //        //sum
        //        vRLAAxisRightZombieSum += vLAAxisRight;
        //        vRLAAxisUpZombieSum += vRLAAxisUpZombie;
        //    }

        //    //right lower arm zombie

        //    //1-correct right Axis
        //    vRLAQuatRightsZombie = Quaternion.FromToRotation(vRLAAxisRightZombie, Vector3.forward);
        //    float vForwrdsaAgle = Vector3.Angle(vRLAAxisRightZombie, Vector3.forward);

        //    ///******** 2-correct up axis with Vector3.up **********
        //    float vUpsAngleRLA = Vector3.Angle(vRLAAxisUpZombie, Vector3.up);


        //    ///******** 2-a correct up axis with t-pose **********
        //    float vUpsAngleRLA1 = Vector3.Angle(vRLAAxisUpZombie, vRLAAxisUpTPose);
        //    //vRLAQuatUpsZombie = Quaternion.FromToRotation(vRLAAxisUpZombie, vRLAAxisUpTPose);

        //    //3-correct cross product of zombie and t-pose
        //    Vector3 vRightsCrossRLA = Vector3.Cross(vRLAAxisRightZombie, vRLAAxisRightTPose);
        //    float vCrossUpAngleRLA = Vector3.Angle(vRightsCrossRLA, Vector3.up);
        //    //vRLAQuatUpsZombie = Quaternion.FromToRotation(vRightsCrossRLA, Vector3.up);

        //    //4-correct cross product of zombie and vector3.right
        //    Vector3 vRightsCrossRLA1 = Vector3.Cross(vRLAAxisRightZombie, Vector3.right);
        //    float vCrossUpAngleRLA1 = Vector3.Angle(vRightsCrossRLA, Vector3.up);
        //    //Debug.Log("Right" + counterR + "rights;" + vRLAAxisRightZombie + "Ups-vec.up;" + vUpsAngleRLA + "zombie up=" + vRLAAxisUpZombie  + "Ups-tposeup;" + vUpsAngleRLA1 +"T up="+ vRLAAxisUpTPose + "cross;" + vCrossUpAngleRLA + "rightsCross=" + vRightsCrossRLA );

        //    vRLAQuatUpsZombie = Quaternion.FromToRotation(vRLAAxisUpZombie, Vector3.up);
        //    //right upper arm  zombie
        //    vRUAQuatUpsZombie = Quaternion.FromToRotation(vRUAAxisUpZombie, Vector3.up);

        //    //Debug.Log("Zombie-before-R" + counterR + ";" + vLoArmQuat * Vector3.up + ";" + vLoArmQuat * Vector3.right + ";" + vUpArmQuat * Vector3.up + ";" + vUpArmQuat * Vector3.right);

        //    if (IsCalibrating)
        //    {
        //        if (counterR > 500) // apply calibration from now on// how??
        //        {
        //            //vLoArmQuat = vLoArmQuat * (vRLAQuatUpsZombie); *vRLAQuatUpsZombie Quaternion.Inverse(vRLAQuatUpsZombie) 
        //            //vUpArmQuat = vUpArmQuat * Quaternion.Inverse(vRUAQuatUpsZombie); //Quaternion.Inverse *vRUAQuatUpsZombie       *(vRUAQuatUpsZombie)             
        //        }

        //        //Debug.Log("Zombie-After-R" + counterR + ";" + vLoArmQuat * Vector3.up + ";" + vLoArmQuat * Vector3.right + ";" + vUpArmQuat * Vector3.up + ";" + vUpArmQuat * Vector3.right);

        //        /////////////// soldier pose ////////
        //        if (counterR < 1000 && counterR > 907) // soldier pose-horizontal  (90 degree rotation frames) -TO DO : average
        //        {
        //            // Lower arm
        //            vRLAAxisRightSoldier = vLoArmQuat * Vector3.right; //right direction in zombie pose: the ideal is (0,0,1)
        //            vRLAAxisUpSoldier = vLoArmQuat * Vector3.up;  //up direction in zombie pose ideal is (1,0,0) we suppose ideal movement around up initial axis at T-pose
        //            vRLAAxisFwdSoldier = vLoArmQuat * Vector3.forward;
        //            // Upper arm
        //            vRUAAxisRightSoldier = vUpArmQuat * Vector3.right;
        //            vRUAAxisUpSoldier = vUpArmQuat * Vector3.up;
        //            vRUAAxisFwdSoldier = vUpArmQuat * Vector3.forward;

        //            vRLAAxisRightSoldierSum += vLAAxisRight;
        //            vRLAAxisUpSoldierSum += vRLAAxisUpSoldier;
        //        }

        //        //right lower arm Soldier
        //        //1- forward axis and vector3.forward
        //        vRLAQuatforwardsSoldier = Quaternion.FromToRotation(vRLAAxisFwdSoldier, Vector3.forward);
        //        float vFwdsAngleRLA = Vector3.Angle(vRLAAxisFwdSoldier, Vector3.forward);
        //        //2- cross product of rights and vector3.forward 
        //        Vector3 vCrossRightsSoldierRLA = Vector3.Cross(vRLAAxisRightSoldier, Vector3.right);
        //        vRLAQuatforwardsSoldier = Quaternion.FromToRotation(vCrossRightsSoldierRLA, Vector3.forward);
        //        float vCrossAngleSoldierRLA = Vector3.Angle(vCrossRightsSoldierRLA, Vector3.forward);

        //        //Debug.Log("Right-soldier" + counterR + "forward=" + vRLAAxisFwdSoldier + "fwd ang=" + vFwdsAngleRLA + "soldier cross=" + vCrossRightsSoldierRLA + "cross angl=" + vCrossAngleSoldierRLA);

        //        ////right uper arm  Soldier
        //        //1- forward axis and vector3.forward
        //        vRUAQuatforwardsSoldier = Quaternion.FromToRotation(vRUAAxisFwdSoldier, Vector3.forward);
        //        float vFwdsAngleRUA = Vector3.Angle(vRUAAxisFwdSoldier, Vector3.forward);

        //        //2- cross product of rights and vector3.forward 
        //        Vector3 vCrossRightsSoldierRUA = Vector3.Cross(vRUAAxisRightSoldier, Vector3.right);
        //        //vRUAQuatforwardsSoldier = Quaternion.FromToRotation(vCrossRightsSoldierRUA, Vector3.forward);
        //        float vCrossAngleSoldierRUA = Vector3.Angle(vCrossRightsSoldierRUA, Vector3.forward);
        //        Debug.Log("Right-soldier" + counterR + "forward=" + vRLAAxisFwdSoldier + "fwd ang=" + vFwdsAngleRUA + "soldier cross=" + vCrossRightsSoldierRUA + "cross angl=" + vCrossAngleSoldierRUA);
        //        Debug.Log("Soldier-before-R" + counterR + ";" + vLoArmQuat * Vector3.up + ";" + vLoArmQuat * Vector3.right + ";" + vUpArmQuat * Vector3.up + ";" + vUpArmQuat * Vector3.right);
        //        if (counterR > 1000) // apply Soldier calibration from now on//
        //        {
        //            //vLoArmQuat = vLoArmQuat * (vRLAQuatforwardsSoldier);* (vRLAQuatforwardsSoldier); *vRLAQuatUpsZombie Quaternion.Inverse
        //            vUpArmQuat = vUpArmQuat * Quaternion.Inverse(vRUAQuatforwardsSoldier); //Quaternion.Inverse *vRUAQuatUpsZombie                    
        //        }
        //        Debug.Log("Soldier-After-R" + counterR + ";" + vLoArmQuat * Vector3.up + ";" + vLoArmQuat * Vector3.right + ";" + vUpArmQuat * Vector3.up + ";" + vUpArmQuat * Vector3.right);
        //    }
        //}
        //else //left arm -TODO: Averaging the mis aligned value
        //{
        //    if (counterL < 390) //T-pose-horizontal 
        //    {
        //        //Debug.Log("Left" + counterL + ";" + vUAAxisRight + ";" + vLAAxisRight);
        //        vLLAAxisRightTPose = vLAAxisRight; //right direction in T-pose ,it is ideal(1,0,0)
        //    }
        //    if (counterL < 590 && counterL > 475)
        //    {
        //        //Debug.Log("Left" + counterL + ";" + vUAAxisRight + ";" + vLAAxisRight);
        //        vLLAAxisRightZombie = vLAAxisRight;
        //        //right direction in zombie pose the ideal is (0,0,1)
        //        //vLLAAxisRightZombie.x = -vLLAAxisRightZombie.x;
        //        vLLAAxisUpZombie = vLAAxisUp;  //up direction in zombie pose ideal is (1,0,0) we suppose ideal movement around up initial axis at t-pose
        //                                       // Upper arm
        //        vLUAAxisRightZombie = vUAAxisRight;
        //        vLUAAxisUpZombie = vUAAxisUp;

        //        vLLAAxisRightZombieSum += vLAAxisRight;
        //        vLLAAxisUpZombieSum += vLLAAxisUpZombie;
        //    }
        //    //left lower arm
        //    vLLAQuatRightsZombie = Quaternion.FromToRotation(vLLAAxisRightZombie, Vector3.forward);
        //    vLLAQuatUpsZombie = (Quaternion.FromToRotation(vLLAAxisUpZombie, Vector3.up));
        //    //left upper arm
        //    vLUAQuatRightsZombie = Quaternion.FromToRotation(vLUAAxisRightZombie, Vector3.forward);
        //    vLUAQuatUpsZombie = (Quaternion.FromToRotation(vLUAAxisUpZombie, Vector3.up));
        //    //Debug.Log(counterL + ";" + Vector3.Cross(vLLAAxisRightTPose, vLLAAxisRightZombie) + ";" + vLLAAxisUpZombie);
        //    if (counterL > 590)
        //    {
        //        vLoArmQuat = Quaternion.Inverse(vLLAQuatUpsZombie) * vLoArmQuat * vLLAQuatUpsZombie;
        //        vUpArmQuat = Quaternion.Inverse(vLUAQuatUpsZombie) * vUpArmQuat * vLUAQuatUpsZombie;
        //    }
        //}

            ///************* calibation for specific movements  *****
            //if (vIsRight)
            //{
            //    counterR++;
            //}
            //else
            //{
            //    counterL++;
            //}
            //vUAAxisRight = vUpArmQuat * Vector3.right;
            //vLAAxisRight = vLoArmQuat * Vector3.right;
            //vUAAxisUp = vUpArmQuat * Vector3.up;
            //vLAAxisUp = vLoArmQuat * Vector3.up;

            /////****************** T-pose-horizontal elbow movement*****************
            //    if (vIsRight)
            //{
            //    if (counterR < 20) //T-pose-horizontal elbow movement
            //    {
            //        //Debug.Log( "Right"+counterR + ";" + vUAAxisRight + ";" + vLAAxisRight);
            //        vRLAAxisRightTPose = vLAAxisRight; //right direction in T-pose ,it is ideal =(1,0,0)                    
            //    }

            //    if (counterR < 297 && counterR > 293) // zombie pose-horizontal elbow movement (90 degree rotation frames)
            //    {
            //        Debug.Log(counterR + ";" + vUAAxisRight + ";" + vLAAxisRight);
            //        vRLAAxisRightZombie = vLAAxisRight; //right direction in zombie pose: the ideal is (0,0,1)
            //        vRLAAxisUpZombie = vLAAxisUp;  //up direction in zombie pose ideal is (1,0,0) we suppose ideal movement around up initial axis at T-pose
            //        vRLAAxisRightZombieSum += vLAAxisRight;
            //        vRLAAxisUpZombieSum += vRLAAxisUpZombie;
            //    }

            //    //vRAQuatRights = Quaternion.FromToRotation(vRLAAxisRightZombie, Vector3.forward);
            //    vRLAQuatUpsZombie = Quaternion.FromToRotation(vRLAAxisUpZombie, Vector3.up);

            //    //Debug.Log("Right" + counterR + ";" + Vector3.Cross(vRLAAxisRightTPose, vRLAAxisRightZombie)  + ";" + vLLAQuatUpsZombie);
            //    if (counterR > 296) // apply calibration from now on// this may mess up T-pose at first, TODO: fix it!
            //    {
            //        vLoArmQuat = vRLAQuatUpsZombie * vLoArmQuat;
            //    }
            //}

            //else //left arm
            //{

            //    if (counterL < 20) //T-pose-horizontal elbow
            //    {
            //        Debug.Log("Left" + counterL + ";" + vUAAxisRight + ";" + vLAAxisRight);
            //        vLLAAxisRightTPose = vLAAxisRight; //right direction in T-pose ,it is ideal(1,0,0)

            //    }
            //    if (counterL < 297 && counterL > 293)
            //    {
            //        Debug.Log("Left" + counterL + ";" + vUAAxisRight + ";" + vLAAxisRight);
            //        vLLAAxisRightZombie = vLAAxisRight;
            //        //right direction in zombie pose the ideal is (0,0,1)
            //        //vLLAAxisRightZombie.x = -vLLAAxisRightZombie.x;
            //        vLLAAxisUpZombie = vLAAxisUp;  //up direction in zombie pose ideal is (1,0,0) we suppose ideal movement around up initial axis at t-pose
            //                                       // vLLAAxisUpZombie.x = -vLLAAxisUpZombie.x;
            //        vLLAAxisRightZombieSum += vLAAxisRight;
            //        vLLAAxisUpZombieSum += vLLAAxisUpZombie;
            //    }

            //    //vLAQuatRights = Quaternion.FromToRotation(vLLAAxisRightZombie, Vector3.forward);
            //    vLLAQuatUpsZombie = (Quaternion.FromToRotation(vLLAAxisUpZombie, Vector3.up));
            //    //Debug.Log(counterL + ";" + vLAQuatRights + ";" + vLAQuatUps);
            //    Debug.Log(counterL + ";" + Vector3.Cross(vLLAAxisRightTPose, vLLAAxisRightZombie) + ";" + vLLAAxisUpZombie);
            //    if (counterL > 296)
            //    {
            //        vLoArmQuat = vLoArmQuat* vLLAQuatUpsZombie;
            //    }
            //}
            //  **************** horizontal shoulder movement***********
            //if (vIsRight)
            //{
            //    if (counterR < 35) //T-pose-horizontal shoulder movement
            //    {
            //        vRLAAxisRightTPose = vLAAxisRight; //right direction in T-pose ,it is ideal =(1,0,0) 
            //    }
            //    Debug.Log(counterR + ";beforeif" + vUAAxisRight + ";" + vLAAxisRight + ";" + vUAAxisUp + ";" + vLAAxisUp);
            //    if (counterR < 202 && counterR > 195)
            //    {
            //        Debug.Log(counterR + ";" + vUAAxisRight + ";" + vLAAxisRight + ";" + vUAAxisUp + ";" + vLAAxisUp);
            //        vRLAAxisRightZombie = vLAAxisRight; //right direction in zombie pose: the ideal is (0,0,1)
            //        vRLAAxisUpZombie = vLAAxisUp;  //up direction in zombie pose, ideal is (1,0,0) we suppose ideal movement around up initial axis at T-pose
            //        vRLAAxisRightZombieSum += vLAAxisRight;
            //        vRLAAxisUpZombieSum += vRLAAxisUpZombie;
            //    }

            //    //vRLAAxisUpZombie = vRLAAxisUpZombieSum / 3;
            //    //vRLAAxisRightZombie = vRLAAxisRightZombieSum / 3;

            //    vRAQuatRights = Quaternion.FromToRotation(vRLAAxisRightZombie, Vector3.forward);
            //    vRAQuatUps = Quaternion.FromToRotation(vRLAAxisUpZombie, Vector3.up);

            //    //Debug.Log("Right" + counterR + ";" + Vector3.Cross(vRLAAxisRightTPose, vRLAAxisRightZombie) + ";" + vRAQuatRights + ";" + vRAQuatUps);
            //    if (counterR > 201) // apply calibration from now on// this may mess up T-pose at first, TODO: fix it!
            //    {
            //        vLoArmQuat = vRAQuatUps * vLoArmQuat;
            //    }
            //}
            //else //left arm
            //{
            //    if (counterL < 35) //T-pose-horizontal shoulder
            //    {
            //        //Debug.Log("Left" + counterL + ";" + vUAAxisRight + ";" + vLAAxisRight);
            //        vLLAAxisRightTPose = vLAAxisRight; //right direction in T-pose ,it is ideal(1,0,0)
            //    }

            //    if (counterL < 202 && counterL > 195)
            //    {
            //        //Debug.Log("Left" + counterL + ";" + vUAAxisRight + ";" + vLAAxisRight);
            //        vLLAAxisRightZombie = vLAAxisRight;
            //        //right direction in zombie pose the ideal is (0,0,1)
            //        //vLLAAxisRightZombie.x = -vLLAAxisRightZombie.x;
            //        vLLAAxisUpZombie = vLAAxisUp;  //up direction in zombie pose ideal is (1,0,0) we suppose ideal movement around up initial axis at t-pose
            //                                       // vLLAAxisUpZombie.x = -vLLAAxisUpZombie.x;
            //        vLLAAxisRightZombieSum += vLAAxisRight;
            //        vLLAAxisUpZombieSum += vLLAAxisUpZombie;
            //    }

            //    //vLLAAxisUpZombie = vLLAAxisUpZombieSum / 3;
            //    //vLLAAxisRightZombie = vLLAAxisRightZombieSum / 3;

            //    vLAQuatRights = Quaternion.FromToRotation(vLLAAxisRightZombie, Vector3.forward);
            //    vLAQuatUps = (Quaternion.FromToRotation(vLLAAxisUpZombie, Vector3.up));
            //    //Debug.Log(counterL + ";" + vLAQuatRights + ";" + vLAQuatUps);
            //    Debug.Log(counterL + ";" + Vector3.Cross(vLLAAxisRightTPose, vLLAAxisRightZombie) + ";" + vLLAAxisUpZombie);
            //    if (counterL > 201)
            //    {
            //        vLoArmQuat = vLAQuatUps * vLoArmQuat;
            //    }
            //}           
           //} //calibration end

            if (IsProjectingXY)
        {
            vLANewRight = Vector3.ProjectOnPlane(vLAAxisRight, vUAAxisForward);
            vLANewUp = Vector3.Cross(vLAAxisForward, vLANewRight);
            vLANewForward = Vector3.Cross(vLANewRight, vLANewUp);
            vLoArmQuat = Quaternion.LookRotation(vLANewForward, vLANewUp);
        }
        else if (IsProjectingXZ)
        {
            vLANewRight = Vector3.ProjectOnPlane(vLAAxisRight, vUAAxisUp);
            vLANewUp = Vector3.Cross(vLAAxisForward, vLANewRight);
            vLANewForward = Vector3.Cross(vLANewRight, vLANewUp);
            vLoArmQuat = Quaternion.LookRotation(vLANewForward, vLANewUp);
        }
        else if (IsProjectingYZ)
        {
            vLANewRight = Vector3.ProjectOnPlane(vLAAxisRight, vUAAxisRight);
            vLANewUp = Vector3.Cross(vLAAxisForward, vLANewRight);
            vLANewForward = Vector3.Cross(vLANewRight, vLANewUp);
            vLoArmQuat = Quaternion.LookRotation(vLANewForward, vLANewUp);
        }

        vUAAxisUp = vUpArmQuat * Vector3.up;
        vUAAxisRight = vUpArmQuat * Vector3.right;
        vUAAxisForward = vUpArmQuat * Vector3.forward;

        vLAAxisUp = vLoArmQuat * Vector3.up;
        vLAAxisRight = vLoArmQuat * Vector3.right;
        vLAAxisForward = vLoArmQuat * Vector3.forward;

        if (IsFusingSubSegments)
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
                if (!vIsRight)
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
            //if(vIsRight) Debug.Log(SegmentAnalysis.GetSignedAngle(vNewUAAxisRight, vNewLAAxisRight, vNewAxisUp));
            //vLoArmQuat = Quaternion.AngleAxis(SegmentAnalysis.GetSignedAngle(vNewLAAxisRight, vNewUAAxisRight, vNewAxisUp), vNewAxisUp);
        }

        Quaternion vNewUpArmQuat = Quaternion.identity;
        Quaternion vNewLoArmQuat = Quaternion.identity;

        if (IsUsingInterpolation)
        {
            if (IsAdjustingSegmentAxis)
            {
                vNewUpArmQuat = Quaternion.Slerp(vUASubsegment.SubsegmentOrientation, vUpArmQuat * Quaternion.Inverse(vUpArmQuatX), InterpolationSpeed);
                vNewLoArmQuat = Quaternion.Slerp(vLASubsegment.SubsegmentOrientation, vLoArmQuat * Quaternion.Inverse(vLoArmQuatX), InterpolationSpeed);
            }
            else
            {
                vNewUpArmQuat = Quaternion.Slerp(vUASubsegment.SubsegmentOrientation, vUpArmQuat, InterpolationSpeed);
                vNewLoArmQuat = Quaternion.Slerp(vLASubsegment.SubsegmentOrientation, vLoArmQuat, InterpolationSpeed);
            }
        }
        else
        {
            if (IsAdjustingSegmentAxis)
            {
                vNewUpArmQuat = vUpArmQuat * Quaternion.Inverse(vUpArmQuatX);
                vNewLoArmQuat = vLoArmQuat * Quaternion.Inverse(vLoArmQuatX);
            }
            else
            {
                vNewUpArmQuat = vUpArmQuat;
                vNewLoArmQuat = vLoArmQuat;
            }
        }

        vUASubsegment.UpdateSubsegmentOrientation(vNewUpArmQuat, 1, true);
        vLASubsegment.UpdateSubsegmentOrientation(vNewLoArmQuat, 1, true);
    }

    public float getAngleDelta(float vCurAngle, float vInitAngle)
    {
        float vResult = 0;

        if (vCurAngle > 0)
        {
            if (Mathf.Abs(vCurAngle) > Mathf.Abs(vInitAngle))
            {
                vResult = Mathf.Abs(vCurAngle) - Mathf.Abs(vInitAngle);
            }
            else
            {
                vResult = Mathf.Abs(vInitAngle) - Mathf.Abs(vCurAngle);
            }
        }
        else
        {
            if (Mathf.Abs(vCurAngle) > Mathf.Abs(vInitAngle))
            {
                vResult = Mathf.Abs(vInitAngle) - Mathf.Abs(vCurAngle);
            }
            else
            {
                vResult = Mathf.Abs(vCurAngle) - Mathf.Abs(vInitAngle);
            }
        }

        return vResult;
    }

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
            Transform vSubSegmentTransform = vRendered.GetSubSegment((BodyStructureMap.SubSegmentTypes)vsubSegment.Key);
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

#endif

}

