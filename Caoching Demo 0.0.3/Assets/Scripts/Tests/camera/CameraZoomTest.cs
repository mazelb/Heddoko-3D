using Assets.Scripts.Body_Data.View;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.cameraSubControls;
using Assets.Scripts.UI.DemoKit;
using UnityEngine;

namespace Assets.Scripts.Tests.camera
{
    public class CameraZoomTest : MonoBehaviour
    {
        public AndroidRecordingPlayerView PlayerView;
        private CameraZoom CamZoom;
        
        private RenderedBody mbody;

        void Awake()
        {
            
        }
        void Update()
        {
            try
            {
                if (mbody == null)
                {
                    mbody = PlayerView.RootBody.RenderedBody;
                }
                if (CamZoom == null)
                {
                    CamZoom = PlayerView.RootNode.PanelSettings.CameraToBodyPair.PanelCamera.CameraZoom;
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        CamZoom.LookAtSubsegmentType = BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf;
                    }
                }
               
            }
            catch
            {

            }

        }
    }
}