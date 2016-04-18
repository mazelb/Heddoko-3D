
/** 
* @file BodySegmentOptionSetter.cs
* @brief Contains the BodySegmentOptionSetter class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Assets.Scripts.Body_Data.View; 
using Assets.Scripts.Utils.DebugContext;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Demos
{
    /// <summary>
    /// Enables and disables BodySegment options, options are set through reflection
    /// </summary>
    public class BodySegmentOptionSetter : MonoBehaviour
    {
        public Button ResetButton;
        public GameObject DebuggingPanel;
        //prefab of a button
        public GameObject BodySegmentOptionButtonPrefab;
        private Dictionary<string, FieldInfo> mButtonMappings = new Dictionary<string, FieldInfo>();
        public RectTransform ParentPanel;
      
        void Awake()
        {
            //Get all the fields marked as public and static 
            mButtonMappings =
                typeof(BodySegment).GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(vf => vf.FieldType == typeof(bool))
                    .ToDictionary(vf => vf.Name);

            var vKeybindings = typeof(HeddokoDebugKeyMappings).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(vf => vf.FieldType == typeof(KeyCode)).ToDictionary(vf => vf.Name);

            var vQuery = from vKeyValuePair1 in mButtonMappings
                         join vKeyValuePair2 in vKeybindings on vKeyValuePair1.Key equals vKeyValuePair2.Key
                         select new
                         {
                             vKey = vKeyValuePair1.Key,
                             value1 = vKeyValuePair1.Value,
                             value2 = vKeyValuePair2.Value
                         };

            var vMappedDictionary = vQuery.ToDictionary(vf => vf.vKey, vf => vf.value2);
            foreach (var vKVPair in mButtonMappings)
            {
                try
                {
                    GameObject vButton = Instantiate(BodySegmentOptionButtonPrefab);
                    vButton.transform.SetParent(ParentPanel.transform, false);
                    string vTextualInfo = SplitCamelCase(vKVPair.Key);
                    BodySegmentOptionButton vButtonBSegment = vButton.GetComponent<BodySegmentOptionButton>();
                    vButtonBSegment.TextualInfo.text = vTextualInfo;

                    bool vVal = (bool)vKVPair.Value.GetValue(null);
                    vButtonBSegment.OnOffText.text = ReturnOnOffFromBool((vVal));

                    KeyValuePair<string, FieldInfo> vNewKvPair = new KeyValuePair<string, FieldInfo>(vKVPair.Key, vKVPair.Value);
                    UnityAction vAction = () => SetValue(vNewKvPair, vButtonBSegment);
                    KeyCode vKeyCode = (KeyCode)(vMappedDictionary[vKVPair.Key].GetValue(null));
                    InputHandler.RegisterKeyboardAction(vKeyCode, vAction);
                    vButtonBSegment.AssociatedButton.onClick.AddListener(vAction);
                }   
                catch(KeyNotFoundException)
                {
                    Debug.Log("option "+  vKVPair.Key + " in BodySegment.cs not found in HeddokoDebugKeyMappings. Make sure that you have this  mapped in HeddokoDebugKeyMappings");
                }
            }

            ResetButton.onClick.AddListener(ResetBodiesMetrics);

            InputHandler.RegisterKeyboardAction(KeyCode.H,
                () =>
                {
                    bool vIsActive = DebuggingPanel.gameObject.activeSelf;
                    DebuggingPanel.gameObject.SetActive(!vIsActive);
                });

        }

        /// <summary>
        /// Reset the bodies metrics
        /// </summary>
        private void ResetBodiesMetrics()
        {
            foreach (var vRenderedBody in RenderedBodyPool.sInUsePool)
            {
                try
                {
                    vRenderedBody.AssociatedBodyView.AssociatedBody.ResetBodyMetrics();
                }
                catch (NullReferenceException  )
                {
                   Debug.Log("Following Rendered body hasn't had a body/view assigned:  "+vRenderedBody.name);
                }
               
            }
        }
        /// <summary>
        /// Returns on/off based on true/false
        /// </summary>
        /// <param name="vValue"></param>
        /// <returns></returns>
        public static string ReturnOnOffFromBool(bool vValue)
        {
            return vValue ? "ON" : "OFF";
        }

        /// <summary>
        /// Splits camel case strings
        /// </summary>
        /// <param name="vValue"></param>
        /// <returns></returns>
        public static string SplitCamelCase(string vStr)
        {
            return Regex.Replace(Regex.Replace(vStr, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"),
                                @"(\p{Ll})(\P{Ll})", "$1 $2");
        }


        void SetValue(KeyValuePair<string, FieldInfo> vKVPair, BodySegmentOptionButton vButtonBSegment)
        {
            bool vCurrVal = (bool)vKVPair.Value.GetValue(null);
            bool vNewVal = !vCurrVal;
            vButtonBSegment.OnOffText.text = ReturnOnOffFromBool(vNewVal);
            vKVPair.Value.SetValue(null, vNewVal);
        }


    }

}
