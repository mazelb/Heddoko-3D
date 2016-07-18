/**
* @file PlaybackSettings.cs
* @brief Contains the PlaybackKeyboardSettingsClass
* @author Mohammed Haider( mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using UnityEngine;
 
namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording
{
    public class PlaybackSettings
    {
        public float MaxForwardSpeed = 5f;
        public float MaxBackSpeed = -5f;
        public float CurrForwardSpeed = 1f;
        public float CurrBackSpeed = -1f;
       public float  PlaybackSpeed = 1f;
        /// <summary>
        /// Loops through the current recording
        /// </summary>
        public bool IsLooping = true;

        public int ForwardFrameSkip =1;
        public int BackwardFrameSkip = 1;

        private static int mFrameSkip = 10;

        public static int FrameSkip
        {
            get { return mFrameSkip*FrameSkipMultiplier; }
            set { mFrameSkip = value; }
        }
        public static int FrameSkipMultiplier = 1;
    }
}