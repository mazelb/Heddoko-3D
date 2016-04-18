/** 
* @file DebugLogSettings.cs
* @brief Contains the DebugLogSettings  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Runtime.Serialization;

namespace Assets.Scripts.Utils.DebugContext.logging
{
    /// <summary>
    /// Settings for debugging
    /// </summary>
    [DataContract]
    public class DebugLogSettings : IExtensibleDataObject
    {
        [DataMember]
        public bool LogAll = true;
        [DataMember]
        public bool LogAllBrainpackContext = true;
        [DataMember]
        public bool LogAllApplicationContext = true;
        [DataMember]
        public bool BrainpackCommandLog = true;
        [DataMember]
        public bool BrainpackResponseLog = true;
        [DataMember]
        public bool BrainpackFrameData = true;
        [DataMember]
        public bool ApplicationCommandLog = true;
        [DataMember]
        public bool ApplicationResponseLog = true;
        [DataMember]
        public bool ApplicationFrameData = true;

        [DataMember]
        public bool SocketClientLogging = true;

        [DataMember]
        public long MaxFileSizeMb = 250;

        public ExtensionDataObject ExtensionData { get; set; }

        public void AllFalse()
        {
            LogAll = LogAllBrainpackContext = LogAllApplicationContext = BrainpackCommandLog =
                BrainpackResponseLog = BrainpackFrameData = ApplicationCommandLog =
                    ApplicationResponseLog = ApplicationFrameData = SocketClientLogging = false;
        }

        /// <summary>
        /// The max file size
        /// </summary>
        /// <returns></returns>
        private double GetMaxFileSize()
        {
            if (MaxFileSizeMb < 5)
            {
                MaxFileSizeMb = 5;
            }
            return MaxFileSizeMb;
        }
    }
}
