/** 
* @file SubViewCameraPool.cs
* @brief Contains the SubViewCameraPool abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections.Generic;
using Assets.Scripts.Cameras;
using UnityEngine; 

namespace Assets.Scripts.UI.Analysis
{
    /// <summary>
    /// Subview camera pool. Returns a camera control object with an associated camera
    /// </summary>
    public static class SubViewCameraPool
    {
        private static List<AnalysisSubViewCamControl> sAvailableCamControls = new List<AnalysisSubViewCamControl>();
        private static List<AnalysisSubViewCamControl> sInUseCamControls = new List<AnalysisSubViewCamControl>();
        public static Transform CameraParent;
        public static Transform OriginalTarget { get; set; }
        public static int CullingMask { get; set; }
        public static int Depth { get; set; }
        public static AnalysisSubViewCamControl GetCamControl
        {
            get
            {
                if (sAvailableCamControls.Count != 0)
                {
                    AnalysisSubViewCamControl vPooledObject = sAvailableCamControls[0];
                    sInUseCamControls.Add(vPooledObject);
                    sAvailableCamControls.RemoveAt(0);
                    vPooledObject.gameObject.SetActive(true);
                    PrepCameraTransform(vPooledObject);
                    return vPooledObject;
                }
                else
                {
                    GameObject vPooledGo = new GameObject();
                    Camera vCam = vPooledGo.AddComponent<Camera>();
                    vPooledGo.transform.parent = CameraParent.transform;
                    AnalysisSubViewCamControl vSubcamCtrl = vPooledGo.AddComponent<AnalysisSubViewCamControl>();
                    vSubcamCtrl.AssociatedCamera = vCam;
                    vPooledGo.name = "AnalysisSubViewCamera";
                    
                    PrepCameraTransform(vSubcamCtrl);

                    return vSubcamCtrl;
                } 
            }
        }

        /// <summary>
        /// Prepares the camera's transform in order to look at the target from the forward direction 
        /// </summary>
        private static void PrepCameraTransform(AnalysisSubViewCamControl vSubcamCtrl )
        {
            Camera vCam = vSubcamCtrl.AssociatedCamera;
            vCam.clearFlags = CameraClearFlags.Depth;
            vCam.cullingMask = CullingMask;
            vCam.orthographic = true;
            vCam.nearClipPlane = 0.3f;
            vCam.farClipPlane = 1000f;
            vCam.orthographicSize = 1.6f;
            vCam.depth = Depth;
            vSubcamCtrl.DistanceFromTarget = 12f;
            vSubcamCtrl.Target = OriginalTarget;
            
            vSubcamCtrl.MoveSpeed = 1f;
            //set the camera to be directly in front of the target's forward by a constant Distance
            Vector3 vNewPosition = OriginalTarget.position + (OriginalTarget.forward.normalized * vSubcamCtrl.DistanceFromTarget);
            vSubcamCtrl.SetCameraPos(vNewPosition);
            vSubcamCtrl.LookAtTarget();
            sInUseCamControls.Add(vSubcamCtrl);
        }

        public static void Release(AnalysisSubViewCamControl vSubViewCam)
        { 
            sInUseCamControls.Remove(vSubViewCam);
            sAvailableCamControls.Add(vSubViewCam); 
            vSubViewCam.gameObject.SetActive(false); 
        }
    }
}
