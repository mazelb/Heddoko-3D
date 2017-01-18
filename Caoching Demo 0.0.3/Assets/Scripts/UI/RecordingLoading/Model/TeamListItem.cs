// /**
// * @file TeamListItem.cs
// * @brief Contains the TeamListItem class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date January 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using System.Collections.Generic;
using HeddokoSDK.Models;

namespace Assets.Scripts.UI.RecordingLoading.Model
{
    public class TeamListItem
    {
        /// <summary>
        /// The assocated organization the instance is associated with
        /// </summary>
        public OrganizationListItem AssociatedOrganization { get; set; }

        public List<RecordingListItem> RecordingsList = new List<RecordingListItem>();  

        /// <summary>
        /// The team the TeamListItem is associated with
        /// </summary>
        public Team Team { get; set; }

        /// <summary>
        /// Get the name of the team with its organization
        /// </summary>
        public string GetName
        {
            get { return AssociatedOrganization.Organization.Name + " : " + Team.Name; }
        }
    }
}