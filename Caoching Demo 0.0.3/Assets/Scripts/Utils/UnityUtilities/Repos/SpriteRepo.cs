 
using UnityEngine;

namespace Assets.Scripts.Utils.UnityUtilities.Repos
{
    /// <summary>
    /// A repository of sprites located in the resources folder.
    /// </summary>
  public class SpriteRepo
    {
        public static Sprite BluetoothIcon
        {
            get
            {
                return Resources.Load<Sprite>("ArtAssets/General/Bluetooth/bluetoothIcon");
                //bluetoothIcon
            }
        }
    }
}
