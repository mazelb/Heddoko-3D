using System.IO;
using UnityEngine; 
using Assets.Scripts.UI.DemoKit;
using UnityEditor;


public class MakeRecordingAsset : EditorWindow  {

    [MenuItem("Window/Create Recording  Asset")]
    static void Init()
    {
        Startnt();
    }
    public  static void Startnt()
    {
        BodyFrameRecordingAsset vRecordingAsset = ScriptableObject.CreateInstance<BodyFrameRecordingAsset>();

        string vPath = LaunchWindow();
        if (vPath.Length > 0)
        {
          
            vRecordingAsset.Init(vPath);
            EditorUtility.SetDirty(vRecordingAsset);
            //Get the file name
            FileInfo vFileInfo = new FileInfo(vPath);

            AssetDatabase.CreateAsset(vRecordingAsset, "Assets/Resources/ScriptableAssets/"+ vFileInfo.Name+".asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = vRecordingAsset; EditorUtility.SetDirty(vRecordingAsset);
        }


    
    }

    private static  string LaunchWindow()
    {
        return EditorUtility.OpenFilePanel("Select Body Frame Recording path", "", "csv");
         

    }
}
