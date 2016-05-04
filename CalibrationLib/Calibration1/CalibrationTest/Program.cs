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
        {   bool Test    = true;              
            Matrix4x4 t  = Matrix4x4.zero;
            Matrix4x4 X  = Matrix4x4.zero;
            Matrix<float> Xtmp = Matrix<float>.Build.Dense(4, 4);
            Matrix<float> FakeDataSensors = Matrix<float>.Build.Dense(6, 1);            
            ShiuTransform AvatarToDataSensorsTransform = new ShiuTransform(Test);
            float K   = 2.0F * Mathf.PI / 360;
            float pitch = 90.0F; float roll = 0.0F ; float yaw = 0.0F;
            AvatarToDataSensorsTransform.TestFunction("CleanRotation", pitch*K, roll*K, yaw*K);
            Xtmp = AvatarToDataSensorsTransform.Shiufunc();
            X    = AvatarToDataSensorsTransform.MtoU(Xtmp);

          /*
             * float  angle = 90F;                     
             * Vector3 axis = new Vector3(0.0F, 0.0F, 1.0F);
             * Quaternion Q = Quaternion.AngleAxis(angle, axis);
             */
          ///X = AvatarToDataSensorsTransform.Shiufunc(phi1, psy1, theta1, phi2, psy2, theta2);
          ///t =  AvatarToDataSensorsTransform.QuaterniontoRigthHanded(Q);
          ///ShiuTransform AvatarToDataSensorsTransform = new ShiuTransform(Test) ;
          ///AvatarToDataSensorsTransform.TestFunction("CleanRotation",0,0,0);
        }
    }
}