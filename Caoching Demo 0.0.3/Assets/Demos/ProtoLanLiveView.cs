// /**
// * @file ProtoLanLiveView.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 09 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;
using Assets.Scripts.UI.RecordingLoading;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Communication;
using Assets.Scripts.Communication.Controller;

namespace Assets.Demos
{
    public class ProtoLanLiveView : LiveSuitFeedView
    {
        public UdpSocketListener mSocketListener; 
        void Awake()
        {
            mSocketListener = new UdpSocketListener();
               BodySegment.GBodyFrameUsingQuaternion = true;
            List<ControlPanelType> vLeftSide = new List<ControlPanelType>();
            vLeftSide.Add(ControlPanelType.LiveBPFeedView);
            ControlPanelTypeList.Add(vLeftSide);
            Hide();
        }

        void Start()
        {
         
        }

        void OnApplicationQuit()
        {
            mSocketListener.Stop();
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
            mLiveFeedViewControlPanel.Body = BrainpackBody;
            mLiveFeedViewControlPanel.SuitChanger.SuitConnection = BpController;
            mLiveFeedViewControlPanel.gameObject.SetActive(true);
            mLiveFeedViewControlPanel.Show();
            mIsInitialized = true;
           
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
            BodySegment.GBodyFrameUsingQuaternion = true;
            BrainpackBody.PlayFromDataStream(mSocketListener.FrameRouter);
            mSocketListener.Start();
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
             
            
        }

        /// <summary>
        /// Set information relative to the context of this view
        /// </summary>
        private void SetContextualInfo()
        {
            BodyFrameDataControl.SetBody(BrainpackBody);
            FrameGraphControl.SetBody(BrainpackBody);
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
                mSocketListener.Stop();
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