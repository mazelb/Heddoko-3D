/** 
* @file PlaybackControl.cs
* @brief Contains the PlaybackControl  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System.Collections;
using System.Collections.Generic;
using System;
using Assets.Scripts.Frames_Pipeline;
using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.Localization;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;
using Assets.Scripts.UI.AbstractViews.Permissions;
using Assets.Scripts.UI.ModalWindow;
using Assets.Scripts.Utils;
using HeddokoSDK.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording
{

    public delegate void BodyUpdated(Body vBody);
    public delegate void RecordingUpdated(PlaybackControlPanel vControlPanel);
    public delegate void NewRecordingSelected();

    /// <summary>
    /// Provides controls for recording play back
    /// </summary>
    [UserRolePermission()]
    public class PlaybackControlPanel : AbstractControlPanel, IPermissionLevelContractor
    {
        private RecordingPlaybackTask mPlaybackTask;
        public event FinalFramePositionReached FinalFramePositionEvent;
        private Body mBody;
        public RecordingProgressSubControl RecordingProgressSliderSubControl;
        public RecordingForwardSubControl RecordingForwardSubControl;
        public RecordingRewindSubControl RecordingRewindSubControl;
        public RecordingPlaybackSpeedDisplay RecordingPlaybackSpeedDisplay;
        public RecordingPlayPause PlayPauseSubControls;
        public RecordingPlaySpeedModSubControl PlaybackSpeedModifierSubControl;
        public RecordingProgressSubControl PlaybackProgressSubControl;
        public RecordingIndexValue RecordingIndexValue;
        public LoadSingleRecordingSubControl SingleRecordingLoadSubControl;
        public NewRecordingSelected NewRecordingSelectedEvent;
        public event BodyUpdated BodyUpdatedEvent;
        public event RecordingUpdated RecordingUpdatedEvent;
        public Text CurrentRecordingInfo;
        private bool mIsNewRecording = true;
        private List<AbstractSubControl> mAbstractSubControls = new List<AbstractSubControl>();
        [SerializeField]
        private PlaybackState mCurrentState;
        private PlaybackState mPrevState;
        private PlaybackSettings mPlaybackSettings = new PlaybackSettings();
        private bool mCanUse = true;
        private bool mPreviousBodyLerpVal;
        public event Action<RecordingPlaybackTask> RecordingPlaybackTaskUpdated;
        
        public PlaybackState CurrentState
        {
            get
            {
                return mCurrentState;
            }
            set { mCurrentState = value; }
        }
        /// <summary>
        /// Sets the playback speed. Get;Set;
        /// </summary>
        public float PlaybackSpeed
        {
            get
            {
                return 1f;
            }
            set
            {
                if (mPlaybackSettings.PlaybackSpeed.Equals(0))
                {
                    if (mPlaybackTask != null)
                    {
                        mPlaybackTask.IsPaused = true;
                    }
                }
                mPlaybackSettings.PlaybackSpeed = value;
            }
        }


        public RecordingPlaybackTask PlaybackTask
        {
            get { return mPlaybackTask; }
        }

        /// <summary>
        /// Updates recording for the current playback panel
        /// </summary>
        public void UpdateRecording(RecordingPlaybackTask vPlaybackTask)
        {
            if (mPlaybackTask != null)
            {
                mPlaybackTask.RemoveFinalFrameEventHandler(InvokeFinalFrameReachedEvent);
            }
            if (NewRecordingSelectedEvent != null)
            {
                NewRecordingSelectedEvent();
            }
            if (vPlaybackTask != null)
            {
                mPlaybackTask = vPlaybackTask;
                mPlaybackTask.AttachFinalFrameEventHandler(InvokeFinalFrameReachedEvent);
                foreach (AbstractSubControl vAbsSubCtrl in mSubControls)
                {
                    vAbsSubCtrl.Enable();
                }
                mPlaybackTask.LoopPlaybackEnabled = mPlaybackSettings.IsLooping;
                PlaybackProgressSubControl.UpdateMaxTimeAndMaxValue(mPlaybackTask.RecordingCount,
 mPlaybackTask.TotalRecordingTime);
                //reset sliders positions
                PlaybackProgressSubControl.UpdateCurrentTime(0);
                //set the playback task's current frame to 0
                PlaybackTask.PlayFromIndex(0);
                PlaybackTask.PlaybackSpeed = 1;
                PlaybackSpeedModifierSubControl.PlaybackSpeedSlider.value = 1;
                if (RecordingUpdatedEvent != null)
                {
                    RecordingUpdatedEvent(this);
                }
                ChangeState(PlaybackState.Play);
            }
        }

        public override ControlPanelType PanelType
        {
            get { return ControlPanelType.RecordingPlaybackControlPanel; }
        }

        public PlaybackSettings PlaybackSettings
        {
            get { return mPlaybackSettings; }
        }

        /// <summary>
        /// initialize internal paramaters
        /// </summary>
        /// <param name="vParent">the parent rect transform</param>
        /// <param name="vParentNode">the parent panel node</param>
        public override void Init(RectTransform vParent, PanelNode vParentNode)
        {
            base.Init(vParent, vParentNode);
            mSubControls.Add(RecordingProgressSliderSubControl);
            mSubControls.Add(RecordingForwardSubControl);
            mSubControls.Add(RecordingRewindSubControl);
            mSubControls.Add(PlayPauseSubControls);
            mSubControls.Add(PlaybackSpeedModifierSubControl);
            mSubControls.Add(PlaybackProgressSubControl);
            mSubControls.Add(SingleRecordingLoadSubControl);
            RecordingProgressSliderSubControl.Init(this);
            RecordingForwardSubControl.Init(this);
            RecordingRewindSubControl.Init(this);
            PlayPauseSubControls.Init(this);
            PlaybackSpeedModifierSubControl.Init(this);
            PlaybackProgressSubControl.Init(this);
            SingleRecordingLoadSubControl.Init(this);
            ValidatePlaybackTaskForControls();
            mAbstractSubControls.Add(RecordingProgressSliderSubControl);
            mAbstractSubControls.Add(RecordingForwardSubControl);
            mAbstractSubControls.Add(RecordingRewindSubControl);
            mAbstractSubControls.Add(RecordingPlaybackSpeedDisplay);
            mAbstractSubControls.Add(PlayPauseSubControls);
            mAbstractSubControls.Add(PlaybackSpeedModifierSubControl);
            mAbstractSubControls.Add(PlaybackProgressSubControl);
            mAbstractSubControls.Add(SingleRecordingLoadSubControl);
        }

        public void SkipFrameBack()
        {

        }
        public void SkipFrameFwd()
        {

        }
        void OnApplicationQuit()
        {
            PlayPauseSubControls.PlayPauseButton.onClick.RemoveAllListeners();
        }
        /// <summary>
        /// Changes the current state of the player
        /// </summary>
        /// <param name="vNewState"></param>
        public void ChangeState(PlaybackState vNewState)
        {
            if (!ValidateStateChange(vNewState))
            {
                return;
            }

            int vCurrIdx = mPlaybackTask.GetCurrentPlaybackIndex;
            switch (vNewState)
            {
                case PlaybackState.Pause:
                    mPrevState = mCurrentState;
                    mPlaybackTask.IsPaused = true;
                    OnPause();
                    break;

                case PlaybackState.Play:
                    mPlaybackTask.PlaybackSpeed = mPlaybackSettings.CurrForwardSpeed = 1f;
                    mPlaybackTask.IsPaused = false;
                    OnPlay();
                    break;
                case PlaybackState.FastBackward:
                    mPlaybackSettings.CurrBackSpeed--;
                    if (mPlaybackSettings.CurrBackSpeed < mPlaybackSettings.MaxBackSpeed)
                    {
                        mPlaybackSettings.CurrBackSpeed = -1f;
                    }
                    mPlaybackTask.PlaybackSpeed = mPlaybackSettings.CurrBackSpeed;
                    RecordingPlaybackSpeedDisplay.UpdateSpeedText(mPlaybackTask.PlaybackSpeed);
                    mPlaybackTask.IsPaused = false;
                    PlaybackSpeedModifierSubControl.IsInteractable = false;
                    break;
                case PlaybackState.FastForward:
                    mPlaybackSettings.CurrForwardSpeed++;
                    if (mPlaybackSettings.CurrForwardSpeed > mPlaybackSettings.MaxForwardSpeed)
                    {
                        mPlaybackSettings.CurrForwardSpeed = mPlaybackSettings.MaxForwardSpeed;
                    }
                    mPlaybackTask.PlaybackSpeed = mPlaybackSettings.CurrForwardSpeed;
                    mPlaybackTask.IsPaused = false;
                    RecordingPlaybackSpeedDisplay.UpdateSpeedText(mPlaybackTask.PlaybackSpeed);
                    PlaybackSpeedModifierSubControl.IsInteractable = false;
                    break;
                case PlaybackState.StepBackward:
                    vCurrIdx -= PlaybackSettings.FrameSkip;
                    mPlaybackTask.PlayFromIndex(vCurrIdx);
                    mPlaybackTask.IsPaused = true;
                    vNewState = PlaybackState.Pause;
                    PlaybackSpeedModifierSubControl.IsInteractable = false;
                    PlaybackProgressSubControl.UpdateCurrentTime(vCurrIdx);
                    RecordingIndexValue.SetIndexValue(vCurrIdx);
                    OnPause();
                    break;
                case PlaybackState.StepForward:
                    vCurrIdx += PlaybackSettings.FrameSkip;
                    PlaybackProgressSubControl.UpdateCurrentTime(vCurrIdx);
                    RecordingIndexValue.SetIndexValue(vCurrIdx);
                    mPlaybackTask.PlayFromIndex(vCurrIdx);
                    mPlaybackTask.IsPaused = true;
                    vNewState = PlaybackState.Pause;
                    PlaybackSpeedModifierSubControl.IsInteractable = false;
                    OnPause();
                    break;
                case PlaybackState.SlowMotionForward:
                    mPlaybackTask.PlaybackSpeed = mPlaybackSettings.CurrForwardSpeed;
                    mPlaybackTask.IsPaused = false;
                    break;
                case PlaybackState.Null:
                    //stop recording
                    break;
            }
            mCurrentState = vNewState;
        }
        /// <summary>
        /// checks the validity of the new state compared to the current state and checks if the transition is valid
        /// </summary>
        /// <param name="vNewState">new state to change to </param>
        /// <returns>valid state change</returns>
        private bool ValidateStateChange(PlaybackState vNewState)
        {
            if (!ValidatePlaybackTaskForControls())
            {
                return false;
            }
            if (vNewState == PlaybackState.Null)
            {
                return true;
            }

            switch (CurrentState)
            {
                case PlaybackState.Null:
                    if (vNewState == PlaybackState.Null
                        || vNewState == PlaybackState.Pause
                        || vNewState == PlaybackState.Play
                        )
                    {
                        return true;
                    }
                    break;
                case PlaybackState.Play:
                    if (vNewState == PlaybackState.Null
                        || vNewState == PlaybackState.FastBackward
                        || vNewState == PlaybackState.FastForward
                        || vNewState == PlaybackState.Pause
                        || vNewState == PlaybackState.SlowMotionForward
                        )
                    {
                        return true;
                    }
                    break;
                case PlaybackState.FastForward:
                    if (vNewState == PlaybackState.FastBackward
                        || vNewState == PlaybackState.FastForward
                        || vNewState == PlaybackState.Play
                        || vNewState == PlaybackState.Pause
                        )
                    {
                        return true;
                    }

                    break;
                case PlaybackState.FastBackward:
                    if (vNewState == PlaybackState.Pause
                         || vNewState == PlaybackState.FastBackward
                         || vNewState == PlaybackState.FastForward
                         || vNewState == PlaybackState.Play
                         )
                    {
                        return true;
                    }
                    break;

                case PlaybackState.Pause:
                    if (vNewState == PlaybackState.FastBackward
                         || vNewState == PlaybackState.FastForward
                        || vNewState == PlaybackState.StepBackward
                        || vNewState == PlaybackState.StepForward
                        || vNewState == PlaybackState.Play
                         || vNewState == PlaybackState.SlowMotionForward
                        )

                    {
                        return true;
                    }
                    break;

                case PlaybackState.StepBackward:
                    if (vNewState == PlaybackState.Pause)
                    {
                        return true;
                    }
                    break;

                case PlaybackState.StepForward:
                    if (vNewState == PlaybackState.Pause)
                    {
                        return true;
                    }

                    break;

                case PlaybackState.SlowMotionForward:
                    if (vNewState == PlaybackState.Play
                         || vNewState == PlaybackState.Pause)
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }


        /// <summary>
        /// Plays or pauses the recording
        /// </summary>
        public void SetPlayState()
        {
            bool vIsPaused = mPlaybackTask.IsPaused;
            if (vIsPaused)
            {
                ChangeState(PlaybackState.Play);
            }
            else
            {
                ChangeState(PlaybackState.Pause);
            }
        }

        /// <summary>
        /// Verifies if the current playback task is in a valid state. Otherwise, disable playback controls until it is
        /// </summary>
        public bool ValidatePlaybackTaskForControls()
        {

            if (mPlaybackTask == null)
            {

                foreach (var vSubControl in mSubControls)
                {
                    vSubControl.Disable();
                }
                return false;
            }
            return true;
        }
        /// <summary>
        /// Pauses recording
        /// </summary>
        public void OnPause()
        {
            mPlaybackTask.IsPaused = true;
            RecordingForwardSubControl.IsPaused = true;
            RecordingRewindSubControl.IsPaused = true;
            RecordingPlaybackSpeedDisplay.IsPaused = true;
            PlayPauseSubControls.IsPaused = true;
            PlaybackSpeedModifierSubControl.IsPaused = true;
            PlaybackSpeedModifierSubControl.IsInteractable = false;
            BodySegment.IsUsingInterpolation = false;
        }



        public void OnPlay()
        {
            mPlaybackTask.IsPaused = false;
            RecordingForwardSubControl.IsPaused = false;
            RecordingRewindSubControl.IsPaused = false;
            RecordingPlaybackSpeedDisplay.IsPaused = false;
            PlayPauseSubControls.IsPaused = false;
            RecordingPlaybackSpeedDisplay.UpdateSpeedText(mPlaybackTask.PlaybackSpeed);
            PlaybackSpeedModifierSubControl.IsPaused = false;
            PlaybackSpeedModifierSubControl.IsInteractable = true;
            PlaybackSpeedModifierSubControl.UpdateCurrentPlaybackSpeed(1f);
            BodySegment.IsUsingInterpolation = mPreviousBodyLerpVal;
        }

        void BodyFrameUpdateHandler(BodyFrame vFrame)
        {
            RecordingProgressSliderSubControl.UpdateCurrentTime(vFrame.Index);
            RecordingIndexValue.SetIndexValue(vFrame.Index);
        }



        public void Update()
        {
            //update the RecordingProgressSliderSubControl slider value
            if (mPlaybackTask != null)
            {
                //check if the conversion is completed
                if (mPlaybackTask.ConversionCompleted)
                {
                    //check if this a new recording that was loaded. 
                    if (mIsNewRecording)
                    {
                        //todo: put this in an event!!!
                        mIsNewRecording = false;
                        UpdateRecording(mBody.MBodyFrameThread.PlaybackTask);
                    }
                }
            }
        }



        /// <summary>
        /// Get a timestamp for the frame 
        /// </summary>
        /// <param name="vIndex"></param>
        /// <returns></returns>
        public float GetTimeStampFromFrameIdx(int vIndex)
        {
            return mPlaybackTask.GetBodyFrameAtIndex(vIndex).Timestamp;

        }

        /// <summary>
        /// Sets the play position at index
        /// </summary>
        /// <param name="vIndex"></param>
        public void SetPlayPositionAt(int vIndex)
        {
            mPlaybackTask.PlayFromIndex(vIndex);
        }

        /// <summary>
        /// set the slo mo speed. Only avaiable when in a playback state
        /// </summary>
        /// <param name="vNewSpeed"></param>
        public void ChangeSloMoSpeed(float vNewSpeed)
        {
            if (CurrentState != PlaybackState.Play)
            {
                return;
            }
            else
            {
                mPlaybackSettings.PlaybackSpeed = vNewSpeed;
                mPlaybackTask.PlaybackSpeed = vNewSpeed;
                RecordingPlaybackSpeedDisplay.UpdateSpeedText(vNewSpeed);
            }
        }


        /// <summary>
        /// Body has been updated through another sub control and this control gets updated
        /// </summary>
        /// <param name="vBody"></param>
        public override void BodyUpdated(Body vBody)
        {
            if (mBody != null)
            {
                mBody.StopThread();
                mBody.View.BodyFrameUpdatedEvent -= BodyFrameUpdateHandler;
            }
            mBody = vBody;
            mBody.View.BodyFrameUpdatedEvent += BodyFrameUpdateHandler;
            mIsNewRecording = true;
            if (BodyUpdatedEvent != null)
            {
                BodyUpdatedEvent(vBody);
            }
            if (mBody != null)
            {
                mPreviousBodyLerpVal = BodySegment.IsUsingInterpolation;
            }

        }

        /// <summary>
        /// A new recording has been selected, calon a fucntion to start converting.
        /// </summary>
        /// <param name="vNewCsvBodyFramesRecording"></param>
        public void NewRecordingSelected(BodyFramesRecordingBase vNewCsvBodyFramesRecording)
        {
            if (mBody != null && vNewCsvBodyFramesRecording != null)
            {
                mBody.StopThread();
                if (mBody.InitialBodyFrame != null)
                {
                    var vPrev = BodySegment.IsUsingInterpolation;
                    BodySegment.IsUsingInterpolation = false;
                    mBody.View.ResetInitialFrame();
                    BodySegment.IsUsingInterpolation = vPrev;
                }

                if (vNewCsvBodyFramesRecording.RecordingRawFramesCount <= RecordingPlaybackTask.StartConversionIndex)
                {
                    ModalPanel.SingleChoice("ERROR", LocalizationBinderContainer.GetString(KeyMessage.RecordingFileLessThanSkippableFrames),
                      () => { });
                    ReleaseResources();
                    return;
                }

                string vRecGuid = vNewCsvBodyFramesRecording.BodyRecordingGuid;
                mBody.PlayRecording(vRecGuid);
                //update the recording playback task by polling the body
                StopCoroutine(CaptureRecordingPlaybackTask());
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(CaptureRecordingPlaybackTask());
                }
                if (BodyUpdatedEvent != null)
                {
                    BodyUpdatedEvent(mBody);
                }
                Debug.Log("playing back " + vNewCsvBodyFramesRecording.Title);
            }
            mIsNewRecording = true;
        }


        private void ResetControls()
        {
            ReleaseResources();
        }
        /// <summary>
        /// Polls the body until its create a recording playback task
        /// </summary> 
        /// <returns></returns>
        IEnumerator CaptureRecordingPlaybackTask()
        {
            while (mBody.MBodyFrameThread.PlaybackTask == null)
            {
                yield return null;
            }
            mPlaybackTask = mBody.MBodyFrameThread.PlaybackTask;  
            if(RecordingPlaybackTaskUpdated!= null)
            {
                RecordingPlaybackTaskUpdated(mPlaybackTask);
            }   
        }

        /// <summary>
        /// fast forward playback 
        /// </summary>
        public void FastForward(int vStepForwardMultiplier)
        {
            if (CurrentState == PlaybackState.Pause)
            {
                PlaybackSettings.FrameSkipMultiplier = vStepForwardMultiplier;
                ChangeState(PlaybackState.StepForward);
            }
        }

        public void Rewind(int vStepbackMultiplier)
        {
            if (CurrentState == PlaybackState.Pause)
            {
                PlaybackSettings.FrameSkipMultiplier = vStepbackMultiplier;
                ChangeState(PlaybackState.StepBackward);
            }

        }

        void OnEnable()
        {
            if (!mCanUse)
            {
                gameObject.SetActive(false);
                return;
            }
            PlayPauseSubControls.RequestResources();
            RecordingForwardSubControl.RequestResources();
            RecordingRewindSubControl.RequestResources();
        }

        /// <summary>
        /// Stops the current player, resets the body
        /// </summary>
        public override void ReleaseResources()
        {
            ChangeState(PlaybackState.Pause);
            if (PlaybackTask != null)
            {

                foreach (var vSubControl in mSubControls)
                {
                    vSubControl.Disable();
                }
            }
            RecordingProgressSliderSubControl.PlaySlider.value = 0;
            PlaybackSpeedModifierSubControl.PlaybackSpeedSlider.value = 1;
            PlayPauseSubControls.ReleaseResources();
            RecordingForwardSubControl.ReleaseResources();
            RecordingRewindSubControl.ReleaseResources();
        }

        /// <summary>
        /// Pauses playback
        /// </summary>
        public void Pause()
        {
            ChangeState(PlaybackState.Pause);
        }

        public void SetInteractionLevel(UserRoleType vRoleType)
        {
            mCanUse = UserRolePermission.HasPermission(this.GetType(), vRoleType);
            foreach (var vAbstractSubControl in mAbstractSubControls)
            {
                vAbstractSubControl.SetInteractionLevel(vRoleType);
            }
        }

        private void InvokeFinalFrameReachedEvent()
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                if (FinalFramePositionEvent != null)
                {
                    FinalFramePositionEvent();
                }
            });

        }
    }

    /// <summary>
    /// Current state of the player control panel
    /// </summary>
    public enum PlaybackState
    {
        /// <summary>
        ///Null:        Depicts a state where there is no recording loaded
        /// </summary>
        Null,
        /// <summary>
        /// Play:       Depicts a state where there is a recording loaded and is currently playing the recording
        /// </summary>
        Play,
        /// <summary>
        /// Pause:      Depicts a state where there is a recording loaded but is paused and ready to be played
        /// </summary>
        Pause,
        /// <summary>
        ///FastBackward: Depicts a state where there is a recording loaded and is currently playing the recording backwards at high speed
        /// </summary>
        FastBackward,
        /// <summary>
        /// go Forward: Depicts a state where there is a recording loaded and is currently playing the recording backwards at high speed
        /// </summary>
        FastForward,
        /// <summary>
        /// StepForward:Depicts a state where there is a recording loaded, paused and steps forward one frame
        /// </summary>
        StepForward,
        /// <summary>
        /// StepBackward: Depicts a state where there is a recording loaded, paused and steps backwards one frame
        /// </summary>
        StepBackward,
        /// <summary>
        /// SlowMotionForward:Depicts a state where there is a recording loaded, being played and at a rate of speed between 0.1 and exclusive 1 
        /// </summary>
        SlowMotionForward
    }

}
