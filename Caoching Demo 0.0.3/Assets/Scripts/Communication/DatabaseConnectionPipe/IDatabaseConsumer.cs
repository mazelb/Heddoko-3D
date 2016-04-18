/** 
* @file IDatabaseConsumer.cs
* @brief Contains the IDatabaseConsumer interface
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
namespace Assets.Scripts.Communication.DatabaseConnectionPipe
{
    /// <summary>
    /// The interface used by a Database consumer
    /// </summary>
    public interface IDatabaseConsumer
    {
         Database Database { get; set; }
    }
}