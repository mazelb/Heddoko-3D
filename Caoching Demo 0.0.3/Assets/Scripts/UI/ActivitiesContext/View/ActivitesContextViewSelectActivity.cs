/** 
* @file ActivitesContextViewSelectActivity.cs
* @brief Contains the ActivitesContextViewSelectActivity class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System; 
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ActivitiesContext.View
{
    /// <summary>
    /// The view associated with the learning  view
    /// </summary>
    [Serializable]
    public class ActivitesContextViewSelectActivity: AbstractInActivityView
    {
         
        public Button SquatButton;
        public Button BikeButton;
        [SerializeField]
        private Button mBackButton;
 

        public override Button Backbutton
        {
            get
            {
                return mBackButton;
            }
        }

        /// <summary>
        /// Enables and displays the main Activities  learning  view
        /// </summary>
        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void CreateDefaultLayout()
        {
            throw new NotImplementedException();
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
