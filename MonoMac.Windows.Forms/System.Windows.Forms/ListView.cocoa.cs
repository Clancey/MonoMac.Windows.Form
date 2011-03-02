using System;
using System.Drawing;
using System.Collections;
namespace System.Windows.Forms
{
	public partial class ListView
	{
		
		#region Public Constructors
		public ListView ()
		{
			background_color = Color.White;//ThemeEngine.Current.ColorWindow;
			groups = new ListViewGroupCollection (this);
			items = new ListViewItemCollection (this);
			items.Changed += new CollectionChangedHandler (OnItemsChanged);
			checked_indices = new CheckedIndexCollection (this);
			checked_items = new CheckedListViewItemCollection (this);
			columns = new ColumnHeaderCollection (this);
			foreground_color = SystemColors.WindowText;
			selected_indices = new SelectedIndexCollection (this);
			selected_items = new SelectedListViewItemCollection (this);
			items_location = new Point [16];
			items_matrix_location = new ItemMatrixLocation [16];
			reordered_items_indices = new int [16];
			item_tooltip = new ToolTip ();
			item_tooltip.Active = false;
			insertion_mark = new ListViewInsertionMark (this);

			InternalBorderStyle = BorderStyle.Fixed3D;

			header_control = new HeaderControl (this);
			header_control.Visible = false;
			Controls.AddImplicit (header_control);

			item_control = new ItemControl (this);
			Controls.AddImplicit (item_control);

			h_marker = v_marker = 0;
			keysearch_tickcnt = 0;

			// scroll bars are disabled initially
			
			h_scroll.Visible = false;
			//h_scroll.ValueChanged += new EventHandler(HorizontalScroller);
			v_scroll.Visible = false;
			//v_scroll.ValueChanged += new EventHandler(VerticalScroller);

			// event handlers
			base.KeyDown += new KeyEventHandler(ListView_KeyDown);
			SizeChanged += new EventHandler (ListView_SizeChanged);
			//GotFocus += new EventHandler (FocusChanged);
			//LostFocus += new EventHandler (FocusChanged);
			//MouseWheel += new MouseEventHandler(ListView_MouseWheel);
			//MouseEnter += new EventHandler (ListView_MouseEnter);
			Invalidated += new InvalidateEventHandler (ListView_Invalidated);

			BackgroundImageTiled = false;

			this.SetStyle (ControlStyles.UserPaint | ControlStyles.StandardClick
				| ControlStyles.UseTextForAccessibility
				, false);
		}
		#endregion	// Public Constructors
		
		#region Private Internal Properties
		internal Size CheckBoxSize {
			get {
				if (this.check_boxes) {
					if (this.state_image_list != null)
						return this.state_image_list.ImageSize;
					else
						return new Size(25,25);//ThemeEngine.Current.ListViewCheckBoxSize;
				}
				return Size.Empty;
			}
		}
		
		
		internal bool UsingGroups {
			get {
				return show_groups && groups.Count > 0 && view != View.List;// && 	Application.VisualStylesEnabled;
			}
		}
		
		
		Size TileItemSize {
			get {
				// Calculate tile size if needed
				// It appears that using Font.Size instead of a SizeF value can give us
				// a slightly better approach to the proportions defined in .Net
				if (tile_size == Size.Empty) {
					int image_w = LargeImageList == null ? 0 : LargeImageList.ImageSize.Width;
					int image_h = LargeImageList == null ? 0 : LargeImageList.ImageSize.Height;
					int w =  (int)Font.Size * 25 + image_w + 4;//(int)Font.Size * ThemeEngine.Current.ListViewTileWidthFactor + image_w + 4;
					int h = image_h;//Math.Max ((int)Font.Size * ThemeEngine.Current.ListViewTileHeightFactor, image_h);
				
					tile_size = new Size (w, h);
				}
			
				return tile_size;
			}
		}
		
		
		
		// Returns the size of biggest item text in a column
		// or the sum of the text and indent count if we are on 2.0
		private Size BiggestItem (int col)
		{
			Size temp = Size.Empty;
			Size ret_size = Size.Empty;
    	
			return ret_size;
		}
		
		
		private void CalculateScrollBars ()
		{
		
		}

		

		#endregion
		
		
		

		#region Internal Methods Properties
		
		internal int FirstVisibleIndex {
			get {
				// there is no item
				if (this.items.Count == 0)
					return 0;
				
				if (h_marker == 0 && v_marker == 0)
					return 0;
				
				Size item_size = ItemSize;
				// In virtual mode we always have fixed positions, and we can infer the positon easily
				if (virtual_mode) {
					int first = 0;
					switch (view) {
						case View.Details:
							first = v_marker / item_size.Height;
							break;
						case View.LargeIcon:
						case View.SmallIcon:
							first = (v_marker / (item_size.Height + y_spacing)) * cols;
							break;
						case View.List:
							first = (h_marker / (item_size.Width * x_spacing)) * rows;
							break;
					}

					if (first >= items.Count)
						first = items.Count;

					return first;
				}
				for (int i = 0; i < items.Count; i++) {
					Rectangle item_rect = new Rectangle (GetItemLocation (i), item_size);
					if (item_rect.Right >= 0 && item_rect.Bottom >= 0)
						return i;
				}

				return 0;
			}
		}

		
		internal int LastVisibleIndex {
			get {
				for (int i = FirstVisibleIndex; i < Items.Count; i++) {
					if (View == View.List || Alignment == ListViewAlignment.Left) {
						if (GetItemLocation (i).X > item_control.ClientRectangle.Right)
							return i - 1;
					} else {
						if (GetItemLocation (i).Y > item_control.ClientRectangle.Bottom)
							return i - 1;
					}
				}
				
				return Items.Count - 1;
			}
		}

		internal void OnSelectedIndexChanged ()
		{
			if (is_selection_available)
				OnSelectedIndexChanged (EventArgs.Empty);
		}

		internal int TotalWidth {
			get { return Math.Max (this.Width, this.layout_wd); }
		}

		internal int TotalHeight {
			get { return Math.Max (this.Height, this.layout_ht); }
		}

		internal void Redraw (bool recalculate)
		{
			// Avoid calculations when control is being updated
			if (updating)
				return;
			// VirtualMode doesn't do any calculations until handle is created
			if (virtual_mode && !IsHandleCreated)
				return;
			//TODO: invalidate
		}

		void InvalidateSelection ()
		{
			foreach (int selected_index in SelectedIndices)
				items [selected_index].Invalidate ();
		}

		const int text_padding = 15;

		const int max_wrap_padding = 30;

		// Sets the size of the biggest item text as per the view
		private void CalcTextSize ()
		{
			// clear the old value
			text_size = Size.Empty;

			if (items.Count == 0)
				return;

			text_size = BiggestItem (0);

			if (view == View.LargeIcon && this.label_wrap) {
				Size temp = Size.Empty;
				if (this.check_boxes)
					temp.Width += 2 * this.CheckBoxSize.Width;
				int icon_w = LargeImageList == null ? 12 : LargeImageList.ImageSize.Width;
				temp.Width += icon_w + max_wrap_padding;
				// wrapping is done for two lines only
				if (text_size.Width > temp.Width) {
					text_size.Width = temp.Width;
					text_size.Height *= 2;
				}
			}
			else if (view == View.List) {
				// in list view max text shown in determined by the
				// control width, even if scolling is enabled.
				int max_wd = this.Width - (this.CheckBoxSize.Width - 2);
				if (this.small_image_list != null)
					max_wd -= this.small_image_list.ImageSize.Width;

				if (text_size.Width > max_wd)
					text_size.Width = max_wd;
			}

			// we do the default settings, if we have got 0's
			if (text_size.Height <= 0)
				text_size.Height = this.Font.Height;
			if (text_size.Width <= 0)
				text_size.Width = this.Width;

			// little adjustment
			text_size.Width += 2;
			text_size.Height += 2;
		}

		private void SetScrollValue (ScrollBar scrollbar, int val)
		{
			int max;
			if (scrollbar == h_scroll)
				max = h_scroll.Maximum - h_scroll.LargeChange + 1;
			else
				max = v_scroll.Maximum - v_scroll.LargeChange + 1;

			if (val > max)
				val = max;
			else if (val < scrollbar.Minimum)
				val = scrollbar.Minimum;

			scrollbar.Value = val;
		}

		private void Scroll (ScrollBar scrollbar, int delta)
		{
			if (delta == 0 || !scrollbar.Visible)
				return;

			SetScrollValue (scrollbar, scrollbar.Value + delta);
		}

		internal int GetReorderedColumnIndex (ColumnHeader column)
		{
			if (reordered_column_indices == null)
				return column.Index;

			for (int i = 0; i < Columns.Count; i++)
				if (reordered_column_indices [i] == column.Index)
					return i;

			return -1;
		}

		internal ColumnHeader GetReorderedColumn (int index)
		{
			if (reordered_column_indices == null)
				return Columns [index];
			else
				return Columns [reordered_column_indices [index]];
		}

		internal void ReorderColumn (ColumnHeader col, int index, bool fireEvent)
		{
			if (fireEvent) {
				ColumnReorderedEventHandler eh = (ColumnReorderedEventHandler) (Events [ColumnReorderedEvent]);
				if (eh != null){
					ColumnReorderedEventArgs args = new ColumnReorderedEventArgs (col.Index, index, col);

					eh (this, args);
					if (args.Cancel) {
						header_control.Invalidate ();
						item_control.Invalidate ();
						return;
					}
				}
			}
			int column_count = Columns.Count;

			if (reordered_column_indices == null) {
				reordered_column_indices = new int [column_count];
				for (int i = 0; i < column_count; i++)
					reordered_column_indices [i] = i;
			}

			if (reordered_column_indices [index] == col.Index)
				return;

			int[] curr = reordered_column_indices;
			int [] result = new int [column_count];
			int curr_idx = 0;
			for (int i = 0; i < column_count; i++) {
				if (curr_idx < column_count && curr [curr_idx] == col.Index)
					curr_idx++;

				if (i == index)
					result [i] = col.Index;
				else
					result [i] = curr [curr_idx++];
			}

			ReorderColumns (result, true);
		}

		internal void ReorderColumns (int [] display_indices, bool redraw)
		{
			reordered_column_indices = display_indices;
			for (int i = 0; i < Columns.Count; i++) {
				ColumnHeader col = Columns [i];
				col.InternalDisplayIndex = reordered_column_indices [i];
			}
			if (redraw && view == View.Details && IsHandleCreated) {
				//LayoutDetails ();
				header_control.Invalidate ();
				item_control.Invalidate ();
			}
		}

		internal void AddColumn (ColumnHeader newCol, int index, bool redraw)
		{
			int column_count = Columns.Count;
			newCol.SetListView (this);

			int [] display_indices = new int [column_count];
			for (int i = 0; i < column_count; i++) {
				ColumnHeader col = Columns [i];
				if (i == index) {
					display_indices [i] = index;
				} else {
					int display_index = col.InternalDisplayIndex;
					if (display_index < index) {
						display_indices [i] = display_index;
					} else {
						display_indices [i] = (display_index + 1);
					}
				}
			}

			ReorderColumns (display_indices, redraw);
			Invalidate ();
		}

		Size LargeIconItemSize
		{
			get {
				int image_w = LargeImageList == null ? 12 : LargeImageList.ImageSize.Width;
				int image_h = LargeImageList == null ? 2 : LargeImageList.ImageSize.Height;
				int h = text_size.Height + 2 + Math.Max (CheckBoxSize.Height, image_h);
				int w = Math.Max (text_size.Width, image_w);

				if (check_boxes)
					w += 2 + CheckBoxSize.Width;

				return new Size (w, h);
			}
		}

		Size SmallIconItemSize {
			get {
				int image_w = SmallImageList == null ? 0 : SmallImageList.ImageSize.Width;
				int image_h = SmallImageList == null ? 0 : SmallImageList.ImageSize.Height;
				int h = Math.Max (text_size.Height, Math.Max (CheckBoxSize.Height, image_h));
				int w = text_size.Width + image_w;

				if (check_boxes)
					w += 2 + CheckBoxSize.Width;

				return new Size (w, h);
			}
		}

		int GetDetailsItemHeight ()
		{
			int item_height;
			int checkbox_height = CheckBoxes ? CheckBoxSize.Height : 0;
			int small_image_height = SmallImageList == null ? 0 : SmallImageList.ImageSize.Height;
			item_height = Math.Max (checkbox_height, text_size.Height);
			item_height = Math.Max (item_height, small_image_height);
			return item_height;
		}

		void SetItemLocation (int index, int x, int y, int row, int col)
		{
			Point old_location = items_location [index];
			if (old_location.X == x && old_location.Y == y)
				return;

			items_location [index] = new Point (x, y);
			items_matrix_location [index] = new ItemMatrixLocation (row, col);

			//
			// Initial position matches item's position in ListViewItemCollection
			//
			reordered_items_indices [index] = index;
		}


		void ShiftItemsPositions (int from, int to, bool forward)
		{
			if (forward) {
				for (int i = to + 1; i > from; i--) {
					reordered_items_indices [i] = reordered_items_indices [i - 1];

					ListViewItem item = items [reordered_items_indices [i]];
					item.Invalidate ();
					item.DisplayIndex = i;
					item.Invalidate ();
				}
			} else {
				for (int i = from - 1; i < to; i++) {
					reordered_items_indices [i] = reordered_items_indices [i + 1];

					ListViewItem item = items [reordered_items_indices [i]];
					item.Invalidate ();
					item.DisplayIndex = i;
					item.Invalidate ();
				}
			}
		}

		internal void ChangeItemLocation (int display_index, Point new_pos)
		{
			/*
			int new_display_index = GetDisplayIndexFromLocation (new_pos);
			if (new_display_index == display_index)
				return;

			int item_index = reordered_items_indices [display_index];
			ListViewItem item = items [item_index];

			bool forward = new_display_index < display_index;
			int index_from, index_to;
			if (forward) {
				index_from = new_display_index;
				index_to = display_index - 1;
			} else {
				index_from = display_index + 1;
				index_to = new_display_index;
			}

			ShiftItemsPositions (index_from, index_to, forward);

			reordered_items_indices [new_display_index] = item_index;

			item.Invalidate ();
			item.DisplayIndex = new_display_index;
			item.Invalidate ();
			*/
		}
		// When using groups, the items with no group assigned
		// belong to the DefaultGroup
		int GetDefaultGroupItems ()
		{
			int count = 0;
			foreach (ListViewItem item in items)
				if (item.Group == null)
					count++;

			return count;
		}

		// cache the spacing to let virtualmode compute the positions on the fly
		int x_spacing;
		int y_spacing;
		int rows;
		int cols;
		int[,] item_index_matrix;

		void CalculateRowsAndCols (Size item_size, bool left_aligned, int x_spacing, int y_spacing)
		{
			Rectangle area = ClientRectangle;

			if (UseCustomColumnWidth)
				CalculateCustomColumnWidth ();
			if (UsingGroups) {
				// When groups are used the alignment is always top-aligned
				rows = 0;
				cols = 0;
				int items = 0;

				groups.DefaultGroup.ItemCount = GetDefaultGroupItems ();
				for (int i = 0; i < groups.InternalCount; i++) {
					ListViewGroup group = groups.GetInternalGroup (i);
					int items_in_group = group.GetActualItemCount ();

					if (items_in_group == 0)
						continue;

					int group_cols = (int) Math.Floor ((double)(area.Width - v_scroll.Width + x_spacing) / (double)(item_size.Width + x_spacing));
					if (group_cols <= 0)
						group_cols = 1;
					int group_rows = (int) Math.Ceiling ((double)items_in_group / (double)group_cols);

					group.starting_row = rows;
					group.rows = group_rows;
					group.starting_item = items;
					group.current_item = 0; // Reset layout

					cols = Math.Max (group_cols, cols);
					rows += group_rows;
					items += items_in_group;
				}
			} else
			{
				// Simple matrix if no groups are used
				if (left_aligned) {
					rows = (int) Math.Floor ((double)(area.Height - h_scroll.Height + y_spacing) / (double)(item_size.Height + y_spacing));
					if (rows <= 0)
						rows = 1;
					cols = (int) Math.Ceiling ((double)items.Count / (double)rows);
				} else {
					if (UseCustomColumnWidth)
						cols = (int) Math.Floor ((double)(area.Width - v_scroll.Width) / (double)(custom_column_width));
					else
						cols = (int) Math.Floor ((double)(area.Width - v_scroll.Width + x_spacing) / (double)(item_size.Width + x_spacing));

					if (cols < 1)
						cols = 1;

					rows = (int) Math.Ceiling ((double)items.Count / (double)cols);
				}
			}

			item_index_matrix = new int [rows, cols];
		}

		// When using custom column width, we look for the minimum one
		void CalculateCustomColumnWidth ()
		{
			int min_width = Int32.MaxValue;
			for (int i = 0; i < columns.Count; i++) {
				int col_width = columns [i].Width;

				if (col_width < min_width)
					min_width = col_width;
			}

			custom_column_width = min_width;
		}

		void LayoutIcons (Size item_size, bool left_aligned, int x_spacing, int y_spacing)
		{
			header_control.Visible = false;
			header_control.Size = Size.Empty;
			item_control.Visible = true;
			item_control.Location = Point.Empty;
			ItemSize = item_size; // Cache item size
			this.x_spacing = x_spacing;
			this.y_spacing = y_spacing;

			if (items.Count == 0)
				return;

			Size sz = item_size;

			CalculateRowsAndCols (sz, left_aligned, x_spacing, y_spacing);

			layout_wd = UseCustomColumnWidth ? cols * custom_column_width : cols * (sz.Width + x_spacing) - x_spacing;
			layout_ht = rows * (sz.Height + y_spacing) - y_spacing;

			if (virtual_mode) { // no actual assignment is needed on items for virtual mode
				item_control.Size = new Size (layout_wd, layout_ht);
				return;
			}

			bool using_groups = UsingGroups;
			if (using_groups) // the groups layout will override layout_ht
				CalculateGroupsLayout (sz, y_spacing, 0);

			int row = 0, col = 0;
			int x = 0, y = 0;
			int display_index = 0;

			for (int i = 0; i < items.Count; i++) {
				ListViewItem item = items [i];
				if (using_groups) {
					ListViewGroup group = item.Group;
					if (group == null)
						group = groups.DefaultGroup;

					Point group_items_loc = group.items_area_location;
					int current_item = group.current_item++;
					int starting_row = group.starting_row;

					display_index = group.starting_item + current_item;
					row = (current_item / cols);
					col = current_item % cols;

					x = UseCustomColumnWidth ? col * custom_column_width : col * (item_size.Width + x_spacing);
					y = row * (item_size.Height + y_spacing) + group_items_loc.Y;

					SetItemLocation (display_index, x, y, row + starting_row, col);
					SetItemAtDisplayIndex (display_index, i);
					item_index_matrix [row + starting_row, col] = i;

				} else
				{
					x = UseCustomColumnWidth ? col * custom_column_width : col * (item_size.Width + x_spacing);
					y = row * (item_size.Height + y_spacing);
					display_index = i; // Same as item index in Items

					SetItemLocation (i, x, y, row, col);
					item_index_matrix [row, col] = i;

					if (left_aligned) {
						row++;
						if (row == rows) {
							row = 0;
							col++;
						}
					} else {
						if (++col == cols) {
							col = 0;
							row++;
						}
					}
				}

				item.Layout ();
				item.DisplayIndex = display_index;
				item.SetPosition (new Point (x, y));
			}

			item_control.Size = new Size (layout_wd, layout_ht);
		}

		void CalculateGroupsLayout (Size item_size, int y_spacing, int y_origin)
		{
			int y = y_origin;
			bool details = view == View.Details;

			for (int i = 0; i < groups.InternalCount; i++) {
				ListViewGroup group = groups.GetInternalGroup (i);
				if (group.ItemCount == 0)
					continue;

				y += LayoutGroupHeader (group, y, item_size.Height, y_spacing, details ? group.ItemCount : group.rows);
			}

			layout_ht = y; // Update height taking into account Groups' headers heights
		}

		int LayoutGroupHeader (ListViewGroup group, int y_origin, int item_height, int y_spacing, int rows)
		{
			Rectangle client_area = ClientRectangle;
			int header_height = Font.Height + 15; // one line height + some padding

			group.HeaderBounds = new Rectangle (0, y_origin, client_area.Width - v_scroll.Width, header_height);
			group.items_area_location = new Point (0, y_origin + header_height);

			int items_area_height = ((item_height + y_spacing) * rows);
			return header_height + items_area_height + 10; // Add a small bottom margin
		}

		void CalculateDetailsGroupItemsCount ()
		{
			int items = 0;

			groups.DefaultGroup.ItemCount = GetDefaultGroupItems ();
			for (int i = 0; i < groups.InternalCount; i++) {
				ListViewGroup group = groups.GetInternalGroup (i);
				int items_in_group = group.GetActualItemCount ();

				if (items_in_group == 0)
					continue;

				group.starting_item = items;
				group.current_item = 0; // Reset layout.
				items += items_in_group;
			}
		}

		internal Point GetItemLocation (int index)
		{
			Point loc = Point.Empty;
			if (virtual_mode)
				loc = GetFixedItemLocation (index);
			else
				loc = items_location [index];

			loc.X -= h_marker; // Adjust to scroll
			loc.Y -= v_marker;

			return loc;
		}

		Point GetFixedItemLocation (int index)
		{
			Point loc = Point.Empty;

			switch (view) {
				case View.LargeIcon:
				case View.SmallIcon:
					loc.X = index % cols * (item_size.Width + x_spacing);
					loc.Y = index / cols * (item_size.Height + y_spacing);
					break;
				case View.List:
					loc.X = index / rows * (item_size.Width + x_spacing);
					loc.Y = index % rows * (item_size.Height + y_spacing);
					break;
				case View.Details:
					loc.Y = header_control.Height + (index * item_size.Height);
					break;
			}

			return loc;
		}

		internal int GetItemIndex (int display_index)
		{
			if (virtual_mode)
				return display_index; // no reordering in virtual mode.
			return reordered_items_indices [display_index];
		}

		internal ListViewItem GetItemAtDisplayIndex (int display_index)
		{
			// in virtual mode there's no reordering at all.
			if (virtual_mode)
				return items [display_index];
			return items [reordered_items_indices [display_index]];
		}

		internal void SetItemAtDisplayIndex (int display_index, int index)
		{
			reordered_items_indices [display_index] = index;
		}

		private bool KeySearchString (KeyEventArgs ke)
		{
			int current_tickcnt = Environment.TickCount;
			if (keysearch_tickcnt > 0 && current_tickcnt - keysearch_tickcnt > keysearch_keydelay) {
				keysearch_text = string.Empty;
			}
			
			if (!Char.IsLetterOrDigit ((char)ke.KeyCode))
				return false;

			keysearch_text += (char)ke.KeyCode;
			keysearch_tickcnt = current_tickcnt;

			int prev_focused = FocusedItem == null ? 0 : FocusedItem.DisplayIndex;
			int start = prev_focused + 1 < Items.Count ? prev_focused + 1 : 0;

			ListViewItem item = FindItemWithText (keysearch_text, false, start, true, true);
			if (item != null && prev_focused != item.DisplayIndex) {
				selected_indices.Clear ();

				SetFocusedItem (item.DisplayIndex);
				item.Selected = true;
				EnsureVisible (GetItemIndex (item.DisplayIndex));
			}

			return true;
		}

		private void OnItemsChanged ()
		{
			ResetSearchString ();
		}

		private void ResetSearchString ()
		{
			keysearch_text = String.Empty;
		}

		int GetAdjustedIndex (Keys key)
		{
			int result = -1;

			if (View == View.Details) {
				switch (key) {
				case Keys.Up:
					result = FocusedItem.DisplayIndex - 1;
					break;
				case Keys.Down:
					result = FocusedItem.DisplayIndex + 1;
					if (result == items.Count)
						result = -1;
					break;
				case Keys.PageDown:
					int last_index = LastVisibleIndex;
					Rectangle item_rect = new Rectangle (GetItemLocation (last_index), ItemSize);
					if (item_rect.Bottom > item_control.ClientRectangle.Bottom)
						last_index--;
					if (FocusedItem.DisplayIndex == last_index) {
						if (FocusedItem.DisplayIndex < Items.Count - 1) {
							int page_size = item_control.Height / ItemSize.Height - 1;
							result = FocusedItem.DisplayIndex + page_size - 1;
							if (result >= Items.Count)
								result = Items.Count - 1;
						}
					} else
						result = last_index;
					break;
				case Keys.PageUp:
					int first_index = FirstVisibleIndex;
					if (GetItemLocation (first_index).Y < 0)
						first_index++;
					if (FocusedItem.DisplayIndex == first_index) {
						if (first_index > 0) {
							int page_size = item_control.Height / ItemSize.Height - 1;
							result = first_index - page_size + 1;
							if (result < 0)
								result = 0;
						}
					} else
						result = first_index;
					break;
				}
				return result;
			}

			if (virtual_mode)
				return GetFixedAdjustedIndex (key);

			ItemMatrixLocation item_matrix_location = items_matrix_location [FocusedItem.DisplayIndex];
			int row = item_matrix_location.Row;
			int col = item_matrix_location.Col;

			int adjusted_index = -1;

			switch (key) {
			case Keys.Left:
				if (col == 0)
					return -1;
				adjusted_index = item_index_matrix [row, col - 1];
				break;

			case Keys.Right:
				if (col == (cols - 1))
					return -1;
				while (item_index_matrix [row, col + 1] == 0) {
					row--;
					if (row < 0)
						return -1;
				}
				adjusted_index = item_index_matrix [row, col + 1];
				break;

			case Keys.Up:
				if (row == 0)
					return -1;
				while (item_index_matrix [row - 1, col] == 0 && row != 1) {
					col--;
					if (col < 0)
						return -1;
				}
				adjusted_index = item_index_matrix [row - 1, col];
				break;

			case Keys.Down:
				if (row == (rows - 1) || row == Items.Count - 1)
					return -1;
				while (item_index_matrix [row + 1, col] == 0) {
					col--;
					if (col < 0)
						return -1;
				}
				adjusted_index = item_index_matrix [row + 1, col];
				break;

			default:
				return -1;
			}

			return items [adjusted_index].DisplayIndex;
		}

		// Used for virtual mode, where items *cannot* be re-arranged
		int GetFixedAdjustedIndex (Keys key)
		{
			int result;

			switch (key) {
				case Keys.Left:
					if (view == View.List)
						result = focused_item_index - rows;
					else
						result = focused_item_index - 1;
					break;
				case Keys.Right:
					if (view == View.List)
						result = focused_item_index + rows;
					else
						result = focused_item_index + 1;
					break;
				case Keys.Up:
					if (view != View.List)
						result = focused_item_index - cols;
					else
						result = focused_item_index - 1;
					break;
				case Keys.Down:
					if (view != View.List)
						result = focused_item_index + cols;
					else
						result = focused_item_index + 1;
					break;
				default:
					return -1;

			}

			if (result < 0 || result >= items.Count)
				result = focused_item_index;

			return result;
		}

		ListViewItem selection_start;

		private bool SelectItems (ArrayList sel_items)
		{
			bool changed = false;
			foreach (ListViewItem item in SelectedItems)
				if (!sel_items.Contains (item)) {
					item.Selected = false;
					changed = true;
				}
			foreach (ListViewItem item in sel_items)
				if (!item.Selected) {
					item.Selected = true;
					changed = true;
				}
			return changed;
		}


		bool HandleNavKeys (Keys key_data)
		{
			if (Items.Count == 0 || !item_control.Visible)
				return false;

			if (FocusedItem == null)
				SetFocusedItem (0);

			switch (key_data) {
			case Keys.End:
				SelectIndex (Items.Count - 1);
				break;

			case Keys.Home:
				SelectIndex (0);
				break;

			case Keys.Left:
			case Keys.Right:
			case Keys.Up:
			case Keys.Down:
			case Keys.PageUp:
			case Keys.PageDown:
				SelectIndex (GetAdjustedIndex (key_data));
				break;

			case Keys.Space:
				SelectIndex (focused_item_index);
				ToggleItemsCheckState ();
				break;
			case Keys.Enter:
				if (selected_indices.Count > 0)
					OnItemActivate (EventArgs.Empty);
				break;

			default:
				return false;
			}

			return true;
		}

		void ToggleItemsCheckState ()
		{
			if (!CheckBoxes)
				return;

			// Don't modify check state if StateImageList has less than 2 elements
			if (StateImageList != null && StateImageList.Images.Count < 2)
				return;

			if (SelectedIndices.Count > 0) {
				for (int i = 0; i < SelectedIndices.Count; i++) {
					ListViewItem item = Items [SelectedIndices [i]];
					item.Checked = !item.Checked;
				}
				return;
			} 
			
			if (FocusedItem != null) {
				FocusedItem.Checked = !FocusedItem.Checked;
				SelectIndex (FocusedItem.Index);
			}
		}

		void SelectIndex (int display_index)
		{
			/*
			if (display_index == -1)
				return;

			if (MultiSelect)
				UpdateMultiSelection (display_index, true);
			else if (!GetItemAtDisplayIndex (display_index).Selected)
				GetItemAtDisplayIndex (display_index).Selected = true;

			SetFocusedItem (display_index);
			EnsureVisible (GetItemIndex (display_index)); // Index in Items collection, not display index
			*/
		}

		private void ListView_KeyDown (object sender, KeyEventArgs ke)
		{
			if (ke.Handled || Items.Count == 0 || !item_control.Visible)
				return;

			if (ke.Alt || ke.Control)
				return;
				
			ke.Handled = KeySearchString (ke);
		}

		private MouseEventArgs TranslateMouseEventArgs (MouseEventArgs args)
		{
			Point loc = PointToClient (Control.MousePosition);
			return new MouseEventArgs (args.Button, args.Clicks, loc.X, loc.Y, args.Delta);
		}

		internal class ItemControl : Control {

			ListView owner;
			ListViewItem clicked_item;
			ListViewItem last_clicked_item;
			bool hover_processed = false;
			bool checking = false;
			ListViewItem prev_hovered_item;
			ListViewItem prev_tooltip_item;
			int clicks;
			Point drag_begin = new Point (-1, -1);
			internal int dragged_item_index = -1;
			
			ListViewLabelEditTextBox edit_text_box;
			internal ListViewItem edit_item;
			LabelEditEventArgs edit_args;

			public ItemControl (ListView owner)
			{
				this.owner = owner;
				this.SetStyle (ControlStyles.DoubleBuffer, true);
				//DoubleClick += new EventHandler(ItemsDoubleClick);
				//MouseDown += new MouseEventHandler(ItemsMouseDown);
				//MouseMove += new MouseEventHandler(ItemsMouseMove);
				//MouseHover += new EventHandler(ItemsMouseHover);
				//MouseUp += new MouseEventHandler(ItemsMouseUp);
			}

			void ItemsDoubleClick (object sender, EventArgs e)
			{
				if (owner.activation == ItemActivation.Standard)
					owner.OnItemActivate (EventArgs.Empty);
			}

			enum BoxSelect {
				None,
				Normal,
				Shift,
				Control
			}

			BoxSelect box_select_mode = BoxSelect.None;
			IList prev_selection;
			Point box_select_start;

			Rectangle box_select_rect;
			internal Rectangle BoxSelectRectangle {
				get { return box_select_rect; }
				set {
					if (box_select_rect == value)
						return;

					InvalidateBoxSelectRect ();
					box_select_rect = value;
					InvalidateBoxSelectRect ();
				}
			}

			void InvalidateBoxSelectRect ()
			{
				if (BoxSelectRectangle.Size.IsEmpty)
					return;

				Rectangle edge = BoxSelectRectangle;
				edge.X -= 1;
				edge.Y -= 1;
				edge.Width += 2;
				edge.Height = 2;
				Invalidate (edge);
				edge.Y = BoxSelectRectangle.Bottom - 1;
				Invalidate (edge);
				edge.Y = BoxSelectRectangle.Y - 1;
				edge.Width = 2;
				edge.Height = BoxSelectRectangle.Height + 2;
				Invalidate (edge);
				edge.X = BoxSelectRectangle.Right - 1;
				Invalidate (edge);
			}

			private Rectangle CalculateBoxSelectRectangle (Point pt)
			{
				int left = Math.Min (box_select_start.X, pt.X);
				int right = Math.Max (box_select_start.X, pt.X);
				int top = Math.Min (box_select_start.Y, pt.Y);
				int bottom = Math.Max (box_select_start.Y, pt.Y);
				return Rectangle.FromLTRB (left, top, right, bottom);
			}

			bool BoxIntersectsItem (int index)
			{
				Rectangle r = new Rectangle (owner.GetItemLocation (index), owner.ItemSize);
				if (owner.View != View.Details) {
					r.X += r.Width / 4;
					r.Y += r.Height / 4;
					r.Width /= 2;
					r.Height /= 2;
				}
				return BoxSelectRectangle.IntersectsWith (r);
			}

			bool BoxIntersectsText (int index)
			{
				Rectangle r = owner.GetItemAtDisplayIndex (index).TextBounds;
				return BoxSelectRectangle.IntersectsWith (r);
			}

			ArrayList BoxSelectedItems {
				get {
					ArrayList result = new ArrayList ();
					for (int i = 0; i < owner.Items.Count; i++) {
						bool intersects;
						// Can't iterate over specific items properties in virtualmode
						if (owner.View == View.Details && !owner.FullRowSelect && !owner.VirtualMode)
							intersects = BoxIntersectsText (i);
						else
							intersects = BoxIntersectsItem (i);

						if (intersects)
							result.Add (owner.GetItemAtDisplayIndex (i));
					}
					return result;
				}
			}

			private bool PerformBoxSelection (Point pt)
			{
				if (box_select_mode == BoxSelect.None)
					return false;

				BoxSelectRectangle = CalculateBoxSelectRectangle (pt);
				
				ArrayList box_items = BoxSelectedItems;

				ArrayList items;

				switch (box_select_mode) {

				case BoxSelect.Normal:
					items = box_items;
					break;

				case BoxSelect.Control:
					items = new ArrayList ();
					foreach (int index in prev_selection)
						if (!box_items.Contains (owner.Items [index]))
							items.Add (owner.Items [index]);
					foreach (ListViewItem item in box_items)
						if (!prev_selection.Contains (item.Index))
							items.Add (item);
					break;

				case BoxSelect.Shift:
					items = box_items;
					foreach (ListViewItem item in box_items)
						prev_selection.Remove (item.Index);
					foreach (int index in prev_selection)
						items.Add (owner.Items [index]);
					break;

				default:
					throw new Exception ("Unexpected Selection mode: " + box_select_mode);
				}

				SuspendLayout ();
				owner.SelectItems (items);
				ResumeLayout ();

				return true;
			}
			
			private void LabelEditFinished (object sender, EventArgs e)
			{
				//EndEdit (edit_item);
			}

			private void LabelEditCancelled (object sender, EventArgs e)
			{
				edit_args.SetLabel (null);
				//EndEdit (edit_item);
			}

			private void LabelTextChanged (object sender, EventArgs e)
			{
				if (edit_args != null)
					edit_args.SetLabel (edit_text_box.Text);
			}

		}
		
		internal class ListViewLabelEditTextBox : TextBox
		{
			int max_width = -1;
			int min_width = -1;
			
			int max_height = -1;
			int min_height = -1;
			
			int old_number_lines = 1;
			
			SizeF text_size_one_char;
			
			public ListViewLabelEditTextBox ()
			{
				min_height = DefaultSize.Height;
				//text_size_one_char = TextRenderer.MeasureString ("B", Font);
			}
			
			public int MaxWidth {
				set {
					if (value < min_width)
						max_width = min_width;
					else
						max_width = value;
				}
			}
			
			public int MaxHeight {
				set {
					if (value < min_height)
						max_height = min_height;
					else
						max_height = value;
				}
			}
			
			public new int Width {
				get {
					return base.Width;
				}
				set {
					min_width = value;
					base.Width = value;
				}
			}
			
			public override Font Font {
				get {
					return base.Font;
				}
				set {
					base.Font = value;
					//text_size_one_char = TextRenderer.MeasureString ("B", Font);
				}
			}
			
			protected override void OnTextChanged (EventArgs e)
			{
				base.OnTextChanged (e);
			}
			
			protected override bool IsInputKey (Keys key_data)
			{
				if ((key_data & Keys.Alt) == 0) {
					switch (key_data & Keys.KeyCode) {
						case Keys.Enter:
							return true;
						case Keys.Escape:
							return true;
					}
				}
				return base.IsInputKey (key_data);
			}
			
			protected override void OnKeyDown (KeyEventArgs e)
			{
				if (!Visible)
					return;

				switch (e.KeyCode) {
				case Keys.Return:
					Visible = false;
					e.Handled = true;
					OnEditingFinished (e);
					break;
				case Keys.Escape:
					Visible = false;
					e.Handled = true;
					OnEditingCancelled (e);
					break;
				}
			}
			
			protected override void OnLostFocus (EventArgs e)
			{
				if (Visible) {
					OnEditingFinished (e);
				}
			}

			protected void OnEditingCancelled (EventArgs e)
			{
				EventHandler eh = (EventHandler)(Events [EditingCancelledEvent]);
				if (eh != null)
					eh (this, e);
			}
			
			protected void OnEditingFinished (EventArgs e)
			{
				EventHandler eh = (EventHandler)(Events [EditingFinishedEvent]);
				if (eh != null)
					eh (this, e);
			}
			
			private void ResizeTextBoxWidth (int new_width)
			{
				if (new_width > max_width)
					base.Width = max_width;
				else 
				if (new_width >= min_width)
					base.Width = new_width;
				else
					base.Width = min_width;
			}
			
			private void ResizeTextBoxHeight (int new_height)
			{
				if (new_height > max_height)
					base.Height = max_height;
				else 
				if (new_height >= min_height)
					base.Height = new_height;
				else
					base.Height = min_height;
			}
			
			public void Reset ()
			{
				max_width = -1;
				min_width = -1;
				
				max_height = -1;
				
				old_number_lines = 1;
				
				Text = String.Empty;
				
				Size = DefaultSize;
			}

			static object EditingCancelledEvent = new object ();
			public event EventHandler EditingCancelled {
				add { Events.AddHandler (EditingCancelledEvent, value); }
				remove { Events.RemoveHandler (EditingCancelledEvent, value); }
			}

			static object EditingFinishedEvent = new object ();
			public event EventHandler EditingFinished {
				add { Events.AddHandler (EditingFinishedEvent, value); }
				remove { Events.RemoveHandler (EditingFinishedEvent, value); }
			}
		}

		internal override void OnPaintInternal (PaintEventArgs pe)
		{
			if (updating)
				return;
				
			CalculateScrollBars ();
		}

		void FocusChanged (object o, EventArgs args)
		{
			if (Items.Count == 0)
				return;

			if (FocusedItem == null)
				SetFocusedItem (0);

			ListViewItem focused_item = FocusedItem;

			if (focused_item.ListView != null) {
				focused_item.Invalidate ();
				focused_item.Layout ();
				focused_item.Invalidate ();
			}
		}

		private void ListView_Invalidated (object sender, InvalidateEventArgs e)
		{
			// When the ListView is invalidated, we need to invalidate
			// the child controls.
			header_control.Invalidate ();
			item_control.Invalidate ();
		}

		private void ListView_MouseEnter (object sender, EventArgs args)
		{
			hover_pending = true; // Need a hover event for every Enter/Leave cycle
		}

		private void ListView_SizeChanged (object sender, EventArgs e)
		{
			Redraw (true);
		}
		
		private void SetFocusedItem (int display_index)
		{
			if (display_index != -1)
				GetItemAtDisplayIndex (display_index).Focused = true;
			else if (focused_item_index != -1 && focused_item_index < items.Count) // Previous focused item
				GetItemAtDisplayIndex (focused_item_index).Focused = false;
			focused_item_index = display_index;
			if (display_index == -1)
				OnUIAFocusedItemChanged ();
				// otherwise the event will have been fired
				// when the ListViewItem's Focused was set
		}
		
		internal override bool IsInputCharInternal (char charCode)
		{
			return true;
		}
		#endregion	// Internal Methods Properties

		
		
		#region	 Protected Properties
		protected override Size DefaultSize {
			//get { return ThemeEngine.Current.ListViewDefaultSize; }
			get { return new Size(100,100); }
		}
		#endregion
		
		#region Proteced Methods
		protected override void OnHandleCreated (EventArgs e)
		{
			base.OnHandleCreated (e);
			//CalculateListView (alignment);
			if (!virtual_mode) // Sorting is not allowed in virtual mode
				Sort ();
		}
		#endregion
		
		
		public override Color BackColor {
			get {
				if (background_color.IsEmpty)
					return Color.White;
				else
					return background_color;
			}
			set { 
				background_color = value;
				item_control.BackColor = value;
			}
		}
		

		public override Color ForeColor {
			get {
				if (foreground_color.IsEmpty)
					return Color.Black;
				else
					return foreground_color;
			}
			set { foreground_color = value; }
		}

	}
}

