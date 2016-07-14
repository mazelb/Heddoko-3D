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
using System.Text.RegularExpressions;
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

        public static string CamelCaseToSpaces(string vInput)
        {
            var vReturn = Regex.Replace(vInput, "(\\B[A-Z])", " $1");
            return vReturn;
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
            using (StreamWriter vFileOut = new StreamWriter(vPath))
            {
                //write the header
                vFileOut.Write("Timestamp,");
                foreach (var vKey in vAnalysisDataStore.Storage.Values)
                {
                    foreach (var vFieldInfo in vKey)
                    {
                        var vCustomAttribute = vFieldInfo.Key.GetCustomAttributes(typeof(AnalysisSerialization), true);
                        foreach (var vAttri in vCustomAttribute)
                        {
                            vFileOut.Write(((AnalysisSerialization)vAttri).AttributeName + ",");
                        }
                    }
                }
                vFileOut.Write("\r\n");
                //write the body
                for (int i = 0; i<vAnalysisDataStore.SerializedList.Count; i++)
                {
                    vFileOut.Write(vAnalysisDataStore.TimeStamps[i] + ",");
                    foreach (var vSerializableIterable in vAnalysisDataStore.SerializedList[i])
                    {
                        vFileOut.Write(vSerializableIterable.Value+ ",");
                    }
                    vFileOut.Write("\r\n");
                }
                
            }
        }

  

       
    }
}