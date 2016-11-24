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
        private BrainpackNetworkingModel mCurrentBrainpack;
        public Text LatestVersionText;
        public Button ConnectToBrainpackButton;
        public Button StartStreamingButton;
        public Text StartStreamText;
        private bool StreamEnabled;
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
        }

        void Start()
        {
            Clear();
            StartStreamingButton.onClick.AddListener(SetBrainpackStream);
        }

        private void SetBrainpackStream()
        {
            StreamEnabled = !StreamEnabled;
            if (RequestStreamStartEvent != null)
            {
                RequestStreamStartEvent(StreamEnabled);
            }
            if (StreamEnabled)
            {
                StartStreamText.text = LocalizationBinderContainer.GetString(KeyMessage.StartStreamingControl);
            }
            else
            {
                StartStreamText.text = LocalizationBinderContainer.GetString(KeyMessage.StopStreamingControl);

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
            if (vArg1.NewState == BrainpackConnectionState.Connected)
            {
                StartStreamingButton.enabled = true;
                StartStreamText.text = LocalizationBinderContainer.GetString(KeyMessage.StartStreamingControl);
            }
            else if (vArg1.NewState == BrainpackConnectionState.Disconnected)
            {
                StartStreamingButton.enabled = false;
                StartStreamText.text = LocalizationBinderContainer.GetString(KeyMessage.BrainpackDisconnected);
            }
            else if (vArg1.NewState == BrainpackConnectionState.Connecting)
            {
                StartStreamingButton.enabled = false;
                StartStreamText.text = LocalizationBinderContainer.GetString(KeyMessage.BrainpackConnecting);
            }
        }
    }
}