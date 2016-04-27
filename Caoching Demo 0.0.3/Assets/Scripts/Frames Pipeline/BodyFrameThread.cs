/**
* @file BodyFrameThread.cs
* @brief Contains the bodyframethread class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Frames_Pipeline;
using Assets.Scripts.Frames_Pipeline.BodyFrameConversion;
using Assets.Scripts.Utils.DebugContext.logging;
using HeddokoLib.adt;
using HeddokoLib.networking;
using HeddokoLib.utils;
using UnityEngine;
using LogType = Assets.Scripts.Utils.DebugContext.logging.LogType;

/**
* BodyFrameThread class 
* @brief child class for communication threads
todo: can create an interface for handling these, subsequently every routine that needs to use the bodyframe buffer can just use an interface call. 
*/
public class BodyFrameThread : ThreadedJob
{
    private BodyFrameBuffer mBuffer;
    private SourceDataType mDataSourceType;
    private PlaybackState mCurrentPlaybackState = PlaybackState.Pause;
    private PlaybackSettings mPlaybackSettings;
    private List<BodyRawFrame> mRawFrames;
    private RecordingPlaybackTask mPlaybackTask;
    private bool mContinueWorking;
    private const int MinSuitBufferSize = 10;
    private static int mInboudSuitBufferCap = 1500;



    private CircularQueue<HeddokoPacket> mInboundSuitBuffer;//= new CircularQueue<HeddokoPacket>(100, true);
    
    private object mWorkerThreadLockHandle = new object();
    private Vector3[] vPreviouslyValidValues = new Vector3[9];

    private bool mPausedWorker;

    /// <summary>
    /// Sets and gets the inbound suit buffer's capacity
    /// Note: if the value falls below a minimally acceptable level, it will be forcefully set to an acceptable level
    /// </summary>
    public static int InboundSuitBufferCap
    {
        get { return mInboudSuitBufferCap; }
        set
        {
            mInboudSuitBufferCap = value;
            if (mInboudSuitBufferCap < MinSuitBufferSize)
            {
                mInboudSuitBufferCap = MinSuitBufferSize;
            }
         }
    }
    /// <summary>
    /// Paused worker flag
    /// </summary>
    private bool PausedWorker
    {
        get { return mPausedWorker;}
        set
        {
            mPausedWorker = value;
            if (mPlaybackTask != null)
            {
                mPlaybackTask.IsPaused = mPausedWorker;
            }
        }
    }
    /// <summary>
    /// The current playback task
    /// </summary>
    public RecordingPlaybackTask PlaybackTask
    {
        get { return mPlaybackTask;}
    }

    public bool IsDebugging { get; set; }

    public bool ContinueWorking
    {
        get
        {
            bool tmp;
            lock (mWorkerThreadLockHandle)
            {
                tmp = mContinueWorking;
            }
            return tmp;
        }
        set
        {
            lock (mWorkerThreadLockHandle)
            {
                mContinueWorking = value;
                if (mPlaybackTask != null)
                {
                    mPlaybackTask.IsWorking = mContinueWorking;
                }
            }
        }
    }

    public BodyFrameBuffer BodyFrameBuffer
    {
        get
        {
            if (mBuffer == null)
            {
                mBuffer = new BodyFrameBuffer();
            }
            return mBuffer;
        }
    }

    internal CircularQueue<HeddokoPacket> InboundSuitBuffer
    {
        get { return mInboundSuitBuffer; }
    }

    /** 
    * @brief Parameterized constructor that takes in a list of rawframes, transforming thus rawdata into bodyframe data when the thread is started 
    * @param recording 
    */
    public BodyFrameThread(List<BodyRawFrame> mRawFrames, BodyFrameBuffer vBuffer)
    {
        this.mRawFrames = mRawFrames;
        this.mBuffer = vBuffer;
        mDataSourceType = SourceDataType.Recording;
    }
    public BodyFrameThread(BodyFramesRecording vFrameRecording, BodyFrameBuffer vBuffer)
    {
        this.mRawFrames = mRawFrames;
        this.mBuffer = vBuffer;
        mDataSourceType = SourceDataType.Recording;
        mPlaybackTask = new RecordingPlaybackTask(vFrameRecording, BodyFrameBuffer);
    }

    /**
    * @brief Default constructor
    */
    public BodyFrameThread()
    {

    }

    /// <summary>
    /// Preps the buffer to accept raw data from brainpacks
    /// </summary>
    /// <param name="vBuffer"></param>
    public BodyFrameThread(BodyFrameBuffer vBuffer, SourceDataType vDataType)
    {
        this.mBuffer = vBuffer;
        if (vDataType == SourceDataType.BrainFrame)
        {
            mBuffer.AllowOverflow = true;
        }
        if (vDataType == SourceDataType.Recording)
        {
            mBuffer.AllowOverflow = false;
        }

        mDataSourceType = SourceDataType.BrainFrame;
        mInboundSuitBuffer = new CircularQueue<HeddokoPacket>(InboundSuitBufferCap,true);
    }

    public override void Start()
    {
        
        base.Start();
        ContinueWorking = true;

    }

    /// <summary>
    /// Flips the pause state
    /// </summary>
    public void FlipPauseState()
    {
        PausedWorker = !mPausedWorker;
        mPlaybackTask.IsPaused = PausedWorker;
    }

 


    /**
    * ThreadFunction()
    * @brief The thread loop, overwrite this in the base class
    */
    protected override void ThreadFunction()
    {
        switch (mDataSourceType)
        {
            case SourceDataType.BrainFrame:
                BodyFrameBuffer.AllowOverflow = true;
                BrainFrameTask();
                break;
            case SourceDataType.Recording:
                BodyFrameBuffer.AllowOverflow = false;
               //  RecordingTask();
              
                RecordingPlaybackTask();
                break;
            case SourceDataType.Suit:
                //todo
                break;
        }

    }

    /**
    * TaskForRecording()
    * @brief Helping function that ensures that pushes data onto a circular buffer. If the buffer is filled,then the tasks waits until cancelled. this task is for the case that the data 
    * comes from a recording
    */
    private void RecordingTask()
    {
        int vBodyFrameIndex = 0;


        while (ContinueWorking)
        {
            while (true)
            {
                if (!ContinueWorking)
                {
                    break;
                }
                if (BodyFrameBuffer.IsFull() || mPausedWorker)
                {
                    continue;
                }

                try
                {
                    //convert to body frame  : Todo: this can be optimized, we can reduce these calls, but the proposal would induce an additional memory cost
                    BodyFrame vBodyFrame = RawFrameConverter.ConvertRawFrame(mRawFrames[vBodyFrameIndex]);
                    BodyFrameBuffer.Enqueue(vBodyFrame);
                    vBodyFrameIndex++;

                    //reset back to 0
                    if (vBodyFrameIndex >= mRawFrames.Count)
                    {
                        vBodyFrameIndex = 0;
                    }
                }
                catch (Exception e)
                {
                    //ContinueWorking = false;
                    string vMessage = e.GetBaseException().Message;
                    vMessage += "\n" + e.Message;
                    vMessage += "\n" + e.StackTrace;
                    break;
                }

            }
        }
    }

    private void RecordingPlaybackTask()
    {
        mPlaybackTask.Play();
    }

    /**
    * BrainFrameTask()
    * @brief Helping function that ensures that pushes data onto a circular buffer. If the buffer is filled,then the oldest frame gets overwritten. this task is for the case that the data 
    * comes from a the brainframe
    */
    private void BrainFrameTask()
    {
        int vStartTime = 0;
        int vCounter = 0;
        while (true)
        {
            string vLogMessage = "";
            if (!ContinueWorking)
            {
                //finished working
                break;
            }

            if (InboundSuitBuffer.Count == 0)
            {
                continue;
            }

            HeddokoPacket vPacket = InboundSuitBuffer.Dequeue();
            if (vPacket == null)
            {
                continue;
            }

            string vUnwrappedString = "";

            try
            {
                bool vAllClear = false;
                //first unwrap the string and break it down 
                vUnwrappedString = HeddokoPacket.Unwrap(vPacket.Payload);

                //Debug here

                //todo place a check here for valid data
                string[] vExploded = vUnwrappedString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //WriteToDiskSubTask(vUnwrappedString);
                //this data is invalid
                if (vExploded.Length != 12)
                {
                    continue;
                }
                //the first value is a timestamp in int
                int vTimeStamp = Convert.ToInt32(vExploded[0]);
                if (++vCounter == 1)
                {
                    vStartTime = vTimeStamp;
                }
                DebugLogger.Instance.LogMessage(LogType.BodyFrameThreadStart, "Starting frame conversion with timestamp "+ vStartTime);

                //get the bitmask from index 1
                Int16 vBitmask = Convert.ToInt16(vExploded[1], 16);

                int vStartIndex = 2;
                int vEndIndex = 11;
                int vBitmaskCheck = 0;

                //is used to set vPreviouslyValid values indicies 
                int vSetterIndex = 0;

                for (int i = vStartIndex; i < vEndIndex; i++, vBitmaskCheck++, vSetterIndex++)
                {
                    //get the bitmask and check if the sensors values are valid(not disconnected)
                    //data is valid 
                    if ((vBitmask & (1 << vBitmaskCheck)) == (1 << vBitmaskCheck))
                    {
                        //conversion happens here, todo: place a check here for invalid data(less than 4 bytes in length
                        string[] v3data = vExploded[i].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        float vRoll = ConversionTools.ConvertHexStringToFloat((v3data[0]));
                        float vPitch = ConversionTools.ConvertHexStringToFloat((v3data[1]));
                        float vYaw = ConversionTools.ConvertHexStringToFloat((v3data[2]));
                        vPreviouslyValidValues[vSetterIndex] = new Vector3(vPitch, vRoll, vYaw);
                    }
                }
                BodyFrame vBodyFrame = RawFrameConverter.CreateBodyFrame(vPreviouslyValidValues);
                //Todo: convert the timestamp to a float
                vBodyFrame.Timestamp = (float)(vTimeStamp - vStartTime) / 1000f; 
                BodyFrameBuffer.Enqueue(vBodyFrame);
                DebugLogger.Instance.LogMessage(LogType.BodyFrameThreadEnd, "Completed frame conversion with timestamp " + vStartTime);
            }
            catch (IndexOutOfRangeException)
            {
                vLogMessage = "IndexOutOfRangeException in BodyFrameThread. Contents of vUnwrappedString are " +
                                 vUnwrappedString;
            }
            catch (Exception e)
            {
                vLogMessage = e.Message + "\n" + e.GetBaseException() + "\n" + e.StackTrace;

            }


        }
    }

    /// <summary>
    /// Close the file
    /// </summary>
    private void CloseFile()
    {
        try
        {
            //mStreamWriter.Flush();
            //  mStreamWriter.Close();
        }
        catch (Exception)
        {


        }

    }

    /**
    * DataStreamTask()
    * @brief Helping function that ensures that pushes data onto a circular buffer. If the buffer is filled,then the oldest frame gets overwritten. this task is for the case that the data 
    * comes from a the brainframe
    */
    private void DataStreamTask()
    {



    }



    /**
    * CleanUp
    * @brief Helping function cleans ups  
*/
    public void StopThread()
    {

        ContinueWorking = false;
        if (mPlaybackTask != null)
        {
            mPlaybackTask.IsWorking = false;
        }
        CloseFile();
    }

    /**
    * OnFinished()
    * @brief Callback when the thread is done executing
*/
    protected override void OnFinished()
    {
        //This is executed by the Unity main thread when the job is finished 
        //TODO: 
    }

    /// <summary>
    /// Where does the data originate from?
    /// </summary>
    public enum SourceDataType
    {
        Suit,
        Recording,
        BrainFrame,
        DataStream,
        Other
    }

    /// <summary>
    /// Represent the recording playback state
    /// </summary>
    public enum PlaybackState
    {
        Pause,
        Stop,
        Rewind,
        Forward //=> with this we have multiple playback speeds. 
    }

    /// <summary>
    /// Initializes the inbound suit buffer
    /// </summary>
    public void InitializeInboundSuitBuffer()
    {
        if (InboundSuitBuffer == null)
        {
            mInboundSuitBuffer  = new CircularQueue<HeddokoPacket>(InboundSuitBufferCap,true);
        }
        InboundSuitBuffer.Clear();
    }
}
