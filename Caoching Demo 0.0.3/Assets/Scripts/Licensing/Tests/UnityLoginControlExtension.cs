// /**
// * @file UnityLoginControlExtension.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 06 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using Assets.Scripts.Licensing.Authentication;
using Assets.Scripts.Licensing.Model;

namespace Assets.Scripts.Tests
{
    public class UnityLoginControlExtension : UnityLoginControl
    {
        public void RaiseLoginEvent(UserProfileModel vProfile)
        {
            if (base.LoginSuccessEvent != null)
            {
                LoginSuccessEvent(vProfile);
            }
        }
    }
}