// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2004-2006 Novell, Inc.
//
// Authors:
//	Peter Bartok		pbartok@novell.com
//
// Partially based on work by:
//	Aleksey Ryabchuk	ryabchuk@yahoo.com
//	Alexandre Pigolkine	pigolkine@gmx.de
//	Dennis Hayes		dennish@raytek.com
//	Jaak Simm		jaaksimm@firm.ee
//	John Sohn		jsohn@columbus.rr.com
//

#undef DebugRecreate
#undef DebugFocus
#undef DebugMessages

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

namespace System.Windows.Forms
{
	[ComVisible(true)]
	[ClassInterface (ClassInterfaceType.AutoDispatch)]
	[Designer("System.Windows.Forms.Design.ControlDesigner, " + Consts.AssemblySystem_Design, "System.ComponentModel.Design.IDesigner")]
	[DefaultProperty("Text")]
	[DefaultEvent("Click")]
	[DesignerSerializer("System.Windows.Forms.Design.ControlCodeDomSerializer, " + Consts.AssemblySystem_Design, "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + Consts.AssemblySystem_Design)]
	[ToolboxItemFilter("System.Windows.Forms")]
	public partial class Control : Component, ISynchronizeInvoke, IWin32Window
		, IBindableComponent, IDropTarget, IBounds
	{
		#region Local Variables

		// Basic
		Rectangle               explicit_bounds; // explicitly set bounds
		string                  name; // for object naming

		// State
		bool                    is_created; // true if OnCreateControl has been sent
		bool                    is_accessible; // true if the control is visible to accessibility applications
		bool                    is_recreating; // tracks if the handle for the control is being recreated
		bool                    is_focusing; // tracks if Focus has been called on the control and has not yet finished
		int                     tab_index; // position in tab order of siblings
		bool                    is_disposed; // has the window already been disposed?
		bool                    is_disposing; // is the window getting disposed?
		Size                    client_size; // size of the client area (window excluding decorations)
		Rectangle               client_rect; // rectangle with the client area (window excluding decorations)
		ImeMode                 ime_mode;
		object                  control_tag; // object that contains data about our control
		internal int			mouse_clicks;		// Counter for mouse clicks
		internal bool			allow_drop;		// true if the control accepts droping objects on it   
		Region                  clip_region; // User-specified clip region for the window

		// Visuals
		internal Color			background_color;	// background color for control
		string                  text; // window/title text for control
		internal                BorderStyle		border_style;		// Border style of control
		bool                    show_keyboard_cues; // Current keyboard cues 
		internal bool           show_focus_cues; // Current focus cues 
		internal bool		force_double_buffer;	// Always doublebuffer regardless of ControlStyle

		// Layout
		internal enum LayoutType {
			Anchor,
			Dock
		}
		Layout.LayoutEngine layout_engine;
		internal AnchorStyles anchor_style; // anchoring requirements for our control
		internal DockStyle dock_style; // docking requirements for our control
		LayoutType layout_type;
		private bool recalculate_distances = true;  // Delay anchor calculations

		// Please leave the next 2 as internal until DefaultLayout (2.0) is rewritten
		internal int			dist_right; // distance to the right border of the parent
		internal int			dist_bottom; // distance to the bottom border of the parent

		// to be categorized...
		internal bool		use_compatible_text_rendering;
		private bool		use_wait_cursor;

		// double buffering

		static bool verify_thread_handle;
		Size maximum_size;
		Size minimum_size;
		Padding margin;
		private bool nested_layout = false;
		Point auto_scroll_offset;
		private bool suppressing_key_press;

		#endregion	// Local Variables
		
		#region Public Classes
		#endregion	// ControlCollection Class
		
		#region Public Constructors
		public Control (Control parent, string text) : this()
		{
			Text=text;
			Parent=parent;
		}

		public Control (Control parent, string text, int left, int top, int width, int height) : this()
		{
			Parent=parent;
			SetBounds(left, top, width, height, BoundsSpecified.All);
			Text=text;
		}

		public Control (string text) : this()
		{
			Text=text;
		}

		public Control (string text, int left, int top, int width, int height) : this()
		{
			SetBounds(left, top, width, height, BoundsSpecified.All);
			Text=text;
		}

		private delegate void RemoveDelegate(object c);
		#endregion 	// Public Constructors

		#region Internal Properties

		internal Rectangle PaddingClientRectangle
		{
			get {
				return new Rectangle (
					ClientRectangle.Left   + padding.Left,
					ClientRectangle.Top    + padding.Top, 
					ClientRectangle.Width  - padding.Horizontal, 
					ClientRectangle.Height - padding.Vertical);
			}
		}

		// Control is currently selected, like Focused, except maintains state
		// when Form loses focus
		internal bool InternalSelected {
		        get {
		        	IContainerControl container;
			
				container = GetContainerControl();
				
				if (container != null && container.ActiveControl == this)
					return true;
					
				return false;
			}
		}

		
		// Mouse is currently within the control's bounds
		internal bool Entered {
			get { return this.is_entered; }
		}

		internal LayoutType ControlLayoutType {
			get { return layout_type; }
		}
		
		
		internal Size InternalClientSize { set { this.client_size = value; } }
		internal virtual bool ActivateOnShow { get { return true; } }
		internal Rectangle ExplicitBounds { get { return this.explicit_bounds; } set { this.explicit_bounds = value; } }

		internal bool ValidationFailed { 
			get { 
				ContainerControl c = InternalGetContainerControl ();
				if (c != null)
					return c.validation_failed;
				return false;
			}
			set { 
				ContainerControl c = InternalGetContainerControl ();
				if (c != null)
					c.validation_failed = value; 
			}
		}
		#endregion	// Internal Properties

		#region Private & Internal Methods
		
		void IDropTarget.OnDragDrop (DragEventArgs drgEvent)
		{
			OnDragDrop (drgEvent);
		}
		
		void IDropTarget.OnDragEnter (DragEventArgs drgEvent)
		{
			OnDragEnter (drgEvent);
		}
		
		void IDropTarget.OnDragLeave (EventArgs e)
		{
			OnDragLeave (e);
		}

		void IDropTarget.OnDragOver (DragEventArgs drgEvent)
		{
			OnDragOver (drgEvent);
		}

		internal IAsyncResult BeginInvokeInternal (Delegate method, object [] args) {
			
			//TODO: fixme
			return null;
			//return BeginInvokeInternal (method, args, FindControlToInvokeOn ());
		}


		// The CheckForIllegalCrossThreadCalls in the #if 2.0 of
		// Control.Handle throws an exception when we are trying
		// to get the Handle to properly invoke on.  This avoids that.
		
		internal bool IsRecreating {
			get {
				return is_recreating;
			}
		}

		// An internal way to have a fixed height
		// Basically for DataTimePicker 2.0
		internal virtual int OverrideHeight (int height)
		{
			return height;
		}

		private Control FindControlToInvokeOn ()
		{
			Control p = this;
			do {
				if (p.IsHandleCreated)
					break;
				p = p.parent;
			} while (p != null);

			if (p == null || !p.IsHandleCreated)
				throw new InvalidOperationException ("Cannot call Invoke or BeginInvoke on a control until the window handle is created");
			
			return p;
		}

		internal static void SetChildColor(Control parent) {
			Control	child;

			for (int i=0; i < parent.child_controls.Count; i++) {
				child=parent.child_controls[i];
				if (child.child_controls.Count>0) {
					SetChildColor(child);
				}
			}
		}

		internal virtual void DoDefaultAction() {
			// Only here to be overriden by our actual controls; this is needed by the accessibility class
		}

		internal static IntPtr MakeParam (int low, int high){
			return new IntPtr (high << 16 | low & 0xffff);
		}

		internal static int LowOrder (int param) {
			return ((int)(short)(param & 0xffff));
		}

		internal static int HighOrder (long param) {
			return ((int)(short)(param >> 16));
		}

		// This method exists so controls overriding OnPaintBackground can have default background painting done
		internal virtual void DndEnter (DragEventArgs e) {
			try {
				OnDragEnter (e);
			} catch { }
		}

		internal virtual void DndOver (DragEventArgs e) {
			try {
				OnDragOver (e);
			} catch { }
		}

		internal virtual void DndDrop (DragEventArgs e) {
			try {
				OnDragDrop (e);
			} catch (Exception exc) {
				Console.Error.WriteLine ("MWF: Exception while dropping:");
				Console.Error.WriteLine (exc);
			}
		}

		internal virtual void DndLeave (EventArgs e) {
			try {
				OnDragLeave (e);
			} catch { }
		}

		internal virtual void DndFeedback(GiveFeedbackEventArgs e) {
			try {
				OnGiveFeedback(e);
			} catch { }
		}

		internal virtual void DndContinueDrag(QueryContinueDragEventArgs e) {
			try {
				OnQueryContinueDrag(e);
			} catch { }
		}

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

		internal virtual bool ProcessControlMnemonic(char charCode) {
			return ProcessMnemonic(charCode);
		}

		private static Control FindFlatForward(Control container, Control start) {
			Control	found;
			int	index;
			int	end;
			bool hit;

			found = null;
			end = container.child_controls.Count;
			hit = false;

			if (start != null) {
				index = start.tab_index;
			} else {
				index = -1;
			}

			for (int i = 0; i < end; i++) {
				if (start == container.child_controls[i]) {
					hit = true;
					continue;
				}

				if (found == null || found.tab_index > container.child_controls[i].tab_index) {
					if (container.child_controls[i].tab_index > index || (hit && container.child_controls[i].tab_index == index)) {
						found = container.child_controls[i];
					}
				}
			}
			return found;
		}

		private static Control FindControlForward(Control container, Control start) {
			Control found;

			found = null;

			if (start == null) {
				return FindFlatForward(container, start);
			}

			if (start.child_controls != null && start.child_controls.Count > 0 && 
				(start == container || !((start is IContainerControl) &&  start.GetStyle(ControlStyles.ContainerControl)))) {
				return FindControlForward(start, null);
			}
			else {
				while (start != container) {
					found = FindFlatForward(start.parent, start);
					if (found != null) {
						return found;
					}
					start = start.parent;
				}
			}
			return null;
		}

		private static Control FindFlatBackward(Control container, Control start) {
			Control	found;
			int	index;
			int	end;
			bool hit;

			found = null;
			end = container.child_controls.Count;
			hit = false;

			if (start != null) {
				index = start.tab_index;
			} else {
				index = int.MaxValue;
			}

			for (int i = end - 1; i >= 0; i--) {
				if (start == container.child_controls[i]) {
					hit = true;
					continue;
				}

				if (found == null || found.tab_index < container.child_controls[i].tab_index) {
					if (container.child_controls[i].tab_index < index || (hit && container.child_controls[i].tab_index == index))
						found = container.child_controls[i];

				}
			}
			return found;
		}

		private static Control FindControlBackward(Control container, Control start) {

			Control found = null;

			if (start == null) {
				found = FindFlatBackward(container, start);
			}
			else if (start != container) {
				if (start.parent != null) {
					found = FindFlatBackward(start.parent, start);

					if (found == null) {
						if (start.parent != container)
							return start.parent;
						return null;
					}
				}
			}
		
			if (found == null || start.parent == null)
				found = start;

			while (found != null && (found == container || (!((found is IContainerControl) && found.GetStyle(ControlStyles.ContainerControl))) &&
				found.child_controls != null && found.child_controls.Count > 0)) {
//				while (ctl.child_controls != null && ctl.child_controls.Count > 0 && 
//					(ctl == this || (!((ctl is IContainerControl) && ctl.GetStyle(ControlStyles.ContainerControl))))) {
				found = FindFlatBackward(found, null);
			}

			return found;

/*
			Control found;

			found = null;

			if (start != null) {
				found = FindFlatBackward(start.parent, start);
				if (found == null) {
					if (start.parent != container) {
						return start.parent;
					}
				}
			}
			if (found == null) {
				found = FindFlatBackward(container, start);
			}

			if (container != start) {
				while ((found != null) && (!found.Contains(start)) && found.child_controls != null && found.child_controls.Count > 0 && !(found is IContainerControl)) {// || found.GetStyle(ControlStyles.ContainerControl))) {
					found = FindControlBackward(found, null);
					 if (found != null) {
						return found;
					}
				}
			}
			return found;
*/			
		}

		internal virtual void HandleClick(int clicks, MouseEventArgs me) {
			bool standardclick = GetStyle (ControlStyles.StandardClick);
			bool standardclickclick = GetStyle (ControlStyles.StandardDoubleClick);
			if ((clicks > 1) && standardclick && standardclickclick) {
				OnDoubleClick (me);
				OnMouseDoubleClick (me);
			} else if (clicks == 1 && standardclick && !ValidationFailed) {
				OnClick (me);
				OnMouseClick (me);
			}
		}

		private void CheckDataBindings () {
			if (data_bindings == null)
				return;

			foreach (Binding binding in data_bindings) {
				binding.Check ();
			}
		}


		internal virtual Size GetPreferredSizeCore (Size proposedSize)
		{
			return this.explicit_bounds.Size;
		}

		private void UpdateDistances() {
			if (parent != null) {
				if (bounds.Width >= 0)
					dist_right = parent.ClientSize.Width - bounds.X - bounds.Width;
				if (bounds.Height >= 0)
					dist_bottom = parent.ClientSize.Height - bounds.Y - bounds.Height;

				recalculate_distances = false;
			}
		}
		#endregion	// Private & Internal Methods

		#region Public Static Properties
		
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		[Browsable (false)]
		[MonoTODO ("Stub, value is not used")]
		public static bool CheckForIllegalCrossThreadCalls 
		{
			get {
				return verify_thread_handle;
			}

			set {
				verify_thread_handle = value;
			}
		}
		#endregion	// Public Static Properties

		#region Public Instance Properties

		[Localizable(true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(AnchorStyles.Top | AnchorStyles.Left)]
		[MWFCategory("Layout")]
		public virtual AnchorStyles Anchor {
			get {
				return anchor_style;
			}

			set {
				layout_type = LayoutType.Anchor;

				if (anchor_style == value)
					return;
					
				anchor_style=value;
				dock_style = DockStyle.None;
				
				UpdateDistances ();

				if (parent != null)
					parent.PerformLayout(this, "Anchor");
			}
		}

		[Browsable (false)]
		[DefaultValue (typeof (Point), "0, 0")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public virtual Point AutoScrollOffset {
			get {
				return auto_scroll_offset;
			}

			set {
				this.auto_scroll_offset = value;
			}
		}
			
		// XXX: Implement me!
		bool auto_size;
		
		[RefreshProperties (RefreshProperties.All)]
		[Localizable (true)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[DefaultValue (false)]
		public virtual bool AutoSize {
			get { return auto_size; }
			set {
				if (this.auto_size != value) {
					auto_size = value;
					
					// If we're turning this off, reset our size
					if (!value) {
						Size = explicit_bounds.Size;
					} else {
						if (Parent != null)
							Parent.PerformLayout (this, "AutoSize");
					}

					OnAutoSizeChanged (EventArgs.Empty);
				}
			}
		}
		
		[AmbientValue ("{Width=0, Height=0}")]
		[MWFCategory("Layout")]
		public virtual Size MaximumSize {
			get {
				return maximum_size;
			}
			set {
				if (maximum_size != value) {
					maximum_size = value;
					Size = PreferredSize;
				}
			}
		}

		internal bool ShouldSerializeMaximumSize ()
		{
			return this.MaximumSize != DefaultMaximumSize;
		}

		[MWFCategory("Layout")]
		public virtual Size MinimumSize {
			get {
				return minimum_size;
			}
			set {
				if (minimum_size != value) {
					minimum_size = value;
					Size = PreferredSize;
				}
			}
		}

		internal bool ShouldSerializeMinimumSize ()
		{
			return this.MinimumSize != DefaultMinimumSize;
		}

		[DispId(-501)]
		[MWFCategory("Appearance")]
		public virtual Color BackColor {
			get {
				if (background_color.IsEmpty) {
					if (parent!=null) {
						Color pcolor = parent.BackColor;
						if (pcolor.A == 0xff || GetStyle(ControlStyles.SupportsTransparentBackColor))
							return pcolor;
					}
					return DefaultBackColor;
				}
				return background_color;
			}

			set {
				if (!value.IsEmpty && (value.A != 0xff) && !GetStyle(ControlStyles.SupportsTransparentBackColor)) {
					throw new ArgumentException("Transparent background colors are not supported on this control");
				}

				if (background_color != value) {
					background_color=value;
					SetChildColor(this);
					OnBackColorChanged(EventArgs.Empty);
					Invalidate();
				}
			}
		}

		internal bool ShouldSerializeBackColor ()
		{
			return this.BackColor != DefaultBackColor;
		}

		[Localizable(true)]
		[DefaultValue(null)]
		[MWFCategory("Appearance")]
		public virtual Image BackgroundImage {
			get {
				return background_image;
			}

			set {
				if (background_image!=value) {
					background_image=value;
					OnBackgroundImageChanged(EventArgs.Empty);
					Invalidate ();
				}
			}
		}

		[DefaultValue (ImageLayout.Tile)]
		[Localizable (true)]
		[MWFCategory("Appearance")]
		public virtual ImageLayout BackgroundImageLayout {
			get {
				return backgroundimage_layout;
			}
			set {
				if (Array.IndexOf (Enum.GetValues (typeof (ImageLayout)), value) == -1)
					throw new InvalidEnumArgumentException ("value", (int) value, typeof(ImageLayout));

				if (value != backgroundimage_layout) {
					backgroundimage_layout = value;
					Invalidate ();
					OnBackgroundImageLayoutChanged (EventArgs.Empty);
				}
				
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

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Bottom {
			get {
				return this.bounds.Bottom;
			}
		}
		
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanFocus {
			get {
				if (IsHandleCreated && Visible && Enabled) {
					return true;
				}
				return false;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanSelect {
			get {
				Control	parent;

				if (!GetStyle(ControlStyles.Selectable)) {
					return false;
				}

				parent = this;
				while (parent != null) {
					if (!parent.is_visible || !parent.is_enabled) {
						return false;
					}

					parent = parent.parent;
				}
				return true;
			}
		}

		internal virtual bool InternalCapture {
			get {
				return Capture;
			}

			set {
				Capture = value;
			}
		}

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
		[DescriptionAttribute("ControlCompanyNameDescr")]
		public String CompanyName {
			get {
				return "Mono Project, Novell, Inc.";
			}
		}
		
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Created {
			get {
				return (!is_disposed && is_created);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[ParenthesizePropertyName(true)]
		[RefreshProperties(RefreshProperties.All)]
		[MWFCategory("Data")]
		public ControlBindingsCollection DataBindings {
			get {
				if (data_bindings == null)
					data_bindings = new ControlBindingsCollection (this);
				return data_bindings;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Rectangle DisplayRectangle {
			get {
				return ClientRectangle;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Disposing {
			get {
				return is_disposed;
			}
		}

		[Localizable(true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(DockStyle.None)]
		[MWFCategory("Layout")]
		public virtual DockStyle Dock {
			get {
				return dock_style;
			}

			set {
				// If the user sets this to None, we need to still use Anchor layout
				if (value != DockStyle.None)
					layout_type = LayoutType.Dock;

				if (dock_style == value) {
					return;
				}

				if (!Enum.IsDefined (typeof (DockStyle), value)) {
					throw new InvalidEnumArgumentException ("value", (int) value,
						typeof (DockStyle));
				}

				dock_style = value;
				anchor_style = AnchorStyles.Top | AnchorStyles.Left;

				if (dock_style == DockStyle.None) {
					bounds = explicit_bounds;
					layout_type = LayoutType.Anchor;
				}

				if (parent != null)
					parent.PerformLayout(this, "Dock");
				else if (Controls.Count > 0)
					PerformLayout ();

				OnDockChanged(EventArgs.Empty);
			}
		}

		protected virtual bool DoubleBuffered {
			get {
				return (control_style & ControlStyles.OptimizedDoubleBuffer) != 0;
			}

			set {
				if (value == DoubleBuffered)
					return;
				if (value) {
					SetStyle (ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
				} else {
					SetStyle (ControlStyles.OptimizedDoubleBuffer, false);
				}
				
			}
		}
		
		public void DrawToBitmap (Bitmap bitmap, Rectangle targetBounds)
		{
			Graphics g = Graphics.FromImage (bitmap);
			
			// Only draw within the target bouds, and up to the size of the control
			g.IntersectClip (targetBounds);
			g.IntersectClip (Bounds);
			
			// Logic copied from WmPaint
			PaintEventArgs pea = new PaintEventArgs (g, targetBounds);
			
			if (!GetStyle (ControlStyles.Opaque))
				OnPaintBackground (pea);

			OnPaintBackgroundInternal (pea);

			OnPaintInternal (pea);

			//if (!pea.Handled)
				OnPaint (pea);
			
			g.Dispose ();
		}

		internal bool ShouldSerializeEnabled ()
		{
			return this.Enabled != true;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool Focused {
			get {
				return this.has_focus;
			}
		}

		[DispId(-512)]
		[AmbientValue(null)]
		[Localizable(true)]
		[MWFCategory("Appearance")]
		public virtual Font Font {
			[return: MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof (Font))]
			get {
				if (font != null)
					return font;

				if (parent != null) {
					Font f = parent.Font;
					
					if (f != null)
						return f;
				}

				return DefaultFont;
			}

			[param:MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(Font))]
			set {
				if (font != null && font == value) {
					return;
				}

				font = value;
				Invalidate();
				OnFontChanged (EventArgs.Empty);
				PerformLayout ();
			}
		}

		internal bool ShouldSerializeFont ()
		{
			return !this.Font.Equals (DefaultFont);
		}

		[DispId(-513)]
		[MWFCategory("Appearance")]
		public virtual Color ForeColor {
			get {
				if (foreground_color.IsEmpty) {
					if (parent!=null) {
						return parent.ForeColor;
					}
					return DefaultForeColor;
				}
				return foreground_color;
			}

			set {
				if (foreground_color != value) {
					foreground_color=value;
					Invalidate();
					OnForeColorChanged(EventArgs.Empty);
				}
			}
		}

		internal bool ShouldSerializeForeColor ()
		{
			return this.ForeColor != DefaultForeColor;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HasChildren {
			get {
				if (this.child_controls.Count>0) {
					return true;
				}
				return false;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Height {
			get { return this.bounds.Height; }
			set { SetBounds(bounds.X, bounds.Y, bounds.Width, value, BoundsSpecified.Height); }
		}
		
		[AmbientValue(ImeMode.Inherit)]
		[Localizable(true)]
		[MWFCategory("Behavior")]
		public ImeMode ImeMode {
			get {
				if (ime_mode == ImeMode.Inherit) {
					if (parent != null)
						return parent.ImeMode;
					else
						return ImeMode.NoControl; // default value
				}
				return ime_mode;
			}

			set {
				if (ime_mode != value) {
					ime_mode = value;

					OnImeModeChanged(EventArgs.Empty);
				}
			}
		}

		internal bool ShouldSerializeImeMode ()
		{
			return this.ImeMode != ImeMode.NoControl;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool InvokeRequired {						// ISynchronizeInvoke
			get {
				if (creator_thread != null && creator_thread!=Thread.CurrentThread) {
					return true;
				}
				return false;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsAccessible {
			get {
				return is_accessible;
			}

			set {
				is_accessible = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDisposed {
			get {
				return this.is_disposed;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[MonoNotSupported ("RTL is not supported")]
		public bool IsMirrored {
			get { return false; }
		}

		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Left {
			get {
				return this.bounds.Left;
			}

			set {
				SetBounds(value, bounds.Y, bounds.Width, bounds.Height, BoundsSpecified.X);
			}
		}

		[Localizable(true)]
		[MWFCategory("Layout")]
		public Point Location {
			get {
				return this.bounds.Location;
			}

			set {
				SetBounds(value.X, value.Y, bounds.Width, bounds.Height, BoundsSpecified.Location);
			}
		}

		internal bool ShouldSerializeLocation ()
		{
			return this.Location != new Point (0, 0);
		}

		[Localizable (true)]
		[MWFCategory("Layout")]
		public Padding Margin {
			get { return this.margin; }
			set { 
				if (this.margin != value) {
					this.margin = value; 
					if (Parent != null)
						Parent.PerformLayout (this, "Margin");
					OnMarginChanged (EventArgs.Empty);
				}
			}
		}

		internal bool ShouldSerializeMargin ()
		{
			return this.Margin != DefaultMargin;
		}

		[Browsable(false)]
		public string Name {
			get {
				return name;
			}

			set {
				name = value;
			}
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Control Parent {
			get {
				return this.parent;
			}

			set {
				if (value == this) {
					throw new ArgumentException("A circular control reference has been made. A control cannot be owned or parented to itself.");
				}

				if (parent!=value) {
					if (value==null) {
						parent.Controls.Remove(this);
						parent = null;
						return;
					}

					value.Controls.Add(this);
				}
			}
		}

		[Browsable (false)]
		public Size PreferredSize {
			get { return this.GetPreferredSize (Size.Empty); }
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ProductName {
			get {
				Type t = typeof (AssemblyProductAttribute);
				Assembly assembly = GetType().Module.Assembly;
				object [] attrs = assembly.GetCustomAttributes (t, false);
				AssemblyProductAttribute a = null;
				// On MS we get a NullRefException if product attribute is not
				// set. 
				if (attrs != null && attrs.Length > 0)
					a = (AssemblyProductAttribute) attrs [0];
				if (a == null) {
					return GetType ().Namespace;
				}
				return a.Product;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ProductVersion {
			get {
				Type t = typeof (AssemblyVersionAttribute);
				Assembly assembly = GetType().Module.Assembly;
				object [] attrs = assembly.GetCustomAttributes (t, false);
				if (attrs == null || attrs.Length < 1)
					return "1.0.0.0";
				return ((AssemblyVersionAttribute)attrs [0]).Version;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RecreatingHandle {
			get {
				return is_recreating;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Right {
			get {
				return this.bounds.Right;
			}
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

		internal bool ShouldSerializeRightToLeft ()
		{
			return this.RightToLeft != RightToLeft.No;
		}

		internal bool ShouldSerializeSite ()
		{
			return false;
		}

		[Localizable(true)]
		[MWFCategory("Layout")]
		public Size Size {
			get {
				return new Size(Width, Height);
			}

			set {
				SetBounds(bounds.X, bounds.Y, value.Width, value.Height, BoundsSpecified.Size);
			}
		}

		internal virtual bool ShouldSerializeSize ()
		{
			return this.Size != DefaultSize;
		}

		[Localizable(true)]
		[MergableProperty(false)]
		[MWFCategory("Behavior")]
		public int TabIndex {
			get {
				if (tab_index != -1) {
					return tab_index;
				}
				return 0;
			}

			set {
				if (tab_index != value) {
					tab_index = value;
					OnTabIndexChanged(EventArgs.Empty);
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

		[Localizable(false)]
		[Bindable(true)]
		[TypeConverter(typeof(StringConverter))]
		[DefaultValue(null)]
		[MWFCategory("Data")]
		public object Tag {
			get {
				return control_tag;
			}

			set {
				control_tag = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Top {
			get {
				return this.bounds.Top;
			}

			set {
				SetBounds(bounds.X, value, bounds.Width, bounds.Height, BoundsSpecified.Y);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Control TopLevelControl {
			get {
				Control	p = this;

				while (p.parent != null) {
					p = p.parent;
				}

				return p is Form ? p : null;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(true)]
		[DefaultValue (false)]
		[MWFCategory("Appearance")]
		public bool UseWaitCursor {
			get { return use_wait_cursor; }
			set {
				if (use_wait_cursor != value) {
					use_wait_cursor = value;
					UpdateCursor ();
					OnCursorChanged (EventArgs.Empty);
				}
			}
		}


		internal bool ShouldSerializeVisible ()
		{
			return this.Visible != true;
		}

		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Width {
			get {
				return this.bounds.Width;
			}

			set {
				SetBounds(bounds.X, bounds.Y, value, bounds.Height, BoundsSpecified.Width);
			}
		}
		#endregion	// Public Instance Properties

		#region	Protected Instance Properties
		protected virtual bool CanEnableIme {
			get { return false; }
		}
		
		// Is only false in some ActiveX contexts
		protected override bool CanRaiseEvents {
			get { return true; }
		}
		
		protected virtual ImeMode DefaultImeMode {
			get {
				return ImeMode.Inherit;
			}
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

		protected int FontHeight {
			get {
				return Font.Height;
			}

			set {
				;; // Nothing to do
			}
		}
		[Obsolete ()]
		protected bool RenderRightToLeft {
			get {
				return (this.right_to_left == RightToLeft.Yes);
			}
		}

		protected bool ResizeRedraw {
			get {
				return GetStyle(ControlStyles.ResizeRedraw);
			}

			set {
				SetStyle(ControlStyles.ResizeRedraw, value);
			}
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual bool ScaleChildren {
			get { return ScaleChildrenInternal; }
		}

		internal virtual bool ScaleChildrenInternal {
			get { return true; }
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual bool ShowFocusCues {
			get {
				if (this is Form)
					return show_focus_cues;
					
				if (this.parent == null)
					return false;
					
				Form f = this.FindForm ();
				
				if (f != null)
					return f.show_focus_cues;
					
				return false;
			}
		}

		#endregion	// Protected Instance Properties

		#region Public Static Methods

		public static bool IsMnemonic(char charCode, string text) {
			int amp;

			amp = text.IndexOf('&');

			if (amp != -1) {
				if (amp + 1 < text.Length) {
					if (text[amp + 1] != '&') {
						if (Char.ToUpper(charCode) == Char.ToUpper(text.ToCharArray(amp + 1, 1)[0])) {
							return true;
						}	
					}
				}
			}
			return false;
		}
		#endregion


		#region	Public Instance Methods
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginInvoke(Delegate method) {
			object [] prms = null;
			if (method is EventHandler)
				prms = new object [] { this, EventArgs.Empty };
			return BeginInvokeInternal(method, prms);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IAsyncResult BeginInvoke (Delegate method, params object[] args)
		{
			return BeginInvokeInternal (method, args);
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

		internal virtual void OnDragDropEnd (DragDropEffects effects)
		{
		}


		internal Control FindRootParent ()
		{
			Control	c = this;
			
			while (c.Parent != null)
				c = c.Parent;

			return c;
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
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public bool Focus() {
			return FocusInternal (false);
		}

		internal virtual bool FocusInternal (bool skip_check) {
			if (skip_check || (CanFocus && IsHandleCreated && !has_focus && !is_focusing)) {
				is_focusing = true;
				Select(this);
				is_focusing = false;
			}
			return has_focus;
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

		internal ContainerControl InternalGetContainerControl() {
			Control	current = this;

			while (current!=null) {
				if ((current is ContainerControl) && ((current.control_style & ControlStyles.ContainerControl)!=0)) {
					return current as ContainerControl;
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
		public virtual Size GetPreferredSize (Size proposedSize) {
			Size retsize = GetPreferredSizeCore (proposedSize);
			
			// If we're bigger than the MaximumSize, fix that
			if (this.maximum_size.Width != 0 && retsize.Width > this.maximum_size.Width)
				retsize.Width = this.maximum_size.Width;
			if (this.maximum_size.Height != 0 && retsize.Height > this.maximum_size.Height)
				retsize.Height = this.maximum_size.Height;
				
			// If we're smaller than the MinimumSize, fix that
			if (this.minimum_size.Width != 0 && retsize.Width < this.minimum_size.Width)
				retsize.Width = this.minimum_size.Width;
			if (this.minimum_size.Height != 0 && retsize.Height < this.minimum_size.Height)
				retsize.Height = this.minimum_size.Height;
				
			return retsize;
		}

		public void Hide() {
			this.Visible = false;
		}

		public void Invalidate ()
		{
			Invalidate (ClientRectangle, false);
		}

		public void Invalidate (bool invalidateChildren)
		{
			Invalidate (ClientRectangle, invalidateChildren);
		}

		public void Invalidate (Rectangle rc)
		{
			Invalidate (rc, false);
		}

		public void Invalidate (Region region)
		{
			Invalidate (region, false);
		}

		public void Invalidate (Region region, bool invalidateChildren)
		{
			//using (Graphics g = CreateGraphics ()){
			//	RectangleF bounds = region.GetBounds (g);
			//	Invalidate (new Rectangle ((int) bounds.X, (int) bounds.Y, (int) bounds.Width, (int) bounds.Height), invalidateChildren);
			//}
		}

		public object Invoke (Delegate method) {
			object [] prms = null;
			if (method is EventHandler)
				prms = new object [] { this, EventArgs.Empty };

			return Invoke(method, prms);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void PerformLayout() {
			PerformLayout(null, null);
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
		
		public Rectangle RectangleToClient(Rectangle r) {
			return new Rectangle(PointToClient(r.Location), r.Size);
		}

		public Rectangle RectangleToScreen(Rectangle r) {
			return new Rectangle(PointToScreen(r.Location), r.Size);
		}

		public virtual void Refresh() {
			if (IsHandleCreated && Visible) {
				Invalidate(true);
				Update ();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ResetBackColor() {
			BackColor = Color.Empty;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetBindings() {
			if (data_bindings != null)
				data_bindings.Clear();
		}
		
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ResetFont() {
			font = null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ResetForeColor() {
			foreground_color = Color.Empty;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetImeMode() {
			ime_mode = DefaultImeMode;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ResetRightToLeft() {
			right_to_left = RightToLeft.Inherit;
		}

		public virtual void ResetText() {
			Text = String.Empty;
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
					PerformLayout();
				}
			}
		}
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ()]
		public void Scale(float ratio) {
			ScaleCore(ratio, ratio);
		}
		
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ()]
		public void Scale(float dx, float dy) {
			ScaleCore(dx, dy);
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

		internal ContainerControl FindContainer (Control c)
		{
			while ((c != null) && !(c is ContainerControl))
				c = c.Parent;
			return c as ContainerControl;
		}

		private bool IsContainerAutoScaling (Control c)
		{
			ContainerControl cc = FindContainer (c);
			return (cc != null) && cc.IsAutoScaling;
		}

		public void Select() {
			Select(false, false);	
		}

#if DebugFocus
		private void printTree(Control c, string t) {
			foreach(Control i in c.child_controls) {
				Console.WriteLine ("{2}{0}.TabIndex={1}", i, i.tab_index, t);
				printTree (i, t+"\t");
			}
		}
#endif
		public bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap) {
			Control c;

#if DebugFocus
			Console.WriteLine("{0}", this.FindForm());
			printTree(this, "\t");
#endif

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

#if DebugFocus
				Console.WriteLine("{0} {1}", c, c.CanSelect);
#endif
				if (c.CanSelect && ((c.parent == this) || nested) && (c.tab_stop || !tabStopOnly)) {
					c.Select (true, true);
					return true;
				}
			} while (c != ctl); // If we wrap back to ourselves we stop

			return false;
		}

		public void SendToBack() {
			if (parent != null) {
				parent.child_controls.SetChildIndex(this, parent.child_controls.Count);
			}
		}

		public void SetBounds(int x, int y, int width, int height) {
			SetBounds(x, y, width, height, BoundsSpecified.All);
		}

		public void SetBounds(int x, int y, int width, int height, BoundsSpecified specified) {
			// Fill in the values that were not specified
			if ((specified & BoundsSpecified.X) == 0)
				x = Left;
			if ((specified & BoundsSpecified.Y) == 0)
				y = Top;
			if ((specified & BoundsSpecified.Width) == 0)
				width = Width;
			if ((specified & BoundsSpecified.Height) == 0)
				height = Height;
		
			SetBoundsInternal (x, y, width, height, specified);
		}
		
		public void Show () {
			this.Visible = true;
		}

		public void SuspendLayout() {
			layout_suspended++;
		}

		#endregion	// Public Instance Methods

		#region Protected Instance Methods
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual ControlCollection CreateControlsInstance() {
			return new ControlCollection(this);
		}
		
		protected internal AutoSizeMode GetAutoSizeMode () 
		{
			return auto_size_mode;
		}

		private Rectangle GetScaledBoundsOld (Rectangle bounds, SizeF factor, BoundsSpecified specified)
		{
			RectangleF new_bounds = new RectangleF(bounds.Location, bounds.Size);

			// Top level controls do not scale location
			if (!is_toplevel) {
				if ((specified & BoundsSpecified.X) == BoundsSpecified.X)
					new_bounds.X *= factor.Width;
				if ((specified & BoundsSpecified.Y) == BoundsSpecified.Y)
					new_bounds.Y *= factor.Height;
			}

			if ((specified & BoundsSpecified.Width) == BoundsSpecified.Width && !GetStyle (ControlStyles.FixedWidth)) {
				int border = (this is Form) ? (this.bounds.Width - this.client_size.Width) : 0;
				new_bounds.Width = ((new_bounds.Width - border) * factor.Width + border);
			}
			if ((specified & BoundsSpecified.Height) == BoundsSpecified.Height && !GetStyle (ControlStyles.FixedHeight)) {
				int border = (this is Form) ? (this.bounds.Height - this.client_size.Height) : 0;
				new_bounds.Height = ((new_bounds.Height - border) * factor.Height + border);
			}

			bounds.X = (int)Math.Round (new_bounds.X);
			bounds.Y = (int)Math.Round (new_bounds.Y);
			bounds.Width = (int)Math.Round (new_bounds.Right) - bounds.X;
			bounds.Height = (int)Math.Round (new_bounds.Bottom) - bounds.Y;

			return bounds;
		}

		protected internal bool GetStyle(ControlStyles flag) {
			return (control_style & flag) != 0;
		}

		protected bool GetTopLevel() {
			return is_toplevel;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void InitLayout() {
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected void InvokeGotFocus(Control toInvoke, EventArgs e) {
			toInvoke.OnGotFocus(e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected void InvokeLostFocus(Control toInvoke, EventArgs e) {
			toInvoke.OnLostFocus(e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected void InvokeOnClick(Control toInvoke, EventArgs e) {
			toInvoke.OnClick(e);
		}

		protected void InvokePaint(Control c, PaintEventArgs e) {
			c.OnPaint (e);
		}

		protected void InvokePaintBackground(Control c, PaintEventArgs e) {
			c.OnPaintBackground (e);
		}

		internal virtual bool IsInputCharInternal (char charCode) {
			return false;
		}
		
		protected virtual bool IsInputKey (Keys keyData) {
			// Doc says this one calls IsInputChar; not sure what to do with that
			return false;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void NotifyInvalidate(Rectangle invalidatedArea) {
			// override me?
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

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected void RaiseDragEvent(object key, DragEventArgs e) {
			// MS Internal
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected void RaiseKeyEvent(object key, KeyEventArgs e) {
			// MS Internal
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected void RaiseMouseEvent(object key, MouseEventArgs e) {
			// MS Internal
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected void RaisePaintEvent(object key, PaintEventArgs e) {
			// MS Internal
		}

		private void SetIsRecreating () {
			is_recreating=true;

			foreach (Control c in Controls.GetAllControls()) {
				c.SetIsRecreating ();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected void ResetMouseEventArgs() {
			// MS Internal
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected ContentAlignment RtlTranslateAlignment(ContentAlignment align) {
			if (right_to_left == RightToLeft.No) {
				return align;
			}

			switch (align) {
				case ContentAlignment.TopLeft: {
					return ContentAlignment.TopRight;
				}

				case ContentAlignment.TopRight: {
					return ContentAlignment.TopLeft;
				}

				case ContentAlignment.MiddleLeft: {
					return ContentAlignment.MiddleRight;
				}

				case ContentAlignment.MiddleRight: {
					return ContentAlignment.MiddleLeft;
				}

				case ContentAlignment.BottomLeft: {
					return ContentAlignment.BottomRight;
				}

				case ContentAlignment.BottomRight: {
					return ContentAlignment.BottomLeft;
				}

				default: {
					// if it's center it doesn't change
					return align;
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected HorizontalAlignment RtlTranslateAlignment(HorizontalAlignment align) {
			if ((right_to_left == RightToLeft.No) || (align == HorizontalAlignment.Center)) {
				return align;
			}

			if (align == HorizontalAlignment.Left) {
				return HorizontalAlignment.Right;
			}

			// align must be HorizontalAlignment.Right
			return HorizontalAlignment.Left;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected LeftRightAlignment RtlTranslateAlignment(LeftRightAlignment align) {
			if (right_to_left == RightToLeft.No) {
				return align;
			}

			if (align == LeftRightAlignment.Left) {
				return LeftRightAlignment.Right;
			}

			// align must be LeftRightAlignment.Right;
			return LeftRightAlignment.Left;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected ContentAlignment RtlTranslateContent(ContentAlignment align) {
			return RtlTranslateAlignment(align);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected HorizontalAlignment RtlTranslateHorizontal(HorizontalAlignment align) {
			return RtlTranslateAlignment(align);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected LeftRightAlignment RtlTranslateLeftRight(LeftRightAlignment align) {
			return RtlTranslateAlignment(align);
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

		protected virtual void Select(bool directed, bool forward) {
			IContainerControl	container;
			
			container = GetContainerControl();
			if (container != null && (Control)container != this)
				container.ActiveControl = this;
		}

		protected void SetAutoSizeMode (AutoSizeMode mode)
		{
			if (auto_size_mode != mode) {
				auto_size_mode = mode;
				PerformLayout (this, "AutoSizeMode");
			}
		}
		
		
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			SetBoundsCoreInternal (x, y, width, height, specified);
		}


		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected internal void SetStyle(ControlStyles flag, bool value) {
			if (value) {
				control_style |= flag;
			} else {
				control_style &= ~flag;
			}
		}
		
		// Override this if there is a control that shall always remain on
		// top of other controls (such as scrollbars). If there are several
		// of these controls, the bottom-most should be returned.
		internal virtual IntPtr AfterTopMostControl () {
			return IntPtr.Zero;
		}
		#endregion	// Public Instance Methods
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
			for (int i=0; i<child_controls.Count; i++) child_controls[i].OnParentBackColorChanged(e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnBackgroundImageChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [BackgroundImageChangedEvent]);
			if (eh != null)
				eh (this, e);
			for (int i=0; i<child_controls.Count; i++) child_controls[i].OnParentBackgroundImageChanged(e);
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
			for (int i=0; i<child_controls.Count; i++) child_controls[i].OnParentBindingContextChanged(e);
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
				
			for (int i = 0; i < child_controls.Count; i++) child_controls[i].OnParentCursorChanged (e);
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
			for (int i=0; i<child_controls.Count; i++) child_controls[i].OnParentFontChanged(e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnForeColorChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [ForeColorChangedEvent]);
			if (eh != null)
				eh (this, e);
			for (int i=0; i<child_controls.Count; i++) child_controls[i].OnParentForeColorChanged(e);
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
			if (background_color.IsEmpty && background_image==null) {
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
			if (is_enabled) {
				OnEnabledChanged(e);
			}
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
			if (is_visible) {
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
			PerformLayout(this, "Bounds");

			EventHandler eh = (EventHandler)(Events [ResizeEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnRightToLeftChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [RightToLeftChangedEvent]);
			if (eh != null)
				eh (this, e);
			for (int i=0; i<child_controls.Count; i++) child_controls[i].OnParentRightToLeftChanged(e);
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
			if (Visible)
				CreateControl ();
				
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

		[MWFDescription("Fired when the cursor for the control has been changed"), MWFCategory("PropertyChanged")]
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
	}
}
