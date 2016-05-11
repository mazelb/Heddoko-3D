/** 
* @file BodyControlPanel.cs
* @brief Contains the BodyControlPanel  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Body_Pipeline.Analysis.Views;
using Assets.Scripts.Communication.View.Table; 
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
        public Body Body
        {
            get { return mBody; }
            set { mBody = value; }
        }

        void Awake()
        {

            TrunkButton.onClick.AddListener(() =>
            {
                TogglePanel();
                AnalysisTextContainer.ChangeAnalysisView(AnaylsisTextContainer.CurrentAnalysisTextView.Trunk);
            });

            ShoulderButton.onClick.AddListener(() =>
            {
                TogglePanel();
                AnalysisTextContainer.ChangeAnalysisView(AnaylsisTextContainer.CurrentAnalysisTextView.Shoulders);
            });

            ElbowsButton.onClick.AddListener(() =>
            {
                TogglePanel();
                AnalysisTextContainer.ChangeAnalysisView(AnaylsisTextContainer.CurrentAnalysisTextView.Elbows);
            });

            HipsButton.onClick.AddListener(() =>
            {
                TogglePanel();
                AnalysisTextContainer.ChangeAnalysisView(AnaylsisTextContainer.CurrentAnalysisTextView.Hips);
            });

            KneesButton.onClick.AddListener(() =>
            {
                TogglePanel();
                AnalysisTextContainer.ChangeAnalysisView(AnaylsisTextContainer.CurrentAnalysisTextView.Knees);
            });

            SensorDataButton.onClick.AddListener(ToggleSensorPanel);
        }

        /// <summary>
        /// Toggles the information panel
        /// </summary>
        void TogglePanel()
        {
            if (!BodyContentSlider.IsOpen && !DisablePanelShowClicks)
            {
                BodyContentSlider.Toggle();
            }
        }

        void ToggleSensorPanel()
        {
            BodyFrameSlider.Toggle();
            BodyFrameControl.IsPaused = BodyFrameSlider.IsOpen;
        }


    }
}