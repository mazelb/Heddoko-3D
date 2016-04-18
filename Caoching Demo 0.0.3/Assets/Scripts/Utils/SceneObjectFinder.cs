/** 
* @file SceneObjectFinder.cs
* @brief Contains the SceneObjectFinder class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/
 
using UnityEngine;
namespace Assets.Scripts.Utils
{
    /**
    * SceneObjectFinder class 
    * @brief Static Class that attempts to find objects in scenes with certain criteria
    * @note Note something of interest
    */
    public static class SceneObjectFinder<T> where T : Component
    {
        /// <summary>
        /// Returns the GameObject that contains a Monobehaviour T found in the current unity scene
        /// </summary>
        /// <param name="vTag">The tag of the object to look for</param>
        /// <returns>The found GameObject</returns>
        /**
        * FindObjecInSceneWithObjectAttached(string vTag)
        * @brief Returns the GameObject that contains a Monobehaviour T found in the current unity scene
        * @param string vTag : The tag of the object to look for
        * @note If the object cannot be found in the scene, then a new gameobject will be created with the T Monobehaviourattached
        * @return The found GameObject
        */
        public static GameObject FindObjecInSceneWithObjectAttached(string vTag)
        {
            GameObject foundObject = GameObject.FindGameObjectWithTag(vTag);
            if (foundObject == null)
            { 
                foundObject = new GameObject();
                foundObject.tag = vTag;
                foundObject.AddComponent<T>();
            }
            if (foundObject.GetComponent<T>() == null)
            {
                foundObject.AddComponent<T>();
            }
            return foundObject;
        }

        /// <summary>
        /// Attempts to find a transform in the scene by first checking the scene, then the immediate children and finally a deep search
        /// </summary>
        /// <param name="vParentObject">the parent object</param>
        /// <param name="vTargetToFind">The objects name to find</param>
        /// <returns>The found Component. Note, null can be returned if the Component is not found in the scene</returns>
        /**
        * FindObjectByName(Component vParentObject, string vTargetToFind)
        * @brief  Attempts to find a transform in the scene by first checking the scene, then the immediate children and finally a deep search
        * @param Component vParentObject: the parent object, string vTargetToFind: The objects name to find
        * @note  null can be returned if the Component is not found in the scene
        * @return The found Component
        */
        public static Component FindObjectByName(Component vParentObject, string vTargetToFind)
        {
            Transform vFoundTransform = vParentObject.transform.Find(vTargetToFind);
            if (vFoundTransform == null)
            {
                vFoundTransform = vParentObject.transform.FindChild(vTargetToFind);
                if (vFoundTransform == null)
                {
                    Transform[] transforms = vParentObject.GetComponentsInChildren<Transform>();
                    foreach (Transform t in transforms)
                    {
                        if (t.name == vTargetToFind)
                        {
                            vFoundTransform = t;
                            break;
                        }
                    }
                }
            }
            return vFoundTransform;
        }
    }
}
