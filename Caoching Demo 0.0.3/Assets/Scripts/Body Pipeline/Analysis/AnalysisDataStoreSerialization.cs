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
        private string mPath = "";
        public string GetSerializationStorePath
        {
            get
            {
                string vPath="";
                if (string.IsNullOrEmpty(mPath))
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
         public void WriteFile(AnalysisDataStore vAnalysisDataStore)
        {
            using (StreamWriter vFileOut = new StreamWriter(GetSerializationStorePath))
            {
                //write the header
                vFileOut.Write("Timestamp,");
                foreach (var vKey in vAnalysisDataStore.Storage.Values)
                {
                    foreach (var vFieldInfo in vKey)
                    {
                        var vCustomAttribute = vFieldInfo.Key.GetCustomAttributes(typeof(AnalysisAttribute), true);
                        foreach (var vAttri in vCustomAttribute)
                        {
                            vFileOut.Write(((AnalysisAttribute)vAttri).AttributeName + ",");
                        }
                    }
                }
                vFileOut.Write("\r\n");
                //write the body
                for (int i = 0; i < vAnalysisDataStore.mSerializedList.Count; i++)
                {
                    vFileOut.Write(vAnalysisDataStore.TimeStamps[i] + ",");
                    foreach (var vSerializableIterable in vAnalysisDataStore.mSerializedList[i])
                    {
                        vFileOut.Write(vSerializableIterable.Value+ ",");
                    }
                    vFileOut.Write("\r\n");
                }
                
            }
             
        }

        public string GetString(AnalysisDataStore vAnalysisDataStore)
        {
            //set header 
            List<StringBuilder> vBuilders = new List<StringBuilder>();
            StringBuilder vHeaderBuilder = new StringBuilder();
             vHeaderBuilder.Append("Timestamp, ");
            
            foreach (var vKey in vAnalysisDataStore.Storage.Values)
            { 
                foreach (var vFieldInfo in vKey)
                {
                    var vCustomAttribute = vFieldInfo.Key.GetCustomAttributes(typeof(AnalysisAttribute), true);
                    foreach (var vAttri in vCustomAttribute)
                    {
                        vHeaderBuilder.Append(((AnalysisAttribute)vAttri).AttributeName + ", ");
                        //vBuilder.Append(AnalysisDataStoreSerialization.CamelCaseToSpaces(vFieldInfo.Key.Name) + ", ");
                    }
                   
                }
            }
          

            for (int i = 0; i < vAnalysisDataStore.TimeStamps.Count; i++)
            {
                var vNewBuilder = new StringBuilder();
                vNewBuilder.Append(vAnalysisDataStore.TimeStamps[i]+",");
                vBuilders.Add(vNewBuilder);
            }
            int vIndex = 0;

            foreach (var vList in vAnalysisDataStore.mSerializedList)
            {
                var vSb = vBuilders[vIndex];
                foreach (var vItem in vList)
                {
                    vSb.Append(vItem.Value + ", ");
                }
                
                vIndex++;
            }
            var vReturn = vHeaderBuilder.ToString()+"\r\n";
            foreach (var vStringBuilder in vBuilders)
            {
                vReturn += vStringBuilder + "\r\n"  ;
            }
            return vReturn;
        }
    }
}