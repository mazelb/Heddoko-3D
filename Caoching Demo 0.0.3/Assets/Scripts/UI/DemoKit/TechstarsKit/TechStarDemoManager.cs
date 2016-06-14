/**
* @file TechStarDemoManager.cs
* @brief Contains the 
* @author Mohammed Haider( 
* @date May 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using Assets.Scripts.Body_Data.View.Anaylsis;
using Assets.Scripts.UI.AbstractViews.camera;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.UI.DemoKit.TechstarsKit
{
    /// <summary>
    /// Manages events of the tech stars demo
    /// </summary>
    public class TechStarDemoManager : MonoBehaviour
    {
        
        public TechStarsCameraController CameraController;
        private TechStarsDemoRecordingsContainer mRecordingsContainer = new TechStarsDemoRecordingsContainer();
        public TechStarDemoPlayerView TechStarDemoPlayerView;
        public TechStarsDemoInputHandler InputHandler;
        public TechStarsRulaTrunkFlexionScore TrunkFlexionScore;
        public TechStarTrunkFlexionInfo FlexionInfo;
        private Body mBody;
        private PanelCamera mPanelCam;
        private RulaVisualAngleAnalysis mTrunkFlexionExtension;
        private RulaVisualAngleAnalysis mTrunkRotation;
        private DemoPlayer mPlayer; 
        void Awake()
        {
            TechStarDemoPlayerView.ViewUpdatedEvent += Initialize;
        }

        void Start()
        {
            TechStarDemoPlayerView.Init();
           
        }
        /// <summary>
        /// sets the camera and body 
        /// </summary>
        /// <param name="vBody"></param>
        /// <param name="vCam"></param>
        public void Initialize(TechStarDemoPlayerView mView)
        {
            mBody = mView.DefaultBody;
            mPanelCam = mView.PanelCam;
            mPlayer = mView.Player;
            CameraController.Camera = mPanelCam.PanelRenderingCamera;
            mTrunkFlexionExtension = mBody.RenderedBody.GetRulaVisualAngleAnalysis(AnaylsisFeedBackContainer.PosturePosition.TrunkFlexionExtension);
            mTrunkRotation = mBody.RenderedBody.GetRulaVisualAngleAnalysis(AnaylsisFeedBackContainer.PosturePosition.TrunkRotation);
            InputHandler.TrunkFlexionExtension = mTrunkFlexionExtension;
            CameraController.LookAtTarget = mPanelCam.Orbitter.Target;
            mPanelCam.Orbitter.enabled = false;
            mPlayer.DemoBody = mBody; 
             
            InputHandler.RecordingsContainer = mRecordingsContainer;
            InputHandler.Demoplayer = mPlayer;
            mBody.View.BodyFrameUpdatedEvent += BodyUpdated;

        }

        void BodyUpdated(BodyFrame vBodyFrame)
        {
            if (mBody.View.Buffer.Count > 0)
            {
                mTrunkFlexionExtension.IsCountingPoints = true;
                mTrunkRotation.IsCountingPoints = true;
                TrunkFlexionScore.UpdateScore(mTrunkFlexionExtension.Point, mTrunkRotation.Point);
                FlexionInfo.UpdateFlexionText(mTrunkFlexionExtension.SignedAngle);
            }
            else
            {
                mTrunkFlexionExtension.IsCountingPoints = false;
                mTrunkRotation.IsCountingPoints = false;
            }
        }


    }
}