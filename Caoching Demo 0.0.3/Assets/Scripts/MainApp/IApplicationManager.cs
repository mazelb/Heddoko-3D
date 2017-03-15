// /**
// * @file IApplicationManager.cs
// * @brief Contains the IApplicationManager interface
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.Communication;
using Assets.Scripts.Licensing.Model;
using HeddokoSDK;

namespace Assets.Scripts.MainApp
{
    public interface IApplicationManager
    {
        /// <summary>
        /// Initializes the view with regards to user's profile
        /// </summary> 
        void Init(UserProfileModel vProfileModel );

        /// <summary>
        /// The authorization manager used to authorize users from performing certain functions
        /// </summary>
        AuthorizationManager AuthorizationManager { get;   }
    }
}