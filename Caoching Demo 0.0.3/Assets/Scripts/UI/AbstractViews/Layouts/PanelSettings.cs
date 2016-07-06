/** 
* @file PanelSettings.cs
* @brief Contains the PanelSettings  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections.Generic;
using System.Linq; 
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.Move3D_Visualization.Cameras;
using Assets.Scripts.UI.AbstractViews.AbstractPanels;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;
using Assets.Scripts.UI.AbstractViews.camera;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.Layouts
{
    /// <summary>
    /// Settings of a Panel Node
    /// </summary>
    public class PanelSettings
    {
        private LayoutElement mLayoutElementComponent;
        private HorizontalOrVerticalLayoutGroup mLayoutGroup;
        private List<AbstractControlPanel> mControlPanelSet = new List<AbstractControlPanel>();
        private PanelCameraToBodyPair mPanelCameraToBodyPair = new PanelCameraToBodyPair();
        private float mLayoutElementModifier;
        private AbstractControlPanelBuilder mBuilder = new AbstractControlPanelBuilder();
        private PanelNode PanelNode;
 
        public PanelCameraToBodyPair CameraToBodyPair
        {
            get { return mPanelCameraToBodyPair; }
            set { mPanelCameraToBodyPair = value; }
        }

        public LayoutElement LayoutElementComponent
        {
            get { return mLayoutElementComponent; }
            set { mLayoutElementComponent = value; }
        }

        public HorizontalOrVerticalLayoutGroup Group
        {
            get { return mLayoutGroup; }
            set { mLayoutGroup = value; }
        }

        public RectTransform RectTransform
        {
            get { return PanelNode.GetComponent<RectTransform>(); }
        

        }

        public List<AbstractControlPanel> ControlPanelSet
        {
            get { return mControlPanelSet; }
            set { mControlPanelSet = value; }
        }

        /// <summary>
        /// Return the first hit of a control panel from the type requested.
        /// Note: can return null
        /// </summary>
        /// <param name="vPanelType">The requested type</param>
        /// <returns></returns>
        public AbstractControlPanel GetPanelOfType(ControlPanelType vPanelType)
        {
            return ControlPanelSet.First(x => x.PanelType == vPanelType);

        }
        public PanelSettings(PanelNode vAssociatedNode)
        {
            PanelNode = vAssociatedNode;
        }
        /// <summary>
        /// Modify the layout element according to the parent layout group
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vParentHorizontalLayoutGroup"></param>
        public void ModifyLayoutElement(float vValue, HorizontalOrVerticalLayoutGroup vParentHorizontalLayoutGroup)
        {
            if (vParentHorizontalLayoutGroup != null)
            {
                mLayoutElementModifier = vValue;
                if (vParentHorizontalLayoutGroup is HorizontalLayoutGroup)
                {
                    mLayoutElementComponent.preferredWidth = 0;
                    mLayoutElementComponent.preferredWidth = mLayoutElementModifier;
                }
                else if (vParentHorizontalLayoutGroup is VerticalLayoutGroup)
                {
                    mLayoutElementComponent.preferredHeight = 0;
                    mLayoutElementComponent.preferredHeight = mLayoutElementModifier;
                }
            }
        }

        /// <summary>
        /// updates the control panel in the control panel set with the list of subcontrols
        /// </summary>
        /// <param name="vAbstractControlPanel"></param>
        /// <param name="vSubcontrols"></param>
        public void UpdatePanel(AbstractControlPanel vAbstractControlPanel, List<AbstractSubControl> vSubcontrols)
        {
            if (ControlPanelSet.Contains(vAbstractControlPanel))
            {
                vAbstractControlPanel.UpdateSubControlList(vSubcontrols);
            }
            else
            {
                ControlPanelSet.Add(vAbstractControlPanel);
                vAbstractControlPanel.UpdateSubControlList(vSubcontrols);
            }

        }

        /// <summary>
        /// Updates the body of the current set of control panels
        /// </summary>
        /// <param name="vBody"></param>
        public void UpdateBodyInControlPanels(Body vBody)
        {
            foreach (var vAbsCtrlPanel in ControlPanelSet)
            {
                vAbsCtrlPanel.BodyUpdated(vBody);
            }
        }

        /// <summary>
        /// Initialzes the panel with sub control and an optional body
        /// </summary>
        /// <param name="vControlPanelTypes"></param> 
        /// <param name="vNeedRenderedBody"></param>
        /// <param name="vBody"></param>
        public void Init(List<ControlPanelType> vControlPanelTypes, bool vNeedRenderedBody, Body vBody = null)
        {
            int vLayer;
            foreach (var controlPanelType in vControlPanelTypes)
            {
                AbstractControlPanel vAbstractControlPanel = mBuilder.BuildPanel(controlPanelType);

                if (!mControlPanelSet.Contains(vAbstractControlPanel))
                {
                    mControlPanelSet.Add(vAbstractControlPanel);
                    vAbstractControlPanel.Init(RectTransform, PanelNode);
                }
            }
            if (vBody != null)
            {
                if (vNeedRenderedBody)
                {
                    RenderedBody vRendered = RenderedBodyPool.RequestResource(vBody.BodyType);
                    vBody.UpdateRenderedBody(vRendered);
                }
                PanelNode.UpdateBody(vBody);
            }
        }

        /// <summary>
        /// Request resources for the current panel. Optional argument: pass in the rendered body to use instead of 
        /// requesting a new one. 
        /// </summary>
        /// <param name="vOptionalRenderedBody">The RenderedBody to use instead of a new one</param>
        public void RequestResources(RenderedBody vOptionalRenderedBody = null)
        {
            if (CameraToBodyPair.Body != null)
            {
                Body vBody = CameraToBodyPair.Body;
                RenderedBody vRenderedBody = null;
                if (vOptionalRenderedBody == null)
                {
                    RenderedBody vRendered = RenderedBodyPool.RequestResource(vBody.BodyType);
                    vBody.UpdateRenderedBody(vRendered);
                    vRenderedBody = vBody.RenderedBody;
                }
                else
                {
                    vRenderedBody = vOptionalRenderedBody;
                }

                PanelCameraSettings vPanelCameraSettings = new PanelCameraSettings(vRenderedBody.CurrentLayerMask, this);
                CameraToBodyPair.PanelCamera = PanelCameraPool.GetPanelCamResource(vPanelCameraSettings);
                CameraToBodyPair.PanelCamera.SetDefaultTarget(vRenderedBody, 10);
            }


        }

        public AbstractControlPanel GetControlPanel(AbstractControlPanel vControlPanel)
        {
            return null;
        }

        /// <summary>
        /// releases resources back into the pool upon request
        /// </summary>
        public void ReleaseResources()
        {
            //unset the camera
            foreach (var vAbstractControlPanel in ControlPanelSet)
            {
                vAbstractControlPanel.ReleaseResources();
            }
            if (CameraToBodyPair.PanelCamera != null)
            {
                PanelCameraPool.Release(CameraToBodyPair.PanelCamera);
            }
            if (CameraToBodyPair.Body != null)
            {
                CameraToBodyPair.Body.ReleaseResources(); 
            }
        }
    }
}
