

/* @file AbstractControlPanel.cs
* @brief Contains the ActivitiesContextViewAnalyze class
* @author Mohammed Haider(mohamed @heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Permissions;
using Assets.Scripts.Utils.DebugContext;
using HeddokoSDK.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording
{

    /// <summary>
    /// A abstract sub control that changes a recording's play/pause state
    /// </summary>
    [UserRolePermission]
    public class RecordingPlayPause : AbstractSubControl
    {
        public Button PlayPauseButton;
        public Sprite PlaySprite;
        public Sprite PauseSprite;
        public Sprite StopSprite;
        private bool mIsPaused;

        public PlaybackControlPanel ParentPanel;

        private SubControlType mType = SubControlType.RecordingPlayPause;
        public override SubControlType SubControlType
        {
            get { return mType; }
        }

        /// <summary>
        /// Changes the text of the button
        /// </summary>
        public bool IsPaused
        {
            get { return mIsPaused; }
            set
            { 
                mIsPaused = value;
                ChangePlayPauseGraphics();
            }
        }

        public void Init(PlaybackControlPanel mControlPanel)
        {
            ParentPanel = mControlPanel;
            if (PlayPauseButton != null)
            {
                PlayPauseButton = GetComponent<Button>();
            }
            PlayPauseButton.onClick.AddListener(ParentPanel.SetPlayState);
        }
        /// <summary>
        /// Stops the control from being used
        /// </summary>
        private void Stop()
        {
            PlayPauseButton.image.sprite = StopSprite;


        }

        /// <summary>
        /// Changes the text of the play pause button according to the current pause state
        /// </summary>
        private void ChangePlayPauseGraphics()
        {
            PlayPauseButton.image.sprite = IsPaused?   PlaySprite:PauseSprite;
        }
        public override void Disable()
        {
            Interactable = false;
        }

        public override void Enable()
        {
            Interactable = true;
        }

      


        /// <summary>
        /// Sets the interaction of the Button
        /// </summary>
        public bool Interactable
        {
            get { return PlayPauseButton.interactable; }
            set
            {
                PlayPauseButton.interactable = value;

                if (!value)
                {
                    Stop();
                }

                else
                {
                    ChangePlayPauseGraphics();
                }
            }
        }

        /// <summary>
        /// Request resouces to be set up for the subcontrol
        /// </summary>
        public void RequestResources()
        {
            InputHandler.RegisterKeyboardAction(KeyCode.Space, SetPlayState);
            
        }

        private void SetPlayState()
        { 
            ParentPanel.SetPlayState();
        }
    

        /// <summary>
        /// Releases previously held resources
        /// </summary>
        public void ReleaseResources()
        {
            InputHandler.RemoveKeybinding(KeyCode.RightArrow, SetPlayState);
        }
    }
}
