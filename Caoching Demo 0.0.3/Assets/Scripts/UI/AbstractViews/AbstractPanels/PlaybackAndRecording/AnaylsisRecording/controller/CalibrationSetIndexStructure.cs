// /**
// * @file CalibrationSetIndexStructure.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date September 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */
namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording.AnaylsisRecording
{
    public class CalibrationSetIndexStructure
    {
        private int mStartIndex;
        private int mSelectedIndex;
        private int mEndIndex;


        public int StartIndex
        {
            get { return mStartIndex; }
            set { mStartIndex = value; }
        }

        public int SelectedIndex
        {
            get { return mSelectedIndex; }
            set { mSelectedIndex = value; }
        }

        public int EndIndex
        {
            get { return mEndIndex; }
            set { mEndIndex = value; }
        }
    }
}