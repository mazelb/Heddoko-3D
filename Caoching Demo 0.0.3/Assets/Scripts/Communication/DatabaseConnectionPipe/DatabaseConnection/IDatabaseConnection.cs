
/** 
* @file IDatabaseConnection.cs
* @brief Contains the IDatabaseConnection interface
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors;
using Assets.Scripts.UI.Tagging;

namespace Assets.Scripts.Communication.DatabaseConnectionPipe.DatabaseConnection
{
    /// <summary>
    /// Provides an interface for database connections 
    /// </summary>
    public interface IDatabaseConnection : IEquatable<IDatabaseConnection>
    {
        /// <summary>
        /// Unique id
        /// </summary>
        string DbConnectionUuid { get; }

        /// <summary>
        /// Connect to the database
        /// </summary>
        /// <param name="vCallback">(optional)Action to invoke on result of the connection</param> 
        bool Connect(Action vCallback = null);

        /// <summary>
        /// Disconnect from the database
        /// </summary>
        /// <param name="vCallback">(optional) action to invoke after disconnection</param>

        void Disconnect(Action vCallback = null);
        /// <summary>
        /// Allow a loop to break when set to false
        /// </summary>
        bool ContinueWorking { get; set; }
        /// <summary>
        /// Query the database
        /// </summary>
        /// <param name="vQuery">Query to make to the database </param>
        /// <param name="vCallback">optional callback on results</param>
        /// <returns></returns>
        bool Query(string vQuery, Action vCallback = null);
        /// <summary>
        /// Creates a recording entry on the database
        /// </summary>
        /// <param name="vRecording"></param>
        /// <returns></returns>
        bool CreateRecording(BodyFramesRecording vRecording);

        /// <summary>
        /// Creates a recording entry on the database from the given parameters. Invokes a callback with regards to progress
        /// </summary>
        /// <param name="vRecording">the recording to import</param>
        /// <param name="vDescriptor">the recording's description</param>
        /// <param name="vTotalImportProgress">callback to update progress</param>
        /// <returns></returns>
        bool CreateRecording(BodyFramesRecording vRecording, RecordingItemDescriptor vDescriptor,
            Action<int> vTotalImportProgress);

        /// <summary>
        /// get a recording based on the id passed in
        /// </summary>
        /// <param name="vRecordingId">the recording id</param>
        /// <returns></returns>
        BodyFramesRecording GetRawRecording(string vRecordingId);

        Tag GetTagById(string vUid);
        void AddNewTag(Tag vTag);

        /// <summary>
        /// Get a list of tags current attached to this recording
        /// </summary>
        /// <param name="vRecGuid">the recording guid from which to retrieve the tag from</param>
        /// <returns></returns>
        List<Tag> GetTagsOfRecording(string vRecGuid);


        /// <summary>
        /// Attaches a tag to a recording
        /// </summary>
        /// <param name="vRec">the recording to add a tag to</param>
        /// <param name="vTag">the tag to attach</param>
        void AddTagToRecording(BodyFramesRecording vRec, Tag vTag);

        /// <summary>
        /// Loads all tags from the database. 
        /// </summary>
        /// <returns></returns>
        List<Tag> LoadAllTags();

        /// <summary>
        /// Retrieve a list of recording guids that have been tagged 
        /// </summary>
        /// <param name="vTagId"></param>
        /// <returns></returns>
        List<string> GetRecordingGuidFromTagId(string vTagId);

        /// <summary>
        /// Update the the folder a recording is in 
        /// </summary>
        /// <param name="vRecordingUid"></param>
        /// <param name="vFolderUid"></param>
        /// <returns></returns>
        bool UpdateRecordingFolder(string vRecordingUid, string vFolderUid);

        /// <summary>
        /// Get a list of tags by partial title
        /// </summary>
        /// <param name="vTitleKey"></param>
        /// <param name="vTotalResults">(Optional) Limit the number of results</param>
        /// <returns></returns>
        List<Tag> GetTagsByPartialTitle(string vTitleKey, int vTotalResults = 0);

        /// <summary>
        /// Get a list of tags by partial title but excluding the tag list
        /// </summary>
        /// <param name="vTitleKey"></param>
        /// <param name="vExcludingTags"></param>
        /// <param name="vTotalResults">(Optional) Limit the number of results to pass back</param>
        /// <returns></returns>
        List<Tag> GetTagsByPartialTitleExcludingList(string vTitleKey, List<Tag> vExcludingTags, int vTotalResults = 0);
        /// <summary>
        /// Sanitize input string
        /// </summary>
        /// <param name="vInput">the input to sanitize</param>
        string SanitizeInput(string vInput);

        List<RecordingItemDescriptor> GetRecordingDescriptions(string vBodyGuid, List<Tag> vTagFilter);


        /// <summary>
        /// Get tag by title
        /// </summary>
        /// <param name="vTitle">the tag title</param>
        /// <returns></returns>
        Tag GetTagByTitle(string vTitle);
    }
}
