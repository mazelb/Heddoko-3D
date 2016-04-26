/** 
* @file ActivitesContextViewLearn.cs
* @brief Contains the ActivitesContextViewLearn class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ActivitiesContext.View
{
    /// <summary>
    /// The view associated with the learning  view
    /// </summary>
    public class ActivitesContextViewLearn: AbstractView
    {
        public Button SquatButton;
        public Button BikeButton;
        public Button Backbutton;
        /// <summary>
        /// Enables and displays the main Activities  learning  view
        /// </summary>
        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void CreateDefaultLayout()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Hides the main Activities  learning  view
        /// </summary>
        public override void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
