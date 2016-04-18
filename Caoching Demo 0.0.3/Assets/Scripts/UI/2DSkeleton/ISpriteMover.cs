/** 
* @file ISpriteMover.cs
* @brief Contains the ISpriteMover class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

namespace Assets.Scripts.UI._2DSkeleton
{
    /// <summary>
    /// Sprite mover interface
    /// </summary>
    public interface ISpriteMover
    {
        /// <summary>
        /// ResetValues the orientation to the default
        /// </summary>
        void ResetOrientations();

        /// <summary>
        /// Applies transformation  
        /// </summary>
        void ApplyTransformations();

        /// <summary>
        /// Applies translations with respect to the new dispalacement
        /// </summary>
        /// <param name="vNewDisplacement"></param>
        void ApplyTranslations(float vNewDisplacement);

        /// <summary>
        /// activates or deactives the gameobject
        /// </summary>
        void SetActive(bool vFlag);
    }
}