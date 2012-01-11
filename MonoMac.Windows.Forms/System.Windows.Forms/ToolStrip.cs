//
// ToolStrip.cs
//
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
// Copyright (c) 2006 Jonathan Pobst
//
// Authors:
//	Jonathan Pobst (monkey@jpobst.com)
//

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms
{
	[ComVisible (true)]
	[ClassInterface (ClassInterfaceType.AutoDispatch)]
	[DefaultEvent ("ItemClicked")]
	[DefaultProperty ("Items")]
	[Designer ("System.Windows.Forms.Design.ToolStripDesigner, " + Consts.AssemblySystem_Design, "System.ComponentModel.Design.IDesigner")]
	[DesignerSerializer ("System.Windows.Forms.Design.ToolStripCodeDomSerializer, " + Consts.AssemblySystem_Design, "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + Consts.AssemblySystem_Design)]
	public partial class ToolStrip : ScrollableControl, IComponent, IDisposable, IToolStripData
	{
		#region Private Variables
		private bool allow_item_reorder;
		private bool allow_merge;
		private Color back_color;
		private bool can_overflow;
		private ToolStrip currently_merged_with;
		internal ToolStripItemCollection displayed_items;
		private Color fore_color;
		private Padding grip_margin;
		private List<ToolStripItem> hidden_merged_items;
		private ImageList image_list;
		private Size image_scaling_size;
		private bool is_currently_merged;
		private ToolStripItemCollection items;
		private bool keyboard_active;
		private LayoutEngine layout_engine;
		private LayoutSettings layout_settings;
		private Orientation orientation;
		private List<ToolStripItem> pre_merge_items;
		//private ToolStripRenderer renderer;
		private ToolStripRenderMode render_mode;
		private bool show_item_tool_tips;
		private bool stretch;
		private ToolStripTextDirection text_direction;

		private ToolStripItem mouse_currently_over;
		internal bool menu_selected;
		private ToolStripItem tooltip_currently_showing;

		const int InitialToolTipDelay = 500;
		const int ToolTipDelay = 5000;
		#endregion

		#region Public Constructors
		public ToolStrip () : this (null)
		{
			Console.Write("toolstrip");
		}

		#endregion

		#region Public Properties
		[MonoTODO ("Stub, does nothing")]
		[DefaultValue (false)]
		public bool AllowItemReorder {
			get { return this.allow_item_reorder; }
			set { this.allow_item_reorder = value; }
		}
		
		[DefaultValue (true)]
		public bool AllowMerge {
			get { return this.allow_merge; }
			set { this.allow_merge = value; }
		}
		
		public override AnchorStyles Anchor {
			get { return base.Anchor; }
			set { base.Anchor = value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public override bool AutoScroll {
			get { return base.AutoScroll; }
			set { base.AutoScroll = value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public new Size AutoScrollMargin {
			get { return base.AutoScrollMargin; }
			set { base.AutoScrollMargin = value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public new Size AutoScrollMinSize {
			get { return base.AutoScrollMinSize; }
			set { base.AutoScrollMinSize = value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public new Point AutoScrollPosition {
			get { return base.AutoScrollPosition; }
			set { base.AutoScrollPosition = value; }
		}

		[DesignerSerializationVisibility (DesignerSerializationVisibility.Visible)]
		[Browsable (true)]
		[EditorBrowsable (EditorBrowsableState.Always)]
		[DefaultValue (true)]
		public override bool AutoSize {
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}
		
		new public Color BackColor {
			get { return this.back_color; }
			set { this.back_color = value; }
		}

		public override BindingContext BindingContext {
			get { return base.BindingContext; }
			set { base.BindingContext = value; }
		}
		
		[DefaultValue (true)]
		public bool CanOverflow {
			get { return this.can_overflow; }
			set { this.can_overflow = value; }
		}
		
		[Browsable (false)]
		[DefaultValue (false)]
		public new bool CausesValidation {
			get { return base.CausesValidation; }
			set { base.CausesValidation = value; }
		}
		
		[EditorBrowsable (EditorBrowsableState.Never)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public new ControlCollection Controls {
			get { return base.Controls; }
		}

		public override Font Font {
			get { return base.Font; }
			set { 
				if (base.Font != value) {
					base.Font = value;
					
					foreach (ToolStripItem tsi in this.Items)
						tsi.OnOwnerFontChanged (EventArgs.Empty);
				}
			 }
		}
		
		[Browsable (false)]
		public new Color ForeColor {
			get { return this.fore_color; }
			set { 
				if (this.fore_color != value) {
					this.fore_color = value; 
					this.OnForeColorChanged (EventArgs.Empty); 
				}
			}
		}
		
		public Padding GripMargin {
			get { return this.grip_margin; }
			set { 
				if (this.grip_margin != value) {
					this.grip_margin = value; 
					this.PerformLayout (); 
				}
			}
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public new bool HasChildren {
			get { return base.HasChildren; }
		}
		
		[Browsable (false)]
		[DefaultValue (null)]
		public ImageList ImageList {
			get { return this.image_list; }
			set { this.image_list = value; }
		}

		[DefaultValue ("{Width=16, Height=16}")]
		public Size ImageScalingSize {
			get { return this.image_scaling_size; }
			set { this.image_scaling_size = value; }
		}

		[MonoTODO ("Always returns false, dragging not implemented yet.")]
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public bool IsCurrentlyDragging {
			get { return false; }
		}

		[MergableProperty (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
		public virtual ToolStripItemCollection Items {
			get { return this.items; }
		}

		[Browsable (false)]
		[DefaultValue (null)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public LayoutSettings LayoutSettings {
			get { return this.layout_settings; }
			set { 
				if (this.layout_settings != value) {
					this.layout_settings = value;
					PerformLayout (this, "LayoutSettings");
				}
			}
		}

		[Browsable (false)]
		public Orientation Orientation {
			get { return this.orientation; }
		}

		[DefaultValue (true)]
		public bool ShowItemToolTips {
			get { return this.show_item_tool_tips; }
			set { this.show_item_tool_tips = value; }
		}
		
		[DefaultValue (false)]
		public bool Stretch {
			get { return this.stretch; }
			set { this.stretch = value; }
		}
		
		[DefaultValue (false)]
		[DispId(-516)]
		public new bool TabStop {
			get { return base.TabStop; }
			set { 
				base.TabStop = value;
				SetStyle (ControlStyles.Selectable, value);
			}
		}
		#endregion

		#region Protected Properties
		protected virtual DockStyle DefaultDock { get { return DockStyle.Top; } }
		protected virtual Padding DefaultGripMargin { get { return new Padding (2); } }
		protected override Padding DefaultMargin { get { return Padding.Empty; } }
		protected override Padding DefaultPadding { get { return new Padding (0, 0, 1, 0); } }
		protected virtual bool DefaultShowItemToolTips { get { return true; } }
		protected override Size DefaultSize { get { return new Size (100, 25); } }
		protected internal virtual ToolStripItemCollection DisplayedItems { get { return this.displayed_items; } }
		#endregion

		#region Public Methods
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new Control GetChildAtPoint (Point point)
		{
			return base.GetChildAtPoint (point);
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public new Control GetChildAtPoint (Point pt, GetChildAtPointSkip skipValue)
		{
			return base.GetChildAtPoint (pt, skipValue);
		}
		
		public ToolStripItem GetItemAt (Point point)
		{
			foreach (ToolStripItem tsi in this.displayed_items)
				if (tsi.Visible && tsi.Bounds.Contains (point))
					return tsi;

			return null;
		}

		public ToolStripItem GetItemAt (int x, int y)
		{
			return GetItemAt (new Point (x, y));
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public void ResetMinimumSize ()
		{
			this.MinimumSize = new Size (-1, -1);
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public new void SetAutoScrollMargin (int x, int y)
		{
			base.SetAutoScrollMargin (x, y);
		}
		
		public override string ToString ()
		{
			return String.Format ("{0}, Name: {1}, Items: {2}", base.ToString(), this.Name, this.items.Count.ToString ());
		}
		
		[DefaultValue (ToolStripTextDirection.Horizontal)]
		public virtual ToolStripTextDirection TextDirection {
			get { return this.text_direction; }
			set {
				if (!Enum.IsDefined (typeof (ToolStripTextDirection), value))
					throw new InvalidEnumArgumentException (string.Format ("Enum argument value '{0}' is not valid for ToolStripTextDirection", value));

				if (this.text_direction != value) {
					this.text_direction = value;
					
					this.PerformLayout (this, "TextDirection");
						
					this.Invalidate ();
				}
			}
		}
		#endregion

		#region Protected Methods		
		protected override ControlCollection CreateControlsInstance ()
		{
			return base.CreateControlsInstance ();
		}
		
		
		protected internal virtual ToolStripItem CreateDefaultItem (string text, Image image, EventHandler onClick)
		{
			//if (text == "-")
			//	return new ToolStripSeparator ();

			//if (this is ToolStripDropDown)
			//	return new ToolStripMenuItem (text, image, onClick);
			return null;	
			//return new ToolStripButton (text, image, onClick);
		}

		[MonoTODO ("Stub, never called")]
		protected virtual void OnBeginDrag (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events[BeginDragEvent]);
			if (eh != null)
				eh (this, e);
		}
		
		protected override void OnDockChanged (EventArgs e)
		{
			base.OnDockChanged (e);
		}

		[MonoTODO ("Stub, never called")]
		protected virtual void OnEndDrag (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events[EndDragEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected override bool IsInputChar (char charCode)
		{
			return base.IsInputChar (charCode);
		}

		protected override bool IsInputKey (Keys keyData)
		{
			return base.IsInputKey (keyData);
		}
		
		protected override void OnEnabledChanged (EventArgs e)
		{
			base.OnEnabledChanged (e);
			
			foreach (ToolStripItem tsi in this.Items)
				tsi.OnParentEnabledChanged (EventArgs.Empty);
		}

		protected override void OnFontChanged (EventArgs e)
		{
			base.OnFontChanged (e);
		}

		protected override void OnHandleCreated (EventArgs e)
		{
			base.OnHandleCreated (e);
		}

		protected override void OnHandleDestroyed (EventArgs e)
		{
			base.OnHandleDestroyed (e);
		}

		protected override void OnInvalidated (InvalidateEventArgs e)
		{
			base.OnInvalidated (e);
		}



		protected internal virtual void OnItemRemoved (ToolStripItemEventArgs e)
		{
			ToolStripItemEventHandler eh = (ToolStripItemEventHandler)(Events [ItemRemovedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnLayoutCompleted (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [LayoutCompletedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnLayoutStyleChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events[LayoutStyleChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected override void OnLeave (EventArgs e)
		{
			base.OnLeave (e);
		}

		protected override void OnLostFocus (EventArgs e)
		{
			base.OnLostFocus (e);
		}

		protected override void OnMouseCaptureChanged (EventArgs e)
		{
			base.OnMouseCaptureChanged (e);
		}
		



		protected virtual void OnRendererChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [RendererChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected override void OnRightToLeftChanged (EventArgs e)
		{
			base.OnRightToLeftChanged (e);

			foreach (ToolStripItem tsi in this.Items)
				tsi.OnParentRightToLeftChanged (e);
		}

		protected override void OnScroll (ScrollEventArgs se)
		{
			base.OnScroll (se);
		}
		
		protected override void OnTabStopChanged (EventArgs e)
		{
			base.OnTabStopChanged (e);
		}



		protected override bool ProcessCmdKey (ref Message m, Keys keyData)
		{
			return base.ProcessCmdKey (ref m, keyData);
		}


		[MonoTODO ("Stub, does nothing")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void RestoreFocus ()
		{
		}

		protected override void Select (bool directed, bool forward)
		{
			foreach (ToolStripItem tsi in this.DisplayedItems)
				if (tsi.CanSelect) {
					tsi.Select ();
					break;
				}
		}
		
		protected override void SetBoundsCore (int x, int y, int width, int height, BoundsSpecified specified)
		{
			base.SetBoundsCore (x, y, width, height, specified);
		}


		protected internal void SetItemLocation (ToolStripItem item, Point location)
		{
			if (item == null)
				throw new ArgumentNullException ("item");
				
			if (item.Owner != this)
				throw new NotSupportedException ("The item is not owned by this ToolStrip");
				
			item.SetBounds (new Rectangle (location, item.Size));
		}
		

		#endregion

		#region Public Events
		static object BeginDragEvent = new object ();
		static object EndDragEvent = new object ();
		static object ItemAddedEvent = new object ();
		static object ItemClickedEvent = new object ();
		static object ItemRemovedEvent = new object ();
		static object LayoutCompletedEvent = new object ();
		static object LayoutStyleChangedEvent = new object ();
		static object PaintGripEvent = new object ();
		static object RendererChangedEvent = new object ();

		[Browsable (true)]
		[EditorBrowsable (EditorBrowsableState.Always)]
		public new event EventHandler AutoSizeChanged {
			add { base.AutoSizeChanged += value; }
			remove { base.AutoSizeChanged -= value; }
		}

		[MonoTODO ("Event never raised")]
		public event EventHandler BeginDrag {
			add { Events.AddHandler (BeginDragEvent, value); }
			remove { Events.RemoveHandler (BeginDragEvent, value); }
		}

		[Browsable (false)]
		public new event EventHandler CausesValidationChanged {
			add { base.CausesValidationChanged += value; }
			remove { base.CausesValidationChanged -= value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event ControlEventHandler ControlAdded {
			add { base.ControlAdded += value; }
			remove { base.ControlAdded -= value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event ControlEventHandler ControlRemoved {
			add { base.ControlRemoved += value; }
			remove { base.ControlRemoved -= value; }
		}
		
		[Browsable (false)]
		public new event EventHandler CursorChanged {
			add { base.CursorChanged += value; }
			remove { base.CursorChanged -= value; }
		}

		[MonoTODO ("Event never raised")]
		public event EventHandler EndDrag {
			add { Events.AddHandler (EndDragEvent, value); }
			remove { Events.RemoveHandler (EndDragEvent, value); }
		}

		[Browsable (false)]
		public new event EventHandler ForeColorChanged {
			add { base.ForeColorChanged += value; }
			remove { base.ForeColorChanged -= value; }
		}

		public event ToolStripItemEventHandler ItemAdded {
			add { Events.AddHandler (ItemAddedEvent, value); }
			remove { Events.RemoveHandler (ItemAddedEvent, value); }
		}

		public event ToolStripItemClickedEventHandler ItemClicked {
			add { Events.AddHandler (ItemClickedEvent, value); }
			remove { Events.RemoveHandler (ItemClickedEvent, value); }
		}

		public event ToolStripItemEventHandler ItemRemoved {
			add { Events.AddHandler (ItemRemovedEvent, value); }
			remove { Events.RemoveHandler (ItemRemovedEvent, value); }
		}

		public event EventHandler LayoutCompleted {
			add { Events.AddHandler (LayoutCompletedEvent, value); }
			remove { Events.RemoveHandler (LayoutCompletedEvent, value); }
		}

		public event EventHandler LayoutStyleChanged {
			add { Events.AddHandler (LayoutStyleChangedEvent, value); }
			remove { Events.RemoveHandler (LayoutStyleChangedEvent, value); }
		}

		public event PaintEventHandler PaintGrip {
			add { Events.AddHandler (PaintGripEvent, value); }
			remove { Events.RemoveHandler (PaintGripEvent, value); }
		}

		public event EventHandler RendererChanged {
			add { Events.AddHandler (RendererChangedEvent, value); }
			remove { Events.RemoveHandler (RendererChangedEvent, value); }
		}
		#endregion
		
		#region Private Methods
		internal virtual Rectangle CalculateConnectedArea ()
		{
			return Rectangle.Empty;
		}
		
		
		internal virtual void Dismiss ()
		{
			this.Dismiss (ToolStripDropDownCloseReason.AppClicked);
		}

		internal ToolStripItem GetCurrentlySelectedItem ()
		{
			foreach (ToolStripItem tsi in this.DisplayedItems)
				if (tsi.Selected)
					return tsi;
					
			return null;
		}
		

		internal override Size GetPreferredSizeCore (Size proposedSize)
		{
			return GetToolStripPreferredSize (proposedSize);
		}
		
		
		internal virtual ToolStrip GetTopLevelToolStrip ()
		{
			return this;
		}
		
		internal virtual void HandleItemClick (ToolStripItem dismissingItem)
		{
			this.GetTopLevelToolStrip ().Dismiss (ToolStripDropDownCloseReason.ItemClicked);
		}
		

		
		internal virtual bool OnMenuKey ()
		{
			return false;
		}

		#endregion
	}
}
