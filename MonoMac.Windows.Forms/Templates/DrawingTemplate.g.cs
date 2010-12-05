using System;
using System.Collections;
using System.Linq;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	public partial class UserControl 
	{		
		
		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd(this.Handle))
			{
				var events = new PaintEventArgs(graphics,Rectangle.Round(dirtyRect));
				//graphics.TranslateTransform(dirtyRect.X , dirtyRect.Height + dirtyRect.Y);
				//graphics.ScaleTransform(1.0f,-1.0f);
				OnPaintBackground(events);
				OnPaint(events);
			}
		}
		
		protected virtual void OnPaint(PaintEventArgs e)
        {
			base.DrawRect(e.ClipRectangle);
        }
        
		protected virtual void OnPaintBackground(PaintEventArgs e)
		{
			if(BackColor == null)
				BackColor = Color.Transparent;
			if(BackColor == Color.Transparent)
				return;
			Pen pen = new Pen(BackColor);
			e.Graphics.DrawRectangle(pen,e.ClipRectangle);
		}
	}
	
	
	public partial class Button 
	{		
		
		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd(this.Handle))
			{
				var events = new PaintEventArgs(graphics,Rectangle.Round(dirtyRect));
				//graphics.TranslateTransform(dirtyRect.X , dirtyRect.Height + dirtyRect.Y);
				//graphics.ScaleTransform(1.0f,-1.0f);
				OnPaintBackground(events);
				OnPaint(events);
			}
		}
		
		protected virtual void OnPaint(PaintEventArgs e)
        {
			base.DrawRect(e.ClipRectangle);
        }
        
		protected virtual void OnPaintBackground(PaintEventArgs e)
		{
			if(BackColor == null)
				BackColor = Color.Transparent;
			if(BackColor == Color.Transparent)
				return;
			Pen pen = new Pen(BackColor);
			e.Graphics.DrawRectangle(pen,e.ClipRectangle);
		}
	}
	
	
	public partial class TextBox 
	{		
		
		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd(this.Handle))
			{
				var events = new PaintEventArgs(graphics,Rectangle.Round(dirtyRect));
				//graphics.TranslateTransform(dirtyRect.X , dirtyRect.Height + dirtyRect.Y);
				//graphics.ScaleTransform(1.0f,-1.0f);
				OnPaintBackground(events);
				OnPaint(events);
			}
		}
		
		protected virtual void OnPaint(PaintEventArgs e)
        {
			base.DrawRect(e.ClipRectangle);
        }
        
		protected virtual void OnPaintBackground(PaintEventArgs e)
		{
			if(BackColor == null)
				BackColor = Color.Transparent;
			if(BackColor == Color.Transparent)
				return;
			Pen pen = new Pen(BackColor);
			e.Graphics.DrawRectangle(pen,e.ClipRectangle);
		}
	}
	
	
	public partial class ComboBox 
	{		
		
		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd(this.Handle))
			{
				var events = new PaintEventArgs(graphics,Rectangle.Round(dirtyRect));
				//graphics.TranslateTransform(dirtyRect.X , dirtyRect.Height + dirtyRect.Y);
				//graphics.ScaleTransform(1.0f,-1.0f);
				OnPaintBackground(events);
				OnPaint(events);
			}
		}
		
		protected virtual void OnPaint(PaintEventArgs e)
        {
			base.DrawRect(e.ClipRectangle);
        }
        
		protected virtual void OnPaintBackground(PaintEventArgs e)
		{
			if(BackColor == null)
				BackColor = Color.Transparent;
			if(BackColor == Color.Transparent)
				return;
			Pen pen = new Pen(BackColor);
			e.Graphics.DrawRectangle(pen,e.ClipRectangle);
		}
	}
	
	
	public partial class ListBox 
	{		
		
		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd(this.Handle))
			{
				var events = new PaintEventArgs(graphics,Rectangle.Round(dirtyRect));
				//graphics.TranslateTransform(dirtyRect.X , dirtyRect.Height + dirtyRect.Y);
				//graphics.ScaleTransform(1.0f,-1.0f);
				OnPaintBackground(events);
				OnPaint(events);
			}
		}
		
		protected virtual void OnPaint(PaintEventArgs e)
        {
			base.DrawRect(e.ClipRectangle);
        }
        
		protected virtual void OnPaintBackground(PaintEventArgs e)
		{
			if(BackColor == null)
				BackColor = Color.Transparent;
			if(BackColor == Color.Transparent)
				return;
			Pen pen = new Pen(BackColor);
			e.Graphics.DrawRectangle(pen,e.ClipRectangle);
		}
	}
	
	
	public partial class TrackBar 
	{		
		
		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd(this.Handle))
			{
				var events = new PaintEventArgs(graphics,Rectangle.Round(dirtyRect));
				//graphics.TranslateTransform(dirtyRect.X , dirtyRect.Height + dirtyRect.Y);
				//graphics.ScaleTransform(1.0f,-1.0f);
				OnPaintBackground(events);
				OnPaint(events);
			}
		}
		
		protected virtual void OnPaint(PaintEventArgs e)
        {
			base.DrawRect(e.ClipRectangle);
        }
        
		protected virtual void OnPaintBackground(PaintEventArgs e)
		{
			if(BackColor == null)
				BackColor = Color.Transparent;
			if(BackColor == Color.Transparent)
				return;
			Pen pen = new Pen(BackColor);
			e.Graphics.DrawRectangle(pen,e.ClipRectangle);
		}
	}
	
	
}


