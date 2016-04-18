using Assets.Scripts.UI.AbstractViews.AbstractPanels;

namespace Assets.Scripts.UI.DemoKit
{
    /// <summary>
    /// Demo player: uses a single instance of a body and a single instance rendered body. This will allow
    /// the body to be played on multiple screens
    /// </summary>
    public class DemoPlayer : AbstractControlPanel
    {
        public Body DemoBody;
        //need to create a scriptable object that holds a list of recordings 
        //on awake init thread, play on loop
        public override void ReleaseResources()
        {

        }

        public override void BodyUpdated(Body vBody)
        {
            DemoBody = vBody;
        }
        /// <summary>
        /// initialize body playback
        /// </summary>
        /// <param name="vLines"></param>
        public void InitBodyPlayback(string[] vLines)
        {
            BodyFramesRecording vTempRecording = new BodyFramesRecording();
            vTempRecording.ExtractRecordingUUIDs(vLines);
            vTempRecording.ExtractRawFramesData(vLines);
            BodyRecordingsMgr.Instance.AddNewRecording(vLines);
            
            DemoBody.PlayRecording(vTempRecording.BodyRecordingGuid);
            DemoBody.MBodyFrameThread.PlaybackTask.LoopPlaybackEnabled = false;
        }
    }
}