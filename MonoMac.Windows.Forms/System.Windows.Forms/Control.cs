using System;
using System.Drawing;
using MonoMac.AppKit;
using System.Linq;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
	
	[ComVisible(true)]
	[ClassInterface (ClassInterfaceType.AutoDispatch)]
	public partial class Control : Component , IBindableComponent, IWin32Window , IDropTarget, IBounds, ISynchronizeInvoke
	{
		internal virtual NSView c_helper { get; set; }
		#region constructors
		public Control ()
		{
			initialize ();
		}
		public Control (string text)
		{
			initialize ();
			Text = text;
		}
		public Control (Control control, string text)
		{
			initialize ();
			Text = text;
			control.Controls.Add (this);
		}
		public Control (string text, int left, int top, int width, int height)
		{
			initialize ();
			Text = text;
			Location = new Point (left, top);
			Size = new Size (width, height);
		}
		public Control (Control control, string text, int left, int top, int width, int height)
		{
			initialize ();
			Text = text;
			Location = new Point (left, top);
			Size = new Size (width, height);
			control.Controls.Add (this);
		}

		private void initialize ()
		{
			c_helper = new NSView ();
			MaximumSize = DefaultMaximumSize;
			MinimumSize = DefaultMinimumSize;
		}

		public static implicit operator NSView (Control control)
		{
			return control.c_helper;
		}

		#endregion
		
		#region internal Variables

		Control 				parent;
		BindingContext          binding_context;
		Image                   background_image; // background image for control
		ImageLayout 			backgroundimage_layout;		
		internal object			creator_thread;		// thread that created the control
		ControlBindingsCollection data_bindings;
		internal Color			foreground_color;	// foreground color for control
		RightToLeft             right_to_left; // drawing direction for control		
		internal int layout_suspended;
		bool                    causes_validation; // tracks if validation is executed on changes
		#endregion
		
		#region Public Instance Variables
		

		[DefaultValue(true)]
		[MWFCategory("Focus")]
		public bool CausesValidation {
			get {
				return this.causes_validation;
			}

			set {
				if (this.causes_validation != value) {
					causes_validation = value;
					OnCausesValidationChanged(EventArgs.Empty);
				}
			}
		}
		
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Rectangle DisplayRectangle {
			get {
				return this.ClientRectangle;
			}
		}
		
		public bool Contains(Control ctl) {
			while (ctl != null) {
				ctl = ctl.parent;
				if (ctl == this) {
					return true;
				}
			}
			return false;
		}
		
		
		public Form FindForm() {
			Control	c;

			c = this;
			while (c != null) {
				if (c is Form) {
					return (Form)c;
				}
				c = c.Parent;
			}
			return null;
		}
		
		
		[Localizable(true)]
		[MWFCategory("Layout")]
		public Padding Padding {
			get {
				return padding;
			}

			set {
				if (padding != value) {
					padding = value;
					OnPaddingChanged (EventArgs.Empty);
					
					// Changing padding generally requires a new size
					if (this.AutoSize && this.Parent != null)
						parent.PerformLayout (this, "Padding");
					else
						PerformLayout (this, "Padding");
				}
			}
		}

		internal bool ShouldSerializePadding ()
		{
			return this.Padding != DefaultPadding;
		}
		
		[AmbientValue(RightToLeft.Inherit)]
		[Localizable(true)]
		[MWFCategory("Appearance")]
		public virtual RightToLeft RightToLeft {
			get {
				if (right_to_left == RightToLeft.Inherit) {
					if (parent != null)
						return parent.RightToLeft;
					else
						return RightToLeft.No; // default value
				}
				return right_to_left;
			}

			set {
				if (value != right_to_left) {
					right_to_left = value;
					OnRightToLeftChanged(EventArgs.Empty);
					PerformLayout ();
				}
			}
		}
		

		[DispId(-516)]
		[DefaultValue(true)]
		[MWFCategory("Behavior")]
		public bool TabStop {
			get {
				return tab_stop;
			}

			set {
				if (tab_stop != value) {
					tab_stop = value;
					OnTabStopChanged(EventArgs.Empty);
				}
			}
		}
		
		#endregion
		
		#region Public Instance Methods
		
		
		internal ContainerControl FindContainer (Control c)
		{
			while ((c != null) && !(c is ContainerControl))
				c = c.Parent;
			return c as ContainerControl;
		}

		public IContainerControl GetContainerControl() {
			Control	current = this;

			while (current!=null) {
				if ((current is IContainerControl) && ((current.control_style & ControlStyles.ContainerControl)!=0)) {
					return (IContainerControl)current;
				}
				current = current.parent;
			}
			return null;
		}
		
		public Control GetNextControl(Control ctl, bool forward) {

			if (!this.Contains(ctl)) {
				ctl = this;
			}

			if (forward) {
				ctl = FindControlForward(this, ctl);
			}
			else {
				ctl = FindControlBackward(this, ctl);
			}

			if (ctl != this) {
				return ctl;
			}
			return null;
		}
		
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public void Scale (SizeF factor)
		{
			BoundsSpecified bounds_spec = BoundsSpecified.All;

			SuspendLayout ();

			if (this is ContainerControl) {
				if ((this as ContainerControl).IsAutoScaling)
					bounds_spec = BoundsSpecified.Size;
				else if (IsContainerAutoScaling (this.Parent))
					bounds_spec = BoundsSpecified.Location;
			}

			ScaleControl (factor, bounds_spec);

			// Scale children
			if ((bounds_spec != BoundsSpecified.Location) && ScaleChildren) {
				foreach (Control c in Controls.GetAllControls ()) {
					c.Scale (factor);
					if (c is ContainerControl) {
						ContainerControl cc = c as ContainerControl;
						if ((cc.AutoScaleMode == AutoScaleMode.Inherit) && IsContainerAutoScaling (this))
							cc.PerformAutoScale (true);
					}
				}
			}

			ResumeLayout ();
		}

		public bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap) {
			Control c;

			if (!this.Contains(ctl) || (!nested && (ctl.parent != this))) {
				ctl = null;
			}
			c = ctl;
			do {
				c = GetNextControl(c, forward);
				if (c == null) {
					if (wrap) {
						wrap = false;
						continue;
					}
					break;
				}
				if (c.CanSelect && ((c.parent == this) || nested) && (c.tab_stop || !tabStopOnly)) {
					c.Select (true, true);
					return true;
				}
			} while (c != ctl); // If we wrap back to ourselves we stop

			return false;
		}
		
		public void SuspendLayout ()
		{
			
		}
		
		public void ResumeLayout() {
			ResumeLayout (true);
		}

		public void ResumeLayout(bool performLayout) {
			if (layout_suspended > 0) {
				layout_suspended--;
			}

			if (layout_suspended == 0) {
				if (this is ContainerControl)
					(this as ContainerControl).PerformDelayedAutoScale();

				if (!performLayout)
					foreach (Control c in Controls.GetAllControls ())
						c.UpdateDistances ();

				if (performLayout && layout_pending) {
					performLayout();
				}
			}
		}
		
		internal virtual void performLayout ()
		{
			m_helper.ContentView.DisplayIfNeeded ();
		}
		
		#endregion
		
		#region Protected Instance Variables
		

		protected virtual bool CanEnableIme {
			get { return false; }
		}
		
			[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual Rectangle GetScaledBounds (Rectangle bounds, SizeF factor, BoundsSpecified specified)
		{
			// Top level controls do not scale location
			if (!is_toplevel) {
				if ((specified & BoundsSpecified.X) == BoundsSpecified.X)
					bounds.X = (int)Math.Round (bounds.X * factor.Width);
				if ((specified & BoundsSpecified.Y) == BoundsSpecified.Y)
					bounds.Y = (int)Math.Round (bounds.Y * factor.Height);
			}

			if ((specified & BoundsSpecified.Width) == BoundsSpecified.Width && !GetStyle (ControlStyles.FixedWidth)) {
				int border = (this.bounds.Width - this.client_size.Width);
				bounds.Width = (int)Math.Round ((bounds.Width - border) * factor.Width + border);
			}
			if ((specified & BoundsSpecified.Height) == BoundsSpecified.Height && !GetStyle (ControlStyles.FixedHeight)) {
				int border = (this is ComboBox) ? (ThemeEngine.Current.Border3DSize.Height * 2) :
					(this.bounds.Height - this.client_size.Height);
				bounds.Height = (int)Math.Round ((bounds.Height - border) * factor.Height + border);
			}

			return bounds;
		}
		
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void ScaleControl (SizeF factor, BoundsSpecified specified)
		{
			Rectangle new_bounds = GetScaledBounds (bounds, factor, specified);

			SetBounds (new_bounds.X, new_bounds.Y, new_bounds.Width, new_bounds.Height, specified);
		}
		
		[EditorBrowsable (EditorBrowsableState.Never)]
		protected virtual void ScaleCore (float dx, float dy)
		{
			Rectangle new_bounds = GetScaledBoundsOld (bounds, new SizeF (dx, dy), BoundsSpecified.All);

			SuspendLayout ();

			SetBounds (new_bounds.X, new_bounds.Y, new_bounds.Width, new_bounds.Height, BoundsSpecified.All);

			if (ScaleChildrenInternal)
				foreach (Control c in Controls.GetAllControls ())
					c.Scale (dx, dy);

			ResumeLayout ();
		}
		
		
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void PerformLayout(Control affectedControl, string affectedProperty) {
			LayoutEventArgs levent = new LayoutEventArgs(affectedControl, affectedProperty);

			foreach (Control c in Controls.GetAllControls ())
				if (c.recalculate_distances)
					c.UpdateDistances ();

			if (layout_suspended > 0) {
				layout_pending = true;
				return;
			}
					
			layout_pending = false;

			// Prevent us from getting messed up
			layout_suspended++;

			// Perform all Dock and Anchor calculations
			try {
				OnLayout(levent);
			}

				// Need to make sure we decremend layout_suspended
			finally {
				layout_suspended--;
			}
		}
		
		// Override this if there is a control that shall always remain on
		// top of other controls (such as scrollbars). If there are several
		// of these controls, the bottom-most should be returned.
		internal virtual IntPtr AfterTopMostControl () {
			return IntPtr.Zero;
		}

		
		#endregion
		
		#region Protected Instance Methods
		
		
		protected bool GetTopLevel() {
			return is_toplevel;
		}
		
		protected virtual bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if ((context_menu != null) && context_menu.ProcessCmdKey(ref msg, keyData)) {
				return true;
			}

			if (parent != null) {
				return parent.ProcessCmdKey(ref msg, keyData);
			}

			return false;
		}
		
		protected virtual bool ProcessDialogChar(char charCode) {
			if (parent != null) {
				return parent.ProcessDialogChar (charCode);
			}

			return false;
		}
		

		protected virtual bool ProcessDialogKey (Keys keyData) {
			if (parent != null) {
				return parent.ProcessDialogKey (keyData);
			}

			return false;
		}		

		protected virtual bool ProcessMnemonic(char charCode) {
			// override me
			return false;
		}
		
		protected virtual void Select(bool directed, bool forward) {
			IContainerControl	container;
			
			container = GetContainerControl();
			if (container != null && (Control)container != this)
				container.ActiveControl = this;
		}

		#endregion
		
		#region Private and Internal Methods
		
		

		
		internal virtual void FireEnter () {
			OnEnter (EventArgs.Empty);
		}

		internal virtual void FireLeave () {
			OnLeave (EventArgs.Empty);
		}

		internal virtual void FireValidating (CancelEventArgs ce) {
			OnValidating (ce);
		}

		internal virtual void FireValidated () {
			OnValidated (EventArgs.Empty);
		}

		protected internal bool GetStyle(ControlStyles flag) {
			return (control_style & flag) != 0;
		}
		
		internal bool Select(Control control) {
			IContainerControl	container;

			if (control == null) {
				return false;
			}

			container = GetContainerControl();
			if (container != null && (Control)container != control) {
				container.ActiveControl = control;
				if (container.ActiveControl == control && !control.has_focus && control.IsHandleCreated)
					control.c_helper.BecomeFirstResponder();
			}
			else if (control.IsHandleCreated) {
				control.c_helper.BecomeFirstResponder();
			}
			return true;
		}
		

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected internal void SetStyle(ControlStyles flag, bool value) {
			if (value) {
				control_style |= flag;
			} else {
				control_style &= ~flag;
			}
		}
		
		internal virtual bool ProcessControlMnemonic(char charCode) {
			return ProcessMnemonic(charCode);
		}
		#endregion //Private and Internal Methods
		
		#region Internal Properties
		

		// Looks for focus in child controls
		// and also in the implicit ones
		internal bool InternalContainsFocus {
			get {
				IntPtr focused_window;

				focused_window = c_helper.Window.FirstResponder;
				if (IsHandleCreated) {
					if (focused_window == Handle)
						return true;

					foreach (Control child_control in child_controls.GetAllControls ())
						if (child_control.InternalContainsFocus)
							return true;
				}

				return false;
			}
		}
		
		

		internal bool VisibleInternal {
			get { return is_visible; }
		}
		
		#endregion

		#region members
		public virtual AnchorStyles Anchor { get; set; }

		public virtual Point AutoScrollOffset {
			get { return new Point (0, 0); }
			set { }
		}

		public virtual bool AutoSize {get;set;}

		public virtual Color BackColor { get; set; }

		public virtual Image BackgroundImage { get; set; }

		public virtual ImageLayout BackgroundImageLayout { get; set; }

		public int Bottom {
			get {
				if (c_helper.Superview == null)
					return 0;
				return (int)(c_helper.Superview.Frame.Height - (this.Height + this.Location.Y));
			}
		}
		
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual BindingContext BindingContext {
			get {
				if (binding_context != null)
					return binding_context;
				if (Parent == null)
					return null;
				binding_context = Parent.BindingContext;
				return binding_context;
			}
			set {
				if (binding_context != value) {
					binding_context = value;
					OnBindingContextChanged(EventArgs.Empty);
				}
			}
		}
		public Rectangle Bounds {
			get { return Rectangle.Round (c_helper.Bounds); }
			set { c_helper.Bounds = value; }
		}

		public bool CanFocus {
			get { return c_helper.AcceptsFirstResponder (); }
		}


		public Rectangle ClientRectangle {
			get { return Rectangle.Round (c_helper.Frame); }
		}

		internal virtual Size clientSize {
			get { return new Size ((int)Size.Width, (int)Size.Height); }
			set { Size = value; }
		}

		public Size ClientSize {
			get { return clientSize; }
			set { clientSize = value; }
		}

		public virtual bool ContainsFocus {
			get { return c_helper == c_helper.Window.FirstResponder || c_helper.Subviews.Where (x => x == c_helper.Window.FirstResponder).Count () > 0; }
		}


		internal ControlCollection child_controls;
		internal virtual ControlCollection controls {
			get {
				if (child_controls == null)
					child_controls = new ControlCollection (c_helper);
				return child_controls;
			}
		}
		public virtual ControlCollection Controls {
			get { return controls; }
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ParenthesizePropertyName(true)]
		[RefreshProperties(RefreshProperties.All)]
		public ControlBindingsCollection DataBindings {
			get {
				if (data_bindings == null)
					data_bindings = new ControlBindingsCollection (this);
				return data_bindings;
			}
		}

		public static Color DefaultBackColor {
			get { return Color.Gray; }
		}
		
		protected virtual Padding DefaultMargin {
			get { return new Padding (3); }
		}
		
		protected virtual Size DefaultMaximumSize { get { return new Size (); } }
		protected virtual Size DefaultMinimumSize { get { return new Size (); } }
		protected virtual Padding DefaultPadding { get { return new Padding (); } }
		
		protected virtual Size DefaultSize {
			get {
				return new Size(0, 0);
			}
		}
		public virtual DockStyle Dock { get; set; }

		public bool Enabled {
			get { return c_helper.AcceptsFirstResponder (); }
		}
		//TODO: Setter

		public virtual bool Focused {
			get { return c_helper == c_helper.Window.FirstResponder; }
		}

		internal virtual NSFont font { get; set; }
		public virtual System.Drawing.Font Font {
			get {
				if (font == null)
					return new System.Drawing.Font ("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				return new System.Drawing.Font (font.FontName, font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}

			set { font = MonoMac.AppKit.NSFont.FromFontName (value.Name, value.Size); }
		}
		protected int FontHeight {
			get { return Font.Height; }
		}
		//set { Font.Height = value;}
		//TODO: set
		private Color foreColor = Color.Gray;
		public virtual Color ForeColor {
			get { return foreColor; }
			set { foreColor = value; }
		}

		public virtual IntPtr Handle {
			get { return c_helper.Handle; }
		}

		public bool HasChildren {
			get { return Controls.Count > 0; }
		}

		public int Height {
			get { return (int)this.Size.Height; }
			set { this.Size = new Size (this.Size.Width, value); }
		}
		
		public bool InvokeRequired {						// ISynchronizeInvoke
			get {
				//if (creator_thread != null && creator_thread!=Thread.CurrentThread) {
				//	return true;
				//}
				return false;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsHandleCreated {
			get {
				if (c_helper == null || c_helper.Window.Handle == IntPtr.Zero)
					return false;

				return true;
			}
		}

		public int Left {
			get { return ((int)Location.X); }
			set { Location = new Point (value, Location.Y); }
		}

		internal virtual Point location {
			get { return Point.Round (c_helper.Frame.Location); }
			set { c_helper.Frame = new RectangleF (value, c_helper.Frame.Size); }
		}

		public Point Location {
			get { return location; }
			set { location = value; }
		}

		public virtual Size MaximumSize { get; set; }

		public virtual Size MinimumSize { get; set; }

		public string Name { get; set; }
		public Control Parent {
			get {
				if (c_helper.Superview == null)
					return null;
				if (c_helper is IViewHelper)
					return (c_helper as IViewHelper).Host;
				return null;
			}
			set {
				c_helper.RemoveFromSuperview ();
				value.Controls.Add (this);
			}
		}

		internal virtual Size size {
			get { return Size.Round (c_helper.Frame.Size); }
			set { c_helper.Frame = new RectangleF (c_helper.Frame.Location, value); }
		}

		public Size Size {
			get { return size; }
			set {
				int h = value.Height;
				int w = value.Width;
				if (h > MaximumSize.Height && !MaximumSize.IsEmpty)
					h = MaximumSize.Height;
				else if (h < MinimumSize.Height && !MinimumSize.IsEmpty)
					h = MinimumSize.Height;

				if (w > MaximumSize.Width && !MaximumSize.IsEmpty)
					w = MaximumSize.Width;
				else if (w < MinimumSize.Width && !MinimumSize.IsEmpty)
					w = MinimumSize.Width;
				size = new Size (w, h);
			}
		}

		[Obsolete("Not Implemented.", false)]
		public int TabIndex { get; set; }

		public virtual string Text { get; set; }

		public int Top {
			get{ return Location.Y;}
			set{ Location = new Point(Location.X,value);}
		}
		
		public int Right{
			get {
				return this.Location.X+this.Location.Width;
			}
		}

		public bool Visible {
			get { return c_helper.Hidden; }
			set { c_helper.Hidden = value; }
		}

		public int Width {
			get { return (int)this.Size.Width; }
			set { this.Size = new Size (value, this.Size.Height); }
		}
		#endregion
		
		#region misc
		public virtual void Refresh()
		{
			c_helper.Display();
		}
		public void PerformLayout()
		{
			//TODO:
		}

		public void Invalidate()
		{
			if(c_helper != null)
				c_helper.NeedsDisplay = true;
		}

		private void CheckDataBindings () {
			if (data_bindings == null)
				return;

			foreach (Binding binding in data_bindings) {
				binding.Check ();
			}
		}
		
		
		
		// This method exists so controls overriding OnPaintBackground can have default background painting done
		internal virtual void PaintControlBackground (PaintEventArgs pevent) {

	
	
			if (background_image == null) {
					Rectangle paintRect = new Rectangle(pevent.ClipRectangle.X, pevent.ClipRectangle.Y, pevent.ClipRectangle.Width, pevent.ClipRectangle.Height);
					Brush pen =  new SolidBrush(BackColor);//ThemeEngine.Current.ResPool.GetSolidBrush(BackColor);
					pevent.Graphics.FillRectangle(pen, paintRect);
				
				return;
			}

			DrawBackgroundImage (pevent.Graphics);
		}

		void DrawBackgroundImage (Graphics g) {
			Rectangle drawing_rectangle = new Rectangle ();
			g.FillRectangle (new SolidBrush (BackColor), ClientRectangle);
				
			switch (backgroundimage_layout)
			{
			case ImageLayout.Tile:
				using (TextureBrush b = new TextureBrush (background_image, WrapMode.Tile)) {
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
				if ((float)background_image.Width / (float)background_image.Height < (float)drawing_rectangle.Width / (float) drawing_rectangle.Height) {
					drawing_rectangle.Width = (int) (background_image.Width * ((float)drawing_rectangle.Height / (float)background_image.Height));
					drawing_rectangle.X = (ClientRectangle.Width - drawing_rectangle.Width) / 2;
				} else {
					drawing_rectangle.Height = (int) (background_image.Height * ((float)drawing_rectangle.Width / (float)background_image.Width));
					drawing_rectangle.Y = (ClientRectangle.Height - drawing_rectangle.Height) / 2;
				}
				break;
			default:
				return;
			}

			g.DrawImage (background_image, drawing_rectangle);

		}
		
		#endregion

		#region OnXXX methods
		protected virtual void OnAutoSizeChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events[AutoSizeChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void OnBackColorChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [BackColorChangedEvent]);
			if (eh != null)
				eh (this, e);
			for (int i=0; i<child_controls.Count; i++) ((child_controls[i] as IViewHelper).Host).OnParentBackColorChanged(e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnBackgroundImageChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [BackgroundImageChangedEvent]);
			if (eh != null)
				eh (this, e);
			for (int i=0; i<child_controls.Count; i++) ((child_controls[i] as IViewHelper).Host).OnParentBackgroundImageChanged(e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void OnBackgroundImageLayoutChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events[BackgroundImageLayoutChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnBindingContextChanged(EventArgs e) {
			CheckDataBindings ();
			EventHandler eh = (EventHandler)(Events [BindingContextChangedEvent]);
			if (eh != null)
				eh (this, e);
			for (int i=0; i<child_controls.Count; i++)  ((child_controls[i] as IViewHelper).Host).OnParentBindingContextChanged(e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnCausesValidationChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [CausesValidationChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnChangeUICues(UICuesEventArgs e) {
			UICuesEventHandler eh = (UICuesEventHandler)(Events [ChangeUICuesEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnClick(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [ClickEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void OnClientSizeChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events[ClientSizeChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnContextMenuChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [ContextMenuChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void OnContextMenuStripChanged (EventArgs e) {
			EventHandler eh = (EventHandler)(Events [ContextMenuStripChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnControlAdded(ControlEventArgs e) {
			ControlEventHandler eh = (ControlEventHandler)(Events [ControlAddedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnControlRemoved(ControlEventArgs e) {
			ControlEventHandler eh = (ControlEventHandler)(Events [ControlRemovedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnCreateControl() {
			// Override me!
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnCursorChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [CursorChangedEvent]);
			if (eh != null)
				eh (this, e);

			for (int i = 0; i < child_controls.Count; i++) ((child_controls[i] as IViewHelper).Host).OnParentCursorChanged (e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDockChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [DockChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDoubleClick(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [DoubleClickEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDragDrop(DragEventArgs drgevent) {
			DragEventHandler eh = (DragEventHandler)(Events [DragDropEvent]);
			if (eh != null)
				eh (this, drgevent);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDragEnter(DragEventArgs drgevent) {
			DragEventHandler eh = (DragEventHandler)(Events [DragEnterEvent]);
			if (eh != null)
				eh (this, drgevent);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDragLeave(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [DragLeaveEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDragOver(DragEventArgs drgevent) {
			DragEventHandler eh = (DragEventHandler)(Events [DragOverEvent]);
			if (eh != null)
				eh (this, drgevent);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnEnabledChanged(EventArgs e) {
			if (IsHandleCreated) {
				Refresh();
			}

			EventHandler eh = (EventHandler)(Events [EnabledChangedEvent]);
			if (eh != null)
				eh (this, e);

			foreach (Control c in Controls.GetAllControls ())
				c.OnParentEnabledChanged (e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnEnter(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [EnterEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnFontChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [FontChangedEvent]);
			if (eh != null)
				eh (this, e);
			for (int i=0; i<child_controls.Count; i++) ((child_controls[i] as IViewHelper).Host).OnParentFontChanged(e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnForeColorChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [ForeColorChangedEvent]);
			if (eh != null)
				eh (this, e);
			for (int i=0; i<child_controls.Count; i++) ((child_controls[i] as IViewHelper).Host).OnParentForeColorChanged(e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnGiveFeedback(GiveFeedbackEventArgs gfbevent) {
			GiveFeedbackEventHandler eh = (GiveFeedbackEventHandler)(Events [GiveFeedbackEvent]);
			if (eh != null)
				eh (this, gfbevent);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnGotFocus(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [GotFocusEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnHandleCreated(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [HandleCreatedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnHandleDestroyed(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [HandleDestroyedEvent]);
			if (eh != null)
				eh (this, e);
		}

		internal void RaiseHelpRequested (HelpEventArgs hevent) {
			OnHelpRequested (hevent);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnHelpRequested(HelpEventArgs hevent) {
			HelpEventHandler eh = (HelpEventHandler)(Events [HelpRequestedEvent]);
			if (eh != null)
				eh (this, hevent);
		}

		protected virtual void OnImeModeChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [ImeModeChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnInvalidated(InvalidateEventArgs e) {
			if(c_helper != null)
				c_helper.SetNeedsDisplayInRect(e.InvalidRect);

			InvalidateEventHandler eh = (InvalidateEventHandler)(Events [InvalidatedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnKeyDown(KeyEventArgs e) {
			KeyEventHandler eh = (KeyEventHandler)(Events [KeyDownEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnKeyPress(KeyPressEventArgs e) {
			KeyPressEventHandler eh = (KeyPressEventHandler)(Events [KeyPressEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnKeyUp(KeyEventArgs e) {
			KeyEventHandler eh = (KeyEventHandler)(Events [KeyUpEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnLayout(LayoutEventArgs levent) {
			LayoutEventHandler eh = (LayoutEventHandler)(Events [LayoutEvent]);
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
		protected virtual void OnLeave(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [LeaveEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnLocationChanged(EventArgs e) {
			OnMove(e);
			EventHandler eh = (EventHandler)(Events [LocationChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnLostFocus(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [LostFocusEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnMarginChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events[MarginChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void OnMouseCaptureChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [MouseCaptureChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void OnMouseClick (MouseEventArgs e)
		{
			MouseEventHandler eh = (MouseEventHandler)(Events [MouseClickEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void OnMouseDoubleClick (MouseEventArgs e)
		{
			MouseEventHandler eh = (MouseEventHandler)(Events [MouseDoubleClickEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnMouseDown(MouseEventArgs e) {
			MouseEventHandler eh = (MouseEventHandler)(Events [MouseDownEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnMouseEnter(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [MouseEnterEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnMouseHover(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [MouseHoverEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnMouseLeave(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [MouseLeaveEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnMouseMove(MouseEventArgs e) {
			MouseEventHandler eh = (MouseEventHandler)(Events [MouseMoveEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnMouseUp(MouseEventArgs e) {
			MouseEventHandler eh = (MouseEventHandler)(Events [MouseUpEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnMouseWheel(MouseEventArgs e) {
			MouseEventHandler eh = (MouseEventHandler)(Events [MouseWheelEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnMove(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [MoveEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnNotifyMessage(Message m) {
			// Override me!
		}

		protected virtual void OnPaddingChanged (EventArgs e) {
			EventHandler eh = (EventHandler) (Events [PaddingChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnPaint(PaintEventArgs e) {
			PaintEventHandler eh = (PaintEventHandler)(Events [PaintEvent]);
			if (eh != null)
				eh (this, e);
		}

		internal virtual void OnPaintBackgroundInternal(PaintEventArgs e) {
			// Override me
		}

		internal virtual void OnPaintInternal(PaintEventArgs e) {
			// Override me
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnPaintBackground(PaintEventArgs pevent) {
			PaintControlBackground (pevent);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnParentBackColorChanged(EventArgs e) {
			if (BackColor.IsEmpty && background_image==null) {
				Invalidate();
				OnBackColorChanged(e);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnParentBackgroundImageChanged(EventArgs e) {
			Invalidate();
			OnBackgroundImageChanged(e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnParentBindingContextChanged(EventArgs e) {
			if (binding_context==null && Parent != null) {
				binding_context=Parent.binding_context;
				OnBindingContextChanged(e);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnParentChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [ParentChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void OnParentCursorChanged (EventArgs e)
		{
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void OnParentEnabledChanged(EventArgs e) {
			//if (is_enabled) {
			//	OnEnabledChanged(e);
			//}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnParentFontChanged(EventArgs e) {
			if (font==null) {
				Invalidate();
				OnFontChanged(e);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnParentForeColorChanged(EventArgs e) {
			if (foreground_color.IsEmpty) {
				Invalidate();
				OnForeColorChanged(e);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnParentRightToLeftChanged(EventArgs e) {
			if (right_to_left==RightToLeft.Inherit) {
				Invalidate();
				OnRightToLeftChanged(e);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnParentVisibleChanged(EventArgs e) {
			if (Visible) {
				OnVisibleChanged(e);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnQueryContinueDrag (QueryContinueDragEventArgs qcdevent)
		{
			QueryContinueDragEventHandler eh = (QueryContinueDragEventHandler)(Events [QueryContinueDragEvent]);
			if (eh != null)
				eh (this, qcdevent);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void OnPreviewKeyDown (PreviewKeyDownEventArgs e)
		{
			PreviewKeyDownEventHandler eh = (PreviewKeyDownEventHandler)(Events[PreviewKeyDownEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void OnPrint (PaintEventArgs e)
		{
			PaintEventHandler eh = (PaintEventHandler)(Events[PaintEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void OnRegionChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events[RegionChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnResize(EventArgs e) {
			OnResizeInternal (e);
		}

		internal virtual void OnResizeInternal (EventArgs e) {
			//PerformLayout(this, "Bounds");

			EventHandler eh = (EventHandler)(Events [ResizeEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnRightToLeftChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [RightToLeftChangedEvent]);
			if (eh != null)
				eh (this, e);
			for (int i=0; i<child_controls.Count; i++) (child_controls[i] as IViewHelper).Host.OnParentRightToLeftChanged(e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnSizeChanged(EventArgs e) {
			OnResize(e);
			EventHandler eh = (EventHandler)(Events [SizeChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnStyleChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [StyleChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnSystemColorsChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [SystemColorsChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnTabIndexChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [TabIndexChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnTabStopChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [TabStopChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnTextChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [TextChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnValidated(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [ValidatedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnValidating(System.ComponentModel.CancelEventArgs e) {
			CancelEventHandler eh = (CancelEventHandler)(Events [ValidatingEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnVisibleChanged(EventArgs e) {
			//if (Visible)
				//CreateControl ();

			EventHandler eh = (EventHandler)(Events [VisibleChangedEvent]);
			if (eh != null)
				eh (this, e);

			// We need to tell our kids (including implicit ones)
			foreach (Control c in Controls.GetAllControls ())
				if (c.Visible)
					c.OnParentVisibleChanged (e);
		}
		#endregion	// OnXXX methods

		#region Events
		static object AutoSizeChangedEvent = new object ();
		static object BackColorChangedEvent = new object ();
		static object BackgroundImageChangedEvent = new object ();
		static object BackgroundImageLayoutChangedEvent = new object ();
		static object BindingContextChangedEvent = new object ();
		static object CausesValidationChangedEvent = new object ();
		static object ChangeUICuesEvent = new object ();
		static object ClickEvent = new object ();
		static object ClientSizeChangedEvent = new object ();
		static object ContextMenuChangedEvent = new object ();
		static object ContextMenuStripChangedEvent = new object ();
		static object ControlAddedEvent = new object ();
		static object ControlRemovedEvent = new object ();
		static object CursorChangedEvent = new object ();
		static object DockChangedEvent = new object ();
		static object DoubleClickEvent = new object ();
		static object DragDropEvent = new object ();
		static object DragEnterEvent = new object ();
		static object DragLeaveEvent = new object ();
		static object DragOverEvent = new object ();
		static object EnabledChangedEvent = new object ();
		static object EnterEvent = new object ();
		static object FontChangedEvent = new object ();
		static object ForeColorChangedEvent = new object ();
		static object GiveFeedbackEvent = new object ();
		static object GotFocusEvent = new object ();
		static object HandleCreatedEvent = new object ();
		static object HandleDestroyedEvent = new object ();
		static object HelpRequestedEvent = new object ();
		static object ImeModeChangedEvent = new object ();
		static object InvalidatedEvent = new object ();
		static object KeyDownEvent = new object ();
		static object KeyPressEvent = new object ();
		static object KeyUpEvent = new object ();
		static object LayoutEvent = new object ();
		static object LeaveEvent = new object ();
		static object LocationChangedEvent = new object ();
		static object LostFocusEvent = new object ();
		static object MarginChangedEvent = new object ();
		static object MouseCaptureChangedEvent = new object ();
		static object MouseClickEvent = new object ();
		static object MouseDoubleClickEvent = new object ();
		static object MouseDownEvent = new object ();
		static object MouseEnterEvent = new object ();
		static object MouseHoverEvent = new object ();
		static object MouseLeaveEvent = new object ();
		static object MouseMoveEvent = new object ();
		static object MouseUpEvent = new object ();
		static object MouseWheelEvent = new object ();
		static object MoveEvent = new object ();
		static object PaddingChangedEvent = new object ();
		static object PaintEvent = new object ();
		static object ParentChangedEvent = new object ();
		static object PreviewKeyDownEvent = new object ();
		static object QueryAccessibilityHelpEvent = new object ();
		static object QueryContinueDragEvent = new object ();
		static object RegionChangedEvent = new object ();
		static object ResizeEvent = new object ();
		static object RightToLeftChangedEvent = new object ();
		static object SizeChangedEvent = new object ();
		static object StyleChangedEvent = new object ();
		static object SystemColorsChangedEvent = new object ();
		static object TabIndexChangedEvent = new object ();
		static object TabStopChangedEvent = new object ();
		static object TextChangedEvent = new object ();
		static object ValidatedEvent = new object ();
		static object ValidatingEvent = new object ();
		static object VisibleChangedEvent = new object ();

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public event EventHandler AutoSizeChanged {
			add { Events.AddHandler (AutoSizeChangedEvent, value);}
			remove {Events.RemoveHandler (AutoSizeChangedEvent, value);}
		}

		public event EventHandler BackColorChanged {
			add { Events.AddHandler (BackColorChangedEvent, value); }
			remove { Events.RemoveHandler (BackColorChangedEvent, value); }
		}

		public event EventHandler BackgroundImageChanged {
			add { Events.AddHandler (BackgroundImageChangedEvent, value); }
			remove { Events.RemoveHandler (BackgroundImageChangedEvent, value); }
		}

		public event EventHandler BackgroundImageLayoutChanged {
			add {Events.AddHandler (BackgroundImageLayoutChangedEvent, value);}
			remove {Events.RemoveHandler (BackgroundImageLayoutChangedEvent, value);}
		}

		public event EventHandler BindingContextChanged {
			add { Events.AddHandler (BindingContextChangedEvent, value); }
			remove { Events.RemoveHandler (BindingContextChangedEvent, value); }
		}

		public event EventHandler CausesValidationChanged {
			add { Events.AddHandler (CausesValidationChangedEvent, value); }
			remove { Events.RemoveHandler (CausesValidationChangedEvent, value); }
		}

		public event UICuesEventHandler ChangeUICues {
			add { Events.AddHandler (ChangeUICuesEvent, value); }
			remove { Events.RemoveHandler (ChangeUICuesEvent, value); }
		}

		public event EventHandler Click {
			add { Events.AddHandler (ClickEvent, value); }
			remove { Events.RemoveHandler (ClickEvent, value); }
		}

		public event EventHandler ClientSizeChanged {
			add {Events.AddHandler (ClientSizeChangedEvent, value);}
			remove {Events.RemoveHandler (ClientSizeChangedEvent, value);}
		}

		[Browsable (false)]
		public event EventHandler ContextMenuChanged {
			add { Events.AddHandler (ContextMenuChangedEvent, value); }
			remove { Events.RemoveHandler (ContextMenuChangedEvent, value); }
		}

		public event EventHandler ContextMenuStripChanged {
			add { Events.AddHandler (ContextMenuStripChangedEvent, value); }
			remove { Events.RemoveHandler (ContextMenuStripChangedEvent, value);}
		}


		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(true)]
		public event ControlEventHandler ControlAdded {
			add { Events.AddHandler (ControlAddedEvent, value); }
			remove { Events.RemoveHandler (ControlAddedEvent, value); }
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(true)]
		public event ControlEventHandler ControlRemoved {
			add { Events.AddHandler (ControlRemovedEvent, value); }
			remove { Events.RemoveHandler (ControlRemovedEvent, value); }
		}

		public event EventHandler CursorChanged {
			add { Events.AddHandler (CursorChangedEvent, value); }
			remove { Events.RemoveHandler (CursorChangedEvent, value); }
		}
		public event EventHandler DockChanged {
			add { Events.AddHandler (DockChangedEvent, value); }
			remove { Events.RemoveHandler (DockChangedEvent, value); }
		}

		public event EventHandler DoubleClick {
			add { Events.AddHandler (DoubleClickEvent, value); }
			remove { Events.RemoveHandler (DoubleClickEvent, value); }
		}

		public event DragEventHandler DragDrop {
			add { Events.AddHandler (DragDropEvent, value); }
			remove { Events.RemoveHandler (DragDropEvent, value); }
		}

		public event DragEventHandler DragEnter {
			add { Events.AddHandler (DragEnterEvent, value); }
			remove { Events.RemoveHandler (DragEnterEvent, value); }
		}

		public event EventHandler DragLeave {
			add { Events.AddHandler (DragLeaveEvent, value); }
			remove { Events.RemoveHandler (DragLeaveEvent, value); }
		}

		public event DragEventHandler DragOver {
			add { Events.AddHandler (DragOverEvent, value); }
			remove { Events.RemoveHandler (DragOverEvent, value); }
		}

		public event EventHandler EnabledChanged {
			add { Events.AddHandler (EnabledChangedEvent, value); }
			remove { Events.RemoveHandler (EnabledChangedEvent, value); }
		}

		public event EventHandler Enter {
			add { Events.AddHandler (EnterEvent, value); }
			remove { Events.RemoveHandler (EnterEvent, value); }
		}

		public event EventHandler FontChanged {
			add { Events.AddHandler (FontChangedEvent, value); }
			remove { Events.RemoveHandler (FontChangedEvent, value); }
		}

		public event EventHandler ForeColorChanged {
			add { Events.AddHandler (ForeColorChangedEvent, value); }
			remove { Events.RemoveHandler (ForeColorChangedEvent, value); }
		}

		public event GiveFeedbackEventHandler GiveFeedback {
			add { Events.AddHandler (GiveFeedbackEvent, value); }
			remove { Events.RemoveHandler (GiveFeedbackEvent, value); }
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		public event EventHandler GotFocus {
			add { Events.AddHandler (GotFocusEvent, value); }
			remove { Events.RemoveHandler (GotFocusEvent, value); }
		}


		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		public event EventHandler HandleCreated {
			add { Events.AddHandler (HandleCreatedEvent, value); }
			remove { Events.RemoveHandler (HandleCreatedEvent, value); }
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		public event EventHandler HandleDestroyed {
			add { Events.AddHandler (HandleDestroyedEvent, value); }
			remove { Events.RemoveHandler (HandleDestroyedEvent, value); }
		}

		public event HelpEventHandler HelpRequested {
			add { Events.AddHandler (HelpRequestedEvent, value); }
			remove { Events.RemoveHandler (HelpRequestedEvent, value); }
		}

		public event EventHandler ImeModeChanged {
			add { Events.AddHandler (ImeModeChangedEvent, value); }
			remove { Events.RemoveHandler (ImeModeChangedEvent, value); }
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		public event InvalidateEventHandler Invalidated {
			add { Events.AddHandler (InvalidatedEvent, value); }
			remove { Events.RemoveHandler (InvalidatedEvent, value); }
		}

		public event KeyEventHandler KeyDown {
			add { Events.AddHandler (KeyDownEvent, value); }
			remove { Events.RemoveHandler (KeyDownEvent, value); }
		}

		public event KeyPressEventHandler KeyPress {
			add { Events.AddHandler (KeyPressEvent, value); }
			remove { Events.RemoveHandler (KeyPressEvent, value); }
		}

		public event KeyEventHandler KeyUp {
			add { Events.AddHandler (KeyUpEvent, value); }
			remove { Events.RemoveHandler (KeyUpEvent, value); }
		}

		public event LayoutEventHandler Layout {
			add { Events.AddHandler (LayoutEvent, value); }
			remove { Events.RemoveHandler (LayoutEvent, value); }
		}

		public event EventHandler Leave {
			add { Events.AddHandler (LeaveEvent, value); }
			remove { Events.RemoveHandler (LeaveEvent, value); }
		}

		public event EventHandler LocationChanged {
			add { Events.AddHandler (LocationChangedEvent, value); }
			remove { Events.RemoveHandler (LocationChangedEvent, value); }
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		public event EventHandler LostFocus {
			add { Events.AddHandler (LostFocusEvent, value); }
			remove { Events.RemoveHandler (LostFocusEvent, value); }
		}

		public event EventHandler MarginChanged {
			add { Events.AddHandler (MarginChangedEvent, value); }
			remove {Events.RemoveHandler (MarginChangedEvent, value); }
		}
		public event EventHandler MouseCaptureChanged {
			add { Events.AddHandler (MouseCaptureChangedEvent, value); }
			remove { Events.RemoveHandler (MouseCaptureChangedEvent, value); }
		}

		public event MouseEventHandler MouseClick
		{
			add { Events.AddHandler (MouseClickEvent, value); }
			remove { Events.RemoveHandler (MouseClickEvent, value); }
		}
		public event MouseEventHandler MouseDoubleClick
		{
			add { Events.AddHandler (MouseDoubleClickEvent, value); }
			remove { Events.RemoveHandler (MouseDoubleClickEvent, value); }
		}

		public event MouseEventHandler MouseDown {
			add { Events.AddHandler (MouseDownEvent, value); }
			remove { Events.RemoveHandler (MouseDownEvent, value); }
		}

		public event EventHandler MouseEnter {
			add { Events.AddHandler (MouseEnterEvent, value); }
			remove { Events.RemoveHandler (MouseEnterEvent, value); }
		}

		public event EventHandler MouseHover {
			add { Events.AddHandler (MouseHoverEvent, value); }
			remove { Events.RemoveHandler (MouseHoverEvent, value); }
		}

		public event EventHandler MouseLeave {
			add { Events.AddHandler (MouseLeaveEvent, value); }
			remove { Events.RemoveHandler (MouseLeaveEvent, value); }
		}

		public event MouseEventHandler MouseMove {
			add { Events.AddHandler (MouseMoveEvent, value); }
			remove { Events.RemoveHandler (MouseMoveEvent, value); }
		}

		public event MouseEventHandler MouseUp {
			add { Events.AddHandler (MouseUpEvent, value); }
			remove { Events.RemoveHandler (MouseUpEvent, value); }
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		public event MouseEventHandler MouseWheel {
			add { Events.AddHandler (MouseWheelEvent, value); }
			remove { Events.RemoveHandler (MouseWheelEvent, value); }
		}

		public event EventHandler Move {
			add { Events.AddHandler (MoveEvent, value); }
			remove { Events.RemoveHandler (MoveEvent, value); }
		}

		public event EventHandler PaddingChanged
		{
			add { Events.AddHandler (PaddingChangedEvent, value); }
			remove { Events.RemoveHandler (PaddingChangedEvent, value); }
		}

		public event PaintEventHandler Paint {
			add { Events.AddHandler (PaintEvent, value); }
			remove { Events.RemoveHandler (PaintEvent, value); }
		}

		public event EventHandler ParentChanged {
			add { Events.AddHandler (ParentChangedEvent, value); }
			remove { Events.RemoveHandler (ParentChangedEvent, value); }
		}

		public event PreviewKeyDownEventHandler PreviewKeyDown {
			add { Events.AddHandler (PreviewKeyDownEvent, value); }
			remove { Events.RemoveHandler (PreviewKeyDownEvent, value); }
		}

		public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp {
			add { Events.AddHandler (QueryAccessibilityHelpEvent, value); }
			remove { Events.RemoveHandler (QueryAccessibilityHelpEvent, value); }
		}

		public event QueryContinueDragEventHandler QueryContinueDrag {
			add { Events.AddHandler (QueryContinueDragEvent, value); }
			remove { Events.RemoveHandler (QueryContinueDragEvent, value); }
		}

		public event EventHandler RegionChanged {
			add { Events.AddHandler (RegionChangedEvent, value); }
			remove { Events.RemoveHandler (RegionChangedEvent, value); }
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public event EventHandler Resize {
			add { Events.AddHandler (ResizeEvent, value); }
			remove { Events.RemoveHandler (ResizeEvent, value); }
		}

		public event EventHandler RightToLeftChanged {
			add { Events.AddHandler (RightToLeftChangedEvent, value); }
			remove { Events.RemoveHandler (RightToLeftChangedEvent, value); }
		}

		public event EventHandler SizeChanged {
			add { Events.AddHandler (SizeChangedEvent, value); }
			remove { Events.RemoveHandler (SizeChangedEvent, value); }
		}

		public event EventHandler StyleChanged {
			add { Events.AddHandler (StyleChangedEvent, value); }
			remove { Events.RemoveHandler (StyleChangedEvent, value); }
		}

		public event EventHandler SystemColorsChanged {
			add { Events.AddHandler (SystemColorsChangedEvent, value); }
			remove { Events.RemoveHandler (SystemColorsChangedEvent, value); }
		}

		public event EventHandler TabIndexChanged {
			add { Events.AddHandler (TabIndexChangedEvent, value); }
			remove { Events.RemoveHandler (TabIndexChangedEvent, value); }
		}

		public event EventHandler TabStopChanged {
			add { Events.AddHandler (TabStopChangedEvent, value); }
			remove { Events.RemoveHandler (TabStopChangedEvent, value); }
		}

		public event EventHandler TextChanged {
			add { Events.AddHandler (TextChangedEvent, value); }
			remove { Events.RemoveHandler (TextChangedEvent, value); }
		}

		public event EventHandler Validated {
			add { Events.AddHandler (ValidatedEvent, value); }
			remove { Events.RemoveHandler (ValidatedEvent, value); }
		}

		public event CancelEventHandler Validating {
			add { Events.AddHandler (ValidatingEvent, value); }
			remove { Events.RemoveHandler (ValidatingEvent, value); }
		}

		public event EventHandler VisibleChanged {
			add { Events.AddHandler (VisibleChangedEvent, value); }
			remove { Events.RemoveHandler (VisibleChangedEvent, value); }
		}

		#endregion	// Events
		
		
		
		#region Public Classes


		[ComVisible (false)]
		public class ControlCollection : Layout.ArrangedElementCollection, IList, ICollection, ICloneable, IEnumerable {

			#region ControlCollection Local Variables
			ArrayList impl_list;
			Control [] all_controls;
			Control owner;
			#endregion // ControlCollection Local Variables

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
			
			new	public virtual Control this[int index] {
				get {
					if (index < 0 || index >= list.Count) {
						throw new ArgumentOutOfRangeException("index", index, "ControlCollection does not have that many controls");
					}
					return (Control)list[index];
				}
				
				
			}

			#endregion // ControlCollection Public Instance Properties
			
			#region ControlCollection Instance Methods

			public virtual void Add (Control value)
			{
				if (value == null)
					return;

				Form form_value = value as Form;
				Form form_owner = owner as Form;
				bool owner_permits_toplevels = (owner is MdiClient) || (form_owner != null && form_owner.IsMdiContainer);
				bool child_is_toplevel = value.GetTopLevel();
				bool child_is_mdichild = form_value != null && form_value.IsMdiChild;

				if (child_is_toplevel && !(owner_permits_toplevels && child_is_mdichild))
					throw new ArgumentException("Cannot add a top level control to a control.", "value");
				
				if (child_is_mdichild && form_value.MdiParent != null && form_value.MdiParent != owner && form_value.MdiParent != owner.Parent) {
					throw new ArgumentException ("Form cannot be added to the Controls collection that has a valid MDI parent.", "value");
				}
				
				value.recalculate_distances = true;
				
				if (Contains (value)) {
					owner.PerformLayout();
					return;
				}

				if (value.tab_index == -1) {
					int	end;
					int	index;
					int	use;

					use = 0;
					end = owner.child_controls.Count;
					for (int i = 0; i < end; i++) {
						index = owner.child_controls[i].tab_index;
						if (index >= use) {
							use = index + 1;
						}
					}
					value.tab_index = use;
				}

				if (value.parent != null) {
					value.parent.Controls.Remove(value);
				}

				all_controls = null;
				list.Add (value);

				value.ChangeParent(owner);

				value.InitLayout();

				if (owner.Visible)
					owner.UpdateChildrenZOrder();
				owner.PerformLayout(value, "Parent");
				owner.OnControlAdded(new ControlEventArgs(value));
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

				if (AllContains (control)) {
					owner.PerformLayout ();
					return;
				}

				if (control.parent != null) {
					control.parent.Controls.Remove(control);
				}

				all_controls = null;
				impl_list.Add (control);

				control.ChangeParent (owner);
				control.InitLayout ();
				if (owner.Visible)
					owner.UpdateChildrenZOrder ();
				
				// If we are adding a new control that isn't
				// visible, don't trigger a layout
				if (control.VisibleInternal)
					owner.PerformLayout (control, "Parent");
			}
			
			[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
			public virtual void AddRange (Control[] controls)
			{
				if (controls == null)
					throw new ArgumentNullException ("controls");

				owner.SuspendLayout ();

				try {
					for (int i = 0; i < controls.Length; i++) 
						Add (controls[i]);
				} finally {
					owner.ResumeLayout ();
				}
			}

			internal virtual void AddRangeImplicit (Control [] controls)
			{
				if (controls == null)
					throw new ArgumentNullException ("controls");

				owner.SuspendLayout ();

				try {
					for (int i = 0; i < controls.Length; i++)
						AddImplicit (controls [i]);
				} finally {
					owner.ResumeLayout (false);
				}
			}

			public new virtual void Clear ()
			{
				all_controls = null;

				// MS sends remove events in reverse order
				while (list.Count > 0) {
					Remove((Control)list[list.Count - 1]);
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

			internal bool ImplicitContains (Control value) {
				if (impl_list == null)
					return false;

				return impl_list.Contains (value);
			}

			internal bool AllContains (Control value) {
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
				
				foreach (Control c in list) {
					if (c.Name.Equals (key, StringComparison.CurrentCultureIgnoreCase))
						al.Add (c);
						
					if (searchAllChildren)
						al.AddRange (c.Controls.Find (key, true));
				}
				
				return (Control[])al.ToArray (typeof (Control));
			}
			public int GetChildIndex(Control child) {
				return GetChildIndex(child, false);
			}

			public virtual int GetChildIndex(Control child, bool throwException) {
				int index;

				index=list.IndexOf(child);

				if (index==-1 && throwException) {
					throw new ArgumentException("Not a child control", "child");
				}
				return index;
			}

			public override IEnumerator
			GetEnumerator () {
				return new ControlCollectionEnumerator (list);
			}

			internal IEnumerator GetAllEnumerator () {
				Control [] res = GetAllControls ();
				return res.GetEnumerator ();
			}

			internal ArrayList ImplicitControls {
				get { return impl_list; }
			}
			
			internal Control [] GetAllControls () {
				if (all_controls != null)
					return all_controls;

				if (impl_list == null) {
					all_controls = (Control []) list.ToArray (typeof (Control));
					return all_controls;
				}
				
				all_controls = new Control [list.Count + impl_list.Count];
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
				list.Remove(value);

				owner.PerformLayout(value, "Parent");
				owner.OnControlRemoved(new ControlEventArgs(value));

				ContainerControl container = owner.InternalGetContainerControl ();
				if (container != null) { 
					// Inform any container controls about the loss of a child control
					// so that they can update their active control
					container.ChildControlRemoved (value);
				}

				value.ChangeParent(null);

				owner.UpdateChildrenZOrder();
			}

			internal virtual void RemoveImplicit (Control control)
			{
				if (impl_list != null) {
					all_controls = null;
					impl_list.Remove (control);
					owner.PerformLayout (control, "Parent");
					owner.OnControlRemoved (new ControlEventArgs (control));
				}
				control.ChangeParent (null);
				owner.UpdateChildrenZOrder ();
			}

			public void RemoveAt (int index)
			{
				if (index < 0 || index >= list.Count)
					throw new ArgumentOutOfRangeException("index", index, "ControlCollection does not have that many controls");

				Remove ((Control) list [index]);
			}

			public virtual void RemoveByKey (string key)
			{
				int index = IndexOfKey (key);
				
				if (index >= 0)
					RemoveAt (index);
			}
			
			public virtual void SetChildIndex(Control child, int newIndex)
			{
				if (child == null)
					throw new ArgumentNullException ("child");

				int	old_index;

				old_index=list.IndexOf(child);
				if (old_index==-1) {
					throw new ArgumentException("Not a child control", "child");
				}

				if (old_index==newIndex) {
					return;
				}

				all_controls = null;
				list.RemoveAt(old_index);

				if (newIndex>list.Count) {
					list.Add(child);
				} else {
					list.Insert(newIndex, child);
				}
				child.UpdateZOrder();
				owner.PerformLayout();
			}

			#endregion // ControlCollection Private Instance Methods


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
				clone.list = (ArrayList)list.Clone ();		// FIXME: Do we need this?
				return clone;
			}

			#endregion // ControlCollection Interface Methods
		
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
						try {
							return list[position];
						} catch (IndexOutOfRangeException) {
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
		#endregion	// ControlCollection Class

	}
}

