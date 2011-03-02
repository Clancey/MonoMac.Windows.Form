using System;
using MonoMac.AppKit;
using System.Drawing;
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
	
	
		
		public override void DrawRow (int row, Drawing.RectangleF clipRect)
		{
			bool shouldDraw = true;
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{	
				
				var events = new DrawItemEventArgs( graphics,this.Font.ToFont(), Rectangle.Round (clipRect),row,getState(row));
				if(Host is ListBox)
					((ListBox)Host).DrawItemInternal (events);				
				shouldDraw = !events.Handled;
			}
			if (shouldDraw)
				base.DrawRow (row, clipRect);
		}
		
		private DrawItemState getState(int row)
		{
			DrawItemState state = DrawItemState.None;
			if(Host is ListBox)
			{
				var lbox = (ListBox)Host;
				if (lbox.SelectedIndices.Contains (row))
					state |= DrawItemState.Selected;
					
				if (lbox.has_focus && lbox.FocusedItem == row)
					state |= DrawItemState.Focus;
			}
			return state;
		}
	}
}

