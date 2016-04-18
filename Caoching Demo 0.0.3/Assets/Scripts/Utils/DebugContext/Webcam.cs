/** 
* @file Webcam.cs
* @brief Contains the Webcam  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.IO;
using Assets.Scripts.Utils.DebugContext;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Places a webcam stream onto a texture
/// </summary>
public class Webcam : MonoBehaviour
{
    [SerializeField]
    private WebCamTexture mCameraTexture;
    private WebCamDevice[] mDevices;
    public RawImage RawImage;
    private string mCurrentDeviceName;
    private int mCurrentDeviceIndex = 0;
    private Renderer mRenderer;
    private GUITexture mGuiTexture;
    public Text WebcamText;

    public WebCamTexture CameraTexture
    {
        get
        {
            if (mCameraTexture == null && !DeviceName.Equals("NULL"))
            {

                mCameraTexture = new WebCamTexture(DeviceName);


            }
            return mCameraTexture;
        }
        set { mCameraTexture = value; }
    }

    public WebCamDevice[] Devices
    {
        get
        {
            if (mDevices == null)
            {

                mDevices = WebCamTexture.devices;

            }
            if (WebCamTexture.devices.Length == 0)
            {
                WebcamText.text = "No web cameras found";
            }
            else
            {
                WebcamText.text = "";
            }
            return mDevices;
        }

    }
    public Renderer RendererMaterial
    {
        get
        {
            if (mRenderer == null)
            {
                mRenderer = GetComponent<Renderer>();
            }
            return mRenderer;
        }
    }

    public string DeviceName
    {
        get
        {
            if (mCurrentDeviceName == null)
            {
                if (Devices != null && Devices.Length > 0)
                {
                    mCurrentDeviceName = Devices[mCurrentDeviceIndex].name;
                }

            }
            if (Devices != null && Devices.Length > 0)
            {
                mCurrentDeviceName = Devices[mCurrentDeviceIndex].name;
            }
            else
            {
                mCurrentDeviceName = "NULL";
            }
            return mCurrentDeviceName;
        }
        set { mCurrentDeviceName = value; }
    }


    /// <summary>
    /// switches between different cameras
    /// </summary>
    public void SwitchCamera()
    {
        CameraTexture.Stop();
        Debug.Log("stop");
        if (Devices.Length > 0)
        {
            mCurrentDeviceIndex++;
            if (mCurrentDeviceIndex >= Devices.Length)
            {
                mCurrentDeviceIndex = 0;
            }
            CameraTexture.deviceName = DeviceName;

            SetRendererTexture();
            DisplayCameraStream();
        }
    }




    /// <summary>
    /// displays a camera stream
    /// </summary>
    public void DisplayCameraStream()
    {
        if (CameraTexture != null)
        {
            SetRendererTexture();
            CameraTexture.Play();
            WebcamText.text = "current camera: " + DeviceName;
        }
    }

    private void SetRendererTexture()
    {
        //  RendererMaterial.material.mainTexture = CameraTexture;
        RawImage = GetComponent<RawImage>();
        RawImage.material.mainTexture = CameraTexture;
        RawImage.texture = CameraTexture;
    }

    public void HideCameraStream()
    {
        if (CameraTexture != null)
        {
            CameraTexture.Stop();
        }
         

    }



    private void Init()
    {
        UnityEngine.Camera cam = UnityEngine.Camera.main;
        SetRendererTexture();
        DisplayCameraStream();
    }

    void OnEnable()
    {
        InputHandler.RegisterKeyboardAction(KeyCode.F1,
            () =>
            {
                if (CameraTexture != null)
                {
                    SwitchCamera();
                }
            }
            );
        Init();
    }

    void OnDisable()
    {
        InputHandler.RemoveKeybinding(KeyCode.F1, () =>
        {
            if (CameraTexture != null)
            {
                SwitchCamera();
            }
        });
        HideCameraStream();

    }

    void Start()
    {
        InputHandler.RegisterKeyboardAction(KeyCode.F2, () =>
        {
            bool vIsActive = gameObject.activeInHierarchy; 
             gameObject.SetActive(!vIsActive);
        });
    }

}
