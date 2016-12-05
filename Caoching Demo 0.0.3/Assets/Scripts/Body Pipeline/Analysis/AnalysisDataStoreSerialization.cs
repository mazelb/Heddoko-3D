/**
* @file AnalysisDataStoreSerialization.cs
* @brief Contains the AnalysisDataStoreSerialization
* @author Mohammed Haider( mohammed@heddoko.com)
* @date  June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis
{

    /// <summary>
    /// Serializes data to a file
    /// </summary>
    public class AnalysisDataStoreSerialization
    {
        private AnalysisDataStore mDataStore;
        private static string sPath = "";
        public static string GetSerializationStorePath
        {
            get
            {
                string vPath = "";
                if (string.IsNullOrEmpty(sPath))
                {
                    string vTodaysDate = DateTime.Now.ToString("hh-mm-ss-ff");
                    vPath = Application.persistentDataPath;
                    vPath += Path.DirectorySeparatorChar + vTodaysDate + ".csv";
                }
                return vPath;
            }
        }

        public AnalysisDataStoreSerialization(AnalysisDataStore vDataStore)
        {
            mDataStore = vDataStore;
        }


        /// <summary>
        /// Writes the data store to a csv file
        /// </summary>
        public static void WriteFile(AnalysisDataStore vAnalysisDataStore)
        {
            WriteFile(vAnalysisDataStore, GetSerializationStorePath);
        }

        /// <summary>
        /// Writes the data store to a csv file specified by vPath
        /// </summary>
        /// <param name="vAnalysisDataStore">the data store to serialize</param>
        /// <param name="vPath">the path to save to </param>
        public static void WriteFile(AnalysisDataStore vAnalysisDataStore, string vPath)
        {
            List<string> vToBeWritten = new List<string>();
            //prepare the data store to serialize data
            vAnalysisDataStore.PrepareDataStore();
            using (StreamWriter vFileOut = new StreamWriter(vPath))
            {
                //write the header
                vFileOut.Write("Frame Index,");
                vFileOut.Write("TPose Value,");
                vFileOut.Write("Timestamp,");
                List<FieldInfo> vSortedList = new List<FieldInfo>();

                foreach (var vAnalysisFieldDataStructures in vAnalysisDataStore.Storage.Values)
                {
                    foreach (var vKvPair in vAnalysisFieldDataStructures)
                    {
                        vSortedList.Add(vKvPair.Key);
                    }
                }

                //sort it 
                vSortedList.Sort((vX, vY) => vAnalysisDataStore.AnaylsisDataStoreSettings.GetOrderOfAnalysisField(vX).CompareTo(
                   vAnalysisDataStore.AnaylsisDataStoreSettings.GetOrderOfAnalysisField(vY)));
                foreach (var vFieldInfo in vSortedList)
                {
                    var vItem =
                        vAnalysisDataStore.AnaylsisDataStoreSettings.GetAnalysisSerializationItem(vFieldInfo.ToString());
                    vFileOut.Write(vItem.AttributeName + ",");
                }
                vFileOut.Write("\r\n");

                //When a tpose is initiated, update tracking is called twice. This triggers a data collection event twice on the same frame
                // the previous non tposed frame is marked for removal. 
                int vPrevIndex = -1;
                bool vPrevFrameFlaggedForRemoval = false;
                //write the body
                for (int i = 0; i < vAnalysisDataStore.SerializedList.Count; i++)
                {
                    StringBuilder vOut = new StringBuilder();
                    //write frame index
                    var vFrameIndex = vAnalysisDataStore.FrameIndices[i];
                    if (vFrameIndex == vPrevIndex)
                    {
                        vPrevFrameFlaggedForRemoval = true;
                    }

                    vPrevIndex = vFrameIndex;
                    vOut.Append(vFrameIndex + ",");
                    vOut.Append(vAnalysisDataStore.PoseSelectionIndicies[vFrameIndex] + ",");
                    vOut.Append(vAnalysisDataStore.TimeStamps[i] + ",");

                    var vSerializedList = vAnalysisDataStore.SerializedList[i];
                    foreach (var vItem in vSortedList)
                    {
                        if ((vSerializedList.ContainsKey(vItem)))
                        {
                            var vSerializedItem = vSerializedList[vItem];
                            vOut.Append(vSerializedList[vItem] + ",");
                        }
                        else
                        {
                            Debug.Log("already have key " + vItem);
                        }

                    }

                    vOut.AppendLine();
                    vToBeWritten.Add(vOut.ToString());
                    if (vPrevFrameFlaggedForRemoval)
                    {
                        vPrevFrameFlaggedForRemoval = false;
                    }
                }
                foreach (var vLine in vToBeWritten)
                {
                    vFileOut.Write(vLine);
                }
            }
            
            // associated raw data output
            if (vPath.Contains(".csv"))
            {
                int vIndex = vPath.IndexOf(".csv");
                vPath = vPath.Remove(vIndex, 4);
            }
            var vFileInfo = new FileInfo(vPath);
            if (vFileInfo != null)
            {

                var vRawDataPath = vFileInfo.Directory.ToString() + Path.DirectorySeparatorChar + vFileInfo.Name + "RawData.csv";
                using (StreamWriter vFileWriter = new StreamWriter(vRawDataPath))
                {
                    vFileWriter.Write("Frame Index,");

                    //get first frame of raw data and enumerate its orientation components
                    var vRawData = vAnalysisDataStore.BodyFrames;
                    var vFirstRaw = vRawData[0];
                    for (int i = 0; i < vFirstRaw.FrameData.Count; i++)
                    {
                        vFileWriter.Write(i + "x,");
                        vFileWriter.Write(i + "y,");
                        vFileWriter.Write(i + "z,");
                        vFileWriter.Write(i + "w,");
                    }
                    vFileWriter.Write("\r\n");
                    for (int i = 0; i < vAnalysisDataStore.SerializedList.Count; i++)
                    {
                        vFileWriter.Write(vRawData[i].Index + ",");
                        vFileWriter.Write(vRawData[i].ToCsvnoTsNoKeyIncluded());
                        vFileWriter.Write("\r\n");
                    }

                }
            }


        }




    }
}