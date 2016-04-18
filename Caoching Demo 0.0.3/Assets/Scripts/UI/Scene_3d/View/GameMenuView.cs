 
using UnityEngine; 
namespace Assets.Scripts.UI.Scene_3d.View
{
    /// <summary>
    /// The main menu view
    /// </summary>
   public  class GameMenuView: MonoBehaviour, IInGameMenuItem
   { 
        /// <summary>
        /// Hides the main menu
        /// </summary>
       public void Hide()
       {
           gameObject.SetActive(false);
       }
        /// <summary>
        /// Shows the main menu
        /// </summary>
       public void Show()
       {
            gameObject.SetActive(true);
        }

   
    }
}
