
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
using Assets.Scripts.Body_Pipeline.Analysis.Views;
using Assets.Scripts.Communication.View.Table;
using Assets.Scripts.Licensing.Model;
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
using Assets.Scripts.Utils.DebugContext;

public delegate void RecordingPlayerViewLayoutCreated(RecordingPlayerView vView);
namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    /// Single Player view
    /// </summary>
    [UserRolePermission(new []{UserRoleType.Analyst})]
    public class RecordingPlayerView : AbstractView
    {
        private LayoutType LayoutType = LayoutType.Single;
        public Layout CurrentLayout;
        private PanelNode[] mPanelNodes;
        public List<List<ControlPanelType>> ControlPanelTypeList = new List<List<ControlPanelType>>(2);
        public PlaybackControlPanel PbControlPanel;
        private bool mIsInitialized = false;
        public Body CurrBody;
        public Body CurrRef;
        public BodyFrameDataControl BodyFrameDataControl;
        public BodyFrameGraphControl FrameGraphControl;
        public Button LoadRecordingButton;
        public AnaylsisTextContainer AnaylsisTextContainer;
        public event RecordingPlayerViewLayoutCreated RecordingPlayerViewLayoutCreatedEvent;
        public PanelNode RootNode
        {
            get { return mPanelNodes[0]; }
        }

        void Awake()
        {
            List<ControlPanelType> vLeftSide = new List<ControlPanelType>();
            vLeftSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            List<ControlPanelType> vRightSide = new List<ControlPanelType>();
            vRightSide.Add(ControlPanelType.RecordingPlaybackControlPanel);

            List<ControlPanelType> vcenter = new List<ControlPanelType>();
            vcenter.Add(ControlPanelType.CameraControlPanel);

            ControlPanelTypeList.Add(vLeftSide);
            ControlPanelTypeList.Add(vRightSide);
            ControlPanelTypeList.Add(vcenter);

            SingleRecordingSelection.Instance.StartLoadingEvent += StartLoadHookFunc;
            SingleRecordingSelection.Instance.FinishLoadingEvent += StopLoadHookFunc;
            Hide();

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
            CurrBody = BodiesManager.Instance.CreateNewBody("Root");

            CurrRef = BodiesManager.Instance.CreateNewBody("Reference");
            CurrBody.RangeOfMotion.Reference = CurrRef;
            CurrRef.UpdateRenderedBody(Body_Data.View.RenderedBodyPool.RequestResource(CurrRef.BodyType, true));
            CurrRef.RenderedBody.transform.Translate(2*Vector3.left);

            //CurrBody = BodiesManager.Instance.GetBodyFromUUID("Root");
            mPanelNodes = CurrentLayout.ContainerStructure.RenderingPanelNodes;
            mPanelNodes[0].name = "Main";
            mPanelNodes[0].PanelSettings.Init(ControlPanelTypeList[0], true, CurrBody);
            mIsInitialized = true;
            PbControlPanel =
                (PlaybackControlPanel)
                    mPanelNodes[0].PanelSettings.GetPanelOfType(ControlPanelType.RecordingPlaybackControlPanel);
            PbControlPanel.BodyUpdatedEvent += SetNewBody;
            PbControlPanel.SingleRecordingLoadSubControl.SetNewButtonControl(LoadRecordingButton);
            if (RecordingPlayerViewLayoutCreatedEvent != null)
            {
                RecordingPlayerViewLayoutCreatedEvent(this);
            }
        }

        /// <summary>
        /// releases current resources
        /// </summary>
        public override void Hide()
        {
            bool vIsLerp = BodySegment.Flags.IsUsingInterpolation;
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
                BodySegment.Flags.IsUsingInterpolation = false;
                CurrBody.View.ResetInitialFrame();
                CurrRef.View.ResetInitialFrame();
            }
            catch
            {

            }
            BodySegment.Flags.IsUsingInterpolation = vIsLerp;
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            bool vIsLerp = BodySegment.Flags.IsUsingInterpolation;
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
                BodySegment.Flags.IsUsingInterpolation = false;
                CurrBody.View.ResetInitialFrame();
                CurrRef.View.ResetInitialFrame();
            }
            catch
            {

            }
            BodySegment.Flags.IsUsingInterpolation = vIsLerp;
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

            Button[] vChildButtons = mPanelNodes[0].GetComponentsInChildren<Button>();

            Button TPoseButton = null;
            for (int i = 0; i < vChildButtons.Length; ++i)
                if (vChildButtons[i].name.Contains("Tpose"))
                {
                    TPoseButton = vChildButtons[i];
                    TPoseButton.onClick.RemoveAllListeners();
                    TPoseButton.onClick.AddListener(() =>
                    {
                        if (CurrBody.InitialBodyFrame != null)
                        {
                            bool vPrev = BodySegment.Flags.IsUsingInterpolation;
                            BodySegment.Flags.IsUsingInterpolation = false;
                            CurrBody.View.ResetInitialFrame();
                            CurrRef.View.ResetInitialFrame();
                            BodySegment.Flags.IsUsingInterpolation = vPrev;
                        }
                    });
                    break;
                }
        }

        void Update()
        {
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.ResetFrame))
            {
                try
                {
                    bool vPrev = BodySegment.Flags.IsUsingInterpolation;
                    BodySegment.Flags.IsUsingInterpolation = false;
                    CurrBody.View.ResetInitialFrame();
                    BodySegment.Flags.IsUsingInterpolation = vPrev;
                }
                catch (Exception)
                {
                }
            }
            else if(Input.GetKeyDown(HeddokoDebugKeyMappings.IsCalibratingROM))
            {
                try
                {
                    BodyFlags vPrev = BodySegment.Flags.clone();
                    BodySegment.Flags.IsCalibrating = true;
                    CurrBody.View.ResetInitialFrame();
                    BodySegment.Flags = vPrev;
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