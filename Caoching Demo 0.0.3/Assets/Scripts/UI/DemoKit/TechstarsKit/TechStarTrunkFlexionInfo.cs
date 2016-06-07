/**
* @file TechStarTrunkFlexionInfo.cs
* @brief Contains the TechStarTrunkFlexionInfo class
* @author Mohammed Haider( Mohammed@heddoko.com)
* @date May 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.DemoKit.TechstarsKit
{
    public class TechStarTrunkFlexionInfo: MonoBehaviour
    {
        public Text AngleInfo; 

        public void UpdateFlexionText(float vAngle)
        {
            int vFormattedAngle = (int)vAngle;
            AngleInfo.text = "" + vFormattedAngle + "°";
        }
    }
}