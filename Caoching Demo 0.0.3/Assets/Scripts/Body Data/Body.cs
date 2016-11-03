/** 
* @file Body.cs
* @brief Contains the Body class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts.Body_Data.view;
using Assets.Scripts.Utils;
using System.Linq;
using Assets.Scripts.Body_Data.CalibrationData;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.Body_Pipeline.Analysis;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Trunk;
using Assets.Scripts.Communication;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.ErrorHandling;
using Assets.Scripts.Frames_Pipeline.BodyFrameConversion;
using Assets.Scripts.Frames_Recorder.FramesRecording;

/**
* Body class 
* @brief Body class (represents one body suit)
*/
[Serializable]
public class Body
{
    [SerializeField]
    //Body Unique GUID for ease of cloud access
    public string BodyGuid;

    [SerializeField]
    //Currently connected suit GUID 
    public string SuitGuid;

    //Body Composition
    [SerializeField]
    public Dictionary<BodyStructureMap.SegmentTypes, BodySegment> BodySegments = new Dictionary<BodyStructureMap.SegmentTypes, BodySegment>();

    [SerializeField]
    public BodyStructureMap.BodyTypes BodyType = BodyStructureMap.BodyTypes.BodyType_FullBody;

    [SerializeField]
    public BodyFrame CurrentBodyFrame;
    [SerializeField]
    public BodyFrame PreviousBodyFrame;
    //Initial body Frame
    [SerializeField]
    public BodyFrame InitialBodyFrame { get; set; }

    private BodyFrameThread mBodyFrameThread = new BodyFrameThread();
    public Dictionary<BodyStructureMap.SegmentTypes, SegmentAnalysis> AnalysisSegments = new Dictionary<BodyStructureMap.SegmentTypes, SegmentAnalysis>(5);
    public LeftArmAnalysis LeftArmAnalysis;
    public RightArmAnalysis RightArmAnalysis;
    public LeftLegAnalysis LeftLegAnalysis;
    public RightLegAnalysis RightLegAnalysis;
    public TrunkAnalysis TorsoAnalysis;

    //view associated with this model
    private BodyView mView;
    [SerializeField]
    private RenderedBody mRenderedBody;

    internal BodyFrameCalibrationContainer mBodyFrameCalibrationContainer = new BodyFrameCalibrationContainer();
    /**
    * View
    * @param 
    * @brief View associated with this body
    * @note: a new gameobject is created and this Body is added into it as a component
    * @return returns the view associated with this body
    */
    public BodyView View
    {
        get
        {
            if (mView == null)
            {
                GameObject viewGO = new GameObject("body view " + BodyGuid);
                mView = viewGO.AddComponent<BodyView>();
                mView.AssociatedBody = this;
            }
            return mView;
        }

    }

    /**
    * MBodyFrameThread
    * @param 
    * @brief BodyFrameThread needed to start updated body 
    * @return returns the view associated with this body
    */
    public BodyFrameThread MBodyFrameThread
    {
        get
        {
            if (mBodyFrameThread == null)
            {
                mBodyFrameThread = new BodyFrameThread();
            }
            return mBodyFrameThread;
        }
    }

    public RenderedBody RenderedBody
    {
        get { return mRenderedBody; }
        private set
        {
            mRenderedBody = value;
        }
    }

    /**
    * CreateNewBodyUUID()
    * @brief Creates a new body UUID
    */
    public void CreateNewBodyUUID()
    {
        BodyGuid = Guid.NewGuid().ToString();
    }

    /**
    * InitBody()
    * @param vBodyUUID the new body UUID (could be empty)
    * @brief Initializes a new body 
    */
    public void InitBody(string vBodyUUID, bool vCallFromUnityThread)
    {
        if (vCallFromUnityThread)
        {
            InitBody(vBodyUUID, BodyType);
        }
        else
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => InitBody(vBodyUUID, BodyType));
        }
    }

    /**
    * InitBody(string vBodyUUID , BodyStructureMap.BodyTypes vBodyType)
    * @param vBodyUUID the new body UUID (could be empty), BodyType is the desired BodyType
    * @brief Initializes a new body with a certain body type
    */
    public void InitBody(string vBodyUUID, BodyStructureMap.BodyTypes vBodyType)
    {
        BodyType = vBodyType;

        //Init the body UUID (given or created)
        if (string.IsNullOrEmpty(vBodyUUID))
        {
            CreateNewBodyUUID();
        }
        else
        {
            BodyGuid = vBodyUUID;
        }

        //Init all structures
        CreateBodyStructure(vBodyType);
    }

    /**
    * CreateBodyStructure(BodyStructureMap.BodyTypes vBodyType )
    * @param  vBodyType: the desired BodyType, this also initializes the body's analysis segment
    * @brief Initializes a new body structure's internal properties with the desired body type
    */
    public void CreateBodyStructure(BodyStructureMap.BodyTypes vBodyType)
    {
        //Get the list of segments from the bodystructuremap 
        List<BodyStructureMap.SegmentTypes> vSegmentList = BodyStructureMap.Instance.BodyToSegmentMap[vBodyType];
        TrunkAnalysis vTorsoSegmentAnalysis = new TrunkAnalysis();
        vTorsoSegmentAnalysis.SegmentType = BodyStructureMap.SegmentTypes.SegmentType_Torso;


        foreach (BodyStructureMap.SegmentTypes type in vSegmentList)
        {
            BodySegment vSegment = new BodySegment();
            vSegment.SegmentType = type;
            vSegment.InitializeBodySegment(type);
            vSegment.ParentBody = this;
            //set the reference to the BodyFrameCalibrationContainer
            vSegment.BodyFrameCalibrationContainer = mBodyFrameCalibrationContainer;
            BodySegments.Add(type, vSegment);

            vSegment.AssociatedView.transform.parent = View.transform;

            //Todo: this can can be abstracted and mapped nicely. 
            if (type == BodyStructureMap.SegmentTypes.SegmentType_Torso)
            {
                vSegment.mCurrentAnalysisSegment = vTorsoSegmentAnalysis;
                AnalysisSegments.Add(BodyStructureMap.SegmentTypes.SegmentType_Torso, vTorsoSegmentAnalysis);
                TorsoAnalysis = vTorsoSegmentAnalysis;
            }
            if (type == BodyStructureMap.SegmentTypes.SegmentType_LeftArm)
            {
                LeftArmAnalysis vLeftArmSegmentAnalysis = new LeftArmAnalysis();
                vLeftArmSegmentAnalysis.SegmentType = BodyStructureMap.SegmentTypes.SegmentType_LeftArm;
                vLeftArmSegmentAnalysis.TorsoAnalysisSegment = vTorsoSegmentAnalysis;
                vSegment.mCurrentAnalysisSegment = vLeftArmSegmentAnalysis;
                AnalysisSegments.Add(BodyStructureMap.SegmentTypes.SegmentType_LeftArm, vLeftArmSegmentAnalysis);
                LeftArmAnalysis = vLeftArmSegmentAnalysis;
            }
            if (type == BodyStructureMap.SegmentTypes.SegmentType_RightArm)
            {
                RightArmAnalysis vRightArmSegmentAnalysis = new RightArmAnalysis();
                vRightArmSegmentAnalysis.SegmentType = BodyStructureMap.SegmentTypes.SegmentType_RightArm;
                vRightArmSegmentAnalysis.TorsoAnalysisSegment = vTorsoSegmentAnalysis;
                vSegment.mCurrentAnalysisSegment = vRightArmSegmentAnalysis;
                AnalysisSegments.Add(BodyStructureMap.SegmentTypes.SegmentType_RightArm, vRightArmSegmentAnalysis);
                RightArmAnalysis = vRightArmSegmentAnalysis;
            }
            if (type == BodyStructureMap.SegmentTypes.SegmentType_LeftLeg)
            {
                LeftLegAnalysis vLeftLegAnalysisSegment = new LeftLegAnalysis();
                vLeftLegAnalysisSegment.SegmentType = BodyStructureMap.SegmentTypes.SegmentType_LeftLeg;
                vLeftLegAnalysisSegment.TorsoAnalysisSegment = vTorsoSegmentAnalysis;
                vSegment.mCurrentAnalysisSegment = vLeftLegAnalysisSegment;
                AnalysisSegments.Add(BodyStructureMap.SegmentTypes.SegmentType_LeftLeg, vLeftLegAnalysisSegment);
                LeftLegAnalysis = vLeftLegAnalysisSegment;
            }
            if (type == BodyStructureMap.SegmentTypes.SegmentType_RightLeg)
            {
                RightLegAnalysis vRightLegAnalysisSegment = new RightLegAnalysis();
                vRightLegAnalysisSegment.SegmentType = BodyStructureMap.SegmentTypes.SegmentType_RightLeg;
                vRightLegAnalysisSegment.TorsoAnalysisSegment = vTorsoSegmentAnalysis;
                vSegment.mCurrentAnalysisSegment = vRightLegAnalysisSegment;
                AnalysisSegments.Add(BodyStructureMap.SegmentTypes.SegmentType_RightLeg, vRightLegAnalysisSegment);
                RightLegAnalysis = vRightLegAnalysisSegment;
            }

        }
    }

    /**
    * UpdateBody(BodyFrame vFrame )
    * @param vFrame, the body frame, setCurrentBodyFrame: sets the passed in body frame as the current bodyframe
    * @brief  Set the current body frame from the passed in parameter 
    */
    public void UpdateBody(BodyFrame vFrame)
    {
        PreviousBodyFrame = CurrentBodyFrame;
        CurrentBodyFrame = vFrame;

        foreach (var vKv in BodySegments)
        {
            vKv.Value.UpdateSensorsData(vFrame);
        }
        //for (int i = 0; i < BodySegments.Count; i++)
        //{
        //    BodySegments[i].UpdateSensorsData(vFrame);
        //}
    }

    /**
    * SetInitialFrame(BodyFrame vInitialFrame)
    * @param BodyFrame vInitialFrame, sets the initial frame, subsequently the initial orientations point of the body's subsegment
    * @brief  Set the current body frame from the passed in parameter
    */
    public void SetInitialFrame(BodyFrame vInitialFrame)
    {
        InitialBodyFrame = vInitialFrame;
        //Reset the body frame calibration container's initial time
        if (vInitialFrame != null) ;
        mBodyFrameCalibrationContainer.Reset(vInitialFrame);
        UpdateInitialFrameData();
    }

    /// <summary>
    /// Updates the initial frame sensors data in all segments
    /// </summary>
    public void UpdateInitialFrameData()
    {
        foreach (var vKv in BodySegments)
        {
            vKv.Value.UpdateInitialSensorsData(InitialBodyFrame);
        }
        //for (int i = 0; i < BodySegments.Count; i++)
        //{
        //    BodySegments[i].UpdateInitialSensorsData(InitialBodyFrame);
        //}
    }

    /// <summary>
    /// Resets body metrics
    /// </summary>
    public void ResetBodyMetrics()
    {
        foreach (var vKv in BodySegments)
        {
            vKv.Value.ResetMetrics();
        }
        //for (int i = 0; i < BodySegments.Count; i++)
        //{
        //    BodySegments[i].ResetMetrics();
        //}
    }

    /**
    * ProcessRecording(string vRecUuid)
    * @param vRecUuid, the recording UUID
    * @brief  Play a recording from the given recording UUID. 
    */
    public void PlayRecording(string vRecUuid)
    {
        //Stops the current thread from running.
        StopThread();

        //get the raw frames from recording 
        //first try to get the recording from the recording manager. 
        BodyRecordingsMgr.Instance.TryGetRecordingByUuid(vRecUuid, PlayRecordingCallback);
    }

    /// <summary>
    /// Callback action after a body frames recording has been retrieved
    /// </summary>
    /// <param name="vBodyFrameRecording"></param>
    private void PlayRecordingCallback(BodyFramesRecordingBase vBodyFrameRecording)
    {
        if (vBodyFrameRecording != null && vBodyFrameRecording.RecordingRawFramesCount > 0)
        {
            BodyFrame vBodyFrame = null;

            while (vBodyFrameRecording.RecordingRawFramesCount > 0)
            {
                try
                {
                    vBodyFrame = RawFrameConverter.ConvertRawFrame(vBodyFrameRecording.GetBodyRawFrameAt(0));
                    break;
                }
                catch (IndexOutOfRangeException vE)
                {
                    vBodyFrameRecording.RemoveAt(0);
                }
                catch (FormatException vE)
                {
                    vBodyFrameRecording.RemoveAt(0);
                }

            }
            if (vBodyFrameRecording.RecordingRawFramesCount == 0)
            {
                RecordingErrorHandlerManager.Instance.Notify("IssueLoading", vBodyFrameRecording);
            }

            //Setting the first frame as the initial frame


            SetInitialFrame(vBodyFrame);
            BodyFrameBuffer vBuffer1 = new BodyFrameBuffer();

            // mBodyFrameThread = new BodyFrameThread(bodyFramesRec.RecordingRawFrames, vBuffer1);
            mBodyFrameThread = new BodyFrameThread(vBodyFrameRecording, vBuffer1);
            View.Init(this, vBuffer1);
            View.StartUpdating = true;
            mBodyFrameThread.Start();

        }
    }

    /// <summary>
    /// Start stream from brainpack. Set the body to be ready to play from brainpack
    /// </summary>
    public void StreamFromBrainpack()
    {

        // ===================== How this function works ==================================================================//
        //when trying to connect to the brain pack, first we need to ensure that 
        //1. the brainpack can be connected to 
        //1a.   once we can establish a connection, we need to find a way to get the latest data. This is ensured by the server
        //      which clears out the buffer on any new connections.
        //1b.   once condition 1a is met, plug the body frame thread into the brainpack connector. The connector 
        //      then feeds data into the BodyFramethread.
        //1c.   in case of failure, a message must be brought into the view. and immediately pause the connection.  
        // ===================== End of "How this functions works" ========================================================//
        //stop the current thread and get ready for a new connection. 
        StopThread();
        if (mBodyFrameThread == null)
        {
            BodyFrameBuffer vBuffer1 = new BodyFrameBuffer(24);
            mBodyFrameThread = new BodyFrameThread(vBuffer1, BodyFrameThread.SourceDataType.BrainFrame);
        }
        else
        {
            mBodyFrameThread.InitializeInboundSuitBuffer();
        }
        mBodyFrameThread.BodyFrameBuffer.Clear();
        //1 inform the brainpack connection controller to establish a new connection
        //1i: Listen to the event that the brainpack has been disconnected
        BrainpackConnectionController.Instance.DisconnectedStateEvent += BrainPackStreamDisconnectedListener;
        bool vRegisteredEvent = false;
        //1ii: check if the controller already is already connected. 
        if (BrainpackConnectionController.Instance.ConnectionState == BrainpackConnectionState.Connected)
        {
            BrainPackStreamReadyListener();
            vRegisteredEvent = true;
        }
        if (!vRegisteredEvent)
        {
            BrainpackConnectionController.Instance.ConnectedStateEvent -= BrainPackStreamReadyListener;
            BrainpackConnectionController.Instance.ConnectedStateEvent += BrainPackStreamReadyListener;
        }

        mBodyFrameThread.Start();
        View.Init(this, mBodyFrameThread.BodyFrameBuffer);
        View.StartUpdating = true;
    }

    public void PlayFromDataStream(ProtobuffFrameRouter vRouter)
    {
        mBodyFrameThread.StopThread();
        mBodyFrameThread.Init(vRouter);
        mBodyFrameThread.Start();
        View.Init(this, vRouter.OutBoundBuffer);
        View.StartUpdating = true;
    }
    /// <summary>
    /// Listener whos responsibility is to plug the bodyframe thread into controller.
    /// </summary>
    private void BrainPackStreamReadyListener()
    {
        BrainpackConnectionController.Instance.ReadyToLinkBodyToBP(mBodyFrameThread);
    }

    /// <summary>
    /// Listens to when the brainpack controller has been disconnected from the brainpack
    /// </summary>
    private void BrainPackStreamDisconnectedListener()
    {
        StopThread();
        UnhookBrainpackListeners();
    }

    public void UnhookBrainpackListeners()
    {
        try
        {
            BrainpackConnectionController.Instance.ConnectedStateEvent -= BrainPackStreamReadyListener;
            BrainpackConnectionController.Instance.DisconnectedStateEvent -= BrainPackStreamDisconnectedListener;
        }
        catch
        {

        }

    }

    /// <summary>
    /// Applies tracking on the requested body. 
    /// </summary>
    /// <param name="vBody">Body vBody: The body to apply tracking to. </param> 
    /// <param name="vDic">Dictionary vDic: The tracking matrices to be applied. </param> 
    public static void ApplyTracking(Body vBody, Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vDic)
    {
        //get the list of segments of the speicfied vBody
        var vListBodySegments = vBody.BodySegments;
        foreach (var vBodySegment in vBody.BodySegments)
        {
            List<BodyStructureMap.SensorPositions> vSensPosList =
             BodyStructureMap.Instance.SegmentToSensorPosMap[vBodySegment.Key];
            Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vFilteredDictionary
                = new Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure>(2);
            for (int i = 0; i < vSensPosList.Count; i++)
            {
                if (vDic.ContainsKey(vSensPosList[i]))
                {
                    if (vDic.ContainsKey(vSensPosList[i]))
                    {
                        var vTrackedStruc = vDic[vSensPosList[i]];
                        vFilteredDictionary.Add(vSensPosList[i], vTrackedStruc);
                    }
                }
            }
            //check current segment  and the filtered dictionary. 

            vBodySegment.Value.UpdateSegment(vFilteredDictionary);
        }
        

    }
    /**
    * GetTracking()
    * @brief  Play a recording from the given recording UUID. 
    * @return Returns a dictionary and their respective   transformation matrix
    */
    public static Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> GetTracking(Body vBody)
    {
        Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vDic = new Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure>(9);
        List<BodyStructureMap.SensorPositions> vKeyList = new List<BodyStructureMap.SensorPositions>(vBody.CurrentBodyFrame.FrameData.Keys);

        for (int i = 0; i < vKeyList.Count; i++)
        {
            BodyStructureMap.SensorPositions vKey = vKeyList[i];

            BodyFrame.Vect4 vInitialRawEuler = vBody.InitialBodyFrame.FrameData[vKey];
            BodyFrame.Vect4 vCurrentRawEuler = vBody.CurrentBodyFrame.FrameData[vKey];
            //Vector3 vPreviousRawEuler = vBody.PreviousBodyFrame.FrameData[vKey];

            BodyStructureMap.TrackingStructure vStruct = new BodyStructureMap.TrackingStructure();
            vStruct.InitRawEuler = vInitialRawEuler;
            vStruct.CurrRawEuler = vCurrentRawEuler;
            //vStruct.PrevRawEuler = vPreviousRawEuler;

            vDic.Add(vKey, vStruct);
        }

        return vDic;
    }

    /**
    * GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes vSegmentType)
    * @param  BodyStructureMap.SegmentTypes vSegmentType the interested segment the caller wants
    * @brief  Based off the passed parameter, return a segment of that type
    * @param  Returns a segment from body that is of type vSegmentType
    */
    public BodySegment GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes vSegmentType)
    {
        BodySegment vSegment = null;
        if (BodySegments.ContainsKey(vSegmentType))
        {
            vSegment = BodySegments[vSegmentType];
        }
        //  BodySegment vSegment = BodySegments.First(x => x.SegmentType == vSegmentType);
        return vSegment;
    }

    public void PauseRecording(string vRecUUID)
    {
        //TODO: 
    }

    public void StopRecording(string vRecUUID)
    {
        //TODO: 
    }

    /// <summary>
    /// Stops the current thread and tells the view to stop playback
    /// </summary>
    internal void StopThread()
    {
        if (View != null)
        {
            View.StartUpdating = false;
        }
        if (mBodyFrameThread != null)
        {
            mBodyFrameThread.StopThread();
        }

    }

    /// <summary>
    /// Pause all worker threads that are current working on the body
    /// </summary>
    internal void PauseThread()
    {
        if (mBodyFrameThread != null)
        {
            mBodyFrameThread.FlipPauseState();
        }
    }

    /// <summary>
    /// Sets the passed in RenderedBody component and updates Components with the passed in parameter
    /// </summary>
    /// <param name="vRendered"></param>
    public void UpdateRenderedBody(RenderedBody vRendered)
    {
        RenderedBody = vRendered;
        foreach (var vBodySegment in BodySegments)
        {
            vBodySegment.Value.UpdateRenderedSegment(vRendered);
        }
        RenderedBody.AssociatedBodyView = View;
    }

    /// <summary>
    /// Release 3d resources used by the body.
    /// </summary>
    public void ReleaseResources()
    {
        if (RenderedBody != null)
        {
            RenderedBodyPool.ReleaseResource(RenderedBody);
            RenderedBody = null;
        }
        foreach (var vBodySegment in BodySegments)
        {
            vBodySegment.Value.ReleaseResources();
        }

    }
    /// <summary>
    /// Sets the buffer to stream Body frames    from
    /// </summary>
    /// <param name="vBodyFrames"></param>
    public void SetBuffer(BodyFrameBuffer vBodyFrames)
    {
        StopThread();
        View.Init(this, vBodyFrames);
        View.StartUpdating = true;
    }
}