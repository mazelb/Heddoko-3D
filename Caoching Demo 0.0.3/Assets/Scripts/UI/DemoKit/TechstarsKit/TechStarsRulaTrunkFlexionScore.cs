﻿// /**
// * @file TechStarsRulaTrunkFlexionScore.cs
// * @brief Contains the 
// * @author Mohammed Haider( Mohammed@heddoko.com)
// * @date may 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.UI.DemoKit.TechstarsKit
{
    public class TechStarsRulaTrunkFlexionScore : MonoBehaviour
    {
        

        public Text FlexionScoreText;
        public Text TwistScoreText;
        public Text TotalScoreText; 
        private int mFlexionScore;
        private int mTwistScore;
        private Color mOriginalColor;
        private Color mHedRed;
        private int mMaxFlexionScore = 0;
        private int mMaxTwistScore = 0;
        private int mMaxTotal = 0;
        private int mFlexionScoreSum;
        private int mTwistScoreSum;
        private int mTotalScoreSum;
        void Awake()
        {
            mOriginalColor = TwistScoreText.color;
            mHedRed = new Color(249f / 255, 69f / 255, 97f / 255f, 0.60f);
        }

        public void ResetScores()
        {
            TwistScoreText.color = mOriginalColor;
            FlexionScoreText.color = mOriginalColor;
            mFlexionScoreSum = 0;
            mTwistScoreSum = 0;
            mTotalScoreSum = mFlexionScoreSum + mTwistScoreSum; 

            mMaxFlexionScore = 0;
            mMaxTwistScore = 0;
            mMaxTotal = mFlexionScore + mTwistScore;
            FlexionScoreText.text = "+" + mMaxFlexionScore + "";
            TwistScoreText.text = "+" + mMaxTwistScore + "";
            TotalScoreText.text = "+" + mMaxTotal;
        }
        public void UpdateScore(int vFlexion, int vTwist)
        {
            mFlexionScoreSum += vFlexion;
            mTwistScoreSum += vTwist;
            mTotalScoreSum = mFlexionScoreSum + mTwistScoreSum;
         

            mFlexionScore = vFlexion;
            mTwistScore = vTwist;
            if (mFlexionScore > mMaxFlexionScore)
            {
                mMaxFlexionScore = mFlexionScore;
            }
            if (mTwistScore > mMaxTwistScore)
            {
                mMaxTwistScore = mTwistScore;
            }
            mMaxTotal = mMaxTwistScore + mMaxFlexionScore;

            FlexionScoreText.text = "+" + mMaxFlexionScore + "";
            TwistScoreText.text = "+" + mMaxTwistScore + "";
            TotalScoreText.text = "+" + mMaxTotal;
            if (mMaxTwistScore >= 1)
            {
                TwistScoreText.color = mHedRed;
            }
            if (mMaxFlexionScore >= 4)
            {
                FlexionScoreText.color = mHedRed;
            }

        }

    }
}