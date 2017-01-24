// /**
// * @file RecordingErrorHandler.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using Assets.Scripts.Frames_Recorder.FramesRecording;

namespace Assets.Scripts.ErrorHandling.Model
{
    public class RecordingErrorHandler 
    {
         private Action<BodyFramesRecordingBase> mAction;
        public RecordingErrorHandler( Action<BodyFramesRecordingBase> vAction)
        {
            mAction = vAction;
        }
        public void Notify(BodyFramesRecordingBase vRecordingBase)
        {
            mAction(vRecordingBase);
        }
    }
}