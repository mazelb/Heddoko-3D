/**
* @file UserRolePermission.cs
* @brief Contains the  UserRolePermission attribute
* @author Mohammed Haider( mohammed@heddoko.com)
* @date June 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Linq;
using HeddokoSDK.Models;

namespace Assets.Scripts.UI.AbstractViews.Permissions
{
    /// <summary>
    /// Sets an elements user role permission level
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]

    public class UserRolePermission : Attribute
    {
         private Dictionary<UserRoleType, bool> mUserRolePermissions;
        private HashSet<UserRoleType> mAllowedRoles; 
        /// <summary>
        /// Pass in a specific set of permitted user roles to use the functionality. 
        /// </summary>
        /// <param name="vAllowedUserRoles">The set of user roles that are allowed</param> 
        public UserRolePermission(UserRoleType[] vAllowedUserRoles)
        {
            foreach (var vAllowedUserRole in vAllowedUserRoles)
            {
                if (AllowedRoles.Contains(vAllowedUserRole))
                {
                    continue;
                }
                AllowedRoles.Add(vAllowedUserRole);
            }
        }

        /// <summary>
        /// Default constructor , setting all user roles to true
        /// </summary> 
        public UserRolePermission()
        {
            UserRoleType[] vUserRoles = EnumUtil.GetValues<UserRoleType>() as UserRoleType[];
            foreach (var vUserRole in vUserRoles)
            {
                AllowedRoles.Add(vUserRole);
            } 
        }

        /// <summary>
        /// Pass in one specific user role. Every other role will be denied
        /// </summary>
        /// <param name="vUserRoleType"></param>
        public UserRolePermission(UserRoleType vUserRoleType)
        {
            AllowedRoles.Clear();
            AllowedRoles.Add(vUserRoleType);
        }
   
        /// <summary>
        /// The list of allowed user roles
        /// </summary>
        public HashSet<UserRoleType> AllowedRoles
        {
            get
            {
                if (mAllowedRoles == null)
                {
                    mAllowedRoles = new HashSet<UserRoleType>();
                }
                return mAllowedRoles;
            }
        }
        /// <summary>
        /// Verifies if the passed in type has permssion with regards to the user role type. 
        /// </summary>
        /// <remarks>If the passed in type doesn't have a UserRolePermission attribute, then a NullReference exception is explicitly thrown. </remarks>
        /// <param name="v">The type</param>
        /// <param name="vUserRoleType">the user role type</param>
        /// <returns>does the type allow the user role type to access its features?</returns>
        public static bool HasPermission(Type v, UserRoleType vUserRoleType)
        {
             var vAttribute =
                (UserRolePermission)Attribute.GetCustomAttribute(v, typeof(UserRolePermission));
            if (vAttribute == null)
            {
                throw new NullReferenceException("The type " + v + " doesn't have a UserRolePermission assigned.");
            }

            List<UserRoleType> vType = vAttribute.AllowedRoles.ToList();
            if (!vType.Contains(vUserRoleType))
            {
                return false;
            }
            return true;
        }
    }
}