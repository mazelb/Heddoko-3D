// /**
// * @file TeamSelectionItem.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date January 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using System;
using Assets.Scripts.UI.RecordingLoading.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.RecordingLoading.View
{
    public class TeamSelectionItem : MonoBehaviour
    {
        public Text TextContentView;
        public Toggle Toggle;
        private TeamListItem mListItem;
        private Action<TeamListItem, bool> mOnToggleAction;
        public Action<TeamListItem, bool> OnToggleAction
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
        public TeamListItem ListItem
        {
            get
            {
                return mListItem;
            }
            set
            {
                mListItem = value;
                TextContentView.text = mListItem.GetName;
            }
        }

    }
}