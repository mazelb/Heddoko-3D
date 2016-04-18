using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor; 

// ReSharper disable once CheckNamespace
namespace Kender.uGUI
{
	[CustomEditor(typeof(ComboBox))]
	public class ComboBoxEditor : Editor 
	{
		public override void OnInspectorGUI()
		{
			var vComboBoxGo = target as ComboBox;

			var vAllowUpdate = vComboBoxGo != null && vComboBoxGo.transform.Find("Button") != null;

			if (vAllowUpdate)
				vComboBoxGo.UpdateGraphics();
			
			EditorGUI.BeginChangeCheck();
			DrawDefaultInspector();
			if (EditorGUI.EndChangeCheck())
			{
				if (Application.isPlaying)
				{
				    if (vComboBoxGo != null)
				    {
				        vComboBoxGo.HideFirstItem = vComboBoxGo.HideFirstItem;
				        vComboBoxGo.Interactable = vComboBoxGo.Interactable;
				    }
				}
				else
					if (vAllowUpdate)
						vComboBoxGo.RefreshSelected();
			}
		}
	}

	public class ComboBoxMenuItem
	{
		[MenuItem("GameObject/UI/ComboBox")]
		public static void CreateComboBox()
		{
			var vCanvas = Object.FindObjectOfType<Canvas>();
			var vCanvasGo = vCanvas == null ? null : vCanvas.gameObject;
			if (vCanvasGo == null)
			{
				vCanvasGo = new GameObject("Canvas");
				vCanvas = vCanvasGo.AddComponent<Canvas>();
				vCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
				vCanvasGo.AddComponent<CanvasScaler>();
				vCanvasGo.AddComponent<GraphicRaycaster>();
			}
			var vEventSystem = Object.FindObjectOfType<EventSystem>();
			var vEventSystemGo = vEventSystem == null ? null : vEventSystem.gameObject;
			if (vEventSystemGo == null)
			{
				vEventSystemGo = new GameObject("EventSystem");
			    // ReSharper disable once RedundantAssignment
				vEventSystem = vEventSystemGo.AddComponent<EventSystem>();
				vEventSystemGo.AddComponent<StandaloneInputModule>();
				vEventSystemGo.AddComponent<TouchInputModule>();
			}
			var vComboBoxGo = new GameObject("ComboBox");
			vComboBoxGo.transform.SetParent(vCanvasGo.transform, false);
			var vRTransform = vComboBoxGo.AddComponent<RectTransform>();
			vRTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160);
			vRTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
			for (var vI = 0; vI < Selection.objects.Length; vI++)
			{
				var vSelected = Selection.objects[vI] as GameObject;
			    if (vSelected != null)
			    {
			        var vHierarchyItem = vSelected.transform;
			        vCanvas = null;
			        while (vHierarchyItem != null && (vCanvas = vHierarchyItem.GetComponent<Canvas>()) == null)
			            vHierarchyItem = vHierarchyItem.parent;
			    }
			    if (vCanvas != null)
			    {
			        if (vSelected != null) vComboBoxGo.transform.SetParent(vSelected.transform, false);
			        break;
			    }
			}
			vRTransform.anchoredPosition = Vector2.zero;
			var vComboBox = vComboBoxGo.AddComponent<ComboBox>();
			LoadAssets();
			vComboBox.SpriteUiSprite = sSpriteUiSprite;
			vComboBox.SpriteBackground = sSpriteBackground;
			vComboBox.CreateControl();
			Selection.activeGameObject = vComboBoxGo;
		}

		private static Sprite sSpriteUiSprite;
		private static Sprite sSpriteBackground;
		public static void LoadAssets()
		{
			while (sSpriteUiSprite == null || sSpriteBackground == null)
			{
				var vSprites = Resources.FindObjectsOfTypeAll<Sprite>();
				foreach (var vSprite in vSprites)
					switch (vSprite.name)
					{
						case "UISprite":
							sSpriteUiSprite = vSprite;
							break;
						case "Background":
							sSpriteBackground = vSprite;
							break;
					}
				if (sSpriteUiSprite == null || sSpriteBackground == null)
					AssetDatabase.LoadAllAssetsAtPath("Resources/unity_builtin_extra");
			}
		}
	}
}