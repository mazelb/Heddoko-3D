
/** 
* @file BodyFrame.cs
* @brief Contains the BodyFrame class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using Assets.Scripts.Frames_Pipeline;
using heddoko;
using HeddokoLib.heddokoProtobuff;
using HeddokoLib.utils;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// The frame of data that is populated to sensors, and contains the list of sensors to access sensors data
/// </summary>
[DataContract]
public class BodyFrame
{

    //The frame of data populated to sensors 
    [JsonProperty]
    private Dictionary<BodyStructureMap.SensorPositions, Vect4> mFrameData;

    //The timestamp of a bodybody frame 
    [JsonProperty]
    private float mTimeStamp;

    /// <summary>
    /// Index of the item
    /// </summary>
    public int Index { get; set; }
    [JsonIgnore]
    internal Dictionary<BodyStructureMap.SensorPositions, Vect4> FrameData
    {
        get
        {
            if (mFrameData == null)
            {
                mFrameData = new Dictionary<BodyStructureMap.SensorPositions, Vect4>(18);
            }
            return mFrameData;
        }
        set
        {
            mFrameData = value;
        }
    }
    [JsonIgnore]
    internal float Timestamp
    {
        get { return mTimeStamp; }
        set { mTimeStamp = value; }
    }


    /**
    * ToString()
    * @brief Prepares the current body frame as a string 
    * @return current body frame as a string
    * 
    */
    public override string ToString()
    {
        return ToString(new[] { ' ' }, true, true);
    }

    /// <summary>
    /// Returns a string, with a character delimiter between sensor position and data. 
    /// </summary>
    /// <returns>Frame data as a string</returns>
    public string ToString(char[] vDelimiter, bool vIncludePos = false, bool vLineSeperated = false)
    {
        string vOutput = "";
        foreach (KeyValuePair<BodyStructureMap.SensorPositions, Vect4> vPair in FrameData)
        {
            vOutput += "" + (vIncludePos ? vPair.Key + "" : "") + new string(vDelimiter) + vPair.Value + (vLineSeperated ? "\n" : " ");
        }
        return vOutput;
    }

    public string ToCsvString()
    {
        string vOutput = "" + (long)Timestamp + ",";
        foreach (KeyValuePair<BodyStructureMap.SensorPositions, Vect4> vPair in FrameData)
        {
            vOutput += (int)vPair.Key + "," + vPair.Value.x + ";" + vPair.Value.y + ";" + vPair.Value.z + ",";
        }
        return vOutput;
    }

    /// <summary>
    /// To a csv string without the timestamp or key included. Default seperator is ','
    /// </summary>
    /// <param name="vSeperator"></param>
    /// <returns></returns>

    public string ToCsvnoTsNoKeyIncluded(string vSeperator = ",")
    {
        string vOutput = "";
        foreach (KeyValuePair<BodyStructureMap.SensorPositions, Vect4> vPair in FrameData)
        {
            vOutput += vPair.Value.x + vSeperator + vPair.Value.y + vSeperator + vPair.Value.z + vSeperator + vPair.Value.w + vSeperator;
        }
        return vOutput;
    }
    public BodyFrame(int vIndex)
    {
        Index = vIndex;
    }

    private static Dictionary<int, int> sErrorCount;

    private void CountError(int vId)
    {
        if (sErrorCount == null)
        {
            sErrorCount = new Dictionary<int, int>();
            for (int vI = 0; vI < 10; vI++)
            {
                sErrorCount.Add(vI, 0);
            }
        }
        sErrorCount[vId]++;
    }

    public static void PrintErrorCount()
    {
        string vMsg = "";
        foreach (var vI in sErrorCount)
        {
            vMsg += " index " + vI.Key + " count " + vI.Value + " ,";
        }
        UnityEngine.Debug.Log(vMsg);
    }


    /// <summary>
    /// Create a Bodyframe from a protobuf packet
    /// </summary>
    /// <param name="vPacket">The packet</param>
    public BodyFrame(Packet vPacket )
    {
        Timestamp = vPacket.fullDataFrame.timeStamp / 1000f;
        var vDataList = vPacket.fullDataFrame.imuDataFrame;
        
        for (int vI = 0; vI < vDataList.Count; vI++)
        {
            var vDataFrame = vDataList[vI];
            int vId = (int)vPacket.fullDataFrame.imuDataFrame[vI].imuId;
            var vSensorId = ImuSensorFromPos(vId);
            Vect4 vVect = new Vect4();
            vVect.x = vDataFrame.quat_x_yaw;
            vVect.y = vDataFrame.quat_y_pitch;
            vVect.z = vDataFrame.quat_z_roll;
            vVect.w = vDataFrame.quat_w;
            FrameData.Add(vSensorId, vVect);
        }
    }
     
    /**
    * ConvertRawFrame(BodyRawFrame rawData)
    * @brief Pass in a BodyRawFrame and convert it to a body frame
    * @param BodyRawFrame rawData
    * @return void
    * 
    */
    public static BodyFrame ConvertRawFrame(BodyRawFrame vRawData)
    {
        float vTimestamp = 0;
        float.TryParse(vRawData.RawFrameData[0], out vTimestamp);

        //from startIndex to endIndex, we check the subframes and extrapolate the IMU data. 
        int vStartIndex = 1;
        int vEndIndex = 20;

        //The check index is made such that when we iterate through the list, it is possible that the 19th index isn't of an IMU type, then it must be that it is a stretch sensor
        //at this index we check if we actually hold data for the lower spine. If we do, then we continue, otherwise, we clear and the stretch data is gathered. 
        int vCheckIndex = 19;
        bool vFinishLoop = false;
        BodyFrame vBodyFrame = new BodyFrame(vRawData.Index);
        vBodyFrame.Timestamp = vTimestamp;

        //placeholder data to be used in the dictionary until it gets populated by the following loop
        Vect4 vPlaceholderV3 = new Vect4();

        int vKey = 0;
        BodyStructureMap.SensorPositions vSensorPosAsKey = BodyStructureMap.SensorPositions.SP_RightElbow; //initializing sensor positions to some default value
        for (int vI = vStartIndex; vI < vEndIndex; vI++)
        {
            //first check if the current index falls on a position that can be interpreted as an int
            if (vI % 2 == 1)
            {
                if (vI == vCheckIndex)
                {
                    try
                    {
                        int.TryParse(vRawData.RawFrameData[vI], out vKey);
                    }
                    finally
                    {
                        if (vKey != 10)
                        {
                            vFinishLoop = true;
                        }
                    }
                    if (vFinishLoop)
                    {
                        //set the start index for the next iteration
                        vStartIndex = vI;
                        break;
                    }
                }
                int.TryParse(vRawData.RawFrameData[vI], out vKey);
                vKey--;
                vSensorPosAsKey = ImuSensorFromPos(vKey);
                vBodyFrame.FrameData.Add(vSensorPosAsKey, vPlaceholderV3);
            }
            else
            {
                //split the string into three floats
                string[] v3Data = vRawData.RawFrameData[vI].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                float[] vAlue = new float[3];
                for (int vJ = 0; vJ < 3; vJ++)
                {
                    float.TryParse(v3Data[vJ], out vAlue[vJ]);
                }
                vBodyFrame.FrameData[vSensorPosAsKey] = new Vect4(vAlue[0], vAlue[1], vAlue[2]);// new Vector3(value[0], value[1], value[2]);
            }

        }

        //check if lower spine exists
        if (!vBodyFrame.FrameData.ContainsKey(BodyStructureMap.SensorPositions.SP_LowerSpine))
        {
            vBodyFrame.FrameData.Add(BodyStructureMap.SensorPositions.SP_LowerSpine, new Vect4()); //Vector3.zero));
        }

        //todo stretch sense data extrapolation starting from the updated startingIndex

        return vBodyFrame;

    }

    /// <summary>
    /// Converts a string received by the Brainpack server service into a bodyframe
    /// </summary>
    /// <param name="vHexPacket"></param>
    /// <returns></returns>
    public static BodyFrame ConvertFromHexaString(string vHexPacket)
    {
        string[] vVRawData = vHexPacket.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        //from startIndex to endIndex, we check the subframes and extrapolate the IMU data. 
        int vStartIndex = 1;
        int vEndIndex = 20;
        //The check index is made such that when we iterate through the list, it is possible that the 19th index isn't of an IMU type, then it must be that it is a stretch sensor
        //at this index we check if we actually hold data for the lower spine. If we do, then we continue, otherwise, we clear and the stretch data is gathered. 
        int vCheckIndex = 19;
        bool vFinishLoop = false;
        BodyFrame vBodyFrame = new BodyFrame(-1);

        //placeholder data to be used in the dictionary until it gets populated by the following loop
        Vect4 vPlaceholderV3 = new Vect4();//Vector3.zero); 

        int vKey = 0;

        //initializing sensor positions to some default value
        BodyStructureMap.SensorPositions vSensorPosAsKey = BodyStructureMap.SensorPositions.SP_RightElbow;

        for (int vI = vStartIndex; vI < vEndIndex; vI++)
        {
            //first check if the current index falls on a position that can be interpreted as an int
            if (vI % 2 == 1)
            {
                if (vI == vCheckIndex)
                {
                    try
                    {
                        int.TryParse(vVRawData[vI], out vKey);
                    }
                    finally
                    {
                        if (vKey != 10)
                        {
                            vFinishLoop = true;
                        }
                    }
                    if (vFinishLoop)
                    {
                        //set the start index for the next iteration
                        //todo:Start index in the hexa string method is to be used when grabbing stretch sense data
                        vStartIndex = vI;
                        break;
                    }
                }
                int.TryParse(vVRawData[vI], out vKey);
                vKey--;
                vSensorPosAsKey = ImuSensorFromPos(vKey);
                vBodyFrame.FrameData.Add(vSensorPosAsKey, vPlaceholderV3);
            }
            else
            {
                //split the string into three floats
                string[] v3Data = vVRawData[vI].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                float[] vAlue = new float[3];
                for (int vJ = 0; vJ < 3; vJ++)
                {
                    vAlue[vJ] = ConversionTools.ConvertHexStringToFloat((v3Data[vJ]));
                }
                vBodyFrame.FrameData[vSensorPosAsKey] = new Vect4(vAlue[0], vAlue[1], vAlue[2]);// new Vector3 (value[0], value[1], value[2]);
            }
        }
        return vBodyFrame;
    }

    /// <summary>
    /// Creates a bodyframe from an array of vectors
    /// </summary>
    /// <param name="vBodyFrameData"></param>
    /// <returns></returns>
    public static BodyFrame CreateBodyFrame(Vect4[] vBodyFrameData)
    {
        BodyFrame vBodyFrame = new BodyFrame(-1);
        vBodyFrame.FrameData.Add(BodyStructureMap.SensorPositions.SP_LowerSpine, new Vect4());//(Vector3.zero));
        for (int vI = 0; vI < vBodyFrameData.Length; vI++)
        {
            BodyStructureMap.SensorPositions vSenPos = ImuSensorFromPos(vI);
            vBodyFrame.FrameData.Add(vSenPos, vBodyFrameData[vI]);
        }
        return vBodyFrame;
    }

    /// <summary>
    /// Returns a sensor position fromt eh given position
    /// </summary>
    /// <param name="vPos"></param>
    /// <returns></returns>
    internal static BodyStructureMap.SensorPositions ImuSensorFromPos(int vPos)
    {
        if (vPos == 0)
        {
            return BodyStructureMap.SensorPositions.SP_UpperSpine;
        }
        if (vPos == 1)
        {
            return BodyStructureMap.SensorPositions.SP_RightUpperArm;
        }
        if (vPos == 2)
        {
            return BodyStructureMap.SensorPositions.SP_RightForeArm;
        }
        if (vPos == 3)
        {
            return BodyStructureMap.SensorPositions.SP_LeftUpperArm;
        }
        if (vPos == 4)
        {
            return BodyStructureMap.SensorPositions.SP_LeftForeArm;
        }
        if (vPos == 5)
        {
            return BodyStructureMap.SensorPositions.SP_RightThigh;
        }
        if (vPos == 6)
        {
            return BodyStructureMap.SensorPositions.SP_RightCalf;
        }
        if (vPos == 7)
        {
            return BodyStructureMap.SensorPositions.SP_LeftThigh;
        }
        if (vPos == 8)
        {
            return BodyStructureMap.SensorPositions.SP_LeftCalf;
        }
        else
        {
            return BodyStructureMap.SensorPositions.SP_LowerSpine;
        }
    }



    /// <summary>
    /// Returns a sensor position of a stretch sensor from the given int pos
    /// </summary>
    /// <param name="vPos"></param>
    /// <returns></returns>
    internal BodyStructureMap.SensorPositions StretchSensorFromPos(int vPos)
    {
        if (vPos == 0)
        {
            return BodyStructureMap.SensorPositions.SP_RightElbow;
        }
        if (vPos == 1)
        {
            return BodyStructureMap.SensorPositions.SP_LeftElbow;
        }
        if (vPos == 2)
        {
            return BodyStructureMap.SensorPositions.SP_RightKnee;
        }
        else
        {
            return BodyStructureMap.SensorPositions.SP_LeftKnee;
        }
    }

    /// <summary>
    /// Vector4 with nullable component w
    /// </summary>
    public struct Vect4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vect4(float vX = 0, float vY = 0, float vZ = 0, float vW = 0)
        {
            this.x = vX;
            this.y = vY;
            this.z = vZ;
            this.w = vW;
        }

        
        public float this[int vIndex]
        {
            get
            {
                switch (vIndex)
                {
                    case 0:
                        return x;
                        break;
                    case 1:
                        return y;
                        break;
                    case 2:
                        return z;
                        break;
                    case 3:
                        return w;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vect4 index!");
                        break;
                }
            }
            set
            {
                switch (vIndex)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vect4 index!");
                        break;
                }
            }

        }


    }
}
