/** 
* @file RecordingStats.cs
* @brief Contains the RecordingStats class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System.Collections.Generic;
 

namespace Assets.Scripts.Frames_Pipeline
{
    /**
  * Body class 
  * @brief RecordingStats class (contains statistics of a recording) 
  */
    /// <summary>
    /// Represents statistics of a body raw frame
    /// </summary>
    public class RecordingStats
    { 
        private float mStartRecordingTime;
        private float mEndRecordingTime;
        private int mTotalFrames;
        private float mAverageSecondsBetweenFrames;
        /// <summary>
        /// frames per second
        /// </summary>
        internal float FramesPerSeconds
        {
            get
            {
                if (mTotalFrames < 1)
                {
                    return 0;
                }
                return mTotalFrames / TotalTime;
            }
        }

        internal int TotalFrames
        {
            get { return mTotalFrames; }
        }
        /// <summary>
        /// The total time of the recording
        /// </summary>
        internal float TotalTime
        {
            get
            {
                return mEndRecordingTime - mStartRecordingTime;
            }
        }
        /// <summary>
        /// The average time between frames recording
        /// </summary>
        public float AverageSecondsBetweenFrames
        { get { return mAverageSecondsBetweenFrames; } }
        /// <summary>
        /// Initializes fields and properties, setting the RecordingStats attributes
        /// </summary>
        /// <param name="vBodyRawFrame"></param>
        public void InitAndAnalyze(BodyFramesRecording vBodyRawFrame)
        { 
            List<BodyRawFrame> vListOfBodyRawFrames = vBodyRawFrame.RecordingRawFrames;
            mTotalFrames = vListOfBodyRawFrames.Count;
            float.TryParse(vListOfBodyRawFrames[0].RawFrameData[0], out mStartRecordingTime);
            float.TryParse(vListOfBodyRawFrames[mTotalFrames - 1].RawFrameData[0], out mEndRecordingTime);
            if (mTotalFrames > 1)
            { 
                float vSum = 0;
                float mT1 = mStartRecordingTime;
                for (int i = 1; i < mTotalFrames; i++)
                {
                    float mT2 = 0;
                    float.TryParse(vListOfBodyRawFrames[i].RawFrameData[0], out mT2);
                    vSum += (mT2 - mT1);
                    mT1 = mT2;
                }
          
                mAverageSecondsBetweenFrames = vSum / (mTotalFrames - 1);
            }
           
        }
    }
}
