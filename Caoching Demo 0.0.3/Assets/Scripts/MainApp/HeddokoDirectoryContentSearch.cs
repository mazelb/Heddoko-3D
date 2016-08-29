// /**
// * @file HeddokoDirectoryContentSearch.cs
// * @brief Contains the HeddokoDirectoryContentSearch class
// * @author Mohammed Haider( mohammed@heddoko.com) 
// * @date August 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Assets.Scripts.MainApp
{

    public delegate void FoundFileList(List<FileInfo> vFileInfos);
    /// <summary>
    /// Searches a
    /// </summary>
    public class HeddokoDirectoryContentSearch
    {

        /// <summary>
        /// A list of files that are forbbiden from being included in the search
        /// </summary>
        private List<string> mForbiddenFiles = new List<string>();
        private DirectoryInfo mRootDir;
        private string mSearchPattern;
        private List<FileInfo> mFoundFiles = new List<FileInfo>();
        public event FoundFileList FoundFileListEvent;
         
        /// <summary>
        /// Paramaterized constructor, accepting a directory info, a search pattern and a list of forbidden files to avoid including the final search result list
        /// </summary>
        /// <param name="vDirInfo">The location of the sd card</param>
        /// <param name="vSearchPattern">the search pattern.<seealso cref="https://msdn.microsoft.com/en-us/library/wz42302f(v=vs.110).aspx"/> </param>
        /// <param name="vForbiddenList"></param>
        public HeddokoDirectoryContentSearch(DirectoryInfo vDirInfo ,string vSearchPattern, List<string> vForbiddenList = null)
        {
            if (vDirInfo == null)
            {
                throw new NullReferenceException("DirectoryInfo param cannot be null");
            }

            else
            {
                mRootDir = vDirInfo;
            }

            if (vForbiddenList != null)
            {
                mForbiddenFiles = vForbiddenList;
            }
           
            mSearchPattern = vSearchPattern;
            
        }

        /// <summary>
        /// Searches and populates a list of found files
        /// </summary>
        public void Search()
        {
            mFoundFiles = GetFileInfoList(mRootDir);
            FilterFileInfo(ref mFoundFiles);
            if (FoundFileListEvent != null)
            {
                FoundFileListEvent(mFoundFiles);
            }
        }

        /// <summary>
        /// Filter a list from all forbidden files
        /// </summary>
        /// <param name="vFileInfos"></param>
        public void FilterFileInfo(ref List<FileInfo> vFileInfos)
        {
              vFileInfos.RemoveAll(vX => mForbiddenFiles.Contains(vX.Name));
        }

        /// <summary>
        /// retrieve a list of file information, recursively searches
        /// </summary>
        /// <param name="vRootDir"></param>
        /// <param name="vFileInfos"></param>
        /// <returns></returns>
        public List<FileInfo> GetFileInfoList(DirectoryInfo vRootDir)//, ref List<FileInfo> vFileInfos)
        {
            var vDirectories = vRootDir.GetDirectories().ToList();
            vDirectories.RemoveAll(vDir => vDir.Name.Equals("System Volume Information"));
            var vFileInfos  = new List<FileInfo>();
            if (vDirectories.Count == 0)
            {
                vFileInfos.AddRange(SimpleScan(vRootDir));
                return vFileInfos;
            }
            else
            {

                foreach (var vDirectoryInfo in vDirectories)
                {
                   vFileInfos.AddRange(GetFileInfoList(vDirectoryInfo));
                }
            }
            return vFileInfos;
        }
 
        /// <summary>
        /// does a simple scan on the directory and returns a list of all files within the search pattern defined in the class
        /// </summary>
        /// <param name="vInfo">The directory to retrieve files from</param>
        /// <returns></returns>
        public List<FileInfo> SimpleScan(DirectoryInfo vInfo)
        {
            var vFiles = vInfo.GetFiles(mSearchPattern).ToList(); 
            return vFiles;
        }
 
    }

    
}