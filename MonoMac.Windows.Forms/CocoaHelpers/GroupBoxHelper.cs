using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	internal class GroupBoxHelper : NSBox, IViewHelper
	{
		public GroupBoxHelper (Control parent)
		{
			Host = parent;
			this.ContentView = new FlippedView();
		}

		#region IViewHelper implementation
		public NSCursor Cursor { get; set; }

		public Control Host { get; set; }
		#endregion
	}
}

