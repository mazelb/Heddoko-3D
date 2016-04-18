
/** 
* @file HeddokoDebugKeyMappings.cs
* @brief Contains the HeddokoDebugKeyMappings class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine;
namespace Assets.Scripts.Utils.DebugContext
{
    /// <summary>
    /// keycodes for different functions
    /// </summary>
  public  static class HeddokoDebugKeyMappings
  {
        public static KeyCode ResetFrame = KeyCode.Home;
        public static KeyCode Pause = KeyCode.P;
        public static KeyCode MoveNext = KeyCode.RightArrow;
        public static KeyCode MoveToPrev = KeyCode.LeftArrow;

        public static KeyCode ResetMetrics = KeyCode.Equals;
        public static KeyCode IsTrackingHeight = KeyCode.Alpha0;
 
       
        public static KeyCode IsFusingLegs = KeyCode.Alpha3;
  
      
        public static KeyCode IsTrackingGait = KeyCode.Alpha5;
 
        public static KeyCode IsTrackingHips = KeyCode.Minus;
        public static KeyCode IsHipsEstimateForward = KeyCode.Alpha9;
        public static KeyCode IsHipsEstimateUp = KeyCode.Alpha8;
        public static KeyCode IsUsingInterpolationForBody = KeyCode.Alpha7;
        public static KeyCode IsUsingFusionForBody = KeyCode.F;
 
        public static KeyCode IsAdjustingSegmentAxis = KeyCode.Alpha6;
        public static KeyCode IsUsingInterpolation = KeyCode.Alpha7;
 
 
        public static KeyCode IsFusingSubSegments = KeyCode.F;

        public static KeyCode IncBodyInterpolationSp = KeyCode.J;
        public static KeyCode DecBodyInterpoationSp = KeyCode.K ;
        public static KeyCode HideSegmentFlagPanel = KeyCode.H;
        public static KeyCode SwitchToRecordingFromLive = KeyCode.S;
        public static KeyCode SkipToLiveViewFromRecordingView = KeyCode.D;
        public static KeyCode IsProjectingXZ = KeyCode.Y;
        public static KeyCode IsProjectingXY = KeyCode.U;
        public static KeyCode IsProjectingYZ = KeyCode.I;
        public static KeyCode IsAdjustingArms = KeyCode.Semicolon;
        public static KeyCode IsCalibrating = KeyCode.RightWindows;
        public static KeyCode SettingsButton = KeyCode.Escape;
  }
}
