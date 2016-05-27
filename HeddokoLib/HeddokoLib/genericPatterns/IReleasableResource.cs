// /**
// * @file IReleasableResource.cs
// * @brief Contains the IReleasableResource
// * @author Mohammed Haider( mohammed@ heddoko.com)
// * @date May 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */
namespace HeddokoLib.genericPatterns
{
    /// <summary>
    /// An interface indicating that the resource is releasable
    /// </summary>
    public interface IReleasableResource
    {
        /// <summary>
        /// Clean up resources
        /// </summary>
        void Cleanup();
    }
}