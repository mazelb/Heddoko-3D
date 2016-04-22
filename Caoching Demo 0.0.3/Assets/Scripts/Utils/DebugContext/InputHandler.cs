
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Utils.DebugContext
{
    /// <summary>
    /// Class that handles inputs and their respective actions
    /// Reference for mouse buttons: 0= left click, 1= right click, 2 = middle click
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        private static Dictionary<KeyCode, List<UnityAction>> sgKeyBindActionMaps = new Dictionary<KeyCode, List<UnityAction>>();
        private static Dictionary<int, List<UnityAction>> sgMouseActionBindings = new Dictionary<int, List<UnityAction>>(3);

        /// <summary>
        /// Registers an action to the internal register
        /// </summary>
        /// <param name="vKeyCode"></param>
        /// <param name="vAction"></param>
        public static void RegisterKeyboardAction(KeyCode vKeyCode, UnityAction vAction)
        {

            if (!sgKeyBindActionMaps.ContainsKey(vKeyCode))
            {
                List<UnityAction> vNewActionList = new List<UnityAction>();
                vNewActionList.Add(vAction);
                sgKeyBindActionMaps.Add(vKeyCode, vNewActionList);
            }
            else
            {
                List<UnityAction> vNewActionList = sgKeyBindActionMaps[vKeyCode];
                vNewActionList.Add(vAction);
            }


        }

        /// <summary>
        /// Registers a mouse input with an action
        /// </summary>
        /// <param name="vButtonCode"></param>
        /// <param name="vAction">The action to register</param>
        public static void RegisterMouseInputAction(int vButtonCode, UnityAction vAction)
        {

            if (!sgMouseActionBindings.ContainsKey(vButtonCode))
            {
                List<UnityAction> vNewActionList = new List<UnityAction>();
                vNewActionList.Add(vAction);
                sgMouseActionBindings.Add(vButtonCode, vNewActionList);
            }
            else
            {
                List<UnityAction> vNewActionList = sgMouseActionBindings[vButtonCode];
                vNewActionList.Add(vAction);
            }
        }

        /// <summary>
        ///  Removes an action from the given mouse button binding
        /// </summary>
        /// <param name="vMouseCode">the mouse button to unbinding the action from</param>
        /// <param name="vAction">the action to unbind</param>
        public static void RemoveMouseInputAction(int vMouseCode, UnityAction vAction)
        {
            if (sgMouseActionBindings.ContainsKey(vMouseCode))
            {
                if (sgMouseActionBindings[vMouseCode].Contains(vAction))
                {
                    sgMouseActionBindings[vMouseCode].Remove(vAction);
                }
            }
        }

        private void TryInvokeKeyActions(Event vEvent)
        {
            if (sgKeyBindActionMaps.ContainsKey(vEvent.keyCode))
            {
                for (int vI = 0; vI < sgKeyBindActionMaps[vEvent.keyCode].Count; vI++)
                {
                    sgKeyBindActionMaps[vEvent.keyCode][vI].Invoke();
                }

            }
        }
        void OnGUI()
        {
            var vEvent = Event.current;

            if (vEvent.isKey && vEvent.type == EventType.KeyDown)
            {
                TryInvokeKeyActions(vEvent);
            }

            if (vEvent.isMouse && vEvent.type == EventType.mouseDown)
            {
                int vMouseButton = vEvent.button;
                if (sgMouseActionBindings.ContainsKey(vMouseButton))
                {
                    foreach (var vUnityAction in sgMouseActionBindings[vMouseButton])
                    {
                        vUnityAction.Invoke();
                    }
                }
            }


        }

        /// <summary>
        ///  Removes an action from the given keybind
        /// </summary>
        /// <param name="vKeycode"></param>
        /// <param name="vAction"></param>
        public static void RemoveKeybinding(KeyCode vKeycode, UnityAction vAction)
        {

            if (sgKeyBindActionMaps.ContainsKey(vKeycode))
            {
                if (sgKeyBindActionMaps[vKeycode].Contains(vAction))
                {
                    sgKeyBindActionMaps[vKeycode].Remove(vAction);
                }
            }


        }
    }
}
