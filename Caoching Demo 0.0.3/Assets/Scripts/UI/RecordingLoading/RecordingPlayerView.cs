
/** 
* @file RecordingPlayerView.cs
* @brief Contains the RecordingPlayerView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts; 
using System.Collections.Generic;
using Assets.Scripts.Body_Pipeline.Analysis.Views;
using Assets.Scripts.Communication.View.Table;
using Assets.Scripts.UI.AbstractViews;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    /// Single Player view
    /// </summary>
    public class RecordingPlayerView : AbstractView
    {
        private LayoutType LayoutType = LayoutType.Single;
        public Layout CurrentLayout;
        private PanelNode[] mPanelNodes;
        public List<List<ControlPanelType>> ControlPanelTypeList = new List<List<ControlPanelType>>(2);
        private bool mIsInitialized = false;
        private Body mCurrBody;
        public BodyFrameDataControl BodyFrameDataControl;
        public Button LoadRecordingButton; 
        public AnaylsisTextContainer AnaylsisTextContainer;
        void Awake()
        {
            List<ControlPanelType> vLeftSide = new List<ControlPanelType>();
            vLeftSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            List<ControlPanelType> vRightSide = new List<ControlPanelType>();
            vRightSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            ControlPanelTypeList.Add(vLeftSide);
            ControlPanelTypeList.Add(vRightSide);
            CreateDefaultLayout();
        }



        public override void CreateDefaultLayout()
        {
            BodiesManager.Instance.CreateNewBody("Root");
            mCurrBody = BodiesManager.Instance.GetBodyFromUUID("Root");
            CurrentLayout = new Layout(LayoutType, this);
            mPanelNodes = CurrentLayout.ContainerStructure.RenderingPanelNodes;
            mPanelNodes[0].name = "Main";
            mPanelNodes[0].PanelSettings.Init(ControlPanelTypeList[0], false, mCurrBody);
            mIsInitialized = true;
            PlaybackControlPanel vPbCtrlPanel =
                (PlaybackControlPanel)
                    mPanelNodes[0].PanelSettings.GetPanelOfType(ControlPanelType.RecordingPlaybackControlPanel);
            vPbCtrlPanel.BodyUpdatedEvent += SetNewBody;
            vPbCtrlPanel.SingleRecordingLoadSubControl.SetNewButtonControl(LoadRecordingButton);
            //Call the load recording panel
            vPbCtrlPanel.SingleRecordingLoadSubControl.SelectedRecording();
        }

        /// <summary>
        /// releases current resources
        /// </summary>
        public override void Hide()
        {
          
            if (mIsInitialized)
            {
                foreach (var vPanelNodes in mPanelNodes)
                {
                    vPanelNodes.PanelSettings.ReleaseResources();
                }
            }
            gameObject.SetActive(false);
            if (PreviousView != null)
            {
                PreviousView.Show();
            }
        }

        public override void Show()
        {
            gameObject.SetActive(true);

            if (CurrentLayout == null)
            {
                CreateDefaultLayout();
            }
            else
            { 
                mPanelNodes[0].PanelSettings.RequestResources();
            }
            SetContextualInfo();
            try
            {
                mCurrBody.View.ResetInitialFrame();
            }
            catch
            {
               
            }

        }

        /// <summary>
        /// Set a new body to this view
        /// </summary>
        /// <param name="vBody"></param>
        private void SetNewBody(Body vBody)
        {
            mCurrBody = vBody;
            SetContextualInfo();
        }

        /// <summary>
        /// Set contexual info for this view
        /// </summary>
        private void SetContextualInfo()
        {
            BodyFrameDataControl.Clear();
            BodyFrameDataControl.SetBody(mCurrBody);
            AnaylsisTextContainer.BodyToAnalyze = mCurrBody;
        }

    }
}