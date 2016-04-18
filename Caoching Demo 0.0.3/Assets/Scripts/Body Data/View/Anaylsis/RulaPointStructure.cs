using UnityEngine;

namespace Assets.Scripts.Body_Data.View.Anaylsis
{
    public class RulaPointStructure
    {
         
        public float StartTheta;
        public float EndTheta;
        public float Range;
        public Color32 Color;
        public string PointName;

        /// <summary>
        /// Constructor, automatically sets the range and the color of the current point according to the passed in params
        /// </summary>
        /// <param name="vStartTheta"></param>
        /// <param name="vEndTheta"></param>
        /// <param name="vPoint"></param>
        public RulaPointStructure(float vStartTheta, float vEndTheta, string vPoint)
        {
            StartTheta = vStartTheta;
            EndTheta = vEndTheta;
            Range = EndTheta - StartTheta;
            PointName = vPoint;
            Color = RulaSettings.GetPointColorOfRulaPoint(vPoint);
        }


    }
}