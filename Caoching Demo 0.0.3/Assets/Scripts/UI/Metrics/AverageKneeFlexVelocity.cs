/** 
* @file AverageKneeFlexVelocity.cs
* @brief Contains the AverageKneeFlexVelocity class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System;
using System.Collections;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics
{
    /// <summary>
    /// Compares a previous time frames velocity to the most current time frame average velocity
    /// </summary>
    [Serializable]
    public class AverageKneeFlexVelocity: MonoBehaviour
    {
        //add angular velocity over a given time frame
        private float[] mVelAccumulator = new float[2];

        //mVelAccumulator index
        [SerializeField]
        private int mVelAccIndex = 0;

        [SerializeField]
        private float mNumberOfFramesToCheck = 5f;
        private float mCurrentFrameCheck;
     
        private RightLegAnalysis mRightLegAnalysis;

     
        private string mOutputText = "";

        public AverageKneeFlexVelocity(Body vBody)
        {
            mRightLegAnalysis =
                     vBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                         RightLegAnalysis;
        }
        /// <summary>
        /// to string, returns the analysis on 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            CompareAverageKneeFlexionSpeed();
            return mOutputText;
        }
        public void Reset()
        {
            mVelAccumulator = new float[2];
            mVelAccumulator[0] = -1;
            mVelAccumulator[1] = -1;
            mVelAccIndex = 0;
        }

        /// <summary>
        /// Compares average knee flexion speed to the number of frames specifed
        /// </summary>
        /// <returns></returns>
        public float CompareAverageKneeFlexionSpeed()
        {
            float vOutput = -1;
            //just started
            if (mVelAccumulator[0] < 0 && mVelAccumulator[1] < 0)
            {
                mVelAccumulator[0] = 0;
                mVelAccumulator[0] += Mathf.Abs(mRightLegAnalysis.AngularVelocityKneeFlexion);
                //  mCurrentFrameCheck += Time.deltaTime;
                mCurrentFrameCheck =0;
                return -1f;
            }

            mVelAccumulator[mVelAccIndex] += Mathf.Abs(mRightLegAnalysis.AngularVelocityKneeFlexion);
            mCurrentFrameCheck += Time.deltaTime;

            if (mCurrentFrameCheck >= mNumberOfFramesToCheck)
            {
                int vComparingIndex = mVelAccIndex == 0 ? 1 : mVelAccIndex = 0;

                //check if we can actually compare this data
                if (mVelAccumulator[vComparingIndex] < 0)
                {
                    mCurrentFrameCheck = 0;
                    mVelAccumulator[1] = 0;
                    mVelAccIndex = 1;
                    return -1f;
                }

                //going slower for the past mNumberOfFramesToCheck
                if (mVelAccumulator[mVelAccIndex] < mVelAccumulator[vComparingIndex] - 0.1f)
                {
                    //mOutputText  = "You're going slower";
                  
                    mVelAccumulator[vComparingIndex] = mVelAccumulator[mVelAccIndex];
                    mVelAccumulator[mVelAccIndex] = 0;
                    vOutput = mVelAccumulator[vComparingIndex];
                }

                //relatively the same speed
                else if (Mathf.Abs(mVelAccumulator[mVelAccIndex] - mVelAccumulator[vComparingIndex]) < 0.1f)
                {
                   // mOutputText = "You're going the same speed ";
                    mVelAccumulator[vComparingIndex] = mVelAccumulator[mVelAccIndex];
                    mVelAccumulator[mVelAccIndex] = 0;
                    vOutput = mVelAccumulator[vComparingIndex];
                }

                else
                {
                    //mOutputText = "You're going faster";
                    mVelAccumulator[vComparingIndex] = mVelAccumulator[mVelAccIndex];
                    mVelAccumulator[mVelAccIndex] = 0;
                    vOutput = mVelAccumulator[vComparingIndex];
                }
              //  mVelAccIndex = vComparingIndex;
                mCurrentFrameCheck = 0;
                
            }
            return vOutput/ mNumberOfFramesToCheck;
        }

    }
}
