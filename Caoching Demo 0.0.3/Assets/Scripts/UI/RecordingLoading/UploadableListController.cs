// /**
// * @file UploadableListController.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 08 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.MainApp;
using Assets.Scripts.UI.RecordingLoading.Model;
using Assets.Scripts.UI.RecordingLoading.View;
using HeddokoSDK.Models;
using UnityEngine;
 
namespace Assets.Scripts.UI.RecordingLoading
{
    public class UploadableListController : MonoBehaviour
    {
        [SerializeField]
        private bool mScanSubDirectories;
        private List<UploadableListItem> mUploadableList = new List<UploadableListItem>(); 
        public UploadableListSyncView UploadableListSyncView;
        private string mSelectedDirectory = "";
        private UserProfileModel mProfile;
        private AssetUploader mUploader;
        public void StartDirectorySelection()
        {
            UniFileBrowser.use.OpenFolderWindow(false, SelectDirectory);
            UniFileBrowser.use.limitToInitialFolder = false;
            UniFileBrowser.use.showVolumes = true;
 
        }

        void Awake()
        {
            mUploader = new AssetUploader(Profile);
        }
        public void SelectDirectory(string vDir)
        {
            mSelectedDirectory = vDir;
        }

        public void BeginScan()
        {
            if (!string.IsNullOrEmpty(mSelectedDirectory))
            {
                ScanDirectory(new DirectoryInfo(mSelectedDirectory));

            }
        }
        public UserProfileModel Profile
        {
            get
            {
                if (mProfile == null)
                {
                    mProfile = UserSessionManager.Instance.UserProfile;
                }
                return mProfile;
            }
        }
        public void ScanDirectory(DirectoryInfo vInfo)
        {
            List<FileInfo> vInforinos = new List<FileInfo>();
            GetFileInfoList(vInfo, ref vInforinos);
            FilterFileInfo(ref vInforinos);
            PopulateList(vInforinos);
        }

        /// <summary>
        /// Filter files to not include logindex.dat.
        /// </summary>
        /// <param name="vFileInfos"></param>
        public void FilterFileInfo(ref List<FileInfo> vFileInfos)
        {
            //remove files, use a stack to insert largest indices last.
            Stack<int> vIndices = new Stack<int>();
            for (int i = 0; i < vFileInfos.Count; i++)
            {
                if (vFileInfos[i].Name.Contains("logIndex.dat")|| vFileInfos[i].Name.Contains("logindex.dat"))
                {
                    vIndices.Push(i);
                }
            }
            while (vIndices.Count > 0)
            {
                int vIndex = vIndices.Pop();
                vFileInfos.RemoveAt(vIndex);
            }
        }

        /// <summary>
        /// Recursively search for files from a given rootdirectory and add it to a list of fileinfo
        /// </summary>
        /// <param name="vRootDir">the root directory to begin search</param>
        /// <param name="vFileInfos">The referenced list of FileInfo to add to.</param>
        /// <returns></returns>
        public List<FileInfo> GetFileInfoList(DirectoryInfo vRootDir, ref List<FileInfo> vFileInfos)
        {
            var vDirectories = vRootDir.GetDirectories();
            if (vDirectories.Length == 0)
            {
                vFileInfos.AddRange(SimpleScan(vRootDir));
                return vFileInfos;
            }
            else
            {

                foreach (var vDirectoryInfo in vDirectories)
                {

                    return GetFileInfoList(vDirectoryInfo, ref vFileInfos);
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vInfo"></param>
        void PopulateList(List<FileInfo> vInfo)
        {
            mUploadableList = new List<UploadableListItem>(vInfo.Count);
            foreach (var vFileInfo in vInfo)
            {
                mUploadableList.Add(new UploadableListItem()
                {
                    FileName = vFileInfo.Name,
                    RelativePath = vFileInfo.FullName,
                    IsNew = true,
                    AssetType = AssetType.Record
                }
                    );
            }
            UploadableListSyncView.LoadData(mUploadableList);
        }

        public void StartUpload()
        {
            if (Profile.User.Kit != null)
            {
                foreach (var vUploadable in mUploadableList)
                {
                    UploadSingleRecording(vUploadable);
                }
            }

        }

        
        /// <summary>
        /// upload a single recording
        /// </summary>
        /// <param name="vItem"></param>
        void UploadSingleRecording(UploadableListItem vItem)
        {
            ThreadPool.QueueUserWorkItem(mUploader.UploadSingleItem, vItem);
        }

        public List<FileInfo> SimpleScan(DirectoryInfo vInfo)
        {
            var vFiles = vInfo.GetFiles("*.dat").ToList();
#if DEBUG
            vFiles.AddRange(vInfo.GetFiles("*.csv").ToList());
#endif
            return vFiles;
        }
        public void ToggleScanSubDirectories()
        {
            mScanSubDirectories = !mScanSubDirectories;
        }
        public void Reset()
        {
        }
    }
}