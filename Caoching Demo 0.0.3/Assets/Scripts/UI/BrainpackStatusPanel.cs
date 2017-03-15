// /**
// * @file BrainpackStatusPanel.cs
// * @brief Contains the BrainpackStatusPanel class. A view for  a Brainpack status panel
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date October 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using Assets.Scripts.Communication.Communicators;
using Assets.Scripts.Localization;
using heddoko;
using HeddokoLib.HeddokoDataStructs.Brainpack;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{

    public class BrainpackStatusPanel : MonoBehaviour
    {
        public FirmwareSubPanel FirmwareSubPanel;
        public Text BrainpackSerialNumberText;
        public Text FirmwareVersionText;
        public Text ChargingStatusText;
        public Text BatteryLevelText;
        public Text BrainpackNetworkingInfo;
        public Text CurrentTcpStateText;
        public Text CurrentBrainpackStateText;
        private BrainpackNetworkingModel mCurrentBrainpack;
        public Text LatestVersionText;
        private Button mConnectToBrainpackButton;
        public Button StartStreamingButton;
        public Text StartStreamText;
        BrainpackState mBrainpackState;
        /// <summary>
        /// Event to request a stream to start.
        /// </summary>
        public event Action<bool> RequestStreamStartEvent;

        /// <summary>
        /// Registers the fimrware sub panel's button to begin the Firmware Update process
        /// </summary>
        /// <param name="vCallbackAction"></param>
        public void RegisterFirmwareSubPanelUpdateCallback(Action vCallbackAction)
        {
            FirmwareSubPanel.RegisterUpdateAction(vCallbackAction);
            StartStreamText.text = "";
            StartStreamingButton.enabled = false;
        }

        void Start()
        {
            Clear();
            StartStreamingButton.onClick.AddListener(SetBrainpackStream);
        }

        private void SetBrainpackStream()
        {
            Debug.Log("Set stream " + Time.time);
            Debug.Log("Clickarooni");
            if (RequestStreamStartEvent != null)
            {
                if (mBrainpackState == BrainpackState.Idle)
                {
                    RequestStreamStartEvent(true);
                }
                else
                {
                    RequestStreamStartEvent(false);
                }
            }
        }

        public void UpdateLatestVersionText(string vText)
        {
            LatestVersionText.text = vText;
        }

        public void UpdateView(Packet vPacket)
        {
            if (vPacket == null)
            {
                BatteryLevelText.text = null;
            }
            else
            {
                ChargingStatusText.text = vPacket.brainpackState.ToString();
                BatteryLevelText.text = vPacket.batteryLevel.ToString();
                FirmwareVersionText.text = vPacket.firmwareVersion;
            }
        }

        public void UpdateView(BrainpackNetworkingModel vBrainpack)
        {
            FirmwareVersionText.text = vBrainpack.Version;
            BrainpackSerialNumberText.text = vBrainpack.Id;
            mCurrentBrainpack = vBrainpack;
            BrainpackNetworkingInfo.text = string.Format("{0} ", vBrainpack.TcpIpEndPoint);
        }

        /// <summary>
        /// Clears the view if the passed in brainpack is the same as the current brainpack
        /// </summary>
        /// <param name="vBrainpack"></param>
        public void ClearIfBrainpack(BrainpackNetworkingModel vBrainpack)
        {
            if (mCurrentBrainpack != null)
            {
                if (vBrainpack.Id == mCurrentBrainpack.Id)
                {
                    Clear();
                }
            }
        }
        public void EnableFirmwareUpdateSubpanel()
        {
            FirmwareSubPanel.DisplayFirmwareUpdate();
        }

        public void Clear()
        {
            FirmwareVersionText.text = "";
            BrainpackSerialNumberText.text = "";
            BatteryLevelText.text = "NO BP";
        }

        /// <summary>
        /// Sets the control state 
        /// </summary>
        /// <param name="vArg1"></param>
        public void SetBrainpackTcpControlState(BrainpackConnectionStateChange vArg1)
        {
            CurrentTcpStateText.text = vArg1.NewState.ToString();
        }
        /// <summary>
        /// update the status of the brainpack panel
        /// </summary>
        /// <param name="vBatteryLevel"></param>
        /// <param name="vBatteryChargeState"></param>
        /// <param name="vBrainpackState"></param>
        public void UpdateStatus(int vBatteryLevel, ChargeState vBatteryChargeState, BrainpackState vBrainpackState)
        {
            Debug.Log("response " + Time.time);
            mBrainpackState = vBrainpackState;
            if (vBatteryChargeState == ChargeState.Charging)
            {
                BatteryLevelText.text = "CHRG";
            }
            else
            {
                BatteryLevelText.text = vBatteryLevel.ToString();
            }
            if (mBrainpackState == BrainpackState.Streaming)
            {
                StartStreamText.text = LocalizationBinderContainer.GetString(KeyMessage.StopStreamingControl);
                StartStreamingButton.enabled = true;
            }
            if (mBrainpackState == BrainpackState.Idle)
            {
                StartStreamText.text = LocalizationBinderContainer.GetString(KeyMessage.StartStreamingControl);
                StartStreamingButton.enabled = true;
            }
            CurrentBrainpackStateText.text = mBrainpackState.ToString();


        }
    }
}