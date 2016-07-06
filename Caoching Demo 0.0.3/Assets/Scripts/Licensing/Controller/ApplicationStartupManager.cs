/**
* @file ApplicationStartupManager.cs
* @brief Contains the ApplicationStartupManager
* @author Mohammed Haider(mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using Assets.Scripts.Licensing.Authentication;
using Assets.Scripts.Licensing.Model; 
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

        /// <summary>
        /// Initializes parameters
        /// </summary>
       internal void Awake()
        {
            Init();
        }

        public IViewFactory ViewFactory { get; private set; }

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
            ViewFactory = gameObject.AddComponent<UnityViewFactory>();
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
            string vMsg =
                 "This account has been banned. ";
            Notify.Template("FadingFadoutNotifyTemplate")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true);
        }
        /// <summary>
        /// Handler for inactive user event
        /// </summary> 
        public void InactiveUserHandler(UserProfileModel vProfileModel)
        {
            string vMsg =
                "This account is not active. Please contact your license administrator for further support.";
            Notify.Template("FadingFadoutNotifyTemplate")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true);
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
            string vMsg =
                "The provided license is inactive. Please contact your license administator for further support.";
            Debug.Log("Inactive license");
            Notify.Template("FadingFadoutNotifyTemplate")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true);
        }

      

        /// <summary>
        /// Handler for expired license event
        /// </summary> 
        public void ExpiredLicenseHandler(UserProfileModel vProfileModel)
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
        public void DeletedLicenseHandler(UserProfileModel vProfileModel)
        {
            string vMsg =
                 "We could not find a license associated with your account. Please contact your license administator for further support.";
            Notify.Template("FadingFadoutNotifyTemplate")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First, clearSequence: true);
            Debug.Log("deleted license");
        }
    }
}