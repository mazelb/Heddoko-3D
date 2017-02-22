// /**
// * @file OrganizationListItem.cs
// * @brief Contains the OrganizationListItem class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date January 2017
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */


using System.Collections.Generic;
using HeddokoSDK.Models;

namespace Assets.Scripts.UI.RecordingLoading.Model
{
    /// <summary>
    /// A model for an organization list item
    /// </summary>
    public class OrganizationListItem
    {
        /// <summary>
        /// The organization that this item represents
        /// </summary>
        public Organization Organization { get; set; }

        /// <summary>
        /// Adds a team item to the organization
        /// </summary>
        /// <param name="vItem"></param>
        public void AddTeamItem(TeamListItem vItem)
        {
            vItem.AssociatedOrganization = this;
            TeamCollection.Add(vItem);
        }
        /// <summary>
        /// The team in an organization
        /// </summary>
        public List<TeamListItem> TeamCollection  = new List<TeamListItem>();  


    }
}