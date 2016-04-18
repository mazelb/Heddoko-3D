/** 
* @file WarningBoxView.cs
* @brief The view of the warning box view
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/
using Assets.Scripts.Utils.UnityUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /**
   * WarningBoxView class 
   * @brief Represents the warning box view, containing warning text, icon to be shown as a warning to the viewer
   * @note Note something of interest 
   */
    /// <summary>
    /// Represents the view of the warning box
    /// </summary>
    public class WarningBoxView : MonoBehaviour
    {
        public Text WarningText;
        public Image WarningIcon;
        public FadeInFadeOutEffect WarningIconFadeInFadeOutEffect;
        /// <summary>
        /// Enables and displays the warning box view
        /// </summary>
        /**
        * Show()
        * @brief Enables and displays the warning box view
        */

        public void Show()
        {
            WarningIconFadeInFadeOutEffect.StartEffect = true;
            gameObject.SetActive(true);
           
        }
        /**
        * Show()
        * @brief Disables and hides the warning box view
        */
        /// <summary>
        /// Disables and hides the warning box view
        /// </summary>
        public void Hide()
        {
            WarningIconFadeInFadeOutEffect.StartEffect = false;
            gameObject.SetActive(false);
            
        }
    }
}
