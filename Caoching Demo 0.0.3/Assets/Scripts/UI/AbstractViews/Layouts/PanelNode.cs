/** 
* @file PanelNode .cs
* @brief Contains the PanelNode   class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System;
using System.Collections;
using Assets.Scripts.UI.Layouts;
using System.Collections.Generic;
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.UI.AbstractViews.camera;
using Assets.Scripts.UI.AbstractViews.Layouts;
using Assets.Scripts.UI.AbstractViews.Templates;
using UnityEngine;
using UnityEngine.UI;

public delegate void PanelCamUpdateDelOnNode(PanelNode vNode);
namespace Assets.Scripts.UI.AbstractViews.Layouts
{
    /// <summary>
    /// A traversable node, representing a panel inside a panel container
    /// </summary>
    [Serializable]
    public class PanelNode : MonoBehaviour
    {

        private PanelID mID = new PanelID();
        private PanelNode mParent;
        private List<PanelNode> mChildren = new List<PanelNode>();
        private PanelSettings mPanelSettings;
        public event PanelCamUpdateDelOnNode PanelCamUpdated;


        public List<PanelNode> Children
        {
            get { return mChildren; }
            set { mChildren = value; }
        }

        public PanelNode Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        public PanelID Id
        {
            get { return mID; }
        }

        public PanelSettings PanelSettings
        {
            get { return mPanelSettings; }
            set { mPanelSettings = value; }
        }

        /// <summary>
        /// Unity3d does not like when monobehaviour inheriting objects are created with the new keyword.
        /// this method allows for the creation of a panel node and returns it
        /// </summary>
        /// <param name="vParent"></param>
        /// <param name="vTemplate"></param>
        /// <param name="vParentRectTransform"></param>
        /// <returns></returns>
        public static PanelNode CreatePanelNode(PanelNode vParent, PanelNodeTemplate vTemplate,
            RectTransform vParentRectTransform)
        {
            GameObject vNewPanelNodeGo = new GameObject();
            vNewPanelNodeGo.SetActive(false);
            RectTransform vRect = vNewPanelNodeGo.AddComponent<RectTransform>();

            //set its parent in unity's transform hierarchy
            vRect.transform.SetParent(vParentRectTransform.transform, false);
            PanelNode vResult = vNewPanelNodeGo.AddComponent<PanelNode>();
            vResult.Init(vParent, vTemplate, vParentRectTransform);
            vNewPanelNodeGo.SetActive(true);
            return vResult;
        }

        /// <summary>
        /// Initializes the PanelNode with the passed in template
        /// </summary>
        /// <param name="vParent">The panel nodes' parent</param>
        /// <param name="vTemplate"></param>
        public void Init(PanelNode vParent, PanelNodeTemplate vTemplate, RectTransform vParentRectTransform)
        {
            this.mParent = vParent;
            RectTransform vRectTransform = GetComponent<RectTransform>();
            if (vRectTransform == null)
            {
                vRectTransform = this.gameObject.AddComponent<RectTransform>();

            }
            
            PanelSettings = new PanelSettings(this); 
            if (vTemplate.HorizontalOrVerticalLayoutType != HorizontalOrVerticalLayoutGroupType.Null)
            {
                PanelSettings.Group = PanelNodeTemplate.AttachHorizontalOrVerticalLayoutGroup(this, vTemplate);
                PanelSettings.Group.padding = vTemplate.HorizontalOrVerticalPadding;
            }

            PanelSettings.LayoutElementComponent = gameObject.AddComponent<LayoutElement>();
            if (vParent != null)
            {
                PanelSettings.ModifyLayoutElement(vTemplate.PrefferedWidthOrHeight, vParent.PanelSettings.Group);
            }


        }
        public override bool Equals(object obj)
        {
            var node = obj as PanelNode;
            if (node != null)
            {
                var mNode = node;
                return mID.Id == mNode.mID.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return mID.GetHashCode();
        }
        /// <summary>
        /// Inserts a child at the current index. 
        /// </summary>
        /// <param name="vChild"></param>
        /// <param name="vIdx"></param>
        /// <returns></returns>
        public PanelNode InsertChildAt(PanelNode vChild, int vIdx)
        {
            return null;
        }

        /// <summary>
        /// Is the current PanelNode a leaf?
        /// </summary>
        /// <returns></returns>
        public bool IsLeaf()
        {
            return Children.Count == 0;
        }

        /// <summary>
        /// Inserts the child and sets it at the top of the children hierarchy
        /// </summary>
        /// <param name="vChild"></param>
        /// <returns></returns>
        public PanelNode InsertChildAtFront(PanelNode vChild)
        {
            return InsertChildAt(vChild, 0);
        }
        /// <summary>
        /// Inserts the child and sets it at the bottom of the children hierarchy
        /// </summary>
        /// <param name="vChild"></param>
        /// <returns></returns>
        public PanelNode InsertChildAtEnd(PanelNode vChild)
        {
            return InsertChildAt(vChild, Children.Count - 1);
        }

        /// <summary>
        /// Removes a child from the list of children(subsequently all its children are also removed)
        /// </summary>
        /// <param name="vChild"></param>
        /// <returns></returns>
        public PanelNode RemoveChild(PanelNode vChild)
        {
            return null;
        }

        /// <summary>
        /// Removes a child from the list of children(subsequently all its children are also removed) with an index parameter
        /// </summary>
        /// <param name="vIdx"></param>
        /// <returns></returns>
        public PanelNode RemoveChildAt(int vIdx)
        {
            return null;
        }


        /// <summary>
        /// Splits the panel in half, applies a template to the first half of the panel and a second template to the second half
        /// </summary>
        /// <param name="vType">how to split the node panel</param>
        /// <param name="vFirstChildTemplate">first child template</param>
        /// <param name="vSecondChildTemplate">second child template</param>
        /// <returns>the newly created panel's children</returns>
        public List<PanelNode> SplitPanelInHalf(HorizontalOrVerticalLayoutGroupType vType, PanelNodeTemplate vFirstChildTemplate,
            PanelNodeTemplate vSecondChildTemplate)
        {
            if (Children.Count > 0)
            {
                throw new InvalidSplitRequested();
            }
            List<PanelNode> vPanelNodes = new List<PanelNode>();
            PanelNode vFirstChild = PanelNode.CreatePanelNode(this, vFirstChildTemplate, PanelSettings.RectTransform);
            PanelNode vSecondChild = PanelNode.CreatePanelNode(this, vSecondChildTemplate, PanelSettings.RectTransform);

            //change the current HorizontalOrverticallayout group according to the type
            PanelNodeTemplate.AttachHorizontalOrVerticalLayoutGroup(this, vType);

            vPanelNodes.Add(vFirstChild);
            vPanelNodes.Add(vSecondChild);
            RemoveAllControlPanels();
            Children = vPanelNodes;
            return Children;
        }

        /// <summary>
        /// removes all currently connected control panels
        /// </summary>
        private void RemoveAllControlPanels()
        {
            Debug.Log("Remove all currently connected control panels");
        }

        /// <summary>
        /// Cleans up any resources on removal
        /// </summary>
        public void CleanUpOnRemoval()
        {
            this.Children = null;
            this.Parent = null;
        }

        /// <summary>
        /// sends a message to all controls panels that the body has been updated, and update
        /// current panel camera resources
        /// </summary>
        /// <param name="vBody"></param>
        public void UpdateBody(Body vBody)
        { 

            foreach (var vAbstractControlPanel in PanelSettings.ControlPanelSet)
            {
                vAbstractControlPanel.BodyUpdated(vBody);
            }

            PanelSettings.CameraToBodyPair.Body = vBody;
            if (PanelSettings.CameraToBodyPair.PanelCamera != null)
            {
                RenderedBody vRenderedBody = vBody.RenderedBody;
                PanelSettings.CameraToBodyPair.PanelCamera.UpdateLayerMask(vRenderedBody.CurrentLayerMask);
                if (PanelCamUpdated != null)
                {
                    PanelCamUpdated(this);
                }
            }
            else
            {
                StartCoroutine(UpdateCameraAfterEndOfFrame(vBody));
            }
        }



        /// <summary>
        /// to give a chance for unity to update the current rect transform, wait until the end of two frames
        /// then update the panel camera's viewport
        /// </summary>
        /// <returns></returns>
        IEnumerator UpdateCameraAfterEndOfFrame(Body vBody)
        {
            yield return new WaitForEndOfFrame();
            RenderedBody vRenderedBody = vBody.RenderedBody;
            PanelCameraSettings vPanelCameraSettings = new PanelCameraSettings(vRenderedBody.CurrentLayerMask, PanelSettings);
            PanelSettings.CameraToBodyPair.PanelCamera = PanelCameraPool.GetPanelCamResource(vPanelCameraSettings);
            PanelSettings.CameraToBodyPair.PanelCamera.SetDefaultTarget(vRenderedBody, 10);
            if (PanelCamUpdated != null)
            {
                PanelCamUpdated(this);
            }
        }
    }
    /// <summary>
    /// Panel node splits can only occur on leaf nodes
    /// </summary>
    public class InvalidSplitRequested : Exception { }

}