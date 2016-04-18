using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
namespace Kender.uGUI
{
    [RequireComponent(typeof(RectTransform))]
    public class ComboBox : MonoBehaviour
    {
        public Sprite SpriteUiSprite;
        public Sprite SpriteBackground;
        public Action<int> OnSelectionChanged;
        public Action<int> OnItemSelected;
        [SerializeField]
        private bool
            mInteractable = true;

        public bool Interactable
        {
            get
            {
                return mInteractable;
            }
            set
            {
                mInteractable = value;
                var vButton = ComboButtonRectTransform.GetComponent<Button>();
                vButton.interactable = mInteractable;
                var vImage = ComboImageRectTransform.GetComponent<Image>();
                vImage.color = vImage.sprite == null ? new Color(1.0f, 1.0f, 1.0f, 0.0f) : mInteractable ? vButton.colors.normalColor : vButton.colors.disabledColor;
                if (!Application.isPlaying)
                    return;
                if (!mInteractable && mOverlayGo.activeSelf)
                    ToggleComboBox(false);
            }
        }

        [SerializeField]
        private int
            mItemsToDisplay = 4;

        public int ItemsToDisplay
        {
            get
            {
                return mItemsToDisplay;
            }
            set
            {
                if (mItemsToDisplay == value)
                    return;
                mItemsToDisplay = value;
                Refresh();
            }
        }

        [SerializeField]
        private bool
            mHideFirstItem;

        public bool HideFirstItem
        {
            get
            {
                return mHideFirstItem;
            }
            set
            {
                if (value)
                    mScrollOffset--;
                else
                    mScrollOffset++;
                mHideFirstItem = value;
                Refresh();
            }
        }

        [SerializeField]
        private int mSelectedIndex;

        public int SelectedIndex
        {
            get
            {
                return mSelectedIndex;
            }
            set
            {
                if (mSelectedIndex == value)
                    return;
                if (value > -1 && value < Items.Length)
                {
                    mSelectedIndex = value;
                    RefreshSelected();
                }
            }
        }

        [SerializeField]
        private ComboBoxItem[]
            mItems;

        public ComboBoxItem[] Items
        {
            get
            {
                if (mItems == null)
                    mItems = new ComboBoxItem[0];
                return mItems;
            }
            set
            {
                mItems = value;
                Refresh();
            }
        }

        private GameObject mOverlayGo;
        private GameObject mScrollPanelGo;
        Vector2 mLastScreenSize;
        private int mScrollOffset;
        private float _scrollbarWidth = 20.0f;
        private Transform mCanvasTransform;

        private Transform CanvasTransform
        {
            get
            {
                if (mCanvasTransform == null)
                {
                    mCanvasTransform = transform;
                    while (mCanvasTransform.GetComponent<Canvas>() == null)
                        mCanvasTransform = mCanvasTransform.parent;
                }
                return mCanvasTransform;
            }
        }

        private RectTransform mRectTransform;

        private RectTransform RectTransform
        {
            get
            {
                if (mRectTransform == null)
                    mRectTransform = GetComponent<RectTransform>();
                return mRectTransform;
            }
            set
            {
                mRectTransform = value;
            }
        }

        private RectTransform mButtonRectTransform;

        private RectTransform ButtonRectTransform
        {
            get
            {
                if (mButtonRectTransform == null)
                    mButtonRectTransform = RectTransform.Find("Button").GetComponent<RectTransform>();
                return mButtonRectTransform;
            }
            set
            {
                mButtonRectTransform = value;
            }
        }

        private RectTransform mComboButtonRectTransform;

        private RectTransform ComboButtonRectTransform
        {
            get
            {
                if (mComboButtonRectTransform == null)
                    mComboButtonRectTransform = ButtonRectTransform.Find("ComboButton").GetComponent<RectTransform>();
                return mComboButtonRectTransform;
            }
            set
            {
                mComboButtonRectTransform = value;
            }
        }

        private RectTransform mComboImageRectTransform;

        private RectTransform ComboImageRectTransform
        {
            get
            {
                if (mComboImageRectTransform == null)
                    mComboImageRectTransform = ComboButtonRectTransform.Find("Image").GetComponent<RectTransform>();
                return mComboImageRectTransform;
            }
            // ReSharper disable once UnusedMember.Local
            set
            {
                mComboImageRectTransform = value;
            }
        }

        private RectTransform mComboTextRectTransform;

        private RectTransform ComboTextRectTransform
        {
            get
            {
                if (mComboTextRectTransform == null)
                    mComboTextRectTransform = ComboButtonRectTransform.Find("Text").GetComponent<RectTransform>();
                return mComboTextRectTransform;
            }
            // ReSharper disable once UnusedMember.Local
            set
            {
                mComboTextRectTransform = value;
            }
        }

        private RectTransform mComboArrowRectTransform;

        private RectTransform ComboArrowRectTransform
        {
            get
            {
                if (mComboArrowRectTransform == null)
                    mComboArrowRectTransform = ButtonRectTransform.Find("Arrow").GetComponent<RectTransform>();
                return mComboArrowRectTransform;
            }
            // ReSharper disable once UnusedMember.Local
            set
            {
                mComboArrowRectTransform = value;
            }
        }

        private RectTransform mScrollPanelRectTransfrom;

        private RectTransform ScrollPanelRectTransfrom
        {
            get
            {
                if (mScrollPanelRectTransfrom == null)
                    mScrollPanelRectTransfrom = mOverlayGo.transform.Find("ScrollPanel").GetComponent<RectTransform>();
                return mScrollPanelRectTransfrom;
            }
            // ReSharper disable once UnusedMember.Local
            set
            {
                mScrollPanelRectTransfrom = value;
            }
        }

        private RectTransform mItemsRectTransfrom;

        private RectTransform ItemsRectTransfrom
        {
            get
            {
                if (mItemsRectTransfrom == null)
                    mItemsRectTransfrom = ScrollPanelRectTransfrom.Find("Items").GetComponent<RectTransform>();
                return mItemsRectTransfrom;
            }
            set
            {
                mItemsRectTransfrom = value;
            }
        }

        private RectTransform mScrollbarRectTransfrom;

        private RectTransform ScrollbarRectTransfrom
        {
            get
            {
                if (mScrollbarRectTransfrom == null)
                    mScrollbarRectTransfrom = ScrollPanelRectTransfrom.Find("Scrollbar").GetComponent<RectTransform>();
                return mScrollbarRectTransfrom;
            }
            // ReSharper disable once UnusedMember.Local
            set
            {
                mScrollbarRectTransfrom = value;
            }
        }

        private RectTransform mSlidingAreaRectTransform;

        private RectTransform SlidingAreaRectTransform
        {
            get
            {
                if (mSlidingAreaRectTransform == null)
                    mSlidingAreaRectTransform = ScrollbarRectTransfrom.Find("SlidingArea").GetComponent<RectTransform>();
                return mSlidingAreaRectTransform;
            }
            set
            {
                mSlidingAreaRectTransform = value;
            }
        }

        private RectTransform mHandleRectTransfrom;

        private RectTransform HandleRectTransfrom
        {
            get
            {
                if (mHandleRectTransfrom == null)
                    mHandleRectTransfrom = SlidingAreaRectTransform.Find("Handle").GetComponent<RectTransform>();
                return mHandleRectTransfrom;
            }
            // ReSharper disable once UnusedMember.Local
            set
            {
                mHandleRectTransfrom = value;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            InitControl();
            mLastScreenSize = new Vector2(Screen.width, Screen.height);
        }

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            mScrollPanelGo.transform.SetParent(mOverlayGo.transform, true);
        }

        public void OnItemClicked(int index)
        {
            var vSelectionChanged = index != SelectedIndex;
            SelectItem(index);
            ToggleComboBox(true);
            if (vSelectionChanged && OnSelectionChanged != null)
                OnSelectionChanged(index);
        }

        public void SelectItem(int index)
        {
            SelectedIndex = index;
            if (OnItemSelected != null)
                OnItemSelected(index);
        }

        public void AddItems(params object[] vList)
        {
            var cbItems = new List<ComboBoxItem>();
            foreach (var obj in vList)
            {
                if (obj is ComboBoxItem)
                {
                    var vItem = (ComboBoxItem)obj;
                    cbItems.Add(vItem);
                    continue;
                }
                if (obj is string)
                {
                    var vItem = new ComboBoxItem((string)obj, null, false, null);
                    cbItems.Add(vItem);
                    continue;
                }
                if (obj is Sprite)
                {
                    var item = new ComboBoxItem(null, (Sprite)obj, false, null);
                    cbItems.Add(item);
                    continue;
                }
                throw new Exception("Only ComboBoxItem, string and Sprite types are allowed");
            }
            var vNewItems = new ComboBoxItem[Items.Length + cbItems.Count];
            Items.CopyTo(vNewItems, 0);
            cbItems.ToArray().CopyTo(vNewItems, Items.Length);
            Refresh();
            Items = vNewItems;
        }

        public void ClearItems()
        {
            Items = new ComboBoxItem[0];
        }

        public void CreateControl()
        {
            RectTransform = GetComponent<RectTransform>();

            var buttonGO = new GameObject("Button");
            buttonGO.transform.SetParent(transform, false);
            ButtonRectTransform = buttonGO.AddComponent<RectTransform>();
            ButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, RectTransform.sizeDelta.x);
            ButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, RectTransform.sizeDelta.y);
            ButtonRectTransform.anchoredPosition = Vector2.zero;

            var comboButtonGO = new GameObject("ComboButton");
            comboButtonGO.transform.SetParent(ButtonRectTransform, false);
            ComboButtonRectTransform = comboButtonGO.AddComponent<RectTransform>();
            ComboButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ButtonRectTransform.sizeDelta.x);
            ComboButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ButtonRectTransform.sizeDelta.y);
            ComboButtonRectTransform.anchoredPosition = Vector2.zero;

            var comboButtonImage = comboButtonGO.AddComponent<Image>();
            comboButtonImage.sprite = SpriteUiSprite;
            comboButtonImage.type = Image.Type.Sliced;
            var comboButtonButton = comboButtonGO.AddComponent<Button>();
            comboButtonButton.targetGraphic = comboButtonImage;
            var comboButtonColors = new ColorBlock();
            comboButtonColors.normalColor = new Color32(255, 255, 255, 255);
            comboButtonColors.highlightedColor = new Color32(245, 245, 245, 255);
            comboButtonColors.pressedColor = new Color32(200, 200, 200, 255);
            comboButtonColors.disabledColor = new Color32(200, 200, 200, 128);
            comboButtonColors.colorMultiplier = 1.0f;
            comboButtonColors.fadeDuration = 0.1f;
            comboButtonButton.colors = comboButtonColors;

            var comboArrowGO = new GameObject("Arrow");
            comboArrowGO.transform.SetParent(ButtonRectTransform, false);
            var comboArrowText = comboArrowGO.AddComponent<Text>();
            comboArrowText.color = new Color32(0, 0, 0, 255);
            comboArrowText.alignment = TextAnchor.MiddleCenter;
            comboArrowText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            comboArrowText.text = "▼";
            ComboArrowRectTransform.localScale = new Vector3(1.0f, 0.5f, 1.0f);
            ComboArrowRectTransform.pivot = new Vector2(1.0f, 0.5f);
            ComboArrowRectTransform.anchorMin = Vector2.right;
            ComboArrowRectTransform.anchorMax = Vector2.one;
            ComboArrowRectTransform.anchoredPosition = Vector2.zero;
            ComboArrowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ComboButtonRectTransform.sizeDelta.y);
            ComboArrowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ComboButtonRectTransform.sizeDelta.y);
            var comboArrowCanvasGroup = comboArrowGO.AddComponent<CanvasGroup>();
            comboArrowCanvasGroup.interactable = false;
            comboArrowCanvasGroup.blocksRaycasts = false;

            var comboImageGO = new GameObject("Image");
            comboImageGO.transform.SetParent(ComboButtonRectTransform, false);
            var comboImageImage = comboImageGO.AddComponent<Image>();
            comboImageImage.color = new Color32(255, 255, 255, 0);
            ComboImageRectTransform.pivot = Vector2.up;
            ComboImageRectTransform.anchorMin = Vector2.zero;
            ComboImageRectTransform.anchorMax = Vector2.up;
            ComboImageRectTransform.anchoredPosition = new Vector2(4.0f, -4.0f);
            ComboImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ComboButtonRectTransform.sizeDelta.y - 8.0f);
            ComboImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ComboButtonRectTransform.sizeDelta.y - 8.0f);

            var comboTextGO = new GameObject("Text");
            comboTextGO.transform.SetParent(ComboButtonRectTransform, false);
            var comboTextText = comboTextGO.AddComponent<Text>();
            comboTextText.color = new Color32(0, 0, 0, 255);
            comboTextText.alignment = TextAnchor.MiddleLeft;
            comboTextText.lineSpacing = 1.2f;
            comboTextText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            ComboTextRectTransform.pivot = Vector2.up;
            ComboTextRectTransform.anchorMin = Vector2.zero;
            ComboTextRectTransform.anchorMax = Vector2.one;
            ComboTextRectTransform.anchoredPosition = new Vector2(10.0f, 0.0f);
            ComboTextRectTransform.offsetMax = new Vector2(4.0f, 0.0f);
            ComboTextRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ComboButtonRectTransform.sizeDelta.y);
        }

        private void InitControl()
        {
            var cbi = transform.Find("Button/ComboButton/Image");
            var cbt = transform.Find("Button/ComboButton/Text");
            var cba = transform.Find("Button/Arrow");
            if (cbi == null || cbt == null || cba == null)
            {
                foreach (Transform child in transform)
                    Destroy(child);
                CreateControl();
            }

            ComboButtonRectTransform.GetComponent<Button>().onClick.AddListener(() =>
            {
                ToggleComboBox(false);
            });
            var dropdownHeight = ComboButtonRectTransform.sizeDelta.y * Mathf.Min(ItemsToDisplay, Items.Length - (HideFirstItem ? 1 : 0));

            mOverlayGo = new GameObject("CBOverlay");
            mOverlayGo.SetActive(false);
            var overlayImage = mOverlayGo.AddComponent<Image>();
            overlayImage.color = new Color32(0, 0, 0, 0);
            mOverlayGo.transform.SetParent(CanvasTransform, false);
            var overlayRectTransform = mOverlayGo.GetComponent<RectTransform>();
            overlayRectTransform.anchorMin = Vector2.zero;
            overlayRectTransform.anchorMax = Vector2.one;
            overlayRectTransform.offsetMin = Vector2.zero;
            overlayRectTransform.offsetMax = Vector2.zero;
            var overlayButton = mOverlayGo.AddComponent<Button>();
            overlayButton.targetGraphic = overlayImage;
            overlayButton.onClick.AddListener(() =>
            {
                ToggleComboBox(false);
            });

            mScrollPanelGo = new GameObject("ScrollPanel");
            var scrollPanelImage = mScrollPanelGo.AddComponent<Image>();
            scrollPanelImage.sprite = SpriteUiSprite;
            scrollPanelImage.type = Image.Type.Sliced;
            mScrollPanelGo.transform.SetParent(mOverlayGo.transform, false);
            ScrollPanelRectTransfrom.pivot = Vector2.zero;
            ScrollPanelRectTransfrom.anchorMin = Vector2.zero;
            ScrollPanelRectTransfrom.anchorMax = Vector2.one;
            mScrollPanelGo.transform.SetParent(transform, false);
            ScrollPanelRectTransfrom.anchoredPosition = new Vector2(0.0f, -RectTransform.sizeDelta.y * mItemsToDisplay);

            ScrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ComboButtonRectTransform.sizeDelta.x);
            ScrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
            var scrollPanelScrollRect = mScrollPanelGo.AddComponent<ScrollRect>();
            scrollPanelScrollRect.horizontal = false;
            scrollPanelScrollRect.elasticity = 0.0f;
            scrollPanelScrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollPanelScrollRect.inertia = false;
            scrollPanelScrollRect.scrollSensitivity = ComboButtonRectTransform.sizeDelta.y;
            mScrollPanelGo.AddComponent<Mask>();

            var scrollbarWidth = Items.Length - (HideFirstItem ? 1 : 0) > mItemsToDisplay ? _scrollbarWidth : 0.0f;

            var itemsGO = new GameObject("Items");
            itemsGO.transform.SetParent(mScrollPanelGo.transform, false);
            ItemsRectTransfrom = itemsGO.AddComponent<RectTransform>();
            ItemsRectTransfrom.pivot = Vector2.up;
            ItemsRectTransfrom.anchorMin = Vector2.up;
            ItemsRectTransfrom.anchorMax = Vector2.one;
            ItemsRectTransfrom.anchoredPosition = Vector2.right;
            ItemsRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ScrollPanelRectTransfrom.sizeDelta.x - scrollbarWidth);
            var itemsContentSizeFitter = itemsGO.AddComponent<ContentSizeFitter>();
            itemsContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            itemsContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var itemsGridLayoutGroup = itemsGO.AddComponent<GridLayoutGroup>();
            itemsGridLayoutGroup.cellSize = new Vector2(ComboButtonRectTransform.sizeDelta.x - scrollbarWidth, ComboButtonRectTransform.sizeDelta.y);
            itemsGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            itemsGridLayoutGroup.constraintCount = 1;
            scrollPanelScrollRect.content = ItemsRectTransfrom;

            var scrollbarGO = new GameObject("Scrollbar");
            scrollbarGO.transform.SetParent(mScrollPanelGo.transform, false);
            var scrollbarImage = scrollbarGO.AddComponent<Image>();
            scrollbarImage.sprite = SpriteBackground;
            scrollbarImage.type = Image.Type.Sliced;
            var scrollbarScrollbar = scrollbarGO.AddComponent<Scrollbar>();
            var scrollbarColors = new ColorBlock();
            scrollbarColors.normalColor = new Color32(128, 128, 128, 128);
            scrollbarColors.highlightedColor = new Color32(128, 128, 128, 178);
            scrollbarColors.pressedColor = new Color32(88, 88, 88, 178);
            scrollbarColors.disabledColor = new Color32(64, 64, 64, 128);
            scrollbarColors.colorMultiplier = 2.0f;
            scrollbarColors.fadeDuration = 0.1f;
            scrollbarScrollbar.colors = scrollbarColors;
            scrollPanelScrollRect.verticalScrollbar = scrollbarScrollbar;
            scrollbarScrollbar.direction = Scrollbar.Direction.BottomToTop;
            ScrollbarRectTransfrom.pivot = Vector2.one;
            ScrollbarRectTransfrom.anchorMin = Vector2.one;
            ScrollbarRectTransfrom.anchorMax = Vector2.one;
            ScrollbarRectTransfrom.anchoredPosition = Vector2.zero;
            ScrollbarRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
            ScrollbarRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);

            var slidingAreaGO = new GameObject("SlidingArea");
            slidingAreaGO.transform.SetParent(scrollbarGO.transform, false);
            SlidingAreaRectTransform = slidingAreaGO.AddComponent<RectTransform>();
            SlidingAreaRectTransform.anchoredPosition = Vector2.zero;
            SlidingAreaRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            SlidingAreaRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight - ScrollbarRectTransfrom.sizeDelta.x);

            var handleGO = new GameObject("Handle");
            handleGO.transform.SetParent(slidingAreaGO.transform, false);
            var handleImage = handleGO.AddComponent<Image>();
            handleImage.sprite = SpriteUiSprite;
            handleImage.type = Image.Type.Sliced;
            handleImage.color = new Color32(255, 255, 255, 150);
            scrollbarScrollbar.targetGraphic = handleImage;
            scrollbarScrollbar.handleRect = HandleRectTransfrom;
            HandleRectTransfrom.pivot = new Vector2(0.5f, 0.5f);
            HandleRectTransfrom.anchorMin = new Vector2(0.5f, 0.5f);
            HandleRectTransfrom.anchorMax = new Vector2(0.5f, 0.5f);
            HandleRectTransfrom.anchoredPosition = Vector2.zero;
            HandleRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
            HandleRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scrollbarWidth);

            Interactable = Interactable;

            if (Items.Length < 1)
                return;
            Refresh();
        }

        private void Refresh()
        {
            var itemsGridLayoutGroup = ItemsRectTransfrom.GetComponent<GridLayoutGroup>();
            var itemsLength = Items.Length - (HideFirstItem ? 1 : 0);
            var dropdownHeight = ComboButtonRectTransform.sizeDelta.y * Mathf.Min(mItemsToDisplay, itemsLength);
            var scrollbarWidth = itemsLength > ItemsToDisplay ? _scrollbarWidth : 0.0f;
            ScrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
            ScrollbarRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
            ScrollbarRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
            SlidingAreaRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight - ScrollbarRectTransfrom.sizeDelta.x);
            ItemsRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ScrollPanelRectTransfrom.sizeDelta.x - scrollbarWidth);
            itemsGridLayoutGroup.cellSize = new Vector2(ComboButtonRectTransform.sizeDelta.x - scrollbarWidth, ComboButtonRectTransform.sizeDelta.y);
            for (var i = ItemsRectTransfrom.childCount - 1; i > -1; i--)
                DestroyImmediate(ItemsRectTransfrom.GetChild(0).gameObject);
            for (var i = 0; i < Items.Length; i++)
            {
                if (HideFirstItem && i == 0)
                    continue;
                var item = Items[i];
                item.OnUpdate = Refresh;
                var itemTransform = Instantiate(ComboButtonRectTransform) as Transform;
                itemTransform.SetParent(ItemsRectTransfrom, false);
                itemTransform.GetComponent<Image>().sprite = null;
                var itemText = itemTransform.Find("Text").GetComponent<Text>();
                itemText.text = item.Caption;
                if (item.IsDisabled)
                    itemText.color = new Color32(174, 174, 174, 255);
                var itemImage = itemTransform.Find("Image").GetComponent<Image>();
                itemImage.sprite = item.Image;
                itemImage.color = item.Image == null ? new Color32(255, 255, 255, 0) : item.IsDisabled ? new Color32(255, 255, 255, 147) : new Color32(255, 255, 255, 255);
                var itemButton = itemTransform.GetComponent<Button>();
                itemButton.interactable = !item.IsDisabled;
                var index = i;
                itemButton.onClick.AddListener(
                    delegate ()
                {
                    OnItemClicked(index);
                    if (item.OnSelect != null)
                        item.OnSelect();
                }
                );
            }
            RefreshSelected();
            UpdateComboBoxImages();
            FixScrollOffset();
            UpdateHandle();
        }

        public void RefreshSelected()
        {
            var comboButtonImage = ComboImageRectTransform.GetComponent<Image>();
            var item = SelectedIndex > -1 && SelectedIndex < Items.Length ? Items[SelectedIndex] : null;
            var includeImage = item != null && item.Image != null;
            comboButtonImage.sprite = includeImage ? item.Image : null;
            var comboButtonButton = ComboButtonRectTransform.GetComponent<Button>();
            comboButtonImage.color = includeImage ? (Interactable ? comboButtonButton.colors.normalColor : comboButtonButton.colors.disabledColor) : new Color(1.0f, 1.0f, 1.0f, 0);
            UpdateComboBoxImage(ComboButtonRectTransform, includeImage);
            ComboTextRectTransform.GetComponent<Text>().text = item != null ? item.Caption : "";
            if (!Application.isPlaying)
                return;
            var i = 0;
            foreach (Transform child in ItemsRectTransfrom)
            {
                comboButtonImage = child.GetComponent<Image>();
                comboButtonImage.color = SelectedIndex == i + (HideFirstItem ? 1 : 0) ? comboButtonButton.colors.highlightedColor : comboButtonButton.colors.normalColor;
                i++;
            }
        }

        private void UpdateComboBoxImages()
        {
            var includeImages = false;
            foreach (var item in Items)
            {
                if (item.Image != null)
                {
                    includeImages = true;
                    break;
                }
            }
            foreach (Transform child in ItemsRectTransfrom)
                UpdateComboBoxImage(child, includeImages);
        }

        private void UpdateComboBoxImage(Transform comboButton, bool includeImage)
        {
            comboButton.Find("Text").GetComponent<RectTransform>().offsetMin = Vector2.right * (includeImage ? ComboImageRectTransform.rect.width + 8.0f : 10.0f);
        }

        private void FixScrollOffset()
        {
            var selectedIndex = SelectedIndex + (HideFirstItem ? 1 : 0);
            if (selectedIndex < mScrollOffset)
                mScrollOffset = selectedIndex;
            else
                if (selectedIndex > mScrollOffset + ItemsToDisplay - 1)
                mScrollOffset = selectedIndex - ItemsToDisplay + 1;
            var itemsCount = Items.Length - (HideFirstItem ? 1 : 0);
            if (mScrollOffset > itemsCount - ItemsToDisplay)
                mScrollOffset = itemsCount - ItemsToDisplay;
            if (mScrollOffset < 0)
                mScrollOffset = 0;
            ItemsRectTransfrom.anchoredPosition = new Vector2(0.0f, mScrollOffset * RectTransform.sizeDelta.y);
        }

        private void ToggleComboBox(bool directClick)
        {
            mOverlayGo.SetActive(!mOverlayGo.activeSelf);
            if (mOverlayGo.activeSelf)
            {
                var itemsToDisplay = Mathf.Min(mItemsToDisplay, Items.Length - (HideFirstItem ? 1 : 0));

                ScrollPanelRectTransfrom.SetParent(transform, false);
                var dropdownHeight = ComboButtonRectTransform.sizeDelta.y * Mathf.Min(ItemsToDisplay, Items.Length - (HideFirstItem ? 1 : 0));
                ScrollPanelRectTransfrom.anchoredPosition = new Vector2(0.0f, -RectTransform.sizeDelta.y * itemsToDisplay);
                ScrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ComboButtonRectTransform.sizeDelta.x);
                ScrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
                ScrollPanelRectTransfrom.SetParent(mOverlayGo.GetComponent<RectTransform>(), true);

                FixScrollOffset();
            }
            else
            {
                if (directClick)
                    mScrollOffset = (int)Mathf.Round(ItemsRectTransfrom.anchoredPosition.y / RectTransform.sizeDelta.y);
            }
        }

        private void Update()
        {
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);

            if (this.mLastScreenSize != screenSize)
            {
                this.mLastScreenSize = screenSize;
                if (mOverlayGo.activeSelf)
                    UpdateGraphics();
            }
        }

        public void UpdateGraphics()
        {
            UpdateHandle();

            if (RectTransform.sizeDelta != ButtonRectTransform.sizeDelta && ButtonRectTransform.sizeDelta == ComboButtonRectTransform.sizeDelta)
            {
                ButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, RectTransform.sizeDelta.x);
                ButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, RectTransform.sizeDelta.y);
                ComboButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, RectTransform.sizeDelta.x);
                ComboButtonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, RectTransform.sizeDelta.y);
                ComboArrowRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, RectTransform.sizeDelta.y);
                ComboImageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ComboImageRectTransform.rect.height);
                ComboTextRectTransform.offsetMax = new Vector2(4.0f, 0.0f);
                if (mOverlayGo == null)
                    return;
                ScrollPanelRectTransfrom.SetParent(transform, true);
                ScrollPanelRectTransfrom.anchoredPosition = new Vector2(0.0f, -RectTransform.sizeDelta.y * mItemsToDisplay);
                var overlayRectTransform = mOverlayGo.GetComponent<RectTransform>();
                overlayRectTransform.SetParent(CanvasTransform, false);
                overlayRectTransform.offsetMin = Vector2.zero;
                overlayRectTransform.offsetMax = Vector2.zero;
                ScrollPanelRectTransfrom.SetParent(overlayRectTransform, true);
                ScrollPanelRectTransfrom.GetComponent<ScrollRect>().scrollSensitivity = ComboButtonRectTransform.sizeDelta.y;
                UpdateComboBoxImage(ComboButtonRectTransform, Items[SelectedIndex].Image != null);
                Refresh();
            }
            else
            {
                if (mOverlayGo == null)
                    return;
                ScrollPanelRectTransfrom.SetParent(transform, false);
                var dropdownHeight = ComboButtonRectTransform.sizeDelta.y * Mathf.Min(ItemsToDisplay, Items.Length - (HideFirstItem ? 1 : 0));
                ScrollPanelRectTransfrom.anchoredPosition = new Vector2(0.0f, -RectTransform.sizeDelta.y * mItemsToDisplay);
                ScrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ComboButtonRectTransform.sizeDelta.x);
                ScrollPanelRectTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
                ScrollPanelRectTransfrom.SetParent(mOverlayGo.GetComponent<RectTransform>(), true);
            }
        }

        private void UpdateHandle()
        {
            if (mOverlayGo == null)
                return;
            var scrollbarWidth = Items.Length - (HideFirstItem ? 1 : 0) > ItemsToDisplay ? _scrollbarWidth : 0.0f;
            HandleRectTransfrom.offsetMin = -scrollbarWidth / 2 * Vector2.one;
            HandleRectTransfrom.offsetMax = scrollbarWidth / 2 * Vector2.one;
        }
    }
}