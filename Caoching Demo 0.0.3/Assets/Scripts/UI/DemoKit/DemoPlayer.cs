using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.UI.AbstractViews.AbstractPanels;
using Assets.Scripts.UI.AbstractViews.Enums;

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

        public override ControlPanelType PanelType
        {
            get { return ControlPanelType.DebugRecordingPanel;}
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
            CsvBodyFramesRecording vTempRecording = new CsvBodyFramesRecording();
            vTempRecording.ExtractRecordingUuiDs(vLines);
            vTempRecording.ExtractRawFramesData(vLines);
            BodyRecordingsMgr.Instance.AddNewRecording(vLines,"Demo recording",false, PlayRecordingCallback);

        }

        private void PlayRecordingCallback(CsvBodyFramesRecording vRecording)
        {
            DemoBody.PlayRecording(vRecording.BodyRecordingGuid);
            DemoBody.MBodyFrameThread.PlaybackTask.LoopPlaybackEnabled = false;
        }

        public void PlayRecording(string vGuid)
        {
            DemoBody.PlayRecording(vGuid);
            DemoBody.MBodyFrameThread.PlaybackTask.LoopPlaybackEnabled = false;
        }
        public void PauseRecording()
        {
            DemoBody.View.IsPaused = true; 
        }

        public void UnPauseRecording()
        {
            DemoBody.View.IsPaused = false;
        }
 
    }
}