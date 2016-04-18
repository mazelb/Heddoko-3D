
/** 
* @file ImportRecordingsButton.cs
* @brief Contains the ImportRecordingsButton class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainScene
{
    /// <summary>
    /// Represents the import button view in the main menu. Hooks events into recording select button's listeners
    /// </summary>
    public class ImportRecordingsButton : MonoBehaviour
    {  
        private Button mCurrentButton;
        internal void HideButton()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// Shows the import button
        /// </summary>
        internal void ShowButton()
        {
            gameObject.SetActive(true);
        }
        /// <summary>
        /// assigns an action to the button
        /// </summary>
        /// <param name="vEvent"></param>
        internal void AssignAction(UnityAction vEvent)
        {
            CurrentButton.onClick.AddListener(vEvent);
        }
        /// <summary>
        /// The current button
        /// </summary>
        internal Button CurrentButton
        {
            get
            {
                if (mCurrentButton == null)
                {
                    mCurrentButton = GetComponentInChildren<Button>();
                }
                return mCurrentButton;
            }
        }

      

        
 

 
 

    }
}
