using System;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using Assets.Scripts.Licensing.Model;
using Assets.Scripts.MainApp;
using Assets.Scripts.UI.RecordingLoading.Model;
using Assets.Scripts.UI.RecordingLoading.View; 
using Assets.Scripts.Utils;
using HeddokoSDK.Models;

namespace Assets.Scripts.UI.RecordingLoading
{
    public delegate void DownloadCompleted(BaseModel vHedAsset, ref RecordingListItem vItem);

    public delegate void ErrorDownloadingException(Exception vException);

    /// <summary>
    /// A file fetcher, fetching files from heddoko servers. Invokes two events: a <see cref="DownloadCompleted"/>DownloadCompleted and <see cref="ErrorDownloadingException"/> ErrorDownloadingException. 
    /// Register for these events in order in case you are interested. 
    /// </summary>
    public class HeddokoDownloadFetcher
    {
        public string CacheDirectory;  

        public DownloadCompleted DownloadCompletedHandler;
        public ErrorDownloadingException ErrorDownloadingExceptionHandler;
        public IUserProfileManager mProfileManager { get; set; }


        public HeddokoDownloadFetcher(IUserProfileManager vManager)
        {
            mProfileManager = vManager;
        }
        /// <summary>
        /// Fetches data from heddoko servers through a client. The callback structure needs to be of type DataFetchingStructure<see cref="DataFetchingStructure"/>
        /// </summary>
        /// <param name="vCallbackStruct"></param>
        public void FetchData(object vCallbackStruct)
        {
            BaseModel vHedAsset = null;
            try
            {
                DataFetchingStructure vStructure = (DataFetchingStructure)vCallbackStruct;
                vHedAsset = mProfileManager.UserProfile.Client.DownloadFile(vStructure.Item.Location.RelativePath, vStructure.DownloadLocation); 
                if (DownloadCompletedHandler != null)
                {
                    DownloadCompletedHandler(vHedAsset, ref vStructure.Item);
                }
            }
            catch (Exception vE)
            {
                if (ErrorDownloadingExceptionHandler != null)
                {
                    ErrorDownloadingExceptionHandler(vE);
                }
            }
        }
    }
    public struct DataFetchingStructure
    {
        public string DownloadLocation;
        public RecordingListItem Item;
    }
}