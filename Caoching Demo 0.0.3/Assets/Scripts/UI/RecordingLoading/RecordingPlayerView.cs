
/** 
* @file RecordingPlayerView.cs
* @brief Contains the RecordingPlayerView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;
using System.Collections.Generic;
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Pipeline.Analysis.Views;
using Assets.Scripts.Communication.View.Table;
using Assets.Scripts.ErrorHandling;
using Assets.Scripts.ErrorHandling.Model;
using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.Localization;
using Assets.Scripts.Tests;
using Assets.Scripts.UI.AbstractViews;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;
using Assets.Scripts.UI.AbstractViews.Permissions;
using Assets.Scripts.UI.Loading;
using Assets.Scripts.UI.RecordingLoading;
using Assets.Scripts.Utils;
using HeddokoSDK.Models;
using UnityEngine;
using UnityEngine.UI;

public delegate void RecordingPlayerViewLayoutCreated(RecordingPlayerView vView);
namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    /// Single Player view
    /// </summary>
    [UserRolePermission(new[] { UserRoleType.Analyst })]
    public class RecordingPlayerView : AbstractView
    {
        private LayoutType LayoutType = LayoutType.Single;
        public Layout CurrentLayout;
        private PanelNode[] mPanelNodes;
        public List<List<ControlPanelType>> ControlPanelTypeList = new List<List<ControlPanelType>>(2);
        public PlaybackControlPanel PbControlPanel;
        private bool mIsInitialized = false;
        public Body CurrBody;
        public BodyFrameDataControl BodyFrameDataControl;
        public BodyFrameGraphControl FrameGraphControl;
        public AnaylsisTextContainer AnaylsisTextContainer;
        public event RecordingPlayerViewLayoutCreated RecordingPlayerViewLayoutCreatedEvent;
        public CloudLocalStorageViewManager CloudLocalStorageViewManager;
        public Action<RecordingPlayerView> RecordingPlayerViewLayoutEnabled;
        public PanelNode RootNode
        {
            get { return mPanelNodes[0]; }
        }

        public bool Initialized { get; set; }

        void Awake()
        {
            List<ControlPanelType> vLeftSide = new List<ControlPanelType>();
            vLeftSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            List<ControlPanelType> vRightSide = new List<ControlPanelType>();
            vRightSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            ControlPanelTypeList.Add(vLeftSide);
            ControlPanelTypeList.Add(vRightSide);
            Hide();
        }

        void OnEnable()
        {
            if (RecordingPlayerViewLayoutEnabled != null)
            {
                RecordingPlayerViewLayoutEnabled(this);
            }
        }
        /// <summary>
        /// A hooking function to enable the progress bar when a recording is beginning to load
        /// </summary>
        private void StartLoadHookFunc()
        {
            var vProgressBar = DisablingProgressBar.Instance;
            if (vProgressBar != null)
            {
                OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => vProgressBar.StartProgressBar("LOADING RECORDING"));
            }
        }

        /// <summary>
        /// A hooking function to disable the progress bar when a recording has finished loading
        /// </summary>
        private void StopLoadHookFunc()
        {
            var vProgressBar = DisablingProgressBar.Instance;
            if (vProgressBar != null)
            {
                OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => vProgressBar.StopAnimation());
            }

        }


        public override void CreateDefaultLayout()
        {
            CurrentLayout = new Layout(LayoutType, this);
            BodiesManager.Instance.CreateNewBody("Root");
            CurrBody = BodiesManager.Instance.GetBodyFromUUID("Root");
            mPanelNodes = CurrentLayout.ContainerStructure.RenderingPanelNodes;
            mPanelNodes[0].name = "Main";
            mPanelNodes[0].PanelSettings.Init(ControlPanelTypeList[0], true, CurrBody);
            mIsInitialized = true;
            PbControlPanel =
                (PlaybackControlPanel)
                    mPanelNodes[0].PanelSettings.GetPanelOfType(ControlPanelType.RecordingPlaybackControlPanel);
            PbControlPanel.BodyUpdatedEvent += SetNewBody;
            CloudLocalStorageViewManager.RecordingLoadingCompleteEvent += PbControlPanel.NewRecordingSelected;
            if (RecordingPlayerViewLayoutCreatedEvent != null)
            {
                RecordingPlayerViewLayoutCreatedEvent(this);
            }

            //setup error handler
            RecordingErrorHandlerManager.Instance.AddErrorHandler("ErrorParsing", new RecordingErrorHandler(HandleErrorLoadingFile));
            RecordingErrorHandlerManager.Instance.AddErrorHandler("IssueLoading", new RecordingErrorHandler(HandleErrorLoadingFile));
            Initialized = true;
        }

        private void HandleErrorLoadingFile(BodyFramesRecordingBase vObj)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                () =>
                {
                    PbControlPanel.ReleaseResources();
                    LoadingBoard.StopLoadingAnimation();
                    string vErr = LocalizationBinderContainer.GetString(KeyMessage.CannotLoadRecording);
                    ModalWindow.ModalPanel.SingleChoice("ERROR", vErr, () => { });
                }
           );
        }


        /// <summary>
        /// releases current resources
        /// </summary>
        public override void Hide()
        {
            bool vIsLerp = BodySegment.IsUsingInterpolation;
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
            try
            {
                BodySegment.IsUsingInterpolation = false;
                CurrBody.View.ResetInitialFrame();
            }
            catch
            {

            }
            BodySegment.IsUsingInterpolation = vIsLerp;
        }

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
            SetContextualInfo();
            try
            {
                BodySegment.IsUsingInterpolation = false;
                CurrBody.View.ResetInitialFrame();
            }
            catch
            {

            }
            BodySegment.IsUsingInterpolation = vIsLerp;
        }

        /// <summary>
        /// Set a new body to this view
        /// </summary>
        /// <param name="vBody"></param>
        private void SetNewBody(Body vBody)
        {
            CurrBody = vBody;
            SetContextualInfo();
        }

        /// <summary>
        /// Set contexual info for this view
        /// </summary>
        private void SetContextualInfo()
        {
            BodyFrameDataControl.SetBody(CurrBody);
            FrameGraphControl.SetBody(CurrBody);
            AnaylsisTextContainer.BodyToAnalyze = CurrBody;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                try
                {
                    bool vPrev = BodySegment.IsUsingInterpolation;
                    BodySegment.IsUsingInterpolation = false;
                    CurrBody.View.ResetInitialFrame();
                    BodySegment.IsUsingInterpolation = vPrev;
                }
                catch (Exception)
                {
                }
            }
        }

        public void SetPermissions(UserProfileModel vProfileModel)
        {
            var vUserRole = vProfileModel.User.RoleType;
            PbControlPanel.SetInteractionLevel(vUserRole);
            //get the role and apply it to the child control panels.. 
            //maybe apply it on every subcontrol panel
        }
    }
}
