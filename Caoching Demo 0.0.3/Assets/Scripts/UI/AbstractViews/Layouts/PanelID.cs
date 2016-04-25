/** 
* @file PanelID.cs
* @brief PanelID  
* @author Mohammed Haider(mohamed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System; 
namespace Assets.Scripts.UI.Layouts
{
    /// <summary>
    /// The id of a panel
    /// </summary>
    public class PanelID
    {
        private Guid mID = Guid.NewGuid();

        public string Id
        {
            get { return mID.ToString(); }
        }

        /// <summary>
        /// Get the hashcode 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return mID.GetHashCode();
        }
    }
}
