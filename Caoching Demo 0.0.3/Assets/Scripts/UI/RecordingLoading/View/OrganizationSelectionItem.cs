// /**
// * @file OrganizationSelectionItem.cs
// * @brief Contains the OrganizationSelectionItem class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date January 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using System;
using Assets.Scripts.UI.RecordingLoading.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.RecordingLoading.View
{
    /// <summary>
    /// a selectable organization item
    /// </summary>
    public class OrganizationSelectionItem : MonoBehaviour
    {
        public Text TextContentView;
        public Toggle Toggle;
        private OrganizationListItem mListItem;
        private Action<OrganizationListItem, bool> mOnToggleAction;
        public Action<OrganizationListItem, bool> OnToggleAction
        {
            get
            {
                return mOnToggleAction;
            }
            set
            {
                mOnToggleAction = value;
                Toggle.onValueChanged.AddListener(ToggleCallback);  
            }
        }

        /// <summary>
        /// Callback after the toggle has been selected
        /// </summary>
        /// <param name="vBoolValue"></param>
        private void ToggleCallback(bool vBoolValue)
        {
            if (mOnToggleAction != null)
            {
                mOnToggleAction(mListItem, vBoolValue);
            }
        }
        /// <summary>
        /// List item property
        /// </summary>
        public OrganizationListItem ListItem
        {
            get { return mListItem; }
            set
            {
                mListItem = value;
                TextContentView.text = mListItem.Organization.Name;
            }
        }



    }
}