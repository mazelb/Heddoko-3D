 /**
 * @file IViewFactory.cs
 * @brief Contains the IViewFactory
 * @author Mohammed Haider(mohammed@heddoko.com)
 * @date June 2016
 * Copyright Heddoko(TM) 2016,  all rights reserved
 */

using Assets.Scripts.Licensing.Model;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Provides an interface for an IViewFactory
    /// </summary>
    public interface IViewFactory
    {
        void Construct(UserProfileModel vUserProfileModel, PlatformType vPlatformType);
    }

    /// <summary>
    /// Enum for a platform type
    /// </summary>
    public enum PlatformType
    {
        Windows,
        WindowsPhone,
        Android,
        Ios
    }
}