
/** 
* @file CryptoManager.cs
* @brief Contains the CryptoManager class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Frames_Pipeline.BodyFrameEncryption
{
    /// <summary>
    /// Crypto manager that provides methods to decrypt and encrypt bytes[], string
    /// </summary>
    internal class CryptoManager
    {
        private IFrameDecryptor mFrameDecryptor;
        private IFrameEncryptor mFrameEncryptor;

        internal CryptoManager(IFrameDecryptor vFrameDecryptor, IFrameEncryptor vFrameEncryptor)
        {
            mFrameDecryptor = vFrameDecryptor;
            mFrameEncryptor = vFrameEncryptor;
        }

        /// <summary>
        /// Get Crypto version
        /// </summary>
        public string GetCrytpoVersion
        {
            get { return mFrameDecryptor.CryptoRevision; }
        }
        /// <summary>
        /// returns a decrypted string from a provided file path
        /// </summary>
        /// <param name="vFilepath"></param>
        /// <returns></returns>
        internal string Decrypt(string vFilepath )
        { 
            return mFrameDecryptor.Decrypt(vFilepath);
        }

        /// <summary>
        /// A unity3d friendly function that uses an Ienumerator to help with decrypting a file. 
        /// </summary>
        /// <param name="vFilePath"></param>
        /// <param name="vDecryptedMsgSetter">the callback action that will set the decrypted message</param>
        /// <returns></returns>
        internal IEnumerator U3DDecryptFile(string vFilePath, Action<string> vDecryptedMsgSetter )
        {
            Debug.Log("in U3DDecryptFile,start...");

            yield return OutterThreadToUnityThreadIntermediary.HelpStartCoroutine(mFrameDecryptor.U3DDecryptFile(vFilePath, vDecryptedMsgSetter));
        }

        /// <summary>
        /// stops decryption
        /// </summary>
        public void StopDecryption()
        {
            mFrameDecryptor.StopDecryption = true;
        }
    }
}
