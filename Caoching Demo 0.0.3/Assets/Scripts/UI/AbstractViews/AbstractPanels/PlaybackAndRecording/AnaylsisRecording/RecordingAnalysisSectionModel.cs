// /**
// * @file RecordingAnalysisSectionModel.cs
// * @brief Contains the RecordingAnalysisSectionModel
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */
using System;
using Assets.Scripts.Body_Pipeline.Analysis;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording.AnaylsisRecording
{
    public class RecordingAnalysisSectionModel
    {
        public int MaxIndexValue { get; set; }
        private int mStartIndex;
        private int mEndIndex;
        private AnalysisDataStore mDataStore;
 
        public AnalysisDataStore DataStore
        {
            get { return mDataStore; }
            set { mDataStore = value; }
        }

        
        /// <summary>
        /// Sets OR gets the start index
        /// <exception cref="InvalidStartIndexException">An InvalidStartIndexException is thrown if the index is smaller than 0 or greater than the maximum allowable index value</exception>
        /// </summary>

        public int StartIndex
        {
            get
            {
                return mStartIndex;
            }
            set
            {
                int vTemp = value;
                if (vTemp >= 0 && vTemp < EndIndex)
                {
                    mStartIndex = vTemp;
                }
                else
                {
                     throw new InvalidStartIndexException();
                }
            }
        }
        /// <summary>
        /// Set or gets the end index
        /// <exception cref="InvalidEndIndexException">An InvalidEndIndexException is thrown if the index is smaller than the start index  or greater than the maximum allowable index value</exception>
        /// </summary> 
        public int EndIndex
        {
            get { return mEndIndex; }
            set
            {
                int vTemp = value;
                if (vTemp > mStartIndex && vTemp <= MaxIndexValue)
                {
                    mEndIndex = vTemp;
                }
                else
                {
                    throw new InvalidEndIndexException();
                }
         
            }
        }

        /// <summary>
        /// Initializes the object with the maximum allowable index value
        /// </summary>
        /// <param name="vMaxIndex"></param>
        public void Init(int vMaxIndex)
        {
            MaxIndexValue = vMaxIndex;
            EndIndex = MaxIndexValue;
        }

        public void Reset()
        {
            MaxIndexValue = 2;
            mStartIndex = 0;
            mEndIndex = 1;
            DataStore.Clear();
        }


        public class InvalidStartIndexException: Exception
        {
        }

        public class InvalidEndIndexException : Exception
        {
        }

        
    }
}