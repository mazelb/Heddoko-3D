/** 
* @file DatabaseConnection.cs
* @brief Contains the DatabaseConnection class
* @author Mohammed Haider(Mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Communication.DatabaseConnectionPipe.DatabaseConnection; 

namespace Assets.Scripts.Communication.DatabaseConnectionPipe
{
    /// <summary>
    /// A database from which the application connects to
    /// </summary>
    public class Database
    {

        private IDatabaseConnection mConnection;
        //     private RecordingQuery mRecordingQuer = new RecordingQuery();
    

        /// <summary>
        /// Constructor requires a type of database connection before it starts
        /// </summary>
        /// <param name="vDbConnectionType"></param>
        public Database(DatabaseConnectionType vDbConnectionType)
        {
            switch (vDbConnectionType)
            {
                case DatabaseConnectionType.Local:
                    mConnection = new LocalDatabaseConnection();
                    break;
                case DatabaseConnectionType.LocalServiceLocalDb:
                    break;
                case DatabaseConnectionType.LocalServiceRemoteDb:
                    break;
                case DatabaseConnectionType.RemoteServiceRemoteDb:
                    break; 
            }
        }

        public IDatabaseConnection Connection
        {
            get { return mConnection; }
        }

 

        /// <summary>
        /// cleans up resources, disconnects database connections
        /// </summary>
        public void CleanUp()
        {
            mConnection.Disconnect();
        }

        /// <summary>
        /// Initializes the database
        /// </summary>
        public void Init()
        {
            mConnection.Connect(); 
        }
    }

    public enum DatabaseConnectionType
    {
        /// <summary>
        /// Database is located locally
        /// </summary>
        Local,
        /// <summary>
        /// Database is located locally and is accessed through a local service
        /// </summary>
        LocalServiceLocalDb,
        /// <summary>
        /// Database is located remotely and is accessed through a local service
        /// </summary>
        LocalServiceRemoteDb,
        /// <summary>
        /// Database is located remotely and is accessed through a remote service
        /// </summary>
        RemoteServiceRemoteDb

    }
}
