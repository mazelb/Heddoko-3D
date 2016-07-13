/** 
* @file BodySubsegmentView.cs
* @brief The view for a body's subsegment. Application of transforms happen in here. 
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.UI._2DSkeleton;
using UnityEngine;
namespace Assets.Scripts.Body_Data.view
{
    /**
    * BodySubsgmentView  class 
    * @brief BodySubsegmentView class (represents one subsegment view)
    */
    public class BodySubsegmentView : MonoBehaviour
    {
        //the transforms required to transform
        public BodySubSegment AssociatedSubSegment;
        public Transform SubsegmentTransform;
        public List<Transform> SubSegmentTransforms = new List<Transform>();

        //Initial subsegment position 
        private Vector3 mInitialPosition = Vector3.zero;
        public Quaternion mInitialRotation;

        //Sprite Transform2D
        private ISpriteMover mSpriteMover;

        //This flag turned on, mean that the assigned Transform is a 3D model, else use a 2D sprite
        private bool mUsing3DModel = true;

        /**
        * View
        * @param 
        * @brief View associated with this body
        * @note: a new gameobject is created and this Body is added into it as a component
        * @return returns the view associated with this body
        */
        public void AssignTransforms()
        {
            //find the object in the scene with the tag
            GameObject[] vGameObjects = GameObject.FindGameObjectsWithTag(gameObject.name);

            //Initialize the view of each subsegments related object
            foreach (GameObject vObj in vGameObjects)
            {
                Transform vTransform = vObj.transform;
                SubSegmentTransforms.Add(vTransform);
                SubsegmentTransform = vTransform;
                //Todo: add proper setting for the initial position 
                mInitialPosition = vTransform.localPosition;
            }
            try
            {
                GameObject v2DGameObject = GameObject.FindGameObjectWithTag(gameObject.name + "2D");
                if (v2DGameObject != null)
                {
                    // SpriteTransform = v2DGameObject.transform;
                    mSpriteMover = v2DGameObject.GetComponent<ISpriteMover>();
                }
            }
            catch (UnityException exception)
            {
                //exception is thrown if the tag ins't found in the tag manager.
            }
            //Find the 2D representation of the object in the scene 
        }

        /// <summary>
        /// Assign a transform from the given associate transform
        /// </summary>
        /// <param name="vAssociatedTransform"></param>
        public void AssignTransforms(Transform vAssociatedTransform)
        {
            SubSegmentTransforms.Add(vAssociatedTransform);
            SubsegmentTransform = vAssociatedTransform;
            mInitialPosition = vAssociatedTransform.localPosition;

            try
            {
                GameObject v2DGameObject = GameObject.FindGameObjectWithTag(gameObject.name + "2D");
                if (v2DGameObject != null)
                {
                    // SpriteTransform = v2DGameObject.transform;
                    mSpriteMover = v2DGameObject.GetComponent<ISpriteMover>();
                }
            }
            catch (UnityException exception)
            {
                //exception is thrown if the tag ins't found in the tag manager.
            }
            //Find the 2D representation of the object in the scene 
        }

        public void ApplyTransformations(Quaternion vNewOrientation, int vApplyLocal = 0, bool vResetRotation = false)
        {
            foreach (Transform vObjTransform in SubSegmentTransforms)
            {
                if (vResetRotation)
                {
                    vObjTransform.localRotation = mInitialRotation;
                    vObjTransform.rotation = mInitialRotation;
                }

                switch (vApplyLocal)
                {
                    case 0:
                        vObjTransform.Rotate(vNewOrientation.eulerAngles, Space.Self);
                        break;
                    case 1:
                        vObjTransform.rotation = vNewOrientation;
                        break;
                    case 2:
                        vObjTransform.localRotation = vNewOrientation;
                        break;
                    case 3:
                        vObjTransform.Rotate(vNewOrientation.eulerAngles, Space.World);
                        break;
                }
            }
            if (mSpriteMover != null)
            {
                mSpriteMover.ApplyTransformations();
            }
        }


        public void ApplyTranslations(Vector3 vNewDisplacement)
        {
            foreach (Transform vObjTransform in SubSegmentTransforms)
            {
                //Debug.Log(vNewDisplacement - vObjTransform.position);
                vObjTransform.position = vNewDisplacement;
            }

            if (mSpriteMover != null)
            {
                //mSpriteMover.ApplyTranslations(vNewDisplacement);
            }
        }

        public void ResetOrientations()
        {
            foreach (Transform vObjTransform in SubSegmentTransforms)
            {
                vObjTransform.localRotation = mInitialRotation;
                vObjTransform.rotation = mInitialRotation;
            }

            if (mSpriteMover != null)
            {
                mSpriteMover.ResetOrientations();
            }
            //if (Camera.main != null && Camera.main.gameObject.activeInHierarchy)
            //{
            //    Camera.main.Render();
            //}
        }

        public void ResetPositions()
        {
            foreach (Transform vObjTransform in SubSegmentTransforms)
            {
                vObjTransform.localPosition = mInitialPosition;
                //vObjTransform.position = mInitialPosition;
            }

            //if (Camera.main != null)
            //{
            //    Camera.main.Render();
            //}
        }

        /**
        * UpdateOrientation(Quaternion vNewOrientation)
        * @param Quaternion vNewOrientation: the new orientation of the subsegment
        * @brief Updates the current orientation with the passed in parameter
        */
        internal void UpdateOrientation(Quaternion vNewOrientation, int vApplyLocal = 0, bool vResetRotation = false)
        {
            try
            {
                ApplyTransformations(vNewOrientation, vApplyLocal, vResetRotation);
            }
            catch (Exception)
            {

            }
        }

        /**
        * UpdateOrientation(Quaternion vNewOrientation)
        * @param Quaternion vNewOrientation: the new orientation of the subsegment
        * @brief Updates the current orientation with the passed in parameter
        */
        internal void UpdatePosition(Vector3 vNewDisplacemetn)
        {
            try
            {
                ApplyTranslations(vNewDisplacemetn);
            }
            catch (Exception)
            {

            }
        }

        /**
        * ResetOrientation()
        * @brief Resets the current orientation to its initial value
        */
        internal void ResetTransforms()
        {
            ResetOrientations();
            ResetPositions();
        }

        /**
        * Awake()
        * @brief On Awake: set the segment's initial orientation .
        */
        internal void Awake()
        {
            AssignTransforms();
        }

        internal void Start()
        {
            foreach (Transform vObjTransform in SubSegmentTransforms)
            {
                //mInitialRotation = new Quaternion(vObjTransform.localRotation.x, vObjTransform.localRotation.y, vObjTransform.localRotation.z, vObjTransform.localRotation.w);
                vObjTransform.rotation = Quaternion.identity;
                vObjTransform.localRotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// clears out transforms 
        /// </summary>
        internal void Clear()
        {
            SubsegmentTransform = null;
            SubSegmentTransforms = new List<Transform>();
        }
    }
}