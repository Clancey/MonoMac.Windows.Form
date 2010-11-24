using System;
using System.Linq;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
using System.Collections;
using System.Reflection;
namespace System.Windows.Forms
{
	public class ListBox : NSScrollView
	{
		private NSTableView tableView;
		private NSTableColumn column;
		
		public ListBox () : base ()
		{
	        tableView = new NSTableView();
	
	        tableView.AllowsEmptySelection = true;
			tableView.AllowsMultipleSelection = true;
			tableView.AllowsColumnResizing = true;
			tableView.AllowsColumnSelection = false;
	        //[tableView setFocusRingType:NSFocusRingTypeNone];
	
			column = new NSTableColumn(new NSString(@"Column"));
			column.DataCell.Editable = false;
			tableView.AddColumn(column);
			tableView.HeaderView = null;
	
			this.AutohidesScrollers  = true;
			this.HasVerticalScroller = true;
			this.HasHorizontalScroller = false;
			//this.AutoresizingMask = NSViewResizingMask.HeightSizable;
			this.DocumentView = tableView;
	
    /*
			table = new NSTableView();
			table.AddColumn(column);
			table.SizeToFit();
			this.ContentView.AddSubview(table);
			*/
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
		
		public SizeF Size
		{
			get{ return this.Frame.Size;	}
			set{this.SetFrameSize(value);}
				                       
		}
		
	public PointF Location
		{
			get{ return this.Frame.Location;	}
			set{this.SetFrameOrigin(value);}
				                       
		}
		public class ListboxDataSource : NSTableViewDataSource
		{
			public object[] dataArray; 
			private ListBox lbox;
			public ListboxDataSource(ListBox listBox, object theObject)
			{
				lbox = listBox;
				if(theObject is IList)
				{
					var inArray = theObject as IList;
					dataArray = inArray.Cast<object>().ToArray();
					
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
					returnString =  new NSString(getPropertyStringValue(dataArray[row],lbox.DisplayMember));
				return returnString;
				
			}
			public override int GetRowCount (NSTableView tableView)
			{
				if(lbox == null)
					return 0;
				return dataArray.Count();
			}
			
			private string getPropertyStringValue(object inObject, string propertyName)
			{
				PropertyInfo[] props = inObject.GetType().GetProperties();
				PropertyInfo prop = props.Select(p => p).Where(p =>  p.Name == propertyName).FirstOrDefault();
					if (prop != null)
						return prop.GetValue(inObject,null).ToString();
				return "";
			}
			
			private object getPropertyValue(object inObject, string propertyName)
			{
				PropertyInfo[] props = inObject.GetType().GetProperties();
				PropertyInfo prop = props.Select(p => p).Where(p =>  p.Name == propertyName).FirstOrDefault();
					if (prop != null)
						return prop.GetValue(inObject,null).ToString();
				return null;
			}
			
			
		}
	}
}

