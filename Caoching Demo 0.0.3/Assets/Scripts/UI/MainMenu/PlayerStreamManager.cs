
/** 
* @file PlayerStreamManager.cs
* @brief Contains the PlayerStreamManager class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
 
using System.Collections.Generic;
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.Loading;
using Assets.Scripts.UI.Metrics; 
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainMenu
{
    /// <summary>
    /// Preps the body for playing a recording. BodyPlayer class , essentially a player that streams from a brainpack or recording. Todo: this class holds both the view and controls. 
    /// </summary>
    public class PlayerStreamManager : MonoBehaviour
    {
        public Body CurrentBodyInPlay { get; set; }
        public RenderedBody RenderedBody;
        [SerializeField]
        private BodyPlaybackState mCurrentState = BodyPlaybackState.Waiting;


        public delegate void BodyChangedDelegate(Body vNewBody);

        public static event BodyChangedDelegate BodyChangedEvent;

        //  private bool mPlayButtonPushed; 
        public float PauseThreadTimer = 1f;
        public float OriginalSpineHeight = 0.09226318f;
        public float SpineYOffset = 2.13f;
        public Transform Spine;
        public bool UseLocalPos;
        public string SquatRecordingUUID;
        public string BikeRecordingUUID;


        private string mSelectedRecordingPath;
        // pause thread routine started
        private bool mResetRoutineStarted = false;

        private string mBodyRecordingUUID;
        private bool mUsingBrainpack = false;
        public bool SpineSplitDisabled = false;
        private bool mCanUseBrainpack = false;

        public Button[] TPoseButtons;

        public List<IResettableMetricView> ResettableViews = new List<IResettableMetricView>(4);
        private Dictionary<string, BodyFramesRecording> mRecordings = new Dictionary<string, BodyFramesRecording>(10);

        public BodyPlaybackState CurrentState { get { return mCurrentState; } }
        /// <summary>
        /// Current state of the playback
        /// </summary>
        public enum BodyPlaybackState
        {
            //waiting for a response
            Waiting,
            PlayingRecording,
            StreamingFromBrainPack
        }
        /// <summary>
        /// On the start of the scene, initialize all the Components to be able to start playing
        /// </summary>
        void Awake()
        {
            if (CurrentBodyInPlay == null)
            {
                //check what the current count is
                int vBodiesCount = BodiesManager.Instance.Bodies.Count;

                if (vBodiesCount == 0)
                {
                    //create a new body from the body manager
                    BodiesManager.Instance.CreateNewBody("BrainpackPlaceholderBody");
                }

                //get the first body
                CurrentBodyInPlay = BodiesManager.Instance.GetBodyFromUUID("BrainpackPlaceholderBody");
                RenderedBody.Init();
                CurrentBodyInPlay.UpdateRenderedBody(RenderedBody);
            }

            for (int i = 0; i < TPoseButtons.Length; i++)
            {
                TPoseButtons[i].onClick.AddListener(ResetBody);
            }
        }

        /// <summary>
        /// OnEnable, hook listeners
        /// </summary>
        void OnEnable()
        {
            BrainpackConnectionController.Instance.ConnectedStateEvent += OnBrainpackConnectSuccessListener;
            BrainpackConnectionController.Instance.DisconnectedStateEvent += OnBrainpackDisconnectListener;
        }

        /// <summary>
        /// OnDisable, unhook listeners
        /// </summary>
        public void OnDisable()
        {
            BrainpackConnectionController.Instance.ConnectedStateEvent -= OnBrainpackConnectSuccessListener;
            BrainpackConnectionController.Instance.DisconnectedStateEvent -= OnBrainpackDisconnectListener;
        }


        /**
        * PlayCurrentBody 
        * @brief Will play the recording for the prepped body
        */
        public void PlayCurrentBody()
        { 
            Stop();
            if (CurrentBodyInPlay != null)
            {
                ChangeState(BodyPlaybackState.PlayingRecording);
            }
        }

        /// <summary>
        /// Sets/unsets the torso from the hips
        /// </summary>
        /// <param name="vFlag"></param>

        public void StickTorsoToHips(bool vFlag)
        {
            if (CurrentBodyInPlay != null)
            {
                Vector3 vPos = Vector3.zero;
                if (vFlag)
                {
                    mBodyRecordingUUID = SquatRecordingUUID;
                    if (Spine)
                    {
                        if (UseLocalPos)
                        {
                            vPos = Spine.localPosition;
                            vPos.y = OriginalSpineHeight;
                            Spine.localPosition = vPos;
                        }
                        else
                        {
                            vPos = Spine.position;
                            vPos.y = OriginalSpineHeight;
                            Spine.position = vPos;
                        }
                    }
                }
                else
                {
                    mBodyRecordingUUID = BikeRecordingUUID;
                    if (!SpineSplitDisabled)
                    {
                        if (UseLocalPos)
                        {
                            vPos = Spine.localPosition;
                            vPos.y = SpineYOffset;
                            Spine.localPosition = vPos;
                        }
                        else
                        {
                            vPos = Spine.position;
                            vPos.y = SpineYOffset;
                            Spine.position = vPos;
                        }
                    }
                }
                ChangeState(BodyPlaybackState.PlayingRecording);
            }

        } 

        /**
       * ResetOrientations 
       * @brief will set the Initial Frame
       */
        private void ResetInitialFrame()
        {
            if (CurrentBodyInPlay != null && CurrentBodyInPlay.CurrentBodyFrame != null)
            {
                CurrentBodyInPlay.View.ResetInitialFrame();
                if (mCurrentState == BodyPlaybackState.StreamingFromBrainPack)
                {
                    BrainpackConnectionController.Instance.FlushBrainpack();
                } 
            } 
        }

        /// <summary>
        /// Sets the current body to stream from recording
        /// </summary>
        public void SetBodyToStreamFromRecording()
        {
            ChangeState(BodyPlaybackState.PlayingRecording);
        }

        /// <summary>
        /// sets the current body to stream from the brainpack
        /// </summary>
        public void SetBodytoStreamFromBrainpack()
        {
            ChangeState(BodyPlaybackState.StreamingFromBrainPack);
        }

        /// <summary>
        /// Safely change the current state of the body player
        /// </summary>
        /// <param name="vNewstate"></param>
        public void ChangeState(BodyPlaybackState vNewstate)
        {
            switch (mCurrentState)
            {
                case BodyPlaybackState.Waiting:
                    {
                        if (vNewstate == BodyPlaybackState.PlayingRecording)
                        {
                            if (!string.IsNullOrEmpty(mBodyRecordingUUID))
                            {
                                CurrentBodyInPlay.StopThread();
                                CurrentBodyInPlay.PlayRecording(mBodyRecordingUUID);

                            }
                            mCurrentState = vNewstate;
                            break;
                        }
                        if (vNewstate == BodyPlaybackState.StreamingFromBrainPack)
                        {
                            Stop();
                            CurrentBodyInPlay.StreamFromBrainpack();
                            mCurrentState = vNewstate;
                        }
                        break;
                    }
                case BodyPlaybackState.PlayingRecording:
                    {
                        if (vNewstate == BodyPlaybackState.Waiting)
                        {
                            ResetBody();
                            Stop();
                            mCurrentState = vNewstate;
                            break;
                        }
                        if (vNewstate == BodyPlaybackState.StreamingFromBrainPack)
                        {
                            Stop();
                            CurrentBodyInPlay.StreamFromBrainpack();
                            mCurrentState = vNewstate;
                            break;
                        }
                        if (vNewstate == BodyPlaybackState.PlayingRecording)
                        {
                            if (!string.IsNullOrEmpty(mBodyRecordingUUID))
                            {
                                CurrentBodyInPlay.PlayRecording(mBodyRecordingUUID);
                            }
                            mCurrentState = vNewstate;

                        }
                        break;
                    }
                case BodyPlaybackState.StreamingFromBrainPack:
                    {
                        if (vNewstate == BodyPlaybackState.Waiting)
                        {
                            ResetBody();
                            Stop();
                            
                            mCurrentState = vNewstate;
                            break;
                        }
                        if (vNewstate == BodyPlaybackState.PlayingRecording)
                        {
                            if (!string.IsNullOrEmpty(mBodyRecordingUUID))
                            {
                                Stop();
                                CurrentBodyInPlay.PlayRecording(mBodyRecordingUUID);
                            }

                            mCurrentState = vNewstate;
                        }

                        //in the case that a new body and its associated view have been created, the stream needs to be connected
                        //to this body
                        if (vNewstate == BodyPlaybackState.StreamingFromBrainPack)
                        {
                            Stop();
                            CurrentBodyInPlay.StreamFromBrainpack();
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

        public void ResumeFromPauseState()
        {
            if (CurrentBodyInPlay.View.IsPaused)
            {
                CurrentBodyInPlay.View.PauseFrame();
            }
        }


        /// <summary>
        /// Listens to when the brainpackcontroller is in a connected state
        /// </summary>
        private void OnBrainpackConnectSuccessListener()
        {
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
                    CurrentBodyInPlay = BodiesManager.Instance.GetBodyFromUUID("BrainpackPlaceholderBody"); 
                     CurrentBodyInPlay.UpdateRenderedBody(RenderedBody);

            }
            mCanUseBrainpack = true;
        }

        /// <summary>
        /// Listens to when the BrainpackController is in a disconnected state
        /// </summary>
        private void OnBrainpackDisconnectListener()
        {
            mCanUseBrainpack = false;
        }

        public void Stop()
        {
            if (CurrentBodyInPlay != null)
            {
                CurrentBodyInPlay.StopThread();
            }
        }


        /// <summary>
        /// Clears the buffer of the current body in play.
        /// </summary>
        public void ClearBuffer()
        {
            if (CurrentBodyInPlay != null)
            {
                CurrentBodyInPlay.View.ClearBodyBuffer();
            }
        }
        /// <summary>
        /// Resets the body and the metrics associated with body.
        /// </summary>
        public void ResetBody()
        {
            bool vPreIntVal = BodySegment.IsUsingInterpolation;
            BodySegment.IsUsingInterpolation = false;
            ResetInitialFrame();
            BodySegment.IsUsingInterpolation = vPreIntVal;
            if (CurrentBodyInPlay != null)
            {
                RightLegAnalysis vRightLegAnalysis =
                      CurrentBodyInPlay.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                          RightLegAnalysis;
                if (vRightLegAnalysis != null)
                {
                    vRightLegAnalysis.NumberofSquats = 0;
                }
            }

            if (ResettableViews != null)
            {
                for (int vI = 0; vI < ResettableViews.Count; vI++)
                {
                    ResettableViews[vI].ResetValues();
                }
            }

        }
        /// <summary>
        /// Requests to load a recording based on the requested index
        /// </summary>
        /// <param name="vRecordingIndex">the recording index </param>
        public void RequestRecordingForPlayback(int vRecordingIndex)
        {
            Stop();
            if (vRecordingIndex >= 0 && vRecordingIndex < BodyRecordingsMgr.Instance.FilePaths.Length)
            {
                //get the recording path from the list of all the scanned file paths
                mSelectedRecordingPath = BodyRecordingsMgr.Instance.FilePaths[vRecordingIndex];

                LoadingBoard.StartLoadingAnimation();

                //check if the recording doesnt exists 
                if (!mRecordings.ContainsKey(mSelectedRecordingPath))
                {
                    // ChangeState(BodyPlaybackState.Waiting);
                    BodyRecordingsMgr.Instance.ReadRecordingFile(mSelectedRecordingPath, RequestRecordingCallback);
                }

                //start play back
                else
                {
                    LoadingBoard.StopLoadingAnimation();
                    BodyFramesRecording vRecording = mRecordings[mSelectedRecordingPath];
                    CurrentBodyInPlay = BodiesManager.Instance.GetBodyFromRecordingUUID(vRecording.BodyRecordingGuid);
                    mBodyRecordingUUID = vRecording.BodyRecordingGuid;
                    ChangeState(BodyPlaybackState.PlayingRecording);
                }
            }
        }

        /// <summary>
        /// Requests to load a recording based on the requested partial or full path
        /// </summary>
        /// <param name="vSubPath"></param>
        public void RequestRecordingForPlayback(string vSubPath)
        {
            //check if the path exists first from the subpath parameter
            int vRecordingIndex = -1;
            string[] vFullPaths = BodyRecordingsMgr.Instance.FilePaths;
            for (int vI = 0; vI < vFullPaths.Length; vI++)
            {
                if (vFullPaths[vI].Contains(vSubPath))
                {
                    vRecordingIndex = vI;
                }
            }

            RequestRecordingForPlayback(vRecordingIndex);
        }
        /// <summary>
        /// After a request has been initiated, the callback function is called on completion
        /// </summary>

        private void RequestRecordingCallback(BodyFramesRecording vRecording)
        {
            if (vRecording != null)
            {

                LoadingBoard.StopLoadingAnimation();
                //find the recording in play
                CurrentBodyInPlay = BodiesManager.Instance.GetBodyFromUUID(vRecording.BodyGuid);
                //add it to the current list of recordings
                mRecordings.Add(mSelectedRecordingPath, vRecording);
                mBodyRecordingUUID = vRecording.BodyRecordingGuid;
                
                PlayCurrentBody();
            }
        }

        /// <summary>
        /// On application quit, call the BodyRecordingsManager stop function
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private void OnApplicationQuit()
        {
            BodyRecordingsMgr.Stop();
        }

    }
}
