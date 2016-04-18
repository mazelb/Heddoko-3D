
/** 
* @file EncryptionVersion0.cs
* @brief Contains the EncryptionVersion0 class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

namespace Assets.Scripts.Frames_Pipeline.BodyFrameEncryption.Encryption
{
     /// <summary>
     /// Encrytion Version 0: per byte adds the value of 0x80
     /// </summary>
    internal class EncryptionVersion0 : IFrameEncryptor
    {
         public string CryptoRevision { get; private set; }
    }
}
