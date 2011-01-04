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
		public bool shouldDraw {get;set;}
		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
			}
			if (shouldDraw)
				base.DrawRect (dirtyRect);
		}
			
		public PaintEventHandler Paint { get; set; }
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
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
	}
	
	
	public partial class ButtonHelper 
	{	
		public bool shouldDraw {get;set;}
		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
			}
			if (shouldDraw)
				base.DrawRect (dirtyRect);
		}
			
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
	}
	
	
	public partial class TextBoxMouseView 
	{	
		public bool shouldDraw {get;set;}
		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
			}
			if (shouldDraw)
				base.DrawRect (dirtyRect);
		}
			
		public PaintEventHandler Paint { get; set; }
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
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
	}
	
	
	public partial class ComboBoxMouseView 
	{	
		public bool shouldDraw {get;set;}
		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
			}
			if (shouldDraw)
				base.DrawRect (dirtyRect);
		}
			
		public PaintEventHandler Paint { get; set; }
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
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
	}
	
	
	public partial class ListBoxMouseView 
	{	
		public bool shouldDraw {get;set;}
		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
			}
			if (shouldDraw)
				base.DrawRect (dirtyRect);
		}
			
		public PaintEventHandler Paint { get; set; }
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
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
	}
	
	
	public partial class TrackBarMouseView 
	{	
		public bool shouldDraw {get;set;}
		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
			}
			if (shouldDraw)
				base.DrawRect (dirtyRect);
		}
			
		public PaintEventHandler Paint { get; set; }
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
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
	}
	
	
	public partial class PanelMouseView 
	{	
		public bool shouldDraw {get;set;}
		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
			}
			if (shouldDraw)
				base.DrawRect (dirtyRect);
		}
			
		public PaintEventHandler Paint { get; set; }
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
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
	}
	
	
	public partial class UserControlMouseView 
	{	
		public bool shouldDraw {get;set;}
		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
			}
			if (shouldDraw)
				base.DrawRect (dirtyRect);
		}
			
		public PaintEventHandler Paint { get; set; }
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
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
	}
	
	
}


