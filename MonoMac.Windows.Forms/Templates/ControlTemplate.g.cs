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
		/*
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
		
		public AnchorStyles Anchor {get;set;}
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
		
		
		public virtual void Invalidate()
		{
			this.SetNeedsDisplay();	
		}
				
		public void Focus()
		{
			this.BecomeFirstResponder();
		}
		public virtual void Invalidate(Region region)
		{
			var rect = region.GetBounds(Graphics.FromHwnd (this.Handle));
			this.SetNeedsDisplayInRect(rect);
		}
		
		[Obsolete("Not Implemented.", false)]
		public int TabIndex {get;set;}
		
		[Obsolete("Not Implemented.", false)]
		public AutoScaleMode AutoScaleMode {get;set;}
		
		public event EventHandler AutoSizeChanged;
		public event EventHandler AutoValidateChanged;
		public event EventHandler GotFocus;
		public event EventHandler SizeChanged;
		
		#region Mouse
		
		// Mouse Down
		public override void FireMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void RightMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Right, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireOtherMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Middle, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		// Mouse Up
		public override void FireMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void RightMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Right, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireOtherMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Middle, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		// Mouse Move
		public override void FireMouseMoved (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));

		}
		
		public override void FireScrollWheel (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseWheel!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		
		public override void FireMouseEntered (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseEnter!= null)
				MouseEnter(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireMouseExited(NSEvent theEvent)
		{
			if(MouseLeave != null)
				MouseLeave(this,new EventArgs());
		}
		public override void FireMouseDragged(NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));	
		}
		public override void FireRightMouseDragged(NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));	
		}
		*/
		public event MouseEventHandler MouseDown;
		public event MouseEventHandler MouseUp;
		public event MouseEventHandler MouseMove;
		public event MouseEventHandler MouseDoubleClick;
		public event MouseEventHandler MouseWheel;
		public event EventHandler MouseEnter;
		public event EventHandler MouseHover;
		public event EventHandler MouseLeave;
		
		//#endregion
		
				
	}
	
	
	public partial class Button 
	{	
		/*
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
		
		public AnchorStyles Anchor {get;set;}
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
		
		
		public virtual void Invalidate()
		{
			this.SetNeedsDisplay();	
		}
				
		public void Focus()
		{
			this.BecomeFirstResponder();
		}
		public virtual void Invalidate(Region region)
		{
			var rect = region.GetBounds(Graphics.FromHwnd (this.Handle));
			this.SetNeedsDisplayInRect(rect);
		}
		
		[Obsolete("Not Implemented.", false)]
		public int TabIndex {get;set;}
		
		[Obsolete("Not Implemented.", false)]
		public AutoScaleMode AutoScaleMode {get;set;}
		
		public event EventHandler AutoSizeChanged;
		public event EventHandler AutoValidateChanged;
		public event EventHandler GotFocus;
		public event EventHandler SizeChanged;
		
		#region Mouse
		
		// Mouse Down
		public override void FireMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void RightMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Right, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireOtherMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Middle, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		// Mouse Up
		public override void FireMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void RightMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Right, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireOtherMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Middle, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		// Mouse Move
		public override void FireMouseMoved (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));

		}
		
		public override void FireScrollWheel (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseWheel!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		
		public override void FireMouseEntered (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseEnter!= null)
				MouseEnter(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireMouseExited(NSEvent theEvent)
		{
			if(MouseLeave != null)
				MouseLeave(this,new EventArgs());
		}
		public override void FireMouseDragged(NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));	
		}
		public override void FireRightMouseDragged(NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));	
		}
		*/
		public event MouseEventHandler MouseDown;
		public event MouseEventHandler MouseUp;
		public event MouseEventHandler MouseMove;
		public event MouseEventHandler MouseDoubleClick;
		public event MouseEventHandler MouseWheel;
		public event EventHandler MouseEnter;
		public event EventHandler MouseHover;
		public event EventHandler MouseLeave;
		
		//#endregion
				
	}
	
	
	public partial class TextBox 
	{	
		/*
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
		
		public AnchorStyles Anchor {get;set;}
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
		
		
		public virtual void Invalidate()
		{
			this.SetNeedsDisplay();	
		}
				
		public void Focus()
		{
			this.BecomeFirstResponder();
		}
		public virtual void Invalidate(Region region)
		{
			var rect = region.GetBounds(Graphics.FromHwnd (this.Handle));
			this.SetNeedsDisplayInRect(rect);
		}
		
		[Obsolete("Not Implemented.", false)]
		public int TabIndex {get;set;}
		
		[Obsolete("Not Implemented.", false)]
		public AutoScaleMode AutoScaleMode {get;set;}
		
		public event EventHandler AutoSizeChanged;
		public event EventHandler AutoValidateChanged;
		public event EventHandler GotFocus;
		public event EventHandler SizeChanged;
		
		#region Mouse
		
		// Mouse Down
		public override void FireMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void RightMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Right, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireOtherMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Middle, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		// Mouse Up
		public override void FireMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void RightMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Right, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireOtherMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Middle, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		// Mouse Move
		public override void FireMouseMoved (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));

		}
		
		public override void FireScrollWheel (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseWheel!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		
		public override void FireMouseEntered (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseEnter!= null)
				MouseEnter(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireMouseExited(NSEvent theEvent)
		{
			if(MouseLeave != null)
				MouseLeave(this,new EventArgs());
		}
		public override void FireMouseDragged(NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));	
		}
		public override void FireRightMouseDragged(NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));	
		}
		*/
		public event MouseEventHandler MouseDown;
		public event MouseEventHandler MouseUp;
		public event MouseEventHandler MouseMove;
		public event MouseEventHandler MouseDoubleClick;
		public event MouseEventHandler MouseWheel;
		public event EventHandler MouseEnter;
		public event EventHandler MouseHover;
		public event EventHandler MouseLeave;
		
		//#endregion
				
	}
	
	
	public partial class ComboBox 
	{	
		/*
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
		
		public AnchorStyles Anchor {get;set;}
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
		
		
		public virtual void Invalidate()
		{
			this.SetNeedsDisplay();	
		}
				
		public void Focus()
		{
			this.BecomeFirstResponder();
		}
		public virtual void Invalidate(Region region)
		{
			var rect = region.GetBounds(Graphics.FromHwnd (this.Handle));
			this.SetNeedsDisplayInRect(rect);
		}
		
		[Obsolete("Not Implemented.", false)]
		public int TabIndex {get;set;}
		
		[Obsolete("Not Implemented.", false)]
		public AutoScaleMode AutoScaleMode {get;set;}
		
		public event EventHandler AutoSizeChanged;
		public event EventHandler AutoValidateChanged;
		public event EventHandler GotFocus;
		public event EventHandler SizeChanged;
		
		#region Mouse
		
		// Mouse Down
		public override void FireMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void RightMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Right, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireOtherMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Middle, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		// Mouse Up
		public override void FireMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void RightMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Right, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireOtherMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Middle, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		// Mouse Move
		public override void FireMouseMoved (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));

		}
		
		public override void FireScrollWheel (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseWheel!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		
		public override void FireMouseEntered (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseEnter!= null)
				MouseEnter(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireMouseExited(NSEvent theEvent)
		{
			if(MouseLeave != null)
				MouseLeave(this,new EventArgs());
		}
		public override void FireMouseDragged(NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));	
		}
		public override void FireRightMouseDragged(NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));	
		}
		*/
		public event MouseEventHandler MouseDown;
		public event MouseEventHandler MouseUp;
		public event MouseEventHandler MouseMove;
		public event MouseEventHandler MouseDoubleClick;
		public event MouseEventHandler MouseWheel;
		public event EventHandler MouseEnter;
		public event EventHandler MouseHover;
		public event EventHandler MouseLeave;
		
		//#endregion
				
	}
	
	
	public partial class TrackBar 
	{	
		/*
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
		
		public AnchorStyles Anchor {get;set;}
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
		
		
		public virtual void Invalidate()
		{
			this.SetNeedsDisplay();	
		}
				
		public void Focus()
		{
			this.BecomeFirstResponder();
		}
		public virtual void Invalidate(Region region)
		{
			var rect = region.GetBounds(Graphics.FromHwnd (this.Handle));
			this.SetNeedsDisplayInRect(rect);
		}
		
		[Obsolete("Not Implemented.", false)]
		public int TabIndex {get;set;}
		
		[Obsolete("Not Implemented.", false)]
		public AutoScaleMode AutoScaleMode {get;set;}
		
		public event EventHandler AutoSizeChanged;
		public event EventHandler AutoValidateChanged;
		public event EventHandler GotFocus;
		public event EventHandler SizeChanged;
		
		#region Mouse
		
		// Mouse Down
		public override void FireMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void RightMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Right, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireOtherMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Middle, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		// Mouse Up
		public override void FireMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void RightMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Right, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireOtherMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Middle, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		// Mouse Move
		public override void FireMouseMoved (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));

		}
		
		public override void FireScrollWheel (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseWheel!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		
		public override void FireMouseEntered (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseEnter!= null)
				MouseEnter(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireMouseExited(NSEvent theEvent)
		{
			if(MouseLeave != null)
				MouseLeave(this,new EventArgs());
		}
		public override void FireMouseDragged(NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));	
		}
		public override void FireRightMouseDragged(NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));	
		}
		*/
		public event MouseEventHandler MouseDown;
		public event MouseEventHandler MouseUp;
		public event MouseEventHandler MouseMove;
		public event MouseEventHandler MouseDoubleClick;
		public event MouseEventHandler MouseWheel;
		public event EventHandler MouseEnter;
		public event EventHandler MouseHover;
		public event EventHandler MouseLeave;
		
		//#endregion
				
	}
	
	
	public partial class Panel 
	{	
		/*
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
		
		public AnchorStyles Anchor {get;set;}
		public BorderStyle BorderStyle{get;set;}
		
				public bool Focused
		{
			get {return this.Window.FirstResponder == this;}
		}
		
		public virtual void Refresh()
		{
			this.Display();
		}
		
		
		public virtual void Invalidate()
		{
			this.SetNeedsDisplayInRect(this.Frame);
		}
		
				
		public void Focus()
		{
			this.BecomeFirstResponder();
		}
		public virtual void Invalidate(Region region)
		{
			var rect = region.GetBounds(Graphics.FromHwnd (this.Handle));
			this.SetNeedsDisplayInRect(rect);
		}
		
		[Obsolete("Not Implemented.", false)]
		public int TabIndex {get;set;}
		
		[Obsolete("Not Implemented.", false)]
		public AutoScaleMode AutoScaleMode {get;set;}
		
		public event EventHandler AutoSizeChanged;
		public event EventHandler AutoValidateChanged;
		public event EventHandler GotFocus;
		public event EventHandler SizeChanged;
		
		#region Mouse
		
		// Mouse Down
		public override void FireMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void RightMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Right, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireOtherMouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseDown!= null)
				MouseDown(this,new MouseEventArgs (MouseButtons.Middle, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		// Mouse Up
		public override void FireMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void RightMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Right, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireOtherMouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseUp!= null)
				MouseUp(this,new MouseEventArgs (MouseButtons.Middle, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		// Mouse Move
		public override void FireMouseMoved (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));

		}
		
		public override void FireScrollWheel (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseWheel!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		
		public override void FireMouseEntered (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseEnter!= null)
				MouseEnter(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
		}
		public override void FireMouseExited(NSEvent theEvent)
		{
			if(MouseLeave != null)
				MouseLeave(this,new EventArgs());
		}
		public override void FireMouseDragged(NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));	
		}
		public override void FireRightMouseDragged(NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			if(MouseMove!= null)
				MouseMove(this,new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));	
		}
		*/
		public event MouseEventHandler MouseDown;
		public event MouseEventHandler MouseUp;
		public event MouseEventHandler MouseMove;
		public event MouseEventHandler MouseDoubleClick;
		public event MouseEventHandler MouseWheel;
		public event EventHandler MouseEnter;
		public event EventHandler MouseHover;
		public event EventHandler MouseLeave;
		
		//#endregion
				
	}
	
	
}


