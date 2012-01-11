using System;
using System.Collections;
using System.Linq;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	internal partial class InternalUserControl 
	{	
		public override void DrawRect (RectangleF dirtyRect)
		{
		
			bool shouldDraw = true;
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
				shouldDraw = !events.Handled;
			}
			if (shouldDraw)
				base.DrawRect(dirtyRect);
		}
		/*
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
        */
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
		
		public void FontChanged()
		{
			this.Font = Host.font.ToNsFont();
		}
	}
	
	
	internal partial class ButtonHelper 
	{	
		public override void DrawRect (RectangleF dirtyRect)
		{
		
			bool shouldDraw = true;
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
				shouldDraw = !events.Handled;
			}
			if (shouldDraw)
				base.DrawRect(dirtyRect);
		}
		/*
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
        */
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
		
		public void FontChanged()
		{
			this.Font = Host.font.ToNsFont();
		}
	}
	
	
	internal partial class TextBoxMouseView 
	{	
		public override void DrawRect (RectangleF dirtyRect)
		{
		
			bool shouldDraw = true;
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
				shouldDraw = !events.Handled;
			}
			if (shouldDraw)
				base.DrawRect(dirtyRect);
		}
		/*
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
        */
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
		
		public void FontChanged()
		{
			this.Font = Host.font.ToNsFont();
		}
	}
	
	
	internal partial class ComboBoxHelper 
	{	
		public override void DrawRect (RectangleF dirtyRect)
		{
		
			bool shouldDraw = true;
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
				shouldDraw = !events.Handled;
			}
			if (shouldDraw)
				base.DrawRect(dirtyRect);
		}
		/*
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
        */
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
		
		public void FontChanged()
		{
			this.Font = Host.font;
		}
	}
	
	
	internal partial class ListBoxMouseView 
	{	
		public override void DrawRect (RectangleF dirtyRect)
		{
		
			bool shouldDraw = true;
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
				shouldDraw = !events.Handled;
			}
			if (shouldDraw)
				base.DrawRect(dirtyRect);
		}
		/*
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
        */
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
		
		public void FontChanged()
		{
			//this.Font = Host.font.ToNsFont();
		}
	}
	
	
	internal partial class TrackBarMouseView 
	{	
		public override void DrawRect (RectangleF dirtyRect)
		{
		
			bool shouldDraw = true;
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
				shouldDraw = !events.Handled;
			}
			if (shouldDraw)
				base.DrawRect(dirtyRect);
		}
		/*
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
        */
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
		
		public void FontChanged()
		{
			this.Font = Host.font.ToNsFont();
		}
	}
	
	
	internal partial class PanelMouseView 
	{	
		public override void DrawRect (RectangleF dirtyRect)
		{
		
			bool shouldDraw = true;
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
				shouldDraw = !events.Handled;
			}
			if (shouldDraw)
				base.DrawRect(dirtyRect);
		}
		/*
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
        */
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
		
		public void FontChanged()
		{
			//this.Font = Host.font.ToNsFont();
		}
	}
	
	
	internal partial class UserControlMouseView 
	{	
		public override void DrawRect (RectangleF dirtyRect)
		{
		
			bool shouldDraw = true;
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
				shouldDraw = !events.Handled;
			}
			if (shouldDraw)
				base.DrawRect(dirtyRect);
		}
		/*
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
        */
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
		
		public void FontChanged()
		{
			this.Font = Host.font.ToNsFont();
		}
	}
	
	
	internal partial class TextBoxHelper 
	{	
		public override void DrawRect (RectangleF dirtyRect)
		{
		
			bool shouldDraw = true;
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
				shouldDraw = !events.Handled;
			}
			if (shouldDraw)
				base.DrawRect(dirtyRect);
		}
		/*
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
        */
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
		
		public void FontChanged()
		{
			//this.Font = Host.font.ToNsFont();
		}
	}
	
	
	internal partial class ViewHelper 
	{	
		public override void DrawRect (RectangleF dirtyRect)
		{
		
			bool shouldDraw = true;
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Host.Draw (events);
				shouldDraw = !events.Handled;
			}
			if (shouldDraw)
				base.DrawRect(dirtyRect);
		}
		/*
		protected virtual void OnPaint(PaintEventArgs e)
        {
			if (Paint != null)
				Paint (this, e);
        	shouldDraw = true;
        }
        */
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
		
		public void FontChanged()
		{
			//this.Font = Host.font.ToNsFont();
		}
	}
	
	
}


