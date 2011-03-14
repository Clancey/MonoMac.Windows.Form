using System;
using MonoMac.AppKit;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
namespace System.Windows.Forms
{
	public class ToolBarHelper : NSToolbar, IViewHelper
	{
		public ToolStrip ToolStrip;
		public ToolBarHelper(ToolStrip parent)
		{
			Host = parent;
			this.Delegate = new ToolBarDelegate();
			this.AllowsUserCustomization = true;
			this.DisplayMode = NSToolbarDisplayMode.IconAndLabel;
		}
		
		internal class ToolBarDelegate : NSToolbarDelegate
		{
			public override NSToolbarItem WillInsertItem (NSToolbar toolbar, string itemIdentifier, bool willBeInserted)
			{
				var internalToolbar = (ToolBarHelper)toolbar;
				var item = internalToolbar.ToolStrip.Items[0];
				var toolbarItem = new NSToolbarItem(itemIdentifier);
				toolbarItem.Image = item.Image.ToNSImage();
				toolbarItem.Label = item.Text;
				toolbarItem.PaletteLabel = item.Text;
				toolbarItem.ToolTip = item.Text;
				toolbarItem.VisibilityPriority = 1;
				return toolbarItem;
				//throw new NotImplementedException ();
				//base.WillInsertItem(toolbar,itemIdentifier,willBeInserted);
			}
			public override string[] AllowedItemIdentifiers (NSToolbar toolbar)
			{
				var bar = toolbar as ToolBarHelper;
				return bar.Items.Select(x=> x.Label).ToArray();
				return new string[2]{"NSToolbarPrintItemIdentifier","&New Document"};
			}
			
			#region implemented abstract members of MonoMac.AppKit.NSToolbarDelegate
			public override string[] DefaultItemIdentifiers (NSToolbar toolbar)
			{
				return new string[]{"NSToolbarPrintItemIdentifier"};
			}
			
			
			public override string[] SelectableItemIdentifiers (NSToolbar toolbar)
			{
				throw new System.NotImplementedException();
			}
			
			
			public override void WillAddItem (MonoMac.Foundation.NSNotification notification)
			{
				throw new System.NotImplementedException();
			}
			
			
			public override void DidRemoveItem (MonoMac.Foundation.NSNotification notification)
			{
				throw new System.NotImplementedException();
			}
			
			#endregion
		}
	

		#region IViewHelper implementation
		public NSCursor Cursor {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public Control Host {
			get {return ToolStrip;}
			set {ToolStrip = (ToolStrip)value;	}
		}
		#endregion
}
}

