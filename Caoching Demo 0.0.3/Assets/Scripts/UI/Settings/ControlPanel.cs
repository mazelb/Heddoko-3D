/** 
* @file ControlPanel.cs
* @brief Contains the ControlPanel  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Communication.View;
using Assets.Scripts.MainApp;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording.AnaylsisRecording;
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
        public Button ExitApplicationButton;
        public Button RenameRecordingButton;
        public Button BrainpackConnectionButton;
        public Button ResolutionSettingsButton;
        public LiveConnectionButton LiveViewButton;
        public SlideBlock BrainpackConnectionSlider;
        public SlideBlock ResolutionSettingSlideBlock;
        public SegmentAnalysisRecordingController SegmentAnalysisRecordingController;
        public BrainpackConnectionController ConnectionController;
        public RecordingPlayerView RecordingPlayerView;
        public LiveSuitFeedView SuitFeedView;
        public RenameRecordingModule RenameRecordingModule;
        public BodyControlPanel BodyControlPanel;
        public CloudLocalStorageViewManager CloudLocalStorageViewManager;
        public WindowsApplicationManager AppManager;
        public void Awake()
        {
            ExitApplicationButton.onClick.AddListener(QuitApplication);
            RenameRecordingButton.onClick.AddListener(() => RenameRecordingModule.Toggle());
            CloudLocalStorageViewManager.RecordingLoadingCompleteEvent +=
                (vX) => SegmentAnalysisRecordingController.EnableControl();
           
            RenameRecordingModule.Init(ConnectionController);
            LiveViewButton.Disable();
            LoadRecordingsButton.onClick.AddListener(() =>
            {
                if (BrainpackConnectionSlider.IsOpen)
                {
                    BrainpackConnectionSlider.Toggle();
                    if (BodyControlPanel.PanelIsVisible())
                    {
                        BodyControlPanel.TogglePanel();
                    }
                }
                CloudLocalStorageViewManager.Show();
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
                SegmentAnalysisRecordingController.DisableControl();
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
                CloudLocalStorageViewManager.Hide();
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
                CloudLocalStorageViewManager.Hide();

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
                CloudLocalStorageViewManager.Hide();
            }
             );
            ConnectionController.ConnectedStateEvent += LiveViewButton.Enable;
            ConnectionController.DisconnectedStateEvent += LiveViewButton.Disable;
            SegmentAnalysisRecordingController.DataCollectionEndedEvent +=
                () => LoadRecordingsButton.interactable = true;
            SegmentAnalysisRecordingController.DataCollectionStartedEvent +=
                () => LoadRecordingsButton.interactable = false;
            AppManager.OnLogoutEvent += OnLogout;
            AppManager.OnLoginEvent += InitComponents;
        }

        public void InitComponents()
        {
            CloudLocalStorageViewManager.InitializeComponents();
        }

        void OnLogout()
        {
            CloudLocalStorageViewManager.Clear();
            CloudLocalStorageViewManager.Hide();
            RecordingPlayerView.Hide();
            SuitFeedView.Hide();
            RenameRecordingModule.Hide();
            if (BrainpackConnectionSlider.IsOpen)
            {
                BrainpackConnectionSlider.Toggle();
            }
            if (ResolutionSettingSlideBlock.IsOpen)
            {
                ResolutionSettingSlideBlock.Toggle();
            }
            if (BodyControlPanel.PanelIsVisible())
            {
                BodyControlPanel.HidePanel();
            }
            if (BrainpackConnectionController.Instance.ConnectionState == BrainpackConnectionState.Connected)
            {
                BrainpackConnectionController.Instance.DisconnectBrainpack();
            }
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

        void OnApplicationQuit()
        {
            OnLogout();
        }
    }
}