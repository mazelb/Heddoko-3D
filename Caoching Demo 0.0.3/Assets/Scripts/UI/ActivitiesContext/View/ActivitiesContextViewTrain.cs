
/** 
* @file ActivitiesContextViewTrain.cs
* @brief Contains the ActivitiesContextViewTrain class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using Assets.Scripts.Communication.Controller;
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
    /// Represents the training context view
    /// </summary>
    [Serializable]
    public class ActivitiesContextViewTrain : AbstractInActivityView
    {
        /// <summary>
        /// When the model needs to be shown, it will be placed on this anchor
        /// </summary>
        public Transform Heddoko3DModelEnabledAnchor;
        public Transform Heddoko2DModelEnabledAnchor;
        /// <summary>
        /// When the model needs to be hidden, it will be placed on this anchor
        /// </summary>
        public Transform HeddokoModelHiddenAnchor;
        [SerializeField]
        private Button mBackButton;
        public Camera TrainingAndLearningCam;

        public ActivitiesContextController ActivitiesContextController;

        public SquatMetricsView SquatMetrics;
        public NonSquatWrapperView NonSquatMetrics;


        private PlayerStreamManager mPlayerStreamManager;
        private bool mIsActive { get; set; }
        public Model2D3DSwitch ModelSwitcher;
        public Button RecordButton;
        public override Button Backbutton
        {
            get { return mBackButton; }
        }
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

        void Awake()
        {
            if (RecordButton != null)
            {
                BrainpackConnectionController.Instance.ConnectedStateEvent += () =>
                {
                    RecordButton.interactable = true;
                };
                BrainpackConnectionController.Instance.DisconnectedStateEvent += () =>
                {
                    RecordButton.interactable = false;
                };

                RecordButton.onClick.AddListener(BrainpackConnectionController.Instance.InitiateSuitRecording);
            }
            
        }
        /// <summary>
        /// Enables and shows the view
        /// </summary>
        public override void Show()
        {
            gameObject.SetActive(true);
            ModelSwitcher.TransformInview3DLocation = Heddoko3DModelEnabledAnchor;
            ModelSwitcher.TransformInview2DLocation = Heddoko2DModelEnabledAnchor;
            ModelSwitcher.Show();
            TrainingAndLearningCam.gameObject.SetActive(true);
            PlayerStreamManager.ChangeState(PlayerStreamManager.BodyPlaybackState.StreamingFromBrainPack);
            if (RecordButton != null)
            {
                RecordButton.interactable = BrainpackConnectionController.Instance.ConnectionState == BrainpackConnectionState.Connected;
            }

        }

        public override void CreateDefaultLayout()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Hides the view
        /// </summary>
        public override void Hide()
        {
            SquatMetrics.Hide();
            NonSquatMetrics.Hide();
            ModelSwitcher.Hide();
            TrainingAndLearningCam.gameObject.SetActive(false);
            PlayerStreamManager.Stop();
            PlayerStreamManager.ResetBody();
            gameObject.SetActive(false);
            mIsActive = false;

        }


    }
}
