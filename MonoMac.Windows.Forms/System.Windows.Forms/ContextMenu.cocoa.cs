// 
//  Copyright 2011  James Clancey
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.using System;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Drawing;

#if MAC64
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
using CGFloat = System.Double;
#else
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
using NSPoint = System.Drawing.PointF;
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using CGFloat = System.Single;
#endif

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
		ContextMenuHelper helper;
		void CreateHandle ()
		{
			helper = new ContextMenuHelper(this);
			m_view = helper;
		}
		
		
		public void Show (Control control, Point pos)
		{
			if (control == null)
				throw new ArgumentException ();

			src_control = control;
		
			OnPopup (EventArgs.Empty);
			//m_view.RemoveAllItems();
			//m_view.InsertItem("beep",null,"",0);


			NSPoint _pos = new NSPoint (pos.X, pos.Y);
			var point = control.m_view.ConvertPointToBase(_pos);
			NSMenu.PopUpContextMenu(m_view,NSEvent.MouseEvent(NSEventType.LeftMouseUp,point,NSEventModifierMask.Shift,0,NSApplication.SharedApplication.MainWindow.WindowNumber,new NSGraphicsContext(),0,1,1f),control,NSFont.MenuBarFontOfSize(12));
			//m_view.PopUpMenu(new NSMenuItem("test",""),pos,control);
		 
			
			OnCollapse (EventArgs.Empty);
		}
		
		internal override void OnMenuChanged (EventArgs e)
		{
			if(helper != null)
				helper.ItemsChanged();
			base.OnMenuChanged (e);
		}
	}
}

