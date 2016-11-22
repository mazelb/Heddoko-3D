// /**
// * @file UserProfileModel.cs
// * @brief Contains the UserProfileModel
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using HeddokoSDK;
using HeddokoSDK.Models;

namespace Assets.Scripts.Licensing.Model
{
    /// <summary>
    /// A user's profile model
    /// </summary>
    public class UserProfileModel
    {
        private string mDeviceToken="";

        public User User { get; set; }
        public LicenseInfo LicenseInfo { get; set; }

        public HeddokoClient Client { get; set; }
        public ListCollection<User> UserList { get; set; }

        public string DeviceToken { get; set; }

        //todo: add a the current kit to be used(for uploading)
    }
}