
/** 
* @file IFrameDecryptor.cs
* @brief Contains the IFrameDecryptor interface
* @author Mohammed Haider (Mohammed@heddoko.com)
* @date January 2016
*/

using System;
using System.Collections;
using JetBrains.Annotations;

namespace Assets.Scripts.Frames_Pipeline.BodyFrameEncryption
{
    /// <summary>
    /// Provides an interface for frame decr
    /// </summary>
    public interface IFrameDecryptor
    {
        string CryptoRevision { get; }
        string Decrypt(string vFilePath);

        /// <summary>
        /// Unity3D friendly function that decrypts a file using Ienumerator(unity will use these in coroutines)
        /// </summary>
        /// <param name="vFilePath">the path of the file to be decrypted</param> 
        /// <param name="vDecryptedSetter">A function that is used to return the resulting decrypted string</param>
        /// <returns></returns>
        IEnumerator U3DDecryptFile(string vFilePath, Action<string> vDecryptedSetter);

        /// <summary>
        /// Stops decryption
        /// </summary>
        bool StopDecryption { get; set; }
    }


}