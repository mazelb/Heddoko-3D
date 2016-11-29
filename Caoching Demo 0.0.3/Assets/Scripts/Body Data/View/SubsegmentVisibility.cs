using UnityEngine;

namespace Assets.Scripts.Body_Data.View
{
    public class SubsegmentVisibility : MonoBehaviour
    {
        private int mMaterialIndex;
        private SkinnedMeshRenderer mSkinnedMeshRenderer;
        private bool mIsVisible = true;
        private Material mAssociatedMaterial;
        private Shader mInvisibleShader;
        private Shader mRegularShader;

        public void Initialize(int vMaterialIndex, SkinnedMeshRenderer vSkinnedMeshRenderer)
        {
            mMaterialIndex = vMaterialIndex;
            mSkinnedMeshRenderer = vSkinnedMeshRenderer;
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                mAssociatedMaterial = vSkinnedMeshRenderer.sharedMaterials[mMaterialIndex];
            }
            else
#endif
            mAssociatedMaterial = vSkinnedMeshRenderer.materials[mMaterialIndex];
            mInvisibleShader = Shader.Find("Mobile/Mobile-XrayEffect");
            mRegularShader = Shader.Find("Standard");
        }
        public bool IsVisible
        {
            get { return mIsVisible; }
            set
            {
                mIsVisible = value;
                if (mIsVisible)
                {
                    mAssociatedMaterial.shader = mRegularShader;
                }
                else
                {
                    mAssociatedMaterial.shader = mInvisibleShader;
                }
            }
        }

        public void ToggleVisiblity()
        {
            Debug.Log("toggling");
            IsVisible = !IsVisible;
        }
    }
}