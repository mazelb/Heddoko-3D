using System.Collections.Generic;

namespace Assets.Scripts.Body_Data.View.Anaylsis
{
    /// <summary>
    /// A global field of the RULA posture angles
    /// </summary>
    public class RulaPostureAngles
    {

        private List<RulaPointStructure> mRulaAngleRanges = new List<RulaPointStructure>();
        private AnaylsisFeedBackContainer.PosturePosition mPosturePosition;

        public AnaylsisFeedBackContainer.PosturePosition PosturePosition
        {
            get { return mPosturePosition; }
            set
            {
                mPosturePosition = value;
                RulaAngleRanges = RulaSettings.PointStructureList(mPosturePosition);
            }
        }

        /// <summary>
        /// List of Rula angle ranges
        /// </summary>
        public List<RulaPointStructure> RulaAngleRanges
        {
            get { return mRulaAngleRanges; }
            private set { mRulaAngleRanges = value; }
        }





    }
}