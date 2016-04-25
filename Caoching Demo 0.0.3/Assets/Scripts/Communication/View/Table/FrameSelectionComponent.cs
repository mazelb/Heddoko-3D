/** 
* @file FrameSelectionComponent.cs
* @brief FrameSelectionComponent class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/ 
using UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;

namespace Assets.Scripts.Communication.View.Table
{
    /// <summary>
    /// A selectable frame component, holding incoming Frame data. 
    /// </summary>
    public class FrameSelectionComponent :ListViewItem, IResizableItem
    {

        public Text TimeStamp;
        public Text SensorData; 
        /// <summary>
        /// The gameobject that can be resized
        /// </summary>
        public GameObject[] ObjectsToResize
        {
            get { return new GameObject[] { TimeStamp.gameObject, SensorData.gameObject }; }
        }
        /// <summary>
        /// Sets data from the given body frame
        /// </summary> 
        /// <param name="vBodyFrame"></param>
        public void SetData(BodyFrame vBodyFrame)
        {
            TimeStamp.text = vBodyFrame.Timestamp + "";
            SensorData.text = vBodyFrame.ToString(new[] { ' ' });
        }

       

    }
}