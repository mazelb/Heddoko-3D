// /**
// * @file TestConnection.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Net;
using Assets.Scripts.Communication.Communicators;
using Assets.Scripts.UI;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.Utils;
using heddoko;
using HeddokoLib.HeddokoDataStructs.Brainpack;
using UnityEngine;

namespace Assets.Scripts.Communication
{
    public class TestConnection : MonoBehaviour
    {
        public SuitConnectionManager SuitConnectionManager;
        private Brainpack mBrainpack;
        public BrainpackStatusPanel BrainpackStatusPanel;
        public BrainpackContainerPanel BrainpackContainerPanel;
        private Version mServerVersion;
        private Version mBrainpackVersion;
        private BrainpackAdvertisingListener mAdvertisingListener;
        
        void Awake()
        {
            OutterThreadToUnityThreadIntermediary.Instance.Init();
        }
        void Start()
        {
            mBrainpack = new Brainpack();
             SuitConnectionManager = new SuitConnectionManager(mBrainpack);
            // SuitConnectionManager.ConnectToSuit("127.0.0.1",8844);
            //SuitConnectionManager.NetworkSuitConnectionEstablishedEvent += UpdateStatusPanelView;
            //SuitConnectionManager.AddBrainpackUpdatedHandler(BrainpackStatusHandle);
            //BrainpackStatusPanel.RegisterFirmwareSubPanelUpdateCallback(UpdateFirmware);
            mAdvertisingListener = new BrainpackAdvertisingListener(10);
            mAdvertisingListener.BrainpackFoundEvent += NewBrainpackFound;
            mAdvertisingListener.BrainpackLostEvent += BrainpackLostHandler;
            mAdvertisingListener.StartListener(6668);
            mServerVersion = new Version(1, 5, 8, 0);
            BrainpackContainerPanel.BrainpackSelectedEvent += BrainpackSelected;
            BrainpackStatusPanel.UpdateLatestVersionText(mServerVersion.ToString());
        }

        private void BrainpackSelected(Brainpack vSelected)
        {
            Version vBpVersion = new Version(vSelected.Version);
            if (vBpVersion.CompareTo(mServerVersion) < 0)
            {
                BrainpackStatusPanel.EnableFirmwareUpdateSubpanel();
                BrainpackStatusPanel.RegisterFirmwareSubPanelUpdateCallback(UpdateFirmware);
            }
            mBrainpack = vSelected;

            IPEndPoint vEndPoint = (IPEndPoint)vSelected.Point;
            string vIpAddress = vEndPoint.Address.ToString();
            SuitConnectionManager.ConnectToSuit(vIpAddress, vSelected.TcpControlPort);
            Packet vPacket = new Packet();
            vPacket.type = PacketType.StartDataStream;
            vPacket.recordingRate = 20u;
            vPacket.endpoint = new Endpoint();
            vPacket.endpoint.address = "192.168.11.140";
            vPacket.endpoint.port = 8458;

            SuitConnectionManager.SendPacket(vPacket);
        }

        private void BrainpackLostHandler(Brainpack vBrainpack)
        {
           OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
           {
               Brainpack vBrainpackCopy = new Brainpack();
               vBrainpackCopy.Id = vBrainpack.Id;
               vBrainpackCopy.Version = vBrainpack.Version;
               vBrainpackCopy.Point = vBrainpack.Point;
               vBrainpackCopy.Status = vBrainpack.Status;
               BrainpackContainerPanel.RemoveBrainpackModel(vBrainpackCopy);
           }); 
         
        }

        private void NewBrainpackFound(Brainpack vBrainpack)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                () => BrainpackContainerPanel.AddBrainpackModel(vBrainpack));
       
        }

        private void UpdateStatusPanelView()
        {

 
        }

        private void UpdateFirmware()
        {
            if (SuitConnectionManager.IsConnectedToSuitViaNetwork)
            {
                SuitConnectionManager.UpdateFirmware("firmware.bin");
            }
            else
            {
                IPEndPoint vIp = mBrainpack.Point as IPEndPoint;
                SuitConnectionManager.ConnectToSuit(vIp.Address.ToString(),mBrainpack.TcpControlPort);
                SuitConnectionManager.UpdateFirmware("firmware.bin");
            }
          
        }

        private void BrainpackStatusHandle(Packet vPacket)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                if (mBrainpackVersion == null)
                {
                    if (vPacket != null)
                    {
                        if (vPacket.firmwareVersionSpecified)
                        {
                            mBrainpackVersion = new Version(vPacket.firmwareVersion);
                            if (mBrainpackVersion.CompareTo(mServerVersion) < 0)
                            {
                                BrainpackStatusPanel.EnableFirmwareUpdateSubpanel();
                            }
                        }

                    }
                }

                BrainpackStatusPanel.UpdateView(vPacket);
            });

        }


        void Update()
        {

            if (Input.GetKeyDown(KeyCode.B))
            {
                SuitConnectionManager.RequestSuitStatus();
            }
        }


        void OnApplicationQuit()

        {
            if (SuitConnectionManager != null)
            {
                SuitConnectionManager.CleanUp();
            }
            mAdvertisingListener.StopListening();
            mAdvertisingListener.BrainpackFoundEvent -= NewBrainpackFound;
            mAdvertisingListener.BrainpackLostEvent -= BrainpackLostHandler;
            BrainpackContainerPanel.BrainpackSelectedEvent -= BrainpackSelected;

        }
    }
}