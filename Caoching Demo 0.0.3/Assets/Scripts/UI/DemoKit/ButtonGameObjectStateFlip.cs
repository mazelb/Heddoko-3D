// /**
// * @file ButtonGameObjectStateFlip.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 11 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.CodeDom;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.DemoKit
{
    [RequireComponent(typeof(Button))]
    public class ButtonGameObjectStateFlip : MonoBehaviour
    {
        public Button Button;
        private bool mIsActive=false;
        public GameObject GameObjectToStateChange;
        void Awake()
        {
            Button = GetComponent<Button>();
            Button.onClick.AddListener(() =>
            {
                mIsActive = !mIsActive;
                GameObjectToStateChange.SetActive(mIsActive);
            });    
        }
    }
}