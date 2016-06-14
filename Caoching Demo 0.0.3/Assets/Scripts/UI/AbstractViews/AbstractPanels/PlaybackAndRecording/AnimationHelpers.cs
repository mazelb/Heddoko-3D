using System.Collections;
using UnityEngine;
using UnityEngine.UI;

static internal class AnimationHelpers
{
    /// <summary>
    /// Fades a text box with a message
    /// </summary>
    /// <param name="vMsg">The message</param>
    /// <param name="vText">The text box</param>
    /// <param name="vFadingTime">(optional)Fading time. Default to 4 seconds.</param>
    /// <returns></returns>
    public static IEnumerator FadeTextBoxWithMessage(string vMsg, Text vText, float vFadingTime = 4f)
    {
        vText.text = vMsg;
        if (!vText.enabled)
        {
            vText.enabled = true;
        }
        Color vTextColor = vText.color;
        vTextColor.a = 1;
        vText.color = vTextColor;
        var vPercentage = 0f;
        var vStartTime = Time.time; 
        //Start reducing the text's alpha
        while (vPercentage <=1f)
        {
            vPercentage = (Time.time  - vStartTime) /4f;
            vTextColor.a = Mathf.Lerp(1, 0, vPercentage);
            vText.color = vTextColor;
            yield return null;
        }
    }
}