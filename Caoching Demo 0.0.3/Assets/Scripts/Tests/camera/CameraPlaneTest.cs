/** 
* @file CameraPlaneTest.cs
* @brief Contains the CameraPlaneTest  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.UI.AbstractViews.camera;
using Assets.Scripts.UI.DemoKit;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tests.camera
{
    /// <summary>
    /// Unit test, testing camera view planes
    /// </summary>
    public class CameraPlaneTest: MonoBehaviour
    {
        public AndroidRecordingPlayerView PlayerView;
        public SlideBlock CameraPlaneSlideBlock;
        public Button TogglePanelButton;
        public Button TransverseButton;
        public Button SagittalButton;
        public Button FrontalViewButton;
        private bool mComplete = false;

        void Awake()
        {
            TogglePanelButton.onClick.AddListener(CameraPlaneSlideBlock.Toggle);
        }
        void Update()
        {
            if (!mComplete)
            {
                
            }
            try
            {
                TransverseButton.onClick.RemoveAllListeners();
                TransverseButton.onClick.AddListener(() => PlayerView.RootNode.PanelSettings.CameraToBodyPair.PanelCamera.CamViewPlane.ViewPlane = CameraViewPlane.Transverse);
                SagittalButton.onClick.RemoveAllListeners();
                SagittalButton.onClick.AddListener(() => PlayerView.RootNode.PanelSettings.CameraToBodyPair.PanelCamera.CamViewPlane.ViewPlane = CameraViewPlane.Sagital);
                FrontalViewButton.onClick.RemoveAllListeners();
                FrontalViewButton.onClick.AddListener(() => PlayerView.RootNode.PanelSettings.CameraToBodyPair.PanelCamera.CamViewPlane.ViewPlane = CameraViewPlane.Frontal);
 
            }
            catch
            {
                mComplete = false;
            }
           
        }
    }
}