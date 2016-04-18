/** 
* @file HipExensionView.cs
* @brief Contains the HipExensionView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine;

namespace Assets.Scripts.UI._2DSkeleton
{
    /// <summary>
    /// Represents the view the hip extension
    /// </summary>
   public class HipExensionView : MonoBehaviour
    {
        public Transform HipMotor;
        public Transform KneeJoint;
        public Axis2D Axis;


        void Awake()
        {
            Axis.Initialize(transform);
        }



    }
}
