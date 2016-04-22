/**  
* @file ActivitiesContextController.cs 
* @brief Contains the ActivitiesContextController class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/

using System.Collections.Generic; 
using Assets.Scripts.UI.ActivitiesContext.View;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.UI.MainMenu.Controller; 
using Assets.Scripts.Utils.DebugContext;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ActivitiesContext.Controller
{
    /** 
    * ActivitiesContextController class  
    * @brief ActivitiesContextController: the controller for the activities context view
    * @note Note something of interest 
    */

    public class ActivitiesContextController : MonoBehaviour
    {
        //current view state 
        public ActivitiesContextViewState CurrentState = ActivitiesContextViewState.Idle;
        public ActivitiesContextView ActivitesContextView;
        public MainMenuController MainMenuController;
        public PlayerStreamManager PlayerStreamManager;
        private Dictionary<Button, IContextSpecificLearningView> ContextSpecificMap = new Dictionary<Button, IContextSpecificLearningView>();
       // public IActivitiesContextViewSubcomponent 

        public string SquatRecordingSubPath;
        public string BikeRecordingSubPath;
        private string ActivityTypeSubPath;

        [SerializeField] private bool mGoToRecordingInstead = false;

        public bool UsingSquats { get; set; }

        /// <summary>
        /// On Awake set up the buttons
        /// </summary>
        private void Awake()
        {
            ActivitesContextView.MainView.Backbutton.onClick.AddListener(ReturnToMainMenu);
 
        }

        /// <summary>
        /// hooks this function into the learning squat button
        /// </summary>
        private void SquatHookFunction()
        { 
            ActivityTypeSubPath = SquatRecordingSubPath;
            PlayerStreamManager.RequestRecordingForPlayback(ActivityTypeSubPath);
            //BodySelectedInfo.Instance.UpdateSelectedRecording(ActivityTypeSubPath);
            UsingSquats = true;
            SwitchToLearnByRecordingState();
        }

        public void NonSquatHookFunction()
        {
            ActivityTypeSubPath = BikeRecordingSubPath;
            PlayerStreamManager.RequestRecordingForPlayback(ActivityTypeSubPath);
          // BodySelectedInfo.Instance.UpdateSelectedRecording(ActivityTypeSubPath);dafda
            UsingSquats = false;
            SwitchToLearnByRecordingState();
        }


        /// <summary>
        /// changes states to Learn
        /// </summary>
        public void SwitchToLearnByRecordingState()
        {
            ChangeState(ActivitiesContextViewState.LearnByRecording);
        }

        /// <summary>
        /// Changes the state of the view
        /// </summary>
        /// <param name="vNewState"></param>
        private void ChangeState(ActivitiesContextViewState vNewState)
        {
            switch (CurrentState)
            {
                case (ActivitiesContextViewState.Idle):
                    {
                        if (vNewState == ActivitiesContextViewState.Main)
                        {
                            CurrentState = ActivitiesContextViewState.Main;
                            ActivitesContextView.Show();
                            ActivitesContextView.SwitchToMainView();  
                        }
                    }
                    break;
                case (ActivitiesContextViewState.Main):
                    {
                        if (vNewState == ActivitiesContextViewState.Idle)
                        {
                            CurrentState = ActivitiesContextViewState.Idle;
                            ActivitesContextView.SwitchToIdleView();
                            ActivitesContextView.Hide();
                            MainMenuController.SwitchToMainMenu();
                            break;
                        }
                        if (vNewState == ActivitiesContextViewState.Learn)
                        {
                            CurrentState = ActivitiesContextViewState.Learn;
                            ActivitesContextView.HideMainView();
                            ActivitesContextView.SwitchToLearningView();
                            break;
                        }
                    }
                    break;
                case (ActivitiesContextViewState.Learn):
                    {
                        if (vNewState == ActivitiesContextViewState.LearnByRecording)
                        {
                            CurrentState = ActivitiesContextViewState.LearnByRecording;
                            ActivitesContextView.HideLearningView();
                            ActivitesContextView.SwitchToLearnByRecordingView();
                            break;
                        }
                        if (vNewState == ActivitiesContextViewState.Main)
                        {
                            CurrentState = ActivitiesContextViewState.Main;
                            ActivitesContextView.HideLearningView();
                            ActivitesContextView.Show();
                            ActivitesContextView.SwitchToMainView();
                        }
                        if (vNewState == ActivitiesContextViewState.Train)
                        {
                            CurrentState = ActivitiesContextViewState.Train;
                            ActivitesContextView.HideLearningView();
                            ActivitesContextView.HideLearnByRecordingView();
                            ActivitesContextView.SwitchToTrainingView();
                            PlayerStreamManager.SetBodytoStreamFromBrainpack();

                            break;
                        }
                        break;
                    }

                case (ActivitiesContextViewState.LearnByRecording):
                    {
                        if (vNewState == ActivitiesContextViewState.Learn)
                        {
                            CurrentState = ActivitiesContextViewState.Learn;
                            ActivitesContextView.HideLearnByRecordingView();
                            ActivitesContextView.SwitchToLearningView();
                            break;
                        }
                        if (vNewState == ActivitiesContextViewState.Train)
                        {
                            CurrentState = ActivitiesContextViewState.Train;
                            ActivitesContextView.HideLearnByRecordingView();
                            ActivitesContextView.SwitchToTrainingView();
                            PlayerStreamManager.SetBodytoStreamFromBrainpack();
                            break;
                        }

                        break;
                    }

                case (ActivitiesContextViewState.Train):
                    {
                        if (vNewState == ActivitiesContextViewState.LearnByRecording)
                        {
                            CurrentState = ActivitiesContextViewState.LearnByRecording;
                            ActivitesContextView.HideTrainingView();
                            ActivitesContextView.SwitchToLearnByRecordingView();
                            PlayerStreamManager.SetBodyToStreamFromRecording();
                        }
                        if (vNewState == ActivitiesContextViewState.Learn)
                        {
                            CurrentState = ActivitiesContextViewState.Learn;
                            ActivitesContextView.HideTrainingView();
                            ActivitesContextView.SwitchToLearningView();
                            break;
                        }
                        break;
                    }
            }
        }




        /// <summary>
        /// Switch to learning view state
        /// </summary>
        public void SwitchToLearningViewState()
        {
            ChangeState(ActivitiesContextViewState.Learn);
        }

        /// <summary>
        /// Switches to the training view state
        /// </summary>
        public void SwitchtoTrainingViewState()
        {
            ChangeState(ActivitiesContextViewState.Train);
        }
        /// <summary>
        /// Change the current state to main
        /// </summary>
        public void SwitchtoMainActivityView()
        {
            ChangeState(ActivitiesContextViewState.Main);
        }
        /// <summary>
        /// Return to the main menu
        /// </summary>
        public void ReturnToMainMenu()
        {

            ChangeState(ActivitiesContextViewState.Idle);
        }
        /// <summary>
        /// Change the current state idle
        /// </summary>
        public void SetToIdleState()
        {
            if (CurrentState != ActivitiesContextViewState.Idle)
            {
                ChangeState(ActivitiesContextViewState.Idle);
            }
        }
        public enum ActivitiesContextViewState
        {
            Idle,
            Main,
            Learn,
            LearnByRecording,
            Train
        }

        void Update()
        {
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.SwitchToRecordingFromLive) && CurrentState  == ActivitiesContextViewState.Learn)
            {
                mGoToRecordingInstead = true;
            }
        }

      

        
        
    }
}
