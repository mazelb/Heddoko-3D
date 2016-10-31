// /**
// * @file BrainpackStatusPanel.cs
// * @brief Contains the BrainpackStatusPanel class. A view for  a Brainpack status panel
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date October 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
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
        private Brainpack mCurrentBrainpack;
        public Text LatestVersionText;

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

        public void UpdateView(Brainpack vBrainpack)
        {
            FirmwareVersionText.text = vBrainpack.Version;
            BrainpackSerialNumberText.text = vBrainpack.Id;
            mCurrentBrainpack = vBrainpack;
        }

        /// <summary>
        /// Clears the view if the passed in brainpack is the same as the current brainpack
        /// </summary>
        /// <param name="vBrainpack"></param>
        public void ClearIfBrainpack(Brainpack vBrainpack)
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
    }
}