using System;
using MonoMac.AppKit;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
namespace System.Windows.Forms
{
	public class ToolBarHelper : NSMenu, IViewHelper
	{
		public ToolStrip ToolStrip;
		public ToolBarHelper(ToolStrip parent)
		{
			Host = parent;
			//this.AllowsUserCustomization = true;
			//this.DisplayMode = NSToolbarDisplayMode.IconAndLabel;
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
		#region IViewHelper implementation
		public void FontChanged ()
		{
			
		}
		
		#endregion
}
}

