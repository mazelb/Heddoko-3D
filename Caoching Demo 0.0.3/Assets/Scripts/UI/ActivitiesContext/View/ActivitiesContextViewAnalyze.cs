/* @file ActivitiesContextViewAnalyze.cs
* @brief Contains the ActivitiesContextViewAnalyze class
* @author Mohammed Haider(mohamed @heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ActivitiesContext.View
{
    /// <summary>
    /// Represents the analysis context view
    /// </summary>
    public class ActivitiesContextViewAnalyze : AbstractView
    {

        public GameObject HeddokoModel;
        public Button Train;
        public Button Cancel;

        //todo: meter 

        /// <summary>
        /// Enables and shows the view
        /// </summary>
        public override void Show()
        {
            HeddokoModel.SetActive(true);
            gameObject.SetActive(true);
        }

        public override void CreateDefaultLayout()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Hides the view
        /// </summary>
        public override void Hide()
        {
            HeddokoModel.SetActive(false);
            gameObject.SetActive(false);
        }

        public Button Backbutton { get; set; }
    }
}
