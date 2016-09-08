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
                string vPath="";
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
        public static void WriteFile (AnalysisDataStore vAnalysisDataStore,string vPath)
        {
            //prepare the data store to serialize data
            vAnalysisDataStore.PrepareDataStore();
            using (StreamWriter vFileOut = new StreamWriter(vPath))
            {
                //write the header
                vFileOut.Write("Frame Index,");
                vFileOut.Write("TPose Value,");
                // vFileOut.Write("Calibration Type");
                vFileOut.Write("Timestamp,");
                List<FieldInfo>  vSortedList = new List<FieldInfo>();
                foreach (var vAnalysisFieldDataStructures in vAnalysisDataStore.Storage.Values)
                {
                    foreach (var vKvPair in vAnalysisFieldDataStructures)
                    {
                        vSortedList.Add(vKvPair.Key);
                    }
                  
                } 
                //sort it 
                vSortedList.Sort((vX,vY)=> vAnalysisDataStore.AnaylsisDataStoreSettings.GetOrderOfAnalysisField(vX).CompareTo(
                   vAnalysisDataStore.AnaylsisDataStoreSettings.GetOrderOfAnalysisField(vY)));
                foreach (var vFieldInfo in vSortedList)
                {
                    var vItem =
                        vAnalysisDataStore.AnaylsisDataStoreSettings.GetAnalysisSerializationItem(vFieldInfo.ToString());
                    vFileOut.Write(vItem.AttributeName + ","); 
                }
                vFileOut.Write("\r\n");
                //write the body
                for (int i = 0; i<vAnalysisDataStore.SerializedList.Count; i++)
                {
                    //write frame index
                    var vFrameIndex = vAnalysisDataStore.FrameIndices[i];
                    vFileOut.Write(vFrameIndex + ",");
                    //write tpose value at the given frame index
                    vFileOut.Write(vAnalysisDataStore.PoseSelectionIndicies[vFrameIndex] +",");
                    //Write timestamp
                    vFileOut.Write(vAnalysisDataStore.TimeStamps[i] + ",");
                    var vSerializedList = vAnalysisDataStore.SerializedList[i];
                    foreach (var vItem in vSortedList)
                    {
                        if((vSerializedList.ContainsKey(vItem)))
                        {
                            var vSerializedItem = vSerializedList[vItem];
                            vFileOut.Write(vSerializedList[vItem] + ",");
                        }
                        else
                        {
                            Debug.Log("already have key "+ vItem);
                        }
                        
                    }
                    vFileOut.Write("\r\n");
                }
                
            }
        }

  

       
    }
}