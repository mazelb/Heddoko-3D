
/**  
* @file CameraController.cs 
* @brief Contains the CamRotate class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/


using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI._2DSkeleton
{
    /// <summary>
    /// Handles switching between different ISpriteMovers
    /// </summary>
    public class LegMoveSwitcher : MonoBehaviour
    {
        public LegSpriteMover[] SpritesGo;
        [SerializeField]
        private int mCurrentSpriteIndex = 0;
        public int CurrentSpriteIndex { get { return mCurrentSpriteIndex;} }


        //disable all sprites except for the first one
        void Awake()
        {
            for (int i = 1; i < SpritesGo.Length; i++)
            {

                SpritesGo[i].SetActive(false);
            }
        }
       

        public void Show()
        {
           
            for (int i = 0; i < SpritesGo.Length; i++)
            {
                SpritesGo[i].SetActive(false);
            }
            TurnOnSprite(mCurrentSpriteIndex);
        }

      public void Hide()
        {
            
        }
        void Update()
        {
            InputHandler();
        }

        private void InputHandler()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                TurnOnSprite(mCurrentSpriteIndex - 1);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                TurnOnSprite(mCurrentSpriteIndex + 1);
            }
        }

        /// <summary>
        /// turns on the sprite according to the index passed. 
        /// </summary>
        /// <param name="vIndex"></param>
        public void TurnOnSprite(int vIndex)
        {
            if (vIndex < 0)
            {
                vIndex = 0;
            }

            else if (vIndex >= SpritesGo.Length)
            {
                vIndex = SpritesGo.Length - 1;
            }

            else if (vIndex == -1)
            {
                return;
            }


            SpritesGo[mCurrentSpriteIndex].SetActive(false);
            SpritesGo[vIndex].SetActive(true);
            mCurrentSpriteIndex = vIndex;
        }
    }
}
