/** 
* @file ActivitiesContextViewMain.cs
* @brief Contains the ActivitiesContextViewMain class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ActivitiesContext.View
{
    /// <summary>
    /// Represents the main view within the activities context
    /// </summary>
    public class ActivitiesContextViewMain :MonoBehaviour, IActivitiesContextViewSubcomponent
    {
        public Button BackButton;
        public Button ActivityTrainingButton;

        /// <summary>
        /// Enables and displays the main Activities context view
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the main Activities context view
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false); 
        }

        public Button Backbutton { get; set; }
    }
}
