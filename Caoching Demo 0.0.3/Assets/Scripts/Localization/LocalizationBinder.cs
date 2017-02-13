// /**
// * @file LocalizationBinder.cs
// * @brief Contains the LocalizationBinder class
// * @author Mohammed Haider( 
// * @date 09 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;

namespace Assets.Scripts.Localization
{

    public class LocalizationBinder
    {

        public Dictionary<KeyMessage, LocalizedString> StringContainer = new Dictionary<KeyMessage, LocalizedString>();

        /// <summary>
        /// Returns  a localized string from the binder.Note, if not found, null will be returned.
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="vIsPlural"></param>
        /// <returns></returns>
        public string GetString(KeyMessage vKey, bool vIsPlural = false)
        {
            if (!StringContainer.ContainsKey(vKey))
            {
                return "NULL";
            }
            else
            {
                return StringContainer[vKey].GetString(vIsPlural);
            }
        }
    }

    public enum KeyMessage
    {
        Greet,
        UploadCompleteMsg,
        DownloadCompleteMsg,
        NewRecordingFoundMsg,
        BrainpackServiceMsgTimedOut,
        BannedAccountMsg,
        InactiveAccountMsg,
        InactiveLicenseMsg,
        ExpiredLicenseMsg,
        DeletedLicenseMsg,
        AnalysisCollectionMsg,
        AnalysisSaveMsg,
        NumberOfExportedItemsMsg,
        NoItemsWereExportedMsg,
        NumberOfBatteryPacksFoundMsg,
        NoBatteryPacksFoundMsg,
        BeginUploadProcessMsg,
        DisconnectedSDCardWithLogMsg,
        IssueUploadingRecordingsMsg,
        NewRecFoundSyncBeginMsg,
        LoginFailureMsg,
        TopLabelLogindexOpeningErrMsg,
        LogindexOpeningErrMsg,
        GenericError,
        ReportErrorCodeMsg,
        InvalidUnPwMsg,
        IssueAccessingAcountGenericMsg,
        AckErrorMsg,
        NoInternetConnectionMsg,
        RecordingFileLessThanSkippableFrames,
        BadKitUpload1,
        BadKitUpload2,
        CannotLoadRecording,
        MessageFromServerReceived,
        ReturnToLoginScreen,
        NoAnalysisFrameSet
    }
}