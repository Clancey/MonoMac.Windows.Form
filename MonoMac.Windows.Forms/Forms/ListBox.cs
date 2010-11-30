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
		private NSTableView tableView;
		private NSTableColumn column;
		private NSString colString = new NSString("ListBox");
		public ListBox () : base ()
		{
	        tableView = new NSTableView();
	
	        tableView.AllowsEmptySelection = true;
			tableView.AllowsMultipleSelection = true;
			tableView.AllowsColumnResizing = true;
			tableView.AllowsColumnSelection = false;
	        //[tableView setFocusRingType:NSFocusRingTypeNone];
	
			column = new NSTableColumn(colString);
			column.DataCell.Editable = false;
			tableView.AddColumn(column);
			tableView.HeaderView = null;
	
			this.AutohidesScrollers  = true;
			this.HasVerticalScroller = true;
			this.HasHorizontalScroller = false;
			//this.AutoresizingMask = NSViewResizingMask.HeightSizable;
			this.DocumentView = tableView;
	
		}
		
		//public virtual NSFont Font {get;set;}
		public virtual NSView CurrentEditor {
			get { return tableView.CurrentEditor;}
		}
		
		public Color BackColor {
			get {
				tableView.BackgroundColor = tableView.BackgroundColor.ColorUsingColorSpaceName(NSColorSpace.CalibratedRGB);
				return  Color.FromArgb( (int)tableView.BackgroundColor.AlphaComponent
				                      ,(int)tableView.BackgroundColor.RedComponent
				                      ,(int)tableView.BackgroundColor.GreenComponent
				                      ,(int)tableView.BackgroundColor.BlueComponent());
			}
			set { tableView.BackgroundColor = NSColor.FromCalibratedRGBA(value.R
			                                                   ,value.G
			                                                   ,value.B
			                                                   ,value.A).ColorUsingColorSpaceName(NSColorSpace.CalibratedRGB);
			}
		}
		public virtual void SetNeedsDisplay()
		{
			tableView.SetNeedsDisplay();
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

