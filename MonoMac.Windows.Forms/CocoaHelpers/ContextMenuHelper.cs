using System;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
namespace System.Windows.Forms
{
	internal class ContextMenuHelper : NSMenu
	{
		public ContextMenuHelper (ContextMenu host) : base ()
		{
			Host = host;
			ItemsChanged();
		}
		internal Menu Host{get;private set;}
		
		public void ItemsChanged()
		{
			this.RemoveAllItems();
			foreach(MenuItem menuItem in Host.MenuItems)
			{
				this.AddItem(menuItem);	
			}
		}
	}
}

