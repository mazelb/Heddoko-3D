/** 
* @file AbstractSubControl.cs
* @brief Contains the AbstractSubControl class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Permissions;
using HeddokoSDK.Models;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls
{
    /// <summary>
    /// Abstract subcontrol: An abstraction of a control panels sub controls
    /// </summary>
    public abstract class AbstractSubControl : MonoBehaviour, IEquatable<AbstractControlPanel>
    {

        public abstract SubControlType SubControlType { get; }

        public bool Equals(AbstractControlPanel other)
        {
            if (other != null)
            {
                return gameObject.GetInstanceID() == other.gameObject.GetInstanceID();
            }
            return false;
        }

        public abstract void Disable();

        public abstract void Enable();

        public virtual void SetInteractionLevel(UserRoleType vRoleType)
        {
            var vCanUse = UserRolePermission.HasPermission(this.GetType(), vRoleType);
            Debug.Log("vCanUse is" + vCanUse);
            gameObject.SetActive(vCanUse);
        }
    }
}
