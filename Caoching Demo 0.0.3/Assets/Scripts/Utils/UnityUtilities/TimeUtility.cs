
/** 
* @file TimeUtility.cs
* @brief Contains the TimeUtility class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine;

namespace Assets.Scripts.Utils.UnityUtilities
{
    /**
   * TimeUtility class 
   * @brief TimeUtility class in order to use Time.time from a thread that isn't the main unity thread, we need to hold the time variable
   //somewhere.  
   */

    /// <summary>
    /// TimeUtility class in order to use Time.time from a thread that isn't the main unity thread, we need to hold the time variable
    /// somewhere.
    /// </summary>
    public class TimeUtility: MonoBehaviour
    {
        public static float Time;

        void Update()
        {
            Time = UnityEngine.Time.time;
        }
    }
}
