/**
* @file LicenseWhiteBoxTesting.cs
* @brief Contains the 
* @author Mohammed Haider( mohammed @ heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using Assets.Scripts.Licensing.Controller;
using Assets.Scripts.Licensing.Model;
using HeddokoSDK.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Licensing.Tests
{
    /// <summary>
    /// A white box test of license
    /// </summary>
    public class LicenseWhiteBoxTesting : MonoBehaviour
    {
        public ApplicationStartupManager ApplicationStartupManager;
        UserProfileModel mUserProfileModel = new UserProfileModel();
        public Button LicenceButton;
        public Button UserButton;
        public Button ComboButton;
        public Dropdown LicenseTypeDropDown;
        public Dropdown UserTypeDropDown;
        private LicenseStatusType mLicenseStatusType;
        private UserStatusType mUserStatusType;
        private LicenseStatusType[] mLicenseStatuses;
        private UserStatusType[] mUserStatuses;
        internal void Awake()
        {
            mUserProfileModel.User = new User();
            mUserProfileModel.LicenseInfo = new LicenseInfo();
            LicenseTypeDropDown.onValueChanged.AddListener(OnLicenseDropDown);
            UserTypeDropDown.onValueChanged.AddListener(OnUserDropDown);
            mLicenseStatuses = Enum.GetValues(typeof(LicenseStatusType)) as LicenseStatusType[];
            mUserStatuses = Enum.GetValues(typeof(UserStatusType)) as UserStatusType[];

            LicenseTypeDropDown.options.Clear();
            if (mLicenseStatuses != null)
            {
                foreach (var vArrayType in mLicenseStatuses)
                {
                    LicenseTypeDropDown.options.Add(new Dropdown.OptionData(vArrayType.GetName()));
                }
            }
            LicenseTypeDropDown.RefreshShownValue();

            UserTypeDropDown.options.Clear();
            if (mUserStatuses != null)
            {
                foreach (var vArrayType in mUserStatuses)
                {
                    UserTypeDropDown.options.Add(new Dropdown.OptionData(vArrayType.GetName()));
                }
            }
            UserTypeDropDown.RefreshShownValue();

            LicenceButton.onClick.AddListener(() => ApplicationStartupManager.Bouncer.ValidateLicense(mUserProfileModel));
            UserButton.onClick.AddListener(() => ApplicationStartupManager.Bouncer.ValidateUser(mUserProfileModel));

            ComboButton.onClick.AddListener(() =>
            {
                ApplicationStartupManager.LoginControl.LoginSuccessEvent(mUserProfileModel);
                //ApplicationStartupManager.Bouncer.ValidateLicense(mUserProfileModel);
                // ApplicationStartupManager.Bouncer.ValidateUser(mUserProfileModel);
            });

        }



        void OnLicenseDropDown(int vValue)
        {
            mLicenseStatusType = mLicenseStatuses[vValue];
            mUserProfileModel.LicenseInfo.Status = mLicenseStatusType;
        }

        void OnUserDropDown(int vValue)
        {
            mUserStatusType = mUserStatuses[vValue];
            mUserProfileModel.User.Status = mUserStatusType;
        }

    }
}