/**  
* @file BodyVisibilityPanel.cs 
* @brief Contains the BodyVisibilityPanel class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved 
*/

using System;
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.UI.DemoKit;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tests.visuals
{
    /// <summary>
    /// Test the body visibility
    /// </summary>
    public class BodyVisibilityPanel : MonoBehaviour
    {
        public Button TogglePanelButton;
        public Button ResetVisibilityButton;

        public AndroidRecordingPlayerView PlayerView;
        private bool mIsInitialized = false;
        public SlideBlock SlideBlock;

        void Awake()
        {
            TogglePanelButton.onClick.AddListener(SlideBlock.Toggle);
        }

        void Update()
        {
            if (!mIsInitialized)
            {
                try
                {
                    Body vBody = PlayerView.RootBody;
                    ResetVisibilityButton.onClick.AddListener(vBody.RenderedBody.ResetVisiblity);
                    mIsInitialized = true;
                }
                catch
                {

                    mIsInitialized = false;
                }
            }
        }
    }
}