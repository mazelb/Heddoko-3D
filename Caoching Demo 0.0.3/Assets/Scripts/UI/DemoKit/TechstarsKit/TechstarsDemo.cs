/**
* @file TechstarsDemo.cs
* @brief Contains the 
* @author Mohammed Haider( 
* @date 05 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using System.Collections;
using Assets.Scripts.Body_Data.View.Anaylsis;
using Assets.Scripts.UI.AbstractViews.camera;
using Assets.Scripts.UI.Settings;
using UnityEngine;

namespace Assets.Scripts.UI.DemoKit.TechstarsKit
{
    public class TechstarsDemo : MonoBehaviour
    {
      public TechStarsCameraController CameraController;
        public TechStarsCameraControllerClosedLoop TechStarsCameraControllerClosedLoop;
        public TechStarsDemoInputHandler InputHandler;
        public TechStarsRulaTrunkFlexionScore TrunkFlexionScore;
        public TechStarTrunkFlexionInfo FlexionInfo;

       
        private PanelCamera mPanelLiveView;
        private RulaVisualAngleAnalysis mTrunkFlexionExtension;
        private RulaVisualAngleAnalysis mTrunkRotation;
        public ControlPanel ControlPanel;

        private Body mBody { get; set; }

        void Awake()
        {
            ControlPanel.RecordingPlayerView.RecordingPlayerViewLayoutCreatedEvent += vRecordingPlayerView =>
            {
                vRecordingPlayerView.PbControlPanel.gameObject.SetActive(false);
            };
        }
        void Start()
        {
            StartCoroutine(StartAfterFrameUpdates(Init));
        }

        IEnumerator StartAfterFrameUpdates(Action vAction)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            if (vAction != null)
            {
                vAction();
            }
        }

        void Init()
        {
            ControlPanel.LoadRecordingsButton.onClick.AddListener(SwitchToRecordingBody);
            ControlPanel.LiveViewButton.Button.onClick.AddListener(SwitchToLiveViewBody);
        }
        void SwitchToRecordingBody()
        {
            if (ControlPanel.RecordingPlayerView.CurrBody == null)
            {
                StartCoroutine(StartAfterFrameUpdates(SwitchToRecordingBody));
            }
            else
            {
                if (mBody != null)
                {
                    mBody.View.BodyFrameUpdatedEvent -= BodyUpdated;
                }
                try
                {
                    mBody = ControlPanel.RecordingPlayerView.CurrBody;
                    mBody.View.BodyFrameUpdatedEvent += BodyUpdated;
                    CameraController.Camera =
                        ControlPanel.RecordingPlayerView.RootNode.PanelSettings.CameraToBodyPair.PanelCamera
                            .PanelRenderingCamera;
                    TechStarsCameraControllerClosedLoop.Camera =ControlPanel.RecordingPlayerView.RootNode.PanelSettings.CameraToBodyPair.PanelCamera
                            .PanelRenderingCamera;
                    SetVisuals();
                }
                catch (NullReferenceException)
                {
                    StartCoroutine(StartAfterFrameUpdates(SwitchToRecordingBody));
                }

            }
        }

        void SwitchToLiveViewBody()
        {
            if (ControlPanel.SuitFeedView.BrainpackBody == null)
            {
                StartCoroutine(StartAfterFrameUpdates(SwitchToLiveViewBody));
            }
            else
            {
                if (mBody != null)
                {
                    mBody.View.BodyFrameUpdatedEvent -= BodyUpdated;
                }
                try
                {
                    mBody = ControlPanel.SuitFeedView.BrainpackBody;
                    mBody.View.BodyFrameUpdatedEvent += BodyUpdated;
                    CameraController.Camera =
                        ControlPanel.SuitFeedView.RootNode.PanelSettings.CameraToBodyPair.PanelCamera
                            .PanelRenderingCamera;
                    TechStarsCameraControllerClosedLoop.Camera = ControlPanel.SuitFeedView.RootNode.PanelSettings.CameraToBodyPair.PanelCamera
                     .PanelRenderingCamera;
                    SetVisuals();
                }
                catch (NullReferenceException)
                {
                    StartCoroutine(StartAfterFrameUpdates(SwitchToLiveViewBody));
                }

            }
        }

        void SetVisuals()
        {
             
            mTrunkFlexionExtension = mBody.RenderedBody.GetRulaVisualAngleAnalysis(AnaylsisFeedBackContainer.PosturePosition.TrunkFlexionExtension);
            mTrunkRotation = mBody.RenderedBody.GetRulaVisualAngleAnalysis(AnaylsisFeedBackContainer.PosturePosition.TrunkRotation);
            InputHandler.TrunkFlexionExtension = mTrunkFlexionExtension;
            CameraController.LookAtTarget = mBody.RenderedBody.Hips;
            TechStarsCameraControllerClosedLoop.LookAtTarget = mBody.RenderedBody.Hips;
            InputHandler.Initialize();
            mTrunkRotation.DisableGraphics();
            mTrunkFlexionExtension.DisableGraphics();
        }
        void BodyUpdated(BodyFrame vBodyFrame)
        {
            if (mTrunkFlexionExtension != null && mTrunkRotation != null)
            {
                mTrunkFlexionExtension.IsCountingPoints = true;
                mTrunkRotation.IsCountingPoints = true;
                TrunkFlexionScore.UpdateScore(mTrunkFlexionExtension.Point, mTrunkRotation.Point);
                mTrunkRotation.EnableCalculation();
                mTrunkRotation.DisableGraphics();
                mTrunkFlexionExtension.EnableCalculation();
                FlexionInfo.UpdateFlexionText(mBody.TorsoAnalysis.SignedTorsoFlexion);
            }

        }
    }
}