/** 
* @file HeddokoMathTools.cs
* @brief Contains the HeddokoMathTools class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;

namespace Assets.Scripts.Utils
{
    /// <summary>
    /// Static class containing math functions
    /// </summary>
    public static class HeddokoMathTools
    {
        /// <summary> 
        /// Establishes a proportion between two ranges of values, and returns a value that is in proportion to the passed in vValue
        /// </summary>
        /// <param name="vValue">a float between vMin1 and vMax1</param>
        /// <param name="vMin1">minimum value of the first set of values</param>
        /// <param name="vMax1">maximum value of the first set of values</param>
        /// <param name="vMin2">minimum value of the second set of values</param>
        /// <param name="vMax2">maximum value of the second set of values</param>
        /// <returns> a value proportional to vValue one that is in between vMin2 and vMax2</returns>
        public static float Map(float vValue, float vMin1, float vMax1, float vMin2, float vMax2)
        {
            float vReturnedValue = vMin2 + (vMax2 - vMin2) * ((vValue - vMin1) / (vMax1 - vMin1));
            return vReturnedValue;
        }

      public  static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360.0f)
            {
                angle += 360.0f;
            }
            if (angle > 360.0f)
            {
                angle -= 360.0f;
            }
            return Mathf.Clamp(angle, min, max);
        }
    }
}
