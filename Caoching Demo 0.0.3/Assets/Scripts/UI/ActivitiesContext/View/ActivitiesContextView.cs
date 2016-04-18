/**  
* @file ActivitiesContextView.cs 
* @brief Contains the ActivitiesContextView class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/
 
using UnityEngine; 

namespace Assets.Scripts.UI.ActivitiesContext.View
{
    public class ActivitiesContextView : MonoBehaviour
    {
        public AbstractInActivityView MainView;
        public IActivitiesContextViewSubcomponent TrainingView;
        public IActivitiesContextViewSubcomponent LearningView;
        public IContextSpecificLearningView LearnFromRecordingView;
        
        /// <summary>
        /// Shows the ActivitiesContextView 
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }
        /// <summary>
        /// Hides the ActivitiesContextView 
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// Switch to main view
        /// </summary>
        public void SwitchToMainView()
        {
            MainView.Show();
        }
        /// <summary>
        /// Switch to idle view(no view)
        /// </summary>
        public void SwitchToIdleView()
        {
            MainView.Hide();
        }

        public void HideMainView()
        {
            MainView.Hide();
        }

        public void SwitchToLearningView()
        {
            LearningView.Show();
        }
        /// <summary>
        /// Hides the learning view
        /// </summary>
        public void HideLearningView()
        {
            LearningView.Hide();
        }
        /// <summary>
        /// Switches to the learning by recording view
        /// </summary>
        public void SwitchToLearnByRecordingView()
        { 
             LearnFromRecordingView.ContextViewSubcomponent.Show();
        }
        /// <summary>
        /// hides the learn by recording view
        /// </summary>
        public void HideLearnByRecordingView()
        {
            LearnFromRecordingView.ContextViewSubcomponent.Hide();
        }
        /// <summary>
        /// Shows the training view
        /// </summary>
        public void SwitchToTrainingView()
        {
            TrainingView.Show();
        }

        /// <summary>
        /// hides the training view
        /// </summary>
        public void HideTrainingView()
        {
            TrainingView.Hide();
        }
    }

}