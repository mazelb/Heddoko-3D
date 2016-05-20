/** 
* @file Sensor.cs
* @brief Contains the Sensor  class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine;
using System;  
using System.Collections.Generic;
public class Sensor
{
    //Sensor type
    public BodyStructureMap.SensorTypes SensorType;
    //Sensor position
    public BodyStructureMap.SensorPositions SensorPosition;
    //Sensor Unique GUID for ease of cloud access
    public string SensorGuid;
    //Sensor ID on the body
    public int SensorBodyId;
    private SensorsData sensorData = new SensorsData();
  
    
    //Data contained in the sensor
    public SensorsData SensorData
    {
        get
        {
            return sensorData;
        }
        set { sensorData = value; }
    }

    public Sensor()
    {
        SensorGuid = "";
        SensorData = new SensorsData();
        
    }



    /**
    * Sensor(Sensor sensor)
    * @param Sensor vSensor
    * @brief BodyFrameThread needed to start updated body 
    * @note 
    * @return returns the view associated with this body
    */

    public Sensor(Sensor vSensor)
    {
        this.SensorType = vSensor.SensorType;
        this.SensorPosition = vSensor.SensorPosition;
        this.SensorGuid = string.Copy(vSensor.SensorGuid);
        this.SensorBodyId = vSensor.SensorBodyId;
       //deep copy
        this.SensorData.Data = new List<short>(vSensor.SensorData.Data);
        this.SensorData.PositionalData = vSensor.SensorData.PositionalData; 
    }

    public void CreateNewUUID()
    {
        SensorGuid = Guid.NewGuid().ToString();

    }
}

public class SensorTuple 
{
    public Sensor InitSensor;
    public Sensor CurrentSensor;
}