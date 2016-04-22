/** 
* @file PanelCameraSettings.cs
* @brief Contains the PanelCameraSettings  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews.Layouts;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.camera
{
    /// <summary>
    /// Contains settings for the panel camera
    /// </summary>
    public class PanelCameraSettings
    {
        private LayerMask mRenderingLayerMask;
        private Vector2 mBottomLeftViewPortPoint;
        private Vector2 mTopRightViewPortPoint;
      
        private float mDepth = 1000f;
  
        /// <summary>
        /// constructor that use's a panels' settings in order to assign minsize, maxSize, height and width
        /// to be used by a camera
        /// </summary>
        /// <param name="vMask"></param>
        /// <param name="vPanelSettings"></param>
        public PanelCameraSettings(LayerMask vMask, PanelSettings vPanelSettings)
        {
            mRenderingLayerMask = vMask;

            RectTransform vRectTransform = vPanelSettings.RectTransform;   
            Vector3[] vCorners =  new Vector3[  4];
            vRectTransform.GetWorldCorners(vCorners);

            vCorners[0].z = Camera.main.transform.position.z;
            vCorners[2].z = Camera.main.transform.position.z;

            Vector2 vBottomLeft = RectTransformUtility.WorldToScreenPoint(Camera.main, vCorners[0]);
            vBottomLeft = Camera.main.ScreenToViewportPoint(vBottomLeft);

            Vector2 vTopRight = RectTransformUtility.WorldToScreenPoint(Camera.main, vCorners[2]);
            vTopRight = Camera.main.ScreenToViewportPoint(vTopRight);

            mTopRightViewPortPoint = vTopRight;
            mBottomLeftViewPortPoint = vBottomLeft;
          
        }

        public LayerMask RenderingLayerMask
        {
            get { return mRenderingLayerMask; }
            set { mRenderingLayerMask = value; }
        }



  

        public Vector2 BottomLeftViewPortPoint
        {
            get { return mBottomLeftViewPortPoint; }
        }

        public Vector2 TopRightViewPortPoint
        {
            get { return mTopRightViewPortPoint; }
        }
    }
}