using System;
using MonoMac.AppKit;
using System.Linq;
using System.Collections.Generic;
namespace System.Windows.Forms
{
	public class ToolBarHelper : NSToolbar, IViewHelper
	{
		public ToolStrip ToolStrip;
		public ToolBarHelper()
		{
			this.Delegate = new ToolBarDelegate();	
		}
		
		internal class ToolBarDelegate : NSToolbarDelegate
		{
			public override NSToolbarItem WillInsertItem (NSToolbar toolbar, string itemIdentifier, bool willBeInserted)
			{
				throw new NotImplementedException ();
			}
			public override string[] AllowedItemIdentifiers (NSToolbar toolbar)
			{
				var bar = toolbar as ToolBarHelper;
				return bar.Items.Select(x=> x.Label).ToArray();
			}
			
			#region implemented abstract members of MonoMac.AppKit.NSToolbarDelegate
			public override string[] DefaultItemIdentifiers (NSToolbar toolbar)
			{
				return new string[]{"&Save Document"};
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

