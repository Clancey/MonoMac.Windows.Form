using System;
using MonoMac.AppKit;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Drawing.Drawing2D;
namespace System.Windows.Forms
{
	public class View : NSView
	{
		public Form Parent;

		public View (Form parent)
		{
			Parent = parent;
		}
		public override bool IsFlipped {
			get { return true; }
		}


		public override void DrawRect (RectangleF dirtyRect)
		{
			using (var graphics = Graphics.FromHwnd (this.Handle))
			{
				var events = new PaintEventArgs (graphics, Rectangle.Round (dirtyRect));
				Parent.onPaintBackground (events);
				Parent.onPaint (events);
			}
		}
		public void PaintRect (RectangleF dirtyRect)
		{
			base.DrawRect (dirtyRect);
		}

		public override void MouseUp (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			
			var button = (MouseButtons)theEvent.ButtonNumber;
			this.Parent.FireMouseUp (Parent, new MouseEventArgs (button, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			base.MouseUp (theEvent);
		}
		public override void MouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			this.Parent.FireMouseDown (Parent, new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			base.MouseDown (theEvent);
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			PointF point = this.ConvertPointfromView(theEvent.LocationInWindow,null);
			this.Parent.FireMouseMove (Parent, new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			
			base.MouseDragged (theEvent);
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			PointF point = theEvent.LocationInWindow;
			this.Parent.FireMouseMove (Parent, new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			base.MouseMoved (theEvent);
		}
	}



	[MonoMac.Foundation.Register("Form")]
	public partial class Form : NSWindow
	{
		private bool maximizeBox = true;
		private bool minimizeBox = true;
		private controls theControls;
		public Form () : base(new RectangleF (50, 50, 400, 400), (NSWindowStyle)(1 | (1 << 1) | (1 << 2) | (1 << 3)), NSBackingStore.Buffered, false)
		{
			theControls = new controls (this);
			ContentView = new View (this);
			setStyle();
		}
		public bool MaximizeBox
		{
			get {return maximizeBox;}
			set {maximizeBox = value; setStyle();}
		}
		public bool MinimizeBox
		{
			get {return minimizeBox;}
			set {minimizeBox = value; setStyle();}
		}
		private void setStyle()
		{
			this.StyleMask = (NSWindowStyle)(1 |  (1 << 1)| (minimizeBox ? 4 : 1) | (maximizeBox ? 8 : 1));
		}
		public void Show ()
		{
			this.MakeKeyAndOrderFront (this);
			//Controls.SetTab ();
		}
		public void Close ()
		{
			if (NSApplication.SharedApplication.ModalWindow == this)
				NSApplication.SharedApplication.StopModal ();
			this.PerformClose (this);
		}


		public Rectangle ClientRectangle {
			get { return Rectangle.Round (this.ContentView.Frame); }
		}

		public void ShowDialog ()
		{
			
			//this.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.BeginSheet (this, NSApplication.SharedApplication.MainWindow);
			NSApplication.SharedApplication.RunModalForWindow (this);
			NSApplication.SharedApplication.EndSheet (this);
			this.OrderOut (this);
			//NSApplication.SharedApplication.BeginSheet(this,NSApplication.SharedApplication.MainWindow);
		}
		/// <summary>
		/// Drawing stuff
		/// </summary>

		public void DrawRect (RectangleF dirtyRect)
		{
			
		}
		public void onPaint (PaintEventArgs e)
		{
			if(ContentView.IsFlipped)
			{
				e.Graphics.TranslateTransform(e.ClipRectangle.X , e.ClipRectangle.Height + e.ClipRectangle.Y);
				e.Graphics.ScaleTransform(1.0f,-1.0f);
			}
			OnPaint (e);
		}
		protected virtual void OnPaint (PaintEventArgs e)
		{
			if (Paint != null)
				Paint (this, e);
			(this.ContentView as View).PaintRect (e.ClipRectangle);
			
		}
		public Graphics CreateGraphics ()
		{
		 	var graphics = Graphics.FromHwnd (ContentView.Handle);
			
			//graphics.TranslateTransform(Frame.Width / 2,Frame.Height / 2 );
			//graphics.Transform = new System.Drawing.Drawing2D.Matrix(1,0,0,-1,0,0);
			return graphics;
		}

		public PaintEventHandler Paint { get; set; }

		public Color BackColor { get; set; }
		public void onPaintBackground (PaintEventArgs e)
		{
			this.OnPaintBackground (e);
		}
		protected virtual void OnPaintBackground (PaintEventArgs e)
		{
			if (BackColor == null)
				BackColor = Color.Transparent;
			if (BackColor == Color.Transparent)
				return;
			Pen pen = new Pen (BackColor);
			e.Graphics.DrawRectangle (pen, e.ClipRectangle);
		}


		/*
		public SizeF ClientSize {
			get { return this.Frame.Size; }
			set { this.SetFrame (new RectangleF (this.Frame.Location, value), true, true); }
		}
		*/

		public controls Controls {
			get {
				if (theControls == null)
					theControls = new controls (this);
				return theControls;
			}
		}
		/*
		public string Name {
			get { return this.FrameAutosaveName; }
			set { this.FrameAutosaveName = value; }
		}
		*/
		public string Text {
			get { return this.Title; }
			set { this.Title = value; }
		}
		public object components { get; set; }
		public void SuspendLayout ()
		{
			
		}
		public void ResumeLayout (bool action)
		{
			this.ContentView.DisplayIfNeeded ();
		}
		public void PerformLayout ()
		{
			this.ContentView.SetNeedsDisplayInRect (this.ContentView.Frame);
		}
		public override void BecomeKeyWindow ()
		{
			base.BecomeKeyWindow ();
			if (Load != null)
				Load (this, new EventArgs ());
		}
		public EventHandler Load { get; set; }

		#region From Template
		public string Name { get; set; }
		public SizeF Size {
			get { return this.Frame.Size; }
			set { this.SetFrame (new RectangleF (this.Frame.Location, value), true); }
		}

		public PointF Location {
			get { return this.Frame.Location; }
			set { this.SetFrame (new RectangleF (value, this.Frame.Size), true); }
		}
		/*
		public Rectangle ClientRectangle
		{
			get{ return Rectangle.Round(this.ContentView.Bounds);}
			set{ this.Bounds = value;}
		}
		*/
		public SizeF ClientSize {
			get { return this.Frame.Size; }
			set { this.SetFrame (new RectangleF (this.Frame.Location, value), true); }
		}

		public float Width {
			get { return this.Size.Width; }
			set { this.Size = new SizeF (value, this.Size.Height); }
		}

		public float Height {
			get { return this.Size.Height; }
			set { this.Size = new SizeF (this.Size.Width, value); }
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
		public void FireMouseDown(object sender,MouseEventArgs e)
		{
			if(MouseDown !=null)
				MouseDown(sender,e);
		}
		public event MouseEventHandler MouseDown;
		public void FireMouseUp(object sender, MouseEventArgs e)
		{
			if(MouseUp != null)
				MouseUp(sender,e);
		}
		public event MouseEventHandler MouseUp;
		public event EventHandler GotFocus;
		public void FireMouseMove(object sender,MouseEventArgs e)
		{
			if(MouseMove != null)
				MouseMove(sender,e);
		}
		public event MouseEventHandler MouseMove;
		public event MouseEventHandler MouseDoubleClick;
		public event EventHandler SizeChanged;



		#endregion

		public class controls
		{
			private Form theForm;
			public controls (Form form)
			{
				theForm = form;
			}


			public void Add (NSView item)
			{
				theForm.ContentView.AddSubview (item);
				SetTab ();
			}

			public void Clear ()
			{
				foreach (var view in theForm.ContentView.Subviews)
				{
					view.RemoveFromSuperview ();
				}
			}

			public bool Contains (NSView item)
			{
				return theForm.ContentView.Subviews.Contains (item);
			}

			public void CopyTo (NSView[] array, int arrayIndex)
			{
				theForm.ContentView.Subviews.CopyTo (array, arrayIndex);
			}

			public bool Remove (NSView item)
			{
				item.RemoveFromSuperview ();
				SetTab ();
				return true;
				
			}

			public int Count {
				get { return theForm.ContentView.Subviews.Count (); }
			}

			public bool IsReadOnly {
				get { return false; }
			}

			public int IndexOf (NSView item)
			{
				return theForm.ContentView.Subviews.ToList ().IndexOf (item);
				
			}

			public void Insert (int index, NSView item)
			{
				if (index == 0)
				{
					theForm.ContentView.AddSubview (item, NSWindowOrderingMode.Below, null);
					return;
				}
				
				var rowBelow = theForm.ContentView.Subviews[index - 1];
				theForm.ContentView.AddSubview (item, NSWindowOrderingMode.Below, rowBelow);
				SetTab ();
			}

			public void RemoveAt (int index)
			{
				theForm.ContentView.Subviews[index].RemoveFromSuperview ();
				SetTab ();
			}

			public NSView this[int index] {
				get { return theForm.ContentView.Subviews[index]; }

				set { theForm.ContentView.Subviews[index] = value; }
			}
			//TODO: Make it work, It doesn't work as is
			public void SetTab ()
			{
			}
		}
			/*
				var controls = theForm.ContentView.Subviews.OrderBy (x => x.Tag).ToList ();
				for (int i = 0; i < controls.Count - 1; i++)
				{
					var firstControl = controls[i];
					var nextControl = controls[i + 1];
					firstControl.NextResponder = nextControl;
				}
				*/			
			
			}
}

