using System;
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
namespace System.Windows.Forms
{
	public partial class ComboBox : ComboBoxMouseView, IControl
	{
		public ComboBox () : base ()
		{
			this.Activated += delegate(object sender, EventArgs e) {
				if(SelectedIndexChanged != null)
					SelectedIndexChanged(sender,e);
				if(SelectedValueChanged != null)
					SelectedValueChanged(sender,e);
			};	
			
		}
		
		[Obsolete("Not Implemented.", false)]
		public ComboBoxStyle DropDownStyle {get;set;}
		public Color BackColor {get;set;}
		private string displayMember;
		public string DisplayMember {get{return displayMember; }set{displayMember = value; _dataSource.DisplayMember = value; }}
		private string valueMember;
		public string ValueMember {get{return valueMember; }set{ valueMember = value; _dataSource.ValueMember = value;}}
		ComboBoxDataSource _dataSource = new ComboBoxDataSource();
		public new object DataSource {
			get {
				return _dataSource.dataArray;
			}
			set {
				UsesDataSource = true;
				_dataSource = new ComboBoxDataSource(value){DisplayMember = displayMember};
				base.DataSource = _dataSource;
			}
		}
		public object SelectedItem
		{
			get{return _dataSource.dataArray[base.SelectedIndex];}
			set{this.SelectItem(_dataSource.IndexOfItem(this,value));}
		}
		public override int SelectedIndex {
			get {
				if(!FormattingEnabled && base.SelectedIndex == -1)
					return 0;
				return base.SelectedIndex;
			}
		}
		public bool FormattingEnabled {get;set;}
		
		public object SelectedValue
		{
			get{return _dataSource.GetSelectedValue(this);}
			set {_dataSource.SetSelectedValue(this,value);}
		}
		public string Text
		{
			get{return this.StringValue;}
			set{this.StringValue = value;}
		}
		#region Events
		public EventHandler SelectedIndexChanged {get;set;}
		public EventHandler SelectedValueChanged {get;set;}
		#endregion
		
		
		public object[] Items 
		{
			get{return _dataSource.dataArray;}
			set{_dataSource.dataArray = value;}
		}
		public class ComboBoxDataSource : NSComboBoxDataSource
		{
			public object[] dataArray;
			private List<string> strings;
			public string DisplayMember = "";
			public string ValueMember = "";
			
			public ComboBoxDataSource (object[] inArray)
			{
				dataArray = inArray;
			}
			
			public ComboBoxDataSource (object theObject)
			{
				if(theObject is IList)
				{
					var inArray = theObject as IList;
					dataArray = inArray.Cast<object>().ToArray();
					strings = new List<string>();
					foreach(var obj in dataArray)
					{
						if(string.IsNullOrEmpty(DisplayMember))
							strings.Add(obj.ToString());
						else
							strings.Add(Util.GetPropertyStringValue(obj,DisplayMember));
					}
				}
			}
			
			public ComboBoxDataSource ():base()
			{
			}
			
			public override int ItemCount (NSComboBox comboBox)
			{
				return dataArray.Length;
			}
			
			Dictionary<string,NSString> returnValue = new Dictionary<string, NSString>();
			public override NSObject ObjectValueForItem (NSComboBox comboBox, int index)
			{
				object l = dataArray[index];
				string theString = "";
				if (!string.IsNullOrEmpty(DisplayMember))
					theString = Util.GetPropertyStringValue(l,DisplayMember);
				else
					theString = l.ToString();
				if (!returnValue.ContainsKey(theString))					
					returnValue.Add(theString,new NSString(theString));
				var returnString = returnValue[theString];
				return returnString;
			}
			
			public object GetSelectedValue(NSComboBox comboBox)
			{
				object l = dataArray[comboBox.SelectedIndex];
				if(!string.IsNullOrEmpty(DisplayMember))
				{
					//Use Display Property if they didnt set ValueMember
					var valueMember = string.IsNullOrEmpty(ValueMember) ? DisplayMember : ValueMember;
					return Util.GetPropertyValue(l,valueMember);
				}
				else
				{
					return l.ToString();
				}
			}

			public void SetSelectedValue(NSComboBox combobox,object value)
			{
				if(string.IsNullOrEmpty(DisplayMember))
				{
					combobox.SelectItem(dataArray.ToList().IndexOf(value));
					return;
				}
				var valueMember = string.IsNullOrEmpty(ValueMember) ? DisplayMember : ValueMember;
				var found = dataArray.Where(x=> Util.GetPropertyValue(x,valueMember) == value).FirstOrDefault();
				combobox.SelectItem(dataArray.ToList().IndexOf(found));
				
			}
			
			public override int IndexOfItem (NSComboBox comboBox, string value)
			{
				return strings.IndexOf(value);
			}
			public int IndexOfItem(NSComboBox ComboBox,object value)
			{
				return dataArray.ToList().IndexOf(value);
			}
		}     
	}
}

