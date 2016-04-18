/** 
* @file SplashScreen.cs
* @brief Contains the SplashScreen class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/


using Assets.Scripts.UI.MainMenu.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainMenu.View
{
    /**
    * SplashScreen class 
    * @brief SplashScreen class Disables the splash screen after a set time, then enables the main
    * menu 
    */ 
    public class SplashScreen : MonoBehaviour
    {
        public MainMenuController MainMenuController;
        public Image SplashScreenImage;
        public float DisableTime = 1.5f;

        /// <summary>
        /// On Awake set the alpha to 1
        /// </summary>
        void Awake()
        { 
            Color vSplashScreenColor = SplashScreenImage.color;
            vSplashScreenColor.a = 1;
            SplashScreenImage.color = vSplashScreenColor;
        }
        void Update()
        {
            DisableTime -= Time.deltaTime;
            if (DisableTime < 0)
            {
                //MainMenuObj.SetActive(true);
                MainMenuController.SplashScreenTransitionFinished();
                gameObject.SetActive(false);
            }
        }
    }
}
