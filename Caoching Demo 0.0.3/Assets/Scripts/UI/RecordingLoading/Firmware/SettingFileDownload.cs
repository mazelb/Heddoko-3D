// /**
// * @file SettingFileDownload.cs
// * @brief Contains the SettingFileDownload class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 09 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.MainApp;
using JetBrains.Annotations;

namespace Assets.Scripts.UI.RecordingLoading.Firmware
{
    public class SettingFileDownload
    {
        public HeddokoSdCardSearcher Searcher;
        public UserProfileModel UserProfileModel;
        public SettingFileDownload(HeddokoSdCardSearcher vSearcher, [NotNull]UserProfileModel vModel)
        {
            if (vModel == null)
            {
                throw new NullReferenceException();
            }
            Searcher = vSearcher;
            UserProfileModel = vModel;
        }

        /// <summary>
        /// update and over the settings file on the sd card. <remarks>Will throw a<exception cref="SdCardNotInsertedException"></exception> if the sd card is not inserted</remarks>
        /// </summary>
        public void UpdateSettingsFile()
        {
            if (!Searcher.SdCardIsConnected)
            {
                throw new SdCardNotInsertedException();
            }
            //get the brainpack serial
            var vSerial = Searcher.GetSerialNumFromSdCard();
            //download the file to cache
              
        }
    }

    /// <summary>
    /// The sd card isn't inserted
    /// </summary>
    public class SdCardNotInsertedException : Exception
    {
        
    }
}