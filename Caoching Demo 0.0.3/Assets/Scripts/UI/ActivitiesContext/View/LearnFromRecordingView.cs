
/** 
* @file LearnFromRecordingView.cs
* @brief Contains the LearnFromRecordingView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.UI.ActivitiesContext.Controller;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.UI.Metrics;
using Assets.Scripts.UI.Metrics.View;
using Assets.Scripts.UI.RecordingLoading;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ActivitiesContext.View
{
    /// <summary>
    /// Represents the learn from recording view
    /// </summary>
    public class LearnFromRecordingView : MonoBehaviour, IActivitiesContextViewSubcomponent, IContextSpecificLearningView
    {
        /// <summary>
        /// When the model needs to be shown, it will be placed on this anchor
        /// </summary>
        public Transform HeddokoModel3DEnabledAnchor;
        public Transform HeddokoModel2DEnabledAnchor;
       
        /// <summary>
        /// When the model needs to be hidden, it will be placed on this anchor
        /// </summary>
        public Transform HeddokoModelHiddenAnchor;

        public Camera TrainingAndLearningCam;
     
        public Button TrainButton;
        [SerializeField]
        private  Button mBackButton;

        public IActivitiesContextViewSubcomponent ContextViewSubcomponent { get; set; }
        public Button ContextSpecificButton { get; set; }


        private PlayerStreamManager mPlayerStreamManager;
        public Model2D3DSwitch ModelSwitcher;
        public ActivitiesContextController ActivitiesContextController;
        public SquatMetricsView SquatsMetrics;
        public NonSquatWrapperView NonSquatMetrics;
        // public GameObject SquatsMetrics;
        //public GameObject BikingMetrics;

        public PlayerStreamManager PlayerStreamManager
        {
            get
            {
                if (mPlayerStreamManager == null)
                {
                    mPlayerStreamManager = FindObjectOfType<PlayerStreamManager>();
                }
                return mPlayerStreamManager;
            }
        }

        public Button Backbutton
        {
            get { return mBackButton; }
        }
        /// <summary>
        /// Enables and displays the view
        /// </summary>
        public void Show()
        {

            ModelSwitcher.TransformInview3DLocation = HeddokoModel3DEnabledAnchor;
            ModelSwitcher.TransformInview2DLocation = HeddokoModel2DEnabledAnchor;
            gameObject.SetActive(true);
            ModelSwitcher.Show();
            TrainingAndLearningCam.gameObject.SetActive(true);

            if (ActivitiesContextController.UsingSquats)
            {
                NonSquatMetrics.Hide();
                SquatsMetrics.Show();
            }

            else
            {
                SquatsMetrics.Hide();
                NonSquatMetrics.Show();
            }

            PlayerStreamManager.ResetBody(); 
        }

        /// <summary>
        /// Hides the view
        /// </summary>
        public void Hide()
        {
            Application.targetFrameRate = -1;
            gameObject.SetActive(false);
            PlayerStreamManager.Stop();
            PlayerStreamManager.ResetBody();
            ModelSwitcher.Hide();
            TrainingAndLearningCam.gameObject.SetActive(false);
            SquatsMetrics.Hide();
            NonSquatMetrics.Hide();
        }


        
    }
}
