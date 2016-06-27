using Assets.Scripts.Frames_Recorder.FramesRecording;

public abstract class BodyRecordingReaderBase
{
    public abstract int ReadFile(string vFilePath);

    public string FilePath { get; set; }
}