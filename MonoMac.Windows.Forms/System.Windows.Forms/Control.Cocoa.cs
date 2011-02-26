using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using MonoMac.AppKit;
using System.Linq;

namespace System.Windows.Forms
{
	public partial class Control
	{
		internal NSView m_view;
		internal NSView NSViewForControl {
			get { return m_view; }
		}


		#region constructors
		public Control ()
		{
			// We eventually want to create the handle when the form is first shown
			// or when a control is added to a form that already exists. For now, this
			// is ok
			CreateHandle ();
			//c_helper.ScaleUnitSquareToSize(Util.ScaleSize);
			//(c_helper as IViewHelper).Host = this;
			MaximumSize = DefaultMaximumSize;
			MinimumSize = DefaultMinimumSize;
			
			
			layout_type = LayoutType.Anchor;
			anchor_style = AnchorStyles.Top | AnchorStyles.Left;
			
			is_created = false;
			is_visible = true;
			is_captured = false;
			is_disposed = false;
			is_enabled = true;
			is_entered = false;
			layout_pending = false;
			is_toplevel = false;
			causes_validation = true;
			layout_suspended = 0;
			mouse_clicks = 1;
			tab_index = -1;
			right_to_left = RightToLeft.Inherit;
			border_style = BorderStyle.None;
			background_color = Color.Empty;
			dist_right = 0;
			dist_bottom = 0;
			tab_stop = true;
			ime_mode = ImeMode.Inherit;
			use_compatible_text_rendering = true;
			show_keyboard_cues = false;
			use_wait_cursor = false;
			
			backgroundimage_layout = ImageLayout.Tile;
			padding = this.DefaultPadding;
			maximum_size = new Size ();
			minimum_size = new Size ();
			margin = this.DefaultMargin;
			auto_size_mode = AutoSizeMode.GrowOnly;
			
			control_style = ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Selectable | ControlStyles.StandardClick | ControlStyles.StandardDoubleClick;
			control_style |= ControlStyles.UseTextForAccessibility;
			
			parent = null;
			background_image = null;
			text = string.Empty;
			name = string.Empty;
			
			child_controls = CreateControlsInstance ();
			
			bounds = new Rectangle (Point.Empty, DefaultSize);
			explicit_bounds = bounds;
		}

		#region Public Static Properties

		public static Color DefaultBackColor {
			get { return Color.Empty; }
		}

		public static Color DefaultForeColor {
			get { return Color.Empty; }
		}

		public static Font DefaultFont {
			get { return new Font ("Arial", 10f, GraphicsUnit.Pixel); }
		}
		#endregion


		protected virtual void CreateHandle ()
		{
			if (m_view == null)
				m_view = new ViewHelper (this);
		}

		private void initialize ()
		{
			
		}

		public static implicit operator NSView (Control control)
		{
			return control.NSViewForControl;
		}

		#endregion

		#region internal Variables

		Control parent;
		BindingContext binding_context;
		Image background_image;
		// background image for control
		ImageLayout backgroundimage_layout;
		internal object creator_thread;
		// thread that created the control
		ControlBindingsCollection data_bindings;
		internal Color foreground_color;
		// foreground color for control
		RightToLeft right_to_left;
		// drawing direction for control		
		internal int layout_suspended;
		bool causes_validation;
		// tracks if validation is executed on changes
		bool tab_stop;
		// is the control a tab stop?
		bool layout_pending;
		// true if our parent needs to re-layout us
		internal bool is_toplevel;
		// tracks if the control is a toplevel window
		ControlStyles control_style;
		// rather win32-specific, style bits for control
		internal bool is_entered;
		// is the mouse inside the control?
		bool is_captured;
		// tracks if the control has captured the mouse		
		internal virtual bool is_enabled { get; set; }


		private AutoSizeMode auto_size_mode;

		Padding padding;
		#endregion

		#region Public Instance Methods
		public void BringToFront ()
		{
			NSViewForControl.BringToFront ();
		}


		public void CreateControl ()
		{
			if (is_created)
			{
				return;
			}
			
			if (is_disposing)
			{
				return;
			}
			
			if (!is_visible)
			{
				return;
			}
			
			if (parent != null && !parent.Created)
			{
				return;
			}
			
			
			if (!is_created)
			{
				is_created = true;
				
				// Create all of our children (implicit ones as well) when we are created.
				// The child should fire it's OnLoad before the parents, however
				// if the child checks Parent.Created in it's OnCreateControl, the
				// parent is already created.
				foreach (Control c in Controls.GetAllControls ())
					if (!c.Created && !c.IsDisposed)
						c.CreateControl ();
				
				OnCreateControl ();
			}
		}

		public Graphics CreateGraphics ()
		{
			var graphics = Graphics.FromHwnd (NSViewForControl.Handle);
			
			//graphics.TranslateTransform(Frame.Width / 2,Frame.Height / 2 );
			//graphics.Transform = new System.Drawing.Drawing2D.Matrix(1,0,0,-1,0,0);
			return graphics;
		}


		public DragDropEffects DoDragDrop (object data, DragDropEffects allowedEffects)
		{
			DragDropEffects result = DragDropEffects.None;
			//TODO: fixme
			//if (IsHandleCreated)
			//	result = XplatUI.StartDrag(Handle, data, allowedEffects);
			OnDragDropEnd (result);
			return result;
		}

		internal Control GetRealChildAtPoint (Point pt)
		{
			foreach (Control control in child_controls.GetAllControls ())
			{
				if (control.Bounds.Contains (PointToClient (pt)))
				{
					Control child = control.GetRealChildAtPoint (pt);
					if (child == null)
						return control;
					else
						return child;
				}
			}
			
			return null;
		}

		public Control GetChildAtPoint (Point pt)
		{
			return GetChildAtPoint (pt, GetChildAtPointSkip.None);
		}

		public Control GetChildAtPoint (Point pt, GetChildAtPointSkip skipValue)
		{
			
			// Microsoft's version of this function doesn't seem to work, so I can't check
			// if we only consider children or also grandchildren, etc.
			// I'm gonna say 'children only'
			foreach (Control child in Controls)
			{
				if ((skipValue & GetChildAtPointSkip.Disabled) == GetChildAtPointSkip.Disabled && !child.Enabled)
					continue;
				else if ((skipValue & GetChildAtPointSkip.Invisible) == GetChildAtPointSkip.Invisible && !child.Visible)
					continue;
				else if ((skipValue & GetChildAtPointSkip.Transparent) == GetChildAtPointSkip.Transparent && child.BackColor.A == 0x0)
					continue;
				else if (child.Bounds.Contains (pt))
					return child;
			}
			
			return null;
		}

		public object Invoke (Delegate method, params object[] args)
		{
			Control control = FindControlToInvokeOn ();
			
			if (!this.InvokeRequired)
			{
				return method.DynamicInvoke (args);
			}
			//TODO: fixme
						/*
			IAsyncResult result = BeginInvokeInternal (method, args, control);
			return EndInvoke(result);
			*/
return null;
		}


		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public object EndInvoke (IAsyncResult asyncResult)
		{
			//TODO: Fixme
						/*
			AsyncMethodResult result = (AsyncMethodResult) asyncResult;
			return result.EndInvoke ();
			*/
return null;
		}

		public Point PointToClient (Point p)
		{
			int x = p.X;
			int y = p.Y;
			return Point.Round (NSViewForControl.ConvertPointToBase (p));
		}

		public Point PointToScreen (Point p)
		{
			int x = p.X;
			int y = p.Y;
			//TODO: FIXME
			//XplatUI.ClientToScreen(Handle, ref x, ref y);
			
			return new Point (x, y);
		}

		internal virtual void UpdateBounds ()
		{
			NSViewForControl.Frame = bounds;
		}
		internal virtual void SetBoundsInternal (int x, int y, int width, int height, BoundsSpecified specified)
		{
			bounds = new Rectangle (x, y, width, height);
			UpdateBounds ();
			// If the user explicitly moved or resized us, recalculate our anchor distances
			//if (specified != BoundsSpecified.None)
			//	UpdateDistances ();
			
			if (parent != null)
				parent.PerformLayout (this, "Bounds");
		}

		internal virtual void SetBoundsCoreInternal (int x, int y, int width, int height, BoundsSpecified specified)
		{
			SetBoundsInternal (x, y, width, height, specified);
		}

		internal virtual void performLayout ()
		{
			NSViewForControl.DisplayIfNeeded ();
		}


		public void Update ()
		{
			NSViewForControl.SetNeedsDisplayInRect (NSViewForControl.Frame);
		}

		#endregion

		#region Protected Instance Methods
		protected virtual bool ProcessCmdKey (ref Message msg, Keys keyData)
		{
			//if ((context_menu != null) && context_menu.ProcessCmdKey(ref msg, keyData)) {
			//	return true;
			//}
			
			if (parent != null)
			{
				return parent.ProcessCmdKey (ref msg, keyData);
			}
			
			return false;
		}
		
		
		internal void onKeyPress(KeyPressEventArgs e) {
			OnKeyPress(e);	
		}
		internal void onKeyDown(KeyEventArgs e) {
			OnKeyDown(e);	
		}
		internal void onKeyUp(KeyEventArgs e) {
			OnKeyUp(e);	
		}
		protected virtual bool ProcessKeyEventArgs (ref Message m)
		{
			/*
			KeyEventArgs key_event;

			switch (m.Msg) {
				case (int)Msg.WM_SYSKEYDOWN:
				case (int)Msg.WM_KEYDOWN: {
					key_event = new KeyEventArgs ((Keys) m.WParam.ToInt32 ());
					OnKeyDown (key_event);
					suppressing_key_press = key_event.SuppressKeyPress;
					return key_event.Handled;
				}

				case (int)Msg.WM_SYSKEYUP:
				case (int)Msg.WM_KEYUP: {
					key_event = new KeyEventArgs ((Keys) m.WParam.ToInt32 ());
					OnKeyUp (key_event);
					return key_event.Handled;
				}

				case (int)Msg.WM_SYSCHAR:
				case (int)Msg.WM_CHAR: {
					if (suppressing_key_press)
						return true;
					KeyPressEventArgs key_press_event;

					key_press_event = new KeyPressEventArgs ((char) m.WParam);
					OnKeyPress(key_press_event);
					m.WParam = (IntPtr) key_press_event.KeyChar;
					return key_press_event.Handled;
				}

				default: {
					break;
				}
			}
			 */			
			return false;
		}
		#endregion

		#region Private and Internal Methods
		private void ChangeParent (Control new_parent)
		{
			bool pre_enabled;
			bool pre_visible;
			Font pre_font;
			Color pre_fore_color;
			Color pre_back_color;
			RightToLeft pre_rtl;
			
			// These properties are inherited from our parent
			// Get them pre parent-change and then send events
			// if they are changed after we have our new parent
			pre_enabled = Enabled;
			pre_visible = Visible;
			pre_font = Font;
			pre_fore_color = ForeColor;
			pre_back_color = BackColor;
			pre_rtl = RightToLeft;
			// MS doesn't seem to send a CursorChangedEvent
			
			parent = new_parent;
			
			Form frm = this as Form;
			if (frm == null && IsHandleCreated)
			{
				IntPtr parent_handle = IntPtr.Zero;
				if (new_parent != null && new_parent.IsHandleCreated)
					new_parent.NSViewForControl.AddSubview (this);
				else
					this.NSViewForControl.RemoveFromSuperview ();
			}
			
			OnParentChanged (EventArgs.Empty);
			
			if (pre_enabled != Enabled)
			{
				OnEnabledChanged (EventArgs.Empty);
			}
			
			if (pre_visible != Visible)
			{
				OnVisibleChanged (EventArgs.Empty);
			}
			
			if (pre_font != Font)
			{
				OnFontChanged (EventArgs.Empty);
			}
			
			if (pre_fore_color != ForeColor)
			{
				OnForeColorChanged (EventArgs.Empty);
			}
			
			if (pre_back_color != BackColor)
			{
				OnBackColorChanged (EventArgs.Empty);
			}
			
			if (pre_rtl != RightToLeft)
			{
				// MS sneaks a OnCreateControl and OnHandleCreated in here, I guess
				// because when RTL changes they have to recreate the win32 control
				// We don't really need that (until someone runs into compatibility issues)
				OnRightToLeftChanged (EventArgs.Empty);
			}
			
			
			if ((binding_context == null))
			{
				OnBindingContextChanged (EventArgs.Empty);
			}
		}

		internal void Draw (PaintEventArgs events)
		{
			OnPaintBackground (events);
			OnPaint (events);
		}

		internal void FireMouseDown (object sender, MouseEventArgs e)
		{
			OnMouseDown (e);
		}

		internal void FireMouseUp (object sender, MouseEventArgs e)
		{
			OnMouseUp (e);
		}

		internal void FireMouseMove (object sender, MouseEventArgs e)
		{
			OnMouseMove (e);
		}



		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual Rectangle GetScaledBounds (Rectangle bounds, SizeF factor, BoundsSpecified specified)
		{
			return GetScaledBoundsOld (bounds, factor, specified);
		}


		protected virtual bool IsInputChar (char charCode)
		{
			return IsInputCharInternal (charCode);
		}

		internal bool Select (Control control)
		{
			IContainerControl container;
			
			if (control == null)
			{
				return false;
			}
			
			container = GetContainerControl ();
			if (container != null && (Control)container != control)
			{
				container.ActiveControl = control;
				if (container.ActiveControl == control && !control.Focused && control.IsHandleCreated)
					control.NSViewForControl.BecomeFirstResponder ();
			}

			else if (control.IsHandleCreated)
			{
				control.NSViewForControl.BecomeFirstResponder ();
			}
			return true;
		}


		private void UpdateCursor ()
		{
		}
			/*
			if (!IsHandleCreated)
				return;
			
			if (!Enabled) {
				XplatUI.SetCursor (window.Handle, GetAvailableCursor ().handle);
				return;
			}

			Point pt = PointToClient (Cursor.Position);

			if (!bounds.Contains (pt) && !Capture)
				return;

			if (cursor != null || use_wait_cursor) {
				XplatUI.SetCursor (window.Handle, Cursor.handle);
			} else {
				XplatUI.SetCursor (window.Handle, GetAvailableCursor ().handle);
			}
			*/			
		#endregion
		
		#region Internal Properties
		
		
		// Looks for focus in child controls
		// and also in the implicit ones
				internal bool InternalContainsFocus {
			get {
				IntPtr focused_window;
				
				focused_window = NSViewForControl.Window.FirstResponder.Handle;
				if (IsHandleCreated)
				{
					if (focused_window == Handle)
						return true;
					
					foreach (Control child_control in controls.GetAllControls ())
						if (child_control.InternalContainsFocus)
							return true;
				}
				
				return false;
			}
		}

		internal BorderStyle InternalBorderStyle {
			get { return border_style; }

			set {
				if (!Enum.IsDefined (typeof(BorderStyle), value))
					throw new InvalidEnumArgumentException (string.Format ("Enum argument value '{0}' is not valid for BorderStyle", value));
				
				if (border_style != value)
				{
					border_style = value;
					
					//if (IsHandleCreated) {
					//RecreateHandle ();
					//	Refresh ();
					//} else
					//	client_size = ClientSizeFromSize (bounds.Size);
				}
			}
		}

		internal bool VisibleInternal {
			get { return is_visible; }
		}


		#endregion

		#region Public Instance Properties

		internal Rectangle bounds;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle Bounds {
			get { return this.bounds; }

			set { SetBounds (value.Left, value.Top, value.Width, value.Height, BoundsSpecified.All); }
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Capture {
			get { return this.is_captured; }

			set {
				// Call OnMouseCaptureChanged when we get WM_CAPTURECHANGED.
				if (value != is_captured)
				{
					if (value)
					{
						is_captured = true;
						NSViewForControl.BecomeFirstResponder ();
						//XplatUI.GrabWindow(Handle, IntPtr.Zero);
					}

					else
					{
						if (IsHandleCreated)
							NSViewForControl.ResignFirstResponder ();
						is_captured = false;
					}
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle ClientRectangle {
// client rectangle is in client coordinate system
			get { return Rectangle.Round (NSViewForControl.Bounds); }
		}

		internal virtual Size clientSize {
			get { return new Size ((int)Size.Width, (int)Size.Height); }
			set { Size = value; }
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size ClientSize {
			get { return clientSize; }
			set {
				clientSize = value;
				this.OnClientSizeChanged (EventArgs.Empty);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool ContainsFocus {
			get { return NSViewForControl == NSViewForControl.Window.FirstResponder || NSViewForControl.Subviews.Where (x => x == NSViewForControl.Window.FirstResponder).Count () > 0; }
		}


		internal ControlCollection child_controls;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		internal virtual ControlCollection controls {
			get {
				if (child_controls == null)
					child_controls = new ControlCollection (this);
				return child_controls;
			}
		}
		public virtual ControlCollection Controls {
			get { return controls; }
		}


		internal bool has_focus {
			get {
				if (NSViewForControl == null)
					return false;
				if (NSViewForControl.Window == null)
					return false;
				return NSViewForControl == NSViewForControl.Window.FirstResponder;
			}
		}



		[DispId(-514)]
		[Localizable(true)]
		[MWFCategory("Behavior")]
		public bool Enabled {
			get {
				if (!is_enabled)
				{
					return false;
				}
				
				if (parent != null)
				{
					return parent.Enabled;
				}
				
				return true;
			}

			set {
				if (this.is_enabled == value)
					return;
				
				bool old_value = is_enabled;
				
				is_enabled = value;
				enabled = value;
				if (!value)
					UpdateCursor ();
				
				if (old_value != value && !value && this.has_focus)
					SelectNextControl (this, true, true, true, true);
				
				OnEnabledChanged (EventArgs.Empty);
			}
		}

		internal virtual bool enabled {
			get { return is_enabled; }
			set { is_enabled = value; }
		}


		internal virtual NSFont nsFont { get; set; }
		internal Font font {
			get {
				if (nsFont == null)
					return new System.Drawing.Font ("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				return new System.Drawing.Font (nsFont.FontName, nsFont.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}

			set { nsFont = MonoMac.AppKit.NSFont.FromFontName (value.Name, value.Size); }
		}

		[DispId(-515)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IntPtr Handle {
			get { return NSViewForControl.Handle; }
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsHandleCreated {
			get {
				
				if (NSViewForControl == null)
					return false;
				//else if(c_helper.Window.Handle == IntPtr.Zero)
				//	return false;
				
				return true;
			}
		}

		internal virtual Point location {
			get { return Point.Round (NSViewForControl.Frame.Location); }
			set { NSViewForControl.Frame = new RectangleF (value, NSViewForControl.Frame.Size); }
		}

		internal bool is_visible { get; set; }

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public override ISite Site {
			get { return base.Site; }

			set {
				base.Site = value;
				
				if (value != null)
				{
					AmbientProperties ap = (AmbientProperties)value.GetService (typeof(AmbientProperties));
					if (ap != null)
					{
						BackColor = ap.BackColor;
						ForeColor = ap.ForeColor;
						//Cursor = ap.Cursor;
						Font = ap.Font;
					}
				}
			}
		}


		[DispId(-517)]
		[Localizable(true)]
		[BindableAttribute(true)]
		[MWFCategory("Appearance")]
		public virtual string Text {
// Our implementation ignores ControlStyles.CacheText - we always cache
			get { return this.text; }

			set {
				if (value == null)
				{
					value = String.Empty;
				}
				
				if (text != value)
				{
					text = value;
					UpdateWindowText ();
					OnTextChanged (EventArgs.Empty);
					
					// Label has its own AutoSize implementation
					if (AutoSize && Parent != null && (!(this is Label)))
						Parent.PerformLayout (this, "Text");
				}
			}
		}

		internal virtual void UpdateWindowText()
		{
				
		}
		[Localizable(true)]
		[MWFCategory("Behavior")]
		public bool Visible {
			get {
				if (!is_visible)
				{
					return false;
				}

				else if (parent != null)
				{
					return parent.Visible;
				}
				
				return true;
			}

			set {
				if (this.is_visible != value)
				{
					//TODO: Set Visible
					//SetVisibleCore(value);
					
					if (parent != null)
						parent.PerformLayout (this, "Visible");
				}
			}
		}
		#endregion

		#region Protected Instance Properties

		protected virtual CreateParams CreateParams {
			get {
				CreateParams create_params = new CreateParams ();
				
				try
				{
					create_params.Caption = Text;
				}
				catch
				{
					create_params.Caption = text;
				}
				
				try
				{
					create_params.X = Left;
				}
				catch
				{
					create_params.X = this.bounds.X;
				}
				
				try
				{
					create_params.Y = Top;
				}
				catch
				{
					create_params.Y = this.bounds.Y;
				}
				
				try
				{
					create_params.Width = Width;
				}
				catch
				{
					create_params.Width = this.bounds.Width;
				}
				
				try
				{
					create_params.Height = Height;
				}
				catch
				{
					create_params.Height = this.bounds.Height;
				}
				
				
				create_params.ClassName = GetType ().Name;
				create_params.ExStyle = 0;
				create_params.Param = 0;
				/*
				if (allow_drop) {
					create_params.ExStyle |= (int)WindowExStyles.WS_EX_ACCEPTFILES;
				}

				if ((parent!=null) && (parent.IsHandleCreated)) {
					create_params.Parent = parent.Handle;
				}

				create_params.Style = (int)WindowStyles.WS_CHILD | (int)WindowStyles.WS_CLIPCHILDREN | (int)WindowStyles.WS_CLIPSIBLINGS;

				if (is_visible) {
					create_params.Style |= (int)WindowStyles.WS_VISIBLE;
				}

				if (!is_enabled) {
					create_params.Style |= (int)WindowStyles.WS_DISABLED;
				}

				switch (border_style) {
					case BorderStyle.FixedSingle:
						create_params.Style |= (int) WindowStyles.WS_BORDER;
						break;
					case BorderStyle.Fixed3D:
						create_params.ExStyle |= (int) WindowExStyles.WS_EX_CLIENTEDGE;
						break;
				}
				*/				
				create_params.control = this;
				
				return create_params;
			}
		}

		#endregion

		#region Public Static Methods

		[MonoTODO("Only implemented for Win32, others always return false")]
		public static bool IsKeyLocked (Keys keyVal)
		{
			return false;
		}
		#endregion

		#region misc

		public void Invalidate (Rectangle rc, bool invalidateChildren)
		{
			// Win32 invalidates control including when Width and Height is equal 0
			// or is not visible, only Paint event must be care about this.
			if (!IsHandleCreated)
				return;
			
			if (rc.IsEmpty)
				rc = ClientRectangle;
			
			if (rc.Width > 0 && rc.Height > 0)
			{
				
				NotifyInvalidate (rc);
				
				NSViewForControl.SetNeedsDisplayInRect (rc);
				
				if (invalidateChildren)
				{
					Control[] controls = child_controls.GetAllControls ();
					for (int i = 0; i < controls.Length; i++)
						controls[i].Invalidate ();
				}

				else
				{
					// If any of our children are transparent, we
					// have to invalidate them anyways
					foreach (Control c in Controls)
						if (c.BackColor.A != 255)
							c.Invalidate ();
				}
			}
			OnInvalidated (new InvalidateEventArgs (rc));
		}

		// This method exists so controls overriding OnPaintBackground can have default background painting done
		internal virtual void PaintControlBackground (PaintEventArgs pevent)
		{
			if (background_image == null)
			{
				Rectangle paintRect = new Rectangle (pevent.ClipRectangle.X, pevent.ClipRectangle.Y, pevent.ClipRectangle.Width, pevent.ClipRectangle.Height);
				Brush pen = new SolidBrush (BackColor);
				//ThemeEngine.Current.ResPool.GetSolidBrush(BackColor);
				pevent.Graphics.FillRectangle (pen, paintRect);
				return;
			}
			
			DrawBackgroundImage (pevent.Graphics);
		}

		void DrawBackgroundImage (Graphics g)
		{
			Rectangle drawing_rectangle = new Rectangle ();
			g.FillRectangle (new SolidBrush (BackColor), ClientRectangle);
			
			switch (backgroundimage_layout)
			{
			case ImageLayout.Tile:
				using (TextureBrush b = new TextureBrush (background_image, WrapMode.Tile))
				{
					g.FillRectangle (b, ClientRectangle);
				}

				return;
			case ImageLayout.Center:
				drawing_rectangle.Location = new Point (ClientSize.Width / 2 - background_image.Width / 2, ClientSize.Height / 2 - background_image.Height / 2);
				drawing_rectangle.Size = background_image.Size;
				break;
			case ImageLayout.None:
				drawing_rectangle.Location = Point.Empty;
				drawing_rectangle.Size = background_image.Size;
				break;
			case ImageLayout.Stretch:
				drawing_rectangle = ClientRectangle;
				break;
			case ImageLayout.Zoom:
				drawing_rectangle = ClientRectangle;
				if ((float)background_image.Width / (float)background_image.Height < (float)drawing_rectangle.Width / (float)drawing_rectangle.Height)
				{
					drawing_rectangle.Width = (int)(background_image.Width * ((float)drawing_rectangle.Height / (float)background_image.Height));
					drawing_rectangle.X = (ClientRectangle.Width - drawing_rectangle.Width) / 2;
				}

				else
				{
					drawing_rectangle.Height = (int)(background_image.Height * ((float)drawing_rectangle.Width / (float)background_image.Width));
					drawing_rectangle.Y = (ClientRectangle.Height - drawing_rectangle.Height) / 2;
				}

				break;
			default:
				return;
			}
			
			g.DrawImage (background_image, drawing_rectangle);
			
		}

		#endregion

		#region OnXXX Methods
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnEnabledChanged (EventArgs e)
		{
			if (IsHandleCreated)
			{
				Refresh ();
			}
			
			EventHandler eh = (EventHandler)(Events[EnabledChangedEvent]);
			if (eh != null)
				eh (this, e);
			
			foreach (Control c in Controls.GetAllControls ())
				c.OnParentEnabledChanged (e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnInvalidated (InvalidateEventArgs e)
		{
			if (NSViewForControl != null)
				NSViewForControl.SetNeedsDisplayInRect (e.InvalidRect);
			
			InvalidateEventHandler eh = (InvalidateEventHandler)(Events[InvalidatedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnLayout (LayoutEventArgs levent)
		{
			LayoutEventHandler eh = (LayoutEventHandler)(Events[LayoutEvent]);
			if (eh != null)
				eh (this, levent);
			
			Size s = Size;
			
			// If our layout changed our PreferredSize, our parent
			// needs to re-lay us out.  However, it's not always possible to
			// be our preferred size, so only try once so we don't loop forever.
			//if (Parent != null && AutoSize &&  PreferredSize != s) {
			//	Parent.PerformLayout ();
			//}
			
		}



		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnPaint (PaintEventArgs e)
		{
			PaintEventHandler eh = (PaintEventHandler)(Events[PaintEvent]);
			if (eh != null)
				eh (this, e);
			e.Handled = false;
		}


		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnSizeChanged (EventArgs e)
		{
			OnResize (e);
			EventHandler eh = (EventHandler)(Events[SizeChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		#endregion


		#region Public Classes


		[ComVisible(false)]
		public class ControlCollection : Layout.ArrangedElementCollection, IList, ICollection, ICloneable, IEnumerable
		{

			#region ControlCollection Local Variables
			ArrayList impl_list;
			Control[] all_controls;
			Control owner;
			#endregion

			#region ControlCollection Public Constructor
			public ControlCollection (Control owner)
			{
				this.owner = owner;
			}
			#endregion

			#region ControlCollection Public Instance Properties


			public Control Owner {
				get { return this.owner; }
			}

			public virtual Control this[string key] {
				get {
					int index = IndexOfKey (key);
					
					if (index >= 0)
						return this[index];
					
					return null;
				}
			}

			public virtual new Control this[int index] {
				get {
					if (index < 0 || index >= list.Count)
					{
						throw new ArgumentOutOfRangeException ("index", index, "ControlCollection does not have that many controls");
					}
					return (Control)list[index];
				}
			}



			#endregion

			#region ControlCollection Instance Methods

			public virtual void Add (Control value)
			{
				if (value == null)
					return;
				
				Form form_value = value as Form;
				Form form_owner = owner as Form;
				//TODO:
				bool owner_permits_toplevels = true;
				// (owner is MdiClient) || (form_owner != null && form_owner.IsMdiContainer);
				bool child_is_toplevel = value.GetTopLevel ();
				bool child_is_mdichild = false;
				//form_value != null && form_value.IsMdiChild;
				if (child_is_toplevel && !(owner_permits_toplevels && child_is_mdichild))
					throw new ArgumentException ("Cannot add a top level control to a control.", "value");
				/*
				if (child_is_mdichild && form_value.MdiParent != null && form_value.MdiParent != owner && form_value.MdiParent != owner.Parent) {
					throw new ArgumentException ("Form cannot be added to the Controls collection that has a valid MDI parent.", "value");
				}
				*/				
				
				//value.recalculate_distances = true;
				
				if (Contains (value))
				{
					owner.PerformLayout ();
					return;
				}
				
				if (value.parent != null)
				{
					value.parent.Controls.Remove (value);
				}
				
				all_controls = null;
				list.Add (value);
				
				value.ChangeParent (owner);
				
				//value.InitLayout();
				
				//if (owner.Visible)
				//	owner.UpdateChildrenZOrder();
				owner.PerformLayout (value, "Parent");
				owner.OnControlAdded (new ControlEventArgs (value));
			}

			internal void AddToList (Control c)
			{
				all_controls = null;
				list.Add (c);
			}

			internal virtual void AddImplicit (Control control)
			{
				if (impl_list == null)
					impl_list = new ArrayList ();
				
				if (AllContains (control))
				{
					owner.PerformLayout ();
					return;
				}
				
				if (control.parent != null)
				{
					control.parent.Controls.Remove (control);
				}
				
				all_controls = null;
				impl_list.Add (control);
				
				control.ChangeParent (owner);
				//control.InitLayout ();
				//if (owner.Visible)
				//	owner.UpdateChildrenZOrder ();
				
				// If we are adding a new control that isn't
				// visible, don't trigger a layout
				if (control.VisibleInternal)
					owner.PerformLayout (control, "Parent");
			}

			[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
			public virtual void AddRange (Control[] controls)
			{
				if (controls == null)
					throw new ArgumentNullException ("controls");
				
				owner.SuspendLayout ();
				
				try
				{
					for (int i = 0; i < controls.Length; i++)
						Add (controls[i]);
				}
				finally
				{
					owner.ResumeLayout ();
				}
			}

			internal virtual void AddRangeImplicit (Control[] controls)
			{
				if (controls == null)
					throw new ArgumentNullException ("controls");
				
				owner.SuspendLayout ();
				
				try
				{
					for (int i = 0; i < controls.Length; i++)
						AddImplicit (controls[i]);
				}
				finally
				{
					owner.ResumeLayout (false);
				}
			}

			public virtual new void Clear ()
			{
				all_controls = null;
				
				// MS sends remove events in reverse order
				while (list.Count > 0)
				{
					Remove ((Control)list[list.Count - 1]);
				}
			}

			internal virtual void ClearImplicit ()
			{
				if (impl_list == null)
					return;
				all_controls = null;
				impl_list.Clear ();
			}

			public bool Contains (Control control)
			{
				return list.Contains (control);
			}

			internal bool ImplicitContains (Control value)
			{
				if (impl_list == null)
					return false;
				
				return impl_list.Contains (value);
			}

			internal bool AllContains (Control value)
			{
				return Contains (value) || ImplicitContains (value);
			}

			public virtual bool ContainsKey (string key)
			{
				return IndexOfKey (key) >= 0;
			}

			// LAMESPEC: MSDN says AE, MS implementation throws ANE
			public Control[] Find (string key, bool searchAllChildren)
			{
				if (string.IsNullOrEmpty (key))
					throw new ArgumentNullException ("key");
				
				ArrayList al = new ArrayList ();
				
				foreach (Control c in list)
				{
					if (c.Name.Equals (key, StringComparison.CurrentCultureIgnoreCase))
						al.Add (c);
					
					if (searchAllChildren)
						al.AddRange (c.Controls.Find (key, true));
				}
				
				return (Control[])al.ToArray (typeof(Control));
			}
			public int GetChildIndex (Control child)
			{
				return GetChildIndex (child, false);
			}

			public virtual int GetChildIndex (Control child, bool throwException)
			{
				int index;
				
				index = list.IndexOf (child);
				
				if (index == -1 && throwException)
				{
					throw new ArgumentException ("Not a child control", "child");
				}
				return index;
			}

			public override IEnumerator GetEnumerator ()
			{
				return new ControlCollectionEnumerator (list);
			}

			internal IEnumerator GetAllEnumerator ()
			{
				Control[] res = GetAllControls ();
				return res.GetEnumerator ();
			}

			internal ArrayList ImplicitControls {
				get { return impl_list; }
			}

			internal Control[] GetAllControls ()
			{
				if (all_controls != null)
					return all_controls;
				
				if (impl_list == null)
				{
					all_controls = (Control[])list.ToArray (typeof(Control));
					return all_controls;
				}
				
				all_controls = new Control[list.Count + impl_list.Count];
				impl_list.CopyTo (all_controls);
				list.CopyTo (all_controls, impl_list.Count);
				
				return all_controls;
			}


			public int IndexOf (Control control)
			{
				return list.IndexOf (control);
			}

			public virtual int IndexOfKey (string key)
			{
				if (string.IsNullOrEmpty (key))
					return -1;
				
				for (int i = 0; i < list.Count; i++)
					if (((Control)list[i]).Name.Equals (key, StringComparison.CurrentCultureIgnoreCase))
						return i;
				
				return -1;
			}

			public virtual void Remove (Control value)
			{
				if (value == null)
					return;
				
				all_controls = null;
				list.Remove (value);
				
				owner.PerformLayout (value, "Parent");
				owner.OnControlRemoved (new ControlEventArgs (value));
				
				//ContainerControl container = owner.InternalGetContainerControl ();
				//if (container != null) { 
				// Inform any container controls about the loss of a child control
				// so that they can update their active control
				//	container.ChildControlRemoved (value);
				//}
				
				value.ChangeParent (null);
				
				//owner.UpdateChildrenZOrder();
			}

			internal virtual void RemoveImplicit (Control control)
			{
				if (impl_list != null)
				{
					all_controls = null;
					impl_list.Remove (control);
					owner.PerformLayout (control, "Parent");
					owner.OnControlRemoved (new ControlEventArgs (control));
				}
				control.ChangeParent (null);
				//owner.UpdateChildrenZOrder ();
			}

			public void RemoveAt (int index)
			{
				if (index < 0 || index >= list.Count)
					throw new ArgumentOutOfRangeException ("index", index, "ControlCollection does not have that many controls");
				
				Remove ((Control)list[index]);
			}

			public virtual void RemoveByKey (string key)
			{
				int index = IndexOfKey (key);
				
				if (index >= 0)
					RemoveAt (index);
			}

			public virtual void SetChildIndex (Control child, int newIndex)
			{
				if (child == null)
					throw new ArgumentNullException ("child");
				
				int old_index;
				
				old_index = list.IndexOf (child);
				if (old_index == -1)
				{
					throw new ArgumentException ("Not a child control", "child");
				}
				
				if (old_index == newIndex)
				{
					return;
				}
				
				all_controls = null;
				list.RemoveAt (old_index);
				
				if (newIndex > list.Count)
				{
					list.Add (child);
				}

				else
				{
					list.Insert (newIndex, child);
				}
				//child.UpdateZOrder();
				owner.PerformLayout ();
			}

			#endregion


			#region ControlCollection Interface Methods

			int IList.Add (object control)
			{
				if (!(control is Control))
					throw new ArgumentException ("Object of type Control required", "control");
				
				if (control == null)
					throw new ArgumentException ("control", "Cannot add null controls");
				
				this.Add ((Control)control);
				return this.IndexOf ((Control)control);
			}

			void IList.Remove (object control)
			{
				if (!(control is Control))
					throw new ArgumentException ("Object of type Control required", "control");
				
				this.Remove ((Control)control);
			}

			Object ICloneable.Clone ()
			{
				ControlCollection clone = new ControlCollection (this.owner);
				clone.list = (ArrayList)list.Clone ();
				// FIXME: Do we need this?
				return clone;
			}

			#endregion

			internal class ControlCollectionEnumerator : IEnumerator
			{
				private ArrayList list;
				int position = -1;

				public ControlCollectionEnumerator (ArrayList collection)
				{
					list = collection;
				}

				#region IEnumerator Members
				public object Current {
					get {
						try
						{
							return list[position];
						}
						catch (IndexOutOfRangeException)
						{
							throw new InvalidOperationException ();
						}
					}
				}

				public bool MoveNext ()
				{
					position++;
					return (position < list.Count);
				}

				public void Reset ()
				{
					position = -1;
				}
				
				#endregion
			}
		}
		#endregion
		
	}
}

