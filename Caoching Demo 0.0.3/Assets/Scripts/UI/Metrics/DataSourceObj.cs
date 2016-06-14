/**
* @file DataSourceObj.cs
* @brief Contains the DataSourceObj class
* @author Mohammed Haider( mohammed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System.Configuration;
using UnityEngine;

namespace Assets.Scripts.UI.Metrics
{
    /// <summary>
    /// A data source object to plug in required components to a WMG_Series object
    /// </summary>
    public class DataSourceObj : MonoBehaviour
    { 
        public WMG_Series Series;
        private WMG_Data_Source mDataSource;
        private float mUpdatabableVariable;
        public int MaxNumberElements = 100;

        /// <summary>
        /// the updatable variable that is grabbed by the data series.
        /// </summary>
        public float UpdatableVariable
        {
            get { return mUpdatabableVariable; }
            set
            {
                mUpdatabableVariable = value;
                if (Series.pointValues.Count > MaxNumberElements)
                {
                    Series.pointValues.RemoveAt(0);
                }
            }
        }
    
        // Use this for initialization
        void Start()
        {
            mDataSource = GetComponent<WMG_Data_Source>();
            mDataSource.setDataProvider(this);
            mDataSource.setVariableName("UpdatableVariable");
            Series.realTimeDataSource = mDataSource;
            Series.StartRealTimeUpdate();
        }
         
    

    }
}
