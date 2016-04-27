/** 
* @file ControlPanelRegistry.cs
* @brief a registry for abstract control panel prefab's paths
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections.Generic; 
using Assets.Scripts.UI.AbstractViews.Enums;

namespace Assets.Scripts.UI.AbstractViews
{
    public static class ControlPanelRegistry
    {
        private static Dictionary<ControlPanelType, string> sRegistry = new Dictionary<ControlPanelType, string>();
        private static bool sIsInitialized = false;

        /// <summary>
        /// Initializes registry
        /// </summary>
        private static void Init()
        {
            sIsInitialized = true;
            sRegistry.Add(ControlPanelType.RecordingPlaybackControlPanel, "Prefabs/AbstractControlPanel/PlaybackControls");
            sRegistry.Add(ControlPanelType.DemoKit, "Prefabs/AbstractControlPanel/DemoKitPlayPrefab");
            sRegistry.Add(ControlPanelType.LiveBPFeedView, "Prefabs/AbstractControlPanel/LiveBPFeedView");
        }

        /// <summary>
        /// Requests the file path of the control panel type
        /// </summary>
        /// <param name="vType"></param>
        /// <returns></returns>
        public static string RequestControlPanelPath(ControlPanelType vType)
        {
            string vResult = "";
            if (!sIsInitialized)
            {
                Init();
            }
            if (sRegistry.ContainsKey(vType))
            {
                vResult =  sRegistry[vType];
            }
            return vResult;
        }
    }
}
