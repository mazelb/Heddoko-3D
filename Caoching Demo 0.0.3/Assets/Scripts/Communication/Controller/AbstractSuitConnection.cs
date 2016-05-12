/** 
* @file AbstractSuitConnection.cs
* @brief Contains the AbstractSuitConnection class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;

namespace Assets.Scripts.Communication.Controller
{
    /// <summary>
    /// Provides an interface for a SuitConnection
    /// </summary>
    public abstract class AbstractSuitConnection : MonoBehaviour
    {
        public delegate void BpConnectionControllerDel();

        public delegate void BpConnectionControllerRespDel(string vResponse);

        public delegate void SuitStateHandler(SuitState vSuitState);

        public delegate void TimeOutStateResponse();

        public BpConnectionControllerDel ConnectingStateEvent;
        public BpConnectionControllerDel ConnectedStateEvent;
        public BpConnectionControllerDel DisconnectedStateEvent;
        public BpConnectionControllerDel FailedToConnectStateEvent;
        public BpConnectionControllerDel BrainpackShutdown;
        public BpConnectionControllerDel ResetBrainpackResp;
        public BpConnectionControllerRespDel BrainpackStatusResponse;
        public BpConnectionControllerDel BrainpackTimeSetResp;
        public SuitStateHandler OnSuitStateUpdate;
         

        internal const string  mSuitStatePattern = "(?i)Reset(?-i)|(?i)Idle(?-i)|(?i)Recording(?-i)|(?i)Error(?-i)|(?i)Timeout(?-i)";
        public abstract void ConnectToBrainpack();
        public abstract void StartHeartBeat();

        public abstract void UpdateCurrentSuitState(string vMsg);

        public abstract void InitiateSuitReset();

        public abstract void InitiateSuitRecording();

        public abstract void InitateStopRecordingReq();

        public abstract void FlushBrainpack();

        public abstract void Reset();

        public abstract void TimeoutHandler();

        public abstract void DisconnectBrainpack();
    }
    public enum BrainpackConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
        Failure
    }
    public enum SuitState
    {
        Start,
        Idle,
        Reset,
        Error,
        Recording,
        Undefined
    }
}
