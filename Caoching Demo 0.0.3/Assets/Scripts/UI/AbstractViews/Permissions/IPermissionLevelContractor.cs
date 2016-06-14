/**
* @file IPermissionLevelContractor.cs
* @brief Contains the IPermissionLevelContractor interface
* @author Mohammed Haider(mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using HeddokoSDK.Models;

namespace Assets.Scripts.UI.AbstractViews.Permissions
{
    /// <summary>
    /// A permission level contractor
    /// </summary>
    public interface IPermissionLevelContractor
    {
        void SetInteractionLevel(UserRoleType vRoleType);
    }
}