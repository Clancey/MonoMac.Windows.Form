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
	internal partial class TableViewHelper : NSTableView, IViewHelper,  ITableViewHelper
	{
        public Color BackColor {get;set;}
		protected virtual void OnPaintBackground(PaintEventArgs e)
		{
			if(BackColor == null)
				BackColor = Color.Transparent;
			if(BackColor == Color.Transparent)
				return;
			Pen pen = new Pen(BackColor);
			e.Graphics.DrawRectangle(pen,e.ClipRectangle);
		}
	
	
		public override void DrawRow (NSInteger row, NSRect clipRect)
		{
			bool shouldDraw = true;
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{	
				
				var events = new DrawItemEventArgs(graphics,
				                                   this.Font.ToFont(),
				                                   Util.NSRectToRectangle(clipRect),
				                                   (int)row,
				                                   getState(row));
				if(Host is ListBox)
					((ListBox)Host).DrawItemInternal (events);				
				shouldDraw = !events.Handled;
			}
			if (shouldDraw)
				base.DrawRow (row, clipRect);
		}
		
		private DrawItemState getState(NSInteger row)
		{
			DrawItemState state = DrawItemState.None;
			if(Host is ListBox)
			{
				var lbox = (ListBox)Host;
				if (lbox.SelectedIndices.Contains ((int)row))
					state |= DrawItemState.Selected;
					
				if (lbox.has_focus && lbox.FocusedItem == row)
					state |= DrawItemState.Focus;
			}
			return state;
		}
	}
}

