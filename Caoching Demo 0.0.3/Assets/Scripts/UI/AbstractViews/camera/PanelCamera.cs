/** 
* @file PanelCamera.cs
* @brief Contains the PanelCamera  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.cameraSubControls;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.camera
{
    /// <summary>
    /// a panel camera that fits within the confines of a node panel
    /// </summary>
    public class PanelCamera : IEquatable<PanelCamera>
    {
        private Guid mId = new Guid();
        private Camera mPanelRenderingCamera;
        private PanelCameraSettings mSettings;
        private CameraOrbitter mCameraOrbitter; 

        /// <summary>
        /// the camera rendering a panel
        /// </summary>
        public Camera PanelRenderingCamera
        {
            get { return mPanelRenderingCamera; }
            set { mPanelRenderingCamera = value; }
        }

        /// <summary>
        /// getter for panel camera settings
        /// </summary>
        public PanelCameraSettings Settings
        {
            get { return mSettings; }
        }

        public CameraOrbitter Orbitter
        {
            get
            {
                if (mCameraOrbitter == null)
                {
                    mCameraOrbitter = PanelRenderingCamera.gameObject.GetComponent<CameraOrbitter>();
                    if (mCameraOrbitter == null)
                    {
                        mCameraOrbitter = PanelRenderingCamera.gameObject.AddComponent<CameraOrbitter>();
                    }
                }
                return mCameraOrbitter;
            }
            set
            {
                mCameraOrbitter = value;
            }
        }

        /// <summary>
        /// Sets up the panel camera with the passed in settings
        /// </summary>
        /// <param name="vSettings"></param>
        public void SetupCamera(PanelCameraSettings vSettings)
        {
            mSettings = vSettings;
            PanelRenderingCamera.clearFlags = CameraClearFlags.Depth;
            PanelRenderingCamera.cullingMask = 1 << vSettings.RenderingLayerMask.value;
            PanelRenderingCamera.orthographic = true;
            PanelRenderingCamera.nearClipPlane = 0.3f;
            PanelRenderingCamera.farClipPlane = 1000f;
            PanelRenderingCamera.orthographicSize = 1.6f;
            PanelRenderingCamera.depth = -1;
            PanelRenderingCamera.rect = new Rect(vSettings.BottomLeftViewPortPoint.x, mSettings.BottomLeftViewPortPoint.y, mSettings.TopRightViewPortPoint.x, mSettings.TopRightViewPortPoint.y);
            PanelRenderingCamera.transform.position = Vector3.back * 10;
          
        }

        public bool Equals(PanelCamera other)
        {
            if (other != null)
            {
                return other.mId == mId;
            }
            return false;
        }
        /// <summary>
        /// Updates the current LayerMask with the passed in parameter
        /// </summary>
        /// <param name="vNewLayerMask"></param>
        public void UpdateLayerMask(LayerMask vNewLayerMask)
        {
            Settings.RenderingLayerMask = vNewLayerMask;
            PanelRenderingCamera.cullingMask = Settings.RenderingLayerMask;

        }

        /// <summary>
        /// sets up the cameras position with respect the rendered body
        /// </summary> 
        /// <param name="vBody"></param>
        /// <param name="vDistance"></param>
        public void SetCameraTarget(RenderedBody vBody, int vDistance)
        {
            Transform vTarget = vBody.Hips;
            Vector3 vPos = vTarget.forward * vDistance;
            PanelRenderingCamera.transform.position = vPos;
            PanelRenderingCamera.transform.LookAt(vTarget);
            Orbitter.Target = vTarget;
            Orbitter.TargetsLayer = 1<< vBody.CurrentLayerMask.value;
            Orbitter.Enable();
          

        }
    }
}
