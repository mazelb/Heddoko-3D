 
using System; 
using System.Collections.Generic;
using System.Security.Policy;
using HeddokoLib.body_pipeline;
using UnityEngine;

public class SensorsData  
{
    //Size of Data contained in the list
    public int DataSize { get { return Data.Count; } }

    //List of sensor raw data
    public List<Int16> Data = new List<Int16>();

    public BodyFrame.Vect4 PositionalData { get; set; }
    public ImuFrame SensorFrame { get; set; }
    public float[, ] OrientationMatrix;
}
