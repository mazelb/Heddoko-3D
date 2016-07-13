// /**
// * @file UnityLoginControlExtension.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 06 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.Licensing.Controller;
using Assets.Scripts.Licensing.Model;

namespace Assets.Scripts.Licensing.Tests
{
    public class UnityLoginControlExtension : UnityLoginControl
    {
        public void RaiseLoginEvent(UserProfileModel vProfile)
        {
            if (LoginSuccessEvent != null)
            {
                LoginSuccessEvent(vProfile);
            }
        }
    }
}