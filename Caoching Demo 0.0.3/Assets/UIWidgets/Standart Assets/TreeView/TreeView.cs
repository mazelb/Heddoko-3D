using UnityEngine;

namespace UIWidgets {
	/// <summary>
	/// TreeView.
	/// </summary>
	[AddComponentMenu("UI/TreeView", 252)]
	public class TreeView : TreeViewCustom<TreeViewComponent,TreeViewItem> {

		/// <summary>
		/// Sets vComponenent data with specified vItem.
		/// </summary>
		/// <param name="vComponent">Component.</param>
		/// <param name="vItem">Item.</param>
		protected override void SetData(TreeViewComponent vComponent, ListNode<TreeViewItem> vItem)
		{
			vComponent.SetData(vItem.Node, vItem.Depth);
		}
		
		/// <summary>
		/// Set highlights colors of specified vComponenent.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(TreeViewComponent component)
		{
			base.HighlightColoring(component);
			component.Text.color = HighlightedColor;
		}
		
		/// <summary>
		/// Set select colors of specified vComponenent.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void SelectColoring(TreeViewComponent component)
		{
			base.SelectColoring(component);
			component.Text.color = SelectedColor;
		}
		
		/// <summary>
		/// Set default colors of specified vComponenent.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void DefaultColoring(TreeViewComponent component)
		{
			if (component==null)
			{
				return ;
			}
			base.DefaultColoring(component);
			if (component.Text!=null)
			{
				component.Text.color = DefaultColor;
			}
		}
		
		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/TreeView", false, 1190)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("TreeView");
		}
		#endif
	}
}