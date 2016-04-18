/** 
* @file FolderScanner.cs
* @brief Contains the FolderScanner class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors;
using Assets.Scripts.Utils.DebugContext.logging;
using UnityEngine;
using LogType = Assets.Scripts.Utils.DebugContext.logging.LogType;

namespace Assets.Scripts.UI.AbstractViews.ContextSpecificContainers
{
    public delegate void OnDirectoryScanCompletion(List<RecordingItemDescriptor> vItems);
    /// <summary>
    /// A helper class that maintains a list of scanned directories 
    /// </summary>
    public class FolderScanner
    {
        private List<string> mScannedDirectories = new List<string>();
        public event OnDirectoryScanCompletion DirectoryScanCompleted;

        /// <summary>
        /// After an open folder dialog has been selected, find all the files in this directory that are recordings
        /// </summary>
        /// <param name="vFolder"></param>
        public void ScanDirectory(string vFolder)
        {
            if (mScannedDirectories.Contains(vFolder))
            {
                Debug.Log("folder already selected. Cancelling  ");
                return;
            }
            mScannedDirectories.Add(vFolder);
            List<RecordingItemDescriptor> vItemDescriptors = new List<RecordingItemDescriptor>();
            //add a check to prevent duplicates
            BodyRecordingsMgr.Instance.ScanRecordings(vFolder);
            string[] vPaths = BodyRecordingsMgr.Instance.FilePaths;
            Debug.Log("Begin loading ");
            //the first line of a  file
            byte[] vStartOfFileBuffer = new byte[176];
            //the final line of a file
            byte[] vEndOfFileBuffer = new byte[176];
            List<string> vProblematicPaths = new List<string>();
            List<string> vErrors = new List<string>();
            foreach (var vPath in vPaths)
            {
                FileInfo vInfo = new FileInfo(vPath);
                //skip any file that is less than 352 bytes
                if (vInfo.Length < (352))
                {
                    continue;
                }
                try
                {
                    int vStartTime = 0;
                    int vEndTime = 0;
                    int vTotalTime = 0;
                    RecordingItemDescriptor vDescriptor = new RecordingItemDescriptor();
                    using (FileStream fs = new FileStream(vPath, FileMode.Open, FileAccess.Read))
                    {
                        fs.Read(vStartOfFileBuffer, 0, vStartOfFileBuffer.Length);
                        //The first 8 bytes of the buffer is the start time stamp. Convert this to string, then int
                        byte[] vShortByte = vStartOfFileBuffer.Take(10).ToArray();
                        string vTemp = Encoding.ASCII.GetString(vShortByte);
                        vStartTime = Convert.ToInt32(vTemp);
                        fs.Close();
                    }
                    using (FileStream fs = new FileStream(vPath, FileMode.Open, FileAccess.Read))
                    {
                        fs.Seek(vInfo.Length - vEndOfFileBuffer.Length - 2, SeekOrigin.Begin);
                        fs.Read(vEndOfFileBuffer, 0, vEndOfFileBuffer.Length);
                        byte[] vShortByte = vEndOfFileBuffer.Take(10).ToArray();
                        string vTemp = Encoding.ASCII.GetString(vShortByte);
                        vEndTime = Convert.ToInt32(vTemp);
                        fs.Close();
                    }
                    vTotalTime = vEndTime - vStartTime;
                    vTotalTime /= 1000;
                    vDescriptor.CreatedAtTime = vInfo.CreationTime;
                    vDescriptor.MovementTitle = Path.GetFileNameWithoutExtension(vInfo.Name);
                    vDescriptor.RecordingDuration = vTotalTime;
                    vDescriptor.FilePath = vPath;
                    vItemDescriptors.Add(vDescriptor);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.Log(ex.Message);
                }
                catch (IOException vE)
                {
                    vProblematicPaths.Add(vPath);
                    DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "IO Exception with file "+ vPath +" Error: " +vE);
                }
                catch (FormatException vE)
                {
                    vProblematicPaths.Add(vPath);
                    DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "Format Exception with file " + vPath + " Error: " + vE);

                }
            }

            Debug.Log("Stop loading");
            if (DirectoryScanCompleted != null)
            {
                DirectoryScanCompleted.Invoke(vItemDescriptors);
            }
        }
    }
}
