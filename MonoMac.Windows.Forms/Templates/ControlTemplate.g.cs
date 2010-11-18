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
		public string Name { get; set; }
		public SizeF Size {
			get { return this.Frame.Size; }
			set { this.Frame = new RectangleF (this.Frame.Location, value); }
		}

		public PointF Location {
			get { return this.Frame.Location; }
			set { this.Frame = new RectangleF (value, this.Frame.Size); }
		}
		
		public bool Visible{
			get{ return Hidden;}
			set {Hidden = value;}
		}
		public Rectangle ClientRectangle
		{
			get{ return Rectangle.Round(this.Bounds);}
			set{ this.Bounds = value;}
		}
		public SizeF ClientSize
		{
			get {return this.Bounds.Size;}
			set {this.SetBoundsSize(value);}
		}
				public float Width
		{
			get{return this.Size.Width;}
			set {this.Size = new SizeF(value,this.Size.Height);}
		}
		
		public float Height
		{
			get{return this.Size.Height;}
			set {this.Size = new SizeF(this.Size.Width,value);}
		}
		
		public BorderStyle BorderStyle{get;set;}
		
		public bool Focused
		{
			get {
				if(this.CurrentEditor == null)
					return false;
				return this.Window.FirstResponder == this.CurrentEditor;}
		}
		
		public virtual void Refresh()
		{
			this.Display();
		}
		public virtual void Invalidate(Region region)
		{
			var rect = region.GetBounds(Graphics.FromHwnd (this.Handle));
			this.SetNeedsDisplayInRect(rect);
		}
		
		public virtual void Invalidate()
		{
			this.SetNeedsDisplay();	
		}
		
		public new System.Drawing.Font Font
		{
			get {
				if(base.Font == null)
					return new System.Drawing.Font ("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				return new System.Drawing.Font(base.Font.FontName, base.Font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}
			set {
				base.Font = MonoMac.AppKit.NSFont.FromFontName(value.Name,value.Size);
				
			}
		}
		
		public EventHandler AutoSizeChanged{get;set;}
		public EventHandler AutoValidateChanged {get;set;}
		public MouseEventHandler MouseDown {get;set;}
		public MouseEventHandler MouseUp {get;set;}
		public EventHandler GotFocus {get;set;}
		public MouseEventHandler MouseMove {get;set;}
		public MouseEventHandler MouseDoubleClick{get;set;}
		public EventHandler SizeChanged {get;set;}
		
				
	}
	
	
	public partial class Button 
	{	
		public string Name { get; set; }
		public SizeF Size {
			get { return this.Frame.Size; }
			set { this.Frame = new RectangleF (this.Frame.Location, value); }
		}

		public PointF Location {
			get { return this.Frame.Location; }
			set { this.Frame = new RectangleF (value, this.Frame.Size); }
		}
		
		public bool Visible{
			get{ return Hidden;}
			set {Hidden = value;}
		}
		public Rectangle ClientRectangle
		{
			get{ return Rectangle.Round(this.Bounds);}
			set{ this.Bounds = value;}
		}
		public SizeF ClientSize
		{
			get {return this.Bounds.Size;}
			set {this.SetBoundsSize(value);}
		}
				public float Width
		{
			get{return this.Size.Width;}
			set {this.Size = new SizeF(value,this.Size.Height);}
		}
		
		public float Height
		{
			get{return this.Size.Height;}
			set {this.Size = new SizeF(this.Size.Width,value);}
		}
		
		public BorderStyle BorderStyle{get;set;}
		
		public bool Focused
		{
			get {
				if(this.CurrentEditor == null)
					return false;
				return this.Window.FirstResponder == this.CurrentEditor;}
		}
		
		public virtual void Refresh()
		{
			this.Display();
		}
		public virtual void Invalidate(Region region)
		{
			var rect = region.GetBounds(Graphics.FromHwnd (this.Handle));
			this.SetNeedsDisplayInRect(rect);
		}
		
		public virtual void Invalidate()
		{
			this.SetNeedsDisplay();	
		}
		
		public new System.Drawing.Font Font
		{
			get {
				if(base.Font == null)
					return new System.Drawing.Font ("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				return new System.Drawing.Font(base.Font.FontName, base.Font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}
			set {
				base.Font = MonoMac.AppKit.NSFont.FromFontName(value.Name,value.Size);
				
			}
		}
		
		public EventHandler AutoSizeChanged{get;set;}
		public EventHandler AutoValidateChanged {get;set;}
		public MouseEventHandler MouseDown {get;set;}
		public MouseEventHandler MouseUp {get;set;}
		public EventHandler GotFocus {get;set;}
		public MouseEventHandler MouseMove {get;set;}
		public MouseEventHandler MouseDoubleClick{get;set;}
		public EventHandler SizeChanged {get;set;}
		
				
	}
	
	
	public partial class TextBox 
	{	
		public string Name { get; set; }
		public SizeF Size {
			get { return this.Frame.Size; }
			set { this.Frame = new RectangleF (this.Frame.Location, value); }
		}

		public PointF Location {
			get { return this.Frame.Location; }
			set { this.Frame = new RectangleF (value, this.Frame.Size); }
		}
		
		public bool Visible{
			get{ return Hidden;}
			set {Hidden = value;}
		}
		public Rectangle ClientRectangle
		{
			get{ return Rectangle.Round(this.Bounds);}
			set{ this.Bounds = value;}
		}
		public SizeF ClientSize
		{
			get {return this.Bounds.Size;}
			set {this.SetBoundsSize(value);}
		}
				public float Width
		{
			get{return this.Size.Width;}
			set {this.Size = new SizeF(value,this.Size.Height);}
		}
		
		public float Height
		{
			get{return this.Size.Height;}
			set {this.Size = new SizeF(this.Size.Width,value);}
		}
		
		public BorderStyle BorderStyle{get;set;}
		
		public bool Focused
		{
			get {
				if(this.CurrentEditor == null)
					return false;
				return this.Window.FirstResponder == this.CurrentEditor;}
		}
		
		public virtual void Refresh()
		{
			this.Display();
		}
		public virtual void Invalidate(Region region)
		{
			var rect = region.GetBounds(Graphics.FromHwnd (this.Handle));
			this.SetNeedsDisplayInRect(rect);
		}
		
		public virtual void Invalidate()
		{
			this.SetNeedsDisplay();	
		}
		
		public new System.Drawing.Font Font
		{
			get {
				if(base.Font == null)
					return new System.Drawing.Font ("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				return new System.Drawing.Font(base.Font.FontName, base.Font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}
			set {
				base.Font = MonoMac.AppKit.NSFont.FromFontName(value.Name,value.Size);
				
			}
		}
		
		public EventHandler AutoSizeChanged{get;set;}
		public EventHandler AutoValidateChanged {get;set;}
		public MouseEventHandler MouseDown {get;set;}
		public MouseEventHandler MouseUp {get;set;}
		public EventHandler GotFocus {get;set;}
		public MouseEventHandler MouseMove {get;set;}
		public MouseEventHandler MouseDoubleClick{get;set;}
		public EventHandler SizeChanged {get;set;}
		
				
	}
	
	
}


