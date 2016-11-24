// /**
// * @file TestConnection.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Assets.Scripts.Communication.Communicators;
using Assets.Scripts.UI;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;
using Assets.Scripts.Utils;
using heddoko;
using HeddokoLib.HeddokoDataStructs.Brainpack;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Communication
{
    public class TestConnection : MonoBehaviour
    {
        public SuitConnectionManager SuitConnectionManager;
        private BrainpackNetworkingModel mBrainpack;
        public BrainpackStatusPanel BrainpackStatusPanel;
        public BrainpackContainerPanel BrainpackContainerPanel;
        private Version mServerVersion;
        private Version mBrainpackVersion;
        private BrainpackAdvertisingListener mAdvertisingListener;
        public OnlineMaps Map;
        private Queue<Vector2> mNewPositions = new Queue<Vector2>();
        [SerializeField]
        private float mTimeToComplete = 1.5f;

        public StandaloneInputModule StandaloneInputModule;
        void Awake()
        {
            OutterThreadToUnityThreadIntermediary.Instance.Init();
            StartCoroutine(CalculatePathPosition());
            StandaloneInputModule.forceModuleActive = true;
            Map.redrawOnPlay = true;
        }
        void Start()
        {
            mBrainpack = new BrainpackNetworkingModel();
            SuitConnectionManager = new SuitConnectionManager(mBrainpack);
            mAdvertisingListener = new BrainpackAdvertisingListener(10);
            mAdvertisingListener.BrainpackFoundEvent += NewBrainpackFound;
            mAdvertisingListener.BrainpackLostEvent += BrainpackLostHandler;
            mAdvertisingListener.StartListener(6668);
            mServerVersion = new Version(1, 5, 8, 0);
            BrainpackContainerPanel.BrainpackSelectedEvent += BrainpackSelected;
            BrainpackStatusPanel.UpdateLatestVersionText(mServerVersion.ToString());
            SuitConnectionManager.ConcerningReportIncludedEvent += ConcernReportHandler;
            OnlineMaps.instance.renderInThread = true;
            StandaloneInputModule.forceModuleActive = false;
            OnlineMaps.instance.position = new Vector2(-101.5942f, 49.3028f );
            OnlineMaps.instance.zoom = 3;
            StartCoroutine(DrawAfter());

        }

        IEnumerator DrawAfter()
        {
            yield return new WaitForSeconds(0.5f);
            OnlineMaps.instance.Testerino();
        }
        public bool isDrawing = false;
        /// <summary>
        /// Handle concern report event. 
        /// </summary>
        /// <param name="vIsInPain"></param>
        /// <param name="vVpacket"></param>
        private void ConcernReportHandler(bool vIsInPain, Packet vVpacket)
        {
            //get GPS data
            var vGpsCoords = vVpacket.fullDataFrame.gpsCoordinates;
            Vector2 vGpsNumCoords = new Vector2();
            string[] vGpsCoordsArr = vGpsCoords.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            float.TryParse(vGpsCoordsArr[1], out vGpsNumCoords.x);
            float.TryParse(vGpsCoordsArr[0], out vGpsNumCoords.y);
            string vLabel = vIsInPain ? "PAIN REPORTED" : "CONCERN REPORTED";
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                () =>
                {
                    if (!isDrawing)
                    {
                        OnlineMaps.instance.AddMarker(vGpsNumCoords, vLabel);
                        OnlineMaps.instance.zoom = 18;
                        OnlineMaps.instance.Redraw();

                        Vector2 vNewPos = new Vector2(vGpsNumCoords.x, vGpsNumCoords.y);
                        Debug.Log("X: " + vNewPos.x + " , " + "Y :" + vNewPos.y);
                        Debug.Log("Queue Size" + mNewPositions.Count);
                        mNewPositions.Enqueue(vNewPos);
                        // OnlineMaps.instance.position = vGpsNumCoords;
                        //OnlineMaps.instance.Redraw();
                        //isDrawing = true;
                    }
                });
        }
        Queue<Vector2> mNextPath = new Queue<Vector2>();
        private IEnumerator CalculatePathPosition()
        {
            while (true)
            {
                if (mNewPositions.Count == 0)
                {
                    yield return null;
                    continue;
                }

                Vector2 vStartPath = OnlineMaps.instance.position;
                Vector2 vEndPosition = mNewPositions.Dequeue();
                float vLastCallTime = Time.time;
                float vPercentage = 0;
                float vDuration = 0;  
                while (vDuration < mTimeToComplete)
                {

                    float vDeltaTime = Time.time - vLastCallTime;
                    var vCurrPos = AnimationHelpers.Hermite(vStartPath, vEndPosition, vPercentage);
                    OnlineMaps.instance.position = vCurrPos;
                    vDuration += vDeltaTime;
                    vLastCallTime = Time.time;
                    OnlineMaps.instance.Redraw();
                    vPercentage = vDuration / mTimeToComplete;
                    if (vPercentage >= 1)
                    {
                        vPercentage = 1;
                    }
                    yield return null;

                }
                yield return  new WaitForSeconds(mTimeToComplete * 0.15f);
            }
        }

        private void BrainpackSelected(BrainpackNetworkingModel vSelected)
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
            SuitConnectionManager.ConnectToSuitControlSocket(vSelected);
        }



        public void RequestDataStream()
        {
            SuitConnectionManager.RequestDataStreamFromBrainpack(1258);
            BrainpackStatusPanel.ConnectToBrainpackButton.gameObject.SetActive(false);
        }

        public void SendReq()
        {
            Packet vPacket = new Packet();
            vPacket.type = PacketType.StatusRequest;
            SuitConnectionManager.SendPacket(vPacket);
        }
        private void BrainpackLostHandler(BrainpackNetworkingModel vBrainpack)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                BrainpackNetworkingModel vBrainpackCopy = new BrainpackNetworkingModel();
                vBrainpackCopy.Id = vBrainpack.Id;
                vBrainpackCopy.Version = vBrainpack.Version;
                vBrainpackCopy.Point = vBrainpack.Point;
                vBrainpackCopy.Status = vBrainpack.Status;
                BrainpackContainerPanel.RemoveBrainpackModel(vBrainpackCopy);
            });

        }

        private void NewBrainpackFound(BrainpackNetworkingModel vBrainpack)
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
                SuitConnectionManager.ConnectToSuitControlSocket(mBrainpack);
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
            if (Input.GetKeyDown(KeyCode.P))
            {
                SendReq();
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                isDrawing = !isDrawing;
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