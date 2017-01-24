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
        private string mDeviceToken = "";

        public User User { get; set; }
        public LicenseInfo LicenseInfo { get; set; }

        public HeddokoClient Client { get; set; }
        public ListCollection<User> UserList { get; set; }

        public string DeviceToken { get; set; }

        /// <summary>
        /// Returns a kit id. If the user isn't instantiated, then -1 is returned
        /// </summary>
        public int GetKitIdFromBrainpackLabel(string vLabel)
        {
            int vKitId = -1;
            if (User != null)
            {
                if (User.RoleType == UserRoleType.Analyst)
                {
                    //check if the analyst is uploading one of his brainpacks.
                    if (User.Kit != null)
                    {
                        if (User.Kit.Brainpack != null)
                        {
                            if (User.Kit.Brainpack.Label.Equals(vLabel))
                            {
                                return User.Kit.Brainpack.ID;
                            }
                        }
                    }
                    for (int vI = 0; vI < UserList.TotalCount; vI++)
                    {
                        var vUser = UserList.Collection[vI];
                        if (vUser.Kit != null)
                        {
                            if (vUser.Kit.Brainpack != null)
                            {
                                if (vUser.Kit.Brainpack.Label.Equals(vLabel))
                                {
                                    return vUser.Kit.ID;
                                }
                            }
                        }
                    }
                }
                else if (User.RoleType == UserRoleType.Worker)
                {
                    if (User.Kit != null)
                    {
                        if (User.Kit.Brainpack != null)
                        {
                            if (User.Kit.Brainpack.Label.Equals(vLabel))
                            {
                                return User.Kit.ID;
                            }
                        }
                    }
                }
            }
            return vKitId;
        }
    }
}