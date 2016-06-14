/** 
* @file FrameSelectionComponent.cs
* @brief FrameSelectionComponent class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Text;
using UIWidgets;
using UnityEngine;
 using UnityEngine.UI;

namespace Assets.Scripts.Communication.View.Table
{
    /// <summary>
    /// A selectable frame component, holding incoming Frame data. 
    /// </summary>
    public class FrameSelectionComponent : ListViewItem, IResizableItem
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
            TimeStamp.text = vBodyFrame.Timestamp.ToString("0.00");
            SensorData.text = FormatBodyFrameString(vBodyFrame); //vBodyFrame.ToString(new[] { ' ' });
        }

        public string FormatBodyFrameString(BodyFrame vFrame)
        {
            StringBuilder vBuilder = new StringBuilder();
            vBuilder.Append("");
            foreach (var vKvPair in vFrame.FrameData)
            {
                if (vKvPair.Key != BodyStructureMap.SensorPositions.SP_SensorPositionCount &&
                    vKvPair.Key != BodyStructureMap.SensorPositions.SP_LeftKnee &&
                    vKvPair.Key != BodyStructureMap.SensorPositions.SP_RightKnee &&
                     vKvPair.Key != BodyStructureMap.SensorPositions.SP_LowerSpine)
                {
                    vBuilder.AppendFormat(" [{0:0.00},{1:0.00},{2:0.00}] ", vKvPair.Value.x, vKvPair.Value.y, vKvPair.Value.z);

                }
            }
            return vBuilder.ToString();

        }


    }
}