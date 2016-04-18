/** 
* @file CameraZoom.cs
* @brief Contains the CameraZoom class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;

namespace Assets.Scripts.Utils.UnityUtilities
{
    /**
    * CameraZoom class 
    * @brief CameraZoom class, allowing the camera to  zoom in with a middle button
    * @note something 
    */

    public class CameraZoom : MonoBehaviour
    {
        public float MaxZoomOutFov = 60;
        public float MaxZoomInFov = 20;

        public float MaxOrthoSizeZoomOut = 3f;
        public float MaxOrthoSizeZoomIn = 0.2f;

        public float ZoomSpeed = 8f;
        private Camera mCurrentCam;

        void Start()
        {
            mCurrentCam = GetComponent<Camera>();
        }

        void Update()
        {
            float vMiddleMouse = Input.GetAxis("Mouse ScrollWheel"); 
            if (vMiddleMouse < 0)
            {
                if (!mCurrentCam.orthographic)
                {
                    mCurrentCam.fieldOfView = Mathf.Lerp(mCurrentCam.fieldOfView, MaxZoomOutFov, Time.deltaTime * ZoomSpeed);
                }
                else
                {
                    mCurrentCam.orthographicSize = Mathf.Lerp(mCurrentCam.orthographicSize, MaxOrthoSizeZoomOut, Time.deltaTime * ZoomSpeed);
                }
            }
            if (vMiddleMouse > 0)
            {
                if (!mCurrentCam.orthographic)
                {
                    mCurrentCam.fieldOfView = Mathf.Lerp(mCurrentCam.fieldOfView, MaxZoomInFov, Time.deltaTime * ZoomSpeed);
                }
                else
                {
                    mCurrentCam.orthographicSize = Mathf.Lerp(mCurrentCam.orthographicSize, MaxOrthoSizeZoomIn, Time.deltaTime * ZoomSpeed);

                }
            }
        }
    }
}
