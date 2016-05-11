using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples {
	public class TreeViewSampleComponent : TreeViewComponentBase<ITreeViewSampleItem> {
		ITreeViewSampleItem item;
		
		public ITreeViewSampleItem Item {
			get {
				return item;
			}
			set {
				if (item!=null)
				{
					item.OnChange -= UpdateView;
				}
				item = value;
				if (item!=null)
				{
					item.OnChange += UpdateView;
				}
				UpdateView();
			}
		}
		
		public override void SetData(TreeNode<ITreeViewSampleItem> vNode, int vDepth)
		{
			base.SetData(vNode, vDepth);
			
			Item = (vNode==null) ? null : vNode.Item;
		}
		
		protected virtual void UpdateView()
		{
			if (Item==null)
			{
				Icon.sprite = null;
				Text.text = string.Empty;
			}
			else
			{
				Item.Display(this);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (item!=null)
			{
				item.OnChange -= UpdateView;
			}
		}
	}
}