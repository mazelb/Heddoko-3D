
/** 
* @file DBAccess.cs
* @brief Contains the DBAccess class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using Mono.Data.Sqlite; 
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.UI.Loading;
using Assets.Scripts.UI.ModalWindow;
using Assets.Scripts.UI.Settings;
using UnityEngine;

namespace Assets.Scripts.Utils.DatabaseAccess
{
    /// <summary>
    /// Provides a connection to a local database, whose parameters are set in a hard coded settings file
    /// </summary>
    public class LocalDBAccess
    {
        private SqliteConnection mDbConnection;

        /// <summary>
        /// Connects to the database
        /// </summary>
        internal bool Connect()
        {
            if (mDbConnection == null)
            {
                //check if the file exists first. 
                string vFilePath = ApplicationSettings.LocalDbPath;
              
                if (File.Exists(vFilePath))
                {
                    mDbConnection = new SqliteConnection("URI=file:" + vFilePath);
                    mDbConnection.Open();
                    return true;
                }
                else
                {
                    ModalPanel.SingleChoice("Could not locate the settings database. Please restart the application" +
                                            " through the launcher.", Application.Quit);
                    LoadingBoard.StopLoadingAnimation();
                    return false;
                }
            }
            return false;

        }


        /// <summary>
        /// Get Brainpack results found in the launcher
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, string> GetBrainpackResults()
        {
            bool vConnectionSuccess = Connect();
            if (vConnectionSuccess)
            {
                Dictionary<string, string> vBpResults = new Dictionary<string, string>(0);
                string vSqlCmdStr = "SELECT * " + "FROM " + DBSettings.DbConnectionMapTable;
                SqliteCommand vCmd = mDbConnection.CreateCommand();
                vCmd.CommandText = vSqlCmdStr;
                SqliteDataReader vDataReader = vCmd.ExecuteReader();

                //map results
                while (vDataReader.Read())
                {
                    string vKey = string.Empty;
                    string vVal = string.Empty;
                    object vKeyCheck = vDataReader.GetString(0);
                    object vValCheck = vDataReader.GetString(1);

                    if (vKeyCheck != null && vKeyCheck is string)
                    {
                        vKey = (string)vKeyCheck;
                    }

                    if (vValCheck != null && vValCheck is string)
                    {
                        vVal = (string)vValCheck;
                    }

                    if (!vBpResults.ContainsKey(vKey) && !string.IsNullOrEmpty(vKey))
                    {
                        vBpResults.Add(vKey, vVal);
                    }
                }
                mDbConnection.Close();
                return vBpResults;
            }
            return null;
        }

        /// <summary>
        /// Sets the application settings from the Settings table in the database
        /// </summary>
        /// <returns></returns>
        internal bool SetApplicationSettings()
        {
            //open connection
            if (Connect())
            {
                string vStrCmd = "SELECT * FROM " + DBSettings.DbSettingsTable;
                SqliteCommand vCmd = mDbConnection.CreateCommand();
                vCmd.CommandText = vStrCmd;
                SqliteDataReader vDataReader = vCmd.ExecuteReader();
                while (vDataReader.Read())
                {
                    object vResWidthCheck = vDataReader.GetInt32(0);
                    object vResHeightCheck = vDataReader.GetInt32(1);
                    object vRecStrCheck = vDataReader.GetString(2);
                    object vPrfConnCheck = vDataReader.GetString(3);
                    object vLaunchStart = vDataReader.GetBoolean(4);

                    if (vResWidthCheck is int)
                    {
                        ApplicationSettings.ResWidth = (int)vResWidthCheck;
                    }

                    if (vResHeightCheck is int)
                    {
                        ApplicationSettings.ResHeight = (int)vResHeightCheck;
                    }

                    if (vRecStrCheck != null && vRecStrCheck is string)
                    {
                        ApplicationSettings.PreferedRecordingsFolder = (string)vRecStrCheck;
                    }

                    if (vPrfConnCheck != null && vPrfConnCheck is string)
                    {
                        ApplicationSettings.PreferedConnName = (string)vPrfConnCheck;
                    }

                    if (vPrfConnCheck != null && vLaunchStart is bool)
                    {
                        ApplicationSettings.AppLaunchedSafely = (bool)vLaunchStart;
                    }

                    //break after the first read
                    break;
                }
                mDbConnection.Close();
                return true;
            }
            return false;
        }

    }
}
