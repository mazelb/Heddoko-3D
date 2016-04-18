/** 
* @file MainMenuView.cs
* @brief Contains the MainMenuView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews;
using Assets.Scripts.UI.ActivitiesContext.Controller;
using Assets.Scripts.UI.MainMenu.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainMenu.View
{
    /// <summary>
    /// MainMenuView class:  The view of the main menu, directly after the splash Screen
    /// </summary>
    public class MainMenuView : AbstractView
    {

        public Button BrainpackButton;
        public Button ActivitiesButton;
        public Button RecordingsSelectionButton;
        public Camera TrainingAndLearningCam;
        public Camera RecordingSelectionCam;
        public Button ExitButton;
        public Button SettingsButton;
        public MainMenuController MainmenuController;


     
        //The Activities context view controller
        public ActivitiesContextController ActivitiesContext;

        //The Brainpack/Bluetooth connection view
        public MainMenuBrainpackView BrainpackConnectionView; 
        //public RecordingSelectionView RecordingSelectionView;
        public AbstractView RecordingSelectionView; 
        
        /// <summary>
        /// Shows the Main menu view
        /// </summary>
        public void ShowMainMenuContextView()
        {
            BrainpackButton.interactable = true;
            ActivitiesButton.interactable = true;
            gameObject.SetActive(true);
            TrainingAndLearningCam.gameObject.SetActive(false);
            RecordingSelectionCam.gameObject.SetActive(false);

        }

        /// <summary>
        /// hide the Main menu view
        /// </summary>
        public void HideMainMenuContextView()
        {
            BrainpackButton.interactable = false;
            ActivitiesButton.interactable = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Show the Brainpack context view
        /// </summary>
        public void ShowBrainpackContextView()
        {
            BrainpackConnectionView.Show();
            TrainingAndLearningCam.gameObject.SetActive(false);
            RecordingSelectionCam.gameObject.SetActive(false);
        }

        /// <summary>
        /// Hides the Brainpack context view
        /// </summary>
        public void HideBrainpackContextView()
        {
            BrainpackConnectionView.Hide();
        }

        /// <summary>
        /// Shows the activities context view
        /// </summary>
        public void ShowActivitiesContextView()
        {
            ActivitiesContext.SwitchtoMainActivityView();
           // TrainingAndLearningCam.gameObject.SetActive(true);
            RecordingSelectionCam.gameObject.SetActive(false);
        }

        /// <summary>
        /// hides the activities context view
        /// </summary>
        public void HideActivityContextView()
        {
            ActivitiesContext.SetToIdleState();
        }

        /// <summary>
        /// Shows the recording selections view
        /// </summary>
        public void ShowRecordingsSelection()
        {
            RecordingSelectionView.Show();
            TrainingAndLearningCam.gameObject.SetActive(false);
         //   RecordingSelectionCam.gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the recordings selections view
        /// </summary>
        public void HideRecordingsSelection()
        {
           // RecordingSelectionView.Hide();
        }

        public override void CreateDefaultLayout()
        {
             
        }

        public override void Show()
        {
            MainmenuController.ChangeState(MainMenuState.MainMenu);
        }
    }
}
