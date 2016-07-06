/** 
* @file BrainpackConnectionController.cs
* @brief Contains the BrainpackConnectionModule class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Scripts.Communication.Communicators;
using Assets.Scripts.Communication.View;
using Assets.Scripts.Interfaces;
using Assets.Scripts.UI.MainMenu.View;
using HeddokoLib.networking;
using HeddokoLib.utils;
using UnityEngine;
using Assets.Scripts.Utils.UnityUtilities;
using Assets.Scripts.Utils.UnityUtilities.Repos;
using UIWidgets;

namespace Assets.Scripts.Communication.Controller
{

    /**
   * BrainpackConnectionController class that controls the connection to the heddoko body suit
   * @brief BrainpackConnectionModule class that  
   */

    /// <summary>
    /// BrainpackConnectionController class that controls the connection to the heddoko body suit
    /// </summary>
    public class BrainpackConnectionController : AbstractSuitConnection
    {

        public System.Action<Dictionary<string, string>> BpListUpdateHandle;
        public OutterThreadToUnityTrigger BrainpackConnectedTrigger = new OutterThreadToUnityTrigger();
        public OutterThreadToUnityTrigger SocketClientErrorTrigger = new OutterThreadToUnityTrigger();
        public string Output = "";
        private static BrainpackConnectionController mInstance;

        [SerializeField] private BrainpackConnectionState mCurrentConnectionState =
            BrainpackConnectionState.Disconnected;

        public string BrainpackComPort = "";
        private UdpListener mUdpListener = new UdpListener();

        [SerializeField] private int vTotalTries;

        public float Timeout = 4;
        public int MaxConnectionAttempts = 4;
        // private GameObject mLoadingScreen;
        private BodyFrameThread mBodyFrameThread;
        private IBrainpackConnectionView mView;
        public float StateReqTimer { get; set; }

        /// <summary>
        /// returns the current state of the controller
        /// </summary>
        public BrainpackConnectionState ConnectionState
        {
            get { return mCurrentConnectionState; }
        }

        /// <summary>
        /// Returns the view associated with the controller
        /// </summary>
        public IBrainpackConnectionView View
        {
            get
            {
                //Check what scene this instance is in

                if (mView == null)
                {
                    if (Application.loadedLevelName == "Coaching_utility_scene - split_screen" ||
                        Application.loadedLevelName == "Maz_SandBox")
                    {
                        GameObject vCanvas = GameObject.FindGameObjectWithTag("UiCanvas");
                        GameObject vInstantiated = Instantiate(PrefabRepo.BrainpackConnectionViewPrefab);
                        vInstantiated.transform.SetParent(vCanvas.transform, false);
                        vInstantiated.transform.SetAsLastSibling();
                        mView = vInstantiated.GetComponent<BrainpackConnectionView>();
                    }
                    else
                    {
                        mView = FindObjectOfType<MainMenuBrainpackView>();
                    }

                }
                return mView;
            }
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static BrainpackConnectionController Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject vGo = GameObject.FindGameObjectWithTag("Controllers");
                    mInstance = vGo.GetComponent<BrainpackConnectionController>();
                    if (mInstance == null)
                    {
                        mInstance = vGo.AddComponent<BrainpackConnectionController>();
                    }

                }
                return mInstance;
            }
        }




        /// <summary>
        /// Request to connect to brainpack
        /// </summary>
        public override void ConnectToBrainpack()
        {
            //validate the comport
            if (Validate(BrainpackComPort))
            {
                HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.RequestToConnectToBP, BrainpackComPort);
                ChangeCurrentState(BrainpackConnectionState.Connecting);
                PacketCommandRouter.Instance.Process(this, vHeddokoPacket);
            }

        }


        /// <summary>
        /// validate the comport input
        /// </summary>
        /// <param name="vComport"></param>
        /// <returns></returns>
        private bool Validate(string vComport)
        {
            if (string.IsNullOrEmpty(vComport))
            {
                return false;
            }
            string vPattern = @"(?i)COM(?-i)\d+";
            if (Regex.IsMatch(vComport, vPattern))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set the Brainpack controller to idle
        /// </summary>
        public void SetStateToIdle()
        {
            ChangeCurrentState(BrainpackConnectionState.Disconnected);
        }

        /// <summary>
        /// FailedToConnect listener: listen to events that the brainpack failed to connect
        /// </summary>
        /// <param name="vArgs">The failure message</param>
        internal void FailedToConnectListener(string vArgs)
        {
            SocketClientErrorTrigger.Triggered = true;
            SocketClientErrorTrigger.Args = vArgs;

        }

        public override void DisconnectBrainpack()
        {
            HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.DisconnectBrainpack, "");
            ChangeCurrentState(BrainpackConnectionState.Disconnected);
            PacketCommandRouter.Instance.Process(this, vHeddokoPacket);

        }

        internal void EnableBrainpackSleepTimer()
        {
            HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.EnableSleepTimerReq, "");
            PacketCommandRouter.Instance.Process(this, vHeddokoPacket);
        }

        internal void DisableBrainpackSleepTimer()
        {
            HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.DisableSleepTimerReq, "");
            PacketCommandRouter.Instance.Process(this, vHeddokoPacket);
        }

        /// <summary>
        /// The result of the brainpack connection
        /// </summary>
        /// <param name="vConnectionRes">The connection result</param>
        public void BrainpackConnectionResult(bool vConnectionRes)
        {
            if (!vConnectionRes)
            {
                //check if the max connection attempt has not been achieved
                if (vTotalTries < MaxConnectionAttempts)
                {
                    StartCoroutine(WaitAndThenReconnect(Timeout));
                    ChangeCurrentState(BrainpackConnectionState.Connecting);
                }
                else
                {
                    ChangeCurrentState(BrainpackConnectionState.Failure);
                }
            }
            else
            {
                vTotalTries = 0;
                ChangeCurrentState(BrainpackConnectionState.Connected);

            }
        }




        /// <summary>
        /// Wait and then try to reconnect
        /// </summary>
        /// <param name="vSeconds">the number of seconds to wait</param>
        /// <returns></returns>
        private IEnumerator WaitAndThenReconnect(float vSeconds)
        {
            yield return new WaitForSeconds(vSeconds);
            ConnectToBrainpack();
            vTotalTries++;
        }

        /// <summary>
        /// Change the current state of the controller and send out appropriate events
        /// </summary>
        /// <param name="vNewState">The new state to change to</param>
        internal void ChangeCurrentState(BrainpackConnectionState vNewState)
        {
            switch (mCurrentConnectionState)
            {
                case BrainpackConnectionState.Disconnected:
                {
                    if (vNewState == BrainpackConnectionState.Connecting)
                    {
                        if (ConnectingStateEvent != null)
                        {
                            mCurrentConnectionState = vNewState;
                            ConnectingStateEvent();
                        }
                    }
                    break;
                }
                case BrainpackConnectionState.Connecting:
                {
                    if (vNewState == BrainpackConnectionState.Failure)
                    {
                        if (FailedToConnectStateEvent != null)
                        {
                            mCurrentConnectionState = vNewState;
                            FailedToConnectStateEvent();
                        }

                        break;
                    }
                    if (vNewState == BrainpackConnectionState.Disconnected)
                    {
                        if (DisconnectedStateEvent != null)
                        {
                            mCurrentConnectionState = vNewState;
                            DisconnectedStateEvent();
                        }
                        mUdpListener.Stop();
                        //stop heart beat
                        StopCoroutine(HeartBeatRoutine());
                        break;
                    }
                    if (vNewState == BrainpackConnectionState.Connected)
                    {
                        if (ConnectedStateEvent != null)
                        {
                            mCurrentConnectionState = vNewState;
                            OnConnect();
                        }

                    }
                    break;
                }
                case BrainpackConnectionState.Connected:
                {

                    if (vNewState == BrainpackConnectionState.Connecting)
                    {
                        if (ConnectingStateEvent != null)
                        {
                            mCurrentConnectionState = vNewState;
                            ConnectingStateEvent();
                        }
                        break;
                    }

                    if (vNewState == BrainpackConnectionState.Disconnected)
                    {

                        if (DisconnectedStateEvent != null)
                        {
                            mCurrentConnectionState = vNewState;
                            DisconnectedStateEvent();

                        }
                        mUdpListener.Stop();
                        //stop heart beat
                        StopCoroutine(HeartBeatRoutine());
                        mCurrentConnectionState = vNewState;

                    }
                    break;
                }
                case BrainpackConnectionState.Failure:
                {
                    if (vNewState == BrainpackConnectionState.Connecting)
                    {
                        if (ConnectingStateEvent != null)
                        {
                            mCurrentConnectionState = vNewState;
                            ConnectingStateEvent();
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Send a request to the brainpack service to start sending data to the local listener
        /// </summary>
        private void RegisterListener()
        {
            HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.RegisterListener, "");
            PacketCommandRouter.Instance.Process(this, vHeddokoPacket);
        }

        /// <summary>
        /// On connection
        /// </summary>
        private void OnConnect()
        {
            if (ConnectedStateEvent != null)
            {
                ConnectedStateEvent();
            }
            StartHeartBeat();
            DisableBrainpackSleepTimer();
            SetBrainpackTimeCmd();
            mUdpListener.Start();
            RegisterListener();
        }

        /// <summary>
        /// on Awake: Initialize the instance 
        /// </summary>
        public void Awake()
        {
            BrainpackShutdown += () =>
            {
                ChangeCurrentState(BrainpackConnectionState.Disconnected);
            };

        }

        void OnEnable()
        {
            BrainpackStatusResponse += UpdateCurrentSuitState;
        }

        void OnDisable()
        {
            BrainpackStatusResponse -= UpdateCurrentSuitState;
        }

        /// <summary>
        /// On application quit, stop the socket client
        /// </summary>
        void OnApplicationQuit()
        {
            StopAllCoroutines();
            StopConnection();

        }

        void StopConnection()
        {
            if (mCurrentConnectionState == BrainpackConnectionState.Connected)
            {
                //Enable sleep timer on the brainpack
                EnableBrainpackSleepTimer();
                DisconnectBrainpack();
            }

            mUdpListener.Stop();

            HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.StopHeddokoUnityClient, "");
            PacketCommandRouter.Instance.Process(this, vHeddokoPacket);
        }

        /// <summary>
        /// reset the number of reconnect tries
        /// </summary>
        public void ResetTries()
        {
            vTotalTries = 0;
        }

        /// <summary>
        /// Requests brainpack responses
        /// </summary>
        public void RequestBrainpackResponses()
        {
            SendCommandToBrainpack(HeddokoCommands.GetResponseMessageReq);
        }

        /// <summary>
        /// The update function monitors the connected flag and starts to pull data
        /// </summary>
        void Update()
        {
            if (BrainpackConnectedTrigger.Triggered)
            {
                BrainpackConnectionResult(BrainpackConnectedTrigger.InterestedVariable);
                BrainpackConnectedTrigger.Reset();
            }
            if (SocketClientErrorTrigger.Triggered)
            {
                SocketClientErrorTrigger.Reset();
                if (View != null)
                {
                    View.SetWarningBoxMessage((string) SocketClientErrorTrigger.Args);
                }

                ChangeCurrentState(BrainpackConnectionState.Failure);
            }

            //start pulling data
            if (mCurrentConnectionState == BrainpackConnectionState.Connected)
            {
                //  HeddokoPacket vRequestBrainpackData = new HeddokoPacket(HeddokoCommands.RequestBPData, "Requesting data");
                // PacketCommandRouter.Instance.Process(this, vRequestBrainpackData);
                StateReqTimer -= Time.deltaTime;
            }


        }


        /// <summary>
        /// Sets the body as being ready to connect to the brainpack, once the brainpack is connected, the body will stream the brainpack
        /// </summary>
        /// <param name="vBodyFrameThread">The body frame thread to connect to </param>
        public void ReadyToLinkBodyToBP(BodyFrameThread vBodyFrameThread)
        {
            PacketCommandRouter.Instance.SetBodyFrameThread(vBodyFrameThread);
        }

        /// <summary>
        /// Disconnects the current body from the brainpack
        /// </summary>
        public void DisconnectBodyFromBP()
        {
            PacketCommandRouter.Instance.DisconnectFrameThread();
        }

        /// <summary>
        ///  Sends a command to the brainpack to change its recording prefix
        /// </summary>
        /// <param name="vRecordingPrefix"></param>
        public void ChangeRecordingPrefix(string vRecordingPrefix)
        {
            if (mCurrentConnectionState == BrainpackConnectionState.Connected)
            {
                HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.SetRecordingPrefixReq, vRecordingPrefix);
                PacketCommandRouter.Instance.Process(Instance, vHeddokoPacket);
            }
        }

        /// <summary>
        /// Sends a command to the brainpack to shut down 
        /// </summary>
        public void PowerOffBrainpackCmd()
        {
            SendCommandToBrainpack(HeddokoCommands.ShutdownBrainpackReq);
            ChangeCurrentState(BrainpackConnectionState.Disconnected);
        }

        /// <summary>
        /// Sends a command to the brainpack to reset 
        /// </summary>
        public override void InitiateSuitReset()
        {
            SendCommandToBrainpack(HeddokoCommands.ResetBrainpackReq);
            GetBrainpackStateCmd();
        }

        public override void InitateStopRecordingReq()
        {
            SendCommandToBrainpack(HeddokoCommands.StopRecordingReq);
            GetBrainpackStateCmd();
        }

        /// <summary>
        /// Sends a command to the brainpack to retrieve its current version
        /// </summary>
        public void GetBrainpackVersCmd()
        {
            SendCommandToBrainpack(HeddokoCommands.GetBrainpackVersionReq);
        }

        /// <summary>
        /// Sends a command to the brainpack to set its time to the current local time
        /// </summary>
        public void SetBrainpackTimeCmd()
        {
            SendCommandToBrainpack(HeddokoCommands.SetBrainpackTimeReq);
        }

        /// <summary>
        /// Sends a command to the brainpack to retrieve its current state
        /// </summary>
        public void GetBrainpackStateCmd()
        {
            SendCommandToBrainpack(HeddokoCommands.GetBrainpackStateReq);
        }

        /// <summary>
        /// Private helper method that sends a command to the brainpack 
        /// </summary>
        /// <param name="vCommand"></param>
        private void SendCommandToBrainpack(string vCommand)
        {
            if (mCurrentConnectionState == BrainpackConnectionState.Connected)
            {
                HeddokoPacket vPacket = new HeddokoPacket(vCommand, "");
                PacketCommandRouter.Instance.Process(this, vPacket);
            }

        }

        /// <summary>
        /// Sends a request to currently connected Brainpack to start recording
        /// </summary>
        public override void InitiateSuitRecording()
        {
            SendCommandToBrainpack(HeddokoCommands.StartRecordingReq);
            GetBrainpackStateCmd();
        }

        public override void FlushBrainpack()
        {
            SendCommandToBrainpack(HeddokoCommands.ClearBuffer);
        }

        /// <summary>
        /// Reset the state of the connection
        /// </summary>
        public override void Reset()
        {
            ResetTries();
        }

        /// <summary>
        /// handle timeouts
        /// </summary>
        public override void TimeoutHandler()
        {
            var message =
                "Connection to the brainpack service timedout.You can try to connect again or restart the brainpack service";
            Notify.Template("FadingNotifyTemplate")
                .Show(message, 4.5f, hideAnimation: Notify.FadeOutAnimation, showAnimation: Notify.FadeInAnimation,
                    sequenceType: NotifySequence.First, clearSequence: true);
            Reset();
            ChangeCurrentState(BrainpackConnectionState.Disconnected);
        }

        /// <summary>
        /// Starts the HeartBeat subroutine
        /// </summary>
        public override void StartHeartBeat()
        {
            StartCoroutine(HeartBeatRoutine());
        }

        public override void UpdateCurrentSuitState(string vMsg)
        {
            SuitState vReceivedState = SuitState.Undefined;
            if (Regex.IsMatch(vMsg, "Disconnected", RegexOptions.IgnoreCase))
            {
                ChangeCurrentState(BrainpackConnectionState.Disconnected);
                return;
            }

            if (Regex.IsMatch(vMsg, "Idle", RegexOptions.IgnoreCase) || string.IsNullOrEmpty(vMsg))
            {
                vReceivedState = SuitState.Idle;
            }
            else if (Regex.IsMatch(vMsg, "Reset", RegexOptions.IgnoreCase))
            {
                vReceivedState = SuitState.Reset;
            }
            else if (Regex.IsMatch(vMsg, "Recording", RegexOptions.IgnoreCase))
            {
                vReceivedState = SuitState.Recording;
            }
            else if (Regex.IsMatch(vMsg, "Error", RegexOptions.IgnoreCase))
            {
                vReceivedState = SuitState.Error;
            }

            if (OnSuitStateUpdate != null)
            {
                OnSuitStateUpdate.Invoke(vReceivedState);
            }

        }






        /// <summary>
        /// Heartbeat coroutine
        /// </summary>
        /// <returns></returns>
        private IEnumerator HeartBeatRoutine()
        {
            float vTimer = 3f;
            bool vMessageSent = false;
            while (mCurrentConnectionState == BrainpackConnectionState.Connected)
            {
                if (!vMessageSent)
                {
                    vMessageSent = true;
                    GetBrainpackStateCmd();
                    StateReqTimer = vTimer;
                }
                else
                {
                    yield return new WaitForSeconds(vTimer);
                    vMessageSent = false;
                }
                yield return null;
            }
        }

        /// <summary>
        /// Update the brainpack list
        /// </summary>
        /// <param name="vReturnedResults"></param>
        public void OnBrainpackListUpdate(Dictionary<string, string> vReturnedResults)
        {
            if (BpListUpdateHandle != null)
            {
                BpListUpdateHandle(vReturnedResults);
            }
        }


    }
}
