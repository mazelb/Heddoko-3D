
using System.Collections;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Brainpackwarning
{
    public class BrainpackWarning : MonoBehaviour
    {

        public Text WarningMessage;
        public Image WarningPanel;

        public float ShowTime;
        public float DissapearTime;
        private IEnumerator mImageRoutine;
        private IEnumerator mTextRoutine;
        

        void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            { // UI elements getting the hit/hover

            }
        }

        public void InPointer()
        {
            if (mImageRoutine != null)
            {
                StopCoroutine(mImageRoutine);
            }
            if (mTextRoutine != null)
            {
                StopCoroutine(mTextRoutine);
            }
            mImageRoutine = FaderUtility.FadeImage(WarningPanel, ShowTime, 1);
            mTextRoutine = FaderUtility.FadeText(WarningMessage, ShowTime, 1);
            StartCoroutine(mImageRoutine);
            StartCoroutine(mTextRoutine);

        }

        public void ExitPointer()
        {
            if (mImageRoutine != null)
            {
                StopCoroutine(mImageRoutine);
            }
            if (mTextRoutine != null)
            {
                StopCoroutine(mTextRoutine);
            }
            mImageRoutine = FaderUtility.FadeImage(WarningPanel, DissapearTime, 0);
            mTextRoutine = FaderUtility.FadeText(WarningMessage, ShowTime,0);
            StartCoroutine(mTextRoutine);
            StartCoroutine(mImageRoutine);
        }

    }
}
