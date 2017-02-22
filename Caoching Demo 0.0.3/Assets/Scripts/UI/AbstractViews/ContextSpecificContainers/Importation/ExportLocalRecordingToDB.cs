/** 
* @file ExportLocalRecordingToDB.cs
* @brief Contains the ExportLocalRecordingToDB class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
 
using System.Collections.Generic;
using Assets.Scripts.Communication.DatabaseConnectionPipe;
using Assets.Scripts.UI.AbstractViews.SelectableGridList;
using Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI; 


namespace Assets.Scripts.UI.AbstractViews.ContextSpecificContainers.Importation
{
    /// <summary>
    /// A container view for loading items from a database
    /// </summary>
    public class ExportLocalRecordingToDB : AbstractView, IDatabaseConsumer
    {
        public ExportViewSelectableGridList GridList;
        public Button OpenFolderButton;
        private FolderScanner mFolderScanner = new FolderScanner();
        public ExportItemsToDb ExportItem;
        //private ImportToDatabase6+++++++++++++++++++++++++
        public int NumberOfItems = 200;
        
      
        void Awake()
        {
            CreateDefaultLayout();
        }

        public Database Database { get; set; }


        public override void CreateDefaultLayout()
        {
            GridList.Initialize();
            OpenFolderButton.onClick.AddListener(OpenSelectFolderDialog); 

        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public void OpenSelectFolderDialog()
        {
            mFolderScanner.DirectoryScanCompleted += OnScanCompletion;
            // to work around uni file to only filter folders , add a nonsensical file extension
            UniFileBrowser.use.filterFileExtensions = new[] { "som3rAN66Ddhum3cks2Tension38F" };
            UniFileBrowser.use.filterFiles = true;
            UniFileBrowser.use.showVolumes = true;
            UniFileBrowser.use.volumesAreSeparate = false;
            UniFileBrowser.use.OpenFolderWindow(true, mFolderScanner.ScanDirectory);
        }

        private void OnScanCompletion(List<RecordingItemDescriptor> vDescriptors)
        {
            mFolderScanner.DirectoryScanCompleted -= OnScanCompletion;
            GridList.LoadData(vDescriptors);
        }

        public void ExportItems()
        {
             if (GridList.SelectedItems.Count > 0)
            {
                GameObject vDialog = Dialog.Template(ExportItem.TemplateName).Show("Import File Progress");
                
                ExportItem = vDialog.GetComponent<ExportItemsToDb>();
                ExportItem.Database = Database;
                ExportItem.InitiateImport(GridList.SelectedItems);
                ExportItem.GridList = GridList;
            }



        }



    }
}
