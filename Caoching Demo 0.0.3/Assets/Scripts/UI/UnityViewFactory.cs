/**
* @file UnityViewFactory.cs
* @brief Contains the UnityViewFactory
* @author Mohammed Haider(UnityViewFactory 
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System.Collections;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.MainApp;
using Assets.Scripts.Tests;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{

    public class UnityViewFactory : MonoBehaviour, IViewFactory
    {
        public GameObject LoginRootView;
        public GameObject MainAppRootView;
        /// <summary>
        /// Construct the application view based on the user role and the platform passed in. 
        /// </summary>
        /// <param name="vUserProfile">The profile of the user to load</param>
        /// <param name="vPlatformType"> The platform the construct a scene.</param>
        public void Construct(UserProfileModel vUserProfile, PlatformType vPlatformType)
        {
            MainAppRootView.SetActive(true);
            var vGo = GameObject.FindGameObjectWithTag("ApplicationManagement");
            if (vGo != null)
            {
                var vAppManager = vGo.GetComponent<IApplicationManager>();
                vAppManager.Init(vUserProfile);
                //unload scene after initialization
                LoginRootView.SetActive(false);
            }
        }

        private IEnumerator Load(UserProfileModel vUserProfile, PlatformType vPlatformType)
        {
            var vAsynOperation = SceneManager.LoadSceneAsync("AppMain", LoadSceneMode.Additive);
            while (!vAsynOperation.isDone)
            {
                yield return null;
            }
            var vGo = GameObject.FindGameObjectWithTag("ApplicationManagement");
            if (vGo != null)
            {
                var vAppManager = vGo.GetComponent<IApplicationManager>();
                vAppManager.Init(vUserProfile);
                //unload scene after initialization
                SceneManager.UnloadScene("LoginView");
            }
        }
    }
}