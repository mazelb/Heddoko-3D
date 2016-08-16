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
         public User User { get; set; }
        public LicenseInfo LicenseInfo { get; set; }

        public HeddokoClient Client { get; set; }
    }
}