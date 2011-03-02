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
// Copyright (c) 2004-2005 Novell, Inc. (http://www.novell.com)
//
// Authors:
//	Ravindra Kumar (rkumar@novell.com)
//	Jordi Mas i Hernandez, jordi@ximian.com
//	Mike Kestner (mkestner@novell.com)
//	Daniel Nauck (dna(at)mono-project(dot)de)
//	Carlos Alberto Cortez <calberto.cortez@gmail.com>
//


// NOT COMPLETE


using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections.Generic;

namespace System.Windows.Forms
{
	[DefaultEvent ("SelectedIndexChanged")]
	[DefaultProperty ("Items")]
	[Designer ("System.Windows.Forms.Design.ListViewDesigner, " + Consts.AssemblySystem_Design, "System.ComponentModel.Design.IDesigner")]
	[ClassInterface (ClassInterfaceType.AutoDispatch)]
	[ComVisible (true)]
	[Docking (DockingBehavior.Ask)]
	public partial class ListView : Control
	{
		private ItemActivation activation = ItemActivation.Standard;
		private ListViewAlignment alignment = ListViewAlignment.Top;
		private bool allow_column_reorder;
		private bool auto_arrange = true;
		private bool check_boxes;
		private readonly CheckedIndexCollection checked_indices;
		private readonly CheckedListViewItemCollection checked_items;
		private readonly ColumnHeaderCollection columns;
		internal int focused_item_index = -1;
		private bool full_row_select;
		private bool grid_lines;
		private ColumnHeaderStyle header_style = ColumnHeaderStyle.Clickable;
		private bool hide_selection = true;
		private bool hover_selection;
		private IComparer item_sorter;
		private readonly ListViewItemCollection items;
		private readonly ListViewGroupCollection groups;
		private bool owner_draw;
		private bool show_groups = true;
		private bool label_edit;
		private bool label_wrap = true;
		private bool multiselect = true;
		private bool scrollable = true;
		private bool hover_pending;
		private readonly SelectedIndexCollection selected_indices;
		private readonly SelectedListViewItemCollection selected_items;
		private SortOrder sort_order = SortOrder.None;
		private ImageList state_image_list;
		internal bool updating;
		private View view = View.LargeIcon;
		private int layout_wd;    // We might draw more than our client area
		private int layout_ht;    // therefore we need to have these two.
		//internal HeaderControl header_control;
		//internal ItemControl item_control;
		internal ScrollBar h_scroll; // used for scrolling horizontally
		internal ScrollBar v_scroll; // used for scrolling vertically
		internal int h_marker;		// Position markers for scrolling
		internal int v_marker;
		private int keysearch_tickcnt;
		private string keysearch_text;
		static private readonly int keysearch_keydelay = 1000;
		private int[] reordered_column_indices;
		private int[] reordered_items_indices;
		private Point [] items_location;
		private ItemMatrixLocation [] items_matrix_location;
		private Size item_size; // used for caching item size
		private int custom_column_width; // used when using Columns with SmallIcon/List views
		private int hot_item_index = -1;
		private bool hot_tracking;
		private ListViewInsertionMark insertion_mark;
		private bool show_item_tooltips;
		private ToolTip item_tooltip;
		private Size tile_size;
		private bool virtual_mode;
		private int virtual_list_size;
		private bool right_to_left_layout;
		// selection is available after the first time the handle is created, *even* if later
		// the handle is either recreated or destroyed - so keep this info around.
		private bool is_selection_available;

		// internal variables
		internal ImageList large_image_list;
		internal ImageList small_image_list;
		internal Size text_size = Size.Empty;

		#region Events
		static object AfterLabelEditEvent = new object ();
		static object BeforeLabelEditEvent = new object ();
		static object ColumnClickEvent = new object ();
		static object ItemActivateEvent = new object ();
		static object ItemCheckEvent = new object ();
		static object ItemDragEvent = new object ();
		static object SelectedIndexChangedEvent = new object ();
		static object DrawColumnHeaderEvent = new object();
		static object DrawItemEvent = new object();
		static object DrawSubItemEvent = new object();
		static object ItemCheckedEvent = new object ();
		static object ItemMouseHoverEvent = new object ();
		static object ItemSelectionChangedEvent = new object ();
		static object CacheVirtualItemsEvent = new object ();
		static object RetrieveVirtualItemEvent = new object ();
		static object RightToLeftLayoutChangedEvent = new object ();
		static object SearchForVirtualItemEvent = new object ();
		static object VirtualItemsSelectionRangeChangedEvent = new object ();

		public event LabelEditEventHandler AfterLabelEdit {
			add { Events.AddHandler (AfterLabelEditEvent, value); }
			remove { Events.RemoveHandler (AfterLabelEditEvent, value); }
		}


		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event EventHandler BackgroundImageLayoutChanged {
			add { base.BackgroundImageLayoutChanged += value; }
			remove { base.BackgroundImageLayoutChanged -= value; }
		}

		public event LabelEditEventHandler BeforeLabelEdit {
			add { Events.AddHandler (BeforeLabelEditEvent, value); }
			remove { Events.RemoveHandler (BeforeLabelEditEvent, value); }
		}

		public event ColumnClickEventHandler ColumnClick {
			add { Events.AddHandler (ColumnClickEvent, value); }
			remove { Events.RemoveHandler (ColumnClickEvent, value); }
		}

		public event DrawListViewColumnHeaderEventHandler DrawColumnHeader {
			add { Events.AddHandler(DrawColumnHeaderEvent, value); }
			remove { Events.RemoveHandler(DrawColumnHeaderEvent, value); }
		}

		public event DrawListViewItemEventHandler DrawItem {
			add { Events.AddHandler(DrawItemEvent, value); }
			remove { Events.RemoveHandler(DrawItemEvent, value); }
		}

		public event DrawListViewSubItemEventHandler DrawSubItem {
			add { Events.AddHandler(DrawSubItemEvent, value); }
			remove { Events.RemoveHandler(DrawSubItemEvent, value); }
		}

		public event EventHandler ItemActivate {
			add { Events.AddHandler (ItemActivateEvent, value); }
			remove { Events.RemoveHandler (ItemActivateEvent, value); }
		}

		public event ItemCheckEventHandler ItemCheck {
			add { Events.AddHandler (ItemCheckEvent, value); }
			remove { Events.RemoveHandler (ItemCheckEvent, value); }
		}

		public event ItemCheckedEventHandler ItemChecked {
			add { Events.AddHandler (ItemCheckedEvent, value); }
			remove { Events.RemoveHandler (ItemCheckedEvent, value); }
		}

		public event ItemDragEventHandler ItemDrag {
			add { Events.AddHandler (ItemDragEvent, value); }
			remove { Events.RemoveHandler (ItemDragEvent, value); }
		}

		public event ListViewItemMouseHoverEventHandler ItemMouseHover {
			add { Events.AddHandler (ItemMouseHoverEvent, value); }
			remove { Events.RemoveHandler (ItemMouseHoverEvent, value); }
		}

		public event ListViewItemSelectionChangedEventHandler ItemSelectionChanged {
			add { Events.AddHandler (ItemSelectionChangedEvent, value); }
			remove { Events.RemoveHandler (ItemSelectionChangedEvent, value); }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event EventHandler PaddingChanged {
			add { base.PaddingChanged += value; }
			remove { base.PaddingChanged -= value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event PaintEventHandler Paint {
			add { base.Paint += value; }
			remove { base.Paint -= value; }
		}

		public event EventHandler SelectedIndexChanged {
			add { Events.AddHandler (SelectedIndexChangedEvent, value); }
			remove { Events.RemoveHandler (SelectedIndexChangedEvent, value); }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event EventHandler TextChanged {
			add { base.TextChanged += value; }
			remove { base.TextChanged -= value; }
		}

		public event CacheVirtualItemsEventHandler CacheVirtualItems {
			add { Events.AddHandler (CacheVirtualItemsEvent, value); }
			remove { Events.RemoveHandler (CacheVirtualItemsEvent, value); }
		}

		public event RetrieveVirtualItemEventHandler RetrieveVirtualItem {
			add { Events.AddHandler (RetrieveVirtualItemEvent, value); }
			remove { Events.RemoveHandler (RetrieveVirtualItemEvent, value); }
		}

		public event EventHandler RightToLeftLayoutChanged {
			add { Events.AddHandler (RightToLeftLayoutChangedEvent, value); }
			remove { Events.RemoveHandler (RightToLeftLayoutChangedEvent, value); }
		}

		public event SearchForVirtualItemEventHandler SearchForVirtualItem {
			add { Events.AddHandler (SearchForVirtualItemEvent, value); }
			remove { Events.AddHandler (SearchForVirtualItemEvent, value); }
		}
		
		public event ListViewVirtualItemsSelectionRangeChangedEventHandler VirtualItemsSelectionRangeChanged {
			add { Events.AddHandler (VirtualItemsSelectionRangeChangedEvent, value); }
			remove { Events.RemoveHandler (VirtualItemsSelectionRangeChangedEvent, value); }
		}

		#endregion // Events


		#region Private Internal Properties

		internal Size ItemSize {
			get {
				if (view != View.Details)
					return item_size;

				Size size = new Size ();
				size.Height = item_size.Height;
				for (int i = 0; i < columns.Count; i++)
					size.Width += columns [i].Wd;

				return size;
			}
			set {
				item_size = value;
			}
		}

		internal int HotItemIndex {
			get {
				return hot_item_index;
			}
			set {
				hot_item_index = value;
			}
		}

		internal override bool ScaleChildrenInternal {
			get { return false; }
		}

		internal bool UseCustomColumnWidth {
			get {
				return (view == View.List || view == View.SmallIcon) && columns.Count > 0;
			}
		}

		internal ColumnHeader EnteredColumnHeader {
			get {
				return header_control.EnteredColumnHeader;
			}
		}
		#endregion	// Private Internal Properties

		#region	 Protected Properties
		protected override CreateParams CreateParams {
			get { return base.CreateParams; }
		}

		protected override bool DoubleBuffered {
			get {
				return base.DoubleBuffered;
			}
			set {
				base.DoubleBuffered = value;
			}
		}
		#endregion	// Protected Properties

		#region Public Instance Properties
		[DefaultValue (ItemActivation.Standard)]
		public ItemActivation Activation {
			get { return activation; }
			set { 
				if (value != ItemActivation.Standard && value != ItemActivation.OneClick && 
					value != ItemActivation.TwoClick) {
					throw new InvalidEnumArgumentException (string.Format
						("Enum argument value '{0}' is not valid for Activation", value));
				}
				if (hot_tracking && value != ItemActivation.OneClick)
					throw new ArgumentException ("When HotTracking is on, activation must be ItemActivation.OneClick");
				
				activation = value;
			}
		}

		[DefaultValue (ListViewAlignment.Top)]
		[Localizable (true)]
		public ListViewAlignment Alignment {
			get { return alignment; }
			set {
				if (value != ListViewAlignment.Default && value != ListViewAlignment.Left && 
					value != ListViewAlignment.SnapToGrid && value != ListViewAlignment.Top) {
					throw new InvalidEnumArgumentException (string.Format 
						("Enum argument value '{0}' is not valid for Alignment", value));
				}
				
				if (this.alignment != value) {
					alignment = value;
					// alignment does not matter in Details/List views
					if (this.view == View.LargeIcon || this.View == View.SmallIcon)
						this.Redraw (true);
				}
			}
		}

		[DefaultValue (false)]
		public bool AllowColumnReorder {
			get { return allow_column_reorder; }
			set { allow_column_reorder = value; }
		}

		[DefaultValue (true)]
		public bool AutoArrange {
			get { return auto_arrange; }
			set {
				if (auto_arrange != value) {
					auto_arrange = value;
					// autoarrange does not matter in Details/List views
					if (this.view == View.LargeIcon || this.View == View.SmallIcon)
						this.Redraw (true);
				}
			}
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout {
			get {
				return base.BackgroundImageLayout;
			}
			set {
				base.BackgroundImageLayout = value;
			}
		}

		[DefaultValue (false)]
		public bool BackgroundImageTiled {
			get {
				return item_control.BackgroundImageLayout == ImageLayout.Tile;
			}
			set {
				ImageLayout new_image_layout = value ? ImageLayout.Tile : ImageLayout.None;
				if (new_image_layout == item_control.BackgroundImageLayout)
					return;

				item_control.BackgroundImageLayout = new_image_layout;
			}
		}

		[DefaultValue (BorderStyle.Fixed3D)]
		[DispId (-504)]
		public BorderStyle BorderStyle {
			get { return InternalBorderStyle; }
			set { InternalBorderStyle = value; }
		}

		[DefaultValue (false)]
		public bool CheckBoxes {
			get { return check_boxes; }
			set {
				if (check_boxes != value) {
					if (value && View == View.Tile)
						throw new NotSupportedException ("CheckBoxes are not"
							+ " supported in Tile view. Choose a different"
							+ " view or set CheckBoxes to false.");

					check_boxes = value;
					this.Redraw (true);

					//UIA Framework: Event used by ListView to set/unset Toggle Pattern
					OnUIACheckBoxesChanged ();
				}
			}
		}

		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public CheckedIndexCollection CheckedIndices {
			get { return checked_indices; }
		}

		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public CheckedListViewItemCollection CheckedItems {
			get { return checked_items; }
		}

		[Editor ("System.Windows.Forms.Design.ColumnHeaderCollectionEditor, " + Consts.AssemblySystem_Design, typeof (System.Drawing.Design.UITypeEditor))]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
		[Localizable (true)]
		[MergableProperty (false)]
		public ColumnHeaderCollection Columns {
			get { return columns; }
		}

		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public ListViewItem FocusedItem {
			get {
				if (focused_item_index == -1)
					return null;

				return GetItemAtDisplayIndex (focused_item_index);
			}
			set {
				if (value == null || value.ListView != this || 
						!IsHandleCreated)
					return;

				SetFocusedItem (value.DisplayIndex);
			}
		}
		
		[DefaultValue (false)]
		public bool FullRowSelect {
			get { return full_row_select; }
			set { 
				if (full_row_select != value) {
					full_row_select = value;
					InvalidateSelection ();
				}
			}
		}

		[DefaultValue (false)]
		public bool GridLines {
			get { return grid_lines; }
			set {
				if (grid_lines != value) {
					grid_lines = value;
					this.Redraw (false);
				}
			}
		}

		[DefaultValue (ColumnHeaderStyle.Clickable)]
		public ColumnHeaderStyle HeaderStyle {
			get { return header_style; }
			set {
				if (header_style == value)
					return;

				switch (value) {
				case ColumnHeaderStyle.Clickable:
				case ColumnHeaderStyle.Nonclickable:
				case ColumnHeaderStyle.None:
					break;
				default:
					throw new InvalidEnumArgumentException (string.Format 
						("Enum argument value '{0}' is not valid for ColumnHeaderStyle", value));
				}
				
				header_style = value;
				if (view == View.Details)
					Redraw (true);
			}
		}

		[DefaultValue (true)]
		public bool HideSelection {
			get { return hide_selection; }
			set {
				if (hide_selection != value) {
					hide_selection = value;
					InvalidateSelection ();
				}
			}
		}

		[DefaultValue (false)]
		public bool HotTracking {
			get {
				return hot_tracking;
			}
			set {
				if (hot_tracking == value)
					return;
				
				hot_tracking = value;
				if (hot_tracking) {
					hover_selection = true;
					activation = ItemActivation.OneClick;
				}
			}
		}

		[DefaultValue (false)]
		public bool HoverSelection {
			get { return hover_selection; }
			set { 
				if (hot_tracking && value == false)
					throw new ArgumentException ("When HotTracking is on, hover selection must be true");
				hover_selection = value; 
			}
		}

		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		[Browsable (false)]
		public ListViewInsertionMark InsertionMark {
			get {
				return insertion_mark;
			}
		}

		[Editor ("System.Windows.Forms.Design.ListViewItemCollectionEditor, " + Consts.AssemblySystem_Design, typeof (System.Drawing.Design.UITypeEditor))]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
		[Localizable (true)]
		[MergableProperty (false)]
		public ListViewItemCollection Items {
			get { return items; }
		}

		[DefaultValue (false)]
		public bool LabelEdit {
			get { return label_edit; }
			set { 
				if (value != label_edit) {
					label_edit = value; 

					// UIA Framework: Event used by Value Pattern in ListView.ListItem provider
					OnUIALabelEditChanged ();
				}

			}
		}

		[DefaultValue (true)]
		[Localizable (true)]
		public bool LabelWrap {
			get { return label_wrap; }
			set {
				if (label_wrap != value) {
					label_wrap = value;
					this.Redraw (true);
				}
			}
		}

		[DefaultValue (null)]
		public ImageList LargeImageList {
			get { return large_image_list; }
			set {
				large_image_list = value;
				this.Redraw (true);
			}
		}

		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public IComparer ListViewItemSorter {
			get {
				if (View != View.SmallIcon && View != View.LargeIcon && item_sorter is ItemComparer)
					return null;
				return item_sorter;
			}
			set {
				if (item_sorter != value) {
					item_sorter = value;
					Sort ();
				}
			}
		}

		[DefaultValue (true)]
		public bool MultiSelect {
			get { return multiselect; }
			set {
				if (value != multiselect) {
					multiselect = value; 

					// UIA Framework: Event used by Selection Pattern in ListView.ListItem provider
					OnUIAMultiSelectChanged ();
				}
			}
		}


		[DefaultValue(false)]
		public bool OwnerDraw {
			get { return owner_draw; }
			set { 
				owner_draw = value;
				Redraw (true);
			}
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public new Padding Padding {
			get {
				return base.Padding;
			}
			set {
				base.Padding = value;
			}
		}
		
		[MonoTODO ("RTL not supported")]
		[Localizable (true)]
		[DefaultValue (false)]
		public virtual bool RightToLeftLayout {
			get { return right_to_left_layout; }
			set { 
				if (right_to_left_layout != value) {
					right_to_left_layout = value;
					OnRightToLeftLayoutChanged (EventArgs.Empty);
				}
			}
		}

		[DefaultValue (true)]
		public bool Scrollable {
			get { return scrollable; }
			set {
				if (scrollable != value) {
					scrollable = value;
					this.Redraw (true);
				}
			}
		}

		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public SelectedIndexCollection SelectedIndices {
			get { return selected_indices; }
		}

		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public SelectedListViewItemCollection SelectedItems {
			get { return selected_items; }
		}

		[DefaultValue(true)]
		public bool ShowGroups {
			get { return show_groups; }
			set {
				if (show_groups != value) {
					show_groups = value;
					Redraw(true);

					// UIA Framework: Used to update a11y Tree
					OnUIAShowGroupsChanged ();
				}
			}
		}

		[LocalizableAttribute (true)]
		[MergableProperty (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
		[Editor ("System.Windows.Forms.Design.ListViewGroupCollectionEditor, " + Consts.AssemblySystem_Design, typeof (System.Drawing.Design.UITypeEditor))]
		public ListViewGroupCollection Groups {
			get { return groups; }
		}

		[DefaultValue (false)]
		public bool ShowItemToolTips {
			get {
				return show_item_tooltips;
			}
			set {
				show_item_tooltips = value;
				item_tooltip.Active = false;
			}
		}

		[DefaultValue (null)]
		public ImageList SmallImageList {
			get { return small_image_list; }
			set {
				small_image_list = value;
				this.Redraw (true);
			}
		}

		[DefaultValue (SortOrder.None)]
		public SortOrder Sorting {
			get { return sort_order; }
			set { 
				if (!Enum.IsDefined (typeof (SortOrder), value)) {
					throw new InvalidEnumArgumentException ("value", (int) value,
						typeof (SortOrder));
				}
				
				if (sort_order == value)
					return;

				sort_order = value;

				if (virtual_mode) // Sorting is not allowed in virtual mode
					return;

				if (value == SortOrder.None) {
					if (item_sorter != null) {
						// ListViewItemSorter should never be reset for SmallIcon
						// and LargeIcon view
						if (View != View.SmallIcon && View != View.LargeIcon)
							item_sorter = null;
					}
					this.Redraw (false);
				} else {
					if (item_sorter == null)
						item_sorter = new ItemComparer (value);
					if (item_sorter is ItemComparer) {
						item_sorter = new ItemComparer (value);
					}
					Sort ();
				}
			}
		}

		private void OnImageListChanged (object sender, EventArgs args)
		{
			item_control.Invalidate ();
		}

		[DefaultValue (null)]
		public ImageList StateImageList {
			get { return state_image_list; }
			set {
				if (state_image_list == value)
					return;

				if (state_image_list != null)
					state_image_list.Images.Changed -= new EventHandler (OnImageListChanged);

				state_image_list = value;

				if (state_image_list != null)
					state_image_list.Images.Changed += new EventHandler (OnImageListChanged);

				this.Redraw (true);
			}
		}

		[Bindable (false)]
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public override string Text {
			get { return base.Text; } 
			set {
				if (value == base.Text)
					return;

				base.Text = value;
				this.Redraw (true);
			}
		}

		[Browsable (true)]
		public Size TileSize {
			get {
				return tile_size;
			}
			set {
				if (value.Width <= 0 || value.Height <= 0)
					throw new ArgumentOutOfRangeException ("value");

				tile_size = value;
				if (view == View.Tile)
					Redraw (true);
			}
		}

		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public ListViewItem TopItem {
			get {
				if (view == View.LargeIcon || view == View.SmallIcon || view == View.Tile)
					throw new InvalidOperationException ("Cannot get the top item in LargeIcon, SmallIcon or Tile view.");
				// there is no item
				if (this.items.Count == 0)
					return null;
				// if contents are not scrolled
				// it is the first item
				else if (h_marker == 0 && v_marker == 0)
					return this.items [0];
				// do a hit test for the scrolled position
				else {
					int header_offset = header_control.Height;
					for (int i = 0; i < items.Count; i++) {
						Point item_loc = GetItemLocation (i);
						if (item_loc.X >= 0 && item_loc.Y - header_offset >= 0)
							return items [i];
					}
					return null;
				}
			}
			set {
				if (view == View.LargeIcon || view == View.SmallIcon || view == View.Tile)
					throw new InvalidOperationException ("Cannot set the top item in LargeIcon, SmallIcon or Tile view.");

				// .Net doesn't throw any exception in the cases below
				if (value == null || value.ListView != this)
					return;

				// Take advantage this property is only valid for Details view.
				SetScrollValue (v_scroll, item_size.Height * value.Index);
			}
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[DefaultValue (true)]
		[Browsable (false)]
		[MonoInternalNote ("Stub, not implemented")]
		public bool UseCompatibleStateImageBehavior {
			get {
				return false;
			}
			set {
			}
		}

		[DefaultValue (View.LargeIcon)]
		public View View {
			get { return view; }
			set { 
				if (!Enum.IsDefined (typeof (View), value))
					throw new InvalidEnumArgumentException ("value", (int) value,
						typeof (View));

				if (view != value) {
					if (CheckBoxes && value == View.Tile)
						throw new NotSupportedException ("CheckBoxes are not"
							+ " supported in Tile view. Choose a different"
							+ " view or set CheckBoxes to false.");
					if (VirtualMode && value == View.Tile)
						throw new NotSupportedException ("VirtualMode is"
							+ " not supported in Tile view. Choose a different"
							+ " view or set ViewMode to false.");

					h_scroll.Value = v_scroll.Value = 0;
					view = value; 
					Redraw (true);

					// UIA Framework: Event used to update UIA Tree.
					OnUIAViewChanged ();
				}
			}
		}

		[DefaultValue (false)]
		[RefreshProperties (RefreshProperties.Repaint)]
		public bool VirtualMode {
			get {
				return virtual_mode;
			}
			set {
				if (virtual_mode == value)
					return;

				if (!virtual_mode && items.Count > 0)
					throw new InvalidOperationException ();
				if (value && view == View.Tile)
					throw new NotSupportedException ("VirtualMode is"
						+ " not supported in Tile view. Choose a different"
						+ " view or set ViewMode to false.");

				virtual_mode = value;
				Redraw (true);
			}
		}

		[DefaultValue (0)]
		[RefreshProperties (RefreshProperties.Repaint)]
		public int VirtualListSize {
			get {
				return virtual_list_size;
			}
			set {
				if (value < 0)
					throw new ArgumentException ("value");

				if (virtual_list_size == value)
					return;

				virtual_list_size = value;
				if (virtual_mode) {
					selected_indices.Reset ();
					Redraw (true);
				}
			}
		}
		#endregion	// Public Instance Properties
		
		#region Protected Methods
		protected override void CreateHandle ()
		{
			base.CreateHandle ();
			is_selection_available = true;
			for (int i = 0; i < SelectedItems.Count; i++)
				OnSelectedIndexChanged (EventArgs.Empty);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				large_image_list = null;
				small_image_list = null;
				state_image_list = null;

				foreach (ColumnHeader col in columns)
					col.SetListView (null);

				if (!virtual_mode) // In virtual mode we don't save the items
					foreach (ListViewItem item in items)
						item.Owner = null;
			}
			
			base.Dispose (disposing);
		}

		protected override bool IsInputKey (Keys keyData)
		{
			switch (keyData) {
			case Keys.Up:
			case Keys.Down:
			case Keys.PageUp:
			case Keys.PageDown:
			case Keys.Right:
			case Keys.Left:
			case Keys.End:
			case Keys.Home:
				return true;

			default:
				break;
			}
			
			return base.IsInputKey (keyData);
		}

		protected virtual void OnAfterLabelEdit (LabelEditEventArgs e)
		{
			LabelEditEventHandler eh = (LabelEditEventHandler)(Events [AfterLabelEditEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected override void OnBackgroundImageChanged (EventArgs e)
		{
			item_control.BackgroundImage = BackgroundImage;
			base.OnBackgroundImageChanged (e);
		}

		protected virtual void OnBeforeLabelEdit (LabelEditEventArgs e)
		{
			LabelEditEventHandler eh = (LabelEditEventHandler)(Events [BeforeLabelEditEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected internal virtual void OnColumnClick (ColumnClickEventArgs e)
		{
			ColumnClickEventHandler eh = (ColumnClickEventHandler)(Events [ColumnClickEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected internal virtual void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
		{
			DrawListViewColumnHeaderEventHandler eh = (DrawListViewColumnHeaderEventHandler)(Events[DrawColumnHeaderEvent]);
			if (eh != null)
				eh(this, e);
		}

		protected internal virtual void OnDrawItem(DrawListViewItemEventArgs e)
		{
			DrawListViewItemEventHandler eh = (DrawListViewItemEventHandler)(Events[DrawItemEvent]);
			if (eh != null)
				eh(this, e);
		}

		protected internal virtual void OnDrawSubItem(DrawListViewSubItemEventArgs e)
		{
			DrawListViewSubItemEventHandler eh = (DrawListViewSubItemEventHandler)(Events[DrawSubItemEvent]);
			if (eh != null)
				eh(this, e);
		}

		protected override void OnFontChanged (EventArgs e)
		{
			base.OnFontChanged (e);
			Redraw (true);
		}

		protected override void OnHandleDestroyed (EventArgs e)
		{
			base.OnHandleDestroyed (e);
		}

		protected virtual void OnItemActivate (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [ItemActivateEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected internal virtual void OnItemCheck (ItemCheckEventArgs ice)
		{
			ItemCheckEventHandler eh = (ItemCheckEventHandler)(Events [ItemCheckEvent]);
			if (eh != null)
				eh (this, ice);
		}

		protected internal virtual void OnItemChecked (ItemCheckedEventArgs e)
		{
			ItemCheckedEventHandler eh = (ItemCheckedEventHandler)(Events [ItemCheckedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnItemDrag (ItemDragEventArgs e)
		{
			ItemDragEventHandler eh = (ItemDragEventHandler)(Events [ItemDragEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnItemMouseHover (ListViewItemMouseHoverEventArgs e)
		{
			ListViewItemMouseHoverEventHandler eh = (ListViewItemMouseHoverEventHandler)(Events [ItemMouseHoverEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected internal virtual void OnItemSelectionChanged (ListViewItemSelectionChangedEventArgs e)
		{
			ListViewItemSelectionChangedEventHandler eh = 
				(ListViewItemSelectionChangedEventHandler) Events [ItemSelectionChangedEvent];
			if (eh != null)
				eh (this, e);
		}

		protected override void OnMouseHover (EventArgs e)
		{
			base.OnMouseHover (e);
		}

		protected override void OnParentChanged (EventArgs e)
		{
			base.OnParentChanged (e);
		}

		protected virtual void OnSelectedIndexChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [SelectedIndexChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected override void OnSystemColorsChanged (EventArgs e)
		{
			base.OnSystemColorsChanged (e);
		}

		protected internal virtual void OnCacheVirtualItems (CacheVirtualItemsEventArgs e)
		{
			CacheVirtualItemsEventHandler eh = (CacheVirtualItemsEventHandler)Events [CacheVirtualItemsEvent];
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnRetrieveVirtualItem (RetrieveVirtualItemEventArgs e)
		{
			RetrieveVirtualItemEventHandler eh = (RetrieveVirtualItemEventHandler)Events [RetrieveVirtualItemEvent];
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void OnRightToLeftLayoutChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)Events[RightToLeftLayoutChangedEvent];
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnSearchForVirtualItem (SearchForVirtualItemEventArgs e)
		{
			SearchForVirtualItemEventHandler eh = (SearchForVirtualItemEventHandler) Events [SearchForVirtualItemEvent];
			if (eh != null)
				eh (this, e);
		}
		
		protected virtual void OnVirtualItemsSelectionRangeChanged (ListViewVirtualItemsSelectionRangeChangedEventArgs e)
		{
			ListViewVirtualItemsSelectionRangeChangedEventHandler eh = 
				(ListViewVirtualItemsSelectionRangeChangedEventHandler) Events [VirtualItemsSelectionRangeChangedEvent];
			if (eh != null)
				eh (this, e);
		}

		protected void RealizeProperties ()
		{
			// FIXME: TODO
		}

		protected void UpdateExtendedStyles ()
		{
			// FIXME: TODO
		}

		bool refocusing = false;

		#endregion // Protected Methods

		#region Public Instance Methods
		public void ArrangeIcons ()
		{
			ArrangeIcons (this.alignment);
		}

		public void ArrangeIcons (ListViewAlignment value)
		{
			// Icons are arranged only if view is set to LargeIcon or SmallIcon
			if (view == View.LargeIcon || view == View.SmallIcon)
				Redraw (true);
		}

		public void AutoResizeColumn (int columnIndex, ColumnHeaderAutoResizeStyle headerAutoResize)
		{
			if (columnIndex < 0 || columnIndex >= columns.Count)
				throw new ArgumentOutOfRangeException ("columnIndex");

			columns [columnIndex].AutoResize (headerAutoResize);
		}

		public void AutoResizeColumns (ColumnHeaderAutoResizeStyle headerAutoResize)
		{
			BeginUpdate ();
			foreach (ColumnHeader col in columns) 
				col.AutoResize (headerAutoResize);
			EndUpdate ();
		}

		public void BeginUpdate ()
		{
			// flag to avoid painting
			updating = true;
		}

		public void Clear ()
		{
			columns.Clear ();
			items.Clear ();	// Redraw (true) called here
		}

		public void EndUpdate ()
		{
			// flag to avoid painting
			updating = false;

			// probably, now we need a redraw with recalculations
			this.Redraw (true);
		}

		public void EnsureVisible (int index)
		{
			if (index < 0 || index >= items.Count || scrollable == false || updating)
				return;

			Rectangle view_rect = item_control.ClientRectangle;
			// Avoid direct access to items in virtual mode, and use item bounds otherwise, since we could have reordered items
			Rectangle bounds = virtual_mode ? new Rectangle (GetItemLocation (index), ItemSize) : items [index].Bounds;

			if (view == View.Details && header_style != ColumnHeaderStyle.None) {
				view_rect.Y += header_control.Height;
				view_rect.Height -= header_control.Height;
			}

			if (view_rect.Contains (bounds))
				return;

			if (View != View.Details) {
				if (bounds.Left < 0)
					h_scroll.Value += bounds.Left;
				else if (bounds.Right > view_rect.Right)
					h_scroll.Value += (bounds.Right - view_rect.Right);
			}

			if (bounds.Top < view_rect.Y)
				v_scroll.Value += bounds.Top - view_rect.Y;
			else if (bounds.Bottom > view_rect.Bottom)
				v_scroll.Value += (bounds.Bottom - view_rect.Bottom);
		}

		public ListViewItem FindItemWithText (string text)
		{
			if (items.Count == 0)
				return null;

			return FindItemWithText (text, true, 0, true);
		}

		public ListViewItem FindItemWithText (string text, bool includeSubItemsInSearch, int startIndex)
		{
			return FindItemWithText (text, includeSubItemsInSearch, startIndex, true, false);
		}

		public ListViewItem FindItemWithText (string text, bool includeSubItemsInSearch, int startIndex, bool isPrefixSearch)
		{
			return FindItemWithText (text, includeSubItemsInSearch, startIndex, isPrefixSearch, false);
		}
		
		internal ListViewItem FindItemWithText (string text, bool includeSubItemsInSearch, int startIndex, bool isPrefixSearch, bool roundtrip)
		{
			if (startIndex < 0 || startIndex >= items.Count)
				throw new ArgumentOutOfRangeException ("startIndex");

			if (text == null)
				throw new ArgumentNullException ("text");

			if (virtual_mode) {
				SearchForVirtualItemEventArgs args = new SearchForVirtualItemEventArgs (true,
						isPrefixSearch, includeSubItemsInSearch, text, Point.Empty, 
						SearchDirectionHint.Down, startIndex);

				OnSearchForVirtualItem (args);
				int idx = args.Index;
				if (idx >= 0 && idx < virtual_list_size)
					return items [idx];

				return null;
			}

			int i = startIndex;
			while (true) {
				ListViewItem lvi = items [i];

				if (isPrefixSearch) { // prefix search
					if (CultureInfo.CurrentCulture.CompareInfo.IsPrefix (lvi.Text, text, CompareOptions.IgnoreCase))
					       	return lvi;
				} else if (String.Compare (lvi.Text, text, true) == 0) // match
					return lvi;

				if (i + 1 >= items.Count) {
					if (!roundtrip)
						break;

					i = 0;
				} else 
					i++;

				if (i == startIndex)
					break;
			}

			// Subitems have a minor priority, so we have to do a second linear search
			// Also, we don't need to to a roundtrip search for them by now
			if (includeSubItemsInSearch) {
				for (i = startIndex; i < items.Count; i++) {
					ListViewItem lvi = items [i];
					foreach (ListViewItem.ListViewSubItem sub_item in lvi.SubItems)
						if (isPrefixSearch) {
							if (CultureInfo.CurrentCulture.CompareInfo.IsPrefix (sub_item.Text, 
								text, CompareOptions.IgnoreCase))
								return lvi;
						} else if (String.Compare (sub_item.Text, text, true) == 0)
							return lvi;
				}
			}

			return null;
		}

		public ListViewItem FindNearestItem (SearchDirectionHint searchDirection, int x, int y)
		{
			return FindNearestItem (searchDirection, new Point (x, y));
		}

		public ListViewItem FindNearestItem (SearchDirectionHint dir, Point point)
		{
			if (dir < SearchDirectionHint.Left || dir > SearchDirectionHint.Down)
				throw new ArgumentOutOfRangeException ("searchDirection");

			if (view != View.LargeIcon && view != View.SmallIcon)
				throw new InvalidOperationException ();

			if (virtual_mode) {
				SearchForVirtualItemEventArgs args = new SearchForVirtualItemEventArgs (false,
						false, false, String.Empty, point, 
						dir, 0);

				OnSearchForVirtualItem (args);
				int idx = args.Index;
				if (idx >= 0 && idx < virtual_list_size)
					return items [idx];

				return null;
			}

			ListViewItem item = null;
			int min_dist = Int32.MaxValue;

			//
			// It looks like .Net does a previous adjustment
			//
			switch (dir) {
				case SearchDirectionHint.Up:
					point.Y -= item_size.Height;
					break;
				case SearchDirectionHint.Down:
					point.Y += item_size.Height;
					break;
				case SearchDirectionHint.Left:
					point.X -= item_size.Width;
					break;
				case SearchDirectionHint.Right:
					point.X += item_size.Width;
					break;
			}

			for (int i = 0; i < items.Count; i++) {
				Point item_loc = GetItemLocation (i);

				if (dir == SearchDirectionHint.Up) {
					if (point.Y < item_loc.Y)
						continue;
				} else if (dir == SearchDirectionHint.Down) {
					if (point.Y > item_loc.Y)
						continue;
				} else if (dir == SearchDirectionHint.Left) {
					if (point.X < item_loc.X)
						continue;
				} else if (dir == SearchDirectionHint.Right) {
					if (point.X > item_loc.X)
						continue;
				}

				int x_dist = point.X - item_loc.X;
				int y_dist = point.Y - item_loc.Y;

				int dist = x_dist * x_dist  + y_dist * y_dist;
				if (dist < min_dist) {
					item = items [i];
					min_dist = dist;
				}
			}

			return item;
		}
		
		public ListViewItem GetItemAt (int x, int y)
		{
			Size item_size = ItemSize;
			for (int i = 0; i < items.Count; i++) {
				Point item_location = GetItemLocation (i);
				Rectangle item_rect = new Rectangle (item_location, item_size);
				if (item_rect.Contains (x, y))
					return items [i];
			}

			return null;
		}

		public Rectangle GetItemRect (int index)
		{
			return GetItemRect (index, ItemBoundsPortion.Entire);
		}

		public Rectangle GetItemRect (int index, ItemBoundsPortion portion)
		{
			if (index < 0 || index >= items.Count)
				throw new IndexOutOfRangeException ("index");

			return items [index].GetBounds (portion);
		}

		public ListViewHitTestInfo HitTest (Point point)
		{
			return HitTest (point.X, point.Y);
		}

		public ListViewHitTestInfo HitTest (int x, int y)
		{
			if (x < 0)
				throw new ArgumentOutOfRangeException ("x");
			if (y < 0)
				throw new ArgumentOutOfRangeException ("y");

			ListViewItem item = GetItemAt (x, y);
			if (item == null)
				return new ListViewHitTestInfo (null, null, ListViewHitTestLocations.None);

			ListViewHitTestLocations locations = 0;
			if (item.GetBounds (ItemBoundsPortion.Label).Contains (x, y))
				locations |= ListViewHitTestLocations.Label;
			else if (item.GetBounds (ItemBoundsPortion.Icon).Contains (x, y))
				locations |= ListViewHitTestLocations.Image;
			else if (item.CheckRectReal.Contains (x, y))
				locations |= ListViewHitTestLocations.StateImage;

			ListViewItem.ListViewSubItem subitem = null;
			if (view == View.Details)
				foreach (ListViewItem.ListViewSubItem si in item.SubItems)
					if (si.Bounds.Contains (x, y)) {
						subitem = si;
						break;
					}

			return new ListViewHitTestInfo (item, subitem, locations);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public void RedrawItems (int startIndex, int endIndex, bool invalidateOnly)
		{
			if (startIndex < 0 || startIndex >= items.Count)
				throw new ArgumentOutOfRangeException ("startIndex");
			if (endIndex < 0 || endIndex >= items.Count)
				throw new ArgumentOutOfRangeException ("endIndex");
			if (startIndex > endIndex)
				throw new ArgumentException ("startIndex");

			if (updating)
				return;

			for (int i = startIndex; i <= endIndex; i++)
				items [i].Invalidate ();

			if (!invalidateOnly)
				Update ();
		}

		public void Sort ()
		{
			if (virtual_mode)
				throw new InvalidOperationException ();

			Sort (true);
		}

		// we need this overload to reuse the logic for sorting, while allowing
		// redrawing to be done by caller or have it done by this method when
		// sorting is really performed
		//
		// ListViewItemCollection's Add and AddRange methods call this overload
		// with redraw set to false, as they take care of redrawing themselves
		// (they even want to redraw the listview if no sort is performed, as 
		// an item was added), while ListView.Sort () only wants to redraw if 
		// sorting was actually performed
		private void Sort (bool redraw)
		{
			if (!IsHandleCreated || item_sorter == null) {
				return;
			}
			
			items.Sort (item_sorter);
			if (redraw)
				this.Redraw (true);
		}

		public override string ToString ()
		{
			int count = this.Items.Count;

			if (count == 0)
				return string.Format ("System.Windows.Forms.ListView, Items.Count: 0");
			else
				return string.Format ("System.Windows.Forms.ListView, Items.Count: {0}, Items[0]: {1}", count, this.Items [0].ToString ());
		}
		#endregion	// Public Instance Methods


		#region Subclasses

		private class ItemComparer : IComparer {
			readonly SortOrder sort_order;

			public ItemComparer (SortOrder sortOrder)
			{
				sort_order = sortOrder;
			}

			public int Compare (object x, object y)
			{
				ListViewItem item_x = x as ListViewItem;
				ListViewItem item_y = y as ListViewItem;
				if (sort_order == SortOrder.Ascending)
					return String.Compare (item_x.Text, item_y.Text);
				else
					return String.Compare (item_y.Text, item_x.Text);
			}
		}

		[ListBindable (false)]
		public class CheckedIndexCollection : IList, ICollection, IEnumerable
		{
			private readonly ListView owner;

			#region Public Constructor
			public CheckedIndexCollection (ListView owner)
			{
				this.owner = owner;
			}
			#endregion	// Public Constructor

			#region Public Properties
			[Browsable (false)]
			public int Count {
				get { return owner.CheckedItems.Count; }
			}

			public bool IsReadOnly {
				get { return true; }
			}

			public int this [int index] {
				get {
					int [] indices = GetIndices ();
					if (index < 0 || index >= indices.Length)
						throw new ArgumentOutOfRangeException ("index");
					return indices [index];
				}
			}

			bool ICollection.IsSynchronized {
				get { return false; }
			}

			object ICollection.SyncRoot {
				get { return this; }
			}

			bool IList.IsFixedSize {
				get { return true; }
			}

			object IList.this [int index] {
				get { return this [index]; }
				set { throw new NotSupportedException ("SetItem operation is not supported."); }
			}
			#endregion	// Public Properties

			#region Public Methods
			public bool Contains (int checkedIndex)
			{
				int [] indices = GetIndices ();
				for (int i = 0; i < indices.Length; i++) {
					if (indices [i] == checkedIndex)
						return true;
				}
				return false;
			}

			public IEnumerator GetEnumerator ()
			{
				int [] indices = GetIndices ();
				return indices.GetEnumerator ();
			}

			void ICollection.CopyTo (Array dest, int index)
			{
				int [] indices = GetIndices ();
				Array.Copy (indices, 0, dest, index, indices.Length);
			}

			int IList.Add (object value)
			{
				throw new NotSupportedException ("Add operation is not supported.");
			}

			void IList.Clear ()
			{
				throw new NotSupportedException ("Clear operation is not supported.");
			}

			bool IList.Contains (object checkedIndex)
			{
				if (!(checkedIndex is int))
					return false;
				return Contains ((int) checkedIndex);
			}

			int IList.IndexOf (object checkedIndex)
			{
				if (!(checkedIndex is int))
					return -1;
				return IndexOf ((int) checkedIndex);
			}

			void IList.Insert (int index, object value)
			{
				throw new NotSupportedException ("Insert operation is not supported.");
			}

			void IList.Remove (object value)
			{
				throw new NotSupportedException ("Remove operation is not supported.");
			}

			void IList.RemoveAt (int index)
			{
				throw new NotSupportedException ("RemoveAt operation is not supported.");
			}

			public int IndexOf (int checkedIndex)
			{
				int [] indices = GetIndices ();
				for (int i = 0; i < indices.Length; i++) {
					if (indices [i] == checkedIndex)
						return i;
				}
				return -1;
			}
			#endregion	// Public Methods

			private int [] GetIndices ()
			{
				ArrayList checked_items = owner.CheckedItems.List;
				int [] indices = new int [checked_items.Count];
				for (int i = 0; i < checked_items.Count; i++) {
					ListViewItem item = (ListViewItem) checked_items [i];
					indices [i] = item.Index;
				}
				return indices;
			}
		}	// CheckedIndexCollection

		[ListBindable (false)]
		public class CheckedListViewItemCollection : IList, ICollection, IEnumerable
		{
			private readonly ListView owner;
			private ArrayList list;

			#region Public Constructor
			public CheckedListViewItemCollection (ListView owner)
			{
				this.owner = owner;
				this.owner.Items.Changed += new CollectionChangedHandler (
					ItemsCollection_Changed);
			}
			#endregion	// Public Constructor

			#region Public Properties
			[Browsable (false)]
			public int Count {
				get {
					if (!owner.CheckBoxes)
						return 0;
					return List.Count;
				}
			}

			public bool IsReadOnly {
				get { return true; }
			}

			public ListViewItem this [int index] {
				get {
					if (owner.VirtualMode)
						throw new InvalidOperationException ();
					ArrayList checked_items = List;
					if (index < 0 || index >= checked_items.Count)
						throw new ArgumentOutOfRangeException ("index");
					return (ListViewItem) checked_items [index];
				}
			}

			public virtual ListViewItem this [string key] {
				get {
					int idx = IndexOfKey (key);
					return idx == -1 ? null : (ListViewItem) List [idx];
				}
			}

			bool ICollection.IsSynchronized {
				get { return false; }
			}

			object ICollection.SyncRoot {
				get { return this; }
			}

			bool IList.IsFixedSize {
				get { return true; }
			}

			object IList.this [int index] {
				get { return this [index]; }
				set { throw new NotSupportedException ("SetItem operation is not supported."); }
			}
			#endregion	// Public Properties

			#region Public Methods
			public bool Contains (ListViewItem item)
			{
				if (!owner.CheckBoxes)
					return false;
				return List.Contains (item);
			}

			public virtual bool ContainsKey (string key)
			{
				return IndexOfKey (key) != -1;
			}

			public void CopyTo (Array dest, int index)
			{
				if (owner.VirtualMode)
					throw new InvalidOperationException ();
				if (!owner.CheckBoxes)
					return;
				List.CopyTo (dest, index);
			}

			public IEnumerator GetEnumerator ()
			{
				if (owner.VirtualMode)
					throw new InvalidOperationException ();
				if (!owner.CheckBoxes)
					return (new ListViewItem [0]).GetEnumerator ();
				return List.GetEnumerator ();
			}

			int IList.Add (object value)
			{
				throw new NotSupportedException ("Add operation is not supported.");
			}

			void IList.Clear ()
			{
				throw new NotSupportedException ("Clear operation is not supported.");
			}

			bool IList.Contains (object item)
			{
				if (!(item is ListViewItem))
					return false;
				return Contains ((ListViewItem) item);
			}

			int IList.IndexOf (object item)
			{
				if (!(item is ListViewItem))
					return -1;
				return IndexOf ((ListViewItem) item);
			}

			void IList.Insert (int index, object value)
			{
				throw new NotSupportedException ("Insert operation is not supported.");
			}

			void IList.Remove (object value)
			{
				throw new NotSupportedException ("Remove operation is not supported.");
			}

			void IList.RemoveAt (int index)
			{
				throw new NotSupportedException ("RemoveAt operation is not supported.");
			}

			public int IndexOf (ListViewItem item)
			{
				if (owner.VirtualMode)
					throw new InvalidOperationException ();
				if (!owner.CheckBoxes)
					return -1;
				return List.IndexOf (item);
			}

			public virtual int IndexOfKey (string key)
			{
				if (owner.VirtualMode)
					throw new InvalidOperationException ();
				if (key == null || key.Length == 0)
					return -1;

				ArrayList checked_items = List;
				for (int i = 0; i < checked_items.Count; i++) {
					ListViewItem item = (ListViewItem) checked_items [i];
					if (String.Compare (key, item.Name, true) == 0)
						return i;
				}

				return -1;
			}
			#endregion	// Public Methods

			internal ArrayList List {
				get {
					if (list == null) {
						list = new ArrayList ();
						foreach (ListViewItem item in owner.Items) {
							if (item.Checked)
								list.Add (item);
						}
					}
					return list;
				}
			}

			internal void Reset ()
			{
				// force re-population of list
				list = null;
			}

			private void ItemsCollection_Changed ()
			{
				Reset ();
			}
		}	// CheckedListViewItemCollection

		[ListBindable (false)]
		public class ColumnHeaderCollection : IList, ICollection, IEnumerable
		{
			internal ArrayList list;
			private ListView owner;

			#region UIA Framework Events 
			//NOTE:
			//	We are using Reflection to add/remove internal events.
			//	Class ListViewProvider uses the events when View is Details.
			//
			//Event used to generate UIA StructureChangedEvent
			static object UIACollectionChangedEvent = new object ();

			internal event CollectionChangeEventHandler UIACollectionChanged {
				add { 
					if (owner != null)
						owner.Events.AddHandler (UIACollectionChangedEvent, value); 
				}
				remove { 
					if (owner != null)
						owner.Events.RemoveHandler (UIACollectionChangedEvent, value); 
				}
			}

			internal void OnUIACollectionChangedEvent (CollectionChangeEventArgs args)
			{
				if (owner == null)
					return;

				CollectionChangeEventHandler eh
					= (CollectionChangeEventHandler) owner.Events [UIACollectionChangedEvent];
				if (eh != null)
					eh (owner, args);
			}

			#endregion UIA Framework Events 

			#region Public Constructor
			public ColumnHeaderCollection (ListView owner)
			{
				list = new ArrayList ();
				this.owner = owner;
			}
			#endregion	// Public Constructor

			#region Public Properties
			[Browsable (false)]
			public int Count {
				get { return list.Count; }
			}

			public bool IsReadOnly {
				get { return false; }
			}

			public virtual ColumnHeader this [int index] {
				get {
					if (index < 0 || index >= list.Count)
						throw new ArgumentOutOfRangeException ("index");
					return (ColumnHeader) list [index];
				}
			}

			public virtual ColumnHeader this [string key] {
				get {
					int idx = IndexOfKey (key);
					if (idx == -1)
						return null;

					return (ColumnHeader) list [idx];
				}
			}

			bool ICollection.IsSynchronized {
				get { return true; }
			}

			object ICollection.SyncRoot {
				get { return this; }
			}

			bool IList.IsFixedSize {
				get { return list.IsFixedSize; }
			}

			object IList.this [int index] {
				get { return this [index]; }
				set { throw new NotSupportedException ("SetItem operation is not supported."); }
			}
			#endregion	// Public Properties

			#region Public Methods
			public virtual int Add (ColumnHeader value)
			{
				int idx = list.Add (value);
				owner.AddColumn (value, idx, true);

				//UIA Framework event: Item Added
				OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, value));

				return idx;
			}

			public virtual ColumnHeader Add (string text, int width, HorizontalAlignment textAlign)
			{
				string str = text;
				ColumnHeader colHeader = new ColumnHeader (this.owner, str, textAlign, width);
				this.Add (colHeader);
				return colHeader;
			}

			public virtual ColumnHeader Add (string text)
			{
				return Add (String.Empty, text);
			}

			public virtual ColumnHeader Add (string text, int width)
			{
				return Add (String.Empty, text, width);
			}

			public virtual ColumnHeader Add (string key, string text)
			{
				ColumnHeader colHeader = new ColumnHeader ();
				colHeader.Name = key;
				colHeader.Text = text;
				Add (colHeader);
				return colHeader;
			}

			public virtual ColumnHeader Add (string key, string text, int width)
			{
				return Add (key, text, width, HorizontalAlignment.Left, -1);
			}

			public virtual ColumnHeader Add (string key, string text, int width, HorizontalAlignment textAlign, int imageIndex)
			{
				ColumnHeader colHeader = new ColumnHeader (key, text, width, textAlign);
				colHeader.ImageIndex = imageIndex;
				Add (colHeader);
				return colHeader;
			}

			public virtual ColumnHeader Add (string key, string text, int width, HorizontalAlignment textAlign, string imageKey)
			{
				ColumnHeader colHeader = new ColumnHeader (key, text, width, textAlign);
				colHeader.ImageKey = imageKey;
				Add (colHeader);
				return colHeader;
			}

			public virtual void AddRange (ColumnHeader [] values)
			{
				foreach (ColumnHeader colHeader in values) {
					int idx = list.Add (colHeader);
					owner.AddColumn (colHeader, idx, false);
				}
				
				owner.Redraw (true);
			}

			public virtual void Clear ()
			{
				foreach (ColumnHeader col in list)
					col.SetListView (null);
				list.Clear ();
				owner.ReorderColumns (new int [0], true);

				//UIA Framework event: Items cleared
				OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Refresh, null));

			}

			public bool Contains (ColumnHeader value)
			{
				return list.Contains (value);
			}

			public virtual bool ContainsKey (string key)
			{
				return IndexOfKey (key) != -1;
			}

			public IEnumerator GetEnumerator ()
			{
				return list.GetEnumerator ();
			}

			void ICollection.CopyTo (Array dest, int index)
			{
				list.CopyTo (dest, index);
			}

			int IList.Add (object value)
			{
				if (! (value is ColumnHeader)) {
					throw new ArgumentException ("Not of type ColumnHeader", "value");
				}

				return this.Add ((ColumnHeader) value);
			}

			bool IList.Contains (object value)
			{
				if (! (value is ColumnHeader)) {
					throw new ArgumentException ("Not of type ColumnHeader", "value");
				}

				return this.Contains ((ColumnHeader) value);
			}

			int IList.IndexOf (object value)
			{
				if (! (value is ColumnHeader)) {
					throw new ArgumentException ("Not of type ColumnHeader", "value");
				}

				return this.IndexOf ((ColumnHeader) value);
			}

			void IList.Insert (int index, object value)
			{
				if (! (value is ColumnHeader)) {
					throw new ArgumentException ("Not of type ColumnHeader", "value");
				}

				this.Insert (index, (ColumnHeader) value);
			}

			void IList.Remove (object value)
			{
				if (! (value is ColumnHeader)) {
					throw new ArgumentException ("Not of type ColumnHeader", "value");
				}

				this.Remove ((ColumnHeader) value);
			}

			public int IndexOf (ColumnHeader value)
			{
				return list.IndexOf (value);
			}

			public virtual int IndexOfKey (string key)
			{
				if (key == null || key.Length == 0)
					return -1;

				for (int i = 0; i < list.Count; i++) {
					ColumnHeader col = (ColumnHeader) list [i];
					if (String.Compare (key, col.Name, true) == 0)
						return i;
				}

				return -1;
			}

			public void Insert (int index, ColumnHeader value)
			{
				// LAMESPEC: MSDOCS say greater than or equal to the value of the Count property
				// but it's really only greater.
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException ("index");

				list.Insert (index, value);
				owner.AddColumn (value, index, true);

				//UIA Framework event: Item added
				OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, value));
			}

			public void Insert (int index, string text)
			{
				Insert (index, String.Empty, text);
			}

			public void Insert (int index, string text, int width)
			{
				Insert (index, String.Empty, text, width);
			}

			public void Insert (int index, string key, string text)
			{
				ColumnHeader colHeader = new ColumnHeader ();
				colHeader.Name = key;
				colHeader.Text = text;
				Insert (index, colHeader);
			}

			public void Insert (int index, string key, string text, int width)
			{
				ColumnHeader colHeader = new ColumnHeader (key, text, width, HorizontalAlignment.Left);
				Insert (index, colHeader);
			}

			public void Insert (int index, string key, string text, int width, HorizontalAlignment textAlign, int imageIndex)
			{
				ColumnHeader colHeader = new ColumnHeader (key, text, width, textAlign);
				colHeader.ImageIndex = imageIndex;
				Insert (index, colHeader);
			}

			public void Insert (int index, string key, string text, int width, HorizontalAlignment textAlign, string imageKey)
			{
				ColumnHeader colHeader = new ColumnHeader (key, text, width, textAlign);
				colHeader.ImageKey = imageKey;
				Insert (index, colHeader);
			}

			public void Insert (int index, string text, int width, HorizontalAlignment textAlign)
			{
				string str = text;
				ColumnHeader colHeader = new ColumnHeader (this.owner, str, textAlign, width);
				this.Insert (index, colHeader);
			}

			public virtual void Remove (ColumnHeader column)
			{
				if (!Contains (column))
					return;

				list.Remove (column);
				column.SetListView (null);

				int rem_display_index = column.InternalDisplayIndex;
				int [] display_indices = new int [list.Count];
				for (int i = 0; i < display_indices.Length; i++) {
					ColumnHeader col = (ColumnHeader) list [i];
					int display_index = col.InternalDisplayIndex;
					if (display_index < rem_display_index) {
						display_indices [i] = display_index;
					} else {
						display_indices [i] = (display_index - 1);
					}
				}

				column.InternalDisplayIndex = -1;
				owner.ReorderColumns (display_indices, true);

				//UIA Framework event: Item Removed
				OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Remove, column));
			}

			public virtual void RemoveByKey (string key)
			{
				int idx = IndexOfKey (key);
				if (idx != -1)
					RemoveAt (idx);
			}

			public virtual void RemoveAt (int index)
			{
				if (index < 0 || index >= list.Count)
					throw new ArgumentOutOfRangeException ("index");

				ColumnHeader col = (ColumnHeader) list [index];
				Remove (col);
			}
			#endregion	// Public Methods
			

		}	// ColumnHeaderCollection

		[ListBindable (false)]
		public class ListViewItemCollection : IList, ICollection, IEnumerable
		{
			private readonly ArrayList list;
			private ListView owner;
			private ListViewGroup group;

			#region UIA Framework Events 
			//NOTE:
			//	We are using Reflection to add/remove internal events.
			//	Class ListViewProvider uses the events.
			//
			//Event used to generate UIA StructureChangedEvent
			static object UIACollectionChangedEvent = new object ();

			internal event CollectionChangeEventHandler UIACollectionChanged {
				add { 
					if (owner != null)
						owner.Events.AddHandler (UIACollectionChangedEvent, value); 
				}
				remove { 
					if (owner != null)
						owner.Events.RemoveHandler (UIACollectionChangedEvent, value); 
				}
			}

			internal void OnUIACollectionChangedEvent (CollectionChangeEventArgs args)
			{
				if (owner == null)
					return;

				CollectionChangeEventHandler eh
					= (CollectionChangeEventHandler) owner.Events [UIACollectionChangedEvent];
				if (eh != null)
					eh (owner, args);
			}

			#endregion UIA Framework Events 

			// The collection can belong to a ListView (main) or to a ListViewGroup (sub-collection)
			// In the later case ListViewItem.ListView never gets modified
			private bool is_main_collection = true;

			#region Public Constructor
			public ListViewItemCollection (ListView owner)
			{
				list = new ArrayList (0);
				this.owner = owner;
			}
			#endregion	// Public Constructor

			internal ListViewItemCollection (ListView owner, ListViewGroup group) : this (owner)
			{
				this.group = group;
				is_main_collection = false;
			}

			#region Public Properties
			[Browsable (false)]
			public int Count {
				get {
					if (owner != null && owner.VirtualMode)
						return owner.VirtualListSize;

					return list.Count; 
				}
			}

			public bool IsReadOnly {
				get { return false; }
			}

			public virtual ListViewItem this [int index] {
				get {
					if (index < 0 || index >= Count)
						throw new ArgumentOutOfRangeException ("index");

					if (owner != null && owner.VirtualMode)
						return RetrieveVirtualItemFromOwner (index);
					return (ListViewItem) list [index];
				}

				set {
					if (index < 0 || index >= Count)
						throw new ArgumentOutOfRangeException ("index");

					if (owner != null && owner.VirtualMode)
						throw new InvalidOperationException ();

					if (list.Contains (value))
						throw new ArgumentException ("An item cannot be added more than once. To add an item again, you need to clone it.", "value");

					if (value.ListView != null && value.ListView != owner)
						throw new ArgumentException ("Cannot add or insert the item '" + value.Text + "' in more than one place. You must first remove it from its current location or clone it.", "value");

					if (is_main_collection)
						value.Owner = owner;
					else {
						if (value.Group != null)
							value.Group.Items.Remove (value);

						value.SetGroup (group);
					}

					//UIA Framework event: Item Replaced
					OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Remove, list [index]));

					list [index] = value;

					CollectionChanged (true);

					//UIA Framework event: Item Replaced
					OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, value));

				}
			}

			public virtual ListViewItem this [string key] {
				get {
					int idx = IndexOfKey (key);
					if (idx == -1)
						return null;

					return this [idx];
				}
			}

			bool ICollection.IsSynchronized {
				get { return true; }
			}

			object ICollection.SyncRoot {
				get { return this; }
			}

			bool IList.IsFixedSize {
				get { return list.IsFixedSize; }
			}

			object IList.this [int index] {
				get { return this [index]; }
				set {
					//UIA Framework event: Item Replaced
					OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Remove, this [index]));

					if (value is ListViewItem)
						this [index] = (ListViewItem) value;
					else
						this [index] = new ListViewItem (value.ToString ());

					OnChange ();
					//UIA Framework event: Item Replaced
					OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, value));
				}
			}
			#endregion	// Public Properties

			#region Public Methods
			public virtual ListViewItem Add (ListViewItem value)
			{
				if (owner != null && owner.VirtualMode)
					throw new InvalidOperationException ();

				AddItem (value);

				// Item is ignored until it has been added to the ListView
				if (is_main_collection || value.ListView != null)
					CollectionChanged (true);

				//UIA Framework event: Item Added
				OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, value));

				return value;
			}

			public virtual ListViewItem Add (string text)
			{
				ListViewItem item = new ListViewItem (text);
				return this.Add (item);
			}

			public virtual ListViewItem Add (string text, int imageIndex)
			{
				ListViewItem item = new ListViewItem (text, imageIndex);
				return this.Add (item);
			}

			public virtual ListViewItem Add (string text, string imageKey)
			{
				ListViewItem item = new ListViewItem (text, imageKey);
				return this.Add (item);
			}

			public virtual ListViewItem Add (string key, string text, int imageIndex)
			{
				ListViewItem item = new ListViewItem (text, imageIndex);
				item.Name = key;
				return this.Add (item);
			}

			public virtual ListViewItem Add (string key, string text, string imageKey)
			{
				ListViewItem item = new ListViewItem (text, imageKey);
				item.Name = key;
				return this.Add (item);
			}

			public void AddRange (ListViewItem [] items)
			{
				if (items == null)
					throw new ArgumentNullException ("Argument cannot be null!", "items");
				if (owner != null && owner.VirtualMode)
					throw new InvalidOperationException ();

				owner.BeginUpdate ();
				
				foreach (ListViewItem item in items) {
					AddItem (item);

					//UIA Framework event: Item Added
					OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, item));
				}

				owner.EndUpdate ();
				
				CollectionChanged (true);
			}

			public void AddRange (ListViewItemCollection items)
			{
				if (items == null)
					throw new ArgumentNullException ("Argument cannot be null!", "items");

				ListViewItem[] itemArray = new ListViewItem[items.Count];
				items.CopyTo (itemArray,0);
				this.AddRange (itemArray);
			}

			public virtual void Clear ()
			{
				if (owner != null && owner.VirtualMode)
					throw new InvalidOperationException ();
				if (is_main_collection && owner != null) {
					owner.SetFocusedItem (-1);
					owner.h_scroll.Value = owner.v_scroll.Value = 0;

					// first remove any item in the groups that *are* part of this LV too
					foreach (ListViewGroup group in owner.groups)
						group.Items.ClearItemsWithSameListView ();
				
					foreach (ListViewItem item in list) {
						owner.item_control.CancelEdit (item);
						item.Owner = null;
					}
				}
				else
					foreach (ListViewItem item in list)
						item.SetGroup (null);

				list.Clear ();
				CollectionChanged (false);

				//UIA Framework event: Items Removed
				OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Refresh, null));

			}

			// This method is intended to be used from ListViewGroup.Items, not from ListView.Items,
			// added for performance reasons (avoid calling manually Remove for every item on ListViewGroup.Items)
			void ClearItemsWithSameListView ()
			{
				if (is_main_collection)
					return;

				int counter = list.Count - 1;
				while (counter >= 0) {
					ListViewItem item = list [counter] as ListViewItem;

					// remove only if the items in group have being added to the ListView too
					if (item.ListView == group.ListView) {
						list.RemoveAt (counter);
						item.SetGroup (null);
					}
						
					counter--;
				}
			}

			public bool Contains (ListViewItem item)
			{
				return IndexOf (item) != -1;
			}

			public virtual bool ContainsKey (string key)
			{
				return IndexOfKey (key) != -1;
			}

			public void CopyTo (Array dest, int index)
			{
				list.CopyTo (dest, index);
			}

			public ListViewItem [] Find (string key, bool searchAllSubItems)
			{
				if (key == null)
					return new ListViewItem [0];

				List<ListViewItem> temp_list = new List<ListViewItem> ();
				
				for (int i = 0; i < list.Count; i++) {
					ListViewItem lvi = (ListViewItem) list [i];
					if (String.Compare (key, lvi.Name, true) == 0)
						temp_list.Add (lvi);
				}

				ListViewItem [] retval = new ListViewItem [temp_list.Count];
				temp_list.CopyTo (retval);

				return retval;
			}

			public IEnumerator GetEnumerator ()
			{
				if (owner != null && owner.VirtualMode)
					throw new InvalidOperationException ();

				// This enumerator makes a copy of the collection so
				// it can be deleted from in a foreach
				return new Control.ControlCollection.ControlCollectionEnumerator (list);
			}

			int IList.Add (object item)
			{
				int result;
				ListViewItem li;

				if (owner != null && owner.VirtualMode)
					throw new InvalidOperationException ();

				if (item is ListViewItem) {
					li = (ListViewItem) item;
					if (list.Contains (li))
						throw new ArgumentException ("An item cannot be added more than once. To add an item again, you need to clone it.", "item");

					if (li.ListView != null && li.ListView != owner)
						throw new ArgumentException ("Cannot add or insert the item '" + li.Text + "' in more than one place. You must first remove it from its current location or clone it.", "item");
				}
				else
					li = new ListViewItem (item.ToString ());

				li.Owner = owner;
				
				
				result = list.Add (li);
				CollectionChanged (true);

				//UIA Framework event: Item Added
				OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, li));

				return result;
			}

			bool IList.Contains (object item)
			{
				return Contains ((ListViewItem) item);
			}

			int IList.IndexOf (object item)
			{
				return IndexOf ((ListViewItem) item);
			}

			void IList.Insert (int index, object item)
			{
				if (item is ListViewItem)
					this.Insert (index, (ListViewItem) item);
				else
					this.Insert (index, item.ToString ());

				//UIA Framework event: Item Added
				OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, this [index]));
			}

			void IList.Remove (object item)
			{
				Remove ((ListViewItem) item);
			}

			public int IndexOf (ListViewItem item)
			{
				if (owner != null && owner.VirtualMode) {
					for (int i = 0; i < Count; i++)
						if (RetrieveVirtualItemFromOwner (i) == item)
							return i;

					return -1;
				}
				
				return list.IndexOf (item);
			}

			public virtual int IndexOfKey (string key)
			{
				if (key == null || key.Length == 0)
					return -1;

				for (int i = 0; i < Count; i++) {
					ListViewItem lvi = this [i];
					if (String.Compare (key, lvi.Name, true) == 0)
						return i;
				}

				return -1;
			}

			public ListViewItem Insert (int index, ListViewItem item)
			{
				if (index < 0 || index > list.Count)
					throw new ArgumentOutOfRangeException ("index");

				if (owner != null && owner.VirtualMode)
					throw new InvalidOperationException ();

				if (list.Contains (item))
					throw new ArgumentException ("An item cannot be added more than once. To add an item again, you need to clone it.", "item");

				if (item.ListView != null && item.ListView != owner)
					throw new ArgumentException ("Cannot add or insert the item '" + item.Text + "' in more than one place. You must first remove it from its current location or clone it.", "item");

				if (is_main_collection)
					item.Owner = owner;
				else {
					if (item.Group != null)
						item.Group.Items.Remove (item);

					item.SetGroup (group);
				}

				list.Insert (index, item);

				if (is_main_collection || item.ListView != null)
					CollectionChanged (true);

				// force an update of the selected info if the new item is selected.
				if (item.Selected)
					item.SetSelectedCore (true);
				//UIA Framework event: Item Added
				OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, item));

				return item;
			}

			public ListViewItem Insert (int index, string text)
			{
				return this.Insert (index, new ListViewItem (text));
			}

			public ListViewItem Insert (int index, string text, int imageIndex)
			{
				return this.Insert (index, new ListViewItem (text, imageIndex));
			}

			public ListViewItem Insert (int index, string text, string imageKey)
			{
				ListViewItem lvi = new ListViewItem (text, imageKey);
				return Insert (index, lvi);
			}

			public virtual ListViewItem Insert (int index, string key, string text, int imageIndex)
			{
				ListViewItem lvi = new ListViewItem (text, imageIndex);
				lvi.Name = key;
				return Insert (index, lvi);
			}

			public virtual ListViewItem Insert (int index, string key, string text, string imageKey)
			{
				ListViewItem lvi = new ListViewItem (text, imageKey);
				lvi.Name = key;
				return Insert (index, lvi);
			}

			public virtual void Remove (ListViewItem item)
			{
				if (owner != null && owner.VirtualMode)
					throw new InvalidOperationException ();

				int idx = list.IndexOf (item);
				if (idx != -1)
					RemoveAt (idx);
			}

			public virtual void RemoveAt (int index)
			{
				if (index < 0 || index >= Count)
					throw new ArgumentOutOfRangeException ("index");

				if (owner != null && owner.VirtualMode)
					throw new InvalidOperationException ();

				ListViewItem item = (ListViewItem) list [index];

				bool selection_changed = false;
				if (is_main_collection && owner != null) {

					int display_index = item.DisplayIndex;
					if (item.Focused && display_index + 1 == Count) // Last item
						owner.SetFocusedItem (display_index == 0 ? -1 : display_index - 1);

					selection_changed = owner.SelectedIndices.Contains (index);
					owner.item_control.CancelEdit (item);
				}

				list.RemoveAt (index);

				if (is_main_collection) {
					item.Owner = null;
					if (item.Group != null)
						item.Group.Items.Remove (item);
				} else
					item.SetGroup (null);

				CollectionChanged (false);
				if (selection_changed && owner != null)
					owner.OnSelectedIndexChanged (EventArgs.Empty);


				//UIA Framework event: Item Removed 
				OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Remove, item));
			}

			public virtual void RemoveByKey (string key)
			{
				int idx = IndexOfKey (key);
				if (idx != -1)
					RemoveAt (idx);
			}

			#endregion	// Public Methods

			internal ListView Owner {
				get {
					return owner;
				}
				set {
					owner = value;
				}
			}

			internal ListViewGroup Group {
				get {
					return group;
				}
				set {
					group = value;
				}
			}

			void AddItem (ListViewItem value)
			{
				if (list.Contains (value))
					throw new ArgumentException ("An item cannot be added more than once. To add an item again, you need to clone it.", "value");

				if (value.ListView != null && value.ListView != owner)
					throw new ArgumentException ("Cannot add or insert the item '" + value.Text + "' in more than one place. You must first remove it from its current location or clone it.", "value");
				if (is_main_collection)
					value.Owner = owner;
				else {
					if (value.Group != null)
						value.Group.Items.Remove (value);

					value.SetGroup (group);
				}

				list.Add (value);

				// force an update of the selected info if the new item is selected.
				if (value.Selected)
					value.SetSelectedCore (true);
			}

			void CollectionChanged (bool sort)
			{
				if (owner != null) {
				        if (sort)
				                owner.Sort (false);

					OnChange ();
					owner.Redraw (true);
				}
			}

			ListViewItem RetrieveVirtualItemFromOwner (int displayIndex)
			{
				RetrieveVirtualItemEventArgs args = new RetrieveVirtualItemEventArgs (displayIndex);

				owner.OnRetrieveVirtualItem (args);
				ListViewItem retval = args.Item;
				retval.Owner = owner;
				retval.DisplayIndex = displayIndex;
				retval.Layout ();

				return retval;
			}

			internal event CollectionChangedHandler Changed;

			internal void Sort (IComparer comparer)
			{
				list.Sort (comparer);
				OnChange ();
			}

			internal void OnChange ()
			{
				if (Changed != null)
					Changed ();
			}
		}	// ListViewItemCollection

			
		// In normal mode, the selection information resides in the Items,
		// making SelectedIndexCollection.List read-only
		//
		// In virtual mode, SelectedIndexCollection directly saves the selection
		// information, instead of getting it from Items, making List read-and-write
		[ListBindable (false)]
		public class SelectedIndexCollection : IList, ICollection, IEnumerable
		{
			private readonly ListView owner;
			private ArrayList list;

			#region Public Constructor
			public SelectedIndexCollection (ListView owner)
			{
				this.owner = owner;
				owner.Items.Changed += new CollectionChangedHandler (ItemsCollection_Changed);
			}
			#endregion	// Public Constructor

			#region Public Properties
			[Browsable (false)]
			public int Count {
				get {
					if (!owner.is_selection_available)
						return 0;

					return List.Count;
				}
			}

			public bool IsReadOnly {
				get { 
					return false;
				}
			}

			public int this [int index] {
				get {
					if (!owner.is_selection_available || index < 0 || index >= List.Count)
						throw new ArgumentOutOfRangeException ("index");

					return (int) List [index];
				}
			}

			bool ICollection.IsSynchronized {
				get { return false; }
			}

			object ICollection.SyncRoot {
				get { return this; }
			}

			bool IList.IsFixedSize {
				get { 
					return false;
				}
			}

			object IList.this [int index] {
				get { return this [index]; }
				set { throw new NotSupportedException ("SetItem operation is not supported."); }
			}
			#endregion	// Public Properties

			#region Public Methods
			public int Add (int itemIndex)
			{
				if (itemIndex < 0 || itemIndex >= owner.Items.Count)
					throw new ArgumentOutOfRangeException ("index");

				if (owner.virtual_mode && !owner.is_selection_available)
					return -1;

				owner.Items [itemIndex].Selected = true;

				if (!owner.is_selection_available)
					return 0;

				return List.Count;
			}

			public void Clear ()
			{
				if (!owner.is_selection_available)
					return;

				int [] indexes = (int []) List.ToArray (typeof (int));
				foreach (int index in indexes)
					owner.Items [index].Selected = false;
			}

			public bool Contains (int selectedIndex)
			{
				return IndexOf (selectedIndex) != -1;
			}

			public void CopyTo (Array dest, int index)
			{
				List.CopyTo (dest, index);
			}

			public IEnumerator GetEnumerator ()
			{
				return List.GetEnumerator ();
			}

			int IList.Add (object value)
			{
				throw new NotSupportedException ("Add operation is not supported.");
			}

			void IList.Clear ()
			{
				Clear ();
			}

			bool IList.Contains (object selectedIndex)
			{
				if (!(selectedIndex is int))
					return false;
				return Contains ((int) selectedIndex);
			}

			int IList.IndexOf (object selectedIndex)
			{
				if (!(selectedIndex is int))
					return -1;
				return IndexOf ((int) selectedIndex);
			}

			void IList.Insert (int index, object value)
			{
				throw new NotSupportedException ("Insert operation is not supported.");
			}

			void IList.Remove (object value)
			{
				throw new NotSupportedException ("Remove operation is not supported.");
			}

			void IList.RemoveAt (int index)
			{
				throw new NotSupportedException ("RemoveAt operation is not supported.");
			}

			public int IndexOf (int selectedIndex)
			{
				if (!owner.is_selection_available)
					return -1;

				return List.IndexOf (selectedIndex);
			}

			public void Remove (int itemIndex)
			{
				if (itemIndex < 0 || itemIndex >= owner.Items.Count)
					throw new ArgumentOutOfRangeException ("itemIndex");

				owner.Items [itemIndex].Selected = false;
			}
			#endregion	// Public Methods

			internal ArrayList List {
				get {
					if (list == null) {
						list = new ArrayList ();
						if (!owner.VirtualMode)
						for (int i = 0; i < owner.Items.Count; i++) {
							if (owner.Items [i].Selected)
								list.Add (i);
						}
					}
					return list;
				}
			}

			internal void Reset ()
			{
				// force re-population of list
				list = null;
			}

			private void ItemsCollection_Changed ()
			{
				Reset ();
			}

			internal void RemoveIndex (int index)
			{
				int idx = List.BinarySearch (index);
				if (idx != -1)
					List.RemoveAt (idx);
			}

			// actually store index in the collection
			// also, keep the collection sorted, as .Net does
			internal void InsertIndex (int index)
			{
				int iMin = 0;
				int iMax = List.Count - 1;
				while (iMin <= iMax) {
					int iMid = (iMin + iMax) / 2;
					int current_index = (int) List [iMid];

					if (current_index == index)
						return; // Already added
					if (current_index > index)
						iMax = iMid - 1;
					else
						iMin = iMid + 1;
				}

				List.Insert (iMin, index);
			}

		}	// SelectedIndexCollection

		[ListBindable (false)]
		public class SelectedListViewItemCollection : IList, ICollection, IEnumerable
		{
			private readonly ListView owner;

			#region Public Constructor
			public SelectedListViewItemCollection (ListView owner)
			{
				this.owner = owner;
			}
			#endregion	// Public Constructor

			#region Public Properties
			[Browsable (false)]
			public int Count {
				get {
					return owner.SelectedIndices.Count;
				}
			}

			public bool IsReadOnly {
				get { return true; }
			}

			public ListViewItem this [int index] {
				get {
					if (!owner.is_selection_available || index < 0 || index >= Count)
						throw new ArgumentOutOfRangeException ("index");

					int item_index = owner.SelectedIndices [index];
					return owner.Items [item_index];
				}
			}

			public virtual ListViewItem this [string key] {
				get {
					int idx = IndexOfKey (key);
					if (idx == -1)
						return null;

					return this [idx];
				}
			}

			bool ICollection.IsSynchronized {
				get { return false; }
			}

			object ICollection.SyncRoot {
				get { return this; }
			}

			bool IList.IsFixedSize {
				get { return true; }
			}

			object IList.this [int index] {
				get { return this [index]; }
				set { throw new NotSupportedException ("SetItem operation is not supported."); }
			}
			#endregion	// Public Properties

			#region Public Methods
			public void Clear ()
			{
				owner.SelectedIndices.Clear ();
			}

			public bool Contains (ListViewItem item)
			{
				return IndexOf (item) != -1;
			}

			public virtual bool ContainsKey (string key)
			{
				return IndexOfKey (key) != -1;
			}

			public void CopyTo (Array dest, int index)
			{
				if (!owner.is_selection_available)
					return;
				if (index > Count) // Throws ArgumentException instead of IOOR exception
					throw new ArgumentException ("index");

				for (int i = 0; i < Count; i++)
					dest.SetValue (this [i], index++);
			}

			public IEnumerator GetEnumerator ()
			{
				if (!owner.is_selection_available)
					return (new ListViewItem [0]).GetEnumerator ();

				ListViewItem [] items = new ListViewItem [Count];
				for (int i = 0; i < Count; i++)
					items [i] = this [i];

				return items.GetEnumerator ();
			}

			int IList.Add (object value)
			{
				throw new NotSupportedException ("Add operation is not supported.");
			}

			bool IList.Contains (object item)
			{
				if (!(item is ListViewItem))
					return false;
				return Contains ((ListViewItem) item);
			}

			int IList.IndexOf (object item)
			{
				if (!(item is ListViewItem))
					return -1;
				return IndexOf ((ListViewItem) item);
			}

			void IList.Insert (int index, object value)
			{
				throw new NotSupportedException ("Insert operation is not supported.");
			}

			void IList.Remove (object value)
			{
				throw new NotSupportedException ("Remove operation is not supported.");
			}

			void IList.RemoveAt (int index)
			{
				throw new NotSupportedException ("RemoveAt operation is not supported.");
			}

			public int IndexOf (ListViewItem item)
			{
				if (!owner.is_selection_available)
					return -1;

				for (int i = 0; i < Count; i++)
					if (this [i] == item)
						return i;

				return -1;
			}

			public virtual int IndexOfKey (string key)
			{
				if (!owner.is_selection_available || key == null || key.Length == 0)
					return -1;

				for (int i = 0; i < Count; i++) {
					ListViewItem item = this [i];
					if (String.Compare (item.Name, key, true) == 0)
						return i;
				}

				return -1;
			}
			#endregion	// Public Methods

		}	// SelectedListViewItemCollection

		internal delegate void CollectionChangedHandler ();

		struct ItemMatrixLocation
		{
			int row;
			int col;

			public ItemMatrixLocation (int row, int col)
			{
				this.row = row;
				this.col = col;
		
			}
		
			public int Col {
				get {
					return col;
				}
				set {
					col = value;
				}
			}

			public int Row {
				get {
					return row;
				}
				set {
					row = value;
				}
			}
	
		}

		#endregion // Subclasses
		protected override void OnResize (EventArgs e)
		{
			base.OnResize (e);
		}

		protected override void OnMouseLeave (EventArgs e)
		{
			base.OnMouseLeave (e);
		}

		//
		// ColumnReorder event
		//
		static object ColumnReorderedEvent = new object ();
		public event ColumnReorderedEventHandler ColumnReordered {
			add { Events.AddHandler (ColumnReorderedEvent, value); }
			remove { Events.RemoveHandler (ColumnReorderedEvent, value); }
		}

		protected virtual void OnColumnReordered (ColumnReorderedEventArgs e)
		{
			ColumnReorderedEventHandler creh = (ColumnReorderedEventHandler) (Events [ColumnReorderedEvent]);

			if (creh != null)
				creh (this, e);
		}

		//
		// ColumnWidthChanged
		//
		static object ColumnWidthChangedEvent = new object ();
		public event ColumnWidthChangedEventHandler ColumnWidthChanged {
			add { Events.AddHandler (ColumnWidthChangedEvent, value); }
			remove { Events.RemoveHandler (ColumnWidthChangedEvent, value); }
		}

		protected virtual void OnColumnWidthChanged (ColumnWidthChangedEventArgs e)
		{
			ColumnWidthChangedEventHandler eh = (ColumnWidthChangedEventHandler) (Events[ColumnWidthChangedEvent]);
			if (eh != null)
				eh (this, e);
		}
		
		void RaiseColumnWidthChanged (int resize_column)
		{
			ColumnWidthChangedEventArgs n = new ColumnWidthChangedEventArgs (resize_column);

			OnColumnWidthChanged (n);
		}
		
		//
		// ColumnWidthChanging
		//
		static object ColumnWidthChangingEvent = new object ();
		public event ColumnWidthChangingEventHandler ColumnWidthChanging {
			add { Events.AddHandler (ColumnWidthChangingEvent, value); }
			remove { Events.RemoveHandler (ColumnWidthChangingEvent, value); }
		}

		protected virtual void OnColumnWidthChanging (ColumnWidthChangingEventArgs e)
		{
			ColumnWidthChangingEventHandler cwceh = (ColumnWidthChangingEventHandler) (Events[ColumnWidthChangingEvent]);
			if (cwceh != null)
				cwceh (this, e);
		}
		
		//
		// 2.0 profile based implementation
		//
		bool CanProceedWithResize (ColumnHeader col, int width)
		{
			ColumnWidthChangingEventHandler cwceh = (ColumnWidthChangingEventHandler) (Events[ColumnWidthChangingEvent]);
			if (cwceh == null)
				return true;
			
			ColumnWidthChangingEventArgs changing = new ColumnWidthChangingEventArgs (col.Index, width);
			cwceh (this, changing);
			return !changing.Cancel;
		}

		internal void RaiseColumnWidthChanged (ColumnHeader column)
		{
			int index = Columns.IndexOf (column);
			RaiseColumnWidthChanged (index);
		}

		
		#region UIA Framework: Methods, Properties and Events
		
		static object UIALabelEditChangedEvent = new object ();
		static object UIAShowGroupsChangedEvent = new object ();
		static object UIAMultiSelectChangedEvent = new object ();
		static object UIAViewChangedEvent = new object ();
		static object UIACheckBoxesChangedEvent = new object ();
		static object UIAFocusedItemChangedEvent = new object ();

		internal Rectangle UIAHeaderControl {
			get { return header_control.Bounds; }
		}

		internal int UIAColumns {
			get { return cols; }
		}

		internal int UIARows {
			get { return rows; }
		}

		internal ListViewGroup UIADefaultListViewGroup 
		{
			get { return groups.DefaultGroup; }
		}

		internal ScrollBar UIAHScrollBar {
			get { return h_scroll; }
		}

		internal ScrollBar UIAVScrollBar {
			get { return v_scroll; }
		}

		internal event EventHandler UIAShowGroupsChanged {
			add { Events.AddHandler (UIAShowGroupsChangedEvent, value); }
			remove { Events.RemoveHandler (UIAShowGroupsChangedEvent, value); }
		}

		internal event EventHandler UIACheckBoxesChanged {
			add { Events.AddHandler (UIACheckBoxesChangedEvent, value); }
			remove { Events.RemoveHandler (UIACheckBoxesChangedEvent, value); }
		}

		internal event EventHandler UIAMultiSelectChanged {
			add { Events.AddHandler (UIAMultiSelectChangedEvent, value); }
			remove { Events.RemoveHandler (UIAMultiSelectChangedEvent, value); }
		}

		internal event EventHandler UIALabelEditChanged {
			add { Events.AddHandler (UIALabelEditChangedEvent, value); }
			remove { Events.RemoveHandler (UIALabelEditChangedEvent, value); }
		}

		internal event EventHandler UIAViewChanged {
			add { Events.AddHandler (UIAViewChangedEvent, value); }
			remove { Events.RemoveHandler (UIAViewChangedEvent, value); }
		}

		internal event EventHandler UIAFocusedItemChanged {
			add { Events.AddHandler (UIAFocusedItemChangedEvent, value); }
			remove { Events.RemoveHandler (UIAFocusedItemChangedEvent, value); }
		}

		internal Rectangle UIAGetHeaderBounds (ListViewGroup group)
		{
			return group.HeaderBounds;
		}

		internal int UIAItemsLocationLength
		{
			get { return items_location.Length; }
		}

		private void OnUIACheckBoxesChanged ()
		{
			EventHandler eh = (EventHandler) Events [UIACheckBoxesChangedEvent];
			if (eh != null)
				eh (this, EventArgs.Empty);
		}

		private void OnUIAShowGroupsChanged ()
		{
			EventHandler eh = (EventHandler) Events [UIAShowGroupsChangedEvent];
			if (eh != null)
				eh (this, EventArgs.Empty);
		}

		private void OnUIAMultiSelectChanged ()
		{
			EventHandler eh = (EventHandler) Events [UIAMultiSelectChangedEvent];
			if (eh != null)
				eh (this, EventArgs.Empty);
		}

		private void OnUIALabelEditChanged ()
		{
			EventHandler eh = (EventHandler) Events [UIALabelEditChangedEvent];
			if (eh != null)
				eh (this, EventArgs.Empty);
		}
		
		private void OnUIAViewChanged ()
		{
			EventHandler eh = (EventHandler) Events [UIAViewChangedEvent];
			if (eh != null)
				eh (this, EventArgs.Empty);
		}

		internal void OnUIAFocusedItemChanged ()
		{
			EventHandler eh = (EventHandler) Events [UIAFocusedItemChangedEvent];
			if (eh != null)
				eh (this, EventArgs.Empty);
		}

		#endregion // UIA Framework: Methods, Properties and Events

	}
}
