using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples {
	public class TreeViewSample : TreeViewCustom<TreeViewSampleComponent,ITreeViewSampleItem> {
		
		/// <summary>
		/// Sets vComponenent data with specified vItem.
		/// </summary>
		/// <param name="vComponent">Component.</param>
		/// <param name="vItem">Item.</param>
		protected override void SetData(TreeViewSampleComponent vComponent, ListNode<ITreeViewSampleItem> vItem)
		{
			vComponent.SetData(vItem.Node, vItem.Depth);
		}
		
		/// <summary>
		/// Set highlights colors of specified vComponenent.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(TreeViewSampleComponent component)
		{
			base.HighlightColoring(component);
			component.Text.color = HighlightedColor;
		}
		
		/// <summary>
		/// Set select colors of specified vComponenent.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void SelectColoring(TreeViewSampleComponent component)
		{
			base.SelectColoring(component);
			component.Text.color = SelectedColor;
		}
		
		/// <summary>
		/// Set default colors of specified vComponenent.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void DefaultColoring(TreeViewSampleComponent component)
		{
			base.DefaultColoring(component);
			component.Text.color = DefaultColor;
		}
	}
}