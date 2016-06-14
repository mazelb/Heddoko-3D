// /**
// * @file ViewPasswordButton.cs
// * @brief Contains the ViewPasswordButton class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date June 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Licensing.Authentication
{
    public delegate void ViewPasswordButtonPressed();

    public delegate void ViewPasswordButtonReleased();
    public class ViewPasswordButton : MonoBehaviour, IPointerUpHandler,IPointerDownHandler
    {
        private ViewPasswordButtonPressed mViewPasswordButtonPressedEvent;
        private ViewPasswordButtonReleased mViewPasswordButtonReleasedEvent;

        public void AddViewPasswordButtonPressedEventHandler(ViewPasswordButtonPressed vEventHandler)
        {
            mViewPasswordButtonPressedEvent += vEventHandler;
        }

        /// <summary>
        /// Adds an event handler to the button released event
        /// </summary>
        /// <param name="vEventHandler"></param>
        public void AddViewPasswordButtonReleasedEventHandler(ViewPasswordButtonReleased vEventHandler)
        {
            mViewPasswordButtonReleasedEvent += vEventHandler;
        }
      
       
      

        public void OnPointerUp(PointerEventData eventData)
        {
            if (mViewPasswordButtonReleasedEvent != null)
            {
                mViewPasswordButtonReleasedEvent.Invoke();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (mViewPasswordButtonPressedEvent != null)
            {
                mViewPasswordButtonPressedEvent.Invoke();
            }
        }
    }
}