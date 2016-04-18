using UnityEngine.UI;
namespace Assets.Scripts.UI.Scene_3d
{
    /// <summary>
    /// Provides an interface for in game menu items
    /// </summary>
    public interface IInGameMenuItem
    {
        /// <summary>
        /// Hides the item from view
        /// </summary>
        void Hide();
        /// <summary>
        /// Bring the item into view
        /// </summary>
        void Show();
 

    }
}
