/** 
* @file ConnectToBrainpackView.cs
* @brief Contains the ConnectToBrainpackView     class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Utils.UnityUtilities;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Scene_3d.View
{ 
    /// <summary>
    /// ConnectToBrainpackView class (This view the in 3d scene that is associated with the "Connect To Brainpack" button)
    /// </summary>
    public class ConnectToBrainpackView : MonoBehaviour, IInGameMenuItem
    {
        //once this view is enabled, then the brainpack info panel is shown , the info panel copies 
        //this Placeholder transform's attribute
        public RectTransform PlaceholderTransform; 

        /// <summary>
        /// the "Connect to Brainpack" button
        /// </summary>
        public Button CurrentButton
        {
            get
            {
                return gameObject.GetComponent<Button>();
            }
        }

        /// <summary>
        /// Hide the associated Components from this view
        /// </summary>
        public void Hide()
        {
            BrainpackConnectionController.Instance.View.Hide();
        } 

        /// <summary>
        /// Shows the associated Components from this view
        /// </summary>
        public void Show()
        {
            RectTransformUtilities.CopyRectTransformProperties(BrainpackConnectionController.Instance.View.RectTransform, PlaceholderTransform);
            BrainpackConnectionController.Instance.View.Show();

        }
    }
}
