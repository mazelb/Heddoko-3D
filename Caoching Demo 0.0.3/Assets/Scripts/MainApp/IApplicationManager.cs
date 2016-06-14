// /**
// * @file IApplicationManager.cs
// * @brief Contains the IApplicationManager interface
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.Licensing.Model;

namespace Assets.Scripts.MainApp
{
    public interface IApplicationManager
    {
        /// <summary>
        /// Initializes the view with regards to user's profile
        /// </summary>
        /// <param name="vProfileModel"></param>
        void Init(UserProfileModel vProfileModel);
    }
}