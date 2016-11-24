// /**
// * @file AuthorizationManager.cs
// * @brief Contains the AuthorizationManager class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using Assets.Scripts.Licensing.Model;
using HeddokoLib.HeddokoDataStructs.Brainpack;
using HeddokoSDK.Models;

namespace Assets.Scripts.Communication
{
    public class AuthorizationManager
    {
        private UserProfileModel mModel;

        public bool BrainpackIsAuthorized(BrainpackNetworkingModel vBrainpack)
        {
#if DEBUG
            return  true;
#endif
            bool vIsAuthorized = false;
            if (mModel != null)
            {
                if (mModel.User != null && mModel.User.Kit != null)
                {
                    if (mModel.User.RoleType == UserRoleType.Analyst)
                    {
                        if (mModel.UserList.Collection != null)
                        {
                            foreach (var vUserItem in mModel.UserList.Collection)
                            {
                                if (BrainpackIsAuthorized(vBrainpack, vUserItem))
                                {
                                    vIsAuthorized = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        BrainpackIsAuthorized(vBrainpack, mModel.User);
                    }
                }
            }
            return vIsAuthorized;
        }

        /// <summary>
        /// Is the Brainpack networking model authorized for the passed in user?
        /// </summary>
        /// <param name="vBrainpack"></param>
        /// <param name="vUser"></param>
        /// <returns></returns>
        private bool BrainpackIsAuthorized(BrainpackNetworkingModel vBrainpack, User vUser)
        {
            bool vIsAuthorized = false;
            if (vUser.Kit != null && vUser.Kit.Brainpack != null)
            {
                if (vUser.Kit.Brainpack.Label.Equals(vBrainpack.Id, StringComparison.InvariantCultureIgnoreCase))
                {
                    vIsAuthorized = true;
                }
            }
            return vIsAuthorized;
        }
    }
}