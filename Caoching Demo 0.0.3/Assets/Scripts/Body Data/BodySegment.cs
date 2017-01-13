
/** 
* @file BodySegment.cs
* @brief Contains the BodySegment  class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
 using Assets.Scripts.Body_Data.view;
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Data.CalibrationData;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.Body_Pipeline.Analysis;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Trunk;
using Assets.Scripts.Utils;
using MathNet.Numerics.LinearAlgebra;

/// <summary>
/// BodySegment class: represents one abstracted reprensentation of a body segment.
/// </summary>
public partial class BodySegment
{

#if !SEGMENTS_DEBUG && !SEGMENTS_DEBUG_SIM
    public BodyFrameCalibrationContainer BodyFrameCalibrationContainer { get; internal set; }
    //Segment Type 
    public BodyStructureMap.SegmentTypes SegmentType;

    //Body SubSegments 
    public Dictionary<int, BodySubSegment> BodySubSegmentsDictionary = new Dictionary<int, BodySubSegment>();
    private BodyFrameCalibrationContainer mBodyFrameCalibrationContainer;
    public Body ParentBody;
    public static bool GBodyFrameUsingQuaternion = false;
    //Is segment tracked (based on body type) 
    public bool IsTracked = true;
    public bool IsReseting = false;
    public int ResetCounter = 0;
    static public bool IsTrackingHeight = true;
    static public bool IsTrackingGait = false;
    static public bool IsTrackingHips = false;
    static public bool IsAdjustingSegmentAxis = true;
    static public bool IsFusingSubSegments = true;
    static public bool IsProjectingXZ = false;
    static public bool IsProjectingXY = false;
    static public bool IsProjectingYZ = false;
    static public bool IsHipsEstimateForward = true;
    static public bool IsHipsEstimateUp = false;
    static public bool IsUsingInterpolation = true;
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

    static int svRightCounter = 0;
    static int svLeftCounter = 0;
    static int svStandCounter = 0;

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
    public void UpdateInitialSensorsData(BodyFrame vFrame)
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

    /// <summary>
    /// Applies a rotation to align the referenced input rotation with the position of the sensor
    /// </summary> 
    /// <param name="vSensorPosition">the position of the sensor</param>
    /// <param name="vInputRotation">the rotation</param>
    internal void ApplySensorMappingRotation(BodyStructureMap.SensorPositions vSensorPosition, ref Quaternion vInputRotation)
    {
        vInputRotation *= BodyStructureMap.Instance.PniSensorRotationToSegmentMap[vSensorPosition];
    }

    void Q2HPR(Quaternion vQ, ref Vector3 ypr)
    {
        ypr[0] = Mathf.Rad2Deg * Mathf.Atan2((2 * vQ[0] * vQ[1] + 2 * vQ[3] * vQ[2]), (2 * vQ[3] * vQ[3] + 2 * vQ[0] * vQ[0] - 1));
        ypr[1] = Mathf.Rad2Deg * Mathf.Asin(-(2 * vQ[0] * vQ[2] - 2 * vQ[3] * vQ[1]));
        ypr[2] = Mathf.Rad2Deg * Mathf.Atan2((2 * vQ[1] * vQ[2] + 2 * vQ[3] * vQ[0]), (2 * vQ[3] * vQ[3] + 2 * vQ[2] * vQ[2] - 1));
        if (ypr[0] < 0)
            ypr[0] += 360;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vQ"></param>
    public static Quaternion PniQuaternionConversion(Quaternion vQ)
    {
        Quaternion vReturn = Quaternion.identity;
        vReturn.x = -vQ.y;
        vReturn.y = vQ.z;
        vReturn.z = -vQ.x;
        vReturn.w = vQ.w;
        return vReturn;
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
    internal void MapTrunkSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
    {
        TrunkAnalysis vTorsoAnalysis = (TrunkAnalysis)mCurrentAnalysisSegment;

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
        BodyFrame.Vect4 vTrunkInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitRawData;
        BodyFrame.Vect4 vTrunkCur = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].CurrRawData;

        Quaternion vTrunkQuat = Quaternion.identity;
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

        if (GBodyFrameUsingQuaternion)
        {
            // a rotation applied to the sensor data to align it with a body segment
            Quaternion vTrunkInitRawQuat = Quaternion.Euler(0,180,0); // new Quaternion(-vTrunkInit.y, vTrunkInit.z, -vTrunkInit.x, vTrunkInit.w);
            Quaternion vTrunkCurRawQuat = new Quaternion(-vTrunkCur.y, vTrunkCur.z, -vTrunkCur.x, vTrunkCur.w);
            Quaternion vNewTrunkQuat = Quaternion.Inverse(Quaternion.Euler(90,0 ,0)) * Quaternion.Inverse(vTrunkInitRawQuat) * vTrunkCurRawQuat * Quaternion.Euler(90, 0, 0);

            //Apply transform on Trunk
            if (IsUsingInterpolation)
            {
                vHipQuat = Quaternion.Slerp(vLSSubsegment.SubsegmentOrientation, Quaternion.Euler(vUpAngle, 0, 0) * Quaternion.Euler(0, vForwardAngle, 0), InterpolationSpeed);
                vTrunkQuat = Quaternion.Slerp(vUSSubsegment.SubsegmentOrientation, Quaternion.Inverse(vHipQuat) * vNewTrunkQuat, InterpolationSpeed);
            }
            else
            {
                vHipQuat = Quaternion.Euler(vUpAngle, 0, 0) * Quaternion.Euler(0, vForwardAngle, 0);
                vTrunkQuat = Quaternion.Inverse(vHipQuat) * vNewTrunkQuat;
            }
        }
        else
        {
            Vector3 vTorsoInitialRawEuler = new Vector3(vTrunkInit.x, vTrunkInit.y, vTrunkInit.z) * 180f / Mathf.PI;
            Vector3 vTorsoCurrentRawEuler = new Vector3(vTrunkCur.x, vTrunkCur.y, vTrunkCur.z) * 180f / Mathf.PI;

            Quaternion vTorsoInitQuat = Quaternion.Euler(0, -vTorsoInitialRawEuler.z, 0);
            Quaternion vTorsoQuatY = Quaternion.Euler(0, -vTorsoCurrentRawEuler.z, 0);
            vTorsoQuatY = Quaternion.Inverse(vTorsoInitQuat) * vTorsoQuatY;

            vTorsoInitQuat = Quaternion.Euler(-vTorsoInitialRawEuler.x, 0, 0);
            Quaternion vTorsoQuatX = Quaternion.Euler(-vTorsoCurrentRawEuler.x, 0, 0);
            vTorsoQuatX = Quaternion.Inverse(vTorsoInitQuat) * vTorsoQuatX;
            
            vTorsoInitQuat = Quaternion.Euler(0, 0, vTorsoInitialRawEuler.y);
            Quaternion vTorsoQuatZ = Quaternion.Euler(0, 0, vTorsoCurrentRawEuler.y);
            vTorsoQuatZ = Quaternion.Inverse(vTorsoInitQuat) * vTorsoQuatZ;

            // Apply Results To Trunk 
            if (IsUsingInterpolation)
            {
                vHipQuat = Quaternion.Slerp(vLSSubsegment.SubsegmentOrientation, Quaternion.Euler(vUpAngle, 0, 0) * Quaternion.Euler(0, vForwardAngle, 0), InterpolationSpeed);
                vTrunkQuat = Quaternion.Slerp(vUSSubsegment.SubsegmentOrientation, Quaternion.Inverse(vHipQuat) * vTorsoQuatY * vTorsoQuatX * vTorsoQuatZ, InterpolationSpeed);
            }
            else
            {
                vHipQuat = Quaternion.Euler(vUpAngle, 0, 0) * Quaternion.Euler(0, vForwardAngle, 0);
                vTrunkQuat = Quaternion.Inverse(vHipQuat) * vTorsoQuatY * vTorsoQuatX * vTorsoQuatZ;
            }
        }

        //Apply results
        vUSSubsegment.UpdateSubsegmentOrientation(vTrunkQuat, 1, true);
        vLSSubsegment.UpdateSubsegmentOrientation(vHipQuat, 3, true);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        //Update the analysis inputs
        vTorsoAnalysis.TrunkTransform = vUSSubsegment.AssociatedView.SubsegmentTransform;
        vTorsoAnalysis.HipGlobalTransform = vLSSubsegment.AssociatedView.SubsegmentTransform;
        vTorsoAnalysis.DeltaTime = DeltaTime;
        vTorsoAnalysis.AngleExtraction();
        //UpdateHipPosition(vLSSubsegment);//*/
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
        BodyFrame.Vect4 vThighInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh].InitRawData;
        BodyFrame.Vect4 vThighCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh].CurrRawData;
        BodyFrame.Vect4 vKneeInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf].InitRawData;
        BodyFrame.Vect4 vKneeCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf].CurrRawData;

        if (GBodyFrameUsingQuaternion)
        {
            // a rotation applied to the sensor data to align it with a body segment
            Quaternion vThighInitialRawQuat = new Quaternion(-vThighInit.y, vThighInit.z, -vThighInit.x, vThighInit.w);
            Quaternion vThighCurrentRawQuat = new Quaternion(-vThighCurr.y, vThighCurr.z, -vThighCurr.x, vThighCurr.w);
            Quaternion vKneeInitialRawQuat = new Quaternion(-vKneeInit.y, vKneeInit.z, -vKneeInit.x, vKneeInit.w);
            Quaternion vKneeCurrentRawQuat = new Quaternion(-vKneeCurr.y, vKneeCurr.z, -vKneeCurr.x, vKneeCurr.w);
            
            MapLegsOrientationsQuat(vThighInitialRawQuat, vThighCurrentRawQuat, vKneeInitialRawQuat, vKneeCurrentRawQuat, vULSubsegment, vLLSubsegment, vHipsSubsegment);
        }
        else
        {
            Vector3 vThighInitialRawEuler = new Vector3(vThighInit.x, vThighInit.y, vThighInit.z) * Mathf.Rad2Deg;
            Vector3 vThighCurrentRawEuler = new Vector3(vThighCurr.x, vThighCurr.y, vThighCurr.z) * Mathf.Rad2Deg;
            Vector3 vKneeInitialRawEuler = new Vector3(vKneeInit.x, vKneeInit.y, vKneeInit.z) * Mathf.Rad2Deg;
            Vector3 vKneeCurrentRawEuler = new Vector3(vKneeCurr.x, vKneeCurr.y, vKneeCurr.z) * Mathf.Rad2Deg;

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

        BodyFrame.Vect4 vThighInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh].InitRawData;
        BodyFrame.Vect4 vThighCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh].CurrRawData;
        BodyFrame.Vect4 vKneeInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf].InitRawData;
        BodyFrame.Vect4 vKneeCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf].CurrRawData; 

        if (GBodyFrameUsingQuaternion)
        {
            Quaternion vThighInitialRawQuat = PniQuaternionConversion(new Quaternion(vThighInit.x, vThighInit.y, vThighInit.z, vThighInit.w));
            Quaternion vThighCurrentRawQuat = PniQuaternionConversion(new Quaternion(vThighCurr.x, vThighCurr.y,  vThighCurr.z ,vThighCurr.w));
            Quaternion vKneeInitialRawQuat = PniQuaternionConversion(new Quaternion(vKneeInit.x, vKneeInit.y, vKneeInit.z, vKneeInit.w));
            Quaternion vKneeCurrentRawQuat = PniQuaternionConversion(new Quaternion(vKneeCurr.x, vKneeCurr.y, vKneeCurr.z, vKneeCurr.w));
 
            MapLegsOrientationsQuat(vThighInitialRawQuat, vThighCurrentRawQuat, vKneeInitialRawQuat, vKneeCurrentRawQuat, vULSubsegment, vLLSubsegment, vHipsSubsegment, false);
        }
        else
        {
            Vector3 vThighInitialRawEuler = new Vector3(vThighInit.x, vThighInit.y, vThighInit.z) * Mathf.Rad2Deg;
            Vector3 vThighCurrentRawEuler = new Vector3(vThighCurr.x, vThighCurr.y, vThighCurr.z) * Mathf.Rad2Deg;
            Vector3 vKneeInitialRawEuler = new Vector3(vKneeInit.x, vKneeInit.y, vKneeInit.z) * Mathf.Rad2Deg;
            Vector3 vKneeCurrentRawEuler = new Vector3(vKneeCurr.x, vKneeCurr.y, vKneeCurr.z) * Mathf.Rad2Deg;

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

        // a rotation applied to the sensor data to align it with a body segment
        Quaternion vULInitRawQuat = new Quaternion(-vULInitQuat.y, vULInitQuat.z, -vULInitQuat.x, vULInitQuat.w);
        Quaternion vLLInitRawQuat = new Quaternion(-vLLInitQuat.y, vLLInitQuat.z, -vLLInitQuat.x, vLLInitQuat.w);
        Quaternion vULCurRawQuat = new Quaternion(-vULCurQuat.y, vULCurQuat.z, -vULCurQuat.x, vULCurQuat.w);
        Quaternion vLLCurRawQuat = new Quaternion(-vLLCurQuat.y, vLLCurQuat.z, -vLLCurQuat.x, vLLCurQuat.w);

        Quaternion vThighQuat = Quaternion.Inverse(Quaternion.Euler(0, 0, 0)) * Quaternion.Inverse(vULInitRawQuat) * vULCurRawQuat * Quaternion.Euler(0, 0, 0);
        Quaternion vKneeQuat = Quaternion.Inverse(Quaternion.Euler(0, 0, 0)) * Quaternion.Inverse(vLLInitRawQuat) * vULCurRawQuat * Quaternion.Euler(0, 0, 0);

        Quaternion vNewThighQuat = Quaternion.identity;
        Quaternion vNewKneeQuat = Quaternion.identity;

        //Apply transform on Trunk
        if (IsUsingInterpolation)
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
        //vLLSubsegment.UpdateSubsegmentOrientation(vNewKneeQuat, 1, true);
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

        Quaternion vNewThighQuat = Quaternion.identity;
        Quaternion vNewKneeQuat = Quaternion.identity;

        if (IsUsingInterpolation)
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

        BodyFrame.Vect4 vUpArmInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].InitRawData;
        BodyFrame.Vect4 vUpArmCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].CurrRawData;
        BodyFrame.Vect4 vLoArmInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm].InitRawData;
        BodyFrame.Vect4 vLoArmCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm].CurrRawData;
        BodyFrame.Vect4 vTorsoInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].InitRawData;
        BodyFrame.Vect4 vTorsoCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].CurrRawData;

        if (GBodyFrameUsingQuaternion)
        {
            Quaternion vUpArmInitialRawQuat = PniQuaternionConversion(new Quaternion(vUpArmInit.x, vUpArmInit.y, vUpArmInit.z, vUpArmInit.w));
            Quaternion vUpArmCurrentRawQuat = PniQuaternionConversion(new Quaternion(vUpArmCurr.x, vUpArmCurr.y, vUpArmCurr.z, vUpArmCurr.w));
            Quaternion vLoArmInitialRawQuat = PniQuaternionConversion(new Quaternion(vLoArmInit.x, vLoArmInit.y, vLoArmInit.z, vLoArmInit.w));
            Quaternion vLoArmCurrentRawQuat = PniQuaternionConversion(new Quaternion(vLoArmCurr.x, vLoArmCurr.y, vLoArmCurr.z, vLoArmCurr.w));
            Quaternion vTorsoInitialRawQuat = PniQuaternionConversion(new Quaternion(vTorsoInit.x, vTorsoInit.y, vTorsoInit.z, vTorsoInit.w));
            Quaternion vTorsoCurrentRawQuat = PniQuaternionConversion(new Quaternion(vTorsoCurr.x, vTorsoCurr.y, vTorsoCurr.z, vTorsoCurr.w));

            //map orientation quaternion
            MapArmsOrientationsQuat(vUpArmInitialRawQuat, vUpArmCurrentRawQuat, vLoArmInitialRawQuat, vLoArmCurrentRawQuat, vTorsoInitialRawQuat, vTorsoCurrentRawQuat, vUASubsegment, vLASubsegment, vTorsoSubSegment, vHipsSubsegment, true);
        }
        else
        {
            Vector3 vUpArmInitialRawEuler = new Vector3(vUpArmInit.x, vUpArmInit.y, vUpArmInit.z) * Mathf.Rad2Deg;
            Vector3 vUpArmCurrentRawEuler = new Vector3(vUpArmCurr.x, vUpArmCurr.y, vUpArmCurr.z) * Mathf.Rad2Deg;
            Vector3 vLoArmInitialRawEuler = new Vector3(vLoArmInit.x, vLoArmInit.y, vLoArmInit.z) * Mathf.Rad2Deg;
            Vector3 vLoArmCurrentRawEuler = new Vector3(vLoArmCurr.x, vLoArmCurr.y, vLoArmCurr.z) * Mathf.Rad2Deg;
            Vector3 vTorsoInitialRawEuler = new Vector3(vTorsoInit.x, vTorsoInit.y, vTorsoInit.z) * Mathf.Rad2Deg;
            Vector3 vTorsoCurrentRawEuler = new Vector3(vTorsoCurr.x, vTorsoCurr.y, vTorsoCurr.z) * Mathf.Rad2Deg;

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
    /// <param name="vTransformatricies">transformation matrices mapped to sensor positions.</param>
    internal void MapLeftArmSubsegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
    {
        BodySubSegment vUASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm];
        BodySubSegment vLASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm];
        BodySegment vTorsoSegment = ParentBody.GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes.SegmentType_Torso);
        BodySubSegment vTorsoSubSegment = vTorsoSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine).Value;
        BodySubSegment vHipsSubsegment = vTorsoSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine).Value;

        ////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        BodyFrame.Vect4 vUpArmInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].InitRawData;
        BodyFrame.Vect4 vUpArmCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].CurrRawData;
        BodyFrame.Vect4 vLoArmInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm].InitRawData;
        BodyFrame.Vect4 vLoArmCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm].CurrRawData;
        BodyFrame.Vect4 vTorsoInit = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitRawData;
        BodyFrame.Vect4 vTorsoCurr = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].CurrRawData;

        if (GBodyFrameUsingQuaternion)
        {
            Quaternion vUpArmInitialRawQuat = PniQuaternionConversion(new Quaternion(vUpArmInit.x, vUpArmInit.y, vUpArmInit.z, vUpArmInit.w));
            Quaternion vUpArmCurrentRawQuat = PniQuaternionConversion(new Quaternion(vUpArmCurr.x, vUpArmCurr.y, vUpArmCurr.z, vUpArmCurr.w));
            Quaternion vLoArmInitialRawQuat = PniQuaternionConversion(new Quaternion(vLoArmInit.x, vLoArmInit.y, vLoArmInit.z, vLoArmInit.w));
            Quaternion vLoArmCurrentRawQuat = PniQuaternionConversion(new Quaternion(vLoArmCurr.x, vLoArmCurr.y, vLoArmCurr.z, vLoArmCurr.w));
            Quaternion vTorsoInitialRawQuat = PniQuaternionConversion(new Quaternion(vTorsoInit.x, vTorsoInit.y, vTorsoInit.z, vTorsoInit.w));
            Quaternion vTorsoCurrentRawQuat = PniQuaternionConversion(new Quaternion(vTorsoCurr.x, vTorsoCurr.y, vTorsoCurr.z, vTorsoCurr.w)); 

            MapArmsOrientationsQuat(vUpArmInitialRawQuat, vUpArmCurrentRawQuat, vLoArmInitialRawQuat, vLoArmCurrentRawQuat, vTorsoInitialRawQuat, vTorsoCurrentRawQuat, vUASubsegment, vLASubsegment, vTorsoSubSegment, vHipsSubsegment);
        }
        else
        {
            Vector3 vUpArmInitialRawEuler = new Vector3(vUpArmInit.x, vUpArmInit.y, vUpArmInit.z) * Mathf.Rad2Deg;
            Vector3 vUpArmCurrentRawEuler = new Vector3(vUpArmCurr.x, vUpArmCurr.y, vUpArmCurr.z) * Mathf.Rad2Deg;
            Vector3 vLoArmInitialRawEuler = new Vector3(vLoArmInit.x, vLoArmInit.y, vLoArmInit.z) * Mathf.Rad2Deg;
            Vector3 vLoArmCurrentRawEuler = new Vector3(vLoArmCurr.x, vLoArmCurr.y, vLoArmCurr.z) * Mathf.Rad2Deg;
            Vector3 vTorsoInitialRawEuler = new Vector3(vTorsoInit.x, vTorsoInit.y, vTorsoInit.z) * Mathf.Rad2Deg;
            Vector3 vTorsoCurrentRawEuler = new Vector3(vTorsoCurr.x, vTorsoCurr.y, vTorsoCurr.z) * Mathf.Rad2Deg;

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

        //map sensors by applying a rotation on the sensor data
        //ApplySensorMappingRotation(BodyStructureMap.SensorPositions.SP_LeftUpperArm, ref vUAInitQuat);
        //ApplySensorMappingRotation(BodyStructureMap.SensorPositions.SP_LeftUpperArm, ref vUACurQuat);
        //ApplySensorMappingRotation(BodyStructureMap.SensorPositions.SP_LeftForeArm, ref vLAInitQuat);
        //ApplySensorMappingRotation(BodyStructureMap.SensorPositions.SP_LeftForeArm, ref vLACurQuat);

        Quaternion vUpArmQuat = Quaternion.Inverse(vUAInitQuat) * vUACurQuat;
        Quaternion vLoArmQuat = Quaternion.Inverse(vLAInitQuat) * vLACurQuat;

        Quaternion vNewUpArmQuat = Quaternion.identity;
        Quaternion vNewLoArmQuat = Quaternion.identity;

        if (IsUsingInterpolation)
        {
            vNewUpArmQuat = Quaternion.Slerp(vUASubsegment.SubsegmentOrientation, vUpArmQuat, InterpolationSpeed);
            vNewLoArmQuat = Quaternion.Slerp(vLASubsegment.SubsegmentOrientation, vLoArmQuat, InterpolationSpeed);
        }
        else
        {
            vNewUpArmQuat = vUpArmQuat;
            vNewLoArmQuat = vLoArmQuat;
        }


        //vUASubsegment.UpdateSubsegmentOrientation(vNewUpArmQuat, 1, true);
        //vLASubsegment.UpdateSubsegmentOrientation(vNewLoArmQuat, 1, true);
    }


    /// <summary>
    /// MapArmsOrientations: Updates the arm orientations from the initial and current eulerangles.
    /// </summary>

    public void MapArmsOrientations(Vector3 vUAInitEuler, Vector3 vUACurEuler, Vector3 vLAInitEuler, Vector3 vLACurEuler, Vector3 vTorsoInitEuler, Vector3 vTorsoCurEuler,
                                    BodySubSegment vUASubsegment, BodySubSegment vLASubsegment, BodySubSegment vTorsoSubSegment, BodySubSegment vHipsSubsegment, bool vIsRight = true)
    {

        //Upper arm
        Quaternion vUpArmInitQuatY = Quaternion.Euler(0, (vIsRight) ? vUAInitEuler.z : (vUAInitEuler.z), 0);
        Quaternion vUpArmQuatY = Quaternion.Euler(0, (vIsRight) ? vUACurEuler.z : (vUACurEuler.z), 0);
        vUpArmQuatY = Quaternion.Inverse(vUpArmInitQuatY) * vUpArmQuatY;
        vUpArmQuatY = Quaternion.Inverse(vUpArmQuatY);

        Quaternion vUpArmInitQuatX = Quaternion.Euler(vUAInitEuler.x, 0, 0);
        Quaternion vUpArmQuatX = Quaternion.Euler(vUACurEuler.x, 0, 0);
        vUpArmQuatX = Quaternion.Inverse(vUpArmInitQuatX) * vUpArmQuatX;

        Quaternion vUpArmInitQuatZ = Quaternion.Euler(0, 0, (vIsRight) ? vUAInitEuler.y : (vUAInitEuler.y));
        Quaternion vUpArmQuatZ = Quaternion.Euler(0, 0, (vIsRight) ? vUACurEuler.y : (vUACurEuler.y));
        vUpArmQuatZ = Quaternion.Inverse(vUpArmInitQuatZ) * vUpArmQuatZ;
        vUpArmQuatZ = Quaternion.Inverse(vUpArmQuatZ);

        Quaternion vLoArmInitQuatY = Quaternion.Euler(0, (vIsRight) ? vLAInitEuler.z : (vLAInitEuler.z), 0);
        Quaternion vLoArmQuatY = Quaternion.Euler(0, (vIsRight) ? vLACurEuler.z : (vLACurEuler.z), 0);
        vLoArmQuatY = Quaternion.Inverse(vLoArmInitQuatY) * vLoArmQuatY;
        vLoArmQuatY = Quaternion.Inverse(vLoArmQuatY);

        Quaternion vLoArmInitQuatX = Quaternion.Euler(vLAInitEuler.x, 0, 0);
        Quaternion vLoArmQuatX = Quaternion.Euler(vLACurEuler.x, 0, 0);
        vLoArmQuatX = Quaternion.Inverse(vLoArmInitQuatX) * vLoArmQuatX;

        Quaternion vLoArmInitQuatZ = Quaternion.Euler(0, 0, (vIsRight) ? vLAInitEuler.y : (vLAInitEuler.y));
        Quaternion vLoArmQuatZ = Quaternion.Euler(0, 0, (vIsRight) ? vLACurEuler.y : (vLACurEuler.y));
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

        vUAAxisUp = vUpArmQuat * Vector3.up;
        vUAAxisRight = vUpArmQuat * Vector3.right;
        vUAAxisForward = vUpArmQuat * Vector3.forward;

        vLAAxisUp = vLoArmQuat * Vector3.up;
        vLAAxisRight = vLoArmQuat * Vector3.right;
        vLAAxisForward = vLoArmQuat * Vector3.forward;

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
            if (!vFilteredDictionary.ContainsKey(BodyStructureMap.SensorPositions.SP_UpperSpine))
            {
                return;
            }
            MapTrunkSegment(vFilteredDictionary);
        }
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_RightArm)
        {
            if (!vFilteredDictionary.ContainsKey(BodyStructureMap.SensorPositions.SP_RightForeArm) ||

               !vFilteredDictionary.ContainsKey(BodyStructureMap.SensorPositions.SP_RightUpperArm))
            {
                return;
            }
            //Debug.Log("RIGHT ARM");
            MapRightArmSubsegment(vFilteredDictionary);
        }
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_LeftArm)
        {
            if (!vFilteredDictionary.ContainsKey(BodyStructureMap.SensorPositions.SP_LeftForeArm) ||
                !vFilteredDictionary.ContainsKey(BodyStructureMap.SensorPositions.SP_LeftUpperArm))
            {
                return;
            }
            //Debug.Log("LEFT ARM");
            MapLeftArmSubsegment(vFilteredDictionary);
        }
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_RightLeg)
        {
            if (!vFilteredDictionary.ContainsKey(BodyStructureMap.SensorPositions.SP_RightCalf) ||
                !vFilteredDictionary.ContainsKey(BodyStructureMap.SensorPositions.SP_RightThigh))
            {
                return;
            }
            //Debug.Log("RIGHT LEG");
            MapRightLegSegment(vFilteredDictionary);
        }
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_LeftLeg)
        {
            if (!vFilteredDictionary.ContainsKey(BodyStructureMap.SensorPositions.SP_LeftCalf) ||
                !vFilteredDictionary.ContainsKey(BodyStructureMap.SensorPositions.SP_LeftThigh))
            {
                return;
            }
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
            Transform vSubSegmentTransform = vRendered.GetSubSegmentTransform((BodyStructureMap.SubSegmentTypes)vsubSegment.Key);
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