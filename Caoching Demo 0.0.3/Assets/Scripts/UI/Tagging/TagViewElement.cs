/** 
* @file TagViewElement.cs
* @brief Contains the TagViewElement class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Tagging
{
    public delegate void RemoveTagDel(TagViewElement vTagElement);
    /// <summary>
    ///  a tag view element, held by a tagging container
    /// </summary>
    public class TagViewElement : MonoBehaviour
    {
        public Button RemoveTagButton; 
        public Tag Tag;
        public Text TagText;
        public event RemoveTagDel RemoveTagViewElement;

        /// <summary>
        /// initializes the tag view with the given tag
        /// </summary> 
        /// <param name="vTag"></param>
        public void Init( Tag vTag)
        {
            gameObject.SetActive(true);
            Tag = vTag;
            TagText.text = vTag.Title;
            RemoveTagButton.onClick.AddListener(Remove);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnAwake()
        {
           
        }
        public void Remove()
        {
            if (RemoveTagViewElement != null)
            {
                RemoveTagViewElement(this);
            }
        }

        /// <summary>
        /// Remove the delegate from the event handler
        /// </summary>
        /// <param name="vDel"></param>
        public void RemoveOnRemoveEvent(RemoveTagDel vDel)
        {
            RemoveTagViewElement -= vDel;
        }

        /// <summary>
        /// Register a delegate to the event handlers
        /// </summary>
        /// <param name="vDel"></param>
        public void RegisterOnRemoveEvent(RemoveTagDel vDel)
        {
            RemoveTagViewElement += vDel;
        }
    }
}