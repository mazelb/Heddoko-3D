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
            float  angle = 90F;            
            Matrix4x4 t  = Matrix4x4.zero;
            Matrix4x4 X  = Matrix4x4.zero;
            Vector3 axis = new Vector3(0.0F, 0.0F, 1.0F);
            Matrix<float> Xtmp = Matrix<float>.Build.Dense(4, 4);
            Matrix<float> FakeDataSensors = Matrix<float>.Build.Dense(6, 1);
            MathNet.Numerics Q = Quaternion.AngleAxis(angle, axis);
            ShiuTransform AvatarToDataSensorsTransform = new ShiuTransform(Test);           
            Xtmp = AvatarToDataSensorsTransform.Shiufunc();
            X    = AvatarToDataSensorsTransform.MtoU(Xtmp);

          ///X = AvatarToDataSensorsTransform.Shiufunc(phi1, psy1, theta1, phi2, psy2, theta2);
          ///t =  AvatarToDataSensorsTransform.QuaterniontoRigthHanded(Q);
          ///ShiuTransform AvatarToDataSensorsTransform = new ShiuTransform(Test) ;
          ///AvatarToDataSensorsTransform.TestFunction("CleanRotation",0,0,0);
        }
    }
}