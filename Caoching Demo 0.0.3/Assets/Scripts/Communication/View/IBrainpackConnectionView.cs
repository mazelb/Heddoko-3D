/** 
* @file IBrainpackConnectionView.cs
* @brief Contains the IBrainpackConnectionView interface
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/

using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    /// <summary>
    /// Interface for brainpack connection views
    /// </summary>
    public interface IBrainpackConnectionView
    {
        /// <summary>
        /// Sets a message in the case of a warning
        /// </summary>
        /// <param name="vMsg"></param>
        void SetWarningBoxMessage(string vMsg);

        /// <summary>
        /// Displays/Shows the view  
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the view 
        /// </summary>
        void Hide();

        /// <summary>
        /// The rect transform to the view
        /// </summary>
        RectTransform RectTransform { get; }

        /// <summary>
        /// Display the connecting views
        /// </summary> 
        void OnConnection();

        /// <summary>
        /// on connect view
        /// </summary> 
        void OnConnect();

        /// <summary>
        ///  Display the failed Disconnected views
        /// </summary> 
        void OnDisconnect();


        /// <summary>
        ///  Display the failed connection views
        /// </summary> 
        void FailedConnection();

        /// <summary>
        ///  The pairing button has been engaged
        /// </summary>
        void PairButtonEngaged();
    }
}