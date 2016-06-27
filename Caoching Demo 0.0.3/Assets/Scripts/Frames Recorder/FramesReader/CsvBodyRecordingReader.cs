/** 
* @file CsvBodyRecordingReader.cs
* @brief Contains the BodyFramesReader class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
*/
 
using System; 
using System.IO;
using Assets.Scripts.Frames_Pipeline.BodyFrameEncryption;
using Assets.Scripts.Frames_Pipeline.BodyFrameEncryption.Decryption;
using Assets.Scripts.Frames_Pipeline.BodyFrameEncryption.Encryption;
 

/**
* CsvBodyRecordingReader class 
* @brief Class reads body frames from Recorded CSV file
* CSV line structure (single frame):
* RECORDING GUID
* CONTAINING BODY GUID
* SUIT GUID
* BOIMECH_sensorID_1, Yaw;Pitch;Roll, ... BOIMECH_sensorID_9, Yaw;Pitch;Roll, FLEXCORE_sensorID_1, SensorValue, ... ,FLEXCORE_sensorID_4, SensorValue
*/
public class CsvBodyRecordingReader : BodyRecordingReaderBase
{
   
    //The entire file content
    private string mFileContents;
    //Line by line content
    private string[] mFileLines;

    private CryptoManager mCryptoManager;

    

    /// <summary>
    /// Was the recording taken from a dat file or csv file?
    /// </summary>
    public bool IsFromDatFile { get;   set; }

    internal CryptoManager CrytoManager
    {
        get { return mCryptoManager; }
    }

    /// <summary>
    /// A body recording reader with a path to default reading from a csv file
    /// </summary>
    /// <param name="vFilepath"></param>
    public CsvBodyRecordingReader(string vFilepath)
    {
        mCryptoManager = new CryptoManager(new DecryptionVersion0(), new EncryptionVersion0());
        FilePath = vFilepath;
    }

    /**
    * ReadFile()
    * @param vFilePath: The file path to read
    * @brief Reads full content of file to memory
    * if the file contents are not empty automatically 
    * populates line data
    */
    public override int ReadFile(string vFilePath)
    {
        FilePath = vFilePath;

        //open file from the disk (file path is the path to the file to be opened)
        if (vFilePath.Contains(".csv"))
        {
            using (StreamReader vStreamReader = new StreamReader(File.OpenRead(FilePath)))
            {
                mFileContents = vStreamReader.ReadToEnd();
                if (mFileContents.Length > 0)
                {
                    PopulateRecordingLines(mFileContents);
                }
            }
        }
        else
        {
            try
            {
                // byte[] vContents = File.ReadAllBytes(vFilePath);
                mFileContents = mCryptoManager.Decrypt(vFilePath);
                if (mFileContents.Length > 0)
                {
                    IsFromDatFile = true;
                    PopulateRecordingLines(mFileContents);
                }


            }
            catch (Exception)
            {

            }
        }
 
        return mFileContents.Length;
    }
 


    /**
    * PopulateRecordingLines()
    * @brief splits the string into frame lines
    */
    public   void PopulateRecordingLines()
    {
        if (mFileContents.Length > 0)
        {
            PopulateRecordingLines(mFileContents);
        }
    }

    /**
    * PopulateRecordingLines()
    * @param vFileContent: The file content
    * @brief splits the string into frame lines
    */
    public void PopulateRecordingLines(string vFileContent)
    {
        if (vFileContent.Length > 0)
        {
            //Split the data into single lines. Each line is a Frame
            mFileLines = vFileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }
    }



    /**
    * GetRecordingLines()
    * @brief Returns all lines read from CSV
    */
    public string[] GetRecordingLines()
    {
        return mFileLines;
    }

    /// <summary>
    /// stops processes safely
    /// </summary>
    internal void Stop()
    {
        mCryptoManager.StopDecryption();
    }
}
