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
using System.Text;
using System.Threading;
using Assets.Scripts.Licensing.Authentication;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.Localization;
using Assets.Scripts.MainApp;
using Assets.Scripts.UI.ModalWindow;
using Assets.Scripts.Utils;
using HeddokoSDK;
using HeddokoSDK.Models;
 using UIWidgets;
using UnityEngine;

 
namespace Assets.Scripts.Licensing.Controller
{
    public delegate void OnLoginSuccess(UserProfileModel vModel);

    /// <summary>
    /// A login control to be used with unity
    /// </summary>
    public class UnityLoginControl : MonoBehaviour
    {
        public LoginModel LoginModel = new LoginModel();
        public LoginView LoginView;
        
        private LoginController mLoginController;
        private HeddokoClient mClient;
        internal OnLoginSuccess LoginSuccessEvent;
        private Thread mConnectionThread;
        private string mUrl;
        private string mSecret;
        private const int RECONNECTION_ATTEMPTS = 10;
        private const int RECONNECTION_DELAY = 2000;
        public LoginController LoginController
        {
            get { return mLoginController; }
        }

        internal void Awake()
        {
            mUrl = GlobalConfig.MainServer;
            mSecret = GlobalConfig.MainServerKey;
#if DEBUG
            mUrl = GlobalConfig.DevServer;
            mSecret = GlobalConfig.DevServerKey;
#endif
            HeddokoConfig vConfig = new HeddokoConfig(mUrl, mSecret, RECONNECTION_ATTEMPTS, RECONNECTION_DELAY);
            mClient = new HeddokoClient(vConfig);
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidationCallback;
            mLoginController = new LoginController();
            mLoginController.Init(LoginModel, LoginView);
            mLoginController.AddErrorHandler(LoginErrorType.CannotAuthenticate, DisplayErrorNotification);
            mLoginController.AddErrorHandler(LoginErrorType.CannotAuthenticate, vX => EnableControls());
            mLoginController.AddErrorHandler(LoginErrorType.NullLicense, DisplayErrorNotification);
            mLoginController.AddErrorHandler(LoginErrorType.NullLicense, vX => EnableControls());
            mLoginController.AddErrorHandler(LoginErrorType.NoNetworkConnectionFound, NoNetworkConnectionErrorHandler);
            mLoginController.AddErrorHandler(LoginErrorType.ZeroLengthPassword, LoginView.DisplayProblemWithPassword);
            mLoginController.AddErrorHandler(LoginErrorType.ZeroLengthUserName, LoginView.DisplayProblemWithUsername);
            mLoginController.AddErrorHandler(LoginErrorType.Other, DisplayErrorNotification);
            mLoginController.AddErrorHandler(LoginErrorType.Other, vX => EnableControls());
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
            if (LoginSuccessEvent != null)
            {
                // ReSharper disable once DelegateSubtraction
                if (vHandler != null)
                {
                    LoginSuccessEvent -= vHandler;
                }
            }
        }

        /// <summary>
        /// Displays an error notification
        /// </summary>
        /// <param name="vMsg"></param>
        void DisplayErrorNotification(string vMsg)
        {
            Notify.Template("fade").Show(vMsg);
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => LoginView.SetLoadingIconAsActive(false));
        }

        void SubmitLogin(LoginModel vModel)
        {
            //verify internet connection first
            LoginView.DisableSubmissionControls();
            StartCoroutine(VerifyInternetConnection(vModel));
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => LoginView.SetLoadingIconAsActive(true));
        }

        /// <summary>
        /// Verify the internet connection first
        /// </summary>
        /// <returns></returns>
        IEnumerator VerifyInternetConnection(LoginModel vModel)
        {
            WWW vWww = new WWW("http://google.com");
            yield return vWww;
            if (vWww.error != null)
            {
                //error: no internet connection
                string vMsg = LocalizationBinderContainer.GetString(KeyMessage.NoInternetConnectionMsg);
                mLoginController.RaiseErrorEvent(LoginErrorType.NoNetworkConnectionFound, vMsg);
            }
            else
            {
                //No error: continue 
                mConnectionThread = new Thread(() => SubmitLoginInfo(vModel));
                mConnectionThread.IsBackground = true;
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
                if (!vUser.IsOk)
                { 
                    OutterThreadToUnityThreadIntermediary.QueueActionInUnity(EnableControls);
                    var vErrorMsg = FormatLoginNoOkError(vUser.Errors);
                    string vMsg = LocalizationBinderContainer.GetString(KeyMessage.LoginFailureMsg);
                    Action vRaiseModalPanelAction = () => ModalPanel.Instance().Choice(vMsg, vErrorMsg, () =>
                    {
                        Application.OpenURL(mUrl);
                    }, () => { });
                    OutterThreadToUnityThreadIntermediary.QueueActionInUnity(vRaiseModalPanelAction);
                    OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                        () => LoginView.SetLoadingIconAsActive(false));
                    return;
                }
                LicenseInfo vLicense = vUser.LicenseInfo;

                if (vUser.IsOk)
                {
                    var vToken = mClient.GenerateDeviceToken();
                     UserProfileModel vProfileModel = new UserProfileModel()
                    {
                        User = vUser,
                        LicenseInfo = vLicense,
                        Client = mClient,
                        DeviceToken = vToken
                    };
                    //if user is an analyst
                    if (vUser.RoleType == UserRoleType.Analyst)
                    {
                        ListCollection<User> vUsers = mClient.UsersCollection(new ListRequest()
                        {
                            Take = 1000,
                            Skip = 0
                        });
                        vProfileModel.UserList = vUsers;
                    }
                    if (LoginSuccessEvent != null)
                    {
                        OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => LoginSuccessEvent(vProfileModel));
                        OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                            () => LoginView.SetLoadingIconAsActive(false));
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
                                                    LocalizationBinderContainer.GetString(KeyMessage.ReportErrorCodeMsg) +
                                                    "404"));
                                        break;
                                    case HttpStatusCode.Unauthorized:
                                        OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                                            () =>
                                                mLoginController.RaiseErrorEvent(LoginErrorType.CannotAuthenticate,
                                                    LocalizationBinderContainer.GetString(KeyMessage.InvalidUnPwMsg)));
                                        vIsHandled = true;
                                        break;
                                    default:
                                        vIsHandled = true;
                                        OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                                            () =>
                                                mLoginController.RaiseErrorEvent(LoginErrorType.Other,
                                                    LocalizationBinderContainer.GetString(KeyMessage.ReportErrorCodeMsg) +
                                                    " " + vWebResponse.StatusCode));
                                        break;
                                }
                            }
                            if (!vIsHandled)
                            {
                                OutterThreadToUnityThreadIntermediary.QueueActionInUnity(
                                    () =>
                                        mLoginController.RaiseErrorEvent(LoginErrorType.Other,
                                            LocalizationBinderContainer.GetString(KeyMessage.ReportErrorCodeMsg) +
                                            vWebException));
                            }
                        }
                        break;
                }
            }
            catch (Exception vE)
            {
                OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
                {
                    LoginView.EnableButtonControls();
                    LoginView.SetLoadingIconAsActive(false);
                    mLoginController.RaiseErrorEvent(LoginErrorType.Other,
                        LocalizationBinderContainer.GetString(KeyMessage.ReportErrorCodeMsg) +
                        " " + vE.Message);
                });
           
            }
        }


        /// <summary>
        /// Since we need to use HTTPS to authenticate, the application needs to add a valid certificate to unity's empty certificates  store.
        /// </summary> 
        /// <returns></returns>
        public static bool RemoteCertificateValidationCallback(System.Object vSender, X509Certificate vCertificate, X509Chain vChain, SslPolicyErrors vSslPolicyErrors)
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
        internal void OnApplicationQuit()
        {
            if (mConnectionThread != null)
            {
                try
                {
                    mConnectionThread.Abort();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private static string FormatLoginNoOkError(ErrorCollection vErrors)
        {
            int vListCount = 1;
            StringBuilder vBuilder = new StringBuilder();
            string vMsg = LocalizationBinderContainer.GetString(KeyMessage.IssueAccessingAcountGenericMsg,
                vErrors.Errors.Count > 1);
            vBuilder.Append(vMsg);
            foreach (var vError in vErrors.Errors)
            {
                vBuilder.Append("\r\n" + vListCount + "." + vError.Message + "");
                vListCount++;
            }
            string vYesMsg = LocalizationBinderContainer.GetString(KeyMessage.AckErrorMsg);
            vBuilder.AppendLine(vYesMsg);
            return vBuilder.ToString();
        }

        /// <summary>
        /// clears the input fields
        /// </summary>
        public void Clear()
        {
            LoginView.Clear();
        }
    }



}
