/**
* @file DisablingProgressBar.cs
* @brief Contains the 
* @author Mohammed Haider( mohammed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Loading
{
    /// <summary>
    /// Disables all content by placing a masking image on top of all elements. 
    /// </summary>
    public class DisablingProgressBar: MonoBehaviour
    {
        public Progressbar ProgressBar;
        public Image MaskingImage;
        public Text Text;

        private static DisablingProgressBar sInstance;

        public static DisablingProgressBar Instance
        {
            get
            {
                if (sInstance == null)
                {
                    var vGo =GameObject.FindGameObjectWithTag("LoadingPanel") as GameObject;
                    if (vGo != null)
                    {
                        sInstance = vGo.GetComponent<DisablingProgressBar>();
                    }
                }

                return sInstance;
            }
        }

        /// <summary>
        /// Starts the progress bar and optionally use a label
        /// </summary>
        /// <param name="vText"></param>
        public void StartProgressBar(string vText = "")
        {
            gameObject.SetActive(true);
            ProgressBar.gameObject.SetActive(true);
            Text.gameObject.SetActive(true);
            Text.text = vText;
            MaskingImage.enabled = true;
            ProgressBar.Animate();
        }
        /// <summary>
        /// Stops the progress bar animation
        /// </summary>
        public void StopAnimation( )
        {
            gameObject.SetActive(false);
            ProgressBar.Stop();
            Text.gameObject.SetActive(false);
            MaskingImage.enabled = false;
            ProgressBar.gameObject.SetActive(false);
        }
    }
}