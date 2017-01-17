// /**
// * @file TeamListItem.cs
// * @brief Contains the TeamListItem class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date January 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using HeddokoSDK.Models;

namespace Assets.Scripts.UI.RecordingLoading.Model
{
    public class TeamListItem
    {
        /// <summary>
        /// The assocated organization the instance is associated with
        /// </summary>
        public OrganizationListItem AssociatedOrganization { get; set; }

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