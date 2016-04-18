/** 
* @file ImportViewSelectableListModifier.cs
* @brief Contains the ImportViewSelectableListModifier class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.ContextMenuSubControl.ConcreteImpletors;
using Assets.Scripts.UI.AbstractViews.ContextSpecificContainers.Importation;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.SelectableGridList
{
    public class ImportViewSelectableListModifier
    {

        public ExportLocalRecordingToDB RecordingsExporter;
        private RightClickButtonStructure mRenameItemsStructure;
        private RightClickButtonStructure mAddTagToItemsStructure;
        private RightClickButtonStructure mRemoveAllTagsStructure;
        private RightClickButtonStructure mMarkOrUnmarkDeletionToItemsStructure;
        private RightClickButtonStructure mMarkSelectedRecordingsForDeletion;
        private RightClickButtonStructure mUnMarkSelectedRecordingsForDeletion;
        private RightClickButtonStructure mClearSelectionStructure;
        private RightClickButtonStructure mExportRecordingStructure;

        private List<RightClickButtonStructure> mSingleStructContainer = new List<RightClickButtonStructure>(4);

        private List<RightClickButtonStructure> mMultipleStructContainer = new List<RightClickButtonStructure>(5);
        /// <summary>
        /// Will return a list of button structures
        /// Note: will return null if there are no elements selected
        /// </summary>
        public List<RightClickButtonStructure> StructContainer
        {
            get
            {
                List<RightClickButtonStructure> vContainer = null;
                int vCount = RecordingsExporter.GridList.SelectedItems.Count;
                //first check how many elements are selected 
                if (vCount == 1)
                {
                    vContainer = mSingleStructContainer;
                    mRenameItemsStructure.Info = "Rename movement";
                    mAddTagToItemsStructure.Info = "Add Tag to movement";
                    mRemoveAllTagsStructure.Info = "Remove all tags from movement";
                    bool vIsMarkedForDeletion = RecordingsExporter.GridList.SelectedItems[0].IsMarkedForDeletion;
                    if (vIsMarkedForDeletion)
                    {
                        mMarkOrUnmarkDeletionToItemsStructure.Info = "Unmark movement for deletion";
                        mMarkOrUnmarkDeletionToItemsStructure.CallbackAction = UnMarkedSelectedItemsForDeletion;
                    }
                    else
                    {
                        mMarkOrUnmarkDeletionToItemsStructure.Info = "Mark movement for deletion";
                        mMarkOrUnmarkDeletionToItemsStructure.CallbackAction = MarkSelectedItemsForDeletion;
                    }
                    mExportRecordingStructure.Info = "Export movement";

                }
                else if (vCount > 1)
                {
                    vContainer = mMultipleStructContainer;
                    mRenameItemsStructure.Info = "Rename movements";
                    mAddTagToItemsStructure.Info = "Add tag to selected movements";
                    mRemoveAllTagsStructure.Info = "Remove all tags from selected movement";
                    mMarkSelectedRecordingsForDeletion.Info = "Mark selected movements for deletion";
                    mUnMarkSelectedRecordingsForDeletion.Info = "Unmark selected movements for deletion";
                    mExportRecordingStructure.Info = "Export selected movements";
                }
                return vContainer;
            }
        }

        public void Init()
        {
            mRenameItemsStructure = new RightClickButtonStructure();
            mRenameItemsStructure.CallbackAction = RenameItems;
            mAddTagToItemsStructure = new RightClickButtonStructure();
            mAddTagToItemsStructure.CallbackAction = AddTagToItems;
            mMarkOrUnmarkDeletionToItemsStructure = new RightClickButtonStructure();
            mExportRecordingStructure = new RightClickButtonStructure();
            mExportRecordingStructure.CallbackAction = ExportItems;
            mRemoveAllTagsStructure = new RightClickButtonStructure();
            mRemoveAllTagsStructure.CallbackAction = RemoveAllTags;
            mMarkSelectedRecordingsForDeletion = new RightClickButtonStructure();
            mMarkSelectedRecordingsForDeletion.CallbackAction = MarkSelectedItemsForDeletion;
            mUnMarkSelectedRecordingsForDeletion = new RightClickButtonStructure();
            mUnMarkSelectedRecordingsForDeletion.CallbackAction = UnMarkedSelectedItemsForDeletion;
            mClearSelectionStructure = new RightClickButtonStructure();
            mClearSelectionStructure.Info = "Clear Selection";
            mClearSelectionStructure.CallbackAction = ClearSelection;

            mSingleStructContainer.Add(mRenameItemsStructure);
            mSingleStructContainer.Add(mAddTagToItemsStructure);
            mSingleStructContainer.Add(mRemoveAllTagsStructure);
            mSingleStructContainer.Add(mMarkOrUnmarkDeletionToItemsStructure);
            mSingleStructContainer.Add(mExportRecordingStructure);
            mSingleStructContainer.Add(mClearSelectionStructure);

            mMultipleStructContainer.Add(mRenameItemsStructure);
            mMultipleStructContainer.Add(mAddTagToItemsStructure);
            mMultipleStructContainer.Add(mRemoveAllTagsStructure);
            mMultipleStructContainer.Add(mUnMarkSelectedRecordingsForDeletion);
            mMultipleStructContainer.Add(mMarkSelectedRecordingsForDeletion);
            mMultipleStructContainer.Add(mExportRecordingStructure);
            mMultipleStructContainer.Add(mClearSelectionStructure);
        }

        /// <summary>
        /// Renames selected items
        /// </summary>
        private void RenameItems()
        {
            Dialog vDialog = Dialog.Template("DialogSingleInput");
            // helper component with references to input fields
            DialogSingleInputHelper vHelper = vDialog.GetComponent<DialogSingleInputHelper>();
            // reset input fields to default
            vHelper.Refresh();

            // open dialog
            vDialog.Show(
                title: mRenameItemsStructure.Info,
                buttons: new DialogActions(){
					// on click call Update Tag 
					{"Apply", () => RenameItemsCallback(vHelper)},
					// on click close dialog
					{"Cancel", Dialog.Close},
                },
                modal: true,
                modalColor: new Color(0, 0, 0, 0.8f)
            );
        }

        /// <summary>
        /// callback after dialog open. renames all selected items and appends a number immediately after
        /// </summary>
        /// <param name="vHelper"></param>
        /// <returns></returns>
        private bool RenameItemsCallback(DialogSingleInputHelper vHelper)
        {
            if (!vHelper.Validate())
            {
                // return false to keep dialog open
                return false;
            }
            //update tags on all selected items
            int vIndex = 1;
            foreach (var vSelectedItems in RecordingsExporter.GridList.SelectedItems)
            {
                vSelectedItems.MovementTitle = vHelper.SingleInputField.text + "_" + vIndex++;
                vSelectedItems.CreatedAtTime = DateTime.Now;
            }
            RecordingsExporter.GridList.UpdateItems();
            // return true to close dialog
            return true;
        }
        /// <summary>
        /// Clears selection
        /// </summary>
        private void ClearSelection()
        {
            int vSelection = 0;
            while (RecordingsExporter.GridList.SelectedIndicies.Count > 0)
            {
                int vItemCount = RecordingsExporter.GridList.SelectedIndicies.Count;
                vSelection = RecordingsExporter.GridList.SelectedIndicies[vItemCount - 1];
                RecordingsExporter.GridList.Deselect(vSelection);
            }

            RecordingsExporter.GridList.UpdateItems();
        }



        /// <summary>
        /// Add a tag to items
        /// </summary>
        private void AddTagToItems()
        {
            //get the Component of this index
            Dialog dialog = Dialog.Template("DialogSingleInput");
            // helper component with references to input fields
            DialogSingleInputHelper helper = dialog.GetComponent<DialogSingleInputHelper>();
            // reset input fields to default
            helper.Refresh();

            // open dialog
            dialog.Show(
                title: "Add Tag to All Selection",
                buttons: new DialogActions(){
					// on click call Update Tag 
					{"Add", () => AddTagToItemsCallback(helper)},
					// on click close dialog
					{"Cancel", Dialog.Close},
                },
                modal: true,
                modalColor: new Color(0, 0, 0, 0.8f)
            );

        }
        /// <summary>
        /// On the addition of a tag, remove this callback and apply a tag to every item that was selected
        /// </summary>
        private bool AddTagToItemsCallback(DialogSingleInputHelper vHelper)
        {
            if (!vHelper.Validate())
            {
                // return false to keep dialog open
                return false;
            }
            //update tags on all selected items
            foreach (var vSelectedItems in RecordingsExporter.GridList.SelectedItems)
            {
                vSelectedItems.AddTag(vHelper.SingleInputField.text);
            }
            RecordingsExporter.GridList.UpdateItems();
            // return true to close dialog
            return true;
        }

        /// <summary>
        /// Marks the items for deletion
        /// </summary>
        private void MarkSelectedItemsForDeletion()
        {
            foreach (var vItem in RecordingsExporter.GridList.SelectedItems)
            {
                vItem.IsMarkedForDeletion = true;
            }
            RecordingsExporter.GridList.UpdateItems();
        }

        /// <summary>
        /// Unmarks the items for deletion
        /// </summary>
        private void UnMarkedSelectedItemsForDeletion()
        {
            foreach (var vItem in RecordingsExporter.GridList.SelectedItems)
            {
                vItem.IsMarkedForDeletion = false;
            }
            RecordingsExporter.GridList.UpdateItems();
        }

        /// <summary>
        /// Start the import process
        /// </summary>
        private void ExportItems()
        {
            RecordingsExporter.ExportItems();
        }

        /// <summary>
        /// Clears out tags
        /// </summary>
        private void RemoveAllTags()
        {
            foreach (var vVar in RecordingsExporter.GridList.SelectedItems)
            {
                vVar.TagSet.Clear();
            }
            RecordingsExporter.GridList.UpdateItems();

        }

    }
}