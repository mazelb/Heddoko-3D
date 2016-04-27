/** 
* @file PanelCamera.cs
* @brief Contains the PanelCamera  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.cameraSubControls;
using UnityEngine;
using UnityEngine.EventSystems;

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
        private CameraZoom mCameraZoom;
        internal CamViewPlane CamViewPlane = new CamViewPlane();
        internal PhysicsRaycaster PhysicsRaycaster;
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

        public CameraZoom CameraZoom
        {
            get
            {
                if (mCameraZoom == null)
                {
                    mCameraZoom = PanelRenderingCamera.gameObject.GetComponent<CameraZoom>();
                    if (mCameraZoom == null)
                    {
                        mCameraZoom = PanelRenderingCamera.gameObject.AddComponent<CameraZoom>();
                    }
                }
                return mCameraZoom;
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
      PanelRenderingCamera.rect = new Rect(mSettings.BottomLeftViewPortPoint.x, mSettings.BottomLeftViewPortPoint.y, mSettings.TopRightViewPortPoint.x - mSettings.BottomLeftViewPortPoint.x, mSettings.TopRightViewPortPoint.y - mSettings.BottomLeftViewPortPoint.y);
            
            PanelRenderingCamera.transform.position = Vector3.back * 10;
            CamViewPlane.CameraPositionChangedEvent -= MoveCameraToPosition;
            CamViewPlane.CameraPositionChangedEvent += MoveCameraToPosition;
            CameraZoom.Camera = PanelRenderingCamera;
            PhysicsRaycaster =PanelRenderingCamera.gameObject.AddComponent<PhysicsRaycaster>();
            PhysicsRaycaster.eventMask = 1 << mSettings.RenderingLayerMask.value;


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
        public void SetDefaultTarget(RenderedBody vBody, int vDistance)
        {
            CamViewPlane.TargetBody = vBody;
            Transform vTarget = CamViewPlane.TargetBody.Hips;
            Orbitter.Target = vTarget;
            Orbitter.TargetsLayer = 1 << vBody.CurrentLayerMask.value;
            Orbitter.Enable();
            CameraZoom.TargetRenderedBody = vBody;
            CameraZoom.LookAtSubsegmentType = BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine;
            CameraZoom.TargetsLayer = 1 << vBody.CurrentLayerMask.value;
            CameraZoom.Enable();
            CamViewPlane.ReferenceSubSegment = BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine;
            CamViewPlane.ViewPlane = CameraViewPlane.Frontal;
        }

        

        /// <summary>
        /// Moves the camera to a the specified new position
        /// </summary>
        /// <param name="vNewPos">the new position</param>
        private void MoveCameraToPosition(Vector3 vNewPos)
        {
            PanelRenderingCamera.transform.position = vNewPos;
            PanelRenderingCamera.transform.LookAt(CamViewPlane.TargetBody.GetSubSegment(CamViewPlane.ReferenceSubSegment));
        }
    }


}
