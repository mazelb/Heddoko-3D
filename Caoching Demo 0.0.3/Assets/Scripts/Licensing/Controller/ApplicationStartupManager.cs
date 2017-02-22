/**
* @file ApplicationStartupManager.cs
* @brief Contains the ApplicationStartupManager
* @author Mohammed Haider(mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using Assets.Scripts.Licensing.Authentication;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.Localization;
using Assets.Scripts.MainApp;
using Assets.Scripts.UI;
using HeddokoSDK.Models;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.Licensing.Controller
{

    /// <summary>
    /// The startup manager of the application 
    /// </summary>
    public class ApplicationStartupManager : MonoBehaviour, IApplicationStartupManager
    {
        public UnityLoginControl LoginControl;
        public ApplicationBouncer Bouncer = new ApplicationBouncer();
        public UnityViewFactory UnityViewFactory;
        public HeddokoAppStart Appstart;
        public IViewFactory ViewFactory { get; private set; }
        /// <summary>
        /// Initializes parameters
        /// </summary>
        internal void Awake()
        {
            Init();
            ViewFactory = UnityViewFactory;
        }
        /// <summary>
        /// Initializes the object
        /// </summary>
        public void Init()
        {
            Bouncer.RegisterLicenseAccessActionEvent(LicenseStatusType.Active, ActiveLicenseHandler);
            Bouncer.RegisterLicenseAccessActionEvent(LicenseStatusType.Deleted, DeletedLicenseHandler);
            Bouncer.RegisterLicenseAccessActionEvent(LicenseStatusType.Expired, ExpiredLicenseHandler);
            Bouncer.RegisterLicenseAccessActionEvent(LicenseStatusType.Inactive, InActiveLicenseHandler);
            Bouncer.RegisterUserAccessActionEvent(UserStatusType.Active, ActiveUserHandler);
            Bouncer.RegisterUserAccessActionEvent(UserStatusType.Banned, BannedUserHandler);
            Bouncer.RegisterUserAccessActionEvent(UserStatusType.NotActive, InactiveUserHandler);
            LoginControl.RegisterOnLoginEvent(Bouncer.ValidateUser);
        }

        /// <summary>
        /// Handler for active user event
        /// </summary> 
        public void ActiveUserHandler(UserProfileModel vProfileModel)
        {
            //Ask the bouncer if the license if valid
            if (vProfileModel.LicenseInfo != null)
            {
                Bouncer.ValidateLicense(vProfileModel);
            }
            else
            {
                LoginControl.LoginController.RaiseErrorEvent(LoginErrorType.NullLicense, "There is no license associated with this account.");
            }
        }
        /// <summary>
        /// Handler for banned user
        /// </summary> 
        public void BannedUserHandler(UserProfileModel vProfileModel)
        {
            string vMsg = LocalizationBinderContainer.GetString(KeyMessage.BannedAccountMsg);
              //   "This account has been banned. ";
            Notify.Template("fade")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true);
            LoginControl.EnableControls();

        }
        /// <summary>
        /// Handler for inactive user event
        /// </summary> 
        public void InactiveUserHandler(UserProfileModel vProfileModel)
        {
            string vMsg = LocalizationBinderContainer.GetString(KeyMessage.InactiveAccountMsg);
          //  "This account is not active. Please contact your license administrator for further support.";
            Notify.Template("fade")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true);
            LoginControl.EnableControls();

        }

        /// <summary>
        /// Handler for active license event
        /// </summary> 
        public void ActiveLicenseHandler(UserProfileModel vProfileModel)
        {
            ViewFactory.Construct(vProfileModel, PlatformType.Windows);
        }
        /// <summary>
        ///Handler for inactive license event
        /// </summary> 
        public void InActiveLicenseHandler(UserProfileModel vProfileModel)
        {
            string vMsg = LocalizationBinderContainer.GetString(KeyMessage.InactiveLicenseMsg);
            Notify.Template("fade")
               .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true);
        }
        
        /// <summary>
        /// Handler for expired license event
        /// </summary> 
        public void ExpiredLicenseHandler(UserProfileModel vProfileModel)
        {
            string vMsg = LocalizationBinderContainer.GetString(KeyMessage.ExpiredLicenseMsg); 
            Notify.Template("fade")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true);
            LoginControl.EnableControls();

        }/// <summary>
         /// Handler for deleted license event
         /// </summary>
         /// <param name="vProfileModel"></param>
        public void DeletedLicenseHandler(UserProfileModel vProfileModel)
        {
            string vMsg = LocalizationBinderContainer.GetString(KeyMessage.DeletedLicenseMsg);
            Notify.Template("fade")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true);
            LoginControl.EnableControls();
        }
        
        internal void OnApplicationQuit()
        {
            Appstart.CleanUpOnQuit();
            LoginControl.RemoveOnLoginEvent(Bouncer.ValidateUser);
        }
    }
}