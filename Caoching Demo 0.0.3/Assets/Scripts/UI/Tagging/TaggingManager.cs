/** 
* @file TaggingManager.cs
* @brief Contains the TaggingManager class
* @author Mohammed Haider(Mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Communication.DatabaseConnectionPipe;

namespace Assets.Scripts.UI.Tagging
{
    /// <summary>
    /// 
    /// </summary>
    public class TaggingManager
    { 
        /// <summary>
        /// tag set, where the key is defined by the tag uuid
        /// </summary>
        private Dictionary<string, Tag> mTags   = new Dictionary<string, Tag>();
        public Database Database { get; set; }
         
        /// <summary>
        /// returns a tag from a title
        /// </summary>
        /// <param name="vTitle"></param>
        public Tag GetTagByTitle(string vTitle)
        {
            Tag vTag = null;
            bool vFound = mTags.Values.Any(value => value.Title.Equals(vTitle));
            if (!vFound)
            {
                vTag = Database.Connection.GetTagByTitle(vTitle);
                //try to find the tag from the database first
                if (vTag == null)
                {
                    vTag = new Tag() { TagUid = Guid.NewGuid().ToString(), Title = vTitle };
                }
                if (!mTags.ContainsKey(vTag.TagUid))
                {
                    mTags.Add(vTag.TagUid, vTag);
                }
     
                Database.Connection.AddNewTag(vTag);
            }
            else
            {
                vTag = mTags.First(x => x.Value.Title.Equals(vTitle)).Value;
            }
            return vTag;
        }

        /// <summary>
        /// Loads all tags from the database
        /// </summary>
        /// <returns></returns>
        public List<Tag> LoadAllTags()
        {
            List<Tag> vTags = Database.Connection.LoadAllTags();
            return vTags;
        }

        public void AttachTagToRecording(BodyFramesRecording vRec, Tag vTag)
        {
            Database.Connection.AddTagToRecording(vRec, vTag);
            //try to add the tag to the current collection
            if (!mTags.ContainsKey(vTag.TagUid))
            {
                mTags.Add(vTag.TagUid,vTag);
            }
            vRec.AddTag(vTag);
        }
        public void SetDatabase(Database vDatabase)
        {
            Database = vDatabase;
        }

        /// <summary>
        /// Get a tag but its unique identifier
        /// Note: this will return null if no tags have been found
        /// </summary>
        /// <param name="vUid"></param>
        public Tag GetTagByUid(string vUid)
        {
            Tag vFoundObj = null;
            mTags.TryGetValue(vUid, out vFoundObj);
            if (vFoundObj == null)
            {
                vFoundObj = Database.Connection.GetTagById(vUid);
                if (vFoundObj != null)
                {
                    mTags.Add(vUid, vFoundObj);
                }
            }
            return vFoundObj;
        }

        /// <summary>
        /// Returns a list of Tags of a recording  
        /// </summary>
        /// <param name="vRecGuid"></param>
        /// <returns></returns>
        public List<Tag> GetTagsFromRecordingUid(string vRecGuid)
        {
            List<Tag> vFoundTags = new List<Tag>();
            vFoundTags = Database.Connection.GetTagsOfRecording(vRecGuid);
            //update the recording from body recordings manager
            BodyFramesRecording vRec = BodyRecordingsMgr.Instance.GetRecordingByUuid(vRecGuid);
            vRec.Tags = vFoundTags;
            return vFoundTags;
        }

        /// <summary>
        /// Returns a list of results found from a partial key
        /// </summary>
        /// <param name="vPartialTitle">the partial result to find</param>
        /// <param name="vTotalResults">optional parameter: the max number of total results to return</param>
        /// <returns></returns>
        public List<Tag> FindTagByPartialTitle(string vPartialTitle, int vTotalResults =0)
        {
            string vSanitized = Database.Connection.SanitizeInput(vPartialTitle);
            List<Tag> vFullMatchingList = new List<Tag>();
            if (vTotalResults > 0)
            {
                vFullMatchingList = mTags.Values.Where(vCurrentValue => vCurrentValue.Title.Contains(vSanitized)).Take(vTotalResults).ToList();
            }
            else if (vTotalResults <=0)
            {
                vFullMatchingList = mTags.Values.Where(vCurrentValue => vCurrentValue.Title.Contains(vSanitized)).ToList();
            }

            if (vFullMatchingList.Count > 0)
            { 
                return vFullMatchingList;
            }
            else
            {
                List<Tag> vFoundTags = new List<Tag>();
                vFoundTags = Database.Connection.GetTagsByPartialTitle(vPartialTitle,vTotalResults);
                for (int i = 0; i < vFoundTags.Count; i++)
                {
                    if (!mTags.ContainsKey(vFoundTags[i].TagUid))
                    {
                        mTags.Add(vFoundTags[i].TagUid, vFoundTags[i]); 
                    }
                } 
                return vFoundTags;
            }
          
        }
         
       
        /// <summary>
        /// Add a tag to the database
        /// </summary>
        /// <param name="vTag">the tag to add</param>
        public void AddTag(Tag vTag)
        {
            if (!mTags.ContainsKey(vTag.TagUid))
            {
                mTags.Add(vTag.TagUid, vTag);
                Database.Connection.AddNewTag(vTag);
            }
        }

     

        /// <summary>
        /// Get a list of tags excluding the exclusion list
        /// </summary>
        /// <param name="vPartial"></param>
        /// <param name="vExclusion"></param>
        /// <param name="vTotalResults">(optional) limit the number of the results returned</param>
        /// <returns></returns>
        public List<Tag> GetTagsByPartialTitleExcludingList(string vPartial, List<Tag> vExclusion, int vTotalResults =0)
        {
            List<Tag> vFoundTags = Database.Connection.GetTagsByPartialTitleExcludingList(vPartial,vExclusion);
            //come back and add the tags
            return vFoundTags;
        }

        /// <summary>
        /// add a tag set to a body frame recording
        /// </summary>
        /// <param name="vTagSet">the tag set to add</param>
        /// <param name="vRecording">the recording to add the tag set to</param>
        public void AttachTagSetToRecording(ICollection<string> vTagSet, BodyFramesRecording vRecording)
        {
            if (vTagSet.Count == 0)
            {
                return;
            }
            else
            {
                foreach (var vTagSetDescritor in  vTagSet)
                {
                    Tag vTag = GetTagByTitle(vTagSetDescritor);
                    AttachTagToRecording(vRecording,vTag);
                    Database.Connection.AddNewTag(vTag);
                }
            }
        }
    }
}
