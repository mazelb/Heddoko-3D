
/** 
* @file BodyRecordingManagerProxy.cs
* @brief Contains the BodyRecordingManagerProxy interface
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved 
*/


using System; 
using Assets.Scripts.Communication.DatabaseConnectionPipe;
using Assets.Scripts.Frames_Recorder.FramesRecording;

namespace Assets.Scripts.Frames_Recorder
{
    /// <summary>
    /// Acts as a proxy to the body recording manager but also has a database connection to 
    /// retrieve and store recordings
    /// </summary>
    public class BodyRecordingManagerProxy
    {
        private static BodyRecordingManagerProxy sInstance = new BodyRecordingManagerProxy();
        public Database Database;

        public static BodyRecordingManagerProxy Instance
        {
            get { return sInstance; }
        }

        /// <summary>
        /// Locates the Recording, if not found searches the database
        /// Note: will return null if not found
        /// </summary>
        /// <param name="vRecguid"></param>
        /// <param name="vCallback">optional paramater to call back once recording is retrieved</param>
        /// <returns></returns>
        public BodyFramesRecordingBase GetRecording(string vRecguid, Action<BodyFramesRecordingBase> vCallback = null)
        {
            BodyFramesRecordingBase vRecording = BodyRecordingsMgr.Instance.GetRecordingByUuid(vRecguid);
            if (vRecording == null)
            {
                //locate it from the database
                vRecording = Database.Connection.GetRawRecording(vRecguid);
                if (vRecording != null)
                {
                    BodyRecordingsMgr.Instance.Recordings.Add(vRecording);
                    if (vCallback != null)
                    {
                        vCallback.Invoke(vRecording);
                    }
                }
            }
            return vRecording;
        }



    }
}
