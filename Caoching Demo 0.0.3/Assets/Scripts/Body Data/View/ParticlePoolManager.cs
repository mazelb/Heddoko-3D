// /**
// * @file ParticlePoolManager.cs
// * @brief Contains the ParticlePoolManager class
// * @author Mohammed Haider( mohammed @heddoko.com)
// * @date November 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{

    /// <summary>
    /// A pool of particles. Pass in a prefab to set the object template that will be created in the pool
    /// </summary>
    public class ParticlePoolManager
    {
        public ParticleSystemDisabler Prefab;
        private List<ParticleSystemDisabler> mActivePool;
        private List<ParticleSystemDisabler> mInactivePool;
        private LayerMask mCurrLayerMask;
        private int mSize;
        public ParticlePoolManager(int vSize, ParticleSystemDisabler vPrefab)
        {
            mSize = vSize;
            mActivePool = new List<ParticleSystemDisabler>(mSize);
            mInactivePool = new List<ParticleSystemDisabler>(mSize);
            Prefab = vPrefab;
        }

        /// <summary>
        /// Requests a particle system with the given position
        /// Note: if there are no more available particles, then null is returned
        /// </summary>
        /// <param name="vPosition"></param>
        /// <returns></returns>
        public ParticleSystemDisabler RequestResource(Vector3 vPosition)
        {
            if (mInactivePool.Count != 0  && mActivePool.Count < mSize)
            {
                ParticleSystemDisabler vPooledObj = mInactivePool[0];
                vPooledObj.gameObject.layer = mCurrLayerMask;
                mActivePool.Add(vPooledObj);
                mInactivePool.RemoveAt(0);
                vPooledObj.StartSystem(2.5f, vPosition);
                return vPooledObj;
            }
            else if(mActivePool.Count < mSize)
            {
                var vNewObj = GameObject.Instantiate(Prefab, vPosition, Quaternion.identity) as ParticleSystemDisabler;
                if (vNewObj != null)
                {
                    vNewObj.gameObject.layer = mCurrLayerMask;
                    vNewObj.RegisterDisableEvent(ReleaseResource);
                    vNewObj.StartSystem(2.5f, vPosition);
                    mActivePool.Add(vNewObj);
                }
                return vNewObj;
            }
            return null;
        }

        public void ReleaseResource(ParticleSystemDisabler vSystem)
        {
            mActivePool.Remove(vSystem);
            mInactivePool.Add(vSystem);
        }

        /// <summary>
        /// Set the default layermask for the instantiated prefab
        /// </summary>
        /// <param name="vCurrLayerMask"></param>
        public void SetLayer(LayerMask vCurrLayerMask)
        {
            mCurrLayerMask = vCurrLayerMask;
        }
    }
}