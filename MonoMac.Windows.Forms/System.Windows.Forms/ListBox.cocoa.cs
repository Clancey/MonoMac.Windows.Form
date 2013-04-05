using System;
using System.Linq;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;

#if MAC64
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
using CGFloat = System.Double;
#else
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
using NSPoint = System.Drawing.PointF;
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using CGFloat = System.Single;
#endif

namespace System.Windows.Forms
{
	
	public partial class ListBox : ListControl//: ListBoxMouseView
	{
		internal ListBoxMouseView m_helper;

    internal TableViewHelper tableView;
		public NSTableColumn column;
		private bool layoutSuspened;
		public NSString colString = new NSString("ListBox");
		
		
		public ListBox () : base()
		{
			items = CreateItemCollection ();
			item_height = -1;
			selected_indices = new SelectedIndexCollection (this);
			selected_items = new SelectedObjectCollection (this);

			requested_height = bounds.Height;
			InternalBorderStyle = BorderStyle.Fixed3D;
			//BackColor = ThemeEngine.Current.ColorWindow;

			/* Vertical scrollbar */
			//TODO: add scroll event handler

			/* Horizontal scrollbar */
			//TODO: add scroll event handler

			/* Events */
			
			SetStyle (ControlStyles.UserPaint, false);

#if NET_2_0
			custom_tab_offsets = new IntegerCollection (this);
#endif
		}
		protected override void CreateHandle ()
		{
			m_helper = new ListBoxMouseView();
      m_view = m_helper;
			m_helper.Host = this;
			_dataSource = new ListboxDataSource(this);
			SetupTable();
			SetupColumn();
			m_helper.AutohidesScrollers  = true;
			m_helper.HasVerticalScroller = true;
			m_helper.HasHorizontalScroller = false;
			//this.AutoresizingMask = NSViewResizingMask.HeightSizable;
			m_helper.DocumentView = tableView;
		}
		
		public static implicit operator NSView(ListBox lb)
		{
			return lb.NSViewForControl;
		}

		public virtual void SetupTable()
		{
	        tableView = new TableViewHelper();
			tableView.Host = this;
	        tableView.AllowsEmptySelection = true;
			tableView.AllowsMultipleSelection = true;
			tableView.AllowsColumnResizing = true;
			tableView.AllowsColumnSelection = false;
			tableView.HeaderView = null;
			tableView.Activated += delegate(object sender, EventArgs e) {
				List<int> newRow = new List<int>();
				
				if(tableView.SelectedRowCount == 1)
					newRow.Add((int)tableView.SelectedRow);
				else if(tableView.SelectedRowCount > 1)
					
				foreach( var row in tableView.SelectedRows.ToArray())
				{
					newRow.Add((int)row);	
				}
				
				selected_indices.Clear();
				foreach(var row in newRow)
				{
					selected_indices.Add(row);	
				}
			};
			tableView.DataSource = _dataSource;
			tableView.SizeToFit();
			
		}
		public virtual void SetupColumn()
		{
			column = new NSTableColumn(colString.ToString());
			column.DataCell.Editable = false;
			tableView.AddColumn(column);
		}
		
		//public virtual NSFont Font {get;set;}
		public virtual NSView CurrentEditor {
			get { return tableView.CurrentEditor;}
		}
		#region Private Properties
		#endregion
		
		
		#region Public Properties
		public override Color BackColor {
			get { return base.BackColor; }
			set {
				if (base.BackColor == value)
					return;
    				base.BackColor = value;
					tableView.BackgroundColor = value.ToNSColor();
				base.Refresh ();	// Careful. Calling the base method is not the same that calling 
			}				// the overriden one that refresh also all the items
		}
		
		
		[DefaultValue (13)]
		[Localizable (true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		public virtual int ItemHeight {
			get {
				if (item_height == -1 || item_height ==0) {
					item_height = (int)tableView.RowHeight;
				}
				return item_height;
			}
			set {
				if (value > 255)
					throw new ArgumentOutOfRangeException ("The ItemHeight property was set beyond 255 pixels");

				explicit_item_height = true;
				if (item_height == value)
					return;

				item_height = value;
				if (IntegralHeight)
					UpdateListBoxBounds ();
				tableView.RowHeight = ItemHeight;
				LayoutListBox ();
			}
		}
		
		
		private void LayoutListBox ()
		{
			if (!IsHandleCreated || suspend_layout)
				return;

			if (MultiColumn)
				LayoutMultiColumn ();
			else
				LayoutSingleColumn ();

			last_visible_index = LastVisibleItem ();
			UpdateScrollBars ();
		}
		
		private void UpdateListBoxBounds()
		{
			tableView.RowHeight = 	ItemHeight;
		}

		
		/*
		private new float AlphaValue {
			get {
				return base.AlphaValue;
			}
			set {
				base.AlphaValue = value;
			}
		}
		private new bool AcceptsFirstMouse (NSEvent theEvent)
		{
			return base.AcceptsFirstMouse (theEvent);
		}
		*/
		
		#endregion
		
		#region Public Methods
		
		public int GetItemHeight (int index)
		{
			if (index < 0 || index >= Items.Count)
				throw new ArgumentOutOfRangeException ("Index of out range");
				
			if (DrawMode == DrawMode.OwnerDrawVariable && IsHandleCreated == true) {
				
				//ItemHeight
			}

			return ItemHeight;
		}
		
		internal override void DrawItemInternal(DrawItemEventArgs e)
		{
			OnDrawItem(e);
		}
		
		protected virtual void OnDrawItem (DrawItemEventArgs e)
		{			
			e.Handled = false;
			switch (DrawMode) {
			case DrawMode.OwnerDrawFixed:
			case DrawMode.OwnerDrawVariable:
				DrawItemEventHandler eh = (DrawItemEventHandler)(Events [DrawItemEvent]);
				if (eh != null)
					eh (this, e);

				break;

			default:
				break;
			}
		}
		
		

		protected override void SetItemsCore (IList value)
		{
			BeginUpdate ();
			try {
				Items.Clear ();
				Items.AddItems (value);
				_dataSource = new ListboxDataSource(this,value);
				tableView.DataSource = _dataSource;
				tableView.SizeToFit();
			} finally {
				EndUpdate ();
			}
		}
		
		private int SnapHeightToIntegral (int height)
		{
			int border = 1;

			height -= (2 * border);
			height -= height % ItemHeight;
			height += (2 * border);

			return height;
		}
		
		#endregion
		
		#region Private Methods
		
		private void LayoutMultiColumn ()
		{
		//	fdf
		}
		
		void UpdateTopItem ()
		{
			tableView.ScrollRowToVisible(top_index);	
		}
		void UpdateScrollBars()
		{
			m_helper.AutohidesScrollers = !ScrollAlwaysVisible;	
		}
		
		#endregion
		
		
		public virtual void SetNeedsDisplay()
		{
			tableView.SetNeedsDisplay();
		}

		[DefaultValue (SelectionMode.One)]
		public virtual SelectionMode SelectionMode {
			get { return selection_mode; }
			set {
				if (!Enum.IsDefined (typeof (SelectionMode), value))
					throw new InvalidEnumArgumentException (string.Format("Enum argument value '{0}' is not valid for SelectionMode", value));

				if (selection_mode == value)
					return;
					
				selection_mode = value;
					
				switch (selection_mode) {
				case SelectionMode.None: 
					SelectedIndices.Clear ();
					break;

				case SelectionMode.One:
					// FIXME: Probably this can be improved
					ArrayList old_selection = (ArrayList) SelectedIndices.List.Clone ();
					for (int i = 1; i < old_selection.Count; i++)
						SelectedIndices.Remove ((int)old_selection [i]);
					break;

				default:
					break;
				}
				tableView.AllowsMultipleSelection = selection_mode == SelectionMode.MultiExtended ||
													selection_mode == SelectionMode.MultiSimple;

#if NET_2_0
				// UIA Framework: Generates SelectionModeChanged event.
				OnUIASelectionModeChangedEvent ();
#endif
			}
		}
		
		
		public string DisplayMember {get;set;}
		public string ValueMember {get;set;}
		internal ListboxDataSource _dataSource;
		
		/*
		public ObjectCollection Items 
		{
			get{return _dataSource.dataArray;}
			set{_dataSource.dataArray = value;}
		}
		*/
		
		public void SetSelected (int index, bool value)
		{
			if (index < 0 || index >= Items.Count)
				throw new ArgumentOutOfRangeException ("Index of out range");

			if (SelectionMode == SelectionMode.None)
				throw new InvalidOperationException ();

			if (value)
			{
				SelectedIndices.Add (index);				
				tableView.SelectRows(new NSIndexSet((uint)index),true);
			}
			else
			{
				SelectedIndices.Remove (index);
				tableView.DeselectRow(index);
			}
		}
		
		/*
		public object[] SelectedItems
		{
			get{
				var selIndex = tableView.SelectedRows;
				List<object> rows = new List<object>();
				for (int i = (int)selIndex.FirstIndex; i <= (int)selIndex.LastIndex;)
				{
					rows.Add(_dataSource.dataArray[i]);
					i = (int)selIndex.IndexGreaterThan((uint)i);
				}
				return rows.ToArray();
			}
		}
		
		public object SelectedItem
		{
			get{return SelectedItems.FirstOrDefault();}	
		}
		*/
		
		[Bindable(true)]
		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public override int SelectedIndex {
			get { 
				if (selected_indices == null)
					return -1;
					
				return selected_indices.Count > 0 ? selected_indices [0] : -1;
			}
			set {
				if (value < -1 || value >= Items.Count)
					throw new ArgumentOutOfRangeException ("Index of out range");

				if (SelectionMode == SelectionMode.None)
					throw new ArgumentException ("cannot call this method if SelectionMode is SelectionMode.None");

				if (value == -1)
					selected_indices.Clear ();
				else
				{
					selected_indices.Add (value);
					switch( SelectionMode)
					{
					case System.Windows.Forms.SelectionMode.One:
						if(tableView.SelectedRow != value)
							tableView.SelectRow((NSUInteger)value,false);
						break;
					default :
						tableView.SelectRow((NSUInteger)value,true);
						break;
					}
				}
			}
		}

		public bool FormattingEnabled {get;set;}
		
		public void ClearSelected()
		{
			selected_indices.Clear ();
			tableView.SelectRows(new NSIndexSet(),false);	
		}
		
		
		private void refresh()
		{
			if(!layoutSuspened && tableView != null)
			{
				this.tableView.ReloadData();
			}
		}
		

		internal virtual void CollectionChanged ()
		{
			if (sorted) 
				Sort (false);
			
			if(Items == null)
				return;
			if (Items.Count == 0) {
				selected_indices.List.Clear ();
				focused_item = -1;
				top_index = 0;
			}
			if (Items.Count <= focused_item)
				focused_item = Items.Count - 1;
			if (!IsHandleCreated || suspend_layout)
				return;
			tableView.DataSource = new ListboxDataSource(this,Items);
			LayoutListBox ();
			refresh();
			base.Refresh ();
		}
		
		
		#region events
		public EventHandler SelectedValueChanged {get;set;}
		
		#endregion
		
		public class ListboxDataSource : NSTableViewDataSource
		{
			public ObjectCollection dataArray; 
			private ListBox lbox;
			private object dataObject;
			public ListboxDataSource(ListBox listBox, object theObject)
			{
				dataObject = theObject;
				lbox = listBox;
				if(theObject is IList)
				{
					dataArray = new ObjectCollection(listBox, (theObject as IList).Cast<object>().ToArray());
				}
			}
			public ListboxDataSource(ListBox listBox)
			{
				lbox = listBox;
				dataArray = new ObjectCollection(listBox, new object[]{});
			}
			public ListboxDataSource(ListBox listBox , ObjectCollection Items)
			{
				lbox = listBox;
				dataArray = Items;
			}
			private NSString returnString;
			public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, NSInteger row)
			{
				if(lbox == null)
					return null;
				if(string.IsNullOrEmpty(lbox.DisplayMember))
					returnString = new NSString(dataArray[(int)row].ToString());
				else
					returnString =  new NSString(Util.GetPropertyStringValue(dataArray[(int)row],lbox.DisplayMember));
				if(tableColumn.DataCell is DataGridViewButtonCell)
					(tableColumn.DataCell as DataGridViewButtonCell).Text = returnString;
				return returnString;
				
			}
			public override NSInteger GetRowCount (NSTableView tableView)
			{
				if(lbox == null)
					return 0;
				return dataArray.Count;
			}			
		}
		
		
		//[ListBindable (false)]
		/*
		public class ObjectCollection : IList, ICollection, IEnumerable
		{
			internal class ListObjectComparer : IComparer
			{
				public int Compare (object a, object b)
				{
					string str1 = a.ToString ();
					string str2 = b.ToString ();
					return str1.CompareTo (str2);
				}
			}

			private ListBox owner;
			internal ArrayList object_items = new ArrayList ();
			
			#region UIA Framework Events 
			//NOTE:
			//	We are using Reflection to add/remove internal events.
			//	Class ListProvider uses the events.
			//
			//Event used to generate UIA StructureChangedEvent
			static object UIACollectionChangedEvent = new object ();



			public ObjectCollection (ListBox owner)
			{
				this.owner = owner;
			}

			public ObjectCollection (ListBox owner, object[] value)
			{
				this.owner = owner;
				AddRange (value);
			}

			public ObjectCollection (ListBox owner,  ObjectCollection value)
			{
				this.owner = owner;
				AddRange (value);
			}
			#endregion

			#region Public Properties
			public int Count {
				get { return object_items.Count; }
			}

			public bool IsReadOnly {
				get { return false; }
			}

			//[Browsable(false)]
			//[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
			public virtual object this [int index] {
				get {
					if (index < 0 || index >= Count)
						throw new ArgumentOutOfRangeException ("Index of out range");

					return object_items[index];
				}
				set {
					if (index < 0 || index >= Count)
						throw new ArgumentOutOfRangeException ("Index of out range");
					if (value == null)
						throw new ArgumentNullException ("value");
						
					
					object_items[index] = value;

				
					owner.CollectionChanged ();
				}
			}

			bool ICollection.IsSynchronized {
				get { return false; }
			}

			object ICollection.SyncRoot {
				get { return this; }
			}

			bool IList.IsFixedSize {
				get { return false; }
			}

			#endregion Public Properties
			
			#region Public Methods
			public int Add (object item)
			{
				int idx;

				idx = AddItem (item);
				owner.CollectionChanged ();
				
				// If we are sorted, the item probably moved indexes, get the real one
				//if (owner.sorted)
					return this.IndexOf (item);
					
				//return idx;
			}

			public void AddRange (object[] items)
			{
				AddItems (items);
			}

			public void AddRange (ObjectCollection value)
			{
				AddItems (value);
			}

			internal void AddItems (IList items)
			{
				if (items == null)
					throw new ArgumentNullException ("items");

				foreach (object mi in items)
					AddItem (mi);

				owner.CollectionChanged ();
			}

			public virtual void Clear ()
			{
				owner.ClearSelected();
				object_items.Clear ();
				owner.CollectionChanged ();
				
				//UIA Framework event: Items list cleared

			}

			public bool Contains (object value)
			{
				if (value == null)
					throw new ArgumentNullException ("value");

				return object_items.Contains (value);
			}

			public void CopyTo (object[] destination, int arrayIndex)
			{
				object [] dest = destination;
				object_items.CopyTo (dest, arrayIndex);
			}

			void ICollection.CopyTo (Array destination, int index)
			{
				Array dest = destination;
				object_items.CopyTo (dest, index);
			}

			public IEnumerator GetEnumerator ()
			{
				return object_items.GetEnumerator ();
			}

			int IList.Add (object item)
			{
				return Add (item);
			}

			public int IndexOf (object value)
			{
				if (value == null)
					throw new ArgumentNullException ("value");

				return object_items.IndexOf (value);
			}

			public void Insert (int index,  object item)
			{
				if (index < 0 || index > Count)
					throw new ArgumentOutOfRangeException ("Index of out range");
				if (item == null)
					throw new ArgumentNullException ("item");
					
				owner.BeginUpdate ();
				object_items.Insert (index, item);
				owner.CollectionChanged ();
				owner.EndUpdate ();
				
				//UIA Framework event: Item Added
				//OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, item));

			}

			public void Remove (object value)
			{
				if (value == null)
					return;

				int index = IndexOf (value);
				if (index != -1)
					RemoveAt (index);
			}

			public void RemoveAt (int index)
			{
				if (index < 0 || index >= Count)
					throw new ArgumentOutOfRangeException ("Index of out range");


				//UIA Framework element removed
				object removed = object_items [index];
				UpdateSelection (index);
				object_items.RemoveAt (index);
				owner.CollectionChanged ();
				
				//UIA Framework event: Item Removed
				//OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Remove, removed));
			}
			#endregion Public Methods

			#region Private Methods
			internal int AddItem (object item)
			{
				if (item == null)
					throw new ArgumentNullException ("item");

				int cnt = object_items.Count;
				object_items.Add (item);

				//UIA Framework event: Item Added
				//OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, item));

				return cnt;
			}

			// we receive the index to be removed
			void UpdateSelection (int removed_index)
			{

			}

			internal void Sort ()
			{
				object_items.Sort (new ListObjectComparer ());
			}

			#endregion Private Methods
		}
		*/
		
		
	}
}

