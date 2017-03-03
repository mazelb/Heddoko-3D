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
        public IUserProfileManager ProfileManager { get; set; }
        private UserProfileModel mModel;
        public OrganizationTeamAccordionView AccordionView;
        private Dictionary<string, bool> mOrganizationTeamsDownloaded = new Dictionary<string, bool>();
        private Dictionary<string, bool> mTeamRecordingsDownloaded = new Dictionary<string, bool>();
        public RecordingListSyncView RecordingsListView;
        private const int RECORDINGS_TO_TAKE = 100;
        void Start()
        {
            ProfileManager = UserSessionManager.Instance;
            mModel = ProfileManager.UserProfile;
            Task.Factory.StartNew(FetchAllOrganizations);
        }

        /// <summary>
        /// Fetches all organizations
        /// </summary>
        private void FetchAllOrganizations()
        {
            ListCollection<Organization> vOrganizations =
                mModel.Client.GetAllOrganizations(new ListRequest { Take = 1000, Skip = 0 });
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
                if (mOrganizationTeamsDownloaded.ContainsKey(vArg1.Organization.Name))
                {
                    if (mOrganizationTeamsDownloaded[vArg1.Organization.Name])
                    {
                        var vList = vArg1.TeamCollection;
                        for (int vI = 0; vI < vList.Count; vI++)
                        {
                            AccordionView.AddTeamItem(vList[vI], DownloadRecordingListForTeam);
                        }
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
            else
            {
                AccordionView.RemoveTeamItemFromList(vArg1);
            }
        }

        /// <summary>
        /// Fetches a whole team list for an organization
        /// </summary>
        /// <param name="vArg1"></param>
        private void FetchTeamForOrganization(OrganizationListItem vArg1)
        {
            var vRequest = new TeamListRequest
            {
                Take = 1000,
                Skip = 0,
                OrganizationId = vArg1.Organization.ID
            };

            ListCollection<Team> vTeams = mModel.Client.GetAllTeams(vRequest);
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

        /// <summary>
        /// Download a list of recordings of a team.
        /// </summary>
        /// <param name="vArg1">The team to download the list for</param>
        /// <param name="vArg2">add or remove recordings?</param>
        private void DownloadRecordingListForTeam(TeamListItem vArg1, bool vArg2)
        {

            if (vArg2)
            {
                if (!mTeamRecordingsDownloaded.ContainsKey(vArg1.GetName))
                {
                    mTeamRecordingsDownloaded.Add(vArg1.GetName, new bool());
                    mTeamRecordingsDownloaded[vArg1.GetName] = true;
                    Task.Factory.StartNew(() =>
                    {
                        int vRemainingRec = 1;
                        bool vStart = true;
                        int vSkip = 0;
                        int vTotalCount = 0;
                        while (vRemainingRec > 0)
                        {

                            if (vStart)
                            {
                                int vTotalRetrieved = AddRecordingsToTeamListItemList(ref vArg1, RECORDINGS_TO_TAKE,
                                    vSkip, out vTotalCount);
                                vStart = false;
                                vRemainingRec = vTotalCount - vTotalRetrieved;
                            }
                            else
                            {
                                if (vRemainingRec < RECORDINGS_TO_TAKE)
                                {
                                    AddRecordingsToTeamListItemList(ref vArg1, vRemainingRec,
                                    vSkip, out vTotalCount);
                                    break;
                                }
                                else
                                {
                                    int vTotalRetrieved = AddRecordingsToTeamListItemList(ref vArg1, RECORDINGS_TO_TAKE,
                                   vSkip, out vTotalCount);
                                    vRemainingRec = vTotalCount - vTotalRetrieved;
                                }

                            }
                        }
                        OutterThreadToUnityThreadIntermediary.QueueActionInUnity(() => RecordingsListView.LoadData(vArg1.RecordingsList));

                    });

                }
                else
                {
                    RecordingsListView.LoadData(vArg1.RecordingsList);
                }

            }
            //remove items
            else
            {
                var vRecList = vArg1.RecordingsList;
                for (int vI = 0; vI < vRecList.Count; vI++)
                {
                    RecordingsListView.Remove(vRecList[vI]);
                }
            }
        }

        /// <summary>
        /// helper method to retrieve file list
        /// </summary>
        /// <param name="vTeamListItem"></param>
        /// <param name="vItemsToTake"></param>
        /// <param name="vSkipItems"></param>
        /// <param name="vTotal"></param>
        /// <returns></returns>
        private int AddRecordingsToTeamListItemList(ref TeamListItem vTeamListItem, int vItemsToTake, int vSkipItems, out int vTotal)
        {
            vTotal = 0;
            int vTotalItemsRetrieved = 0;
            var vRequest = new RecordListRequest
            {
                Take = vItemsToTake,
                Skip = vSkipItems,
                TeamId = vTeamListItem.Team.ID
            };
            ListCollection<Record> vRecords = mModel.Client.GetAllRecords(vRequest);
            if (vRecords != null)
            {
                if (vRecords.Collection.Count > 0)
                {
                    var vRecordCollection = vRecords.Collection;
                    vTotal = vRecords.TotalCount;
                    for (int vI = 0; vI < vRecords.TotalCount; vI++)
                    {
                        var vRecord = vRecordCollection[vI];
                        for (int vJ = 0; vJ < vRecord.Assets.Count; vJ++)
                        {
                            var vRecordedAsset = vRecord.Assets[vJ];
                            if (!string.IsNullOrEmpty(vRecordedAsset.Name) &&
                                vRecordedAsset.Type == (AssetType.Record | AssetType.RawFrameData))
                            {
                                RecordingListItem vItem = new RecordingListItem();
                                vItem.Name = vRecordedAsset.Name;
                                vItem.AssetType = AssetType.Record;
                                RecordingListItem.RecordingItemLocation vLoc =
                                    new RecordingListItem.RecordingItemLocation(vRecordedAsset.Url,
                                        RecordingListItem.LocationType.RemoteEndPoint);
                                vItem.Location = vLoc;
                                vItem.User = vRecords.Collection[vI].User;
                                vTeamListItem.RecordingsList.Add(vItem);
                            }
                        }
                    }
                }
                vTotalItemsRetrieved = vRecords.Collection.Count;
            }
            return vTotalItemsRetrieved;
        }
    }
}