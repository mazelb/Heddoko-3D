/** 
* @file RecordingPlaybackTask .cs
* @brief Contains the PlaybackControl abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
 
using System.Threading;
using Assets.Scripts.Frames_Pipeline.BodyFrameConversion;
using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.Localization;
 
using Assets.Scripts.UI.Loading;
using Assets.Scripts.UI.ModalWindow;
using Assets.Scripts.Utils;
using HeddokoLib.body_pipeline;

namespace Assets.Scripts.Frames_Pipeline
{
    public delegate void BodyFrameConversionCompleted(List<BodyFrame> vBodyFrames);
    public delegate void FinalFramePositionReached();
     /// <summary>
    /// A vRecording playback task used by body frame thread
    /// </summary>
    
    public class RecordingPlaybackTask
    {
        public bool IsWorking;
        public static int StartConversionIndex = 6;
        public bool IsPaused { get; set; }
        public bool LoopPlaybackEnabled = true;
        private bool mIsRewinding;
        private BodyFramesRecordingBase mCurrentRecording;
        private BodyFrameBuffer mFrameBuffer; 
        private int mCurrentIdx;
        private int mFirstPos = 0;
        internal event FinalFramePositionReached FinalFramePositionReachedEvent;
        internal event BodyFrameConversionCompleted BodyFrameConversionCompletedEvent;
        /// <summary>
        /// Depending if the current state is rewinding or going forward, then the 
        /// the index iterator changes the local position of the converted frame data
        /// </summary>
        private int mIteratorAdder = 1;
        private List<BodyFrame> vFrames;
        private BodyFrameBuffer bodyFrameBuffer;
        private int mFinalFramePos { get; set; }

        private float mTotalRecordingTime;
        private List<BodyFrame> mConvertedBodyFrames;
        //    private BodyFrame[] mConvertedFrames;

        private float mPlaybackSpeed = 1f;

        // private List<BodyRawFrameBase> mRawFrames;

        /// <summary>
        /// Body frame conversion completed?
        /// </summary>
        public bool ConversionCompleted { get; private set; }

      

        public float PlaybackSpeed
        {
            get { return mPlaybackSpeed; }
            set
            {
                IteratorAdder = Math.Sign(value) * 1;
                IsRewinding = IteratorAdder < 1;
                //check if playback speed is 0, then set pause to true
                if (mPlaybackSpeed == 0)
                {
                    IsPaused = true;
                    return;
                }

                mPlaybackSpeed = value;

            }
        }
        /// <summary>
        /// Sets the current playback state to rewinding. 
        /// </summary>
        public bool IsRewinding
        {
            get { return mIsRewinding; }
            set
            {
                mIsRewinding = value;
                if (mIsRewinding)
                {
                    IteratorAdder = -1;
                    mFirstPos = ConvertedFrames.Count - 1;
                    mFinalFramePos = 0;
                }
                else
                {
                    mFirstPos = 0;
                    mFinalFramePos = ConvertedFrames.Count - 1;
                    IteratorAdder = 1;
                }
            }
        }

        /// <summary>
        /// Returns the frame count from the vRecording
        /// </summary>
        public int RecordingCount
        {
            get
            {
                if (ConvertedFrames == null)
                {
                    return -1;
                }
                return ConvertedFrames.Count;
            }
        }

        /// <summary>
        /// returns the current index of converted body frame recordings
        /// </summary>
        public int GetCurrentPlaybackIndex
        {
            get { return mCurrentIdx; }
        }

        /// <summary>
        /// Returns the total vRecording time
        /// </summary>
        public float TotalRecordingTime
        {
            get
            {
                if (ConvertedFrames == null)
                {
                    return -1f;
                }
                if (ConvertedFrames.Count == 0)
                {
                    return 0f;
                }

                return mTotalRecordingTime;
            }
            private set { mTotalRecordingTime = value; }
        }
        /// <summary>
        /// Return current body frame
        /// </summary>
        public BodyFrame CurrentBodyFrame
        {
            get
            {
                if (ConvertedFrames == null)
                {
                    return null;
                }
                if (ConvertedFrames.Count == 0)
                {
                    return null;
                }
                return ConvertedFrames[mCurrentIdx];
            }
        }

        /// <summary>
        /// Depending if the current state is rewinding or going forward, then the 
        /// the index iterator changes the local position of the converted frame data
        /// </summary>
        public int IteratorAdder
        {
            get { return mIteratorAdder; }
            private set { mIteratorAdder = value; }
        }

        public BodyFramesRecordingBase CurrentRecording
        {
            get { return mCurrentRecording; }
        }

        //   public BodyFrame[] ConvertedFrames
        //     {
        //        get { return mConvertedFrames; }
        //    }

        public List<BodyFrame> ConvertedFrames
        {
            get { return mConvertedBodyFrames; }
        }

        public int RawFramesCount
        {
            get
            {
                return mCurrentRecording.RecordingRawFramesCount;
            }
        }

        public RecordingPlaybackTask(BodyFramesRecordingBase vRecording, BodyFrameBuffer vBuffer)
        {
            mCurrentRecording = vRecording;
            mFrameBuffer = vBuffer;
        }

        public RecordingPlaybackTask(List<ImuFrame> vFrames, BodyFrameBuffer bodyFrameBuffer)
        {
            //   ConvertedFrames = vFrames;
            this.bodyFrameBuffer = bodyFrameBuffer;
        }

        /// <summary>
        /// The vRecording play back task allows for vRecording playback by converting 
        /// Rawbody frames in to body frames. 
        /// </summary>
        public void Play()
        {
            ConvertFrames();
            int vTotalCount = ConvertedFrames.Count;
            if (vTotalCount == 0)
            {
                return;
            }
            //calculate a delta time between frames, capture the first time stamp
            BodyFrame vFirstFrame = ConvertedFrames[0];
            BodyFrame vLastFrame = ConvertedFrames[vTotalCount - 1];
            float vPrevTimeStamp = vFirstFrame.Timestamp;
            float vRecDeltatime = 0;
            float vStartTime = 0;

            //the position of the first and last frame
            mCurrentIdx = 0;
            mFirstPos = 0;
            mFinalFramePos = vTotalCount - 1;
            //start looping
            while (IsWorking)
            {
                if (!IsWorking)
                {
                    break;
                }
                bool vFrameBufferFull = mFrameBuffer.IsFull();
                if (vFrameBufferFull || IsPaused)
                {
                    continue;
                }
                try
                {
                    //check if we've reached the last position
                    if (mCurrentIdx == mFinalFramePos)
                    {
                        //Send out an event that the last frame has been reached. 
                        if (FinalFramePositionReachedEvent != null)
                        {
                            FinalFramePositionReachedEvent();
                        }

                        //check if looping is enabled, set vCurrPos to first postion
                        if (LoopPlaybackEnabled)
                        {
                            mCurrentIdx = mFirstPos;
                            vPrevTimeStamp = IsRewinding ? vLastFrame.Timestamp : vFirstFrame.Timestamp;
                        }
                        else
                        {
                            continue;
                        }

                    }
                    BodyFrame vCurrBodyFrame = null;
                    BodyFrame vEnquedBodyFrame = null;
                    //if the current position is last -1
                    if (mCurrentIdx == mFinalFramePos)
                    {
                        vEnquedBodyFrame = ConvertedFrames[mCurrentIdx];
                        mFrameBuffer.Enqueue(vEnquedBodyFrame);
                    }

                    vCurrBodyFrame = ConvertedFrames[mCurrentIdx];
                    int vPreviousIndex = IsRewinding ? mCurrentIdx + 1 : mCurrentIdx - 1;
                    if (IsRewinding && vPreviousIndex >= ConvertedFrames.Count)
                    {
                        vPreviousIndex = ConvertedFrames.Count - 1;
                    }
                    if (!IsRewinding && vPreviousIndex < 0)
                    {
                        vPreviousIndex = 0;
                    }
                    vPrevTimeStamp = ConvertedFrames[vPreviousIndex].Timestamp;
                    vRecDeltatime = Math.Abs(vCurrBodyFrame.Timestamp - vPrevTimeStamp);
                    int vSleepTime = (int)((vRecDeltatime / Math.Abs(PlaybackSpeed)) * 1000);
                    Thread.Sleep(vSleepTime);
                    vEnquedBodyFrame = ConvertedFrames[mCurrentIdx];
                    mFrameBuffer.Enqueue(vEnquedBodyFrame);

                    mCurrentIdx += IteratorAdder;
                }
                catch (Exception vException)
                {
                    //todo set up a debug logger
                    string vMessage = vException.GetBaseException().Message;
                    vMessage += "\n" + vException.Message;
                    vMessage += "\n" + vException.StackTrace;
                    break;
                }
            }
        }

        private void ConvertFrames()
        {
            //There are less frames than currently supported, throw an error
            if (mCurrentRecording.RecordingRawFramesCount <= StartConversionIndex)
            {
                OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
                {
                    
                    var vProgressBar = DisablingProgressBar.Instance;
                    if (vProgressBar != null)
                    {
                        vProgressBar.StopAnimation();
                    }
                    ModalPanel.SingleChoice("ERROR",LocalizationBinderContainer.GetString(KeyMessage.RecordingFileLessThanSkippableFrames),
                        () => { });
                });
                return;
            }
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                var vProgressBar = DisablingProgressBar.Instance;
                if (vProgressBar != null)
                {
                    vProgressBar.StartProgressBar("CONVERTING");
                }
            });

            ConversionCompleted = false;
            //first convert all the frames 
            mConvertedBodyFrames = new List<BodyFrame>();
            for (int i = StartConversionIndex, vConvertIndex = 0; i < mCurrentRecording.RecordingRawFramesCount; i++, vConvertIndex++)
            {
                try
                {
                    mConvertedBodyFrames.Add(RawFrameConverter.ConvertRawFrame(mCurrentRecording.GetBodyRawFrameAt(i)));
                }
                catch (Exception vE)
                {
                    //It is very likely that items weren't parsed correctly. don't add them 
                }

            }
            BodyFrame vFirst = mConvertedBodyFrames[0];
            BodyFrame vLast = mConvertedBodyFrames[mConvertedBodyFrames.Count - 1];
            TotalRecordingTime = vLast.Timestamp - vFirst.Timestamp;
            ConversionCompleted = true;
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                var vProgressBar = DisablingProgressBar.Instance;
                if (vProgressBar != null)
                {
                    vProgressBar.StopAnimation();
                }
            });

            if(BodyFrameConversionCompletedEvent != null)
            {
                BodyFrameConversionCompletedEvent(mConvertedBodyFrames);
            }
        }
        /// <summary>
        /// Sets the playback to be played from the passed in index
        /// </summary>
        /// <param name="vConvertedRecIdx"></param>
        public void PlayFromIndex(int vConvertedRecIdx)
        {
            if (vConvertedRecIdx < 0)
            {
                vConvertedRecIdx = 0;
            }
            else if (vConvertedRecIdx >= mConvertedBodyFrames.Count)
            {
                vConvertedRecIdx = mConvertedBodyFrames.Count - 1;
            }

            mFrameBuffer.Clear();
            mCurrentIdx = vConvertedRecIdx;
            mFrameBuffer.Enqueue(mConvertedBodyFrames[mCurrentIdx]);
        }

        /// <summary>
        /// returns a body frame at the current index
        /// </summary>
        /// <param name="vIndex"></param>
        /// <returns></returns>
        public BodyFrame GetBodyFrameAtIndex(int vIndex)
        {
            if (mConvertedBodyFrames.Count == 0)
            {
                return null;
            }
            if (vIndex <= 0)
            {
                return mConvertedBodyFrames[0];
            }
            if (vIndex >= mConvertedBodyFrames.Count)
            {
                return mConvertedBodyFrames[mConvertedBodyFrames.Count - 1];
            }
            else
            {
                return mConvertedBodyFrames[vIndex];
            }
        }

        /// <summary>
        /// Attach an event handler to the final frame position reached event
        /// </summary>
        /// <param name="vFinalFrameHandler">the event handler to attach</param>
        public void AttachFinalFrameEventHandler(FinalFramePositionReached vFinalFrameHandler)
        {
            FinalFramePositionReachedEvent += vFinalFrameHandler;
        }
        /// <summary>
        /// Remove an event handler to the final frame position reached event
        /// </summary>
        /// <param name="vFinalFrameHandler">the event handler to remove</param>
        public void RemoveFinalFrameEventHandler(FinalFramePositionReached vFinalFrameHandler)
        {
            FinalFramePositionReachedEvent -= vFinalFrameHandler;
        }
    }
}