/** 
* @file FaderUtility.cs
* @brief Contains the FaderUtility class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils
{
    /// <summary>
    /// Static class with functions that fade/unfade text or images 
    /// </summary>
    /**
    * FaderUtility class 
    * @brief Static class with functions that fade/unfade text or images  
    */
    public static class FaderUtility
    {
        /// <summary>
        /// FadeImage(Image vTargetImage, float vDuration, float vTargetOpacity)
        /// </summary>
        /// <param name="vTargetImage">The target image</param>
        /// <param name="vDuration">The duration of the effect</param>
        /// <param name="vTargetOpacity">The target opacity</param>
        /// <returns>Next frame in the Enumerator</returns>
        /**
        * FunctionName(object args)
        * @brief Fades the target image to the target opacity
        * @param Image vTargetImage: The target image, float vDuration: The duration of the effect,float vTargetOpacity: The target opacity
        * @return Next frame in the Enumerator
        */
        public static IEnumerator FadeImage(Image vTargetImage, float vDuration, float vTargetOpacity)
        {
            if (vTargetImage == null)
            { 
                yield break;
            }
            float alpha = vTargetImage.color.a;

            for (float i = 0; i < 1.0f; i += Time.deltaTime / vDuration)
            { 
                Color newColor = vTargetImage.color;
                newColor.a = Mathf.SmoothStep(alpha, vTargetOpacity, i);
                vTargetImage.color = newColor;
                yield return null;
            }
        }

        /// <summary>
        /// fades the MaskableGraphic object to the target opacity
        /// </summary>
        /// <param name="vMaskableGraphicObject">The target MaskableGraphic object</param>
        /// <param name="vDuration">The duration of the effect</param>
        /// <param name="vTargetOpacity">The target opacity</param>
        /// <returns>Next frame in the Enumerator</returns>
        /**
        * FadeUiObject(MaskableGraphic vMaskableGraphicObject, float vDuration, float vTargetOpacity)
        * @brief Fades the target image to the target opacity
        * @param Image vMaskableGraphicObject: The target  MaskableGraphic object, float vDuration: The duration of the effect,float vTargetOpacity: The target opacity
        * @return Next frame in the Enumerator
        */
        public static IEnumerator FadeUiObject(MaskableGraphic vMaskableGraphicObject, float vDuration, float vTargetOpacity)
        {
            if (vMaskableGraphicObject == null)
            {
                yield break;
            }
            float alpha = vMaskableGraphicObject.color.a;

            for (float i = 0; i < 1.0f; i += Time.deltaTime / vDuration)
            { 
                Color newColor = vMaskableGraphicObject.color;
                newColor.a = Mathf.SmoothStep(alpha, vTargetOpacity, i);
                vMaskableGraphicObject.color = newColor;
                yield return null;
            }
        }



        /// <summary>
        /// Fades the text to the target opacity
        /// </summary>
        /// <param name="vTargetText">The targetted text</param>
        /// <param name="vDuration">The duration of the effect</param>
        /// <param name="vTargetOpacity">The target opacity</param>
        /// <returns>Next frame in the Enumerator</returns>
        /**
        * FadeText(Text vTargetText, float vDuration, float vTargetOpacity)
        * @brief Fades the text to the target opacity
        * @param Text vTargetText: The targetted text  , float vDuration: The duration of the effect,float vTargetOpacity: The target opacity
        * @return Next frame in the Enumerator
        */
        public static IEnumerator FadeText(Text vTargetText, float vDuration, float vTargetOpacity)
        {
            if (vTargetText == null)
            {
                yield break;
            }
            float alpha = vTargetText.color.a;

            for (float i = 0; i < 1.0f; i += Time.deltaTime / vDuration)
            { 
                Color newColor = vTargetText.color;
                newColor.a = Mathf.SmoothStep(alpha, vTargetOpacity, i);
                vTargetText.color = newColor;
                yield return null;
            }
        }

        /// <summary>
        /// Fades text, then removes its contents. Immediately set the opacity to previous level
        /// </summary>
        /// <param name="vTargetText">The targetted text</param>
        /// <param name="vDuration">The duration of the effect</param> 
        /// <returns>Next frame in the Enumerator</returns>
        /**
        * FadeTextThenRemoveText(Text vTargetText, float vDuration)
        * @brief Fades text, then removes its contents. Immediately set the opacity to previous level
        * @param Text vTargetText: The targetted text  , float vDuration: The duration of the effect,float vTargetOpacity: The target opacity
        * @return Next frame in the Enumerator
        */
        public static IEnumerator FadeTextThenRemoveText(Text vTargetText, float vDuration )
        {
             
            if (vTargetText == null)
            {
                yield break;
            }
            float alpha = vTargetText.color.a;

            for (float i = 0; i < 1.0f; i += Time.deltaTime / vDuration)
            {
                Color vNewColor = vTargetText.color;
                vNewColor.a = Mathf.SmoothStep(alpha, 0, i);
                vTargetText.color = vNewColor;
                yield return null;
            }
            vTargetText.text = "";
            Color vSetColor = vTargetText.color;
            vSetColor.a = 1;
            vTargetText.color =vSetColor;
        }
        /// <summary>
        /// Disables a gameobject after a set amount of time
        /// </summary>
        /// <param name="vGo">The GameObject to disable</param>
        /// <param name="vSeconds">The time before the object is disabled</param>
        /// <returns>Next frame in the Enumerator</returns>
        /**
        * DisableGameObjectAfterSeconds(GameObject vGo, float vDuration)
        * @brief Disables a gameobject after a set amount of time
        * @param GameObject vGo: The GameObject to disablet  , float vSecond: The time before the object is disabled
        * @return Next frame in the Enumerator
        */
        public static IEnumerator DisableGameObjectAfterSeconds(GameObject vGo, float vSeconds)
        {
            yield return new WaitForSeconds(vSeconds);
            vGo.SetActive(false);
        }

        /// <summary>
        /// Enables gameobject after a set amount of time. Checks what type of object the object passed in is
        /// </summary>
        /// <param name="vObj">a single , array or list of gameobjects</param>
        /// <param name="vDuration">The duration before the object is disabled </param>
        /// <returns>Next frame in the Enumerator</returns>
        /**
        *  EnableGameObjectsAfterNSeconds(object vObj, float vDuration)
        * @brief Disables a gameobject after a set amount of time
        * @param GameObject object vObj: a single , array or list of gameobjects , float vDuration:  The duration before the object is disabled
        * @return Next frame in the Enumerator
        */
        public static IEnumerator EnableGameObjectsAfterNSeconds(object vObj, float vDuration)
        {
            yield return new WaitForSeconds(vDuration);

            if (vObj is GameObject)
            {
                GameObject gameObject = (GameObject)vObj;
                gameObject.SetActive(true);
            }
            else if (vObj is Array)
            {
                GameObject[] gameObjects = (GameObject[])vObj;
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    gameObjects[i].SetActive(true);
                }
            }
            else if (vObj is IList)
            {

                List<GameObject> gameObjects = (List<GameObject>)vObj;
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    gameObjects[i].SetActive(true);
                }
            }
        }
    }
}
