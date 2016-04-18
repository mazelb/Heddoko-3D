/** 
* @file ITaggingManagerConsumer.cs
* @brief Contains the ITaggingManagerConsumer interface  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
namespace Assets.Scripts.UI.Tagging
{
    /// <summary>
    /// Interface for Tagging Manager Consumers
    /// </summary>
    public interface ITaggingManagerConsumer
    {
       TaggingManager TaggingManager { get; set; } 
    }
}