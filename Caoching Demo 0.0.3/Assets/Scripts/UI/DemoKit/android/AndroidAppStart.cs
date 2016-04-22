/** 
* @file AndroidAppStart.cs
* @brief Contains the AndroidAppStart  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Body_Data.View;
using Assets.Scripts.UI.AbstractViews.camera; 
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.UI.DemoKit.android
{
    /// <summary>
    /// A helper class that start up specific resources for android
    /// </summary>
    public class AndroidAppStart : MonoBehaviour
    {
        void Awake()
        {
            OutterThreadToUnityThreadIntermediary.Instance.Init();
            BodySegment.IsTrackingHeight = false;
            InitiliazePools(); 
        }
        /// <summary>
        /// Sets up internal pools
        /// </summary>
        private void InitiliazePools()
        {
            GameObject vRenderedBodyGroup = GameObject.FindWithTag("RenderedBodyGroup");
            GameObject vPanelCameraGroup = GameObject.FindWithTag("PanelCameraGroup");

            RenderedBodyPool.ParentGroupTransform = vRenderedBodyGroup.transform;
            PanelCameraPool.CameraParent = vPanelCameraGroup.transform;

        }
    }
}