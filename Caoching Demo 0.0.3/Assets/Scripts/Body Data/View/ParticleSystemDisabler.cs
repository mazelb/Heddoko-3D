// /**
// * @file ParticleSystemDisabler.cs
// * @brief Contains the ParticleSystemDisabler
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date Novembe 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{
    /// <summary>
    /// Throws an event after the gameobject has been disabled
    /// </summary>
    public class ParticleSystemDisabler : MonoBehaviour
    {
        private Action<ParticleSystemDisabler> OnDisableEvent;
        public ParticleSystem ParticleSystem;

        public void RegisterDisableEvent(Action<ParticleSystemDisabler> vAction)
        {
            OnDisableEvent = vAction;
        }

        public void StartSystem(float vDisableTimer, Vector3 vPosition)
        {
            ParticleSystem.Stop(true);
            transform.position = vPosition;
            gameObject.SetActive(true);
            ParticleSystem.Play(true);
            StartCoroutine(CountDown(vDisableTimer));
        }

        private IEnumerator CountDown(float vDisableTimer)
        {
            yield return new WaitForSeconds(vDisableTimer);
            gameObject.SetActive(false);
            if (OnDisableEvent != null)
            {
                OnDisableEvent(this);
            }
        }
    }
}