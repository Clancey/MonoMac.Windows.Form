using System;
using System.Linq;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
namespace System.Windows.Forms
{
	
	public partial class ListBox : Control//: ListBoxMouseView
	{
		internal ListBoxMouseView m_helper;
		internal override NSView c_helper {
			get {
				return m_helper;
			}
			set {
				m_helper = value as ListBoxMouseView;
			}
		}
		public NSTableView tableView;
		public NSTableColumn column;
		private bool layoutSuspened;
		public NSString colString = new NSString("ListBox");
		public ListBox () : base ()
		{

		}
		
		internal override void CreateHelper ()
		{
			m_helper = new ListBoxMouseView();
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
			return lb.c_helper;
		}
		public void BeginUpdate()
		{
			layoutSuspened = true;
		}
		public void EndUpdate()
		{
			layoutSuspened = false;
			refresh();
		}
		public virtual void SetupTable()
		{
	        tableView = new NSTableView();	
	        tableView.AllowsEmptySelection = true;
			tableView.AllowsMultipleSelection = true;
			tableView.AllowsColumnResizing = true;
			tableView.AllowsColumnSelection = false;
			tableView.HeaderView = null;
			tableView.Activated += delegate(object sender, EventArgs e) {
				if(SelectedValueChanged != null)
					SelectedValueChanged(sender,e);
			};
			tableView.DataSource = _dataSource;
			tableView.SizeToFit();
		}
		public virtual void SetupColumn()
		{
			column = new NSTableColumn(colString);
			column.DataCell.Editable = false;
			tableView.AddColumn(column);
		}
		
		//public virtual NSFont Font {get;set;}
		public virtual NSView CurrentEditor {
			get { return tableView.CurrentEditor;}
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
		public Color BackColor {
			get { return tableView.BackgroundColor.ToColor();}
			set { tableView.BackgroundColor = value.ToNSColor();}
		}
		public virtual void SetNeedsDisplay()
		{
			tableView.SetNeedsDisplay();
		}
		private SelectionMode selectionMode;
		public SelectionMode SelectionMode {
			get{return selectionMode;}
			set{selectionMode = value;
				tableView.AllowsMultipleSelection = selectionMode == SelectionMode.MultiExtended ||
													selectionMode == SelectionMode.MultiSimple;
				
			}
		}
		public string DisplayMember {get;set;}
		public string ValueMember {get;set;}
		internal ListboxDataSource _dataSource;
		
		public object DataSource {
			get {
				return _dataSource.dataArray;
			}
			set {
				_dataSource = new ListboxDataSource(this,value);
				tableView.DataSource = _dataSource;
				tableView.SizeToFit();
				//this.DocumentView.Frame = tableView.Frame;
				
			}
		}
		public ObjectCollection Items 
		{
			get{return _dataSource.dataArray;}
			set{_dataSource.dataArray = value;}
		}
		
		public void SetSelected(int index,bool value)
		{
			if(value)
				tableView.SelectRows(new NSIndexSet((uint)index),true);
			else
				tableView.DeselectRow(index);
		}
		
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
		
		public  int SelectedIndex {
			get {
				if(!FormattingEnabled && tableView.SelectedRow == -1)
					return 0;
				return tableView.SelectedRow;
			}
		}
		public bool FormattingEnabled {get;set;}
		
		public void ClearSelected()
		{
			tableView.SelectRows(new NSIndexSet(),false);	
		}
		
		private void refresh()
		{
			if(!layoutSuspened && tableView != null)
				this.tableView.ReloadData();
		}
		public void CollectionChanged()
		{
			refresh();
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
			private NSString returnString;
			public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, int row)
			{
				if(lbox == null)
					return null;
				if(string.IsNullOrEmpty(lbox.DisplayMember))
					returnString = new NSString(dataArray[row].ToString());
				else
					returnString =  new NSString(Util.GetPropertyStringValue(dataArray[row],lbox.DisplayMember));
				if(tableColumn.DataCell is DataGridViewButtonCell)
					(tableColumn.DataCell as DataGridViewButtonCell).Text = returnString;
				return returnString;
				
			}
			public override int GetRowCount (NSTableView tableView)
			{
				if(lbox == null)
					return 0;
				return dataArray.Count;
			}			
		}
		
		
		//[ListBindable (false)]
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
		
		
	}
}

