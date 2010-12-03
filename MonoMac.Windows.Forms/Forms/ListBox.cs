using System;
using System.Linq;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
using System.Collections;
using System.Reflection;
namespace System.Windows.Forms
{
	public partial class ListBox : NSScrollView
	{
		public NSTableView tableView;
		public NSTableColumn column;
		public NSString colString = new NSString("ListBox");
		public ListBox () : base ()
		{
			SetupTable();
			SetupColumn();
			this.AutohidesScrollers  = true;
			this.HasVerticalScroller = true;
			this.HasHorizontalScroller = false;
			//this.AutoresizingMask = NSViewResizingMask.HeightSizable;
			this.DocumentView = tableView;
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
		ListboxDataSource _dataSource = new ListboxDataSource();
		public new object DataSource {
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
		
		public void ClearSelected()
		{
			tableView.SelectRows(new NSIndexSet(),false);	
		}
		
		
		
		#region events
		public EventHandler SelectedValueChanged {get;set;}
		
		#endregion
		
		public class ListboxDataSource : NSTableViewDataSource
		{
			public object[] dataArray; 
			private ListBox lbox;
			public ListboxDataSource(ListBox listBox, object theObject)
			{
				lbox = listBox;
				if(theObject is IList)
				{
					dataArray = (theObject as IList).Cast<object>().ToArray();
					
				}
			}
			public ListboxDataSource()
			{
				
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
				return dataArray.Count();
			}			
		}
	}
}

