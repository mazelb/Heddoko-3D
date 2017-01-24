/**
* @file PasswordInputField.cs
* @brief Contains the PasswordInputField
* @author Mohammed Haider( mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Licensing.Authentication
{
    /// <summary>
    /// Contains a button and password combination that allows a user view a password in a password type input field.
    /// </summary>
    public class PasswordInputField : MonoBehaviour
    {
        public InputField InputField;
        public ViewPasswordButton ViewPasswordButton;
        private string mInputFieldStore;
        public EventSystem EventSystem;
        void Awake()
        {
            ViewPasswordButton.AddViewPasswordButtonReleasedEventHandler(HidePassword);
            ViewPasswordButton.AddViewPasswordButtonPressedEventHandler(ViewPassword);
            InputField.contentType = InputField.ContentType.Password;
        }

        /// <summary>
        /// Trigger event to view password
        /// </summary>
        void ViewPassword()
        {
            InputField.contentType = InputField.ContentType.Alphanumeric;
            StartCoroutine(DeselectPasswordInput());
        }

        /// <summary>
        /// Workaround to set the input field's content type
        /// </summary>
        /// <returns></returns>
        IEnumerator DeselectPasswordInput()
        {
            EventSystem.SetSelectedGameObject(InputField.gameObject);
            yield return null;
            EventSystem.SetSelectedGameObject(ViewPasswordButton.gameObject);
        }

        /// <summary>
        /// Trigger event to hide password
        /// </summary>
        void HidePassword()
        {
            InputField.contentType = InputField.ContentType.Password;
            StartCoroutine(DeselectPasswordInput());
        }
        /// <summary>
        /// Clears the input fields
        /// </summary>
        public void Clear()
        {
            InputField.text = "";
        }

    }
}