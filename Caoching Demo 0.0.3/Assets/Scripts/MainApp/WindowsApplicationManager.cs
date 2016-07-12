// /**
// * @file WindowsApplicationManager.cs
// * @brief Contains the WindowsApplicationManager
// * @author Mohammed Haider( mohammed @ heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.UI.AbstractViews.Permissions;
using Assets.Scripts.UI.RecordingLoading;
using Assets.Scripts.UI.Settings;
using HeddokoSDK.Models;
using UnityEngine;

namespace Assets.Scripts.MainApp
{
    /// <summary>
    /// The application manager in a windows environment
    /// </summary>
    public class WindowsApplicationManager: MonoBehaviour,IApplicationManager
    {

        public RecordingPlayerView RecordingPlayer ;
        public ControlPanel ControlPanel;
        private UserProfileModel mCurrentProfileModel;
        /// <summary>
        /// Initializes the application with the user profile model passed in. 
        /// </summary>
        /// <param name="vProfileModel"></param>

        public void Init(UserProfileModel vProfileModel)
        {
            mCurrentProfileModel = vProfileModel;
            var vUserRole = vProfileModel.User.RoleType;
            var vAttribute =
                (UserRolePermission) Attribute.GetCustomAttribute(typeof (RecordingPlayerView), typeof (UserRolePermission));
            List<UserRoleType> vType = vAttribute.AllowedRoles.ToList();
            if (!vType.Contains(vUserRole))
            {
               // ControlPanel.LoadRecordingsButton.gameObject.SetActive(false);

            }
            RecordingPlayer.RecordingPlayerViewLayoutCreatedEvent += SetPermissions;
        }

        /// <summary>
        /// Sets the recording player sub control panels.
        /// </summary>
        /// <param name="vView"></param>
        private void SetPermissions(RecordingPlayerView vView)
        {
            vView.SetPermissions(mCurrentProfileModel);
        }
    }
}