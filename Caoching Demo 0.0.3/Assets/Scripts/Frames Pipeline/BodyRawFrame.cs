/** 
* @file BodyRawFrame.cs
* @brief Contains the BodyRawFrame class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine;
using System.Collections;
using System;

/**
* BodyRawFrame class 
* @brief Class containing raw frame data
* Frame structure (single frame):
* BOIMECH_sensorID_1, Yaw;Pitch;Roll, ... BOIMECH_sensorID_9, Yaw;Pitch;Roll, FLEXCORE_sensorID_1, SensorValue, ... ,FLEXCORE_sensorID_4, SensorValue
*/
[Serializable]
public class BodyRawFrame
{ 

    //Maximum frame size in bytes
    public static uint sRawFrameSize = 100;
    [SerializeField]
    //containing Recording GUID
    public string BodyRecordingGuid;
    [SerializeField]
    //Containing Body GUID 
    public string BodyGuid;
    [SerializeField]
    //Containing Suit GUID 
    public string SuitGuid;
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
    /// <param name="i">accessor/setter index</param>
    /// <returns></returns>
    public string this[int i]
    {
        get
        {
            return RawFrameData[i];
        }
        set
        {
            RawFrameData[i] = value;
        }
    }

}
