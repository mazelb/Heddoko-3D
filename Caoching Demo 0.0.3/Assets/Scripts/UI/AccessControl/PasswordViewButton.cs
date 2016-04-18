
/** 
* @file PasswordViewButton.cs
* @brief Contains the PasswordViewButton class 
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AccessControl
{
    public delegate void OnPointerDeclickEventDel();

    public delegate void OnPointerClickEventDel();
    /// <summary>
    /// Allows the user to view a password by holding down on a button
    /// </summary>
    public class PasswordViewButton: MonoBehaviour,IPointerDownHandler, IPointerUpHandler
    {
        public Button ViewPasswordButton;
        public event OnPointerClickEventDel PointerClickEvent;
        public event OnPointerDeclickEventDel PointerDeclickEvent;

      

        public void OnPointerUp(PointerEventData eventData)
        {
            
            if (PointerDeclickEvent != null)
            {
                PointerDeclickEvent();
            }

        }
 

        public void OnPointerDown(PointerEventData eventData)
        {
           
            if (PointerClickEvent != null)
            {
                PointerClickEvent();
            }
        }
    }
}