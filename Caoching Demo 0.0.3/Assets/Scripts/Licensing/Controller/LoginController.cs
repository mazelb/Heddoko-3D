// /**
// * @file LoginController.cs
// * @brief Contains the LoginController class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using Assets.Scripts.Licensing.Authentication;

// ReSharper disable DelegateSubtraction
namespace Assets.Scripts.Licensing.Controller
{
    public delegate void LoginError(string vMsg);

    public delegate void LoginSubmission(LoginModel vModel);

    /// <summary>
    /// A login controller. 
    /// </summary>
    public class LoginController
    {
        public LoginModel LoginModel { get; set; }
        public LoginView LoginView { get; set; }
        private Dictionary<LoginErrorType, LoginError> mLoginError = new Dictionary<LoginErrorType, LoginError>();
        internal LoginSubmission LoginSubmissionEvent;

        public void Init(LoginModel vModel, LoginView vView)
        {
            LoginModel = vModel;
            LoginView = vView;
            LoginView.PasswordSubmissionEvent += Login;
            LoginError vGeneric = vX => { };
            var vEnumIterable = Enum.GetValues(typeof (LoginErrorType));
            foreach (var vErrorType in vEnumIterable)
            {
                mLoginError.Add((LoginErrorType) vErrorType, vGeneric);
            }
        }

        /// <summary>
        /// Adds a password summission handler
        /// </summary>
        /// <param name="vHandler">Handler</param>
        public void AddLoginSubmissionHandler(LoginSubmission vHandler)
        {
            LoginSubmissionEvent += vHandler;
        }

        /// <summary>
        /// Removes a password submission handler
        /// </summary>
        /// <param name="vHandler">Handler</param>
        public void RemoveLoginSubmissionHandler(LoginSubmission vHandler)
        {
            if (LoginSubmissionEvent != null)
            {
                LoginSubmissionEvent -= vHandler;
            }
        }

        /// <summary>
        /// Adds an error handler with the specified type
        /// </summary>
        /// <param name="vType">The error type</param>
        /// <param name="vHandler">The event handler</param>
        public void AddErrorHandler(LoginErrorType vType, LoginError vHandler)
        {
            mLoginError[vType] += vHandler;
        }


        public void RemoveErrorHandler(LoginErrorType vType, LoginError vHandler)
        {
            mLoginError[vType] -= vHandler;
        }
        /// <summary>
        /// Login with the provided credentials
        /// </summary>
        /// <param name="vUserName"></param>
        /// <param name="vPassword"></param>
        public void Login(string vUserName, string vPassword)
        {
            LoginModel.UserName = vUserName;
            LoginModel.Password = vPassword;
            if (ValidateUserName() && ValidatePassword())
            {
                //proceed to login. 
                LoginSubmissionEvent.Invoke(LoginModel);
            }

        }

        public void RaiseErrorEvent(LoginErrorType vType, string vMsg)
        {
            mLoginError[vType].Invoke(vMsg);
        }

        
        /// <summary>
        /// Validates user name
        /// </summary>
        private bool ValidateUserName()
        {
            if (string.IsNullOrEmpty(LoginModel.UserName))
            {
                RaiseErrorEvent(LoginErrorType.ZeroLengthUserName, "The user name must be entered before submitting."); 
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the password name
        /// </summary>
        private bool ValidatePassword()
        {
            if (string.IsNullOrEmpty(LoginModel.Password))
            {
                RaiseErrorEvent(LoginErrorType.ZeroLengthPassword, "The password must be entered before submitting."); 
                return false;
            }
            return true;
        }

    }
}