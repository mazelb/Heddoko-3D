/** 
* @file AbstractSuitsSubControl.cs
* @brief Contains the AbstractSuitsSubControl abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using Assets.Scripts.Communication.Controller;
 

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.AbstractSuitSubControls
{

    /// <summary>
    /// Sub control class for any controls dealing with controlling suits
    /// </summary>
    public abstract class AbstractSuitsSubControl : AbstractSubControl
    {
        internal bool mIsConnectedToSuit;
        internal SuitState SuitState;

        public AbstractSuitConnection SuitConnection;
        
       public virtual void OnEnable()
        {
            //Listen to the AbstractSuitConnect OnConnect, OnDisconnect and OnStatusUpdate events
            SuitConnection.OnSuitStateUpdate += OnStatusUpdate;
            SuitConnection.ConnectedStateEvent += OnConnection;
            SuitConnection.DisconnectedStateEvent += OnDisconnect;
        }

        public virtual void OnDisable()
        {
            
            // ReSharper disable once DelegateSubtraction
            SuitConnection.OnSuitStateUpdate -= OnStatusUpdate;
            // ReSharper disable once DelegateSubtraction
            SuitConnection.ConnectedStateEvent -= OnConnection;
            // ReSharper disable once DelegateSubtraction
            SuitConnection.DisconnectedStateEvent -= OnDisconnect;
        }

        /// <summary>
        /// on suit disconnection
        /// </summary>
        public virtual void OnDisconnect()
        {
            mIsConnectedToSuit = false;
        }

        /// <summary>
        /// On status update
        /// </summary>
        /// <param name="vSuitState"></param>
        public abstract void OnStatusUpdate(SuitState vSuitState);

        /// <summary>
        /// on Connection
        /// </summary>
        public abstract void OnConnection();
         

    }
}
