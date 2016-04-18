using UnityEngine;

namespace Assets.Scripts.UI.MainScene.View
{
    /// <summary>
    /// A panel that contains all the available recordings in view. 
    /// </summary>
   public class AvailableRecordingPanelView:MonoBehaviour
    {
        /// <summary>
        /// Shows the available recordings panel view
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }
        /// <summary>
        /// Hides the available recordings panel view
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
