// /**
// * @file ProtoDemoLiveViewConnection.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 06 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;
using Assets.Scripts.UI.RecordingLoading;
using UnityEngine;
using System.Collections.Generic; 
namespace Assets.Demos
{
    public class ProtoDemoLiveViewConnection : LiveSuitFeedView
    { 
        public ProtoDemoController DemoController;
        public PanelNode RootNode { get { return mRootNode; } }
        void Awake()
        {
            List<ControlPanelType> vLeftSide = new List<ControlPanelType>();
            vLeftSide.Add(ControlPanelType.LiveBPFeedView);
            ControlPanelTypeList.Add(vLeftSide);
            Hide();
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

            mRootNode.PanelSettings.Init(ControlPanelTypeList[0], true, BrainpackBody);
            mLiveFeedViewControlPanel = (LiveFeedViewControlPanel)mRootNode.PanelSettings.GetPanelOfType(ControlPanelType.LiveBPFeedView);
            mLiveFeedViewControlPanel.SuitConnection = BpController;
            mLiveFeedViewControlPanel.Body = BrainpackBody;
            mLiveFeedViewControlPanel.gameObject.SetActive(true);
            mLiveFeedViewControlPanel.Show();
            mIsInitialized = true;
            BpController.ConnectedStateEvent += () =>
            {
                if (ProtoDemoController.UseProtoBuff)
                {
                    BrainpackBody.PlayFromDataStream(DemoController.FrameRouter);
                }
                else
                {
                    BrainpackBody.StreamFromBrainpack();
                }
            };
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
            if (ProtoDemoController.UseProtoBuff)
            {
                BrainpackBody.PlayFromDataStream(DemoController.FrameRouter);
            }
            else
            {
                BrainpackBody.StreamFromBrainpack();
            }
           
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
            BodyFrameDataControl.SetBody(BrainpackBody);
      //      FrameGraphControl.SetBody(BrainpackBody);
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