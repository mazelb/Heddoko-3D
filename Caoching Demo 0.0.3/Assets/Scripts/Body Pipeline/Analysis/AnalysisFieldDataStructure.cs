 /**
 * @file AnalysisDataStructure.cs
 * @brief Contains the 
 * @author Mohammed Haider( mohammed@heddoko.com)
 * @date June 2016
 * Copyright Heddoko(TM) 2016,  all rights reserved
 */

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace Assets.Scripts.Body_Pipeline.Analysis
{
    /// <summary>
    /// A generic data structure for an analyis segment component
    /// </summary>
    public class AnalysisFieldDataStructure
    {
        private string mName = ""; 
        public FieldInfo FieldInfoKey;
        private int mCount = 0;
        private readonly List<System.Single> mDataCollection = new List<System.Single>(10000);
 
        public int Count
        {
            get
            {
                return mCount;
            }
        }
        public List<float> DataCollection
        {
            get { return mDataCollection; }
        }

        /// <summary>
        /// Clears the list of data collection
        /// </summary>
        public void ClearDataCollection()
        {
            mDataCollection.Clear();
        }


      
        /// <summary>
        /// Adds a timestamp and value to the current list
        /// </summary>
        /// <param name="vTimeStamp"></param>
        /// <param name="vValue"></param>
        public void Add(  System.Single vValue)
        {
            mDataCollection.Add(vValue); 
            mCount++;
        }
    }
}