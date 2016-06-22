/** 
* @file ControlPanel.cs
* @brief Contains the ControlPanel  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Communication.View;
using Assets.Scripts.UI.RecordingLoading;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Settings
{

    /// <summary>
    /// Control Panel in a view
    /// </summary>
    public class ControlPanel : MonoBehaviour
    {
        public ColorBlock SelectedColorBlock;
        public ColorBlock UnSelectedColorBlock;
        public Button LoadRecordingsButton;
        public LiveConnectionButton LiveViewButton;
        public Button BrainpackConnectionButton;
        public SlideBlock BrainpackConnectionSlider;
        public Button ResolutionSettingsButton;
        public SlideBlock ResolutionSettingSlideBlock;
        public Button ExitApplicationButton;
        public Button RenameRecordingButton;
        public BrainpackConnectionController ConnectionController;
        public RecordingPlayerView RecordingPlayerView;
        public LiveSuitFeedView SuitFeedView;
        public RenameRecordingModule RenameRecordingModule;

        public void Awake()
        {
            ExitApplicationButton.onClick.AddListener(QuitApplication);
            RenameRecordingButton.onClick.AddListener(() => RenameRecordingModule.Toggle());
            RenameRecordingModule.Init(ConnectionController);
            LiveViewButton.Disable();
            LoadRecordingsButton.onClick.AddListener(() =>
            {
                if (BrainpackConnectionSlider.IsOpen)
                {
                    BrainpackConnectionSlider.Toggle();
                }
                if (!RecordingPlayerView.gameObject.activeInHierarchy)
                {
                    SuitFeedView.Hide();
                    RecordingPlayerView.Show();
                }

                LoadRecordingsButton.colors = SelectedColorBlock;
                LiveViewButton.Button.colors = UnSelectedColorBlock;
                BrainpackConnectionButton.colors = UnSelectedColorBlock;
                if (ResolutionSettingSlideBlock.IsOpen)
                {
                    ResolutionSettingSlideBlock.Toggle();
                }
            });
            ResolutionSettingsButton.onClick.AddListener(() =>
            {
                if (!ResolutionSettingSlideBlock.IsOpen)
                {
                    if (BrainpackConnectionSlider.IsOpen)
                    {
                        BrainpackConnectionSlider.Toggle();
                    }
                }
                ResolutionSettingSlideBlock.Toggle();
            }

            );
            LiveViewButton.Button.onClick.AddListener(() =>
            {
                if (ConnectionController.ConnectionState == BrainpackConnectionState.Connected)
                {
                    if (BrainpackConnectionSlider.IsOpen)
                    {
                        BrainpackConnectionSlider.Toggle();
                    }
                }
                if (ResolutionSettingSlideBlock.IsOpen)
                {
                    ResolutionSettingSlideBlock.Toggle();
                }
                if (!SuitFeedView.gameObject.activeInHierarchy)
                {
                    RecordingPlayerView.Hide();
                    SuitFeedView.Show();

                }
                LiveViewButton.Button.colors = SelectedColorBlock;
                LoadRecordingsButton.colors = UnSelectedColorBlock;
                BrainpackConnectionButton.colors = UnSelectedColorBlock;

            });
            BrainpackConnectionButton.onClick.AddListener(() =>
            {
                BrainpackConnectionSlider.Toggle();
                BrainpackConnectionButton.colors = SelectedColorBlock;
                LiveViewButton.Button.colors = UnSelectedColorBlock;
                LoadRecordingsButton.colors = UnSelectedColorBlock;
                if (RecordingPlayerView.gameObject.activeInHierarchy)
                {
                    RecordingPlayerView.Hide();
                }
                if (ResolutionSettingSlideBlock.IsOpen)
                {
                    ResolutionSettingSlideBlock.Toggle();
                }
            }
             );
            ConnectionController.ConnectedStateEvent += LiveViewButton.Enable;
            ConnectionController.DisconnectedStateEvent += LiveViewButton.Disable;
        }


        public void HideAll()
        {
            RecordingPlayerView.gameObject.SetActive(false);
            SuitFeedView.gameObject.SetActive(false);
        }

        private void QuitApplication()
        {
            Application.Quit();
        }
    }
}