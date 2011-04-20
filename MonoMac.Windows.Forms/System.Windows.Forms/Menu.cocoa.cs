using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public abstract partial class Menu
	{
		public static implicit operator NSMenu (Menu menu)
		{
			return menu.NSViewForControl;
		}
		
		internal NSMenu m_view;
		internal NSMenu NSViewForControl {
			get { return m_view; }
		}
	}
}

