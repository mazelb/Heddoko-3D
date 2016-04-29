using System;
using System.Collections;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class SlideRecordingTest : MonoBehaviour
    {
        public RectTransform Container;
        public Vector2 vUpperRight;
        public Vector2 vUpperLeft;
        public Vector2 vBottomleft;
        private float mTime =1;
        public Rect NewRect;
         void Start()
        {
            UniFileBrowser.use.allowMultiSelect = false;
            UniFileBrowser.use.allowWindowDrag = false;
            UniFileBrowser.use.allowWindowResize = false;

            UniFileBrowser.use.SendWindowCloseMessage(Hide);
        }


        public SlideBlock SliderBlock;
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Q))
            {
                UniFileBrowser.use.OpenFileWindow(Callback);
                SliderBlock.Toggle();
                

            }
               
            
            try
            {
                SetBrowserRectTransform();
            }
            catch  
            {
                
                
            }
        }

        void DoNothing(string vBoo)
        {
            
        }
        void SetBrowserRectTransform()
        {
            Vector3[] vWorldCorners = new Vector3[4];
         Container.GetWorldCorners(vWorldCorners);
            
            vUpperLeft = Camera.main.WorldToScreenPoint(vWorldCorners[1]);
            vUpperRight = Camera.main.WorldToScreenPoint(vWorldCorners[2]);
            vBottomleft = Camera.main.WorldToScreenPoint(vWorldCorners[0]);
            NewRect =  new Rect(vUpperLeft.x,Screen.height - vUpperLeft.y, vUpperRight.x - vUpperLeft.x, vUpperRight.y - vBottomleft.y);
            UniFileBrowser.use.fileWindowRect = NewRect;
            UniFileBrowser.use.UpdateRects();
        }

        void Callback(string vpath)
        {

           
            UniFileBrowser.use.OpenFileWindow(DoNothing);
            StartCoroutine(Corut());
        }

        void Hide()
        {
           
            UniFileBrowser.use.OpenFileWindow(DoNothing);
            SetBrowserRectTransform();
            SliderBlock.Toggle();
            StartCoroutine(Corut());
        }

        IEnumerator Corut()
        {
            yield return new WaitForSeconds(1f);
            UniFileBrowser.use.CloseFileWindow();
        }
    }
}