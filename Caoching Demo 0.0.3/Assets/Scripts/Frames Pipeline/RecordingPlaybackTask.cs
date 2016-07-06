/** 
* @file RecordingPlaybackTask .cs
* @brief Contains the PlaybackControl abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Assets.Scripts.Frames_Pipeline.BodyFrameConversion;
using Assets.Scripts.UI.Loading;
using Assets.Scripts.Utils;

namespace Assets.Scripts.Frames_Pipeline
{
    /// <summary>
    /// A recording playback task used by body frame thread
    /// </summary>
    public class RecordingPlaybackTask
    {
        public bool IsWorking;
        public bool IsPaused { get; set; }
        public bool LoopPlaybackEnabled =true;
        private bool mIsRewinding;
        private BodyFramesRecording mCurrentRecording;
        private BodyFrameBuffer mFrameBuffer;
        private int mCurrentIdx;
        private int mFirstPos = 0;

        private int mFinalFramePos { get; set; }

        private float mTotalRecordingTime;

        private BodyFrame[] mConvertedFrames;

        private float mPlaybackSpeed = 1f;

        private List<BodyRawFrame> mRawFrames;

        /// <summary>
        /// Body frame conversion completed?
        /// </summary>
        public bool ConversionCompleted { get; private set; }

        /// <summary>
        /// Depending if the current state is rewinding or going forward, then the 
        /// the index iterator changes the local position of the converted frame data
        /// </summary>
        private int mIteratorAdder = 1;
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
                    mFirstPos = mConvertedFrames.Length-1;
                    mFinalFramePos = 0;
                }
                else
                {
                    mFirstPos = 0;  
                    mFinalFramePos = mConvertedFrames.Length - 1;
                    IteratorAdder = 1;
                } 
               

            }
        }

        /// <summary>
        /// Returns the frame count from the recording
        /// </summary>
        public int RecordingCount
        {
            get
            {
                if (mConvertedFrames == null)
                {
                    return -1;
                }
                return mConvertedFrames.Length;
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
        /// Returns the total recording time
        /// </summary>
        public float TotalRecordingTime
        {
            get
            {
                if (mConvertedFrames == null)
                {
                    return -1f;
                }
                if (mConvertedFrames.Length == 0)
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
                if (mConvertedFrames == null)
                {
                    return null;
                }
                if (mConvertedFrames.Length == 0)
                {
                    return null;
                }
                return mConvertedFrames[mCurrentIdx];
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

        public BodyFramesRecording CurrentRecording
        {
            get { return mCurrentRecording; }
        }

        public BodyFrame[] ConvertedFrames
        {
            get { return mConvertedFrames; }
        }


        public RecordingPlaybackTask(BodyFramesRecording vRecording, BodyFrameBuffer vBuffer)
        {
            mCurrentRecording = vRecording;
            mRawFrames = vRecording.RecordingRawFrames;
            mFrameBuffer = vBuffer;
        }
        /// <summary>
        /// The recording play back task allows for recording playback by converting 
        /// Rawbody frames in to body frames. 
        /// </summary>
        public void Play()
        {
            ConvertFrames();
            int vTotalCount = mConvertedFrames.Length;
            if (vTotalCount == 0)
            {
                return;
            }
            //calculate a delta time between frames, capture the first time stamp
            BodyFrame vFirstFrame = mConvertedFrames[0];
            BodyFrame vLastFrame = mConvertedFrames[vTotalCount - 1];
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
                        //reset the start time
                        vStartTime = 0;
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
                        vEnquedBodyFrame = mConvertedFrames[mCurrentIdx];
                        mFrameBuffer.Enqueue(vEnquedBodyFrame);
                    }

                    vCurrBodyFrame = mConvertedFrames[mCurrentIdx];
                    int vPreviousIndex = IsRewinding ? mCurrentIdx + 1 : mCurrentIdx - 1;
                    if (IsRewinding && vPreviousIndex >= mConvertedFrames.Length)
                    {
                        vPreviousIndex = mConvertedFrames.Length - 1;
                    }
                    if (!IsRewinding && vPreviousIndex < 0)
                    {
                        vPreviousIndex =0;
                    }
                    vPrevTimeStamp = mConvertedFrames[vPreviousIndex].Timestamp;//vCurrBodyFrame.Timestamp;
                    vRecDeltatime = Math.Abs(vCurrBodyFrame.Timestamp - vPrevTimeStamp);
                    int vSleepTime = (int)((vRecDeltatime / Math.Abs(PlaybackSpeed)) * 1000);
                    Thread.Sleep(vSleepTime);
                    vEnquedBodyFrame = mConvertedFrames[mCurrentIdx];
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
            mConvertedFrames = new BodyFrame[mRawFrames.Count];
            
            for (int i = 0; i < mRawFrames.Count; i++)
            {
                try
                {
                    mConvertedFrames[i] = RawFrameConverter.ConvertRawFrame(mRawFrames[i]);

                }
                catch (Exception vE)
                {
                    if (i != 0)
                    {
                        mConvertedFrames[i] = mConvertedFrames[i - 1];
                    } 
                }

            }
            BodyFrame vFirst = mConvertedFrames[0];
            BodyFrame vLast = mConvertedFrames[mRawFrames.Count - 1];
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
            else if ( vConvertedRecIdx >= mConvertedFrames.Length)
            {
                vConvertedRecIdx = mConvertedFrames.Length - 1;
            }
            
            mFrameBuffer.Clear();
            mCurrentIdx = vConvertedRecIdx;
            mFrameBuffer.Enqueue(mConvertedFrames[mCurrentIdx]);
        }

        /// <summary>
        /// returns a body frame at the current index
        /// </summary>
        /// <param name="vIndex"></param>
        /// <returns></returns>
        public BodyFrame GetBodyFrameAtIndex(int vIndex)
        {
            if (mConvertedFrames.Length == 0)
            {
                return null;
            }
            if (vIndex <= 0)
            {
                return mConvertedFrames[0];
            }
            if (vIndex >= mConvertedFrames.Length)
            {
                return mConvertedFrames[mConvertedFrames.Length - 1];
            }
            else
            {
                return mConvertedFrames[vIndex];
            }
        }


    }
}