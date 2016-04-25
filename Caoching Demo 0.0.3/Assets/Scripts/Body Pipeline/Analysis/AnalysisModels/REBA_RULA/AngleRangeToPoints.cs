
/** 
* @file AngleRangeToPoints.cs
* @brief Contains the AngleRangeToPoints class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
namespace Assets.Scripts.Body_Pipeline.Analysis.AnalysisModels.REBA_RULA
{
    /// <summary>
    /// A range of angle to points association
    /// </summary>
    public class AngleRangeToPoints
    {
         public int Point { get; set; }
        public float MinAngle { get; set; }
        public float MaxAngle { get; set; }
    }
}