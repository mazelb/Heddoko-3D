using System;
using System.Collections.Generic;
using System.Text;
using Calibration1.CalibrationTransformation;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;
namespace CalibrationTest
{
    class Program
    {
        static void Main(string[] args)
        {          
          bool   Test  = true;
          Matrix4x4 X  = Matrix4x4.zero;
          Matrix<float> FakeDataSensors = Matrix<float>.Build.Dense(6, 1)      ;
            //ShiuTransform AvatarToDataSensorsTransform = new ShiuTransform(Test) ;
            ShiuTransform AvatarToDataSensorsTransform = new ShiuTransform(false);
            //AvatarToDataSensorsTransform.TestFunction("CleanRotation",0,0,0);
          X = AvatarToDataSensorsTransform.Shiufunc();
          //X = AvatarToDataSensorsTransform.Shiufunc(phi1, psy1, theta1, phi2, psy2, theta2);
        }
    }
}