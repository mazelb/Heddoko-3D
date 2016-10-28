// /**
// * @file FirmwareSubPanel.cs
// * @brief Contains the FirmwareSubPanel
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class FirmwareSubPanel : MonoBehaviour
    {
        public DeterminateProgressBar FirmwareProgressBar;
        public Button FirmwareUpdateButton;
        public GameObject VersionUpdatePanel;
        public Text FirmwareUpdateText;
        
        private System.Action mStartFirmwareUpdate;
        internal void Awake()
        {
            FirmwareUpdateButton.onClick.AddListener(FirmwareUpdateAction);
            HideFirmwareUpdate();
        }

        private void FirmwareUpdateAction()
        {
            if (mStartFirmwareUpdate != null)
            {
                mStartFirmwareUpdate();
            }
            HideFirmwareUpdateText();
            FirmwareProgressBar.gameObject.SetActive(true);
            FirmwareProgressBar.Appear(1f, 3f);
        }

        private void HideFirmwareUpdateText()
        {
            FirmwareUpdateText.gameObject.SetActive(false);
            FirmwareUpdateButton.gameObject.SetActive(false);
           
        }


        public void RegisterUpdateAction(System.Action vUpdateCallback)
        {
            mStartFirmwareUpdate = vUpdateCallback;
        }

        public void DisplayFirmwareUpdate()
        {
            VersionUpdatePanel.SetActive(true);
        }

        public void HideFirmwareUpdate()
        {
            FirmwareProgressBar.Clear();
            VersionUpdatePanel.SetActive(false);
        }
    }
}