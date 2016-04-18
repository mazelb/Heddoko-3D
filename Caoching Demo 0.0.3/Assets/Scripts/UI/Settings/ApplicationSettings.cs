/** 
* @file ApplicationSettings.cs
* @brief Contains the ApplicationSettings class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved

*/


using Assets.Scripts.Utils;
using Assets.Scripts.Utils.DatabaseAccess;
using UnityEngine;

namespace Assets.Scripts.UI.Settings
{
    /// <summary>
    /// Static settings for the application
    /// </summary>
    public static class ApplicationSettings
    {
        private static string sPreferedRecordingsFolder;
        private static string sPreferedConnName;
        private static int sResWidth;
        private static int sResHeight;
        private static bool sAppLaunchedSafely;
        /// <summary>
        /// provides and sets the the prefered recordings folder for the application
        /// </summary>
        public static string PreferedRecordingsFolder
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath + "/Resources/Recordings";
#endif
                return sPreferedRecordingsFolder;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    sPreferedRecordingsFolder = FilePathReferences.RecordingsDirectory;
                    value = sPreferedRecordingsFolder.Replace("\\", "/");
                }

                value = value.Replace("\\", "/");
                sPreferedRecordingsFolder = value;
            }
        }

        /// <summary>
        /// Prefered connection name
        /// </summary>
        public static string PreferedConnName
        {
            get { return sPreferedConnName; }
            set { sPreferedConnName = value; }
        }

        /// <summary>
        /// Returns the prefered resolution width
        /// </summary>
        public static int ResWidth
        {
            get { return sResWidth; }
            set { sResWidth = value; }
        }

        /// <summary>
        /// Returns the prefered resolution height
        /// </summary>
        public static int ResHeight
        {
            get { return sResHeight; }
            set { sResHeight = value; }
        }

        public static bool AppLaunchedSafely
        {
            get { return sAppLaunchedSafely; }
            set { sAppLaunchedSafely = value; }
        }

        public static string LocalDbPath
        {
            get
            {
                string vLocalDbPath = Application.persistentDataPath + "/db/" + DBSettings.DbName;
          
                return vLocalDbPath;
            }
        }
    }
}
