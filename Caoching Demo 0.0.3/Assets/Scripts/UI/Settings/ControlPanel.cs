/** 
* @file ControlPanel.cs
* @brief Contains the ControlPanel  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.Communication.Controller;
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
        public Button LoadRecordingsButton;
        public Button LiveViewButton;
        public Button BrainpackConnectionButton;
        public SlideBlock BrainpackConnectionSlider;
        public Button SettingsButton;
        public Button ExitApplicationButton;
        public Button RenameRecordingButton;
        public BrainpackConnectionController ConnectionController;
        public RecordingPlayerView RecordingPlayerView;
        public LiveSuitFeedView SuitFeedView;
        public RenameRecordingModule RenameRecordingModule;

        public void Awake()
        {
            ExitApplicationButton.onClick.AddListener(QuitApplication);
            RenameRecordingButton.onClick.AddListener(()=>RenameRecordingModule.Toggle());
            RenameRecordingModule.Init(ConnectionController);
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
            });
            LiveViewButton.onClick.AddListener(() =>
            {
                if (ConnectionController.ConnectionState == BrainpackConnectionState.Connected)
                {
                    if (BrainpackConnectionSlider.IsOpen)
                    {
                        BrainpackConnectionSlider.Toggle();
                    }
                }
                if (!SuitFeedView.gameObject.activeInHierarchy)
                {
                    RecordingPlayerView.Hide();
                    SuitFeedView.Show();

                }

            });
            BrainpackConnectionButton.onClick.AddListener(() =>
            {
                BrainpackConnectionSlider.Toggle();
            }
             );
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