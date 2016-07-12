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
        public Image LoadingImage;
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
            LoadingImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// Disable button controls
        /// </summary>
        public void DisableButtonControls()
        {
            SubmitInfoButton.interactable = false;
        }
<<<<<<< HEAD


        void Update()
        {
            //Allow for tab controls
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (EventSystem.currentSelectedGameObject == PasswordInputField.gameObject ||
                    EventSystem.currentSelectedGameObject == UsernameInputField.gameObject)
                {
                    if (EventSystem.currentSelectedGameObject == PasswordInputField.gameObject)
                    {
                        EventSystem.SetSelectedGameObject(UsernameInputField.gameObject);
                    }
                    else
                    {
                        EventSystem.SetSelectedGameObject(PasswordInputField.gameObject);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (EventSystem.currentSelectedGameObject == PasswordInputField.gameObject ||
                    EventSystem.currentSelectedGameObject == UsernameInputField.gameObject)
                {
                    SubmitUserNamePassword();
                }
            }
        }

        /// <summary>
        /// Enables or disables the loading icon
        /// </summary>
        /// <param name="vStatus">true  = active, false = inactive</param>
        public void SetLoadingIconAsActive(bool vStatus)
        {
            LoadingImage.gameObject.SetActive(false);
        }
=======
>>>>>>> 096bb2ae014b51e65bce63c5e77e735a22c23b39
    }
}