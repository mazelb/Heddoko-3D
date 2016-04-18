/**  
* @file IContextSpecificLearningView.cs 
* @brief Contains the IContextSpecificLearningView interface 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved 
*/

using UnityEngine.UI;

namespace Assets.Scripts.UI.ActivitiesContext.View
{
    /// <summary>
    /// Provides an interface for Context specific views
    /// </summary>
    public interface IContextSpecificLearningView
    {
        IActivitiesContextViewSubcomponent ContextViewSubcomponent { get; }
        Button ContextSpecificButton { get; }
       
    }
}
