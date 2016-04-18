/** 
* @file RightClickButtonContainer.cs
* @brief Contains the RightClickButtonContainer class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.ContextMenuSubControl.ConcreteImpletors
{
    /// <summary>
    /// A container for buttons for the right click context menu
    /// </summary>
    public class RightClickButtonContainer : MonoBehaviour
    {
        List<RightClickButtonElement> mElements = new List<RightClickButtonElement>(3);
        /// <summary>
        /// A container for the buttons
        /// </summary>
        public GameObject ButtonContainer;
        public GameObject DefaultItemPrefab;
        private RectTransform mRectTransform;
        private bool mIsHidden = true;
        public RectTransform Bounds;
        public Camera UiCamera;

        /// <summary>
        /// (GETTER) the rect transform of the right click button container. 
        /// </summary>
        public RectTransform ButtonContainerRectTransform
        {
            get
            {
                if (mRectTransform == null)
                {
                    mRectTransform = ButtonContainer.GetComponent<RectTransform>();
                }
                return mRectTransform;
            }
        }
        void OnEnable()
        {
            DefaultItemPrefab.SetActive(false);
        }
        /// <summary>
        /// disable the gameobject on awake
        /// </summary>
        void Awake()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// Initializes the container with a number of buttons, within the bounds of a recttransform, and brings it into view. Verifies if the item is hidden first.
        /// </summary>
        /// <param name="vStructuresList"></param>
        /// <param name="vNewScreenPos">The new position in screen. Verifies if this new position is within the bounds recttransform</param>
        public void Initialize(List<RightClickButtonStructure> vStructuresList, Vector2 vNewScreenPos)
        {
            if (vStructuresList == null)
            {
                return;
            }
            if (mIsHidden)
            {
                Vector3 vNewPos = Vector3.zero;
                //verify if within the bounds of the Bounds Rectransform
                if (WithinOutterRectBounds(vNewScreenPos, out vNewPos))
                { //Check the current count of the passed in collection again the number of buttons inside the mElements list. Remove if necessary
                    //flatten the z component of the vNewPos 
                    vNewPos.z = ButtonContainer.GetComponent<RectTransform>().position.z;
                    //set the new pos
                    ButtonContainerRectTransform.position = vNewPos;
                    if (vStructuresList.Count > mElements.Count)
                    {
                        //count the difference, instantiate a number of objects accordingly
                        int vDifference = vStructuresList.Count - mElements.Count;
                        while (vDifference != 0)
                        {
                            vDifference--;
                            GameObject vObj = GameObject.Instantiate(DefaultItemPrefab);
                            vObj.transform.SetParent(ButtonContainer.transform, false);
                            RightClickButtonElement vElem = vObj.GetComponent<RightClickButtonElement>();
                            mElements.Add(vElem);
                        }
                    }
                    // if there are less elements in the action list than the number of elements in the container, then prune the difference by setting them as inactive objects
                    else if (vStructuresList.Count < mElements.Count)
                    {
                        int vDifference = mElements.Count - vStructuresList.Count ;
                        //start from the last element in
                        int vIndex = mElements.Count - 1;
                        while (vDifference != 0)
                        {
                            mElements[vIndex].Hide();
                            mElements.RemoveAt(vIndex);
                            vDifference--;
                            vIndex--;

                        }
                    }

                    //both collections are equal in size, initialize the elements collection with variables found from the passed in collection
                    for (int i = 0; i < mElements.Count; i++)
                    {
                        mElements[i].Init(vStructuresList[i], Hide);
                    }
                    ButtonContainer.gameObject.SetActive(true);
                    mIsHidden = false;
                }

            }
            else
            {
                Vector3 vNewPos = Vector3.zero;
                //try to move item
                if (WithinOutterRectBounds(vNewScreenPos, out vNewPos))
                {
                    //Check the current count of the passed in collection again the number of buttons inside the mElements list. Remove if necessary
                    //flatten the z component of the vNewPos 
                    vNewPos.z = ButtonContainer.GetComponent<RectTransform>().position.z;
                    //set the new pos
                    ButtonContainerRectTransform.position = vNewPos;
                }
            }

        }


        /// <summary>
        /// Calculates the given screen position is within the bounds of the Bounds RecTransform
        /// </summary>
        /// <param name="vScreenPos"></param>
        /// <param name="vNewPos">returns the new position</param>
        /// <returns></returns>
        public bool WithinOutterRectBounds(Vector2 vScreenPos, out Vector3 vNewPos)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(Bounds, vScreenPos, UiCamera,
               out vNewPos);
            bool vReturn = RectTransformUtility.RectangleContainsScreenPoint(Bounds, vScreenPos, UiCamera);
            return vReturn;
        }

        /// <summary>
        /// Is within the bounds of the container rect
        /// </summary>
        /// <param name="vScreenPos"></param>
        /// <returns></returns>
        public bool WithinContainerBounds(Vector2 vScreenPos)
        {
            bool vReturn = RectTransformUtility.RectangleContainsScreenPoint(ButtonContainerRectTransform, vScreenPos, UiCamera);
           
            return vReturn;
        }
        /// <summary>
        /// Hides the container from view
        /// </summary>
        public void Hide()
        {
            mIsHidden = true; 
            ButtonContainer.gameObject.SetActive(false);
        }

        public void Show()
        {

        }

        /// <summary>
        /// Hides the container if out of bounds
        /// </summary>
        /// <param name="vScreenPos"></param>
        public void HideIfOutOfContainerBounds(Vector3 vScreenPos)
        {
            //cascading check
            if (!mIsHidden)
            { 
                if (!WithinContainerBounds(vScreenPos))
                {
                    Hide();
                }
            }
        }

        public void TestShow(Vector2 vScreenPos)
        {
            Vector3 vNewPos = Vector3.zero;
            //verify if within the bounds of the Bounds Rectransform
            if (WithinOutterRectBounds(vScreenPos, out vNewPos))
            { //Check the current count of the passed in collection again the number of buttons inside the mElements list. Remove if necessary
              //flatten the z component of the vNewPos 
                vNewPos.z = ButtonContainer.GetComponent<RectTransform>().position.z;
                //set the new pos
                ButtonContainerRectTransform.position = vNewPos;
                if (mElements.Count < 6)
                {
                    GameObject vObj = GameObject.Instantiate(DefaultItemPrefab);
                    vObj.transform.SetParent(ButtonContainer.transform, false);
                    RightClickButtonElement vElem = vObj.GetComponent<RightClickButtonElement>();
                    mElements.Add(vElem);
                }

                //both collections are equal in size, initialize the elements collection with variables found from the passed in collection
                for (int i = 0; i < mElements.Count; i++)
                {
                    RightClickButtonStructure vNew = new RightClickButtonStructure();
                    vNew.CallbackAction = () => Debug.Log("Clicked on");
                    vNew.Info = "item num " + i;
                    mElements[i].Init(vNew, Hide);
                }
                ButtonContainer.gameObject.SetActive(true);
                Debug.Log("here");
                mIsHidden = false;
            }
        }
    }
}