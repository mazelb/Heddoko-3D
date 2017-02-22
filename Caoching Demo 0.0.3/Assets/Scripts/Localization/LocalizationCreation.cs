// /**
// * @file LocalizationCreation.cs
// * @brief Contains the 
// * @author Mohammed Haider( 
// * @date 09 2016
// * Copyright Heddoko(TM) 2016,  all rights reserved
// */

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Localization
{
    public class LocalizationCreation : MonoBehaviour
    {
        void Awake()
        {
            string vPath = Application.persistentDataPath + Path.DirectorySeparatorChar+ "Localization.binder";
          //
         //   KeyMessage[] vKeys = new[]
         //   {KeyMessage.DownloadCompleteMsg, KeyMessage.Greet, KeyMessage.NewRecordingFoundMsg, KeyMessage.UploadCompleteMsg};
         //   //donwload complete
         //   LocalizedString v1 = new LocalizedString()
         //   {
         //       PluralForm = "All downloads have been completed",
         //       SingularForm = "Download has been completed"
         //   };

         //   //Greet
         //   LocalizedString v2 = new LocalizedString()
         //   {
         //       PluralForm = "Hello"
         //   };

         //   //New recording found
         //   LocalizedString v3 = new LocalizedString()
         //   {
         //       PluralForm = "New recordings have been found on the SD card",
         //       SingularForm = "A new recording has been found on the Heddoko SD card"
         //   };

         //   // Upload complete message
         //   LocalizedString v4 = new LocalizedString()
         //   {
         //       PluralForm = "All uploads have finished",
         //       SingularForm = "Your upload has been finished"
         //   };
         //   List<LocalizedString> vList = new List<LocalizedString>();
         //   vList.Add(v1);
         //   vList.Add(v2);
         //   vList.Add(v3);
         //   vList.Add(v4);

         //   LocalizationBinder vBinder = new LocalizationBinder();
         //   for (int vI = 0; vI < vList.Count; vI ++)
         //   {
         //       vBinder.StringContainer.Add(vKeys[vI],vList[vI]);
         //   }

         //   LocalizationBinderContainer vBinderContainer = new LocalizationBinderContainer();
         //   vBinderContainer.mContainer.Add("En-Us", vBinder);
         //var vJson =   Utils.JsonUtilities.SerializeObjToJson(vBinderContainer);
         //  // var vJson = JsonUtility.ToJson(vBinderContainer);
         //   using (System.IO.StreamWriter file = new System.IO.StreamWriter(vPath))
         //   {
         //       file.Write(vJson);
         //   }

            var vLoaded = Utils.JsonUtilities.JsonFileToObject<LocalizationBinderContainer>(vPath) ;
            LocalizationBinderContainer vInstantiatedBinder = vLoaded;
        }
    }
}