// /**
// * @file BrainpackMessagePerSecond.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 11 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
using heddoko;
using HeddokoLib.adt;
using UnityEngine;

namespace Assets.Scripts.Communication.View
{
    public class BrainpackMessagePerSecond
    {
        private float mStartTimeInSeconds;
        private float mTotalTimeCounted;
        private int mTimeSlices;
        private int mFrameCounter;
        public float BmPs;
        public float AverageBmPs;
        public bool Initialized = false;
        public int MaxIntevalCount = 10;
        private List<float> mDtIntervals = new List<float>(); 


        public void Init(Packet vDataFrame)
        {
            mFrameCounter = 0;
            mStartTimeInSeconds = vDataFrame.fullDataFrame.timeStamp;
            mTotalTimeCounted = 0;
            mTimeSlices = 0;
            Initialized = true;
        }

        public void CountFrame(Packet vDataFrame)
        {
            
            mFrameCounter++;
            float vCurrentTime = vDataFrame.fullDataFrame.timeStamp;

            float vDiff = vCurrentTime - mStartTimeInSeconds;
           
            if (vDiff >= 1000f)
            {
                if (mFrameCounter != 0)
                {
                    BmPs = vDiff / mFrameCounter;
                }
                mTotalTimeCounted += BmPs;
                mStartTimeInSeconds = vCurrentTime;
                mFrameCounter = 0;
                mTimeSlices++;
                AverageBmPs = mTotalTimeCounted/mTimeSlices;
            }
             
        }

         
    }
}