/** 
* @file ShadedAngleArea.cs
* @brief Contains the ShadedAngleArea class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;
using Assets.Scripts.Utils;
using UnityEngine.UI;

namespace Assets.Scripts.UI._2DSkeleton
{

    /// <summary>
    /// Draws a shaded angle between two transforms
    /// </summary>
    public class ShadedAngleArea : MonoBehaviour
    { 
  
        public Image AngleArc;
        private bool mShowAngle;
        public Transform Target;

        public bool ShowAngle
        {
            get { return mShowAngle; }
            set
            {
                if (!value)
                {
                    AngleArc.fillAmount = 0;
                }
                mShowAngle = value;
            }
        }

        /// <summary>
        /// Set the fill
        /// </summary>
        /// <param name="vAngleValue"></param>
        public void SetFill(float vAngleValue)
        {
          
            if (gameObject.name == "KneeAnglesright")
            {
                string s = "";
            }
            if (ShowAngle)
            {
                 
                float vAngle = HeddokoMathTools.ClampAngle(vAngleValue, -360f, 360f);
                AngleArc.fillAmount = Mathf.Abs(vAngle / 360f);
            }
           
        }

 
 


    }
}
