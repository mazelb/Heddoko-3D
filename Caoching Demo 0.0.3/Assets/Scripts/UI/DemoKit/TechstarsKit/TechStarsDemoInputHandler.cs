/**
* @file TechStarsDemoInputHandler.cs
* @brief Contains the 
* @author Mohammed Haider( 
* @date 05 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System.Collections.Generic;
using Assets.Scripts.Body_Data.View.Anaylsis;
using Assets.Scripts.UI.RecordingLoading;
using Assets.Scripts.UI.Settings;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.DemoKit.TechstarsKit
{
    /// <summary>
    /// Tech stars Demo input handler
    /// </summary>
    public class TechStarsDemoInputHandler : MonoBehaviour
    {

        public SlideBlock BottomRebaPanel;
        public SlideBlock InfoSlideBlock;
        public TechStarsCameraController TechStarsCameraController;
        private RulaVisualAngleAnalysis mTrunkFlexionExtension;
        [SerializeField]
        private bool mIsInitialized = false;
        public SlideBlock[] Slideblocks;
        private Dictionary<SlideBlock, bool> mSlideBlockEnabledHistory = new Dictionary<SlideBlock, bool>();
        public TechStarsRulaTrunkFlexionScore TrunkFlexionScore;
        public TechStarTrunkFlexionInfo FlexionInfo;
        public RecordingPlayerView PlayerView;
        public BodyControlPanel BodyControlPanel;
        public TechStarsCameraControllerClosedLoop TechStarsCameraControllerClosedLoop;
 
        public RulaVisualAngleAnalysis TrunkFlexionExtension
        {
            get { return mTrunkFlexionExtension; }
            set { mTrunkFlexionExtension = value; }
        }

        public TechStarsDemoRecordingsContainer RecordingsContainer { get; set; }
        public DemoPlayer Demoplayer { get; set; }

        /// <summary>
        /// set to initialized
        /// </summary>
        public void Initialize()
        {
            if (mIsInitialized)
            {
                return;
            }
            mIsInitialized = true;
            foreach (var slideblock in Slideblocks)
            {
                if (!mSlideBlockEnabledHistory.ContainsKey(slideblock))
                {
                    mSlideBlockEnabledHistory.Add(slideblock, false);
                }
            }
        }

        void Update()
        {
           
            if (!mIsInitialized)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                ShowSlideBlocks();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                HideSlideBlocks();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SwitchToSideView();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SwitchToFrontalView();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                TechStarsCameraControllerClosedLoop.GotoPrevPos();
            }
            if(Input.GetKeyDown(KeyCode.N))
            {
                TechStarsCameraControllerClosedLoop.GoToNextPos();
            }

            if (Input.GetKey(KeyCode.L))
            {
                TechStarsCameraControllerClosedLoop.GoToStartPos();
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                ShowArc();
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                HideArc();
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                ComboShow();
                SwitchToFrontalView();
      

            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                ComboHide();
                SwitchToSideView();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetScores();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ComboShow();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                ComboHide();
            }
            

        }

        /// <summary>
        /// Switches to the right view
        /// </summary>
        private void SwitchToRightView()
        {

            TechStarsCameraController.PathForward = false;
            TechStarsCameraController.PathBackward = true;
        }

        private void ShowArc()
        {
            mTrunkFlexionExtension.StartAnimationProcess();
        }

        private void HideArc()
        {
            mTrunkFlexionExtension.Hide();
        }

        private void ShowSlideBlocks()
        {
            foreach (var vSlider in mSlideBlockEnabledHistory)
            {
                bool vIsActive = vSlider.Key.IsOpen;
                vSlider.Key.IsOpen = vIsActive;
                if (vIsActive)
                {
                    vSlider.Key.Toggle();
                }

            }
            if (!BottomRebaPanel.IsOpen)
            {
                BottomRebaPanel.Toggle();
            }
            if (!InfoSlideBlock.IsOpen)
            {
                InfoSlideBlock.Toggle();
            }

            BodyControlPanel.DisablePanelShowClicks = true;

        }

        private void ResetScores()
        {
            TrunkFlexionScore.ResetScores();
        }

        private void HideSlideBlocks()
        {

            if (BottomRebaPanel.IsOpen)
            {
                BottomRebaPanel.Toggle();
            }
            if (InfoSlideBlock.IsOpen)
            {
                InfoSlideBlock.Toggle();
            }

            BodyControlPanel.DisablePanelShowClicks = false;


        }

        private void SwitchToSideView()
        {
            TechStarsCameraController.PathForward = false;
            TechStarsCameraController.PathBackward = true;
        }

        private void SwitchToFrontalView()
        {
            TechStarsCameraController.PathForward = true;
            TechStarsCameraController.PathBackward = false;
        }

        private void ComboShow()
        {
            ShowSlideBlocks();
            ShowArc();
        }

        private void ComboHide()
        {
            HideSlideBlocks();
            HideArc();
        }

    }
}