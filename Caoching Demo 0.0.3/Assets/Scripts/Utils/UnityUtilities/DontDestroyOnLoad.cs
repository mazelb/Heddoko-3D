 
using UnityEngine;

namespace Assets.Scripts.Utils.UnityUtilities
{
    /// <summary>
    /// This call will prevent a game object from being destroyed on scene load
    /// </summary>
    public class DontDestroyOnLoad: MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(transform.gameObject);
        }
    }
}
