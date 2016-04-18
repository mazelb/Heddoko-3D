/** 
* @file DebugLogSettings.cs
* @brief Contains the DebugLogSettings  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Runtime.Serialization;

namespace BrainpackService.Tools_and_Utilities.Debugging
{
    /// <summary>
    /// Settings for debugging
    /// </summary>
    [DataContract]
  public  class DebugLogSettings: IExtensibleDataObject
    {
        [DataMember ]
        public bool LogAll;
        [DataMember]
        public bool LogAllBrainpackContext;
        [DataMember]
        public bool LogAllApplicationContext;
        [DataMember]
        public bool BrainpackCommandLog;
        [DataMember]
        public bool BrainpackResponseLog;
        [DataMember]
        public bool BrainpackFrameData;
        [DataMember]
        public bool ApplicationCommandLog;
        [DataMember]
        public bool ApplicationResponseLog;
        [DataMember]
        public bool ApplicationFrameData;
        [DataMember]
        public long MaxFileSizeMb = 250;

        public ExtensionDataObject ExtensionData { get; set; }

        public void AllFalse()
        {
            LogAll = LogAllBrainpackContext = LogAllApplicationContext = BrainpackCommandLog =
                BrainpackResponseLog = BrainpackFrameData = ApplicationCommandLog =
                    ApplicationResponseLog = ApplicationFrameData = false;

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
