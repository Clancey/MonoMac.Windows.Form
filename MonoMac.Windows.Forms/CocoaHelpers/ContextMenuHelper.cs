using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	internal class ContextMenuHelper : NSMenu
	{
		public ContextMenuHelper () : base ()
		{
			
		}
		internal Menu Host;
	}
}

