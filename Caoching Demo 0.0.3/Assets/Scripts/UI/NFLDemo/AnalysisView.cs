
/**  
* @file CameraController.cs 
* @brief Contains the CamRotate class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved 
*/

using System;
using UnityEngine;

namespace Assets.Scripts.UI.NFLDemo
{
    /// <summary>
    /// Provides a view for an analyisis
    /// </summary>
    [Serializable]
    public class AnalysisView : MonoBehaviour
    {
        [SerializeField]
        private bool mInView;
        /// <summary>
        /// Flag indicating that the view is show in screen. Flag is set by view/hide methods. Private setter
        /// </summary>
        public bool InView
        {
            get
            {
                return mInView;
            }
            private set
            {
                mInView = value;
            }
        }

        public void Show()
        {
            InView = true;
        }


        public void Hide()
        {
            InView = false;
        }

    }
}
