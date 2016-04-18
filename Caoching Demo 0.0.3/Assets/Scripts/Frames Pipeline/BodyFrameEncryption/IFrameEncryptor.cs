/** 
* @file IFrameEncryptor.cs
* @brief Contains the IFrameEncryptor interface
* @author Mohammed Haider (Mohammed@heddoko.com)
* @date January 2016
*/

namespace Assets.Scripts.Frames_Pipeline.BodyFrameEncryption
{
    /// <summary>
    /// Provides an interface for decryption
    /// </summary>
    public   interface IFrameEncryptor
    {
           string CryptoRevision { get; }
    }
}