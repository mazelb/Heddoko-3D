 
using UnityEngine;

namespace Assets.Scripts.Utils.UnityUtilities.Repos
{
    /// <summary>
    /// Represents a repo of prefabs
    /// </summary>
    public static class PrefabRepo
    {
        /// <summary>
        /// Returns the BrainpackInfoPanel prefab that contains a BrainpackConnectionViewComponent
        /// </summary>
        public static GameObject BrainpackConnectionViewPrefab
        {
            get
            {
                return Resources.Load<GameObject>("Prefabs/UI/BrainpackInfoPanel");
            } 
        }
        /// <summary>
        /// Returns the LoadingScreenPrefab prefab that contains the loading scene view
        /// </summary>
        public static GameObject LoadingScreenPrefab
        {
            get
            {
                return Resources.Load<GameObject>("Prefabs/UI/LoadingSceneObjs");
            }
        }
        /// <summary>
        /// Returns the WarningBoxPanel prefab that contains the brainpack warning view
        /// </summary>
        public static GameObject WarningIconPanelPrefab
        {
            get
            {
                return Resources.Load<GameObject>("Prefabs/UI/WarningBoxPanel");
            }
        }
    }
}
