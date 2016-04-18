/** 
* @file Tag.cs
* @brief Contains the Tag class
* @author Mohammed Haider(Mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using JetBrains.Annotations;

namespace Assets.Scripts.UI.Tagging
{
    /// <summary>
    /// A tag  containing a tile and its unique identifier
    /// </summary>
    public class Tag : IEquatable<Tag>
    {
        public string TagUid;
        public string Title;
        public bool Equals([NotNull] Tag other)
        {
            if (other == null) throw new ArgumentNullException("other");
            return TagUid.Equals(other.TagUid);
        }
    }
}
