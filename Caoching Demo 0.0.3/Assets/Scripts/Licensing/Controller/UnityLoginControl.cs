// /**
// * @file UnityLoginControl.cs
// * @brief Contains the UnityLoginControl
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Licensing.Model;
 using HeddokoSDK;
using HeddokoSDK.Models;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.Licensing.Authentication
{
    public delegate void OnLoginSuccess(UserProfileModel vModel);

     /// <summary>
    /// A login control to be used with unity
    /// </summary>
    public class UnityLoginControl : MonoBehaviour
    {
        public LoginModel LoginModel = new LoginModel();
        public LoginView LoginView; 
        public Notify NotificationTemplate;
        private LoginController mLoginController;
        private HeddokoClient mClient = new HeddokoClient();
        internal OnLoginSuccess mLoginSuccessEvent;

         public LoginController LoginController
         {
             get { return mLoginController; }
         }

         void Awake()
        {
            mLoginController = new LoginController();
            mLoginController.Init(LoginModel, LoginView);
            mLoginController.AddErrorHandler(LoginErrorType.CannotAuthenticate, DisplayErrorNotification);
            mLoginController.AddErrorHandler(LoginErrorType.CannotAuthenticate, (x)=>EnableControls());
            mLoginController.AddErrorHandler(LoginErrorType.NullLicense, DisplayErrorNotification);
            mLoginController.AddErrorHandler(LoginErrorType.NullLicense, (x) => EnableControls());
            mLoginController.AddErrorHandler(LoginErrorType.ZeroLengthPassword, LoginView.DisplayProblemWithPassword);
            mLoginController.AddErrorHandler(LoginErrorType.ZeroLengthUserName, LoginView.DisplayProblemWithUsername);
            mLoginController.AddLoginSubmissionHandler(SubmitLogin);
        }



        /// <summary>
        /// enable login controls
        /// </summary>
        /// <param name="vVmsg"></param>
        private void EnableControls( )
        {
            LoginView.EnableButtonControls();
        }

        /// <summary>
        /// Registers handler to user's on login success.
        /// </summary>
        /// <param name="vHandler">The handler to registers</param>
        public void RegisterOnLoginEvent(OnLoginSuccess vHandler)
        {
            mLoginSuccessEvent += vHandler;
        }
        /// <summary>
        /// Removes handler to user's on login success.
        /// </summary>
        /// <param name="vHandler">The handler to remove</param>
        public void RemoveOnLoginEvent(OnLoginSuccess vHandler)
        {
            mLoginSuccessEvent -= vHandler;
        }
       

        /// <summary>
        /// Displays an error notification
        /// </summary>
        /// <param name="vMsg"></param>
        void DisplayErrorNotification(string vMsg)
        {
            Notify.Template("FadingFadoutNotifyTemplate")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First); 
        }

        void SubmitLogin(LoginModel vModel)
        {
            //verify internet connection first
            LoginView.DisableButtonControls();
            StartCoroutine(VerifyInternetConnection(vModel));

        }

        /// <summary>
        /// Verify the internet connection first
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IEnumerator VerifyInternetConnection(LoginModel vModel)
        {
            WWW www = new WWW("http://google.com");
            yield return www;
            if (www.error != null)
            {
                //error: no internet connection
                mLoginController.RaiseErrorEvent(LoginErrorType.NoNetworkConnectionFound, "A connection to the internet could not be established. Try again by clicking on the submit button");

            }
            else
            {
                //No error: continue
                SubmitLoginInfo(vModel);
            }
        }
        
        private void SubmitLoginInfo(LoginModel vModel)
        {
            try
            {
                UserRequest vRequest = vModel.UserRequest;
                User vUser = mClient.SignIn(vRequest);
                LicenseInfo vLicense = vUser.LicenseInfo;
                mClient.SetToken(vUser.Token);
                ResultBool check = mClient.Check();
                User profile = mClient.Profile();
                if (check.Result )
                {
                    UserProfileModel vProfileModel = new UserProfileModel()
                    {
                        User = profile,
                        LicenseInfo = vLicense
                    };
                    if (mLoginSuccessEvent != null)
                    {
                        mLoginSuccessEvent(vProfileModel);
                    }
                }
                 
            }
            catch (WebException vWebException)
            {
                switch (vWebException.Status)
                {
                    case WebExceptionStatus.ProtocolError:
                        if (vWebException.Response != null)
                        {
                            var vWebResponse = vWebException.Response as HttpWebResponse;
                            if (vWebResponse != null)
                            {
                                switch (vWebResponse.StatusCode)
                                {
                                    case HttpStatusCode.NotFound:
                                        mLoginController.RaiseErrorEvent(LoginErrorType.HttpStatus404, "Please contact support and inform them of error 404");
                                        break;
                                    case HttpStatusCode.Unauthorized:
                                        mLoginController.RaiseErrorEvent(LoginErrorType.CannotAuthenticate, "Invalid user name and/or password.");
                                        break;
                                    default:
                                        mLoginController.RaiseErrorEvent(LoginErrorType.Other, "There was a problem trying to reach the server. Please report to support with error code " + vWebResponse.StatusCode);
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}