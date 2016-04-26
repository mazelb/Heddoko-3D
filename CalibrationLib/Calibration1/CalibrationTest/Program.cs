using System;
using System.Collections.Generic;
using System.Text;
using Calibration1.CalibrationTransformation;
using UnityEngine;

namespace CalibrationTest
{
    class Program
    {
        static void Main(string[] args)
        {          
          float phi1 = 0.0F;
          float psy1 = 0.0F;
          float theta1 = 0.0F;
          float phi2 = 0.0F;
          float psy2 = 0.0F;
          float theta2 = 0.0F;
          Matrix4x4 X  = Matrix4x4.zero;
          ShiuTransform Cal = new ShiuTransform() ;
          X = Cal.Shiufunc(phi1, psy1, theta1, phi2, psy2, theta2);
          Console.ReadLine();
        }
    }
}