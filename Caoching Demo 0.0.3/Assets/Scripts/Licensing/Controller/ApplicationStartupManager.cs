/**
* @file ApplicationStartupManager.cs
* @brief Contains the 
* @author Mohammed Haider(mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using Assets.Scripts.Licensing.Authentication;
using Assets.Scripts.Licensing.Model;
using HeddokoSDK.Models;
using UIWidgets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Licensing.Controller
{
    /// <summary>
    /// The startup manager of the application 
    /// </summary>
    public class ApplicationStartupManager : MonoBehaviour
    {
        public UnityLoginControl LoginControl;
        public ApplicationBouncer Bouncer = new ApplicationBouncer();


        void Awake()
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
        void ActiveUserHandler(UserProfileModel vProfileModel)
        {
            Debug.Log("Active user");
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
        void BannedUserHandler(UserProfileModel vProfileModel)
        {

            string vMsg =
                 "This account has been banned. ";
            Notify.Template("FadingFadoutNotifyTemplate")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true);
        }
        /// <summary>
        /// Handler for inactive user event
        /// </summary> 
        void InactiveUserHandler(UserProfileModel vProfileModel)
        {
            string vMsg =
                "This account is not active. Please contact your license administrator for further support.";
            Notify.Template("FadingFadoutNotifyTemplate")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true);
        }

        /// <summary>
        /// Handler for active license event
        /// </summary> 
        void ActiveLicenseHandler(UserProfileModel vProfileModel)
        {
            Debug.Log("Active license, proceed to scene loading");

        }/// <summary>
         ///Handler for inactive license event
         /// </summary> 
        void InActiveLicenseHandler(UserProfileModel vProfileModel)
        {
            string vMsg =
                "The provided license is inactive. Please contact your license administator for further support.";
            Debug.Log("Inactive license");
            Notify.Template("FadingFadoutNotifyTemplate")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true);
        }

      

        /// <summary>
        /// Handler for expired license event
        /// </summary> 
        void ExpiredLicenseHandler(UserProfileModel vProfileModel)
        {
            string vMsg =
                  "The provided license has expired. Please contact your license administator for further support.";
            Notify.Template("FadingFadoutNotifyTemplate")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true);
            Debug.Log("expired license");

        }/// <summary>
         /// Handler for deleted license event
         /// </summary>
         /// <param name="vProfileModel"></param>
        void DeletedLicenseHandler(UserProfileModel vProfileModel)
        {
            string vMsg =
                 "We could not find a license associated with your account. Please contact your license administator for further support.";
            Notify.Template("FadingFadoutNotifyTemplate")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true);
            Debug.Log("deleted license");
        }
    }
}