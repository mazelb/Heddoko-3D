// /**
// * @file IApplicationStartupManager.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 06 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.Licensing.Model;
using Assets.Scripts.UI;

namespace Assets.Scripts.Licensing.Controller
{
    public interface IApplicationStartupManager
    {
        IViewFactory ViewFactory { get; }
        void Init();

        /// <summary>
        /// Handler for active user event
        /// </summary> 
        void ActiveUserHandler(UserProfileModel vProfileModel);

        /// <summary>
        /// Handler for banned user
        /// </summary> 
        void BannedUserHandler(UserProfileModel vProfileModel);

        /// <summary>
        /// Handler for inactive user event
        /// </summary> 
        void InactiveUserHandler(UserProfileModel vProfileModel);

        /// <summary>
        /// Handler for active license event
        /// </summary> 
        void ActiveLicenseHandler(UserProfileModel vProfileModel);

        /// <summary>
        ///Handler for inactive license event
        /// </summary> 
        void InActiveLicenseHandler(UserProfileModel vProfileModel);

        /// <summary>
        /// Handler for expired license event
        /// </summary> 
        void ExpiredLicenseHandler(UserProfileModel vProfileModel);

        /// <summary>
        /// Handler for deleted license event
        /// </summary>
        /// <param name="vProfileModel"></param>
        void DeletedLicenseHandler(UserProfileModel vProfileModel);
    }

}