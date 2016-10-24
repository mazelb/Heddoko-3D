/** 
* @file ApplicationSettings.cs
* @brief Contains the ApplicationSettings class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved

*/


using System;
using System.IO;
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
        private static string sAnalysisAttributeSettingsFile;
        private static bool sAppLaunchedSafely;
        private static string sCacheFolderPath;
        //set it to -1 to  force a registry check
        private static int sCacheSize = 0;
        private static int sTftpPortNum = 8845;
        /// <summary>
        /// provides and sets the the prefered recordings folder for the application
        /// </summary>
        public static string PreferedRecordingsFolder
        {
            get
            {
                if (string.IsNullOrEmpty(sPreferedRecordingsFolder))
                {
                    DirectoryInfo vDirectoryInfo = new DirectoryInfo(Application.dataPath);
                    if (Directory.Exists(vDirectoryInfo.Name))
                    {
                        sPreferedRecordingsFolder = vDirectoryInfo.Parent.ToString() + Path.DirectorySeparatorChar + "DemoRecordings";
                        Directory.CreateDirectory(sPreferedRecordingsFolder);
                    }
                }
                return sPreferedRecordingsFolder;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DirectoryInfo vDirectoryInfo = new DirectoryInfo(Application.dataPath);
                    if (Directory.Exists(vDirectoryInfo.Name))
                    {
                        sPreferedRecordingsFolder = vDirectoryInfo.Parent.ToString() + Path.DirectorySeparatorChar + "DemoRecordings";
                        Directory.CreateDirectory(sPreferedRecordingsFolder);
                        value = sPreferedRecordingsFolder;
                    }
                }

                //make sure that this folder exists. else set it to the default application folder
                if (Directory.Exists(value))
                {
                    sPreferedRecordingsFolder = value;
                }
                else
                {
                    sPreferedRecordingsFolder = Application.dataPath + "\\DemoRecordings";
                }
            }
        }

        /// <summary>
        /// Getter/setter property : returns and sets the cache folder.
        /// </summary>
        public static string CacheFolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(sCacheFolderPath))
                {
                    //check player prefs
                    string vVal = Application.persistentDataPath + Path.DirectorySeparatorChar + "RecCache";
                    if (!Directory.Exists(vVal))
                    {
                        Directory.CreateDirectory(vVal);
                    }
                    sCacheFolderPath = vVal;
                }
                return sCacheFolderPath;
            }
            
        }

        /// <summary>
        /// Port property for the local tftp server 
        /// </summary>
        public static int TftpPort
        {
            get { return sTftpPortNum; }
            set { sTftpPortNum = value; }
        }

        /// <summary>
        /// Getter/setter: returns or sets the recording file cache size in mega bytes
        /// </summary>
        public static int CacheSize
        {
            get
            {
                int vVal = PlayerPrefs.GetInt("RecCacheSize");
                if (vVal == 0)
                {
                    vVal = 250;
                    sCacheSize = vVal;
                    PlayerPrefs.SetInt("RecCacheSize", sCacheSize);
                }
                return sCacheSize;
            }
            set
            {
                if (value <= 0)
                {
                    sCacheSize = 250;
                }
                else
                {
                    sCacheSize = value;
                }
                PlayerPrefs.SetInt("RecCacheSize", sCacheSize);
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

        public static string AttributeFileOrderingPath
        {
            get
            { 
                return Application.persistentDataPath + Path.DirectorySeparatorChar + "AnalysisSettings.txt";
                
            }

        }
    }
}
