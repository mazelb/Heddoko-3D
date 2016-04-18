
/** 
* @file LauncherBrainpackSearchResults.cs
* @brief Contains the LauncherBrainpackSearchResults class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.UI.Settings;


namespace Assets.Scripts.Utils.DatabaseAccess
{
    /// <summary>
    /// Results found from brainpack search results
    /// </summary>
    public class LauncherBrainpackSearchResults
    {
        private static Dictionary<string, string> sBrainpackNameToComPort = new Dictionary<string, string>(10);
        private static string sPrefComPort = "";

        public static Dictionary<string, string> BrainpackToComPortMappings
        {
            get { return sBrainpackNameToComPort; }
        }

        /// <summary>
        /// Map results retrieved from 
        /// </summary>
        public static void MapResults()
        {
            LocalDBAccess vDbAccess = new LocalDBAccess();
            sBrainpackNameToComPort = vDbAccess.GetBrainpackResults();
        }

        /// <summary>
        /// Get the prefered com port selected from the launcher
        /// </summary>
        /// <returns>the com port</returns>
        public static string GetPreferedComport()
        {
            LocalDBAccess vDbAccess = new LocalDBAccess();
            string vValue = "";
            if (sBrainpackNameToComPort.Count == 0)
            {
                MapResults();
            }

            //check if any results were found, if not returns an empty string
            if (sBrainpackNameToComPort.Count == 0)
            {
                return String.Empty;
            }

            string vKey = ApplicationSettings.PreferedConnName;
            if (sBrainpackNameToComPort.ContainsKey(vKey))
            {
                vValue = sBrainpackNameToComPort[vKey];
            }
            return vValue;

        }
 
        public static void ResetBrainpackSearchResults()
        {
            sBrainpackNameToComPort = new Dictionary<string, string>(10);
        }
        public static List<string> GetBluetoothDeviceNames()
        {
            return new List<string>(sBrainpackNameToComPort.Keys);
        }
    }
}
 