/** 
* @file SquatCounter.cs
* @brief Contains the SquatCounter class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics
{
    /// <summary>
    /// Shows the numbers of squats in the Squats view
    /// </summary>
    public class SquatCounter : MonoBehaviour
    {
        public Sprite[] Numbers;
        public Image LeftDigit;
        public Image RightDigit;

        /// <summary>
        /// Sets the new number of squats
        /// </summary>
        /// <param name="vNewVal"></param>
        public void SetSquatNumber(int vNewVal)
        {
            int vLeftNumber = 0;
            int vRightNumber = 0;
            if (vNewVal > 99)
            {
                vLeftNumber = 9;
                vRightNumber = 9;
            }
            else
            {
                vLeftNumber = vNewVal / 10;
                vRightNumber = vNewVal - (10 * vLeftNumber);
            }

            LeftDigit.sprite = Numbers[vLeftNumber];
            RightDigit.sprite = Numbers[vRightNumber];
        }
    }
}
