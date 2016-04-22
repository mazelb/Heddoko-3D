/** 
* @file PanelCameraToBodyPair.cs
* @brief Contains the PanelCameraToBodyPair  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.camera
{
    /// <summary>
    /// A one to one relationship between a rendered body and a camera rendering it
    /// </summary>
    public class PanelCameraToBodyPair
    { 
        private LayerMask mRenderingLayerMask;
        private PanelCamera mPanelCamera;
        private Body mBody;

    
        public Body Body
        {
            get { return mBody; }
            set { mBody = value; }
        }

        public PanelCamera PanelCamera
        {
            get
            {
                return mPanelCamera;
            }
            set { mPanelCamera = value; }
        }

        public LayerMask RenderingLayerMask
        {
            get { return mRenderingLayerMask; }
            set { mRenderingLayerMask = value; }
        }
    }
}
