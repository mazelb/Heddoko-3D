
/** 
* @file BrainpackConnectionRouter.cs
* @brief Contains the BrainpackConnectionRouter class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections.Generic;

namespace BrainpackService.BrainpackServer
{
    /// <summary>
    /// Routes brainpack connection to a list of interested stateobjects
    /// </summary>
  public  class BrainpackConnectionRouter
    {
         public ServerCommandRouter ServerCommandRouter { get; set; }
        

    }
}
