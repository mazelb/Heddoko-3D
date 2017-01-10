// /**
// * @file LanControlPanelTest.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 09 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Communication.View;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording.AnaylsisRecording;
using Assets.Scripts.UI.RecordingLoading;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Settings
{
    /// <summary>
    /// Testing panel for scene that uses a lan listener
    /// </summary>
    public class LanControlPanelTest : MonoBehaviour
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
        public GameObject LivePanel;
 
        public RecordingPlayerView RecordingPlayerView;
        public LiveSuitFeedView SuitFeedView;

        public BodyControlPanel BodyControlPanel;
        public CloudLocalStorageViewManager CloudLocalStorageViewManager;
        public void Awake()
        {
            ExitApplicationButton.onClick.AddListener(QuitApplication);
            CloudLocalStorageViewManager.RecordingLoadingCompleteEvent +=
               (vX) => SegmentAnalysisRecordingController.EnableControl();


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
                CloudLocalStorageViewManager.gameObject.SetActive(true);
                CloudLocalStorageViewManager.Show();
                if (!RecordingPlayerView.gameObject.activeInHierarchy)
                {
                    SuitFeedView.Hide();
                    RecordingPlayerView.Show();
               
                }
                LivePanel.SetActive(false);
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
                
                if (ResolutionSettingSlideBlock.IsOpen)
                {
                    ResolutionSettingSlideBlock.Toggle();
                }
                if (!SuitFeedView.gameObject.activeInHierarchy)
                {
                    RecordingPlayerView.Hide();
                    SuitFeedView.Show();

                } 
                LivePanel.SetActive(true);
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
            
            SegmentAnalysisRecordingController.DataCollectionEndedEvent +=
                () => LoadRecordingsButton.interactable = true;
            SegmentAnalysisRecordingController.DataCollectionStartedEvent +=
                () => LoadRecordingsButton.interactable = false;

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