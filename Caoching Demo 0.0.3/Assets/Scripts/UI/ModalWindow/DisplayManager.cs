using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace Assets.Scripts.UI.MainScene
{
    /// <summary>
    /// Display manager for the modal window
    /// 
    /// </summary>
   public  class DisplayManager : MonoBehaviour
    {
        public Text mDisplayText;
        public float mDisplayTime;
        public float mFadeTime;

        private IEnumerator mFadeAlpha;

        private static DisplayManager mDisplayManager;

        public static DisplayManager Instance()
        {
            if (!mDisplayManager)
            {
                mDisplayManager = FindObjectOfType(typeof(DisplayManager)) as DisplayManager;
                if (!mDisplayManager)
                    Debug.LogError("There needs to be one active DisplayManager script on a GameObject in your scene.");
            }

            return mDisplayManager;
        }

        public void DisplayMessage(string message)
        {
            mDisplayText.text = message;
            SetAlpha();
        }

        void SetAlpha()
        {
            if (mFadeAlpha != null)
            {
                StopCoroutine(mFadeAlpha);
            }
            mFadeAlpha = FadeAlpha();
            StartCoroutine(mFadeAlpha);
        }

        IEnumerator FadeAlpha()
        {
            Color resetColor = mDisplayText.color;
            resetColor.a = 1;
            mDisplayText.color = resetColor;

            yield return new WaitForSeconds(mDisplayTime);

            while (mDisplayText.color.a > 0)
            {
                Color displayColor = mDisplayText.color;
                displayColor.a -= Time.deltaTime / mFadeTime;
                mDisplayText.color = displayColor;
                yield return null;
            }
            yield return null;
        }
    }
}
