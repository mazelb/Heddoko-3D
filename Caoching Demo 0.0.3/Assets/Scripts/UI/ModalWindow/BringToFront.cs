using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI.ModalWindow
{
    /// <summary>
    /// Simple script that brings the window to the front
    /// </summary>
    public class BringToFront: MonoBehaviour
    {
        void OnEnable()
        {
            transform.SetAsLastSibling();
        }
    }
}
