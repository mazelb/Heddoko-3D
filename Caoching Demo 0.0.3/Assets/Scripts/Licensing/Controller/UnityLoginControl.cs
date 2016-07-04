// /**
// * @file UnityLoginControl.cs
// * @brief Contains the UnityLoginControl
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.Utils;
using HeddokoSDK;
using HeddokoSDK.Models;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

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
        private HeddokoClient mClient;
        internal OnLoginSuccess LoginSuccessEvent;
        private Thread mConnectionThread;
        public Image LoadingIcon;
        public LoginController LoginController
        {
            get { return mLoginController; }
        }

        void Awake()
        {
            HeddokoConfig vConfig = new HeddokoConfig("https://app.heddoko.com/api/v1", "HEDFstcKsx0NHjPSsjcndjnckSDJjknCCSjcnsJSK89SJDkvVBrk");

#if DEBUG
          //  vConfig = new HeddokoConfig("http://dev.app.heddoko.com/api/v1", "HEDFstcKsx0NHjPSsjfSDJdsDkvdfdkFJPRGldfgdfgvVBrk");
#endif

            mClient = new HeddokoClient(vConfig);
            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
            mLoginController = new LoginController();
            mLoginController.Init(LoginModel, LoginView);
            mLoginController.AddErrorHandler(LoginErrorType.CannotAuthenticate, DisplayErrorNotification);
            mLoginController.AddErrorHandler(LoginErrorType.CannotAuthenticate, (vX) => EnableControls());
            mLoginController.AddErrorHandler(LoginErrorType.NullLicense, DisplayErrorNotification);
            mLoginController.AddErrorHandler(LoginErrorType.NoNetworkConnectionFound, NoNetworkConnectionErrorHandler);
            mLoginController.AddErrorHandler(LoginErrorType.NullLicense, (vX) => EnableControls());
            mLoginController.AddErrorHandler(LoginErrorType.ZeroLengthPassword, LoginView.DisplayProblemWithPassword);
            mLoginController.AddErrorHandler(LoginErrorType.ZeroLengthUserName, LoginView.DisplayProblemWithUsername);
            mLoginController.AddErrorHandler(LoginErrorType.Other, DisplayErrorNotification);
            mLoginController.AddLoginSubmissionHandler(SubmitLogin);
            OutterThreadToUnityThreadIntermediary.Instance.Init();

        }

        /// <summary>
        /// No internet connection error handler. 
        /// </summary>
        /// <param name="vVmsg"></param>
        private void NoNetworkConnectionErrorHandler(string vVmsg)
        {
            DisplayErrorNotification(vVmsg);
            EnableControls();
        }


        /// <summary>
        /// enable login controls
        /// </summary>
        /// <param name="vVmsg"></param>
        public void EnableControls()
        {
            LoginView.EnableButtonControls();
        }

        /// <summary>
        /// Registers handler to user's on login success.
        /// </summary>
        /// <param name="vHandler">The handler to registers</param>
        public void RegisterOnLoginEvent(OnLoginSuccess vHandler)
        {
            LoginSuccessEvent += vHandler;
        }
        /// <summary>
        /// Removes handler to user's on login success.
        /// </summary>
        /// <param name="vHandler">The handler to remove</param>
        public void RemoveOnLoginEvent(OnLoginSuccess vHandler)
        {
            LoginSuccessEvent -= vHandler;
        }


        /// <summary>
        /// Displays an error notification
        /// </summary>
        /// <param name="vMsg"></param>
        void DisplayErrorNotification(string vMsg)
        {
            Notify.Template("fade")
                .Show(vMsg, customHideDelay: 5f, sequenceType: NotifySequence.First);
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => LoadingIcon.gameObject.SetActive(false));

        }

        void SubmitLogin(LoginModel vModel)
        {
            //verify internet connection first
            LoginView.DisableButtonControls();
            StartCoroutine(VerifyInternetConnection(vModel));
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => LoadingIcon.gameObject.SetActive(true));


        }

        /// <summary>
        /// Verify the internet connection first
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IEnumerator VerifyInternetConnection(LoginModel vModel)
        {
            WWW vWww = new WWW("http://google.com");
            yield return vWww;
            if (vWww.error != null)
            {
                //error: no internet connection
                mLoginController.RaiseErrorEvent(LoginErrorType.NoNetworkConnectionFound, "A connection to the internet could not be established. Try again by clicking on the submit button");
            }
            else
            {
                //No error: continue 
                mConnectionThread = new Thread(() => SubmitLoginInfo(vModel));
                mConnectionThread.Start();
            }
        }

        /// <summary>
        /// Submits login information
        /// </summary>
        /// <param name="vModel">The login model to submit</param>
        private void SubmitLoginInfo(LoginModel vModel)
        {
            bool vIsHandled = false;
            try
            {
                UserRequest vRequest = vModel.UserRequest;
                User vUser = mClient.SignIn(vRequest);
                LicenseInfo vLicense = vUser.LicenseInfo;
                mClient.SetToken(vUser.Token);
                ResultBool vCheck = mClient.Check();
                User vProfile = mClient.Profile();
                if (vCheck.Result)
                {
                    UserProfileModel vProfileModel = new UserProfileModel()
                    {
                        User = vProfile,
                        LicenseInfo = vLicense
                    };
                    if (LoginSuccessEvent != null)
                    {
                        OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => LoginSuccessEvent(vProfileModel));
                        OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                            () => LoadingIcon.gameObject.SetActive(false));
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
                                        vIsHandled = true;
                                        OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                                            () =>
                                                mLoginController.RaiseErrorEvent(LoginErrorType.HttpStatus404,
                                                    "Please contact support and inform them of error 404"));
                                        break;
                                    case HttpStatusCode.Unauthorized:
                                        OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                                            () =>
                                                mLoginController.RaiseErrorEvent(LoginErrorType.CannotAuthenticate,
                                                    "Invalid user name and/or password."));
                                        vIsHandled = true;
                                        break;
                                    default:
                                        vIsHandled = true;
                                        OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                                            () =>
                                                mLoginController.RaiseErrorEvent(LoginErrorType.Other,
                                                    "There was a problem trying to reach the server. Please report to support with error code " +
                                                    vWebResponse.StatusCode));
                                        break;
                                }
                            }
                            if (!vIsHandled)
                            {
                                OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                                    () =>
                                        mLoginController.RaiseErrorEvent(LoginErrorType.Other,
                                            "There was a problem trying to reach the server. Please report to support with error code " +
                                            vWebException));
                            }
                        }
                        break;
                }
            }
           
        }


        /// <summary>
        /// Since we need to use HTTPS to authenticate, the application needs to add a valid certificate to unity's empty certificates  store.
        /// </summary> 
        /// <returns></returns>
        public bool RemoteCertificateValidationCallback(System.Object vSender, X509Certificate vCertificate, X509Chain vChain, SslPolicyErrors vSslPolicyErrors)
        {
            bool vIsOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (vSslPolicyErrors != SslPolicyErrors.None)
            {
                for (int vI = 0; vI < vChain.ChainStatus.Length; vI++)
                {
                    if (vChain.ChainStatus[vI].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        vChain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        vChain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        vChain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        vChain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool vChainIsValid = vChain.Build((X509Certificate2)vCertificate);
                        if (!vChainIsValid)
                        {
                            vIsOk = false;
                        }
                    }
                }
            }
            return vIsOk;
        }
        void OnApplicationQuit()
        {
            if (mConnectionThread != null)
            {
                mConnectionThread.Abort();
            }
        }

    }



}