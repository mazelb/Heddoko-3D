/**
* @file GraphicsQualityManager.cs
* @brief Contains the 
* @author Mohammed Haider( mohammed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

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

        /// <summary>
        /// Sets the vsync count
        /// 0 :  vsync off
        /// 1 :  sync every frame update
        /// 2 :  sync every second frame update
        /// </summary>
        /// <param name="vSyncCount"></param>
        public void SetVsync(int vSyncCount)
        {
            int vCount = vSyncCount;
            if (vCount < 0)
            {
                vCount = 0;
            }
            else if (vCount > 2)
            {
                vCount = 2;
            }
            QualitySettings.vSyncCount = vCount;
        }

        /// <summary>
        /// Get the Vsync count
        /// 0 :  vsync off
        /// 1 :  sync every frame update
        /// 2 :  sync every second frame update
        /// </summary>
        public int GetVsyncCount
        {
            get { return QualitySettings.vSyncCount; }
        }

        /// <summary>
        /// Is Vsync on?
        /// </summary>
        public bool VsyncOn
        {
            get
            {
                return GetVsyncCount == 0;
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

        /// <summary>
        /// Sets the app screen to fullscreen mode depending on the flag
        /// </summary>
        /// <param name="vFlag">the flag to set full screen mode to.</param>
        public void SetToWindowedMode(bool vFlag)
        {
            Screen.fullScreen = vFlag;
        }
    }
}