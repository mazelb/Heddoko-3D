
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
            string vStringOut = Guid.NewGuid() + "\r\n" + Guid.NewGuid() + "\r\n" + Guid.NewGuid() + "\r\n";
            try
            { 
   
               byte[] vByteArr = File.ReadAllBytes(vFilepath);
             
                
                for (int i = 0; i < vByteArr.Length; i++)
                {
                    byte vReadbyte = vByteArr[i];
                    const byte vAdd = 0x80;
                    vByteArr[i] -= vAdd;
                    // vStringOut += vTemp.ToString();\
                    //vByteArr[i] = vTemp;
                    if (StopDecryption)
                    {
                        return ""; 
                    }
                   // LoadingBoard.UpdateAnimation();
                }
                vStringOut += System.Text.Encoding.Default.GetString(vByteArr);
           
            }

            catch
            {
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
