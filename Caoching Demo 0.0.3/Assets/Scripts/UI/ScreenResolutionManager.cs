/**
* @file ScreenResolutionManager.cs
* @brief Contains the ScreenResolutionManager
* @author Mohammed Haider( mohammed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using System.Collections;
using UnityEngine;
using Object = System.Object;


namespace Assets.Scripts.UI
{

    public delegate void NewResolutionSet();

    public delegate void AllResolutionsScanned(Resolution[] vResolutions);
    /// <summary>
    /// Manages the screen's resolution
    /// </summary>
    public class ScreenResolutionManager : MonoBehaviour
    {
        private static ScreenResolutionManager sInstance;

        public event NewResolutionSet NewResolutionSetEvent;
        public event AllResolutionsScanned AllResolutionsScannedEvent;
        /// <summary>
        /// a valid aspect ratio. All resolution decisions are based off this
        /// </summary>
        private static float sValidAspectRatio = 16f / 9f;

        private Resolution[] mSupportedResolutions;
        private bool mIsFullscreen = true;
        public Resolution CurrentSelectedResolution;
        /// <summary>
        /// Global function to set the new valid aspect ratio
        /// </summary>
        /// <param name="vWidth">the new width</param>
        /// <param name="vHeight">the new height</param>
        public static void SetNewValidAspectRatio(float vWidth, float vHeight)
        {
            sValidAspectRatio = vWidth / vWidth;
        }

        /// <summary>
        /// The valid aspect ratio
        /// </summary>
        public static float ValidAspectRatio
        {
            get { return sValidAspectRatio; }
        }

        public static ScreenResolutionManager Instance
        {
            get
            {
                if (sInstance == null)
                {
                    var vObj = GameObject.FindGameObjectWithTag("Controllers");
                    var vNewObj = new GameObject("Screen Resolution Manager");
                    if (vObj != null)
                    {
                        vNewObj.transform.SetParent(vObj.transform);
                    }
                    sInstance = vNewObj.AddComponent<ScreenResolutionManager>();
                    sInstance.Initialize();
                }
                return sInstance;
            }
            set { sInstance = value; }
        }

        /// <summary>
        /// Returns a collection of all supported resolution
        /// </summary>
        /// <returns>Collection of supported Resolution</returns>
        public static Resolution[] GetAllSupportedResolution()
        {
            return Instance.mSupportedResolutions;
        }



        /// <summary>
        /// Initializes supported resolutions
        /// </summary>
        private void Initialize()
        {
            mSupportedResolutions = Screen.resolutions;
            if (AllResolutionsScannedEvent != null)
            {
                AllResolutionsScannedEvent(mSupportedResolutions);
            }
        }

        /// <summary>
        /// Sets the resolution of the application.
        /// </summary>
        /// <param name="vResolution">Resolution parameters</param>
        /// <param name="vIsFullScreen">set to fullscreen </param>
        /// <param name="vCallbackAction">callback action on set completion</param>
        public static void SetScreenResolution(Resolution vResolution, bool vIsFullScreen, Action vCallbackAction)
        {
            bool vResolutionSetSuccess = false;
            try
            {
                //compare square magnitudes of both passed in resolution and the current resolution.
                //if they are comparably equal, then don't set the resolution

                Vector2 vCurrentRes = new Vector2(Instance.CurrentSelectedResolution.width, Instance.CurrentSelectedResolution.height);
                Vector2 vNewResolution = new Vector2(vResolution.width, vResolution.height);
                float vCompare = Mathf.Abs(vCurrentRes.sqrMagnitude - vNewResolution.sqrMagnitude);
                if (vCompare > 0.1f)
                {
                    Instance.CurrentSelectedResolution = vResolution;
                    Instance.mIsFullscreen = vIsFullScreen;
                    Screen.SetResolution(vResolution.width, vResolution.height, Instance.mIsFullscreen, vResolution.refreshRate);
                    vResolutionSetSuccess = true;
                }

            }
            catch (Exception vErr)
            {
                vResolutionSetSuccess = false;
            }


            if (vResolutionSetSuccess)
            {
                Instance.StartCoroutine(Instance.InvokeOnNextFrameUpdate(vCallbackAction, vResolution));
            }
        }

        /// <summary>
        /// Invokes a callback after the next frame update
        /// </summary>
        /// <param name="vCallbackAction">callback action to invoke</param>
        /// <param name="vResolution"></param>
        /// <returns></returns>
        private IEnumerator InvokeOnNextFrameUpdate(Action vCallbackAction, Resolution vResolution)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            if (vCallbackAction != null)
            {
                vCallbackAction.Invoke();
            }
            TriggerNewResolutionChangeHandler();
        }

        /// <summary>
        /// Selects the resolution by its ID
        /// </summary>
        /// <param name="vArgs"></param>
        public static void SelectResolutionId(int vArgs)
        {
            SetScreenResolution(Instance.mSupportedResolutions[vArgs], true, null);
        }

        /// <summary>
        /// Triggers handlers
        /// </summary>
        public void TriggerNewResolutionChangeHandler()
        {
            if (NewResolutionSetEvent != null)
            {
                NewResolutionSetEvent();
             }
        }
    }
}