using System;
using System.Windows.Forms;
using MonoMac.AppKit;
using System.Drawing;

namespace System.Windows.Forms
{
	internal class TabViewItemHelper : NSTabViewItem , IViewHelper
	{
		public TabViewItemHelper (Control host) : base ()
		{
			Host = host;
			View = new ViewHelper(host);
			
		}
		
		#region IViewHelper implementation
		public NSCursor Cursor {get;set;}

		public Control Host {get;set;}
		#endregion
		#region IViewHelper implementation
		public void FontChanged ()
		{
			
		}
		
		#endregion
	}
}

