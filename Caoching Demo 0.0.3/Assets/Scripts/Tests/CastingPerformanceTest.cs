/**
* @file CastingPerformanceTest.cs
* @brief Contains the 
* @author Mohammed Haider( mohammed@heddoko.com)
* @date April 2016  
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Diagnostics;
using Assets.Scripts.Body_Data.Interfaces;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// Casting performance test
    /// </summary>
    public class CastingPerformanceTest : MonoBehaviour
    {
        public int ItemNumbers = 100000;
        void Awake()
        {
            Stopwatch vStopwatch = new Stopwatch();
            IBodySubsegmentTransformValues[] vTransformVals = new IBodySubsegmentTransformValues[ItemNumbers];
            UnitySubSegmentTransform[] vConcreteTransforms = new UnitySubSegmentTransform[ItemNumbers];
            //new UnitySubSegmentTransform[ItemNumbers]; 
            for (int i = 0; i < vTransformVals.Length; i++)
            {
                vTransformVals[i] = new UnitySubSegmentTransform();
                vConcreteTransforms[i] = new UnitySubSegmentTransform();
            }
            vStopwatch.Start(); 
            Debug.Log("<color=green>Starting direct vConcrete calculations</color>");
            for (int i = 0; i < vConcreteTransforms.Length; i++)
            {
                vConcreteTransforms[i].SubSegmentPosition += Vector3.back*0.001f;
                vConcreteTransforms[i].SubsegmentOrientation *= Quaternion.AngleAxis(0.01f,Vector3.up); 
            }
            vStopwatch.Stop();
            float vTime = vStopwatch.ElapsedMilliseconds /1000f;
            Debug.Log("<color=green>Completed. time taken:</color>" + "<color=blue>" +  vTime.ToString("0.000") + "ms</color>"); Debug.Log("<color=green>Starting direct vConcrete calculations</color>");
            vStopwatch.Reset();
            vStopwatch.Start();
            Debug.Log("<color=orange>Starting casting calculations</color>");

            for (int i = 0; i < vTransformVals.Length; i++)
            {
                var vT = ((UnitySubSegmentTransform)vTransformVals[i]);
                vT.SubSegmentPosition += Vector3.back * 0.001f;
                vT.SubsegmentOrientation *= Quaternion.AngleAxis(0.01f, Vector3.up);
            }
            vTime = vStopwatch.ElapsedMilliseconds / 1000f;


            Debug.Log("<color=red>Completed. time taken:</color>" + "<color=blue>" + vTime + "ms</color>"); vStopwatch.Reset();
            vStopwatch.Start();
            Debug.Log("<color=orange>Starting AS  casting calculations</color>");

            for (int i = 0; i < vTransformVals.Length; i++)
            {
                var vT = vTransformVals[i] as UnitySubSegmentTransform;
                vT.SubSegmentPosition += Vector3.back * 0.001f;
                vT.SubsegmentOrientation *= Quaternion.AngleAxis(0.01f, Vector3.up);
            }
            vTime = vStopwatch.ElapsedMilliseconds / 1000f;
            Debug.Log("<color=orange>Completed. time taken:</color>" + "<color=blue>" + vTime + "ms</color>");


        }
    }
}