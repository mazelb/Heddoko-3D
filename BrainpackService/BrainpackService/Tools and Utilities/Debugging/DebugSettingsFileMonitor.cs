/** 
* @file DebugSettingsFileMonitor.cs
* @brief Contains the DebugSettingsFileMonitor  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;
namespace BrainpackService.Tools_and_Utilities.Debugging
{
    /// <summary>
    /// Monitors changes on the debug file located in ./Settings
    /// </summary>

    public class DebugSettingsFileMonitor
    {
        //the poll timer in ms
        //30 seconds
        public int PollTimer { get; set; } = 30000; 
        private FileStream mSettingLogFileStream;
        public const string SettingsFileName = "LogSettings.txt";
        public bool ContinueWorking { get; set; } = true;
        private string CurrentDirectory
        {
            get
            {
                var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                var directoryPath = Path.GetDirectoryName(location);
                return directoryPath;
            }
        }

        private string SettingsDirectory
        {
            get { return CurrentDirectory + "\\Settings\\"; }
        }

        private string SettingsFileDirectory
        {
            get { return SettingsDirectory + "\\" + SettingsFileName; }
        }

        private void ReadObject()
        {


            //check if directory exists
            if (!Directory.Exists(SettingsDirectory))
            {
                DebugLogger.Settings.AllFalse();
                return;
            }
            //check if file exists
            if (!File.Exists(SettingsFileDirectory))
            {
                DebugLogger.Settings.AllFalse();
                return;
            }

            mSettingLogFileStream = new FileStream(SettingsFileDirectory, FileMode.OpenOrCreate);
            XmlDictionaryReader vXmlReader =
                XmlDictionaryReader.CreateTextReader(mSettingLogFileStream, new XmlDictionaryReaderQuotas());

            // Create the DataContractSerializer instance.
            DataContractSerializer vSer =
                new DataContractSerializer(typeof(DebugLogSettings));

            // Deserialize the data and read it from the instance.
            DebugLogSettings vNewSettings = (DebugLogSettings)vSer.ReadObject(vXmlReader);
            mSettingLogFileStream.Close();
            DebugLogger.Settings = vNewSettings;
        }


        private void Workerfunction()
        { 
            while (ContinueWorking)
            {
                try
                {
                    ReadObject();
                }
                catch
                {
                    DebugLogger.Settings.AllFalse();
                }
                Thread.Sleep(PollTimer);
              
            }
        }

        public void Start()
        {
            Thread vThread = new Thread(Workerfunction);
            ContinueWorking = true;
            vThread.Start();
        }


        public void Stop()
        {
            ContinueWorking = false;
        }
    }
}