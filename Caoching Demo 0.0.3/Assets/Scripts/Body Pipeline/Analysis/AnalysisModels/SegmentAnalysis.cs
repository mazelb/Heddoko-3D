
/** 
* @file SegmentAnalysis.cs
* @brief SegmentAnalysis class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis
{
    public delegate void AnalysisComplete(SegmentAnalysis vSegmentAnalysis);
    /// <summary>
    /// Parent class to the specific abstracted segment type(leg or arm). 
    /// </summary>
    [Serializable]
    public abstract class SegmentAnalysis
    {
        internal static Dictionary<string, int> SignMap = new Dictionary<string, int>();
        [AnalysisSerialization(IgnoreAttribute = true)]
        public float DeltaTime;
        internal BodyStructureMap.SegmentTypes SegmentType;
        internal event AnalysisComplete AnalysisCompletedEvent;

        /// <summary>
        /// Extraction of angles. The parent class Updates Delta time
        /// </summary>
        public virtual void AngleExtraction()
        {


        }
 

        /// <summary>
        /// Adds an analysis completed listener 
        /// </summary>
        /// <param name="vListener"></param>
        internal void AddAnalysisCompletionListener(AnalysisComplete vListener)
        {
            AnalysisCompletedEvent += vListener; 
        }

        /// <summary>
        /// Removes an analysis completion
        /// </summary>
        /// <param name="vListener"></param>
        internal void RemoveAnalysisCompletionListener(AnalysisComplete vListener)
        {
            if (AnalysisCompletedEvent != null)
            {
                AnalysisCompletedEvent -= vListener;
            }
        }

        /// <summary>
        /// Notify listeners of analysis completion
        /// </summary>
        internal void NotifyAnalysisCompletionListeners()
        {
            if (AnalysisCompletedEvent != null)
            {
                AnalysisCompletedEvent(this);
            }
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
            float angle = Vector3.Angle(vVectorA, vVectorB);
            float sign = Mathf.Sign(Vector3.Dot(vVectorNormal, Vector3.Cross(vVectorA, vVectorB)));
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
        }

        /// <summary>
        /// Set the sign 
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vValue"></param>
        public static void SetSign(string vKey, int vValue)
        {
            int vSign = Math.Sign(vValue);
            if (SignMap.ContainsKey(vKey))
            {
                SignMap[vKey] = vSign;
            }
            else
            {
                AddSign(vKey,vValue);
            }
        }

        /// <summary>
        /// Adds a sign
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vValue"></param>
        public static void AddSign(string vKey, int vValue)
        {
            int vSign = Math.Sign(vValue);
            if (!SignMap.ContainsKey(vKey))
            {
                SignMap.Add(vKey,vValue);
                SignMap[vKey] = vSign;
            }
            else
            {
                SetSign(vKey,vValue);
            }
        }


        /// <summary>
        /// Gets the sign from the given key
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vValue"></param>
        /// <returns></returns>
        internal static int GetSign(string vKey)
        {
            if (SignMap.ContainsKey(vKey))
            {
                return SignMap[vKey];
            }
            return 1;
        }
    }
}
