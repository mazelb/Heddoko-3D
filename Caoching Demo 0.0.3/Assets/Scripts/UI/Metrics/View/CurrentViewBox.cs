
/**  
* @file CurrentViewBox.cs 
* @brief Contains the CurrentViewBox class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/

using Assets.Scripts.UI.Metrics.View;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics
{
    /// <summary>
    /// Displays the information on the current limbs/segments
    /// </summary>
   public class CurrentViewBox : MonoBehaviour
   {
       public Text SideBoxText;
       public Text AnalysingText;
        public BikingMetricsView BikingMetrics; 
        public LegMetricsView LegMetrics;
        public ArmMetricsView ArmMetrics;
        public void UpdateText(bool vIn2DView, int vNewPos)
        {
            if (vIn2DView)
            {
                SideBoxText.text = "SIDE VIEW";
                if (vNewPos == 0)
                {
                    AnalysingText.text = "LEFT LEG";
                    if (BikingMetrics != null)
                    { BikingMetrics.DisplayRightLegAnalysis = false; }

                    LegMetrics.DisplayRightLegAnalysis = false;
                    if (ArmMetrics != null)
                    {
                        ArmMetrics.DisplayRightArmAnalysis = false;
                    }
                }
                else
                {
                    AnalysingText.text = "RIGHT LEG";
                    if (BikingMetrics != null)
                    {
                        BikingMetrics.DisplayRightLegAnalysis = true;
                    }
                    LegMetrics.DisplayRightLegAnalysis = true;
                    if (ArmMetrics != null)
                    {
                        ArmMetrics.DisplayRightArmAnalysis = true;
                    }
                }
            }
            else
            {
                if (vNewPos == 0)
                {
                    SideBoxText.text = "FRONT VIEW"; 
                }
                if (vNewPos == 1)
                {
                    SideBoxText.text = "LEFT VIEW";
                    AnalysingText.text = "";
                    if (BikingMetrics != null)
                    {
                        BikingMetrics.DisplayRightLegAnalysis = false;
                    }
                    LegMetrics.DisplayRightLegAnalysis = false;
                    if (ArmMetrics != null)
                    {
                        ArmMetrics.DisplayRightArmAnalysis = false;
                    }
                }
                if (vNewPos == 2)
                {
                    SideBoxText.text = "BACK VIEW";
                  
                }
                if (vNewPos == 3)
                {
                    SideBoxText.text = "RIGHT VIEW";
                    AnalysingText.text = "";
                    if (BikingMetrics != null)
                    {
                        BikingMetrics.DisplayRightLegAnalysis = true;
                    }
                    LegMetrics.DisplayRightLegAnalysis = true;
                    if (ArmMetrics != null)
                    {
                        ArmMetrics.DisplayRightArmAnalysis = true;
                    }
                }
            }
        }

   }
}
