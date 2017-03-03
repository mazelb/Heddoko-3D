// /**
// * @file SuitController.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date 11 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.Body_Data;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.DebugContext.logging;
using heddoko;
using HeddokoLib.adt;
using HeddokoLib.HeddokoDataStructs.Brainpack;
using UnityEngine;

namespace Assets.Scripts.Communication.Communicators
{
    /// <summary>
    /// The controller to the suit
    /// </summary>
    public class SuitController : MonoBehaviour
    {
        SuitConnectionManager mConnectionManager = new SuitConnectionManager();
        BrainpackContainerController ContainerController = new BrainpackContainerController();
        private BrainpackAdvertisingListener mAdvertisingListener;
        [SerializeField]
        private BrainpackContainerPanel mContainerPanel;
        [SerializeField]
        private BrainpackStatusPanel mBrainpackStatusPanel;

        private const int PacketBufferSize = 8;
        private CircularQueue<Packet> mPacketBuffer;
        private ProtobuffFrameBodyFrameConverter mFrameConverter;



        public ProtobuffFrameBodyFrameConverter FrameConverter
        {
            get { return mFrameConverter; }
            set { mFrameConverter = value; }
        }

        public SuitConnectionManager ConnectionManager
        {
            get
            {
                if (mConnectionManager == null)
                {
                    mConnectionManager = new SuitConnectionManager();
                }
                return mConnectionManager;
            }
        }


        void Awake()
        {

            mPacketBuffer = new CircularQueue<Packet>(PacketBufferSize, true);
            mFrameConverter = new ProtobuffFrameBodyFrameConverter(mPacketBuffer, new BodyFrameBuffer(32));
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                Debug.Log("Remove this");
            });
            ContainerController.ContainerView = mContainerPanel;

            mAdvertisingListener = new BrainpackAdvertisingListener(3);
            mAdvertisingListener.StartListener(6668);
            RegisterHandlers();

        }

        /// <summary>
        /// Registers handlers
        /// </summary>
        private void RegisterHandlers()
        {
            mContainerPanel.BrainpackSelectedEvent += BrainpackSelectedHandler;
            mAdvertisingListener.RegisterBrainpackFoundEventHandler(NewBrainpackFound);
            mAdvertisingListener.RegisterBrainpackLostEventHandler(BrainpackLostHandler);
            ConnectionManager.ConcerningReportIncludedEvent += ConcernReportHandler;
            ConnectionManager.ImuDataFrameReceivedEvent += ImuDataFrameReceivedHandler;
            ConnectionManager.BrainpackConnectionStateChange += BrainpackControlSocketConnectedHandler;
            mBrainpackStatusPanel.RequestStreamStartEvent += RequestStreamStartHandler;
            ConnectionManager.StatusResponseEvent += StatusResponseHandler;
        }

        /// <summary>
        /// Removes all handlers
        /// </summary>
        private void RemoveHandlers()
        {
            mContainerPanel.BrainpackSelectedEvent -= BrainpackSelectedHandler;
            mAdvertisingListener.RemoveBrainpackFoundEventHandler(NewBrainpackFound);
            mAdvertisingListener.RemoveBrainpackLostEventHandler(BrainpackLostHandler);
            ConnectionManager.ConcerningReportIncludedEvent -= ConcernReportHandler;
            ConnectionManager.ImuDataFrameReceivedEvent -= ImuDataFrameReceivedHandler;
            ConnectionManager.BrainpackConnectionStateChange -= BrainpackControlSocketConnectedHandler;
            mBrainpackStatusPanel.RequestStreamStartEvent -= RequestStreamStartHandler;
            ConnectionManager.StatusResponseEvent -= StatusResponseHandler;
        }

        /// <summary>
        /// Status response handler of the brainpack 
        /// </summary>
        /// <param name="vObj"></param>
        private void StatusResponseHandler(Packet vObj)
        {
            int vBatteryLevel = (int)vObj.batteryLevel;
            var vBatteryChargeState = vObj.chargeState;
            var vBrainpackState = vObj.brainpackState;
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => mBrainpackStatusPanel.UpdateStatus(vBatteryLevel, vBatteryChargeState, vBrainpackState));
        }

        /// <summary>
        /// Handler for a stream start
        /// </summary>
        /// <param name="vFlag"></param>
        private void RequestStreamStartHandler(bool vFlag)
        {
            if (vFlag)
            {
                ConnectionManager.RequestDataStreamFromBrainpack(1258);
                ConnectionManager.RequestSuitStatus();
            }
            else
            {
                ConnectionManager.RequestStreamFromBrainpackStop();
                ConnectionManager.RequestSuitStatus();
            }
        }


        /// <summary>
        /// Handler for network state changes.
        /// </summary>
        /// <param name="vArg1"></param>
        /// <param name="vArg2"></param>
        private void BrainpackControlSocketConnectedHandler(BrainpackConnectionStateChange vArg1, BrainpackNetworkingModel vArg2)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                () =>
                {
                    mBrainpackStatusPanel.SetBrainpackTcpControlState(vArg1);
                    if (vArg1.NewState == BrainpackConnectionState.Connected)
                    {
                         mConnectionManager.RequestSuitStatus();
                    }
                });
        }


        /// <summary>
        /// Handler for imu frame data received
        /// </summary>
        /// <param name="vPacket"></param>
        private void ImuDataFrameReceivedHandler(Packet vPacket)
        {
            mPacketBuffer.Enqueue(vPacket);
        }

        /// <summary>
        /// Handler for Concern reporting
        /// </summary>
        /// <param name="vIsInPain"></param>
        /// <param name="vPacket"></param>
        private void ConcernReportHandler(bool vIsInPain, Packet vPacket)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
             () =>
             {
                 Debug.Log("ConcernReportHandler received frame!");
             });
        }

        /// <summary>
        /// brainpack lost handler event
        /// </summary>
        /// <param name="vBrainpack"></param>
        private void BrainpackLostHandler(BrainpackNetworkingModel vBrainpack)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
           () =>
           {
               ContainerController.RemoveBrainpack(vBrainpack);
           });
        }

        /// <summary>
        /// Handler for a new brainpack found event
        /// </summary>
        /// <param name="vBrainpack"></param>
        private void NewBrainpackFound(BrainpackNetworkingModel vBrainpack)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
          () =>
          {
              ContainerController.AddBrainpack(vBrainpack);
          });
        }



        /// <summary>
        /// Brainpack has been selected. Launches and updates a view.
        /// </summary>
        /// <param name="vSelected"></param>
        private void BrainpackSelectedHandler(BrainpackNetworkingModel vSelected)
        { 
            ConnectionManager.ConnectToSuitControlSocket(vSelected);
        }

        internal void OnApplicationQuit()
        {
            FrameConverter.StopIfWorking();
            RemoveHandlers();
            ConnectionManager.CleanUp();
            mAdvertisingListener.StopListening();
            DebugLogger.Instance.Stop();
        }
    }
}