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
using Assets.Scripts.Communication;
using Assets.Scripts.Licensing.Controller;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.Localization;
using Assets.Scripts.Notification;
using Assets.Scripts.UI;
using Assets.Scripts.UI.AbstractViews.Permissions;
 using Assets.Scripts.UI.RecordingLoading;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.Utils;
using HeddokoSDK.Models;
using HeddokoSDK.Models.Activity;
using HeddokoSDK.Models.Enum;
using UnityEngine;
using UnityEngine.EventSystems; 

namespace Assets.Scripts.MainApp
{
    public delegate void OnLogout();

    public delegate void OnLogin();
    /// <summary>
    /// The application manager in a windows environment
    /// </summary>
    public class WindowsApplicationManager : MonoBehaviour, IApplicationManager
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
        /// Because of  a  bug in unity, the event system needs to be disabled and reenabled. 
        /// </summary>
        public EventSystem EventSystem;
        public RecordingPlayerView RecordingPlayer;
        public ControlPanel ControlPanel;
        private UserProfileModel mCurrentProfileModel;
        private ActivitiesManager mActivitiesManager;
        public UploadController UploadController;
        public UnityLoginControl LoginController;
        public OrganizationViewController OrganizationViewController;
        public GraphicsQualityManager GraphicsQualityManager;
        void Awake()
        {
            OutterThreadToUnityThreadIntermediary.Instance.Init();
            GraphicsQualityManager.SetToWindowedMode(false);
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
                (UserRolePermission)Attribute.GetCustomAttribute(typeof(RecordingPlayerView), typeof(UserRolePermission));
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
            RegisterNotificationEvents();
            UploadController.Initialize();
            LoginController.Clear();
            if (mCurrentProfileModel.User.RoleType == UserRoleType.LicenseUniversal)
            {
                OrganizationViewController.gameObject.SetActive(true);
            }
            else
            {
                OrganizationViewController.gameObject.SetActive(false);
            }
        }

        public AuthorizationManager AuthorizationManager { get; private set; }

        /// <summary>
        /// Register notification events 
        /// </summary>
        private void RegisterNotificationEvents()
        {
            if (mActivitiesManager == null)
            {
                mActivitiesManager = new ActivitiesManager(mCurrentProfileModel);
            }
            mActivitiesManager.AddNotificationMessageEventHandler(UserEventType.StreamChannelOpened, StreamChannelOpened);
            mActivitiesManager.AddNotificationMessageEventHandler(UserEventType.StreamChannelClosed, StreamChannelClosed);
            mActivitiesManager.AddNotificationMessageEventHandler(UserEventType.LicenseAddedToOrganization, LicenseAddedToOrganization);
            mActivitiesManager.AddNotificationMessageEventHandler(UserEventType.LicenseAddedToUser, LicenseAddedToUser);
            mActivitiesManager.AddNotificationMessageEventHandler(UserEventType.LicenseRemovedFromUser, LicenseRemovedFromUser);
            mActivitiesManager.AddNotificationMessageEventHandler(UserEventType.LicenseChangedForUser, LicenseChangedForUser);
            mActivitiesManager.AddNotificationMessageEventHandler(UserEventType.LicenseExpiring, LicenseExpiring);
            mActivitiesManager.AddNotificationMessageEventHandler(UserEventType.LicenseExpired, LicenseExpired);
            mActivitiesManager.Start();
        }

        /// <summary>
        /// Unregister from notification events.
        /// </summary>
        private void RemoveNotificationEvents()
        {
            mActivitiesManager.RemoveNotificationMessageEventHandler(UserEventType.StreamChannelOpened, StreamChannelOpened);
            mActivitiesManager.RemoveNotificationMessageEventHandler(UserEventType.StreamChannelClosed, StreamChannelClosed);
            mActivitiesManager.RemoveNotificationMessageEventHandler(UserEventType.LicenseAddedToOrganization, LicenseAddedToOrganization);
            mActivitiesManager.RemoveNotificationMessageEventHandler(UserEventType.LicenseAddedToUser, LicenseAddedToUser);
            mActivitiesManager.RemoveNotificationMessageEventHandler(UserEventType.LicenseRemovedFromUser, LicenseRemovedFromUser);
            mActivitiesManager.RemoveNotificationMessageEventHandler(UserEventType.LicenseChangedForUser, LicenseChangedForUser);
            mActivitiesManager.RemoveNotificationMessageEventHandler(UserEventType.LicenseExpiring, LicenseExpiring);
            mActivitiesManager.RemoveNotificationMessageEventHandler(UserEventType.LicenseExpired, LicenseExpired);
            mActivitiesManager.Dispose();
        }

        /// <summary>
        /// License expired notification handler
        /// </summary>
        /// <param name="vObj"></param>
        private void LicenseExpired(NotificationMessage vObj)
        {
            WriteToDebugLog(vObj);
        }

        /// <summary>
        /// License expiring notification handler
        /// </summary>
        /// <param name="vObj"></param>
        private void LicenseExpiring(NotificationMessage vObj)
        {
            DispatchNotification(vObj);
        }


        /// <summary>
        /// License changed for user notication handler
        /// </summary>
        /// <param name="vObj"></param>
        private void LicenseChangedForUser(NotificationMessage vObj)
        {
            DispatchNotification(vObj);
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(ReturnToLoginView);
        }

        private void DispatchNotification(NotificationMessage vObj)
        {

            string vMsg = LocalizationBinderContainer.GetString(KeyMessage.MessageFromServerReceived);
            vMsg += @"\r\n";
            vMsg += vObj.Text;
            vMsg += @"\r\n";
            vMsg += LocalizationBinderContainer.GetString(KeyMessage.ReturnToLoginScreen);
            NotificationManager.CreateNotification(vObj.Text, NotificationManager.NotificationUrgency.Medium);
        }


        /// <summary>
        /// License removed from user  notication handler
        /// </summary>
        /// <param name="vObj"></param>
        private void LicenseRemovedFromUser(NotificationMessage vObj)
        {
            DispatchNotification(vObj);
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                      ReturnToLoginView);
        }

        /// <summary>
        /// License added to user notification handler
        /// </summary>
        /// <param name="vObj"></param>
        private void LicenseAddedToUser(NotificationMessage vObj)
        {
            DispatchNotification(vObj);
        }

        /// <summary>
        /// License added to organization notication handler
        /// </summary>
        /// <param name="vObj"></param>
        private void LicenseAddedToOrganization(NotificationMessage vObj)
        {
            DispatchNotification(vObj);
        }
        /// <summary>
        /// Stream channel closed notication handler
        /// </summary>
        /// <param name="vObj"></param>
        private void StreamChannelClosed(NotificationMessage vObj)
        {
            WriteToDebugLog(vObj);
        }

        /// <summary>
        /// Stream channel opened  notication handler
        /// </summary>
        /// <param name="vObj"></param>
        private void StreamChannelOpened(NotificationMessage vObj)
        {
            WriteToDebugLog(vObj);
        }

        public static void WriteToDebugLog(NotificationMessage vObj)
        {
            WriteToDebugLog(vObj.Text);
        }

        public static void WriteToDebugLog(string vMsg)
        {
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                       () => Debug.Log(vMsg));
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
            UploadController.Stop();
            RemoveNotificationEvents();
            StopCurrentNotifications();
        }

        /// <summary>
        /// Removes notifications from view. 
        /// </summary>
        private void StopCurrentNotifications()
        {
            NotificationManager.StopNotifications();
        }

        void OnApplicationQuit()
        {
            mActivitiesManager.Dispose();
        }
        /// <summary>
        /// exits the application
        /// </summary>
        public void ExitApplication()
        {
            Application.Quit();
        }

    }
}
