/** 
* @file InformationPanel.cs
* @brief Contains the Joint InformationPanel
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/**
* Body class 
* @brief InformationPanel class, an in scene information panel
* @note something
*/
/// <summary>
///  InformationPanel class, an in scene information panel
/// </summary>
public class InformationPanel : MonoBehaviour
{

    private bool mFrozenText;
    public Text mDisplayText;
    /// <summary>
    /// The Text component of the gameobject
    /// </summary>
    private Text DisplayText
    {
        get
        {
            if (mDisplayText == null)
            {
                mDisplayText = GetComponent<Text>();
            }
            return mDisplayText;
        }
    }
    /**
    * UpdateInformationPanel(string mMessage)
    * @param string mMessage: The message to display
    * @brief  Updates the information panel 
    */
    /// <summary>
    /// Updates the information panel
    /// </summary>
    /// <param name="mMessage">The message to display</param>
    public void UpdateInformationPanel(string mMessage)
    {
        if (!mFrozenText)
        {
            DisplayText.text = mMessage;
        }
    }
    /**
    * FreezeText(float vDuration)
    * @brief IEnumerator used to freeze the text of the information panel for a set duration of time. 
    * @param float vDuration: the duration (in seconds) of how long the freezing  
    **/
    /// <summary>
    /// Freeze the text of the information panel for a set duration of time. 
    /// </summary>
    /// <param name="vDuration">  the duration (in seconds) of how long the freezing </param>
    /// <returns></returns>
    public void FreezeText(float vDuration)
    {
        StartCoroutine(FreezeTxt(vDuration));
    }
    /**
    * FreezeTxt(float vDuration)
    * @brief IEnumerator used to freeze the text of the information panel for a set duration of time. 
    * @param float vDuration: the duration (in seconds) of how long the freezing  
    **/
    /// <summary>
    /// IEnumerator used to freeze the text of the information panel for a set duration of time. 
    /// </summary>
    /// <param name="vDuration">  the duration (in seconds) of how long the freezing </param>
    /// <returns></returns>
    private IEnumerator FreezeTxt(float vDuration)
    {
        mFrozenText = true;
        float vTimer = vDuration;
        while (true)
        {
            vTimer -= Time.deltaTime;
            if (vTimer < 0)
            {
                break;
            }
            yield return null;
        }
        mFrozenText = false;
    }
}
