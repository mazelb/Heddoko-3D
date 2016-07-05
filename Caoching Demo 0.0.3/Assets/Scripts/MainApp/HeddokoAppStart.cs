/** 
* @file HeddokoAppStart.cs
* @brief Contains the HeddokoAppStart class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Body_Data.View;
using Assets.Scripts.Communication.DatabaseConnectionPipe;
using Assets.Scripts.UI.AbstractViews.camera;
using Assets.Scripts.UI.ModalWindow;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.UI.Tagging;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.DatabaseAccess;
using Assets.Scripts.Utils.DebugContext.logging;
using Assets.Scripts.Utils.HMath.Service_Provider;
using Assets.Scripts.Utils.HMath.Structure;
using UnityEngine;
using Application = UnityEngine.Application;

namespace Assets.Scripts.MainApp
{
    /// <summary>
    /// HeddokoAppStart:  This class is the starting point of the application, 
    /// performing a variety of checks before the application can be launched.
    /// </summary>
    public class HeddokoAppStart : MonoBehaviour
    {

        private LocalDBAccess mDbAccess;
        private Database mDatabase;
        private TaggingManager mTaggingManager;
        public GameObject[] GOtoReEnable;
        public GameObject[] DatabaseConsumers;
        public GameObject[] TaggingManagerConsumers;
        public bool IsDemo = false;
        public TaggingManager TaggingManager { get { return mTaggingManager; } }

        void Init()
        {
            OutterThreadToUnityThreadIntermediary.Instance.Init();
            BodySegment.IsTrackingHeight = false;
            mTaggingManager = new TaggingManager();
            InitiliazePools();
            InitializeDatabase();
            InjectDatabaseDependents();
            InjectTaggingManagerDependents();
            InitializeLoggers();
            QualitySettings.vSyncCount = 0;
            bool vAppSafelyLaunched; 
            HVector3.Vector3MathServiceProvider = new UVector3MathServiceProvider();
            BodySegment.IsTrackingHeight = false;
 
            if (!IsDemo)
            {

                mDbAccess = new LocalDBAccess();
                EnableObjects(true);
                mDbAccess.SetApplicationSettings();
            }
        }
        // ReSharper disable once UnusedMember.Local
        void Awake()
        {
            Init();

        }

        void Start()
        {
            UniFileBrowser.use.SetPath(ApplicationSettings.PreferedRecordingsFolder);

        }
        /// <summary>
        /// Injects the single database component into interested consumers
        /// </summary>
        private void InjectDatabaseDependents()
        {
            mTaggingManager.Database = mDatabase;
            BodyRecordingsMgr.Instance.Database = mDatabase;
            foreach (var vDbConsumer in DatabaseConsumers)
            {
                //attempt to grab the database consumer interface from the gameobject
                IDatabaseConsumer vConsumer = vDbConsumer.GetComponent<IDatabaseConsumer>();
                if (vConsumer != null)
                {
                    vConsumer.Database = mDatabase;
                }
            }
        }
        /// <summary>
        ///Injects tagging manager dependents with a tagging manager object
        /// </summary>
        private void InjectTaggingManagerDependents()
        {
            foreach (var vDependent in TaggingManagerConsumers)
            {
                ITaggingManagerConsumer vConsumer = vDependent.GetComponent<ITaggingManagerConsumer>();
                vConsumer.TaggingManager = mTaggingManager;
            }
        }

        private void InitializeLoggers()
        {
            DebugLogger.Instance.Start();

#if !DEBUG
            DebugLogger.Settings.AllFalse();
#endif

        }

        /// <summary>
        /// Sets up internal pools
        /// </summary>
        private void InitiliazePools()
        {
            GameObject vRenderedBodyGroup = GameObject.FindWithTag("RenderedBodyGroup");
            GameObject vPanelCameraGroup = GameObject.FindWithTag("PanelCameraGroup");

            RenderedBodyPool.ParentGroupTransform = vRenderedBodyGroup.transform;
            PanelCameraPool.CameraParent = vPanelCameraGroup.transform;
        }


        /// <summary>
        /// Enables or disable the array of gameobjects 
        /// </summary>
        /// <param name="vFlag"></param>
        void EnableObjects(bool vFlag)
        {
            foreach (var vGo in GOtoReEnable)
            {
                if (vGo != null)
                {
                    vGo.SetActive(vFlag);
                }
            }
        }

        /// <summary>
        /// The application wasn't started with the launcher. Display a message then quit the application
        /// </summary>
        private void AppNotLaunchedThroughLauncher()
        {

            ModalPanel.SingleChoice("","The application wasn't started with the Launcher. Press Ok to exit and try again. ", Application.Quit);
        }

        void OnApplicationQuit()
        { 
            DebugLogger.Instance.Stop();
            mDbAccess.SaveApplicationSettings();
            mDbAccess.Dispose();
        }

        /// <summary>
        /// Initialize the database 
        /// </summary>
        private void InitializeDatabase()
        {
            mDatabase = new Database(DatabaseConnectionType.Local);
            mDatabase.Init();
        }

        private void SetResolution()
        {



        }


    }
}
