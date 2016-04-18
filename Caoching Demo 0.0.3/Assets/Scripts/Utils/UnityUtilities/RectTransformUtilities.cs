using UnityEngine;

namespace Assets.Scripts.Utils.UnityUtilities
{
    /// <summary>
    /// RectTransformUtilities: class that provides utilities to rectransforms
    /// </summary>
    public  static class RectTransformUtilities
    {
        /// <summary>
        /// Copies a RectTransforms properties.
        /// </summary>
        /// <param name="vFrom">The original RectTransform</param>
        /// <param name="vTo">The RectTransform to copy from</param>
        public static void CopyRectTransformProperties(RectTransform vFrom, RectTransform vTo)
        {

            vFrom.anchorMax = vTo.anchorMax;
            vFrom.anchorMin = vTo.anchorMin;
            vFrom.anchoredPosition = vTo.anchorMin;
            vFrom.anchoredPosition3D = vTo.anchoredPosition;
            vFrom.offsetMax = vTo.offsetMax;
            vFrom.offsetMin = vTo.offsetMin;
            vFrom.pivot = vTo.pivot;
            vFrom.position = vFrom.position;
            vFrom.localScale = vFrom.localScale;
            vFrom.localEulerAngles = vFrom.localEulerAngles;
        }
    }
}
