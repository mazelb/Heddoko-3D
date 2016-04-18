/** 
* @file PlaybackSettings.cs
* @brief Contains the PlaybackSettings class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/


namespace Assets.Scripts.Frames_Pipeline
{
    /**
    * PlaybackSettings class 
    * @brief PlaybackSettings class (represents the playback setting of the player in scene. ) 
    */

    public class PlaybackSettings
    {
        public float PlaybackSpeed;
        public float RewindSpeed;
        public int CurrentFramePos;
        public bool Loop { get; set; }
        public int StepForwardIncrement=1;
        public int StepBackwardsIncrements=1;
    }
}
