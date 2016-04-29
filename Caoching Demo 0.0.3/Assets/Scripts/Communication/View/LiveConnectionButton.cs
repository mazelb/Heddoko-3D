/** 
* @file LiveConnectionButton.cs
* @brief Contains the LiveConnectionButton class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Communication.View
{
    /// <summary>
    /// A button for the live connection view
    /// </summary>
    public class LiveConnectionButton : MonoBehaviour
    {
        public Color DisabledColor;
        public Color ShownColor;
        public Button Button;
        public Image Image;
        public Text Text;


        public void Disable()
        {
            Image.color = DisabledColor;
            Text.color = DisabledColor;
            Button.interactable = false;
        }

        public void Enable()
        {
            Button.interactable = true;
            Image.color = ShownColor;
            Text.color = ShownColor;
        }
        
    }
}