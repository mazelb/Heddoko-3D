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
<<<<<<< HEAD
            if (LoginSuccessEvent != null)
=======
            if (base.mLoginSuccessEvent != null)
>>>>>>> 096bb2ae014b51e65bce63c5e77e735a22c23b39
            {
                mLoginSuccessEvent(vProfile);
            }
        }
    }
}