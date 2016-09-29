// /**
// * @file WindowsApplicationManager.cs
// * @brief Contains the WindowsApplicationManager
// * @author Mohammed Haider( mohammed @ heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.UI.AbstractViews.Permissions;
using Assets.Scripts.UI.RecordingLoading;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.Utils;
using HeddokoSDK;
using HeddokoSDK.Models;
using UnityEngine;
using UnityEngine.EventSystems; 

namespace Assets.Scripts.MainApp
{
    public delegate void OnLogout();

    public delegate void OnLogin();
    /// <summary>
    /// The application manager in a windows environment
    /// </summary>
    public class WindowsApplicationManager: MonoBehaviour,IApplicationManager
    {
        /// <summary>
        /// Worker role objects to disable
        /// </summary>
        public GameObject[] DisableWorkerRoleObjects;

        public event OnLogin OnLoginEvent;
        public event OnLogout OnLogoutEvent;
        public GameObject LoginViewRoot;
        public GameObject ApplicationRoot;
        /// <summary>
        /// Because of a bug in unity, the event system needs to be disabled and reenabled. 
        /// </summary>
        public EventSystem EventSystem;
        public RecordingPlayerView RecordingPlayer ;
        public ControlPanel ControlPanel;
        private UserProfileModel mCurrentProfileModel;

        void Awake()
        {
            OutterThreadToUnityThreadIntermediary.Instance.Init();
        }
        
        /// <summary>
        /// Initializes the application with the user profile model passed in. 
        /// </summary>
        /// <param name="vProfileModel"></param>

        public void Init(UserProfileModel vProfileModel)
        {
            mCurrentProfileModel = vProfileModel;
            var vUserRole = vProfileModel.User.RoleType;
            var vAttribute =
                (UserRolePermission) Attribute.GetCustomAttribute(typeof (RecordingPlayerView), typeof (UserRolePermission));
            List<UserRoleType> vType = vAttribute.AllowedRoles.ToList();
            if (vUserRole == UserRoleType.Worker)
            {
                foreach (var vDisableUserRoleObject in DisableWorkerRoleObjects)
                {
                    vDisableUserRoleObject.SetActive(false);
                }
            }
            StartCoroutine(FlipEventSystemStates());
            UserSessionManager.Instance.UserProfile = mCurrentProfileModel;
            if (OnLoginEvent != null)
            {
                OnLoginEvent();
            }
        }
 
        /// <summary>
        /// Flips the event system state, between inactive and active. 
        /// </summary>
        /// <returns></returns>
        IEnumerator FlipEventSystemStates()
        {
            EventSystem.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.6f);
            EventSystem.gameObject.SetActive(true);
        }
        /// <summary>
        /// Sets the recording player sub control panels.
        /// </summary>
        /// <param name="vView"></param>
        private void SetPermissions(RecordingPlayerView vView)
        {
            vView.SetPermissions(mCurrentProfileModel);
        }

        public void ReturnToLoginView()
        {
            ApplicationRoot.gameObject.SetActive(false);
            LoginViewRoot.gameObject.SetActive(true);
            foreach (var vDisableWorkerRoleObject in DisableWorkerRoleObjects)
            {
                vDisableWorkerRoleObject.gameObject.SetActive(true);
            }
            if (OnLogoutEvent != null)
            {
                OnLogoutEvent();
            }
        }

        public void ExitApplication()
        {
            Application.Quit(); 
        }
 
    }
}