/** 
* @file TaggingContainer.cs
* @brief Contains the TaggingContainer class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UI.AbstractViews.ContextSpecificContainers.Tags;
using Assets.Scripts.UI.AbstractViews.SelectableGridList;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Tagging
{
    /// <summary>
    /// A container of tags.
    /// </summary>
    [Serializable]
    public class TaggingContainer : MonoBehaviour, ITaggingManagerConsumer
    {
        public ExportViewSelectableGridList GridList;
        public ImportRecordingGridSelectionComponent ExportComponent;
        public const int MaxTagNumber = 5;
        private LayoutElement mLayoutElement;
        public TagViewElement TagPrefab;
        private Dictionary<string, TagViewElement> mTags = new Dictionary<string, TagViewElement>(MaxTagNumber);
        //list of inactive tags
        private readonly List<TagViewElement> mInactiveTags = new List<TagViewElement>(5);
        public Button AddTagButton;
        [SerializeField]
        public TagAutocomplete AutoComplete;
        private ListView mFloatListView;
        private TaggingManager mTaggingManager;

        /// <summary>
        /// Returns the number of tags in the container
        /// </summary>
        public int TagCount
        {
            get { return mTags.Count; }
        }
        public ListView FloatListView
        {
            get { return mFloatListView; }
            set
            {
                mFloatListView = value;
                AutoComplete.FloatingListView = FloatListView;
            }
        }

        public TaggingManager TaggingManager
        {
            get { return mTaggingManager; }
            set
            {
                mTaggingManager = value;
                AutoComplete.TaggingManager = mTaggingManager;
            }
        }

        public LayoutElement LayoutElement
        {
            get
            {
                if (mLayoutElement == null)
                {
                    mLayoutElement = GetComponent<LayoutElement>();
                }
                return mLayoutElement;
            }
            set { mLayoutElement = value; }
        }

   

        /// <summary>
        /// initiates the logic chain to add a tag element into the container
        /// </summary>
        public void InitiateTagAdd()
        {
            //disable Tag addition
            DisableTagAddition();
            //verify if the container is capable of holding more elements
            if (TagCount < MaxTagNumber)
            {
                AutoComplete.RegisterAutoCompleteSelection(OnTagAddCallback);
                AutoComplete.Show();
            }
            AutoComplete.RegisterOnHide(ValidateTagAddition);
        }

        /// <summary>
        /// add a tag to the container
        /// </summary>
        /// <param name="vData"></param>
        internal void AddTag(string vData)
        {
            if (!mTags.ContainsKey(vData))
            {
                //add a tag view to the container
                Tag vTag = TaggingManager.GetTagByTitle(vData);
                //verify if the inactive pool already has an item
                TagViewElement vNewViewElem = null;
                if (mInactiveTags.Count > 0)
                {
                    vNewViewElem = mInactiveTags[0];
                    mInactiveTags.RemoveAt(0);
                }
                else
                {
                    vNewViewElem = Instantiate(TagPrefab);
                    vNewViewElem.transform.SetParent(transform, false);
                    vNewViewElem.RegisterOnRemoveEvent(RemoveTagViewElement);
                }
                vNewViewElem.Init(vTag);
                mTags.Add(vData, vNewViewElem);
                AutoComplete.Hide();
                ValidateTagAddition();
            }
        }
        /// <summary>
        /// Callback performed on tag addition
        /// </summary>
        private void OnTagAddCallback(string vData)
        {
            AutoComplete.RemoveAutoCompleteSelection(OnTagAddCallback);
            //verify if element is already in the container
            if (!mTags.ContainsKey(vData))
            {
                //add a tag view to the container
                Tag vTag = TaggingManager.GetTagByTitle(vData);
                //verify if the inactive pool already has an item
                TagViewElement vNewViewElem = null;
                if (mInactiveTags.Count > 0)
                {
                    vNewViewElem = mInactiveTags[0];
                    mInactiveTags.RemoveAt(0);
                }
                else
                {
                    vNewViewElem = Instantiate(TagPrefab);
                    vNewViewElem.transform.SetParent(transform,false);
                    vNewViewElem.RegisterOnRemoveEvent(RemoveTagViewElement);
                }
                vNewViewElem.Init(vTag);
                mTags.Add(vData, vNewViewElem);
                GridList.DataSource[ExportComponent.Index].AddTag(vData);
                AutoComplete.Hide();
                ValidateTagAddition();
            }

        }


        /// <summary>
        /// Remove a tag view element
        /// </summary>
        /// <param name="vElement">the element to remove</param>
        public void RemoveTagViewElement(TagViewElement vElement)
        {
            if (!mTags.ContainsKey(vElement.Tag.Title))
            {
                return;
            }
            vElement.Hide();
            mTags.Remove(vElement.Tag.Title);
            //place it into the inactive list
            mInactiveTags.Add(vElement);
            vElement.RegisterOnRemoveEvent(RemoveTagViewElement);

        }
        /// <summary>
        /// Disables the addition of new tags to this container
        /// </summary>
        public void DisableTagAddition()
        {
            AddTagButton.interactable = false;
        }

        /// <summary>
        /// Check  if the container has reached its capacity. Disable functions to add new tags to this container
        /// </summary>
        public void ValidateTagAddition()
        {
            if (TagCount < MaxTagNumber)
            {
                AddTagButton.interactable = true;
            }
            else
            {
                AddTagButton.interactable = false;
            }
            AutoComplete.RemoveOnHideDelegate(ValidateTagAddition);
        }

        void Awake()
        {
            AddTagButton.onClick.AddListener(InitiateTagAdd);
            AutoComplete.RegisterOnSubmit(OnTagAddCallback);
        }

        void OnApplicationQuit()
        {
            AutoComplete.RemoveOnSubmit(OnTagAddCallback);  

        }
        /// <summary>
        /// remove all tags
        /// </summary>
        public void RemoveAllTags()
        {
            List<string> vKeys = mTags.Keys.ToList();
            foreach (var vKey in vKeys)
            {
                RemoveTagViewElement(mTags[vKey]);
            }
        }
    }
}