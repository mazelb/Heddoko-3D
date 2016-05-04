/* @file LiveSuitFeedView.cs
* @brief Contains the LiveSuitFeedView class
* @author Mohammed Haider(mohamed @heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.AbstractViews;
using Assets.Scripts.UI.AbstractViews.Layouts;
using System.Collections.Generic;
using Assets.Scripts.Body_Pipeline.Analysis.Views;
using Assets.Scripts.Communication.View.Table;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    ///   A live feed view
    /// </summary>
    public class LiveSuitFeedView : AbstractView
    {
        private LayoutType LayoutType = LayoutType.Single;
        public Layout CurrentLayout;
        private PanelNode[] mPanelNodes;
        private PanelNode mRootNode;
        public List<List<ControlPanelType>> ControlPanelTypeList = new List<List<ControlPanelType>>(2);
        public Body BrainpackBody;
        public BrainpackConnectionController BpController;
        private LiveFeedViewControlPanel mLiveFeedViewControlPanel;
        private bool mIsInitialized = false;
        public BodyFrameDataControl BodyFrameDataControl;
        public AnaylsisTextContainer AnaylsisTextContainer;
        public Button RenameRecordingButton;
        public Image RenameRecordingImage;
        public Text RenameRecordingText;
        void Awake()
        {
            List<ControlPanelType> vLeftSide = new List<ControlPanelType>();
            vLeftSide.Add(ControlPanelType.LiveBPFeedView);
            ControlPanelTypeList.Add(vLeftSide);
            CreateDefaultLayout();
        }


        /// <summary>
        /// Create a default layout for the live feed view
        /// </summary>
        public override void CreateDefaultLayout()
        {
            BodiesManager.Instance.CreateNewBody("BrainpackPlaceholderBody");
            BrainpackBody = BodiesManager.Instance.GetBodyFromUUID("BrainpackPlaceholderBody");
            CurrentLayout = new Layout(LayoutType, this);
            mPanelNodes = CurrentLayout.ContainerStructure.RenderingPanelNodes;
            mRootNode = mPanelNodes[0];
            mRootNode.name = "Main";

            mRootNode.PanelSettings.Init(ControlPanelTypeList[0], false, BrainpackBody);
            mLiveFeedViewControlPanel = (LiveFeedViewControlPanel)mRootNode.PanelSettings.GetPanelOfType(ControlPanelType.LiveBPFeedView);
            mLiveFeedViewControlPanel.SuitConnection = BpController;
            mLiveFeedViewControlPanel.Body = BrainpackBody;
            mLiveFeedViewControlPanel.gameObject.SetActive(true);
            mLiveFeedViewControlPanel.Show();
            mIsInitialized = true;
            BpController.ConnectedStateEvent += () => BrainpackBody.StreamFromBrainpack();
        }


        /// <summary>
        /// Display the suit feed view and set contextual info
        /// </summary>
        public override void Show()
        {
            gameObject.SetActive(true);
            bool vIsLerp = BodySegment.IsUsingInterpolation;
            if (CurrentLayout == null)
            {
                CreateDefaultLayout();
            }
            else
            {
                mPanelNodes[0].PanelSettings.RequestResources();
            }
            BrainpackBody.StreamFromBrainpack();
            try
            {
                BodySegment.IsUsingInterpolation = false;
                BrainpackBody.View.ResetInitialFrame();
            }
            catch
            {

            }
            BodySegment.IsUsingInterpolation = vIsLerp;
            SetContextualInfo();

            BpController.ConnectedStateEvent += SetRenameRecordingInteractibility;
            BpController.DisconnectedStateEvent += UnsetRenameRecordingInteractibility;
            bool vIsConnected = BpController.ConnectionState == BrainpackConnectionState.Connected;
            if (vIsConnected)
            {
                SetRenameRecordingInteractibility();
            }
            else
            {
                UnsetRenameRecordingInteractibility();
            }
        }

        /// <summary>
        /// Set information relative to the context of this view
        /// </summary>
        private void SetContextualInfo()
        {
            BodyFrameDataControl.Clear();
            BodyFrameDataControl.SetBody(BrainpackBody);
            AnaylsisTextContainer.BodyToAnalyze = BrainpackBody;
        }

        public override void Hide()
        {
            bool vIsLerp = BodySegment.IsUsingInterpolation;
            try
            {
                BodySegment.IsUsingInterpolation = false;
                BrainpackBody.View.ResetInitialFrame(); 
            }
            catch
            {

            }
            //it is entirely possible that the view hasn't been initialized. verify this before proceeding
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
            BpController.ConnectedStateEvent -= SetRenameRecordingInteractibility;
            BpController.DisconnectedStateEvent -= UnsetRenameRecordingInteractibility;
            UnsetRenameRecordingInteractibility();
           
           
            try
            { 
                BrainpackBody.StopThread();
            }
            catch
            {

            }
            BodySegment.IsUsingInterpolation = vIsLerp;
        }

        void SetRenameRecordingInteractibility()
        {
            RenameRecordingButton.interactable = true;
            Color vColorImage = RenameRecordingImage.color;
            vColorImage.a = 1;
            RenameRecordingImage.color = vColorImage;
            Color vColorText = RenameRecordingText.color;
            vColorText.a = 1;
            RenameRecordingText.color = vColorText;
        }

        void UnsetRenameRecordingInteractibility()
        {
            RenameRecordingButton.interactable = false;
            Color vColorImage = RenameRecordingImage.color;
            vColorImage.a = 0.25f;
            RenameRecordingImage.color = vColorImage;
            Color vColorText = RenameRecordingText.color;
            vColorText.a = 0.25f;
            RenameRecordingText.color = vColorText;
        }

       
    }
}