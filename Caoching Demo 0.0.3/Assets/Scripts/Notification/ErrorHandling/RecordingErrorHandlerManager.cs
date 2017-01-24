// /**
// * @file RecordingErrorHandlerManager.cs
// * @brief Contains the 
// * @author Mohammed Haider( mohammed@hw
// * @date 10 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
using Assets.Scripts.ErrorHandling.Model;
using Assets.Scripts.Frames_Recorder.FramesRecording;

namespace Assets.Scripts.ErrorHandling
{
    public class RecordingErrorHandlerManager  
    {

        private Dictionary<string, List<RecordingErrorHandler>> mHandler = new Dictionary<string, List<RecordingErrorHandler>>();
        private static RecordingErrorHandlerManager sRecordingErrorHandlerManager;

        public static RecordingErrorHandlerManager Instance
        {
            get
            {
                if (sRecordingErrorHandlerManager == null)
                {
                    sRecordingErrorHandlerManager = new RecordingErrorHandlerManager();
                }
                return sRecordingErrorHandlerManager;
            }
        }

        public void AddErrorHandler(string vKey, RecordingErrorHandler vHandler)
        {
            if (mHandler.ContainsKey(vKey))
            {
                 
                mHandler[vKey].Add(vHandler);
            }
            else
            {
                mHandler.Add(vKey,new List<RecordingErrorHandler>());
                    
            }
        }

        public void RemoveErrorHandler(string vKey, RecordingErrorHandler vHandler)
        {
            mHandler[vKey].Remove(vHandler);
        }

        
        public void Notify(string vKey, BodyFramesRecordingBase vBase)
        {
            foreach (var vErrorHandler in mHandler[vKey])
            {
                vErrorHandler.Notify(vBase);
            }
        }

        public void Clear()
        {
            mHandler.Clear();
        }

        public  void ClearErrorHandlerKey(string vKey)
        {
            mHandler[vKey].Clear();
        }
    }
}