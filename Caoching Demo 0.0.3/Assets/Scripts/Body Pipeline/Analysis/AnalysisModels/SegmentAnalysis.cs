using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis
{
    /// <summary>
    /// Parent class to the specific abstracted segment type(leg or arm). 
    /// </summary>
    [Serializable]
    public abstract class SegmentAnalysis
    {
        public float DeltaTime;
        internal BodyStructureMap.SegmentTypes SegmentType;
        internal float mLastTimeCalled;

        /// <summary>
        /// Extraction of angles. The parent class Updates Delta time
        /// </summary>
        public virtual void AngleExtraction()
        {
          
            
        }

        /// <summary>
        /// Reset the metrics calculations
        /// </summary>
        public virtual void ResetMetrics()
        {

        }
        
         static public float GetSignedAngle(Vector3 vVectorA, Vector3 vVectorB, Vector3 vVectorNormal)
        {
            // angle in [0,180]
            float angle = Vector3.Angle(vVectorA,vVectorB);
            float sign = Mathf.Sign(Vector3.Dot(vVectorNormal,Vector3.Cross(vVectorA, vVectorB)));
            //Debug.Log(sign);

            // angle in [-179,180]
            float signed_angle = angle * sign;

            return signed_angle;    
        }//*/

        static public float Get360Angle(Vector3 vVectorA, Vector3 vVectorB, Vector3 vVectorNormal)
        {
            // angle in [0,180]
            float angle = Vector3.Angle(vVectorA, vVectorB);
            float sign = Mathf.Sign(Vector3.Dot(vVectorNormal, Vector3.Cross(vVectorA, vVectorB)));

            // angle in [-179,180]
            float signed_angle = angle * sign;

            // angle in [0,360] (not used but included here for completeness)
            float angle360 = (signed_angle + 180) % 360;

            return angle360;
        }//*/
    }
}
