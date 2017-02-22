/** 
* @file AnaylsisTextContainer.cs
* @brief Contains the AnaylsisTextContainer  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System.Collections.Generic;
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Data.View.Anaylsis;
using Assets.Scripts.Body_Data.View.Anaylsis.AnalysisTextViews;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisTextViews;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Pipeline.Analysis.Views
{
    /// <summary>
    /// Containers for Analysis text views
    /// </summary>
    public class AnaylsisTextContainer : MonoBehaviour
    { 
        [SerializeField]
        private Body mBody;
        public GameObject NoBodyWarningPanel; 
        public TrunkAnaylsisTextView TrunkAnalysis;
        public ElbowAnalysisTextView ElbowAnalysis;
        public KneeAnalysisTextView KneeAnalysis;
        public HipsAnalysisTextView HipsAnalysis;
        public ShoulderAnalyisTextView ShoulderAnalysis;

        private Dictionary<CurrentAnalysisTextView, AnaylsisTextView> mCurrentViews;

        private CurrentAnalysisTextView mCurrentAnalysisText = CurrentAnalysisTextView.None;


        public Body BodyToAnalyze
        {
            get { return mBody; }
            set
            {
                mBody = value;
                if (mBody != null)
                {
                    foreach (var vAnalysisView in CurrentViews)
                    {
                        vAnalysisView.Value.BodyToAnalyze = mBody;
                    }
                }
                else
                {
                    NoBodyWarningPanel.SetActive(true);
                }
            }
        }

        private Dictionary<CurrentAnalysisTextView, AnaylsisTextView> CurrentViews
        {
            get
            {
                //initialize the views
                if (mCurrentViews == null)
                {
                    mCurrentViews = new Dictionary<CurrentAnalysisTextView, AnaylsisTextView>();
                    mCurrentViews.Add(CurrentAnalysisTextView.Elbows, ElbowAnalysis);
                    mCurrentViews.Add(CurrentAnalysisTextView.Trunk, TrunkAnalysis);
                    mCurrentViews.Add(CurrentAnalysisTextView.Knees, KneeAnalysis);
                    mCurrentViews.Add(CurrentAnalysisTextView.Hips, HipsAnalysis);
                    mCurrentViews.Add(CurrentAnalysisTextView.Shoulders, ShoulderAnalysis);

                }
                return mCurrentViews;
            }
        }


        

        /// <summary>
        /// Changes the analysis view to the one requested
        /// </summary>
        /// <param name="vNewView"></param>
        public void ChangeAnalysisView(CurrentAnalysisTextView vNewView)
        {
            if (vNewView == CurrentAnalysisTextView.None || BodyToAnalyze == null)
            {
                HideAll();
                mCurrentAnalysisText = CurrentAnalysisTextView.None;
                return;
            }
            else
            {
                //hide the current view
                if (mCurrentAnalysisText != CurrentAnalysisTextView.None)
                {
                    CurrentViews[mCurrentAnalysisText].Hide();
                }
                else
                {
                    NoBodyWarningPanel.SetActive(false);
                }
                //Display the current view
                CurrentViews[vNewView].Show(); 
            }
            mCurrentAnalysisText = vNewView;
        }

        /// <summary>
        /// Hides all views
        /// </summary>
        private void HideAll()
        {
            foreach (var vKvPair in CurrentViews)
            {
                if (vKvPair.Key != CurrentAnalysisTextView.None)
                {
                    vKvPair.Value.Hide();
                }
            }
            NoBodyWarningPanel.SetActive(true);
        }

        public void DisableAllRenderers()
        {
            foreach (var vKvPair in CurrentViews)
            {
                if (vKvPair.Key != CurrentAnalysisTextView.None)
                {
                    vKvPair.Value.Hide();
                }
            }
            NoBodyWarningPanel.SetActive(true);
        }
        public enum CurrentAnalysisTextView
        {
            Trunk,
            Shoulders,
            Elbows,
            Hips,
            Knees,
            None
        }
    }
}