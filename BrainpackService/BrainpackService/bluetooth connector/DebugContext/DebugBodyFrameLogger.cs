
/** 
* @file DebugBodyFrameLogger.cs
* @brief Contains the DebugBodyFrameLogger class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/


using System;
using System.IO;

namespace BrainpackService.bluetooth_connector.DebugContext
{
    /// <summary>
    /// Class used to log files for debugging purposes with respect to how fast a module is handling a Bodyframe(rendering or converting). Calculates averages
    /// </summary>
    public class DebugBodyFrameLogger
    {
        //Log count of the current day
        private int mCurrentDayLogCount; 
        private string mLogType;

        /// <summary>
        /// Constructor that takes in a string which denotes the type of log that we need to view
        /// </summary>
        /// <param name="vLogType"></param>
        public DebugBodyFrameLogger(string vLogType)
        {
            mLogType = vLogType;
        }

        /// <summary>
        /// Writes a log file
        /// </summary>
        /// <param name="vTimeTaken">Total time taken to process or handle a bodyframe entry</param>
        /// <param name="vLogEntry">The log entry</param> 
        public void WriteLog(double vTimeTaken, string vLogEntry)
        { 
            string vSubPath = Directory.GetCurrentDirectory() + "/Logs/" + mLogType + mCurrentDayLogCount;
            bool vPathExists = Directory.Exists(vSubPath);
            if (!vPathExists)
            {
                Directory.CreateDirectory(vSubPath);
            }

            //file name
 
            string vPath = vSubPath + "/" + "_" + mLogType + DateTime.Now.ToString(@"MM-dd-yyyy") + ".csv";
            string vLogMessage = DateTime.Now.Ticks + ","+vTimeTaken + "," + vLogEntry + "\r\n";
            //check if file exists
            bool vFileExists = File.Exists(vPath);
            if (!vFileExists)
            {
                using (StreamWriter vSw = File.CreateText(vPath))
                {
                    vSw.Write(vLogMessage);
                }

            }
            else
            {
                //check the file size first, if its over 250 mb, then don't increment the log count and recursively call the function again.
                FileInfo vFileInfo = new FileInfo(vPath);
                if (vFileInfo.Length > 250000000)
                {
                    mCurrentDayLogCount++;
                    WriteLog(vTimeTaken, vLogEntry);
                    return;
                }
                using (StreamWriter vSw = File.AppendText(vPath))
                {
                    vSw.Write(vLogMessage);
                }
            }
        }


    }
}
