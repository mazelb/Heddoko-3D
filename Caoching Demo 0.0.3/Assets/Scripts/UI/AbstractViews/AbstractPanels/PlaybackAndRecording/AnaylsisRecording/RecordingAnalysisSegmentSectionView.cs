/**
* @file RecordingAnalysisSegmentSectionView.cs
* @brief Contains the RecordingAnalysisSegmentSectionView
* @author Mohammed Haider( mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Body_Pipeline.Analysis;
using HeddokoLib.genericPatterns;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording.AnaylsisRecording
{
    /// <summary>
    /// a section of a recording to analyze
    /// </summary>
    public class RecordingAnalysisSegmentSectionView : MonoBehaviour, IReleasableResource
    {
        private IEnumerator mStartIndexError;
        private IEnumerator mEndIndexError;
        public InputField StartIndexInputField;
        public InputField EndIndexInputField;
        public Text ErrorLabelText;

        private readonly RecordingAnalysisSectionModel mRecordingAnalysisSectionModel = new RecordingAnalysisSectionModel();

        public RecordingAnalysisSectionModel AnalysisSectionModel
        {
            get { return mRecordingAnalysisSectionModel; }
        }


        /// <summary>
        /// Initialize the segment analysis section with respect to the passed in Segment analysis list to analize
        /// </summary>
        /// <param name="vSegmentAnalysis"></param>
        public void Init(List<SegmentAnalysis> vSegmentAnalysis)
        {
            mRecordingAnalysisSectionModel.DataStore = new AnalysisDataStore(vSegmentAnalysis);
            gameObject.SetActive(true);
            StartIndexInputField.onValueChanged.RemoveAllListeners();
            EndIndexInputField.onValueChanged.RemoveAllListeners();
            StartIndexInputField.onValueChanged.AddListener(SetStartIndex);
            StartIndexInputField.contentType = InputField.ContentType.IntegerNumber;
            EndIndexInputField.onValueChanged.AddListener(SetEndIndex);
            EndIndexInputField.contentType = InputField.ContentType.IntegerNumber;
        }
        /// <summary>
        /// Set the max index value
        /// </summary>
        /// <param name="vMax"></param>
        public void SetMaxIndexValue(int vMax)
        {
            mRecordingAnalysisSectionModel.Init(vMax);
        }
    
        private void SetEndIndex(string vArg0)
        {
            try
            {
                int vEndIndex = 0;
                int.TryParse(vArg0, out vEndIndex);
                mRecordingAnalysisSectionModel.EndIndex = vEndIndex;
            }
            catch (RecordingAnalysisSectionModel.InvalidEndIndexException vE)
            {
                if (mEndIndexError != null)
                {
                    StopCoroutine(mEndIndexError);
                }
                mEndIndexError =
                    AnimationHelpers.FadeTextBoxWithMessage(
                        "THE END INDEX MUST BE LARGER THAN THE START INDEX OR LESS THAN " + mRecordingAnalysisSectionModel.MaxIndexValue, ErrorLabelText);
                StartCoroutine(mEndIndexError);
            }
        }

 
        /// <summary>
        /// Sets the starting index
        /// </summary>
        /// <param name="vArg0"></param>
        private void SetStartIndex(string vArg0)
        {
            try
            {
                int vStartIndex = 0;
                int.TryParse(vArg0, out vStartIndex);
                mRecordingAnalysisSectionModel.StartIndex = vStartIndex;
            }
            catch (RecordingAnalysisSectionModel.InvalidStartIndexException vE)
            {
                if (mStartIndexError != null)
                {
                    StopCoroutine(mStartIndexError);
                }
                mStartIndexError =
                    AnimationHelpers.FadeTextBoxWithMessage(
                        "THE START INDEX MUST BE LARGER THAN 0 OR SMALLER THAN THE END INDEX " , ErrorLabelText);
                StartCoroutine(mStartIndexError);
            }
        }

        public void Cleanup()
        {
            AnalysisSectionModel.Reset();
            gameObject.SetActive(false);
            StartIndexInputField.text = "0";
            EndIndexInputField.text = "1";
 
        }
    }
}