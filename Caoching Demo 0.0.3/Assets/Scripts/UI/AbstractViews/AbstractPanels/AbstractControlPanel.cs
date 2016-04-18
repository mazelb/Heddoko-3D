
/* @file AbstractControlPanel.cs
* @brief Contains the ActivitiesContextViewAnalyze class
* @author Mohammed Haider(mohamed @heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels
{
    /// <summary>
    /// An abstract control panel that has a list of abstract sub control panels that change
    /// the state of the current view
    /// </summary>
    public abstract class AbstractControlPanel : MonoBehaviour, IEquatable<AbstractControlPanel>
    {
        protected List<AbstractSubControl> mSubControls = new List<AbstractSubControl>();
        private ControlPanelType mControlPanelType;
        private RectTransform mParent;
        private RectTransform mCurrentRectTransform;
        public PanelNode ParentNode { get; set; }
        public ControlPanelType PanelType
        {
            get { return mControlPanelType; }
        }

        /// <summary>
        /// initializes the current abstract control panel
        /// </summary>
        /// <param name="vParent"></param>
        /// <param name="vParentNode"></param>
        public virtual void Init(RectTransform vParent, PanelNode vParentNode)
        {
            this.ParentNode = vParentNode;
            mParent = vParent;
            mCurrentRectTransform = GetComponent<RectTransform>();
            mCurrentRectTransform.transform.SetParent(mParent, false);
        }

        /// <summary>
        /// update the subcontrol list
        /// </summary>
        /// <param name="vSubcontrols"></param>
        /// <returns>List of subcontrols that were succesfully added</returns>
        public List<AbstractSubControl> UpdateSubControlList(List<AbstractSubControl> vSubcontrols)
        {
            List<AbstractSubControl> vABDifference = mSubControls.Except(vSubcontrols).ToList();
            List<AbstractSubControl> vBADifference = vSubcontrols.Except(mSubControls).ToList();
            List<AbstractSubControl> vReturn = new List<AbstractSubControl>();
            //remove the difference between a and b
            foreach (var vAbsSubCtrl in vABDifference)
            {
                mSubControls.Remove(vAbsSubCtrl);
            }
            //validate and add in the difference between b and a
            foreach (var vAbsSubCtrl in vBADifference)
            {
                bool vIsValid = ControlPanelToSubControlValidator.Validate(this, vAbsSubCtrl);
                if (vIsValid)
                {
                    mSubControls.Add(vAbsSubCtrl);
                    vReturn.Add(vAbsSubCtrl);
                }
            }
            return vReturn;

        }


        /// <summary>
        /// remove a subcontrol from the panel
        /// </summary>
        /// <param name="vSubControl">the subcontrol to remove</param>
        /// <returns>succesfull removal</returns>
        public bool Remove(AbstractSubControl vSubControl)
        {
            //verify if the Subcontrol exists first
            if (mSubControls.Contains(vSubControl))
            {
                mSubControls.Remove(vSubControl);
                return true;
            }
            return false;
        }

        /// <summary>
        /// informs current subcontrols that the body has been updated
        /// </summary>
        /// <param name="vBody"></param>
        public virtual void BodyUpdated(Body vBody)
        {

        }

        /// <summary>
        /// IEquatable implementation to avoid boxing/unboxing while iterating through a list of AbstractControlPanels
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AbstractControlPanel other)
        {
            if (other != null)
            {
                return gameObject.GetInstanceID() == other.gameObject.GetInstanceID();
            }
            return false;
        }

        /// <summary>
        /// Releases resources used by the current control panel
        /// </summary>
        public abstract void ReleaseResources();
    }
}