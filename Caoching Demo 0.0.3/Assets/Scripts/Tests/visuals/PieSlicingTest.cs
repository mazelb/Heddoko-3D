/** 
* @file PieSlicingTest .cs
* @brief Contains the PieSlicingTest  and RulaPostureAngles classes
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Body_Data.View;
using Assets.Scripts.Body_Data.View.Anaylsis;
using UnityEngine;

namespace Assets.Scripts.Tests.visuals
{
    /// <summary>
    /// Testing the RulaVisualAngleAnalysis Component
    /// </summary>
    public class PieSlicingTest : MonoBehaviour
    {
        public AnaylsisFeedBackContainer.PosturePosition Posture= AnaylsisFeedBackContainer.PosturePosition.TrunkRotation;
        public Transform Trans;
        public Transform Comparer;
        public Vector3 TrasForward;
        public Vector3 TrasUp;
        public Vector3 TrasRight;
        public Vector3 ComparerForward;
        public Vector3 ComparerUp;
        public Vector3 ComparerRight;
        public Vector3 Cross;
        public RulaVisualAngleAnalysis Analysis;
        private bool mHasStarted;
        public RenderedBody RenderedBody;



        void Update()
        { 
            if (Input.GetKeyDown(KeyCode.T))
            {
                 
                    if (Analysis != null)
                    {
                        Analysis.Hide();
                    }
                    Analysis = RenderedBody.GetRulaVisualAngleAnalysis(Posture);
                
               
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                mHasStarted = false;
            }
            if(mHasStarted) 
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    Analysis.Animate();
                }
                if (Input.GetKeyDown(KeyCode.B))
                {
                    Analysis.FlipModes();
                }
               // Analysis.SpineAngleCoronalPlane();
            }
        }
    }
}