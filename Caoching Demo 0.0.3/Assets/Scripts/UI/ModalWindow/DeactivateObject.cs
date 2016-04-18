 
using UnityEngine;

namespace Assets.Scripts.UI.ModalWindow
{
    public class DeactivateObject : MonoBehaviour
    {

        void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}
