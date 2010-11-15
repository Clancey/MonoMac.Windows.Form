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
		ComboBoxDataSource _dataSource = new ComboBoxDataSource();
		public new object DataSource {
			get {
				return _dataSource.dataArray;
			}
			set {
				UsesDataSource = true;
				_dataSource = new ComboBoxDataSource(value,"","");
				base.DataSource = _dataSource;
			}
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
			
			public ComboBoxDataSource (object theObject,string displayMember,string valueMember)
			{
				if(theObject is IList)
				{
					var inArray = theObject as IList;
					dataArray = inArray.Cast<object>().ToArray();
					if(dataArray[0] is string)
					{
						strings = dataArray.Cast<string>().ToList();	
					}
					else if(!string.IsNullOrEmpty(DisplayMember))
					{
						foreach(var obj in dataArray)
						{
							strings = new List<string>();
							strings.Add(getPropertyValue(obj,DisplayMember));
						}
					}
				}
			}
			
			public ComboBoxDataSource ():base()
			{
			}
			
			//[Export ("numberOfItemsInComboBox:")]
			public override int ItemCount (NSComboBox comboBox)
			{
				return dataArray.Length;
			}
			//[Export ("comboBox:objetValueForItemAtIndex:")]
			public override NSObject ObjectValueForItem (NSComboBox comboBox, int index)
			{
				object l = dataArray[index];
				string theString = "";
				if(l is string)
				{
					theString = l as string;
				}
				else if (!string.IsNullOrEmpty(DisplayMember))
				{
					theString = getPropertyValue(l,DisplayMember);
				}
				return new NSString(theString);
			}
			
			public override int IndexOfItem (NSComboBox comboBox, string value)
			{
				return strings.IndexOf(value);
			}
			
			private string getPropertyValue(object inObject, string propertyName)
			{
				PropertyInfo[] props = inObject.GetType().GetProperties();
				PropertyInfo prop = props.Select(p => p).Where(p =>  p.Name == propertyName).FirstOrDefault();
					if (prop != null)
						return prop.GetValue(inObject,null).ToString();
				return "";
			}
						                  
				                                           
		}     
	}
}

