/** 
* @file BodyRawFrameBase.cs
* @brief Contains the BodyRawFrame class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using UnityEngine;

namespace Assets.Scripts.Frames_Pipeline
{
    /**
    * BodyRawFrame class 
    * @brief Class containing raw frame data
    * Frame structure (single frame):
    * BOIMECH_sensorID_1, Yaw;Pitch;Roll, ... BOIMECH_sensorID_9, Yaw;Pitch;Roll, FLEXCORE_sensorID_1, SensorValue, ... ,FLEXCORE_sensorID_4, SensorValue
    */
    [Serializable]
    public class BodyRawFrame : BodyRawFrameBase
    {

        //Maximum frame size in bytes
        public static uint sRawFrameSize = 100;

        [SerializeField]
        //Frame raw content 
        public string[] RawFrameData;

        /// <summary>
        /// Has the raw frame been decoded ? this is set to true if the original stream is decoded
        /// </summary>
        [SerializeField]
        public bool IsDecoded { get; set; }



        /// <summary>
        /// overload the array operator
        /// </summary>
        /// <param name="vI">accessor/setter index</param>
        /// <returns></returns>
        public string this[int vI]
        {
            get
            {
                return RawFrameData[vI];
            }
            set
            {
                RawFrameData[vI] = (string)value;
            }
        }

        public BodyRawFrame(int vIndex)
        {
            this.Index = vIndex;
        }

    }
}