
/** 
* @file DBSettings.cs
* @brief Contains the DBSettings class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
namespace Assets.Scripts.Utils.DatabaseAccess
{
    /// <summary>
    /// Settings for DBAccess
    /// </summary>
    public static class DBSettings
    {
        public const string DbPassword = "rg;dN+[vt^%5)uJJ";
        public const string DbName = "HeddokoDesktopV0001db.sqlite";
        public const string DbConnectStringNoPw = "URI=file: HeddokoDesktopV0001db.sqlite";
        public const string DbConnectionMapTable = "ConnectionMapping";
        public const string DbSettingsTable = "Settings";

        public const string RecordingsTable = "movements";
        public const string FramesTable = "frames";
        /// <summary>
        /// the table contains information that is extracted from from raw data but not yet converted into actionable
        /// metrics
        /// </summary>
        public const string InstantaneousData = "instantaneous_data";
        /// <summary>
        /// This table refers to unchanged information of a complex equipment and should be stored in
        /// an external system 
        /// </summary>
        public const string RawDataTable = "raw_data";

        public const string RecordingMeta = "movement_meta";
    }
}
