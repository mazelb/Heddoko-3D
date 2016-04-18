/** 
* @file SettingsModel.cs
* @brief Contains the SettingsModel class
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Newtonsoft.Json;


namespace Assets.Scripts.UI.Settings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SettingsModel
    {

        private static string sComPort = "COM";
        [JsonProperty]
        private static int sComportNum;

        /// <summary>
        /// The RecordingsFolder
        /// </summary>
        [JsonProperty]
        public static string RecordingsFolder;

        /// <summary>
        /// Returns a comport in the form of "COM"+vComport
        /// </summary>
        /// <param name="vComport"></param>
        public string SetComport(int vComport)
        {
            sComportNum = vComport;
            return sComPort + sComportNum;
        }
    }
}
