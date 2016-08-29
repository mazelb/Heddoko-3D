/** 
* @file BodyControlPanel.cs
* @brief Contains the BodyControlPanel  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Body_Pipeline.Analysis.Views;
using Assets.Scripts.Communication.View.Table;
using Assets.Scripts.UI.RecordingLoading;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Settings
{
    /// <summary>
    /// A body control panel containing control buttons to bring up body context relative views
    /// </summary>
    public class BodyControlPanel : MonoBehaviour
    {
        private Body mBody;
        public Button TrunkButton;
        public Button ShoulderButton;
        public Button ElbowsButton;
        public Button HipsButton;
        public Button KneesButton;
        public Button SensorDataButton;
        public SlideBlock BodyContentSlider;
        public SlideBlock BodyFrameSlider;
        public Text InformationPanel;
        public BodyFrameDataControl BodyFrameControl;
        public AnaylsisTextContainer AnalysisTextContainer;
        public bool DisablePanelShowClicks = false;
        public AnalysisContainerToggle Toggler;
        public CloudLocalStorageViewManager CloudLocalStorageViewManager;
        public Body Body
        {
            get { return mBody; }
            set { mBody = value; }
        }

        void Awake()
        {
            ScreenResolutionManager.Instance.NewResolutionSetEvent += BodyContentSlider.ResetPosition;
            ScreenResolutionManager.Instance.NewResolutionSetEvent += BodyFrameSlider.ResetPosition;

            TrunkButton.onClick.AddListener(() =>
            {
                HideCloudLocalStorageViewManager();
                TogglePanel();
                AnalysisTextContainer.ChangeAnalysisView(AnaylsisTextContainer.CurrentAnalysisTextView.Trunk);
                Toggler.OnSwitchToggle("Trunk");
            });


            ShoulderButton.onClick.AddListener(() =>
            {
                HideCloudLocalStorageViewManager();
                TogglePanel();
                AnalysisTextContainer.ChangeAnalysisView(AnaylsisTextContainer.CurrentAnalysisTextView.Shoulders);
                Toggler.OnSwitchToggle("Shoulders");
            });

            ElbowsButton.onClick.AddListener(() =>
            {
                HideCloudLocalStorageViewManager();
                TogglePanel();
                AnalysisTextContainer.ChangeAnalysisView(AnaylsisTextContainer.CurrentAnalysisTextView.Elbows);
                Toggler.OnSwitchToggle("Elbows");
            });

            HipsButton.onClick.AddListener(() =>
            {
                HideCloudLocalStorageViewManager();
                TogglePanel();
                AnalysisTextContainer.ChangeAnalysisView(AnaylsisTextContainer.CurrentAnalysisTextView.Hips);
                Toggler.OnSwitchToggle("Hips");
            });

            KneesButton.onClick.AddListener(() =>
            {
                HideCloudLocalStorageViewManager();
                TogglePanel();
                AnalysisTextContainer.ChangeAnalysisView(AnaylsisTextContainer.CurrentAnalysisTextView.Knees);
                Toggler.OnSwitchToggle("Knees");
            });

            SensorDataButton.onClick.AddListener(ToggleSensorPanel);
        }

        /// <summary>
        /// Toggles the information panel
        /// </summary>
      public  void TogglePanel()
        {
            if (!BodyContentSlider.IsOpen && !DisablePanelShowClicks)
            {
                BodyContentSlider.Toggle();
            }
        }

        public void HideCloudLocalStorageViewManager()
        {
            CloudLocalStorageViewManager.Hide();
        }

        public bool PanelIsVisible()
        {
            return BodyContentSlider.IsOpen;
        }

        public void HidePanel()
        {
            if (BodyContentSlider.IsOpen)
            {
                BodyContentSlider.Toggle();
            }
        }

        /// <summary>
        /// Toggles the sensor panel
        /// </summary>
        void ToggleSensorPanel()
        {
            BodyFrameSlider.Toggle();
            BodyFrameControl.IsPaused = BodyFrameSlider.IsOpen;
        }

        


    }
}