// /**
// * @file SuitController.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date 11 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using heddoko;
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

        /// <summary>
        /// On Start instantiate local instances
        /// </summary>
        void Start()
        {
            ContainerController.ContainerView = mContainerPanel;
            mConnectionManager = new SuitConnectionManager();
            mAdvertisingListener = new BrainpackAdvertisingListener(10);
            mAdvertisingListener.StartListener(6668);
            RegisterHandlers();
        }

        /// <summary>
        /// Registers handlers
        /// </summary>
        private void RegisterHandlers()
        {
            mContainerPanel.BrainpackSelectedEvent += BrainpackSelectedHandler;
            mAdvertisingListener.BrainpackFoundEvent += NewBrainpackFound;
            mAdvertisingListener.BrainpackLostEvent += BrainpackLostHandler;
            mConnectionManager.ConcerningReportIncludedEvent += ConcernReportHandler;
            mConnectionManager.ImuDataFrameReceivedEvent += ImuDataFrameReceivedHandler;
            mConnectionManager.BrainpackConnectionStateChange += BrainpackControlSocketConnectedHandler;
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
                }
                
                );
        }


        /// <summary>
        /// Handler for imu frame data received
        /// </summary>
        /// <param name="vPacket"></param>
        private void ImuDataFrameReceivedHandler(Packet vPacket)
        {
            
        }

        /// <summary>
        /// Handler for Concern reporting
        /// </summary>
        /// <param name="vIsInPain"></param>
        /// <param name="vPacket"></param>
        private void ConcernReportHandler(bool vIsInPain, Packet vPacket)
        {

        }

        /// <summary>
        /// brainpack lost handler event
        /// </summary>
        /// <param name="vBrainpack"></param>
        private void BrainpackLostHandler(BrainpackNetworkingModel vBrainpack)
        {

        }

        /// <summary>
        /// Handler for a new brainpack found event
        /// </summary>
        /// <param name="vBrainpack"></param>
        private void NewBrainpackFound(BrainpackNetworkingModel vBrainpack)
        {

        }

        

        /// <summary>
        /// Brainpack has been selected. Launches and updates a view.
        /// </summary>
        /// <param name="vSelected"></param>
        private void BrainpackSelectedHandler(BrainpackNetworkingModel vSelected)
        {

        }
    }
}