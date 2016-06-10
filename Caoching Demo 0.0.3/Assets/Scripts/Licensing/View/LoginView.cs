/**
* @file LoginView.cs
* @brief Contains the LoginView
* @author Mohammed Haider( mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Licensing.Authentication
{
    public delegate void SubmitLogin(string vUserName, string vPassword);
    /// <summary>
    /// A login view
    /// </summary>
    public class LoginView : MonoBehaviour
    {
        public InputField UsernameInputField;
        public Text UserNameErrorLabel;
        private IEnumerator mPassErrorAnim;
        public Text PasswordErrorLabel;
        private IEnumerator mUserErrorAnim;
        public PasswordInputField PasswordInputField;
        public event SubmitLogin PasswordSubmissionEvent;
        public Button SubmitInfoButton;
        void Awake()
        {
            SubmitInfoButton.onClick.AddListener(SubmitUserNamePassword);
        }

        /// <summary>
        /// Submits the user name and password
        /// </summary>
        void SubmitUserNamePassword()
        {
            var vPassword = PasswordInputField.InputField.text;
            var vUsername = UsernameInputField.text;
            if (PasswordSubmissionEvent != null)
            {
                PasswordSubmissionEvent(vUsername, vPassword);
            }
        }

        /// <summary>
        /// Adds a handler to the password submission
        /// </summary>
        /// <param name="vHandler"></param>
        public void AddPasswordSubmissionHandler(SubmitLogin vHandler)
        {
            PasswordSubmissionEvent += vHandler;
        }

        /// <summary>
        /// Removes a password handler 
        /// </summary>
        /// <param name="vHandler"></param>
        public void RemovePasswordSubmissionHandler(SubmitLogin vHandler)
        {
            PasswordSubmissionEvent -= vHandler;

        }

        /// <summary>
        /// Displays an error with regards to the user name
        /// </summary>
        /// <param name="vMsg">the message to display</param>
        public void DisplayProblemWithUsername(string vMsg)
        {
            if (mUserErrorAnim != null)
            {
                StopCoroutine(mUserErrorAnim);
            }
            mUserErrorAnim = AnimationHelpers.FadeTextBoxWithMessage(vMsg, UserNameErrorLabel, 5f);
            StartCoroutine(mUserErrorAnim);
         }
        /// <summary>
        /// Displays an error with regards to the password
        /// </summary>
        /// <param name="vMsg">the message to display</param>
        public void DisplayProblemWithPassword(string vMsg)
        {
            if (mPassErrorAnim != null)
            {
                StopCoroutine(mPassErrorAnim);
            }
            mPassErrorAnim = AnimationHelpers.FadeTextBoxWithMessage(vMsg, PasswordErrorLabel, 5f);
            StartCoroutine(mPassErrorAnim);

        }

        /// <summary>
        /// Enables button controls
        /// </summary>
        public void EnableButtonControls()
        {
            SubmitInfoButton.interactable = true;
        }

        /// <summary>
        /// Disable button controls
        /// </summary>
        public void DisableButtonControls()
        {
            SubmitInfoButton.interactable = false;
        }
    }
}