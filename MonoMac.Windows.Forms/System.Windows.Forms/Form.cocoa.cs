using System;
using MonoMac.AppKit;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Drawing.Drawing2D;
namespace System.Windows.Forms
{
	public class View : NSView, IViewHelper
	{
		public View (Form parent)
		{
			Host = parent;
			this.AutoresizingMask = (NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable);
			
		}
		
		public override void ViewDidMoveToWindow ()
		{
			base.ViewDidMoveToWindow ();
		}
		
		public Control Host {get;set;}
		public NSCursor Cursor {get;set;}
		
		
		public override bool IsFlipped {
			get { return true; }
		}

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
		
		public override void MouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView (theEvent.LocationInWindow, null);
			
			var button = (MouseButtons)theEvent.ButtonNumber;
			this.Host.FireMouseUp (Host, new MouseEventArgs (button, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			base.MouseUp (theEvent);
		}
		public override void MouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView (theEvent.LocationInWindow, null);
			this.Host.FireMouseDown (Host, new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			base.MouseDown (theEvent);
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView (theEvent.LocationInWindow, null);
			this.Host.FireMouseMove (Host, new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			
			base.MouseDragged (theEvent);
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			PointF point = theEvent.LocationInWindow;
			this.Host.FireMouseMove (Host, new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			//base.MouseMoved (theEvent);
		}
	}

	[MonoMac.Foundation.Register("FormHelper")]
	class FormHelper : NSWindow
	{
		Form m_parent;
		internal FormHelper (Form parent, RectangleF r, NSWindowStyle ws, NSBackingStore back, bool flag) : base(r, ws, back, flag)
		{
			m_parent = parent;
		}
		
		private bool hasLoaded = false;
		public override void BecomeKeyWindow ()
		{
			base.BecomeKeyWindow ();
			if(!hasLoaded)
			{
				m_parent.CallLoad ();
				hasLoaded = true;
			}
		}
		
		
		
		public override void MouseMoved (NSEvent theEvent)
		{
			this.ContentView.MouseMoved(theEvent);
			base.MouseMoved (theEvent);
		}
		
	}

	//[MonoMac.Foundation.Register("Form")]
	public partial class Form : ContainerControl
	{
		//: NSWindow
		internal FormHelper m_helper;
		private bool maximizeBox = true;
		private bool minimizeBox = true;
		//: base(new RectangleF (50, 50, 400, 400), (NSWindowStyle)(1 | (1 << 1) | (1 << 2) | (1 << 3)), NSBackingStore.Buffered, false)
		public Form ()
		{
			m_helper = new FormHelper (this, new RectangleF (50, 50, 400, 400), (NSWindowStyle)(1 | (1 << 1) | (1 << 2) | (1 << 3)), NSBackingStore.Buffered, false);
			m_helper.ContentView = new View (this);
			setStyle ();
			m_helper.AcceptsMouseMovedEvents = true;
			//this.StandardWindowButton().Image
		}
		
		internal override NSView c_helper {
			get {
				return m_helper.ContentView;
			}
			set {
				base.c_helper = value;
			}
		}
		
		#region Private variables
		
		private bool		        autoscale;
		#endregion
		
		#region Public Instance Variables
		
		[MWFCategory("Layout")]
		public bool AutoScale {
			get {
				return autoscale;
			}

			set {
				if (value)
					AutoScaleMode = AutoScaleMode.None;
				autoscale = value;
			}
		}
		#endregion
		
		
		public bool MaximizeBox {
			get { return maximizeBox; }
			set {
				maximizeBox = value;
				setStyle ();
			}
		}
		public bool MinimizeBox {
			get { return minimizeBox; }
			set {
				minimizeBox = value;
				setStyle ();
			}
		}
		private void setStyle ()
		{
			// 1 = NSWindowStyle.Titled
			// 1 << 1 = NSWindowStyle.Closable;
			// 4 = NSWindowStyle.Miniaturizable;
			// 8 = NSWindowStyle.Resizable;
			m_helper.StyleMask = (NSWindowStyle)(1 | (1 << 1) | (minimizeBox ? 4 : 1) | (maximizeBox ? 8 : 1));
		}
		public void Show ()
		{
			m_helper.MakeKeyAndOrderFront (m_helper);
			//Controls.SetTab ();
		}
		public void Close ()
		{
			if (NSApplication.SharedApplication.ModalWindow == m_helper)
				NSApplication.SharedApplication.StopModal ();
			m_helper.PerformClose (m_helper);
		}

		internal NSView ContentView {
			get { return m_helper.ContentView; }
		}

		public Rectangle ClientRectangle {
			get { return Rectangle.Round (m_helper.ContentView.Frame); }
		}
		public DialogResult ShowDialog (IWin32Window parent)
		{
			return ShowDialog ();
		}
		public DialogResult ShowDialog ()
		{
			
			//this.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.BeginSheet (m_helper, NSApplication.SharedApplication.MainWindow);
			NSApplication.SharedApplication.RunModalForWindow (m_helper);
			NSApplication.SharedApplication.EndSheet (m_helper);
			m_helper.OrderOut (m_helper);
			//NSApplication.SharedApplication.BeginSheet(this,NSApplication.SharedApplication.MainWindow);
			return DialogResult.OK;
		}
		/// <summary>
		/// Drawing stuff
		/// </summary>
		/*
		public void onPaint (PaintEventArgs e)
		{
			OnPaint (e);
		}
		protected virtual void OnPaint (PaintEventArgs e)
		{
			if (Paint != null)
				Paint (m_helper, e);
			(m_helper.ContentView as View).shouldDraw = true;
			
		}
		*/
		public Graphics CreateGraphics ()
		{
			var graphics = Graphics.FromHwnd (m_helper.ContentView.Handle);
			
			//graphics.TranslateTransform(Frame.Width / 2,Frame.Height / 2 );
			//graphics.Transform = new System.Drawing.Drawing2D.Matrix(1,0,0,-1,0,0);
			return graphics;
		}

		[Obsolete("Not Implemented.", false)]
		public AutoScaleMode AutoScaleMode { get; set; }

		[Obsolete("Not Implemented.", false)]
		public SizeF AutoScaleDimensions { get; set; }

		[Obsolete("Not Implemented.", false)]
		public Icon Icon { get; set; }

		[Obsolete("Not Implemented.", false)]
		public Button AcceptButton { get; set; }

		[Obsolete("Not Implemented.", false)]
		public Button CancelButton { get; set; }

		public bool Visible {
			get { return m_helper.IsVisible; }
			set { m_helper.IsVisible = value; }
		}
		public bool Modal {
			get { return m_helper.IsSheet; }
		}
		
		/*
		public PaintEventHandler Paint { get; set; }

		public Color BackColor { get; set; }
		public void onPaintBackground (PaintEventArgs e)
		{
			this.OnPaintBackground (e);
		}
		protected virtual void OnPaintBackground (PaintEventArgs e)
		{
			if (BackColor == Color.Empty)
				BackColor = Color.Transparent;
			if (BackColor == Color.Transparent)
				return;
			Pen pen = new Pen (BackColor);
			e.Graphics.DrawRectangle (pen, e.ClipRectangle);
		}

		 */
		/*
		public SizeF ClientSize {
			get { return this.Frame.Size; }
			set { this.SetFrame (new RectangleF (this.Frame.Location, value), true, true); }
		}
		*/

		internal override ControlCollection controls {
			get {
				if (child_controls == null)
					child_controls = new ControlCollection (this);
				return child_controls;
			}
		}
		/*
		public string Name {
			get { return this.FrameAutosaveName; }
			set { this.FrameAutosaveName = value; }
		}
		*/
		public string Text {
			get { return m_helper.Title; }
			set { m_helper.Title = value; }
		}
		public object components { get; set; }
		public DialogResult DialogResult { get; set; }
		public void SuspendLayout ()
		{
			
		}
		internal override void performLayout ()
		{
			m_helper.ContentView.DisplayIfNeeded ();
		}
		public void PerformLayout ()
		{
			m_helper.ContentView.SetNeedsDisplayInRect (m_helper.ContentView.Frame);
		}
		internal void CallLoad ()
		{
			if (Load != null)
				Load (this, new EventArgs ());
		}
		public EventHandler Load { get; set; }

		#region From Template
		public string Name { get; set; }
		internal override Size size {
			get { return Size.Round(m_helper.Frame.Size); }
			set { m_helper.SetFrame (new RectangleF (m_helper.Frame.Location, value), true); }
		}

		internal override Point location {
			get { return Point.Round(m_helper.Frame.Location); }
			set { m_helper.SetFrame (new RectangleF (value, m_helper.Frame.Size), true); }
		}
		
		public BorderStyle BorderStyle { get; set; }

		/*
		public bool Focused
		{
			get {
				if(this.CurrentEditor == null)
					return false;
				return this.Window.FirstResponder == this.CurrentEditor;}
		}
		*/

		public virtual void Refresh ()
		{
			this.PerformLayout ();
			
		}
		/*
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
				return new System.Drawing.Font(base.Font.FontName, base.Font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}
			set {
				base.Font = MonoMac.AppKit.NSFont.FromFontName(value.Name,value.Size);
				
			}
		}
		*/




		public event EventHandler AutoSizeChanged;
		public event EventHandler AutoValidateChanged;
		public event EventHandler SizeChanged;
		
		
		
		#endregion
		
		
	}
}

