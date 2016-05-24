/** 
* @file PanelCameraPool.cs
* @brief Contains the PanelCameraPool  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
 
using UnityEngine;
using System.Collections.Generic;
namespace Assets.Scripts.UI.AbstractViews.camera
{
    /// <summary>
    /// A pool of Panel cameras
    /// </summary>
    public class PanelCameraPool
    {

        private static List<PanelCamera> sAvailablePanelCams = new List<PanelCamera>();
        private static List<PanelCamera> sInUseCameras = new List<PanelCamera>();
        private static Transform sCameraParent;

        public static Transform CameraParent { get; set; }

        /// <summary>
        /// Get a panel camera resource from the available pool
        /// </summary>
        /// <param name="vSettings"></param>
        /// <returns></returns>
        public static PanelCamera GetPanelCamResource(PanelCameraSettings vSettings)
        { 
            if (sAvailablePanelCams.Count != 0)
            {
                PanelCamera vPooledCamObj = sAvailablePanelCams[0];
                sInUseCameras.Add(vPooledCamObj);
                sAvailablePanelCams.RemoveAt(0);
                vPooledCamObj.PanelRenderingCamera.gameObject.SetActive(false);
                vPooledCamObj.SetupCamera(vSettings);
                vPooledCamObj.PanelRenderingCamera.gameObject.SetActive(true);
                ScreenResolutionManager.Instance.NewResolutionSetEvent += vPooledCamObj.CalculateCamRect;

                return vPooledCamObj;
            }
            else
            {
                GameObject vPooledCamObj = new GameObject();
                Camera vCam = vPooledCamObj.AddComponent<Camera>();
                vPooledCamObj.transform.parent = CameraParent.transform;
                PanelCamera vSubcamCtrl = new PanelCamera();
                vSubcamCtrl.PanelRenderingCamera = vCam;
                vSubcamCtrl.SetupCamera(vSettings);
                vPooledCamObj.name = "PanelCamera";
                ScreenResolutionManager.Instance.NewResolutionSetEvent += vSubcamCtrl.CalculateCamRect;
                return vSubcamCtrl;
            }

        }

        /// <summary>
        /// Releases the camera into the free pooled.
        /// </summary>
        /// <param name="vPanelCam">the panel camera to release</param>
        public static void Release(PanelCamera vPanelCam)
        {
            ScreenResolutionManager.Instance.NewResolutionSetEvent -= vPanelCam.CalculateCamRect;
            sInUseCameras.Remove(vPanelCam);
            sAvailablePanelCams.Add(vPanelCam);
            vPanelCam.PanelRenderingCamera.gameObject.SetActive(false);
        }
    }
}