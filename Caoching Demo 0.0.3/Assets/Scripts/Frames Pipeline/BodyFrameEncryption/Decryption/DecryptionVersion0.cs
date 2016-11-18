
/** 
* @file DecryptionVersion0.cs
* @brief Contains the DecryptionVersion0 class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using Assets.Scripts.UI.Loading;
 

namespace Assets.Scripts.Frames_Pipeline.BodyFrameEncryption.Decryption
{
    /// <summary>
    /// As of 1-27-2016, the encryption method obfuscates data by adding 0x80
    /// </summary>
    internal class DecryptionVersion0 : IFrameDecryptor
    {
        public bool StopDecryption { get; set; }

        public string CryptoRevision
        {
            get { return "1"; }
        }

        public string Decrypt(string vFilepath)
        {
            StreamReader vFile = new StreamReader(vFilepath);
            //Read one line, this line is the header line. Older brainpack firmware did not
            //include this header file, so we make sure that this file exist. Otherwise, we add in a guid 
            string vLine ="";
            int vHeaderLength = 0;
            while ((vLine = vFile.ReadLine()) != null)
            {
                break;
            }
            int vSize = 0;
            string vStringOut = Guid.NewGuid() + "\r\n" + Guid.NewGuid() + "\r\n";//+ Guid.NewGuid() + "\r\n";
            try
            {
                if (vLine != null && vLine.Contains("BPVERSION:"))
                {
                    vSize= System.Text.Encoding.Default.GetByteCount(vLine+"\r\n");
                    vLine = vLine.Replace("BPVERSION:", "");
                    var vExploded = vLine.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    vStringOut += vExploded[1]+"\r\n";
                    //add date time
                    vStringOut += vExploded[2] + "\r\n";
                }
            }
            catch (Exception)
            {
                vStringOut = Guid.NewGuid() + "\r\n" + Guid.NewGuid() + "\r\n" + Guid.NewGuid() + "\r\n" + DateTime.Now.ToString("yyy-MM-ddTHH:mm:ff");

            }
            try
            {
                var vStartIndex = vLine == null ? 0 : vSize;

                FileInfo vFileInfo = new FileInfo(vFilepath);
                 
               byte[] vByteArr = new byte[vFileInfo.Length- vStartIndex]; 
                FileStream vFileStream = new FileStream(vFilepath, FileMode.Open, FileAccess.Read);
                vFileStream.Seek(vStartIndex, SeekOrigin.Begin);
                vFileStream.Read(vByteArr, 0, vByteArr.Length);
                for (int vIndex =0; vIndex < vByteArr.Length; vIndex++)
                {
                    byte vReadbyte = vByteArr[vIndex];
                    const byte vAdd = 0x80;
                    vByteArr[vIndex] -= vAdd; 
                    if (StopDecryption)
                    {
                        return ""; 
                    } 
                }
                //strip away first 
                vStringOut += System.Text.Encoding.Default.GetString(vByteArr);
           
            }

            catch (Exception vE)
            {
                UnityEngine.Debug.Log("Error vE, index ");
                //todo: place a error logger here
            }

            return vStringOut;

        }

        public IEnumerator U3DDecryptFile(string vFilePath, Action<string> vGetter)
        {
             
            string vOutPut = Guid.NewGuid() + "\r\n" + Guid.NewGuid() + "\r\n" + Guid.NewGuid() + "\r\n";

            using (FileStream fs = new FileStream(vFilePath, FileMode.Open, FileAccess.Read))
            {
                Int32 vReadbyte = 0x00;
                while ((vReadbyte = (Int32)fs.ReadByte()) != -1)
                {
                    Int32 vTemp = vReadbyte - 0x80;
                    vOutPut += Convert.ToChar((byte)vTemp);
                    yield return null;
                }
            }
          
            vGetter(vOutPut);
        }

        
    }
}
