// /**
// * @file UserSessionManager.cs
// * @brief Contains the UserSessionManager
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.Licensing.Model;
using HeddokoSDK;
using UnityEngine;

namespace Assets.Scripts.MainApp
{
    public interface IUserProfileManager
    {
        /// <summary>
        /// The current loaded profile model
        /// </summary>
        UserProfileModel UserProfile { get; set; }
    }

    /// <summary>
    /// A singleton who's responsibility is to manage a user's session
    /// </summary>
    public class UserSessionManager : MonoBehaviour, IUserProfileManager
    {
        private static UserSessionManager sInstance;
        private UserProfileModel mModel;
        private HeddokoClient mHeddokoClient;

        public static  UserSessionManager Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = GameObject.FindObjectOfType<UserSessionManager>();
                    if (sInstance == null)
                    {
                        GameObject vGo = new GameObject("UserSessionManager");
                        sInstance = vGo.AddComponent<UserSessionManager>();
                    }
                }
                return sInstance;
            }
        }

        /// <summary>
        /// The current loaded profile model
        /// </summary>
        public UserProfileModel UserProfile
        {
            get { return mModel; }
            set { mModel = value; }
        }

        /// <summary>
        /// The current connected client
        /// </summary>
        public HeddokoClient HeddokoClient {
            get { return mHeddokoClient; }
            set { mHeddokoClient = value; }
        }
    }
}