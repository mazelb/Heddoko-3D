
/** 
* @file LoginControl.cs
* @brief Contains the LoginControl class 
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;
using Assets.Scripts.UI.AbstractViews.Enums; 
using UnityEngine.UI; 

namespace Assets.Scripts.UI.AccessControl
{
    public delegate void SubmitLoginInfo(string vUsername, string vPassword);
    /// <summary>
    /// Login control class
    /// </summary>
    public class LoginControl : AbstractSubControl
    {
        public event SubmitLoginInfo SubmitLoginEvent;
        public InputField UsernameInput;
        public InputField PasswordInput;
        public Button SubmitLoginInfoButton;
        public PasswordViewButton ViewPasswordButton;
        private string mPassword ="";
        private string mUsername= "";
        public override SubControlType SubControlType
        {

            get { return SubControlType.LoginControl; }
        }

        /// <summary>
        /// Verifies if the user name and password, then enables or disables the log in button
        /// </summary>
        private void ChangeLoginButtonInteractivity()
        {
            if (mPassword.Length > 0 && mUsername.Length > 0)
            {
                SubmitLoginInfoButton.interactable = true;
            }
            else
            {
                SubmitLoginInfoButton.interactable = false;
            }
        }
        private void ValidateUserNameInput(string vInput )
        { 
            mUsername = vInput;
            ChangeLoginButtonInteractivity();
        }

        private void ValidatePasswordInput(string vInput)
        { 
            mPassword = vInput;
            ChangeLoginButtonInteractivity();
        }

        public override void Disable()
        {

        }

        public override void Enable()
        {

        }

        public void OnEnable()
        {
            SubmitLoginInfoButton.interactable = false;
            ViewPasswordButton.PointerClickEvent += ShowPassword;
            ViewPasswordButton.PointerDeclickEvent += HidePassword;
            UsernameInput.onValueChange.AddListener(ValidateUserNameInput);
            PasswordInput.onValueChange.AddListener(ValidatePasswordInput);
        }

        public void OnDisable()
        {
            ViewPasswordButton.PointerClickEvent -= ShowPassword;
            ViewPasswordButton.PointerDeclickEvent -= HidePassword;
            UsernameInput.onValueChange.RemoveAllListeners();
            PasswordInput.onValueChange.RemoveAllListeners();
        }



        /// <summary>
        /// While the show password button is held down, allow the password to be seen
        /// </summary>
        private void ShowPassword()
        {
            PasswordInput.contentType = InputField.ContentType.Standard;
            string vPassword = PasswordInput.text;
            PasswordInput.text += "s";
            PasswordInput.text = vPassword;

        }
 
        /// <summary>
        /// hides the password from view
        /// </summary>
        private void HidePassword()
        {
            PasswordInput.contentType = InputField.ContentType.Password;
            string vPassword = PasswordInput.text;
            PasswordInput.text += "s";
            PasswordInput.text = vPassword;

        }

        private void Sumbmit()
        {
            if (SubmitLoginEvent != null)
            {
                SubmitLoginEvent(mUsername, mPassword);
            }
        }
     
    }
}