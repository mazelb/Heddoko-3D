/** 
* @file LoadingBoard.cs
* @brief Contains the LoadingBoard class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Loading
{
    /// <summary>
    /// A basic loading screen
    /// </summary>
    public class LoadingBoard : MonoBehaviour
    {
        private static LoadingBoard mInstance;


        //when the StartLoadingAnimation starts, the request counter is incremented until the StopLoading is called, then
        //is incremented by 
        private static int sCounter = 0;
        public float StartTime = 0;
        //time it takes to complete a whole revolution
        public float RevolutionTime = 0.5f;
        private bool mContinueAnimation = false;
        public Image DisablingPanel;
        /// <summary>
        /// the image that will be animated
        /// </summary>
        public MaskableGraphic LoadingImage;

        public float AngleSpeed = 1f;

        public static LoadingBoard Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = FindObjectOfType<LoadingBoard>();
                }
                return mInstance;
            }
            private set { mInstance = value; }
        }


        /// <summary>
        /// Starts the loading animation. If one is already in progress, continue
        /// </summary>
        public static void StartLoadingAnimation()
        {
            //Instance.StartTime = Time.time;                     

            if (++sCounter == 1)
            {
                Instance.mContinueAnimation = true;
                Instance.DisablingPanel.enabled = true;
                Instance.LoadingImage.enabled = true;
                Instance.StartCoroutine(Instance.BeginLoadAnimation());
            }

        }

        public static void UpdateAnimation()
        {
            Vector3 vEuler = Instance.LoadingImage.transform.eulerAngles;
            vEuler.z += 0.002f;
            Instance.LoadingImage.transform.rotation = Quaternion.Euler(vEuler);
        }
        /// <summary>
        /// Attempts to stop the loading animation. If there are other requesters that haven't made the call to stop the animation
        /// then the animation will continue loading
        /// </summary>
        public static void StopLoadingAnimation()
        {
            if (--sCounter < 0)
            {
                sCounter = 0;
            }
            if (sCounter == 0)
            {
                Instance.LoadingImage.enabled = false;
                Instance.DisablingPanel.enabled = false;
                Instance.mContinueAnimation = false;
            }

        }

        public static void ForceStopAnimation()
        {
            sCounter = 0;
            Instance.LoadingImage.enabled = false;
            Instance.DisablingPanel.enabled = false;
            Instance.mContinueAnimation = false;
        }


        /// <summary>
        /// Helper method to stop animation
        /// </summary>
        private void StopAnimation()
        {
            mContinueAnimation = false;
        }

        /// <summary>
        /// Helper method to start animation
        /// </summary>
        private void StartAnimation()
        {
            mContinueAnimation = true;
            StartCoroutine(BeginLoadAnimation());
        }

        private IEnumerator BeginLoadAnimation()
        {
            Vector3 vEuler = LoadingImage.transform.eulerAngles;

            while (mContinueAnimation)
            {
                vEuler.z += AngleSpeed * Time.deltaTime;
                LoadingImage.transform.rotation = Quaternion.Euler(vEuler);
                yield return null;
            }

        }


        void Awake()
        {
            Instance = this;
        }
    }
}
