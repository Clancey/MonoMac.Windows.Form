using System;
using MonoMac.AppKit;
using System.Drawing;
namespace System.Windows.Forms
{
	public partial class ContextMenu : Menu
	{
		public ContextMenu () : base (null)
		{
			CreateHandle();
			tracker = new MenuTracker (this);
			right_to_left = RightToLeft.Inherit;
		}

		public ContextMenu (MenuItem [] menuItems) : base (menuItems)
		{
			CreateHandle();
			tracker = new MenuTracker (this);
			right_to_left = RightToLeft.Inherit;
			
		}

		void CreateHandle ()
		{
			ContextMenuHelper helper = new ContextMenuHelper();
			m_view = helper;
			helper.Host = this;
		}
		
		
		public void Show (Control control, Point pos)
		{
			if (control == null)
				throw new ArgumentException ();

			src_control = control;
		
			OnPopup (EventArgs.Empty);
			m_view.RemoveAllItems();
			m_view.InsertItem("beep",null,"",0);
			var point = control.m_view.ConvertPointToBase(pos);
			NSMenu.PopUpContextMenu(m_view,NSEvent.MouseEvent(NSEventType.LeftMouseUp,point,NSEventModifierMask.ShiftKeyMask,0,NSApplication.SharedApplication.MainWindow.WindowNumber,new NSGraphicsContext(),0,1,1f),control,NSFont.MenuBarFontOfSize(12));
			//m_view.PopUpMenu(new NSMenuItem("test",""),pos,control);
		 
			
			OnCollapse (EventArgs.Empty);
		}
		
	}
}

