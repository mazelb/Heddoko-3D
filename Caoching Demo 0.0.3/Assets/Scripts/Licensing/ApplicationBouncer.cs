/**
* @file ApplicationBouncer.cs
* @brief Contains the ApplicationBouncer class
* @author Mohammed Haider( mohammed @heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Licensing.Model;
using HeddokoSDK.Models;
// ReSharper disable DelegateSubtraction

namespace Assets.Scripts.Licensing
{
    public delegate void UserAccessAction(UserProfileModel vModel);

    public delegate void LicenseAccessAction(UserProfileModel vModel);

    /// <summary>
    /// The ApplicationBouncer behaves as a bouncer to the main application. Depending on the license status and user's access status, a set of event(s) is(are) triggered. 
    /// </summary>
    public class ApplicationBouncer
    {
        /// <summary>
        /// A set of associated access actions to  a user's status
        /// </summary>
        private readonly Dictionary<UserStatusType, UserAccessAction> mUserCallbackAction = new Dictionary<UserStatusType, UserAccessAction>();

        /// <summary>
        /// A set of associated access action to a user's license information
        /// </summary>
        private readonly Dictionary<LicenseStatusType, LicenseAccessAction> mLicenceCallbackAction = new Dictionary<LicenseStatusType, LicenseAccessAction>();



        public ApplicationBouncer()
        {
            var vUserStatusTypes = Enum.GetValues(typeof(UserStatusType)) as UserStatusType[];
            var vLicenseStatusTypes = Enum.GetValues(typeof(LicenseStatusType)) as LicenseStatusType[];
            if (vUserStatusTypes != null)
            {
                foreach (var vUserStatusType in vUserStatusTypes)
                {
                    mUserCallbackAction.Add(vUserStatusType, vX => { });
                }
            }
            if (vLicenseStatusTypes != null)
            {
                foreach (var vLicenseStatusType in vLicenseStatusTypes)
                {
                    mLicenceCallbackAction.Add(vLicenseStatusType, vX => { });
                }
            }
        }


        /// <summary>
        /// Registers a user access action event. 
        /// </summary>
        /// <param name="vStatusType">the user's status</param>
        /// <param name="vHandler">the callback handle</param>
        public void RegisterUserAccessActionEvent(UserStatusType vStatusType, UserAccessAction vHandler)
        {
            mUserCallbackAction[vStatusType] += vHandler;
        }


        /// <summary>
        /// Remove a user access action event. 
        /// </summary>
        /// <param name="vStatusType">the user's status</param>
        /// <param name="vHandler">the callback handle</param>
        public void RemoveUserAccessActionEvent(UserStatusType vStatusType, UserAccessAction vHandler)
        {
            if (mUserCallbackAction.ContainsKey(vStatusType))
            {
                mUserCallbackAction[vStatusType] -= vHandler;
            }
        }

        /// <summary>
        /// Registers a license access action event. 
        /// </summary>
        /// <param name="vStatusType">the license's status</param>
        /// <param name="vHandler">the callback handle</param>
        public void RegisterLicenseAccessActionEvent(LicenseStatusType vStatusType, LicenseAccessAction vHandler)
        {
            mLicenceCallbackAction[vStatusType] += vHandler;
        }


        /// <summary>
        /// Remove a license access action event. 
        /// </summary>
        /// <param name="vStatusType">the license's status</param>
        /// <param name="vHandler">the callback handle</param>
        public void RemoveLicenseAccessActionEvent(LicenseStatusType vStatusType, LicenseAccessAction vHandler)
        {
            mLicenceCallbackAction[vStatusType] -= vHandler;
        }
        /// <summary>
        /// Validates a user object to allow access to the application
        /// </summary>
        /// <param name="vUser">The user to validate</param>
        public void ValidateUser(UserProfileModel vUser)
        {
            mUserCallbackAction[vUser.User.Status].Invoke(vUser);
        }

        /// <summary>
        /// Validates a users license to allow access to the application
        /// </summary>
        /// <param name="vUser">The user to validate</param>
        public void ValidateLicense(UserProfileModel vUser)
        {
            bool vIsActive = vUser.LicenseInfo.IsActive;
            if (vIsActive)
            {
                mLicenceCallbackAction[LicenseStatusType.Active].Invoke(vUser);
            }
            else
            {
                mLicenceCallbackAction[vUser.LicenseInfo.Status].Invoke(vUser);
            }
        }
    }
}