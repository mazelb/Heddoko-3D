/** 
* @file HeddokoAppStart.cs
* @brief Contains the HeddokoAppStart class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;

using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Communication.DatabaseConnectionPipe;
using Assets.Scripts.UI.AbstractViews.camera;
using Assets.Scripts.UI.ModalWindow;
using Assets.Scripts.UI.Scene_3d.View;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.UI.Tagging;
using Assets.Scripts.Utils.DebugContext.logging;
using UnityEngine;
using UnityEngine.Experimental.Director;
using Application = UnityEngine.Application;

namespace Assets.Scripts.Utils.DatabaseAccess
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
        public ScrollablePanel ContentPanel;
        public bool IsDemo = false;
        public TaggingManager TaggingManager { get { return mTaggingManager; } }

        // ReSharper disable once UnusedMember.Local
        void Awake()
        {

            OutterThreadToUnityThreadIntermediary.Instance.Init();
            BodySegment.IsTrackingHeight = false;
            mTaggingManager = new TaggingManager();
            InitiliazePools();
            InitializeDatabase();
            InjectDatabaseDependents();
            InjectTaggingManagerDependents();
            InitializeLoggers();

            bool vAppSafelyLaunched;
            EnableObjects(false);
            BodySegment.IsTrackingHeight = false;
            if (!IsDemo)
            {

                mDbAccess = new LocalDBAccess();

                bool vApplicationSettingsFound = mDbAccess.SetApplicationSettings();


                if (vApplicationSettingsFound)
                {
                    vAppSafelyLaunched = ApplicationSettings.AppLaunchedSafely;
                    if (vAppSafelyLaunched)
                    {
                        string vGet = ApplicationSettings.PreferedConnName;
                        LauncherBrainpackSearchResults.MapResults();
                        if (LauncherBrainpackSearchResults.BrainpackToComPortMappings.ContainsKey(vGet))
                        {
                            vGet = LauncherBrainpackSearchResults.BrainpackToComPortMappings[vGet];
                        }
                        else
                        {
                            vGet = string.Empty;
                        }

                        BrainpackConnectionController.Instance.BrainpackComPort = vGet;

                        List<ScrollableContent> vContentList = new List<ScrollableContent>();

                        foreach (KeyValuePair<string, string> vKV in LauncherBrainpackSearchResults.BrainpackToComPortMappings)
                        {
                            ScrollableContent vContent = new ScrollableContent();
                            vContent.ContentValue = vKV.Value;
                            if (vContent.ContentValue.Contains("COM") || vContent.ContentValue.Contains("com"))
                            {
                                vContent.Key = vKV.Key;
                                vContent.CallbackAction = new Action(() =>
                                {
                                    BrainpackConnectionController.Instance.BrainpackComPort = vContent.ContentValue;
                                });
                                vContentList.Add(vContent);
                                ContentPanel.CurrentlySelectedContent = vContent;
                            }

                        }
                        ContentPanel.Contents = vContentList;
                        ContentPanel.LoadContent();
                        EnableObjects(true);
                    }
                    else
                    {
                        AppNotLaunchedThroughLauncher();
                    }
                }

            }


        }

        void Start()
        {
            if (!IsDemo)
            {
                bool vUseNonPersistentDataFolder = false;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN 
                //check if recordings folder exists first. Create it
                string vPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Recordings";

                vUseNonPersistentDataFolder = true;
                if (!Directory.Exists(vPath))
                {
                    Debug.Log("here1");

                    try
                    {
                        Debug.Log("here2");

                        Directory.CreateDirectory(vPath);
                      
                    }
                    catch (Exception)
                    {

                        vUseNonPersistentDataFolder = false;
                    }
                }
                UniFileBrowser.use.SetPath(vPath);
#endif
                if (!vUseNonPersistentDataFolder)
                {
                    UniFileBrowser.use.SetPath(Application.persistentDataPath);
                }
            }
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

            ModalPanel.SingleChoice("The application wasn't started with the Launcher. Press Ok to exit and try again. ", Application.Quit);
        }

        void OnApplicationQuit()
        {
            // mDatabase.CleanUp();
            DebugLogger.Instance.Stop();
        }

        /// <summary>
        /// Initialize the database 
        /// </summary>
        private void InitializeDatabase()
        {
            mDatabase = new Database(DatabaseConnectionType.Local);
            mDatabase.Init();

        }


    }
}
