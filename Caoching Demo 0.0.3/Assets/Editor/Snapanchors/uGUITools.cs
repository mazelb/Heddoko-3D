/** 
* @file Joint.cs
* @brief Contains the Joint class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/

using UnityEditor;
using UnityEngine;

public class UGUITools : MonoBehaviour
{
    /**
   * AnchorsToCorners 
   * @brief Sets the anchors of the UI object to the corners  
   */
    [MenuItem("Window/AutoAnchors/Anchors to Corners %[")]
    static void AnchorsToCorners()
    {
        RectTransform vRectTransform = Selection.activeTransform as RectTransform;
        RectTransform vParentRectTransform = Selection.activeTransform.parent as RectTransform;

        if (vRectTransform == null || vParentRectTransform == null) return;

        Vector2 vNewAnchorMin = new Vector2(vRectTransform.anchorMin.x + vRectTransform.offsetMin.x / vParentRectTransform.rect.width,
                                            vRectTransform.anchorMin.y + vRectTransform.offsetMin.y / vParentRectTransform.rect.height);
        Vector2 vNewAnchorMax = new Vector2(vRectTransform.anchorMax.x + vRectTransform.offsetMax.x / vParentRectTransform.rect.width,
                                            vRectTransform.anchorMax.y + vRectTransform.offsetMax.y / vParentRectTransform.rect.height);

        vRectTransform.anchorMin = vNewAnchorMin;
        vRectTransform.anchorMax = vNewAnchorMax;
        vRectTransform.offsetMin = vRectTransform.offsetMax = new Vector2(0, 0);
    }
    /**
    * CornersToAnchors 
    * @brief Sets the corners of the Ui object according to its anchors 
    */
    [MenuItem("Window/AutoAnchors/Corners to Anchors %]")]
    static void CornersToAnchors()
    {
        RectTransform vRectTransform = Selection.activeTransform as RectTransform;

        if (vRectTransform == null) return;

        vRectTransform.offsetMin = vRectTransform.offsetMax = new Vector2(0, 0);
    }
}