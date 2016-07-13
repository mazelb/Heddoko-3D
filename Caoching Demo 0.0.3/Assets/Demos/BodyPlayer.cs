
/** 
* @file BodyPlayer.cs
* @brief Contains the BodyPlayer class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
 
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.UI.MainScene.Model;
using Assets.Scripts.Utils.UnityUtilities;
using Assets.Scripts.Utils.UnityUtilities.Repos;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Demos
{
    /// <summary>
    /// Preps the body for playing a recording. BodyPlayer class , essentially a player that streams from a brainpack or recording. Todo: this class holds both the view and controls. 
    /// </summary>
    public class BodyPlayer : MonoBehaviour
    {
        public Body CurrentBodyInPlay { get; set; }
        public Button PlayButton;
        [SerializeField]
        private BodyPlaybackState mCurrentState = BodyPlaybackState.Waiting;
        private bool mPlayButtonPushed;
        public Button ResetButton;
        public float PauseThreadTimer = 1f;  
        private string mBodyRecordingUuid; 
        private Sprite mBluetoothIcon;
        private Sprite mPlayButtonOriginalIcon;
        public DisplayLegAngleExtractions DisplayLegAngleExtractions;

        private Sprite BluetoothIcon 
        {
            get
            {
                if (mBluetoothIcon == null)
                {
                    mBluetoothIcon = SpriteRepo.BluetoothIcon;
                }
                return mBluetoothIcon;
            }
        }
        
        /// <summary>
        /// On the start of the scene, initialize all the Components to be able to start playing
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            mPlayButtonOriginalIcon = PlayButton.image.sprite;
            BodyFramesRecordingBase vRec = BodySelectedInfo.Instance.CurrentSelectedRecording;
            if (vRec != null)
            {
                mBodyRecordingUuid = vRec.BodyRecordingGuid;
                mBodyRecordingUuid = vRec.BodyRecordingGuid;
                CurrentBodyInPlay = BodiesManager.Instance.GetBodyFromRecordingUUID(mBodyRecordingUuid);
            }

            PlayButton.onClick.AddListener(Play);
            ResetButton.onClick.AddListener(ResetInitialFrame);

        }

        /// <summary>
        /// OnEnable, hook listeners
        /// </summary>
        public void OnEnable()
        {
            BodySelectedInfo.Instance.BodyRecordingChangedEvent += ListenToBodyRecordingsChange;
            BrainpackConnectionController.Instance.ConnectedStateEvent += OnBrainpackConnectSuccessListener;
            BrainpackConnectionController.Instance.DisconnectedStateEvent += OnBrainpackDisconnectListener;
        }

        /// <summary>
        /// OnDisable, unhook listeners
        /// </summary>
        public void OnDisable()
        { 
            // ReSharper disable once DelegateSubtraction
            BrainpackConnectionController.Instance.ConnectedStateEvent -= OnBrainpackConnectSuccessListener;
            // ReSharper disable once DelegateSubtraction
            BrainpackConnectionController.Instance.DisconnectedStateEvent -= OnBrainpackDisconnectListener;
        }

        /**
        * Play 
        * @brief Will play the recording for the prepped body
        */
        public void Play()
        {
            if (!mPlayButtonPushed)
            {
                if (CurrentBodyInPlay != null)
                {
                    mPlayButtonPushed = true;
                    PlayButton.gameObject.SetActive(false);
                    ChangeState(BodyPlaybackState.PlayingRecording);
                    if (CurrentBodyInPlay != null && DisplayLegAngleExtractions!= null)
                    {
                        DisplayLegAngleExtractions.CurrentBody = CurrentBodyInPlay;
                    }
                }
            }
        }

        /**
        * ResetOrientations 
        * @brief will set the Initial Frame
        */
        public void ResetInitialFrame()
        {
            CurrentBodyInPlay.View.ResetInitialFrame();
            //StartCoroutine(StartPausingCountdown());
        }

        /// <summary>
        /// Safely change the current state of the body player
        /// </summary>
        /// <param name="vNewstate"></param>
        void ChangeState(BodyPlaybackState vNewstate)
        {
            switch (mCurrentState)
            {
                case BodyPlaybackState.Waiting:
                    {
                        if (vNewstate == BodyPlaybackState.PlayingRecording)
                        { 
                            CurrentBodyInPlay.StopThread();
                            CurrentBodyInPlay.PlayRecording(mBodyRecordingUuid);
                            mCurrentState = vNewstate;
                            break;
                        }
                        if (vNewstate == BodyPlaybackState.StreamingFromBrainPack)
                        {
                            CurrentBodyInPlay.StopThread();
                            CurrentBodyInPlay.StreamFromBrainpack();
                            mCurrentState = vNewstate;
                        }
                        break;
                    }
                case BodyPlaybackState.PlayingRecording:
                    {
                        if (vNewstate == BodyPlaybackState.Waiting)
                        {
                            CurrentBodyInPlay.StopThread();
                            mCurrentState = vNewstate;
                            break;
                        }
                        if (vNewstate == BodyPlaybackState.StreamingFromBrainPack)
                        {
                            CurrentBodyInPlay.StopThread();
                            CurrentBodyInPlay.StreamFromBrainpack();
                            mCurrentState = vNewstate;
                        }
                        break;
                    }
                case BodyPlaybackState.StreamingFromBrainPack:
                    {
                        if (vNewstate == BodyPlaybackState.Waiting)
                        {
                            CurrentBodyInPlay.StopThread();
                            mCurrentState = vNewstate;
                            break;
                        }
                        if (vNewstate == BodyPlaybackState.PlayingRecording)
                        {
                            CurrentBodyInPlay.StopThread();
                            CurrentBodyInPlay.PlayRecording(mBodyRecordingUuid);
                            mCurrentState = vNewstate;
                        }
                        break;
                    }
            }
        }

        /**
        * Pause 
        * @brief Pauses the recording's play back
        */
        public void ChangePauseState()
        {
            CurrentBodyInPlay.View.PauseFrame();
        }
 

        /// <summary>
        /// Listens to when a recording has been selected. sets the current state of the class accordingly
        /// </summary>
        private void ListenToBodyRecordingsChange()
        {
            PlayButton.gameObject.SetActive(true);
            if (CurrentBodyInPlay != null)
            {
                CurrentBodyInPlay.StopThread(); 
            }
            BodyFramesRecordingBase vRec = BodySelectedInfo.Instance.CurrentSelectedRecording;
            mBodyRecordingUuid = vRec.BodyRecordingGuid;
            CurrentBodyInPlay = BodiesManager.Instance.GetBodyFromRecordingUUID(mBodyRecordingUuid);
            mPlayButtonPushed = false;
            ChangeState(BodyPlaybackState.Waiting);
        }
        
        /// <summary>
        /// Listens to when the brainpackcontroller is in a connected state
        /// </summary>
        private void OnBrainpackConnectSuccessListener()
        {
           FadeInFadeOutEffect vFadeInFadeOutEffect= PlayButton.gameObject.AddComponent<FadeInFadeOutEffect>();
            vFadeInFadeOutEffect.FadeEffectTime = 2.5f;
            vFadeInFadeOutEffect.MaxAlpha = 255f;
            vFadeInFadeOutEffect.MinAlpha = 20;
            PlayButton.image.sprite = BluetoothIcon;
            PlayButton.GetComponentInChildren<Text>().text = "";
            PlayButton.interactable = false;
            //first check if there is a current body
            if (CurrentBodyInPlay == null)
            { 
                //check what the current count is
                int vBodiesCount = BodiesManager.Instance.Bodies.Count;
                if (vBodiesCount == 0)
                {
                    //create a new body from the body manager
                    BodiesManager.Instance.CreateNewBody("BrainpackPlaceholderBody");
                }
                CurrentBodyInPlay = BodiesManager.Instance.Bodies[0]; //get the first body
            }
             
            ChangeState(BodyPlaybackState.StreamingFromBrainPack);
        }

 
 
        /// <summary>
        /// Listens to when the BrainpackController is in a disconnected state
        /// </summary>
        private void OnBrainpackDisconnectListener()
        {
            //remove the halo effect 
            FadeInFadeOutEffect vFadeInFadeOutEffectToDestroy = PlayButton.GetComponent<FadeInFadeOutEffect>();
            if (vFadeInFadeOutEffectToDestroy != null)
            {
                Destroy(vFadeInFadeOutEffectToDestroy);
            }
            PlayButton.image.sprite = mPlayButtonOriginalIcon;
            //reset the play button back to its original sprite
            PlayButton.GetComponentInChildren<Text>().text = "Play";
            PlayButton.interactable = true;
            ChangeState(BodyPlaybackState.Waiting);
        } 

    }
    /// <summary>
    /// Current state of the playback
    /// </summary>
    public enum BodyPlaybackState
    {
        Waiting, //waiting for a response
        PlayingRecording,
        StreamingFromBrainPack
    }
}
