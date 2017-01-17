// /**
// * @file OrganizationViewController.cs
// * @brief Contains the OrganizationViewController class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date January 2017
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.MainApp;
using Assets.Scripts.UI.RecordingLoading.Model;
using Assets.Scripts.UI.RecordingLoading.View;
using Assets.Scripts.Utils;
using HeddokoSDK.Models;
using HeddokoSDK.Models.Requests;
using UnityEngine;

namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    /// A controller for an organization view in scene
    /// </summary>
    public class OrganizationViewController : MonoBehaviour
    {
        public IUserProfileManager mProfileManager { get; set; }
        private UserProfileModel mModel;
        public OrganizationTeamAccordionView AccordionView;
        private Dictionary<string, bool> mOrganizationTeamsDownloaded = new Dictionary<string, bool>();
        private Dictionary<string, bool> mTeamRecordingsDownloaded = new Dictionary<string, bool>();
        public RecordingListViewController RecordingListViewController;
        void Start()
        {
            mProfileManager = UserSessionManager.Instance;
            mModel = mProfileManager.UserProfile;
            Task.Factory.StartNew(FetchAllOrganizations);
        }

        /// <summary>
        /// Fetches all organizations
        /// </summary>
        private void FetchAllOrganizations()
        {
            ListCollection<Organization> vOrganizations = mModel.Client.GetAllOrganizations(new ListRequest { Take = 1000, Skip = 0 });
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                foreach (Organization vOrganization in vOrganizations.Collection)
                {
                    OrganizationListItem vItem = new OrganizationListItem();
                    vItem.Organization = vOrganization;
                    AccordionView.AddOrgItem(vItem, DownloadTeamListForOrganization, DownloadRecordingListForTeam);
                }
            });
        }

        /// <summary>
        /// Downloads a team list for an organization
        /// </summary>
        /// <param name="vArg1"></param>
        /// <param name="vArg2"></param>
        private void DownloadTeamListForOrganization(OrganizationListItem vArg1, bool vArg2)
        {
            if (vArg2)
            {
                //Check if a request has already been made
                if (mOrganizationTeamsDownloaded.ContainsKey(vArg1.Organization.Name) )
                {
                    if (mOrganizationTeamsDownloaded[vArg1.Organization.Name])
                    {
                        return;
                    }
                    else
                    {
                        Task.Factory.StartNew(() => FetchTeamForOrganization(vArg1));
                    }
                }
                else
                {
                    bool vBool = new bool();
                    vBool = true;
                    mOrganizationTeamsDownloaded.Add(vArg1.Organization.Name, vBool);
                    Task.Factory.StartNew(() => FetchTeamForOrganization(vArg1));
                }
               
            }
        }

        /// <summary>
        /// Fetches a whole team list for an organization
        /// </summary>
        /// <param name="vArg1"></param>
        private void FetchTeamForOrganization(OrganizationListItem vArg1)
        {
            var request = new TeamListRequest
            {
                Take = 1000,
                Skip = 0,
                OrganizationId = vArg1.Organization.ID
            };

            ListCollection<Team> vTeams = mModel.Client.GetAllTeams(request);
            OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() =>
            {
                foreach (Team vTeam in vTeams.Collection)
                {
                    TeamListItem vTeamItem = new TeamListItem()
                    {
                        AssociatedOrganization = vArg1,
                        Team = vTeam
                    };
                    vArg1.AddTeamItem(vTeamItem);
                    AccordionView.AddTeamItem(vTeamItem, DownloadRecordingListForTeam);
                }
            });
           
        }

        private void DownloadRecordingListForTeam(TeamListItem vArg1, bool vArg2)
        {
        }
    }
}