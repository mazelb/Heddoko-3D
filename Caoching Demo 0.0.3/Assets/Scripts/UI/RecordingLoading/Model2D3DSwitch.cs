/** 
* @file Model2D3DSwitch.cs
* @brief Contains the Model2D3DSwitch class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/


using Assets.Scripts.Cameras;
using Assets.Scripts.UI.ActivitiesContext.View;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.UI.Metrics;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.RecordingLoading
{
    /// <summary>
    /// Switches between the 3D and 2D model
    /// </summary>
    public class Model2D3DSwitch : MonoBehaviour
    {
        public GameObject Model3D;
        public GameObject Model2D;
        //    public AngleInfoMetrics AngleInfo;
        public ActivitiesContextViewTrain TrainingView;
        /// <summary>
        /// Location placement
        /// </summary>
        public Transform TransformInview3DLocation;
        public Transform TransformInview2DLocation;
        public Transform TransformOutOfViewLocation;

        public Button Button2DSwitch;
        public Button Button3DSwitch;

        public PlayerStreamManager PlayerStreamManager;
        public CurrentViewBox InformationBox;
        public CameraController CameraController;

        //  public CameraController OrthoCamController;
        /// <summary>
        /// Flag to check if only using 2D model
        /// </summary>
        public bool OnlyUsing2D;
        [SerializeField]
        private bool mUsing2DModel = false;

        public bool IsNFLDemo;

        /// <summary>
        /// On awake hook a listener to the button and set button interaction according to mUsing2DModelFlag
        /// </summary>
        void Awake()
        {
            if (IsNFLDemo)
            {
                Button2DSwitch.gameObject.SetActive(false);
                Button3DSwitch.gameObject.SetActive(false);
                Model2D.SetActive(false);
                return;
            }
            if (OnlyUsing2D)
            {
                //disable buttons
                Button2DSwitch.gameObject.SetActive(false);
                Button3DSwitch.gameObject.SetActive(false);
                mUsing2DModel = true;

            }
            else
            {
                Button2DSwitch.onClick.AddListener(SwitchTo2DModelView);
                Button3DSwitch.onClick.AddListener(SwitchTo3DModelView);
                Button2DSwitch.interactable = !mUsing2DModel;
                Button3DSwitch.interactable = mUsing2DModel;
            }
        }

        /// <summary>
        /// Switches to a 3d model view
        /// </summary>
        private void SwitchTo3DModelView()
        {
            Bring3DModelIntoView();
            CameraController.PrepFor3DView();
            if (TrainingView != null)
            {
                if (TrainingView.ActivitiesContextController.UsingSquats || PlayerStreamManager.SpineSplitDisabled)
                {
                    CameraController.gameObject.SetActive(true);
                    CameraController.SetCamFov(2.26f, Vector3.zero);

                }
                else
                {
                    CameraController.SetCamFov(1.2f, new Vector3(0, 0.7f - 1, 0f));
                }

            }

            else
            {
                CameraController.SetCamFov(1.33f, new Vector3(0, 0, 0f));
            }

            SetButtonInteraction();


        }

        /// <summary>
        /// Switches to a 2D model view
        /// </summary>
        private void SwitchTo2DModelView()
        {
            Bring2DModelIntoView();
            SetButtonInteraction();
            CameraController.PrepFor2DView();

            if (TrainingView != null)
            {
                CameraController.SetCamFov(5.9f, Vector3.zero);

            }
            else
            {
                CameraController.SetCamFov(3.57f, Vector3.zero);
            }

        }

        /// <summary>
        /// Sets the button's interaction
        /// </summary>
        private void SetButtonInteraction()
        {
            mUsing2DModel = !mUsing2DModel;
            Button2DSwitch.interactable = !mUsing2DModel;
            Button3DSwitch.interactable = mUsing2DModel;
        }

        /// <summary>
        /// Brings them into view depending on what mode currently in
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);

            //set the positions of the model
            if (mUsing2DModel)
            {
                Model2D.transform.position = TransformInview2DLocation.position;
                Model3D.transform.position = TransformOutOfViewLocation.position;
                if (TrainingView != null)
                {
                    CameraController.SetCamFov(5.9f, Vector3.zero);
                }
                else
                {
                    CameraController.SetCamFov(3.57f, Vector3.zero);
                }
            }

            else
            {
                Model3D.transform.position = TransformInview3DLocation.position;
                Model2D.transform.position = TransformOutOfViewLocation.position;
                Button2DSwitch.interactable = !mUsing2DModel;
                Button3DSwitch.interactable = mUsing2DModel;
                // CameraController.enabled = true;

                if (TrainingView != null)
                {
                    if (TrainingView.ActivitiesContextController.UsingSquats || PlayerStreamManager.SpineSplitDisabled)
                    {

                        CameraController.SetCamFov(2.26f, Vector3.zero);


                    }
                    else
                    {
                        CameraController.SetCamFov(1.2f, new Vector3(0, 0.7f - 1, 0f));
                    }

                }
                else
                {
                    CameraController.SetCamFov(1.33f, new Vector3(0, 0, 0));
                }


            }
        }

        /// <summary>
        /// Brings the 2d model into view and hides the 3d model
        /// </summary>
        private void Bring2DModelIntoView()
        {
            Model2D.transform.position = TransformInview2DLocation.position;
            Model3D.transform.position = TransformOutOfViewLocation.position;
        }

        /// <summary>
        /// Brings the 3d model into view and hides the 2d model
        /// </summary>
        private void Bring3DModelIntoView()
        {
            Model3D.transform.position = TransformInview3DLocation.position;
            Model2D.transform.position = TransformOutOfViewLocation.position;
        }

        /// <summary>
        /// hides the models out of view
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            Model3D.transform.position = TransformOutOfViewLocation.position;
            Model2D.transform.position = TransformOutOfViewLocation.position;
        }
    }
}
