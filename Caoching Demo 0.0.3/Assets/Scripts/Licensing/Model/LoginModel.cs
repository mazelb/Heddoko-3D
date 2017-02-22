/*
 * @file LoginModel.cs
 * @brief Contains the LoginModel class
 * @author Mohammed Haider( mohammed@heddoko.com)
 * @date June 2016
 * Copyright Heddoko(TM) 2016,  all rights reserved
 */

using HeddokoSDK.Models;

namespace Assets.Scripts.Licensing.Authentication
{
    /// <summary>
    /// A login model class
    /// </summary>
    public class LoginModel
    {
        private UserRequest mUserRequest = new UserRequest();
        public string UserName
        {
            get
            {return mUserRequest.Username;
            }
            set { mUserRequest.Username = value; }
        }
        public string Password
        {
            get { return mUserRequest.Password; }
            set { mUserRequest.Password = value; }
        }
        public UserRequest UserRequest { get { return mUserRequest; } }
    }

    public enum LoginErrorType
    {
        ZeroLengthUserName,
        ZeroLengthPassword,
        InvalidUserNameCharacters,
        HttpStatus404,
        CannotAuthenticate,
        NoNetworkConnectionFound,
        Timeout,
        NullLicense,
        Other, 
    }
}