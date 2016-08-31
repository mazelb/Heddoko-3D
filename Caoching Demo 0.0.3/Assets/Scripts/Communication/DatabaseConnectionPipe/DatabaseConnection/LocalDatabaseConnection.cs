
/** 
* @file LocalDatabaseConnection.cs
* @brief Contains the LocalDatabaseConnection class, implementing IDatabaseConnection interface
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Assets.Scripts.Frames_Pipeline;
using Assets.Scripts.Frames_Pipeline.BodyFrameConversion;
using Assets.Scripts.Frames_Recorder.FramesRecording;
using Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.UI.Tagging;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.DatabaseAccess;
using UnityEngine;
using JetBrains.Annotations;
using Mono.Data.Sqlite;

namespace Assets.Scripts.Communication.DatabaseConnectionPipe.DatabaseConnection
{
    /// <summary>
    /// A connection to a local database. This connection will persist until the 
    /// </summary>
    public class LocalDatabaseConnection : IDatabaseConnection
    {
        private SqliteConnection mDbConnection;

        public string DbConnectionUuid { get; private set; }
        public LocalDatabaseConnection()
        {
            DbConnectionUuid = Guid.NewGuid().ToString();
        }


        public bool Equals([NotNull] IDatabaseConnection other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            return other.DbConnectionUuid.CompareTo(DbConnectionUuid) == 0;

        }
        public bool Connect(Action vCallback = null)
        {
            bool vReturn = false;
            if (mDbConnection == null)
            {
                //check if the file exists first. 
                string vFilePath = ApplicationSettings.LocalDbPath;
                if (File.Exists(vFilePath))
                {
                    mDbConnection = new SqliteConnection("URI=file:" + vFilePath);
                    mDbConnection.Open();
                    vReturn = true;
                    if (vCallback != null)
                    {
                        vCallback.Invoke();
                    }
                }
            }

            return vReturn;
        }

        /// <summary>
        /// Disconnect from the database
        /// </summary>
        /// <param name="vCallback"></param>
        public void Disconnect(Action vCallback = null)
        {
            try
            {
                mDbConnection.Close();
            }
            catch (SqliteException vEx)
            {
                Debug.Log(vEx.ToString());
            }

        }

        public bool ContinueWorking { get; set; }

        /// <summary>
        /// Query the database with a command
        /// </summary>
        /// <param name="vCommand"></param>
        /// <param name="vCallback"></param>
        /// <returns></returns>
        public bool Query(string vCommand, Action vCallback = null)
        {
            SqliteCommand vCmd = mDbConnection.CreateCommand();
            vCmd.CommandText = vCommand;
            vCmd.ExecuteNonQuery();
            if (vCallback != null)
            {
                vCallback.Invoke();
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vRecording"></param>
        /// <returns></returns>
        public bool CreateRecording(BodyFramesRecordingBase vRecording)
        {
            return CreateRecording(vRecording, null, null);
        }

        /// <summary>
        /// Update the database with a passed in recording
        /// </summary>
        /// <param name="vRecording"></param>
        /// <param name="vDescriptor"></param>
        /// <param name="vTotalImportProgress"></param>
        /// <returns></returns>
        public bool CreateRecording(BodyFramesRecordingBase vRecording, RecordingItemDescriptor vDescriptor, Action<int> vTotalImportProgress)
        {
            bool vResult = false;
            int vTotalFrames = vRecording.RecordingRawFramesCount;
            using (var vCmd = mDbConnection.CreateCommand())
            {
                using (var vTransaction = mDbConnection.BeginTransaction())
                {

                    string vDatePattern = @"M/d/yyyy hh:mm:ss tt";
                    string vTimeNowUsEn = DateTime.UtcNow.ToString(vDatePattern);
                    string vTime = vTimeNowUsEn;
                    string vTitle = vRecording.Title;

                    if (vDescriptor != null)
                    {
                        vTitle = vDescriptor.MovementTitle;
                        vTime = vDescriptor.CreatedAtTime.ToString(vDatePattern);
                    }
                    try
                    {
                        vCmd.CommandText = "INSERT INTO movements (id, complex_equipment_id , profile_id, submitted_by , folder_id,title ,created_at ,updated_at) "
                                       + " VALUES (@param1, @param2,@param3, @param4, @param5, @param6,@param7, @param8)";
                        vCmd.Parameters.Add(new SqliteParameter("@param1", vRecording.BodyRecordingGuid));
                        vCmd.Parameters.Add(new SqliteParameter("@param2", vRecording.SuitGuid));
                        vCmd.Parameters.Add(new SqliteParameter("@param3", vRecording.BodyGuid));
                        vCmd.Parameters.Add(new SqliteParameter("@param4", vRecording.BodyGuid));
                        vCmd.Parameters.Add(new SqliteParameter("@param5", "Default"));
                        vCmd.Parameters.Add(new SqliteParameter("@param6", vTitle));
                        vCmd.Parameters.Add(new SqliteParameter("@param7", vTimeNowUsEn));
                        vCmd.Parameters.Add(new SqliteParameter("@param8", vTime));
                        vCmd.ExecuteNonQuery();

                        vCmd.CommandText = "INSERT INTO movement_meta (id ,movement_id,start_frame,end_frame) "
                                       + " VALUES (@param1, @param2,@param3, @param4)";
                        vCmd.Parameters.Add(new SqliteParameter("@param1", Guid.NewGuid().ToString()));
                        vCmd.Parameters.Add(new SqliteParameter("@param2", vRecording.BodyRecordingGuid));
                        vCmd.Parameters.Add(new SqliteParameter("@param3", "0"));
                        vCmd.Parameters.Add(new SqliteParameter("@param4", (vRecording.RecordingRawFramesCount - 1) + ""));
                        vCmd.ExecuteNonQuery();

                        //initialize variable to avoid GC punishes
                        BodyFrame vConvertedFrame = null;
                        BodyRawFrame vRawFrame = null;
                        string vInstantDataJson = "";
                        string vJoinedRawFrameData = "";
                        for (int i = 0; i < vTotalFrames; i++)
                        {
                            if (!ContinueWorking)
                            {
                                vTransaction.Rollback();
                                break;
                            }
                            try
                            {
                                vRawFrame = (BodyRawFrame)vRecording.GetBodyRawFrameAt(i);//[i];
                                vConvertedFrame = RawFrameConverter.ConvertEncodedRawFrame(vRawFrame);
                                //+++++++++++++++++++++Frames table insertion +++++++++++++++++++++++++++++++++++++++++++++++

                                vCmd.CommandText = "INSERT INTO frames (id,movement_id,format_revision,timestamp) "
                                        + " VALUES (@param1, @param2,@param3, @param4)";
                                string vFrameGuid = Guid.NewGuid().ToString();
                                vCmd.Parameters.Add(new SqliteParameter("@param1", vFrameGuid));
                                vCmd.Parameters.Add(new SqliteParameter("@param2", vRecording.BodyRecordingGuid));
                                vCmd.Parameters.Add(new SqliteParameter("@param3", vRecording.FormatRevision));
                                vCmd.Parameters.Add(new SqliteParameter("@param4", vConvertedFrame.Timestamp + ""));
                                vCmd.ExecuteNonQuery();

                                //+++++++++++++++++++++raw_data table insertion +++++++++++++++++++++++++++++++++++++++++++++

                                vCmd.CommandText = "INSERT INTO raw_data (id ,movement_id,data) "
                                       + " VALUES (@param1, @param2, @param3)";

                                vCmd.Parameters.Add(new SqliteParameter("@param1", i + ""));
                                vCmd.Parameters.Add(new SqliteParameter("@param2", vRecording.BodyRecordingGuid));
                                vCmd.Parameters.Add(new SqliteParameter("@param3", string.Join(",", vRawFrame.RawFrameData)));
                                vCmd.ExecuteNonQuery();
                                //+++++++++++++++++++++instantaneous_data table insertion +++++++++++++++++++++
                                vInstantDataJson = JsonUtilities.SerializeObjToJson(vConvertedFrame);
                                vCmd.CommandText = "INSERT INTO instantaneous_data (id ,type,data,movement_id) "
                                      + " VALUES (@param1, @param2, @param3,@param4)";

                                vCmd.Parameters.Add(new SqliteParameter("@param1", i + ""));
                                vCmd.Parameters.Add(new SqliteParameter("@param2", "default"));
                                vCmd.Parameters.Add(new SqliteParameter("@param3", vInstantDataJson));
                                vCmd.Parameters.Add(new SqliteParameter("@param4", vRecording.BodyRecordingGuid));

                                vCmd.ExecuteNonQuery();
                                if (vTotalImportProgress != null)
                                {
                                    if (OutterThreadToUnityThreadIntermediary.InUnityThread())
                                    {
                                        vTotalImportProgress.Invoke(i);
                                    }
                                    else
                                    {
                                        int vTemp = i;
                                        OutterThreadToUnityThreadIntermediary.EnqueueOverwrittableActionInUnity("vTotalImportProgress", () => vTotalImportProgress(vTemp));
                                    }

                                }
                            }
                            catch (Exception e)
                            {

                                Debug.Log(" error " + e);
                            }

                        }
                        if (ContinueWorking)
                        {
                            vResult = true;
                            vTransaction.Commit();
                        }

                    }

                    catch (SqliteException vEx)
                    {
                        Debug.Log("SqliteException " + vEx);
                        vTransaction.Rollback();
                    }


                }
            }

            return vResult;
        }



        /// <summary>
        /// get a recording based on the id passed in
        /// </summary>
        /// <param name="vRecordingId">the recording id</param> 
        /// <returns></returns>
        public BodyFramesRecordingBase GetRawRecording(string vRecordingId)
        {
            CsvBodyFramesRecording vCsvBodyFramesRecording = new CsvBodyFramesRecording();
            List<BodyRawFrame> vRawFrames = new List<BodyRawFrame>(500);

            SqliteDataReader vDataReader = null;
            string vBodyGuid = "";
            string vSuitGuid = "";
            string vFormatVersion = "0";

            using (var vCmd = mDbConnection.CreateCommand())
            {
                using (var vTransaction = mDbConnection.BeginTransaction())
                {
                    try
                    {
                        vCmd.CommandText = "SELECT * FROM " + DBSettings.RecordingsTable +
                                      " WHERE id = '" + vRecordingId + "'";
                        vDataReader = vCmd.ExecuteReader();
                        while (vDataReader.Read())
                        {
                            vSuitGuid = vDataReader.GetString(1);
                            vBodyGuid = vDataReader.GetString(2);
                        }
                        vDataReader.Close();
                        //Get the format revision from the first frame hit.
                        vCmd.CommandText = "SELECT format_revision FROM " + DBSettings.FramesTable + " " +
                                           "WHERE movement_id ='" + vRecordingId + "' LIMIT 1";

                        vDataReader = vCmd.ExecuteReader();
                        while (vDataReader.Read())
                        {
                            vFormatVersion = vDataReader.GetString(0);
                        }

                        vDataReader.Close();
                        //Get raw data from the raw data table. Ids are the positions
                        //of the raw data frames
                        vCmd.CommandText = "SELECT data FROM " + DBSettings.RawDataTable +
                                           " WHERE raw_data.movement_id = '" + vRecordingId +
                                           "' order by raw_data.id";
                        vDataReader = vCmd.ExecuteReader();
                        string vTemp = "";

                        while (vDataReader.Read())
                        {
                            vTemp = vDataReader.GetString(0);
                            BodyRawFrame vRawFrame = new BodyRawFrame(0);
                            vRawFrame.BodyRecordingGuid = vRecordingId;
                            vRawFrame.BodyGuid = vBodyGuid;
                            vRawFrame.SuitGuid = vSuitGuid;

                            vRawFrame.RawFrameData = vTemp.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            vRawFrames.Add(vRawFrame);
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.Log("<color=red><b>ERROR!</b></color> " + e);
                        vTransaction.Rollback();
                        //todo: callback error
                    }
                  //  vTransaction.Commit();
                }
                vCsvBodyFramesRecording.BodyRecordingGuid = vRecordingId;
                vCsvBodyFramesRecording.BodyGuid = vBodyGuid;
                vCsvBodyFramesRecording.SuitGuid = vSuitGuid;
                vCsvBodyFramesRecording.FromDatFile = false;
                vCsvBodyFramesRecording.RecordingRawFrames = vRawFrames;
                vCsvBodyFramesRecording.FormatRevision = vFormatVersion;
            }
            return vCsvBodyFramesRecording;
        }

        /// <summary>
        /// Locates a tag by the unique identifier passed in from the database
        /// </summary>
        /// <param name="vUid"></param>
        /// <returns></returns>
        public Tag GetTagById(string vUid)
        {
            Tag vOutput = null;
            using (var vCmd = mDbConnection.CreateCommand())
            {
                try
                {
                    vOutput = new Tag();
                    vCmd.CommandText = "SELECT * from tags " +
                                               "WHERE id = '" + vUid + "'";
                    SqliteDataReader vReader = vCmd.ExecuteReader();
                    while (vReader.Read())
                    {
                        vOutput.TagUid = vUid;
                        vOutput.Title = vReader.GetString(1);
                    }
                }
                catch (Exception vExc)
                {

                }
            }
            return vOutput;
        }

        /// <summary>
        /// Adds a new tag to the database
        /// </summary>
        /// <param name="vTag"></param>
        public void AddNewTag(Tag vTag)
        {
            using (var vCmd = mDbConnection.CreateCommand())
            {
                using (var vTransaction = mDbConnection.BeginTransaction())
                {
                    try
                    {
                        vCmd.CommandText = "INSERT OR IGNORE INTO tags(id, title) " +
                                           "VALUES(@param1, @param2)";
                        vCmd.Parameters.Add(new SqliteParameter("@param1", vTag.TagUid));
                        vCmd.Parameters.Add(new SqliteParameter("@param2", vTag.Title));
                        vCmd.ExecuteNonQuery();
                        vTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Debug.Log("<color=red><b>DB error:</b></color> " + e);
                        vTransaction.Rollback();
                    }
                }
            }
        }

        /// <summary>
        /// returns a list of tags of a recording
        /// </summary>
        /// <param name="vRecGuid"></param>
        /// <returns></returns>
        public List<Tag> GetTagsOfRecording(string vRecGuid)
        {
            List<Tag> vFoundTags = new List<Tag>();
            using (var vCmd = mDbConnection.CreateCommand())
            {
                using (var vTransaction = mDbConnection.BeginTransaction())
                {
                    try
                    {
                        vCmd.CommandText = "SELECT * FROM tags AS T1 JOIN " +
                                  " taggable AS T2 ON T1.id = T2.tag_id where T2.movement_id = @param1";
                        vCmd.Parameters.Add(new SqliteParameter("@param1", vRecGuid));
                        SqliteDataReader vReader = vCmd.ExecuteReader();
                        while (vReader.Read())
                        {
                            string vGuid = vReader.GetString(0);
                            string vTitle = vReader.GetString(1);
                            Tag vTag = new Tag() { TagUid = vGuid, Title = vTitle };
                            vFoundTags.Add(vTag);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("<color=red><b>DB error:</b></color> " + e);
                        vTransaction.Rollback();
                    }
                }
            }
            return vFoundTags;
        }

        /// <summary>
        /// Get a list of tags from partial results. 
        /// Note, if passing in a value of zero, or opting out of using the optional paramater,
        /// will return the full list of results
        /// </summary>
        /// <param name="vTitleKey"></param>
        /// <param name="vTotalResults">optional parameter, returns a subset of the total results</param>
        /// <returns></returns>
        public List<Tag> GetTagsByPartialTitle(string vTitleKey, int vTotalResults = 0)
        {
            string vSanitized = SanitizeInput(vTitleKey);
            List<Tag> vFoundTags = new List<Tag>();
            using (var vCmd = mDbConnection.CreateCommand())
            {
                using (var vTransaction = mDbConnection.BeginTransaction())
                {
                    try
                    {
                        vCmd.CommandText = "SELECT * FROM tags WHERE" +
                                           " tags.title LIKE @param1";
                        vCmd.Parameters.Add(new SqliteParameter("@param1", "%" + vSanitized + "%"));
                        if (vTotalResults > 0)
                        {
                            vCmd.CommandText += " LIMIT " + vTotalResults;
                        }
                        SqliteDataReader vReader = vCmd.ExecuteReader();
                        while (vReader.Read())
                        {
                            string vGuid = vReader.GetString(0);
                            string vTitle = vReader.GetString(1);

                            Tag vTag = new Tag() { TagUid = vGuid, Title = vTitle };
                            vFoundTags.Add(vTag);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("<color=red><b>DB error:</b></color> " + e);
                        vTransaction.Rollback();
                    }
                }

            }
            return vFoundTags;
        }

        public string SanitizeInput(string vInput)
        {
            string vSanitized = Regex.Replace(vInput, @"%", "");
            return vSanitized;
        }

        public List<RecordingItemDescriptor> GetRecordingDescriptions(string vBodyGuid, List<Tag> vTagFilter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a tag found by its title from the database. If none are found, then null is returned
        /// </summary>
        /// <param name="vTitle"></param>
        /// <returns></returns>
        public Tag GetTagByTitle(string vTitle)
        {
            Tag vTag = null;

            string vSanitized = SanitizeInput(vTitle);

            using (var vCmd = mDbConnection.CreateCommand())
            {
                using (var vTransaction = mDbConnection.BeginTransaction())
                {
                    try
                    {
                        vCmd.CommandText = "SELECT * FROM tags WHERE" +
                                           " tags.title LIKE @param1";
                        vCmd.Parameters.Add(new SqliteParameter("@param1", vSanitized));
                        //only limit to one 
                        vCmd.CommandText += " LIMIT 1";

                        SqliteDataReader vReader = vCmd.ExecuteReader();
                        while (vReader.Read())
                        {
                            string vTagTitle = vReader.GetString(1); ;
                            string vGuid = vReader.GetString(0); ;
                            vTag = new Tag() { TagUid = vGuid, Title = vTagTitle };
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("<color=red><b>DB error:</b></color> " + e);
                        vTransaction.Rollback();
                    }
                }

            }
            return vTag;
        }

        /// <summary>
        /// Get a list of tags from partial result but excluding the list of tags
        /// Note, will return a full list of results if the optional parameter isn't used, or is passed in with 0
        /// </summary>
        /// <param name="vTitleKey">the </param>
        /// <param name="vExcludingTags">the tags to exclude</param>
        /// <param name="vTotalResult">(Optional) Limit number of results to pass back</param>
        /// <returns></returns>
        public List<Tag> GetTagsByPartialTitleExcludingList(string vTitleKey, List<Tag> vExcludingTags, int vTotalResult = 0)
        {
            List<Tag> vFoundTags = new List<Tag>();
            string vSanitized = SanitizeInput(vTitleKey);
            using (var vCmd = mDbConnection.CreateCommand())
            {
                using (var vTransaction = mDbConnection.BeginTransaction())
                {
                    try
                    {
                        //sanity check
                        vCmd.CommandText = "SELECT * FROM tags WHERE" +
                                           " tags.title LIKE @param1";
                        int vParamCount = 1;
                        string vParam = "@param" + vParamCount++;
                        vCmd.Parameters.Add(new SqliteParameter(vParam, "%" + vSanitized + "%"));
                        for (int i = 0; i < vExcludingTags.Count; i++, vParamCount++)
                        {
                            vParam = "@param" + vParamCount;
                            string vCommandAffix = " AND tags.title NOT LIKE " + vParam;
                            vCmd.CommandText += vCommandAffix;
                            vCmd.Parameters.Add(new SqliteParameter(vParam, "%" + vExcludingTags[i].Title + "%"));
                        }
                        if (vTotalResult > 0)
                        {
                            vCmd.CommandText += " LIMIT " + vTotalResult;
                        }
                        SqliteDataReader vReader = vCmd.ExecuteReader();
                        while (vReader.Read())
                        {
                            string vGuid = vReader.GetString(0);
                            string vTitle = vReader.GetString(1);
                            Tag vTag = new Tag() { TagUid = vGuid, Title = vTitle };
                            vFoundTags.Add(vTag);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("<color=red><b>DB error:</b></color> " + e);
                        vTransaction.Rollback();
                    }
                }

            }
            return vFoundTags;
        }


        /// <summary>
        /// Attaches a tag to a recording
        /// </summary>
        /// <param name="vRec"></param>
        /// <param name="vTag"></param>
        public void AddTagToRecording(BodyFramesRecordingBase vRec, Tag vTag)
        {
            AddNewTag(vTag);

            using (var vCmd = mDbConnection.CreateCommand())
            {
                using (var vTransaction = mDbConnection.BeginTransaction())
                {
                    try
                    {
                        vCmd.CommandText = "INSERT OR IGNORE INTO taggable(tag_id, movement_id, taggable_type) " +
                                          "VALUES(@param1, @param2, @param3)";
                        vCmd.Parameters.Add(new SqliteParameter("@param1", vTag.TagUid));
                        vCmd.Parameters.Add(new SqliteParameter("@param2", vRec.BodyRecordingGuid));
                        vCmd.Parameters.Add(new SqliteParameter("@param3", "recording"));

                        vCmd.ExecuteNonQuery();
                        vTransaction.Commit();

                    }
                    catch (Exception e)
                    {
                        Debug.Log("<color=red><b>DB error:</b></color> " + e);
                        vTransaction.Rollback();
                    }
                }
            }

        }

        public List<Tag> LoadAllTags()
        {
            List<Tag> vFound = new List<Tag>();

            using (var vCmd = mDbConnection.CreateCommand())
            {
                using (var vTransaction = mDbConnection.BeginTransaction())
                {
                    try
                    {
                        vCmd.CommandText = "SELECT * FROM tags";
                        SqliteDataReader vReader = vCmd.ExecuteReader();
                        while (vReader.Read())
                        {
                            string vGuid = vReader.GetString(0);
                            string vTitle = vReader.GetString(1);
                            vFound.Add(new Tag() { TagUid = vGuid, Title = vTitle });
                        }
                        vTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Debug.Log("<color=red><b>DB error:</b></color> " + e);
                        vTransaction.Rollback();
                    }
                }
            }
            return vFound;
        }

        public List<string> GetRecordingGuidFromTagId(string vTagId)
        {
            List<string> vFound = new List<string>();

            using (var vCmd = mDbConnection.CreateCommand())
            {
                using (var vTransaction = mDbConnection.BeginTransaction())
                {
                    try
                    {
                        vCmd.CommandText = "SELECT T1.id FROM movements AS T1 JOIN taggable AS T2 ON T1.id = T2.movement_id " +
                                           "WHERE T2.tag_id = @param1";
                        vCmd.Parameters.Add(new SqliteParameter("@param1", vTagId));
                        SqliteDataReader vReader = vCmd.ExecuteReader();
                        while (vReader.Read())
                        {
                            string vGuid = vReader.GetString(0);
                            vFound.Add(vGuid);
                        }
                        vTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Debug.Log("<color=red><b>DB error:</b></color> " + e);
                        vTransaction.Rollback();
                    }
                }
            }
            return vFound;
        }

        public bool UpdateRecordingFolder(string vRecordingUid, string vFolderUid)
        {
            throw new NotImplementedException();
        }
    }
}
