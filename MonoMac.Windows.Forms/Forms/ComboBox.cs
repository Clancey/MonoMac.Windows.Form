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
	public class ComboBox : NSComboBox
	{
		public ComboBox () : base ()
		{
			
		}
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
			get{return _dataSource.dataArray[this.SelectedIndex];}
			//TODO: set
		}
        public SizeF Size
        {
            get { return this.Frame.Size; }
            set { this.Frame = new RectangleF(this.Frame.Location, value); }
        }

        public PointF Location
        {
            get { return this.Frame.Location; }
            set { this.Frame = new RectangleF(value, this.Frame.Size); }
        }
		
		public new System.Drawing.Font Font
		{
			get {
				return new System.Drawing.Font(base.Font.FontName, base.Font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}
			set {
				base.Font = MonoMac.AppKit.NSFont.FromFontName(value.Name,value.Size);
				
			}
		}
		public object SelectedValue
		{
			get{return _dataSource.GetSelectedValue(this);}
			set {_dataSource.SetSelectedValue(this,value);}
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
							strings.Add(getPropertyStringValue(obj,DisplayMember));
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
			
			public override NSObject ObjectValueForItem (NSComboBox comboBox, int index)
			{
				object l = dataArray[index];
				string theString = "";
				if (!string.IsNullOrEmpty(DisplayMember))
					theString = getPropertyStringValue(l,DisplayMember);
				else
					theString = l.ToString();
					
				return new NSString(theString);
			}
			
			public object GetSelectedValue(NSComboBox comboBox)
			{
				object l = dataArray[comboBox.SelectedIndex];
				if(!string.IsNullOrEmpty(DisplayMember))
				{
					//Use Display Property if they didnt set ValueMember
					var valueMember = string.IsNullOrEmpty(ValueMember) ? DisplayMember : ValueMember;
					return getPropertyValue(l,valueMember);
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
				var found = dataArray.Where(x=> getPropertyValue(x,valueMember) == value).FirstOrDefault();
				combobox.SelectItem(dataArray.ToList().IndexOf(found));
				
			}
			
			public override int IndexOfItem (NSComboBox comboBox, string value)
			{
				return strings.IndexOf(value);
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

