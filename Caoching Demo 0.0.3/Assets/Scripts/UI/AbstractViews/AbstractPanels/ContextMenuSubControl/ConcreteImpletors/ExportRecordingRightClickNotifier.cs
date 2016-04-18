/** 
* @file ExportRecordingRightClickNotifier.cs
* @brief Contains the ExportRecordingRightClickNotifier class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.UI.AbstractViews.ContextSpecificContainers.Importation;
using Assets.Scripts.UI.AbstractViews.SelectableGridList;
using Assets.Scripts.Utils.DebugContext;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.ContextMenuSubControl.ConcreteImpletors
{
    /// <summary>
    /// In the export recording view, this listens to right click events and notifies the observer immediately.
    /// </summary>
    public class ExportRecordingRightClickNotifier : MonoBehaviour
    {
        public ImportViewSelectableListModifier ExportListModifier;
        public ExportLocalRecordingToDB RecordingsExporter;
        public RightClickButtonContainer Container;
     

        /// <summary>
        /// initialize single action and multple action collections
        /// </summary>
        void Awake()
        {
            ExportListModifier = new ImportViewSelectableListModifier();
            ExportListModifier.RecordingsExporter = RecordingsExporter;
            ExportListModifier.Init();

        }


        void OnEnable()
        {
            InputHandler.RegisterKeyboardAction(KeyCode.Menu, InitiateRightClick);
            InputHandler.RegisterMouseInputAction(1, InitiateRightClick);
            InputHandler.RegisterMouseInputAction(0, InitiateLeftClick);
        }

        void OnDisable()
        {
            InputHandler.RemoveKeybinding(KeyCode.Menu, InitiateRightClick);
            InputHandler.RemoveMouseInputAction(1, InitiateRightClick);
            InputHandler.RemoveKeybinding(0, InitiateLeftClick);
        }

        void InitiateRightClick()
        {
            Container.Initialize(ExportListModifier.StructContainer,Input.mousePosition);
        }

        /// <summary>
        /// Left click has been initiated
        /// </summary>
        void InitiateLeftClick()
        {
            Container.HideIfOutOfContainerBounds(Input.mousePosition);
        }

        /// <summary>
        /// verifies if the observer is in a notified state. 
        /// </summary>
        /// <returns></returns>
        bool ObserverInitiated()
        {
            return true;
        }


    }
}