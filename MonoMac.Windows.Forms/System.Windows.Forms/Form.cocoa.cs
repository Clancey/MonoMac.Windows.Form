using System;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using MonoMac.AppKit;
using System.Linq;
using MonoMac.Foundation;
namespace System.Windows.Forms
{

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
		
		public Rectangle GetClientRectangle()
		{
			return Rectangle.Round(GetClientRectangleF());
		}
		public RectangleF GetClientRectangleF()
		{
			return ContentView.Bounds;
		}
		public override void KeyDown (NSEvent theEvent)
		{
			base.KeyDown (theEvent);
		}
		public override void KeyUp (NSEvent theEvent)
		{
			base.KeyUp (theEvent);
		}

		public void SetWindowStyle( bool WS_CAPTION, bool WS_MAXIMIZEBOX, bool WS_MINIMIZEBOX )
		{
			NSWindowStyle mask = NSWindowStyle.Closable;
			if( WS_CAPTION )
				mask |= NSWindowStyle.Titled;
			if( WS_MAXIMIZEBOX )
				mask |= NSWindowStyle.Resizable;
			if( WS_MINIMIZEBOX )
				mask |= NSWindowStyle.Miniaturizable;
			this.StyleMask = mask;
		}
		public override void Close ()
		{
			if (NSApplication.SharedApplication.ModalWindow == this)
				m_parent.Close();
			else
				base.Close ();
		}
		public override void PerformClose (NSObject sender)
		{
			base.PerformClose (sender);
		}
	}

	public partial class Form : ContainerControl
	{
		internal FormHelper m_helper;

		//: base(new RectangleF (50, 50, 400, 400), (NSWindowStyle)(1 | (1 << 1) | (1 << 2) | (1 << 3)), NSBackingStore.Buffered, false)
		public Form ()
		{
			m_helper.SetWindowStyle(true, true, true);
		}

		protected override void CreateHandle ()
		{
			m_helper = new FormHelper (this, new RectangleF (50, 50, 400, 400), (NSWindowStyle)(1 | (1 << 1) | (1 << 2) | (1 << 3)), NSBackingStore.Buffered, false);
			m_helper.ContentView = new ViewHelper (this);
      m_view = m_helper.ContentView;
			//m_helper.ContentView.ScaleUnitSquareToSize(Util.ScaleSize);
			m_helper.AcceptsMouseMovedEvents = true;
      base.CreateHandle();
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
			get {
				return (m_helper.StyleMask & NSWindowStyle.Resizable) == NSWindowStyle.Resizable;
			}
			set {
				if(value)
					m_helper.StyleMask |= NSWindowStyle.Resizable;
				else
					m_helper.StyleMask &= ~NSWindowStyle.Resizable;
			}
		}
		public bool MinimizeBox {
			get {
				return (m_helper.StyleMask & NSWindowStyle.Miniaturizable) == NSWindowStyle.Miniaturizable;
			}
			set {
				if(value)
					m_helper.StyleMask |= NSWindowStyle.Miniaturizable;
				else
					m_helper.StyleMask &= ~NSWindowStyle.Miniaturizable;
			}
		}
		public void Show ()
		{
			m_helper.MakeKeyAndOrderFront (m_helper);
			//Controls.SetTab ();
		}
		public void Show (IWin32Window parent)
		{
			m_helper.MakeKeyAndOrderFront (m_helper);
		}
		public void Close ()
		{
			if (NSApplication.SharedApplication.ModalWindow == m_helper)
				NSApplication.SharedApplication.AbortModal ();
			else
				m_helper.PerformClose (this);
		}

		internal NSView ContentView {
			get { return m_helper.ContentView; }
		}

		public DialogResult ShowDialog (IWin32Window parent)
		{
			return ShowDialog ();
		}
		private bool is_modal;
		public DialogResult ShowDialog ()
		{
			is_modal = true;
			//this.MakeKeyAndOrderFront (this);
			//NSApplication.SharedApplication.BeginSheet (m_helper, NSApplication.SharedApplication.MainWindow);
			NSApplication.SharedApplication.RunModalForWindow (m_helper);
			//NSApplication.SharedApplication.EndSheet (m_helper);
			m_helper.OrderOut (m_helper);
			//NSApplication.SharedApplication.BeginSheet(this,NSApplication.SharedApplication.MainWindow);
			
			m_helper.PerformClose (this);
			return this.DialogResult;
		}
		
		
		internal override void UpdateWindowText()
		{
			m_helper.Title = Text; 
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
		internal override Size clientSize {
			get {
				return m_helper.GetClientRectangle().Size;
			}
			set {
				m_helper.SetContentSize(new SizeF(value));
			}
		}

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
		public object components { get; set; }		
		
		private DialogResult dialog_result;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DialogResult DialogResult {
			get {
				return dialog_result;
			}

			set {
				if (value < DialogResult.None || value > DialogResult.No)
					throw new InvalidEnumArgumentException ("value", (int) value, 
							typeof (DialogResult));

				dialog_result = value;
				if (dialog_result != DialogResult.None && is_modal)
					this.Close();
					//RaiseCloseEvents (false, false); // .Net doesn't send WM_CLOSE here.
			}
		}
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
		internal Rectangle bounds {
			get { return Rectangle.Round(m_helper.Frame); }
			set { 
				m_helper.SetFrame (value, true); }
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

