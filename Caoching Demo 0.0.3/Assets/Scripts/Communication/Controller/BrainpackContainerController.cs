// /**
// * @file BrainpackContainerController.cs
// * @brief Contains the BrainpackContainerController class and its functionalities.
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date 11 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UI;
using HeddokoLib.HeddokoDataStructs.Brainpack;
using HeddokoSDK.Models;

namespace Assets.Scripts.Communication.Controller
{
    /// <summary>
    /// A brainpack container controller, controlling an associated view
    /// </summary>
    public class BrainpackContainerController 
    {
        public AuthorizationManager AuthorizationManager;
        public BrainpackContainerPanel ContainerView;
        HashSet<BrainpackNetworkingModel> BrainpackSet =  new HashSet<BrainpackNetworkingModel>(); 

        public void AddBrainpack(BrainpackNetworkingModel vBrainpack)
        {
            if (BrainpackSet.Contains(vBrainpack))
            {
                throw new Exception("Container already contains Brainpack Id " + vBrainpack.Id);
            }
            if (AuthorizationManager.BrainpackIsAuthorized(vBrainpack))
            {
                ContainerView.AddBrainpackModel(vBrainpack);
                BrainpackSet.Add(vBrainpack);
            }
        }

        /// <summary>
        /// Returns an instance of the brainpack networking model by the given id
        /// </summary>
        /// <param name="vID"></param>
        /// <returns></returns>
        public BrainpackNetworkingModel GetBrainpack(string vId)
        {
            return BrainpackSet.First(x => x.Id.Equals(vId));
        }

        /// <summary>
        /// Updates the brainpack with  new tcp end point and control point.Control point default value is set to -1. If this value is not updated the associated model's port isn't
        /// </summary>
        /// <param name="vId"></param>
        /// <param name="vTcpEndpoint"></param>
        /// <param name="vTcpControlPort"></param>
        /// <returns></returns>
        public BrainpackNetworkingModel UpdateBrainpack(string vId, string vTcpEndpoint, int vTcpControlPort = -1)
        {
            var vBp = GetBrainpack(vId);
           
            if (!string.IsNullOrEmpty(vTcpEndpoint))
            {
                vBp.TcpIpEndPoint = vTcpEndpoint;
            }
            if (vTcpControlPort != -1)
            {
                vBp.TcpControlPort = vTcpControlPort;
            }
            return vBp;
        }

        /// <summary>
        /// Remove the brainpack from the list and clean up container. 
        /// </summary>
        /// <param name="vBrainpack"></param>
        public void RemoveBrainpack(BrainpackNetworkingModel vBrainpack)
        {
            BrainpackSet.Remove(vBrainpack);
        }

       
    }
}