/** 
* @file LayoutContainer.cs
* @brief Contains the LayoutContainer   class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.Templates;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.Layouts
{
    /// <summary>
    /// The container for a tree of Panel nodes 
    /// </summary>
    public class LayoutContainer
    {
        private PanelNode mRoot;
        private List<PanelNode> mNodes = new List<PanelNode>();
        private RectTransform mRectTransform;
        /// <summary>
        /// Is the container currently empty?
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return mRoot == null;
        }

        /// <summary>
        /// returns the root of the container
        /// </summary>
        /// <returns></returns>
        public PanelNode GetRoot()
        {
            return mRoot;
        }

        /// <summary>
        /// Get the panel currently rendering the body. 
        /// Note: will return null if no panels are found
        /// </summary>
        /// <param name="vStartNode">the start node </param>
        /// <param name="vBody">the body </param>
        /// <returns></returns>
        public PanelNode GetPanelOfBody(PanelNode vStartNode, Body vBody)
        {
            PanelNode vEndResult = null;
            if (vStartNode.PanelSettings.CameraToBodyPair != null &&
                vStartNode.PanelSettings.CameraToBodyPair.Body == vBody)
            {
                vEndResult = vStartNode;
            }
            else
            {
                if (vStartNode.Children.Count > 0)
                {
                    foreach (var vChild in vStartNode.Children)
                    {
                        vEndResult = GetPanelOfBody(vChild, vBody);
                        if (vEndResult != null)
                        {
                            break;
                        }
                    }
                }
            }
            return vEndResult;
        }

        public PanelNode FindPanelNode(PanelNode vStartNode, PanelNode vEndNode)
        {
            PanelNode vEndResult = null;
            if (vStartNode.Equals(vEndNode))
            {
                vEndResult = vStartNode;
            }
            else
            {
                if (vStartNode.Children.Count > 0)
                {
                    foreach (var vChild in vStartNode.Children)
                    {
                        vEndResult = FindPanelNode(vChild, vEndNode);
                        if (vEndResult != null)
                        {
                            break;
                        }
                    }
                }
            }
            return vEndResult;
        }

        /// <summary>
        /// Get the first panel found that is playing the recording
        /// Note: will return null if no panels are found
        /// </summary>
        /// <param name="vRecordingUuid">the recordings's UUID</param>
        /// <returns></returns>
        public PanelNode GetPanelOfRecording(PanelNode vStartNode, string vRecordingUuid)
        {
            Body vBody = BodiesManager.Instance.GetBodyFromRecordingUUID(vRecordingUuid);
            PanelNode vEndResult = null;
            if (vBody != null)
            {
                vEndResult = GetPanelOfBody(vStartNode, vBody);
            }
            return vEndResult;

        }

        /// <summary>
        /// Get the first panel found that is playing the recording
        /// Note: will return null if no panels are found
        /// </summary>
        /// <param name="vStartNode">the node to start the search from</param>
        /// <param name="vRecording">the recording</param>
        /// <returns></returns>
        public PanelNode GetPanelOfRecording(PanelNode vStartNode, BodyFramesRecording
            vRecording)
        {
            PanelNode vEndResult = null;
            string vRecUuid = vRecording.BodyRecordingGuid;
            return GetPanelOfRecording(vStartNode, vRecUuid);
        }

        /// <summary>
        /// Replaces the element with a new one
        /// </summary>
        /// <param name="vNewElement">the new  panel node</param>
        /// <param name="vOldElement">the old panel node </param>
        /// <returns></returns>
        public PanelNode Replace(PanelNode vNewElement, PanelNode vOldElement)
        {
            vNewElement.Children = vOldElement.Children;
            vNewElement.Parent = vOldElement.Parent;
            foreach (var vChild in vOldElement.Children)
            {
                vChild.Parent = vNewElement;
            }
            vOldElement.CleanUpOnRemoval();
            return vOldElement;
        }

        /// <summary>
        /// Performs checks on the node
        /// </summary>
        /// <param name="vNode">  </param>
        /// <returns></returns>
        private bool Validate(PanelNode vNode)
        {
            return true;
        }


        /// <summary>
        /// Checks if the vNode is the Root
        /// </summary>
        /// <param name="vNode"></param>
        /// <returns></returns>
        private bool IsRoot(PanelNode vNode)
        {
            return vNode.Equals(mRoot);
        }

        /// <summary>
        /// Adds a root to an empty container. else returns null if there is a root
        /// </summary>
        /// <param name="vParentRectTransform"></param>
        /// <param name="vTemplate"></param>
        /// <returns></returns>
        public PanelNode AddRoot(RectTransform vParentRectTransform ,PanelNodeTemplate vTemplate)
        {
            if (IsEmpty())
            { 
                mRoot = PanelNode.CreatePanelNode(null, vTemplate, vParentRectTransform); 
                return mRoot;
            }
            return null;
        }

        public PanelNode AddPanelNode(PanelNode vParent, RectTransform vParentRectTransform, PanelNodeTemplate vTemplate)
        {
           PanelNode vChild = PanelNode.CreatePanelNode(vParent, vTemplate, vParentRectTransform);
            return AddPanelNode(vParent, vChild);
        }

        /// <summary>
        /// Adds a panel node to a parent 
        /// </summary>
        /// <param name="vParent"></param>
        /// <param name="vChild"></param>
        /// <returns></returns>
        public PanelNode AddPanelNode(PanelNode vParent , PanelNode vChild)
        {
             vParent.Children.Add(vChild);
            return vChild;
        }


        /// <summary>
        /// Returns the children of the passed in PanelNode
        /// </summary>
        /// <param name="vParent"></param>
        /// <returns></returns>
        public static List<PanelNode> GetChildren(PanelNode vParent)
        {
            return vParent.Children;
        }
 


    }
}