using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	internal class TabViewHelper : NSTabView , IViewHelper
	{
		#region IViewHelper implementation
		public NSCursor Cursor {get;set;}

		public Control Host {get;set;}
		#endregion
		public TabViewHelper (Control host) : base ()
		{
			 Host = host;
		}
	}
}

