 
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

    /// <summary>
    /// Acceleration data for the given sensor data frame
    /// </summary>
    public Vector3 AccelData { get; set; }

    /// <summary>
    /// Mag data for the given sensor data frame
    /// </summary>
    public Vector3 MagData { get; set; }

    /// <summary>
    /// Gyro data for the given sensor data frame
    /// </summary>
    public Vector3 GyroData { get; set; }

    public ImuFrame SensorFrame { get; set; }
    public float[, ] OrientationMatrix;
}
