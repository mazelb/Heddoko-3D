
/** 
* @file ExportItemsToDb.cs
* @brief Contains the ExportItemsToDb class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System;
using System.Collections.Generic; 
using System.IO; 
using System.Threading;
using Assets.Scripts.Communication.DatabaseConnectionPipe;
using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.MainApp;
using Assets.Scripts.UI.AbstractViews.SelectableGridList;
using Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors;
using Assets.Scripts.UI.Tagging;
using Assets.Scripts.Utils.DatabaseAccess;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.UI.AbstractViews.ContextSpecificContainers.Importation
{
    /// <summary>
    /// Imports an item to the database and updates the view with the current progress
    /// </summary>
    public class ExportItemsToDb : MonoBehaviour, IDatabaseConsumer, ITemplatable
    {
        public Database Database { get; set; }
        private bool mIsCancelled;
        public Progressbar TotalProgress;
        public Progressbar CurrentFileProgress;
        public Text TotalProgressText;
        public Text CurrentFileProgressText;
        private List<ImportTaskStructure> mTotalExportedItems;
        private Stack<RecordingItemDescriptor> mItemStack;
        private bool mIsImporting = false;
        public Button CancelButton;
        public Button CloseButton;
        public ImportTaskStructure CurrentImportTask;
        private int mMaxItems;
        private string mTemplateName = "ImportProgressDialog";
        private bool mIsInitialized = false;
        public HeddokoAppStart AppStarter;
        public  ExportViewSelectableGridList GridList { get; set; }
        public TaggingManager mTaggingManager;
        public bool IsTemplate { get; set; }

        public string TemplateName
        {
            get { return mTemplateName; }
            set { mTemplateName = value; }
        }

        public TaggingManager TaggingManager
        {
            get
            {
                if (mTaggingManager == null)
                {
                    mTaggingManager = AppStarter.TaggingManager;
                }
                return mTaggingManager;
            }
            
        }

        private void Initialize()
        {
            mIsCancelled = false;
            if (!mIsInitialized)
            {
                mIsInitialized = true;
                CurrentFileProgressText = null;
                CurrentFileProgress = null;
                TotalProgressText = null;
                TotalProgress = null;
                CancelButton = null;
                CloseButton = null;
                Progressbar[] vBars = GetComponentsInChildren<Progressbar>();
                Text[] vTexts = GetComponentsInChildren<Text>();
                Transform vButtonsContainer = transform.FindChild("ButtonsContainer");
                CancelButton = vButtonsContainer.transform.FindChild("CancelButton").GetComponent<Button>();
                CloseButton = vButtonsContainer.transform.FindChild("CloseButton").GetComponent<Button>();
                CancelButton.onClick.AddListener(Cancel);
                CloseButton.onClick.AddListener(Hide);
                // Button[] vButtons = vButtonsContainer.GetComponentsInChildren<Button>();
                foreach (var vBar in vBars)
                {
                    if (vBar.name.Contains("Total File progress bar"))
                    {
                        TotalProgress = vBar;
                        continue;
                    }
                    if (vBar.name.Contains("Current File Progress bar"))
                    {
                        CurrentFileProgress = vBar;
                        continue;
                    }
                }
                foreach (var vText in vTexts)
                {
                    if (TotalProgressText ==null && 
                        vText.gameObject.name.Contains("Total file progress text"))
                    {
                        TotalProgressText = vText;
                        continue;
                    }

                    if (CurrentFileProgressText == null 
                        && vText.gameObject.name.Contains("Current File Progress text"))
                    {
                        CurrentFileProgressText = vText;
                        continue;
                    }

                    if (TotalProgressText != null && CurrentFileProgressText != null)
                    {
                        break;
                    }
                }
             
            }

        }

      
        public void InitiateImport(List<RecordingItemDescriptor> vSelectedItems)
        {
            Initialize();
            if (mIsImporting)
            { 
            }
            if (vSelectedItems == null || vSelectedItems.Count == 0)
            {
                //bad item
                Debug.Log("invalid number of items to record");
                Hide();
                return;
            }
            else
            {
                if (mIsCancelled)
                {
                    return;
                }
                Show();
                mIsImporting = true;
                mTotalExportedItems = new List<ImportTaskStructure>(vSelectedItems.Count);
                mItemStack = new Stack<RecordingItemDescriptor>();
                for (int i = vSelectedItems.Count - 1; i >= 0; i--)
                {
                    mItemStack.Push(vSelectedItems[i]);
                }
                Database.Connection.ContinueWorking = true;
                mMaxItems = TotalProgress.Max = mItemStack.Count;
                TotalProgressText.text = "Number of movements imported: " + mTotalExportedItems.Count + "/" + vSelectedItems.Count;
                Export(); 
            }


        }
        /// <summary>
        /// Start the import process, will be called back after the progress bar has been updated. 
        /// the function will check if there are any remaining items to import, will import them one by one.
        /// </summary>
        private void Export()
        {
            if (mIsCancelled)
            {
                return;
            }
            //all items have been imported
            if (mItemStack.Count == 0 && mTotalExportedItems.Count > 0)
            {
                CurrentFileProgress.Stop();
                //items have finished importing
                FinishedImporting();
                return;
            }
            CurrentFileProgress.Stop();
            CurrentFileProgress.Value = 0;
            //get first recording
            RecordingItemDescriptor vCurrItemDescriptor = mItemStack.Peek();
            string vPath = vCurrItemDescriptor.FilePath;
            BodyRecordingsMgr.Instance.ReadRecordingFile(vPath, RecordingAddCallback);
        }

        /// <summary>
        /// When  all items have been imported, this gets triggered
        /// </summary>
        private void FinishedImporting()
        {
            int vTotalImportCount = mTotalExportedItems.Count;
            //remove items that were succesfully imported
            for (int i = 0; i < vTotalImportCount; i ++)
            {
                GridList.DataSource.Remove(mTotalExportedItems[i].ItemDescriptor);
            }
            CloseButton.gameObject.SetActive(true);
            CancelButton.gameObject.SetActive(false);
            mIsImporting = false;
            //send notification that import has been completed
    
            var message = string.Format("{0} movements have been exported ", vTotalImportCount); 
            Notify.Template("FadingFadoutNotifyTemplate").Show(message, 4.5f, hideAnimation :  Notify.FadeOutAnimation, showAnimation: Notify.FadeInAnimation, sequenceType: NotifySequence.First );

        }

      
        /// <summary>
        /// callback performed after a recording file has been read
        /// </summary>
        /// <param name="vRecording">A recording and it's information</param>
        private void RecordingAddCallback(BodyFramesRecordingBase vRecording)
        {
            //remove the first item from the stack
            RecordingItemDescriptor vCurrItemDescriptor = mItemStack.Pop();
            CurrentImportTask = new ImportTaskStructure() { CurrentProgressIndex = 0, ItemDescriptor = vCurrItemDescriptor, Recording = vRecording };
            //check if item needs to be deleted
            if (CurrentImportTask.ItemDescriptor.IsMarkedForDeletion)
            {
           
                try
                {
                    Debug.Log("Delete item from disk: "+ CurrentImportTask.ItemDescriptor.FilePath);
                    File.Delete(CurrentImportTask.ItemDescriptor.FilePath);
                    GridList.DataSource.Remove(CurrentImportTask.ItemDescriptor);
                }
                catch (Exception e)
                {
                    Debug.Log("Exception e");
                }
            
            }
            //set the max progress value 
            CurrentFileProgress.Max = CurrentImportTask.MaxCount;
            mTotalExportedItems.Add(CurrentImportTask);
            Thread vNextThread = new Thread(() => Database.Connection.CreateRecording(vRecording, vCurrItemDescriptor, UpdateCurrentFileProgress));
            TaggingManager.AttachTagSetToRecording(vCurrItemDescriptor.TagSet,vRecording);
            vNextThread.Start();
        }

        private void OnApplicationQuit()
        {
            if (Database != null)
            {
                Database.Connection.ContinueWorking = false;
            }
        }
        public void UpdateCurrentFileProgress(int vIndexOfConversion)
        {
            if (mIsCancelled)
            {
                return;
            }
            CurrentImportTask.CurrentProgressIndex = vIndexOfConversion;
            if (CurrentFileProgress.IsAnimationRun)
            {
                CurrentFileProgress.Stop();
            }
            CurrentFileProgress.Animate(vIndexOfConversion + 1);
            CurrentImportTask.CurrentProgressIndex = vIndexOfConversion;
            //check if the current item has completed
            if (CurrentImportTask.CurrentProgressIndex == CurrentImportTask.MaxCount - 1)
            {  
                //update total progress and move to next item
                CurrentFileProgress.Stop();
                CurrentFileProgress.Value = CurrentFileProgress.Max;
                UpdateProgress();

            }

        }
        /// <summary>
        /// Update progress
        /// </summary> 
        private void UpdateProgress()
        {
            TotalProgressText.text = "Number of movements imported: " + mTotalExportedItems.Count + "/" + mMaxItems;
            TotalProgress.Animate(mTotalExportedItems.Count);
            //pass to Export, it will check if there are other items that need to be imported
            Export();
        }



        /// <summary>
        /// Cancels and immediately set the import state to finished importing
        /// </summary>
        public void Cancel()
        {
            Database.Connection.ContinueWorking = false;
            TotalProgress.Stop();
            CurrentFileProgress.Stop();
            Debug.Log("Cancel initiated");
            mIsImporting = false;
            FinishedImporting();
            mIsCancelled = true;
        }
        public void Hide()
        {
            if (TotalProgress.IsAnimationRun)
            {
                TotalProgress.Stop();
                TotalProgress.Value = 0;
            }
            gameObject.SetActive(false);
            mIsImporting = false;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            TotalProgress.Value = 0;
            CloseButton.gameObject.SetActive(false);
            CancelButton.gameObject.SetActive(true);

        }

        public struct ImportTaskStructure
        {
            public BodyFramesRecordingBase Recording;
            public RecordingItemDescriptor ItemDescriptor;
            public int CurrentProgressIndex;

            public int MaxCount
            {
                get
                {
                    return Recording.RecordingRawFramesCount;
                }
            }
        }

    
    }
}