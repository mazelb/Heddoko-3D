/** 
* @file GridViewLayoutManager.cs
* @brief Contains the GridViewLayoutManager class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews;
using Assets.Scripts.UI.AbstractViews.Layouts;
using Assets.Scripts.UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Analysis
{
    /// <summary>
    /// Manages the layout of an analysis view
    /// </summary>
    public class GridViewLayoutManager : AbstractView
    {
        public PlayerStreamManager PlayerStreamManager;
        public Button BackButton;
        /// <summary>
        /// The container of which AnalysisSubviews will be sized from
        /// </summary>
        private RectTransform mContainer;

        /// <summary>
        /// 
        /// </summary>
        public Vector2 ContainerRatio { get; set; }

        private GameObject mPlaybackPanel;
        /// <summary>
        /// The margin from the screen of the Rect's position
        /// </summary>
        public float[] Margin = { 0, 0, 0, 0 };

        private LayoutTypeModel mCurrentLayoutType;
        private Dictionary<LayoutType, LayoutTypeModel> mLayoutMapping = new Dictionary<LayoutType, LayoutTypeModel>();
        private List<GridViewCell> mSubviewComponents = new List<GridViewCell>();
        private HorizontalLayoutGroup vloay;
        public RectTransform Container
        {
            get
            {
                return mContainer;
            }
            set
            {
                mContainer = value;
                SetContainerRatio();
            }
        }

        void Awake()
        {
            HorizontalLayoutGroup dafda;
            BackButton.onClick.AddListener(Back);
            Container = GetComponent<RectTransform>();
            SetContainerRect(Margin);
            SubViewCameraPool.Depth = 3;
            SubViewCameraPool.CameraParent = GameObject.FindGameObjectWithTag("CameraGroup").transform;
            SubViewCameraPool.CullingMask = LayerMask.GetMask("UiGridLayer", "model");
            GameObject vCamLookAtTarget = GameObject.FindGameObjectWithTag("CamLookAtTarget");
            SubViewCameraPool.OriginalTarget = vCamLookAtTarget.transform;

            GridViewCellPool.GridViewCellParent = GetComponent<RectTransform>();

            //Register layouts
            //Starting from the left, add all the layout settings for the subviews
            LayoutTypeModel vOneLeftByTwoRightLayout = new LayoutTypeModel(LayoutType.OneLeftByTwoRight);
            LayoutTypeModel vOneRightByTwoLeftLayout = new LayoutTypeModel(LayoutType.OneRightByTwoLeft);
            LayoutTypeModel vHalfHalfVertical = new LayoutTypeModel(LayoutType.HalfHalfVertical);
            LayoutTypeModel vHalfHalfHorizontal = new LayoutTypeModel(LayoutType.HalfHalfHorizontal);
            LayoutTypeModel vSingle = new LayoutTypeModel(LayoutType.Single);

            mLayoutMapping.Add(vOneLeftByTwoRightLayout.LayoutType, vOneLeftByTwoRightLayout);
            mLayoutMapping.Add(vOneRightByTwoLeftLayout.LayoutType, vOneRightByTwoLeftLayout);
            mLayoutMapping.Add(vHalfHalfVertical.LayoutType, vHalfHalfVertical);
            mLayoutMapping.Add(vHalfHalfHorizontal.LayoutType, vHalfHalfHorizontal);
            mLayoutMapping.Add(vSingle.LayoutType, vSingle);

            vOneLeftByTwoRightLayout.Dimensions.Add(new LayoutDescription(Vector2.zero, new Vector2(0.5f, 1f), true));
            vOneLeftByTwoRightLayout.Dimensions.Add(new LayoutDescription(new Vector2(0.5f, 0.5f), Vector2.one, false));
            vOneLeftByTwoRightLayout.Dimensions.Add(new LayoutDescription(new Vector2(0.5f, 0.0f), new Vector2(1f, 0.5f), false));

            vOneRightByTwoLeftLayout.Dimensions.Add(new LayoutDescription(new Vector2(0.5f, 0f), Vector2.one, true));
            vOneRightByTwoLeftLayout.Dimensions.Add(new LayoutDescription(new Vector2(0.0f, 0.5f), new Vector2(0.5f, 1f), false));
            vOneRightByTwoLeftLayout.Dimensions.Add(new LayoutDescription(Vector2.zero, new Vector2(0.5f, 0.5f), false));

            vHalfHalfHorizontal.Dimensions.Add(new LayoutDescription(new Vector2(0.0f, 0.5f), Vector2.one, true));
            vHalfHalfHorizontal.Dimensions.Add(new LayoutDescription(Vector2.zero, new Vector2(1.0f, 0.5f), false));

            vHalfHalfVertical.Dimensions.Add(new LayoutDescription(Vector2.zero, new Vector2(0.5f, 1f), true));
            vHalfHalfVertical.Dimensions.Add(new LayoutDescription(new Vector2(0.5f, 0f), Vector2.one, false));

            vSingle.Dimensions.Add(new LayoutDescription(Vector2.zero, Vector2.one, true));
        }


        /// <summary>
        /// Sets the containers size Rect
        /// </summary>
        void SetContainerRect(float[] vMargin)
        {
            Margin = vMargin;
            Vector2 vMin = Vector2.zero;
            Vector2 vMax = Vector2.one;

            vMin.x += vMargin[0];
            vMin.y += vMargin[1];
            vMax.x -= vMargin[2];
            vMax.y -= vMargin[3];

            Container.anchorMin = vMin;
            Container.anchorMax = vMax;
            SetContainerRatio();
        }

        /// <summary>
        /// Sets the ratio of the container relative to the current rect transform
        /// </summary>
        private void SetContainerRatio()
        {
            float vRatioX = mContainer.anchorMax.x - mContainer.anchorMin.x;
            float vRatioY = mContainer.anchorMax.y - mContainer.anchorMin.y;
            ContainerRatio = new Vector2(vRatioX, vRatioY);
        }

        /// <summary>
        /// Switches to a different layout type
        /// </summary>
        /// <param name="vType"></param>
        public void SwitchLayoutTypes(LayoutType vType)
        {
            if (mCurrentLayoutType == null)
            {
                mCurrentLayoutType = mLayoutMapping[vType];
                RenderLayout(vType);
            }

            else if (mCurrentLayoutType.LayoutType != vType)
            {
                mCurrentLayoutType = mLayoutMapping[vType];
                RenderLayout(vType);
            }
        }

        /// <summary>
        /// Renders the layout based on the type passed
        /// </summary>
        /// <param name="vType"></param>
        private void RenderLayout(LayoutType vType)
        {
            //get count of the number of layouts from the requested type

            List<LayoutDescription> vDescriptions = mLayoutMapping[vType].Dimensions;
            //get the count of the number of dimensions, disables ones that don't have their dimensions registered
            int vCount = vDescriptions.Count;
            ValidateNewLayout(vCount);

            //resize the sub view Components and set the playback controls to one of the panels
            for (int i = 0; i < mSubviewComponents.Count; i++)
            {
                LayoutDescription vLayoutDesc = vDescriptions[i];
                mSubviewComponents[i].SetAnchors(vLayoutDesc.Min, vLayoutDesc.Max);
                if (vLayoutDesc.HasPlaybackPanel)
                {
                    if (mPlaybackPanel == null)
                    {
                        mPlaybackPanel = GameObject.Instantiate(Resources.Load("Prefabs/UI/PlaybackControlPanel")) as GameObject;
                    }
                    mPlaybackPanel.transform.parent = mSubviewComponents[i].transform;
                    RectTransform vRectTransform = mPlaybackPanel.GetComponent<RectTransform>();
                    vRectTransform.offsetMin = vRectTransform.offsetMax = new Vector2(0, 0);
                }
            }
        }

        public override void Hide()
        {
            for (int i = 0; i < mSubviewComponents.Count; i++)
            {
                GridViewCellPool.ReleaseSubview(mSubviewComponents[i]);
            }
            mSubviewComponents.Clear();
            gameObject.SetActive(false);
            BackButton.gameObject.SetActive(false);
            PlayerStreamManager.Stop();
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            if (mCurrentLayoutType == null)
            {
                mCurrentLayoutType = mLayoutMapping[LayoutType.OneLeftByTwoRight];
            }

            RenderLayout(mCurrentLayoutType.LayoutType);

            BackButton.gameObject.SetActive(true);
            for (int i = 0; i < mSubviewComponents.Count; i++)
            {
                mSubviewComponents[i].CameraControl.ResetPositionsAroundTarget();
            }
        }

        public override void CreateDefaultLayout()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Helper function that validates  the new layout set by render layout
        /// </summary>
        /// <param name="vCompareCount">the count of new subviews</param>
        private void ValidateNewLayout(int vCompareCount)
        {
            int vActiveSubViewCount = mSubviewComponents.Count;

            //case 1: count is 0
            if (vActiveSubViewCount == 0)
            {
                for (int i = 0; i < vCompareCount; i++)
                {
                    GridViewCell vSubView = GridViewCellPool.GetSubViewFromPool;
                    vSubView.Parent = this;
                    mSubviewComponents.Add(vSubView);
                }
            }

            //case 2: the number of current subview list exceeds the current count
            else if (vActiveSubViewCount > vCompareCount)
            {
                //get the difference
                int vDif = vActiveSubViewCount - vCompareCount;
                while (vDif > 0)
                {
                    vDif--;
                    //pop it from the list 
                    GridViewCell vSubViewCell = mSubviewComponents[0];
                    mSubviewComponents.RemoveAt(0);
                    GridViewCellPool.ReleaseSubview(vSubViewCell);
                }
            }
            //case 3: the number of current subview lists is less than the current count
            else if (vActiveSubViewCount < vCompareCount)
            {
                //get the difference
                int vDif = vCompareCount - vActiveSubViewCount;
                while (vDif > 0)
                {
                    vDif--;
                    GridViewCell vSubViewCell = GridViewCellPool.GetSubViewFromPool;
                    vSubViewCell.Parent = this;
                    mSubviewComponents.Add(vSubViewCell);
                }
            }
        }

        public LayoutType Type;

 

    }


    /// <summary>
    /// A mapping of a layouttype to an array of margins
    /// </summary>
    [Serializable]
    class LayoutTypeModel
    {
        private LayoutType mLayoutType;

        private List<LayoutDescription> mDimensions = new List<LayoutDescription>();


        public List<LayoutDescription> Dimensions
        {
            get { return mDimensions; }
        }

        public LayoutType LayoutType { get { return mLayoutType; } }

        public LayoutTypeModel(LayoutType vLayoutType)
        {
            mLayoutType = vLayoutType;
        }
    }

    /// <summary>
    /// A simple structure holding minimum and maximum Vector2 values
    /// </summary>
    struct LayoutDescription
    {
        public Vector2 Min;
        public Vector2 Max;
        public bool HasPlaybackPanel;
        public LayoutDescription(Vector2 vMin, Vector2 vMax, bool vHasPlaybackPanel)
        {
            Min = vMin;
            Max = vMax;
            HasPlaybackPanel = vHasPlaybackPanel;
        }
    }





}
