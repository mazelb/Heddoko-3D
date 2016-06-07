/**
* @file HVector3Tests.cs
* @brief Contains the 
* @author Mohammed Haider( mohammed@heddoko.com)
* @date 05 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using System.Diagnostics;
using Assets.Scripts.Utils.HMath;
using Assets.Scripts.Utils.HMath.Service_Provider;
using Assets.Scripts.Utils.HMath.Structure;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public class HVector3Tests : MonoBehaviour
    {
        public int NumberOfObjects;
        public Text Info;
        private HVector3[] HVector3s;
        private Vector3[] Vector3s;
        private Stopwatch mStopwatch = new Stopwatch();
    
        void Awake()
        {
            Vector3 vA = new Vector3(Random.Range(0f, 20f), Random.Range(0f, 20f), Random.Range(0f, 20f));
            Vector3 vB = new Vector3(Random.Range(0f, 20f), Random.Range(0f, 20f), Random.Range(0f, 20f));

            U3DVector3 vUA = new U3DVector3(vA);
            U3DVector3 vUB = new U3DVector3(vB);
            HVector3.Vector3MathServiceProvider = new UVector3MathServiceProvider();
 
            mStopwatch.Start();
            Info.text += "Instantiating HVector3s..\n";
             HVector3s = new HVector3[NumberOfObjects];
             mStopwatch.Stop();
             Info.text += "Time taken to generate: " + (mStopwatch.ElapsedMilliseconds) + " ms \n";
            mStopwatch.Reset();

            Info.text += "Instantiating built in Vector3s..\n";
             mStopwatch.Start();
            Vector3s = new Vector3[NumberOfObjects];
            mStopwatch.Stop();

             Info.text += "Time taken to generate: " + (mStopwatch.ElapsedMilliseconds) + " ms \n";
            mStopwatch.Reset();



            Info.text += "\n\nStarting HVector computation: creating random nums" + "\n";
            mStopwatch.Start();
             for (int i = 0; i < HVector3s.Length; i++)
            {
                var x = Random.Range(0f, 1000f);
                var y = Random.Range(0f, 1000f);
                var z = Random.Range(0f, 1000f);
                HVector3s[i] = new U3DVector3(x, y, z);
            }
             mStopwatch.Stop();
            Info.text += "\n Completed HVector computation: creating random nums" + (mStopwatch.ElapsedMilliseconds) + " ms \n";
             mStopwatch.Reset();

            Info.text += "\n\nStarting Vector3 computation: creating random nums" + "\n";
            mStopwatch.Start();
             for (int i = 0; i < Vector3s.Length; i++)
            {
                var x = Random.Range(0f, 1000f);
                var y = Random.Range(0f, 1000f);
                var z = Random.Range(0f, 1000f);

                Vector3s[i] = new Vector3(x, y, z);
            }

             mStopwatch.Stop();
            Info.text += "\n Completed Vector3 computation: creating random nums" + (mStopwatch.ElapsedMilliseconds) + " ms\n";
 
            HVector3Action(vector3 => HVector3.SqrMagnitude(vector3), " Square Magnitude");
            Vector3Action(vector3 => Vector3.SqrMagnitude(vector3), " Square Magnitude");

            HVector3Action(vector3 => HVector3.Magnitude(vector3), "   Magnitude");
            Vector3Action(vector3 => Vector3.Magnitude(vector3), "   Magnitude");

            HVector3Action(vector3 => HVector3.ClampMagnitude(vector3, Random.Range(1f, 10f)), "   ClampMagnitude");
            Vector3Action(vector3 => Vector3.ClampMagnitude(vector3, Random.Range(1f, 10f)), "   ClampMagnitude");

            HVector3Action(vector3 => HVector3.OrthoNormalize(vector3, vUA, vUB), "   OrthoNormalize");
            Vector3Action(vector3 => Vector3.OrthoNormalize(ref vector3, ref vA, ref vB), "   OrthoNormalize");

            HVector3Action(vector3 => HVector3.ProjectOnPlane(vector3, vUA), "   ProjectOnPlane");
            Vector3Action(vector3 => Vector3.ProjectOnPlane(vector3, vA), "   ProjectOnPlane");


            HVector3Action(vector3 => vector3.Cross( vUA), "   cross");
            Vector3Action(vector3 => Vector3.Cross(vector3, vA), "   cross");

        }

        private void HVector3Action(Action<HVector3> vAction, string vActionName)
        {
            mStopwatch.Reset();


            Info.text += "\n\nStarting HVector computation: " + vActionName + "\n";
            mStopwatch.Start();
             for (int i = 0; i < HVector3s.Length; i++)
            {
                vAction(HVector3s[i]);
            }
             mStopwatch.Stop();
            Info.text += "\n Completed HVector computation: " + vActionName + ": " + (mStopwatch.ElapsedMilliseconds) + "ms \n";
 
        }
        private void Vector3Action(Action<Vector3> vAction, string vActionName)
        {
            mStopwatch.Reset();


            Info.text += "\n\nStarting Vector3 computation: " + vActionName + "\n";
            mStopwatch.Start();
             for (int i = 0; i < Vector3s.Length; i++)
            {
                vAction(Vector3s[i]);
            }
        
            mStopwatch.Stop();
            Info.text += "\n Completed Vector3 computation: " + vActionName + " " + (mStopwatch.ElapsedMilliseconds) + "ms \n";
 
        }
    }
}