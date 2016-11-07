/**
* @file RenderedBodyPool.cs
* @brief Contains the RenderedBodyPool class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{
    /// <summary>
    /// Contains a pool of inactive and active rendered bodies
    /// </summary>
   public static class RenderedBodyPool
    {
        public static List<RenderedBody> sInUsePool  = new List<RenderedBody>(10);
        private static List<RenderedBody> sAvailablePool  = new List<RenderedBody>(10);
        public static Transform ParentGroupTransform;
        //available layers that can be used
        private static List<LayerMask> sAvailableLayers = new List<LayerMask>(8);
        private static List<LayerMask> sInUseLayers = new List<LayerMask>(8);
        private static bool sInitiated;
      
        /// <summary>
        /// Request a bodybrain pack resource
        /// </summary>
        /// <param name="vBodyTypes"></param>
        /// <returns></returns>
        public static RenderedBody RequestResource(BodyStructureMap.BodyTypes vBodyTypes, bool reference = false)
        {
            if (sInitiated == false)
            {
                sInitiated = true;
                   Init();
            }
            RenderedBody vPooledBody = null;
            if (sAvailableLayers.Count == 0)
            {
                return null;
            }

            if (sAvailablePool.Count != 0)
            {
                vPooledBody = sAvailablePool[0];
                vPooledBody.UpdateBodyType(vBodyTypes);
                vPooledBody.gameObject.SetActive(true);
                sAvailablePool.RemoveAt(0);
                sInUsePool.Add(vPooledBody);
                int vLayer = sAvailableLayers[0];
//                 sAvailableLayers.RemoveAt(0);//.Pop();
//                 sInUseLayers.Add(vLayer);
                vPooledBody.CurrentLayerMask =vLayer;
            }
            else
            {
                //instantiate obj from resources
                GameObject vObj = null;
                if(reference)
                    vObj = Resources.Load("Prefabs/Models/reference") as GameObject;
                else
                    vObj = Resources.Load("Prefabs/Models/SegmentedModel") as GameObject;

                GameObject vNew = GameObject.Instantiate(vObj);
                vPooledBody = vNew.GetComponent<RenderedBody>();  
                vPooledBody.gameObject.SetActive(false);
                vPooledBody.Init(vBodyTypes);
                vPooledBody.gameObject.SetActive(true); 
                vNew.transform.SetParent(ParentGroupTransform);
                int vLayer = sAvailableLayers[0];
//                 sAvailableLayers.RemoveAt(0); 
//                 sInUseLayers.Add(vLayer);
                vPooledBody.CurrentLayerMask = vLayer;
            }
            return vPooledBody;
        }

        /// <summary>
        /// Releases a rendered body from the InUse pool, cleans up resources and makes it available for use
        /// </summary>
        /// <param name="vRenderedBody"></param>
        public static void ReleaseResource(RenderedBody vRenderedBody)
        { 
            vRenderedBody.Cleanup();
            sInUsePool.Remove(vRenderedBody);
            sAvailablePool.Add(vRenderedBody);
           
            int vLayer = sInUseLayers[0];
            sInUseLayers.RemoveAt(0);
            sAvailableLayers.Add(vLayer); 
        }

        /// <summary>
        /// initalize masks
        /// </summary>
        static void Init()
        {
            for (int i = 6; i >= 0; i--)
            {
                sAvailableLayers.Add(LayerMask.NameToLayer("RenderedBody"+i));
            }
            //default model
            sAvailableLayers.Add(LayerMask.NameToLayer("model")); 
        }

    }
}
