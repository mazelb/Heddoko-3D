/*@file RestartBrainpackServiceTest.cs
* @brief Contains the RestartBrainpackServiceTest class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections;
using System.ServiceProcess;
using Assets.Scripts.Communication.Controller;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{

    public class RestartBrainpackServiceTest : MonoBehaviour
    {
        public Button RestartServiceButton;
        private ServiceController mServiceController;

        public void Awake()
        {
            mServiceController = new ServiceController("BrainpackService");
            RestartServiceButton.onClick.AddListener(RestartBrainpackService);
        }
        public void RestartBrainpackService()
        {
            RestartServiceButton.interactable = false;
            BrainpackConnectionController.Instance.ChangeCurrentState(BrainpackConnectionState.Disconnected);
            StartCoroutine(RestartBpService(1));
        }

        /// <summary>
        /// Restart the bp service. Timeout before failing
        /// </summary>
        /// <param name="vTimeout"></param>
        /// <returns></returns>
        private IEnumerator RestartBpService(float vTimeout)
        { 
            bool mStopSuccess = false;
            bool vStartSucess = false;
            try
            {
                mServiceController.Stop();
                mStopSuccess = true;
            }
            catch (Exception vE)
            {
                Debug.Log("Problem stopping" + vE);
                mStopSuccess = false;
            }

            if (mStopSuccess)
            {

                yield return new WaitForSeconds(vTimeout);
                //check status
                if (mServiceController.Status == ServiceControllerStatus.Stopped)
                {
                    Debug.Log("Service stopped.");
                    mStopSuccess = true; 
                } 
            }
            try
            {
                if (mStopSuccess)
                {
                    mServiceController.Start();
                    vStartSucess = true;
                }
            }

            catch (Exception vE)
            {
                vStartSucess = false;
                Debug.Log("Problem starting" + vE);
                throw;
            }
            if (vStartSucess && mStopSuccess)
            {
                yield return new WaitForSeconds(vTimeout);
                //check status
                if (mServiceController.Status == ServiceControllerStatus.Running)
                {
                    Debug.Log("Service started again."); 
                }

            }
            if (!mStopSuccess)
            {
                var message =
               "there was a problem with stopping the brainpack service. Verify that you are running the application in an administrative capacity or try again";
                Notify.Template("fade").Show(message, 4.5f, hideAnimation: Notify.FadeOutAnimation, showAnimation: Notify.FadeInAnimation, sequenceType: NotifySequence.First, clearSequence: true);
            }
            if (!vStartSucess)
            {
                var message =
               "there was a problem with starting the brainpack service. Verify that you are running the application in an administrative capacity or try again";
                Notify.Template("fade").Show(message, 4.5f, hideAnimation: Notify.FadeOutAnimation, showAnimation: Notify.FadeInAnimation, sequenceType: NotifySequence.First, clearSequence: true);
            }
            if (mStopSuccess && vStartSucess)
            {
                var message =
               "Service restarted";
                Notify.Template("fade").Show(message, 2.5f, hideAnimation: Notify.FadeOutAnimation, showAnimation: Notify.FadeInAnimation, sequenceType: NotifySequence.First, clearSequence: true);
            }
            RestartServiceButton.interactable = true;
        }

    }
}
