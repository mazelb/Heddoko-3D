/* @file RightClickButtonElement.cs
* @brief Contains the RightClickButtonElement class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.ContextMenuSubControl.ConcreteImpletors
{
    /// <summary>
    /// An element used in right click button container
    /// </summary>
    public class RightClickButtonElement: MonoBehaviour
    {
        public Button ControlButton;
        public Text Info;
        public UnityAction CallbackAction;
        public UnityAction OnSelectAction;

        /// <summary>
        /// Is the element active?
        /// </summary>
        public bool IsActive
        {
            get { return gameObject.activeSelf; }
        }

        /// <summary>
        /// Initializes the button
        /// </summary> 
        public void Init(RightClickButtonStructure vStructure, UnityAction vOnSelection)
        {
            OnSelectAction = vOnSelection;
            gameObject.SetActive(true);
            Info.text = vStructure.Info;
            CallbackAction = vStructure.CallbackAction;
            ControlButton.onClick.RemoveAllListeners();
            ControlButton.onClick.AddListener(() =>
            {
                CallbackAction.Invoke();
                if (OnSelectAction != null)
                {
                    OnSelectAction.Invoke();
                }
            });
        }

        /// <summary>
        /// hide the object
        /// </summary>
        public void Hide()
        {
           gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// A structure for right button clicks
    /// </summary>
    public class RightClickButtonStructure
    {
        public string Info { get; set; }
        public UnityAction CallbackAction { get; set; }
    }
}