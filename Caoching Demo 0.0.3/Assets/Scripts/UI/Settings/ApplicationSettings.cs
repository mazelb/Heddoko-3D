/** 
* @file ApplicationSettings.cs
* @brief Contains the ApplicationSettings class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved

*/
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
        private static string sDownloadCacheFolderPath;
        private static string sBrainpackCacheFolderPath;
        //set it to -1 to  force a registry check
        private static int sCacheSize;
        private static int sTftpPortNum = 8845;
        private const string PREFERED_RECORDINGS_FOLDER_KEY = "PreferedRecordingsFolder";
        /// <summary>
        /// provides and sets the the prefered recordings folder for the application
        /// </summary>
        public static string PreferedRecordingsFolder
        {
            get
            {
                if (string.IsNullOrEmpty(sPreferedRecordingsFolder))
                {
                    //get from registry. 
                    sPreferedRecordingsFolder = PlayerPrefsPathChecker(PREFERED_RECORDINGS_FOLDER_KEY);
                    if (string.IsNullOrEmpty(sPreferedRecordingsFolder))
                    {
                        //create a default directory.
                        DirectoryInfo vDirectoryInfo = new DirectoryInfo(Application.persistentDataPath);
                        if (!Directory.Exists(vDirectoryInfo.Name))
                        {
                            if (vDirectoryInfo.Parent != null)
                            {
                                sPreferedRecordingsFolder = vDirectoryInfo.Parent.ToString() + Path.DirectorySeparatorChar + "DemoRecordings";
                                Directory.CreateDirectory(sPreferedRecordingsFolder);
                                PlayerPrefsPathSetter(PREFERED_RECORDINGS_FOLDER_KEY, sPreferedRecordingsFolder);
                            }
                        }
                    }
                     
                }
                return sPreferedRecordingsFolder;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    DirectoryInfo vDirectoryInfo = new DirectoryInfo(Application.persistentDataPath);
                    if (!Directory.Exists(vDirectoryInfo.Name))
                    {
                        if (vDirectoryInfo.Parent != null)
                        {
                            sPreferedRecordingsFolder = vDirectoryInfo.Parent.ToString() + Path.DirectorySeparatorChar + "DemoRecordings";
                            Directory.CreateDirectory(sPreferedRecordingsFolder);
                            value = sPreferedRecordingsFolder;
                            PlayerPrefsPathSetter(PREFERED_RECORDINGS_FOLDER_KEY, sPreferedRecordingsFolder);
                        }
                    }
                }

                //make sure that this folder exists. else set it to the default application folder
                if (value != null && Directory.Exists(value))
                {
                    sPreferedRecordingsFolder = value;
                    PlayerPrefsPathSetter(PREFERED_RECORDINGS_FOLDER_KEY, sPreferedRecordingsFolder);
                }
                else
                {
                    sPreferedRecordingsFolder = Application.dataPath + "\\DemoRecordings";
                    PlayerPrefsPathSetter(PREFERED_RECORDINGS_FOLDER_KEY, sPreferedRecordingsFolder);
                }
            }
        }

        /// <summary>
        /// Getter  property : returns and sets the cache folder.
        /// </summary>
        public static string DownloadCacheFolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(sDownloadCacheFolderPath))
                {
                    //check player prefs
                    string vVal = Application.persistentDataPath + Path.DirectorySeparatorChar + "RecCache";
                    if (!Directory.Exists(vVal))
                    {
                        Directory.CreateDirectory(vVal);
                    }
                    sDownloadCacheFolderPath = vVal;
                }
                return sDownloadCacheFolderPath;
            }

        }

        /// <summary>
        /// Getter property : returns and sets the cache folder.
        /// </summary>
        public static string BrainpackDownloadCacheFolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(sBrainpackCacheFolderPath))
                {
                    //check player prefs
                    string vVal = Application.persistentDataPath + Path.DirectorySeparatorChar + "BpRecCache";
                    if (!Directory.Exists(vVal))
                    {
                        Directory.CreateDirectory(vVal);
                    }
                    sBrainpackCacheFolderPath = vVal;
                }
                return sBrainpackCacheFolderPath;
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
        /// Checks player preferences. If the item is not found, creates a key registry 
        /// </summary>
        /// <param name="vKey"></param>
        /// <returns></returns>
        private static string PlayerPrefsPathChecker(string vKey)
        {
            return PlayerPrefs.GetString(vKey);
        }

        /// <summary>
        /// Sets a value into player preferences. 
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vValue"></param>
        private static void PlayerPrefsPathSetter(string vKey, string vValue)
        {
            PlayerPrefs.SetString(vKey, vValue);
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

        public static string AnalysisAttributeSettingsFile
        {
            get { return sAnalysisAttributeSettingsFile; }
            set { sAnalysisAttributeSettingsFile = value; }
        }

        /// <summary>
        /// Initializes data paths and returns an array of data paths
        /// </summary>
        /// <returns>array of data paths. Array[0] is the brainpack download cach folder path and Array[1] is the download cache folder path</returns>
        public static string[]  InitializeFilePaths()
        {
            var vBpDlPath = BrainpackDownloadCacheFolderPath;
            var vDlPath = DownloadCacheFolderPath;
            string[] vRet = new string[2];
            vRet[0] = vBpDlPath;
            vRet[1] = vDlPath;
            return vRet;
        }
    }
}
