using System.Collections;
using UnityEngine;
using UnityEngine.UI;

static internal class AnimationHelpers
{
    public static IEnumerator FadeTextBoxWithMessage(string vMsg, Text vText)
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