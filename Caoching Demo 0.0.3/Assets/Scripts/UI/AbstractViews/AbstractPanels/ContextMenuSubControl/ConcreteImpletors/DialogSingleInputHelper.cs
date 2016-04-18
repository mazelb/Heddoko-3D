using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.ContextMenuSubControl.ConcreteImpletors
{
    public class DialogSingleInputHelper: MonoBehaviour
    {
        [SerializeField]
        public InputField SingleInputField;
 

        public void Refresh()
        {
            SingleInputField.text = ""; 
        }

        /// <summary>
        /// Validates the input 
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            var vValidInput = SingleInputField.text.Trim().Length > 0;

            return vValidInput;
        }
    }
}
