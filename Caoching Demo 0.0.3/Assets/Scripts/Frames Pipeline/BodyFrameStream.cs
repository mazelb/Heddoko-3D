using UnityEngine;
using System.Collections;
using System.IO;

public class BodyFrameStream  {
    
    //MemoryStream to hold the received data
    private MemoryStream mMemoryStream = new MemoryStream();

    public void SetStreamLength(long vStreamLength)
    {
        mMemoryStream.SetLength(vStreamLength);
    }

    public void StartFrame()
    {

    }

    public void EndFrame()
    {

    }

    public void WriteBytesToStream(byte[] vReceivedBytes)
    {
        //Write the received bytes to the stream

    }

    //Receive full frame stream
    //Once full stream received 
    //parse frame 
    //Add frame to buffer

    //memStream.SetLength(fileStream.Length);
    ////read file to MemoryStream
    //fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
    //string[] vFrameData = vFileLine.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
}
