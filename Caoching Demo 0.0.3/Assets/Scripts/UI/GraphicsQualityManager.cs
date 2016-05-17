// /**
// * @file GraphicsQualityManager.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date May 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using UnityEngine;

namespace Assets.Scripts.UI
{
    public delegate void AvailableGraphicsQualityScanned();
    public delegate void NewGraphicsQualitySettingSelected(string vQualityName);
    /// <summary>
    /// Graphics quality manager: sets the quality of the application graphics
    /// </summary>

    public class GraphicsQualityManager : MonoBehaviour
    {
        private string[] mGraphicsQualitySettings;
        /// <summary>
        /// All quality settings have been scanned
        /// </summary>
        public AvailableGraphicsQualityScanned AvailableGraphicsQualityScannedEvent;

        public NewGraphicsQualitySettingSelected NewGraphicsQualitySettingSelectedEvent;


        internal void SetNewQualitySetting(int x)
        {
            QualitySettings.SetQualityLevel(x);
            if (NewGraphicsQualitySettingSelectedEvent != null)
            {
                NewGraphicsQualitySettingSelectedEvent(mGraphicsQualitySettings[x]);
            }
        }

        public void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes supported graphics settings
        /// </summary>
        public void Initialize()
        {
            mGraphicsQualitySettings = QualitySettings.names;

            if (AvailableGraphicsQualityScannedEvent != null)
            {
                AvailableGraphicsQualityScannedEvent();
            }
        }


    }
}