using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Body_Data.View
{
    public delegate void SegmentHeldDown();

    public delegate void SegmentPressed(Transform vTransform);

    public delegate void SegmentReleased();
    public class SelectableSegment:MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private float mTimePressed;
        private float mTimeTrigger = 1.5f;
        private bool mPressed;
        private bool mSegmentHeldDownTriggered = false;
        public event SegmentHeldDown SegmentHeldDownEvent;
        public event SegmentPressed SegmentPressedEvent;
        public void OnPointerDown(PointerEventData eventData)
        {
            mPressed = true; 
            if (SegmentPressedEvent != null)
            {
                SegmentPressedEvent(transform);
            }
        }

     

        public void OnPointerExit(PointerEventData eventData)
        {
            mPressed = false;
            mTimePressed = 0;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            mPressed = false;
            mTimePressed = 0;
        }

        private void Update()
        {
            if (mPressed)
            {
                mTimePressed += Time.deltaTime;
                if (mTimePressed > mTimeTrigger)
                {
                    if (!mSegmentHeldDownTriggered)
                    {
                        
                        if (SegmentHeldDownEvent != null)
                        {
                            SegmentHeldDownEvent();
                        }
                        mSegmentHeldDownTriggered = true;
                    }
                }
            }
            else
            {
                mTimePressed = 0;
                mSegmentHeldDownTriggered = false;
            }
        }
    }
}