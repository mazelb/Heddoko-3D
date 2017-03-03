// /**
// * @file OrganizationTeamAccordionView.cs
// * @brief Contains the OrganizationTeamAccordionView class
// * @author Mohammed Haider( mohammed@heddoko.com)
// * @date January 2017
// * Copyright Heddoko(TM) 2017,  all rights reserved
// */

using System;
using System.Collections.Generic;
using Assets.Scripts.UI.RecordingLoading.Model;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.RecordingLoading.View
{
    /// <summary>
    /// A class that holds a view with corresponding organization and teams
    /// </summary>
    public class OrganizationTeamAccordionView : MonoBehaviour
    {
        public Toggle SelectAllOrganizations;
        public Toggle SelectAllTeamsOrganization;
        public Selectable TeamSelectableOption;

        public TeamSelectionItem DefaultTeamItem;
        public OrganizationSelectionItem DefaultOrganizationOption;
        public Accordion Accordion;

        private Dictionary<string, TeamSelectionItem> mTeamSelectionItems = new Dictionary<string, TeamSelectionItem>();
        private Dictionary<string, OrganizationSelectionItem> mOrgSelectionItems = new Dictionary<string, OrganizationSelectionItem>();

        private int mOrganizationsSelected;

        void Start()
        {
            DefaultOrganizationOption.gameObject.SetActive(false);
            DefaultTeamItem.gameObject.SetActive(false);
            SelectAllOrganizations.onValueChanged.AddListener(SelectAllOrganizationsCallback);
            SelectAllTeamsOrganization.onValueChanged.AddListener(SelectAllTeams);
        }

        /// <summary>
        /// Selects or deselects all teams
        /// </summary>
        /// <param name="vArg0">the value of the selection</param>
        private void SelectAllTeams(bool vArg0)
        {
            foreach (var vTeam in mTeamSelectionItems)
            {
                vTeam.Value.Toggle.isOn = vArg0;
            } 
        }

        /// <summary>
        /// Selects all organizations
        /// </summary>
        /// <param name="vArg0"></param>
        private void SelectAllOrganizationsCallback(bool vArg0)
        {
            foreach (var vOrganizationSelectionItem in mOrgSelectionItems)
            {
                vOrganizationSelectionItem.Value.Toggle.isOn = vArg0;
            }
            if (vArg0)
            {
                mOrganizationsSelected = mOrgSelectionItems.Count;
            }
            EnableTeamPanel();

        }

        /// <summary>
        /// Enables the team panel depending on the state of the organization panel. 
        /// </summary>
        private void EnableTeamPanel()
        {
            if (mOrganizationsSelected > 0)
            {
                TeamSelectableOption.enabled = true;
            }
            else
            {
                TeamSelectableOption.enabled = false;
                var vItem = Accordion.Items[1];
                Accordion.Close(vItem);
                //disable all team selection
                foreach (var vKv in mTeamSelectionItems)
                {
                    vKv.Value.Toggle.isOn = false;
                }
            }
        }

        /// <summary>
        /// Builds an organization selection item from a given OrganizationListItem model
        /// </summary>
        /// <param name="vOrgItem">the model to build from</param>
        /// <param name="vCallbackAction">the callback action to be invoked on organization item selection</param>
        private void BuildOrganizationOption(OrganizationListItem vOrgItem, Action<OrganizationListItem, bool> vCallbackAction, Action<TeamListItem, bool> vTeamCallbackAction)
        {
            var vNewItemGo = Instantiate(DefaultOrganizationOption);
            vNewItemGo.transform.SetParent(DefaultOrganizationOption.transform.parent, false);
            vNewItemGo.gameObject.SetActive(true);
            vNewItemGo.ListItem = vOrgItem;
            vNewItemGo.OnToggleAction = vCallbackAction;
            vNewItemGo.Toggle.onValueChanged.AddListener(IncrementNumberOfSelectedItems);
             mOrgSelectionItems.Add(vOrgItem.Organization.Name, vNewItemGo);
            var vTeamList = vOrgItem.TeamCollection;
            for (int vI = 0; vI < vTeamList.Count; vI++)
            {
                BuildTeamOption(vTeamList[vI], vTeamCallbackAction);
            }
        }

        

        /// <summary>
        /// Increment the number of selected organizations
        /// </summary>
        /// <param name="vArg0"></param>
        private void IncrementNumberOfSelectedItems(bool vArg0)
        {
            if (vArg0)
            {
                mOrganizationsSelected++;
            }
            else
            {
                mOrganizationsSelected--;
            }
            EnableTeamPanel();
        }


        /// <summary>
        /// Builds an organization selection item from a given OrganizationListItem model
        /// </summary>
        /// <param name="vTeamItem">the model to build from</param>
        /// <param name="vCallbackAction">the callback action to be invoked on team item selection</param>
        private void BuildTeamOption(TeamListItem vTeamItem, Action<TeamListItem, bool> vCallbackAction)
        {
            var vNewItemGo = Instantiate(DefaultTeamItem);
            vNewItemGo.transform.SetParent(DefaultTeamItem.transform.parent, false);
            vNewItemGo.gameObject.SetActive(true);
            vNewItemGo.ListItem = vTeamItem;
            vNewItemGo.OnToggleAction = vCallbackAction;
            mTeamSelectionItems.Add(vTeamItem.GetName, vNewItemGo);
        }

        /// <summary>
        /// Add a team item
        /// </summary>
        /// <param name="vTeamItem">the team item to add</param>
        /// <param name="vCallbackAction">the callback action to be invoked on team item selection</param>
        public void AddTeamItem(TeamListItem vTeamItem, Action<TeamListItem, bool> vCallbackAction)
        {
            BuildTeamOption(vTeamItem, vCallbackAction);
        }

        /// <summary>
        /// Adds an organization item
        /// </summary>
        /// <param name="vOrgItem">the org item to add</param>
        /// <param name="vCallbackAction">the callback action to be invoked on organization item selection</param>
        public void AddOrgItem(OrganizationListItem vOrgItem, Action<OrganizationListItem, bool> vCallbackAction, Action<TeamListItem, bool> vTeamCallbackAction)
        {
            BuildOrganizationOption(vOrgItem, vCallbackAction, vTeamCallbackAction);
        }

        /// <summary>
        /// Removes a team list item
        /// </summary>
        /// <param name="vTeamItem">the team item to remove</param>
        public void RemoveTeamItem(TeamListItem vTeamItem)
        {
            var vItem = mTeamSelectionItems[vTeamItem.GetName];
            mTeamSelectionItems.Remove(vTeamItem.GetName);
            Destroy(vItem.gameObject);
        }
        /// <summary>
        /// Removes a organization list item and its associated team
        /// </summary>
        /// <param name="vOrgItem">the organization item to remove</param>
        public void RemoveOrganizationItem(OrganizationListItem vOrgItem)
        {
            var vItem = RemoveTeamItemFromList(vOrgItem);
            mOrgSelectionItems.Remove(vOrgItem.Organization.Name);
            Destroy(vItem.gameObject);
        }

        /// <summary>
        /// Removes all the team items for a specific organization
        /// </summary>
        /// <param name="vOrgItem">the organization item</param>
        public OrganizationSelectionItem RemoveTeamItemFromList(OrganizationListItem vOrgItem)
        {
            var vItem = mOrgSelectionItems[vOrgItem.Organization.Name];
            var vTeamList = vItem.ListItem.TeamCollection;
            for (int vI = 0; vI < vTeamList.Count; vI++)
            {
                RemoveTeamItem(vTeamList[vI]);
            }
            return vItem;
        }
    }
}