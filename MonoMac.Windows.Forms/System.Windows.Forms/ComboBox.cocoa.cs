using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Linq;
using System.Collections.Generic;
namespace System.Windows.Forms
{
	public partial class ComboBox : Control //ListControl 
	{
		internal ComboBoxMouseView m_helper;
		internal override NSView c_helper {
			get {
				return m_helper;
			}
			set {
				m_helper = value as ComboBoxMouseView;
			}
		}
		public ComboBox () : base ()
		{
			m_helper = new ComboBoxMouseView();
			m_helper.Host = this;
			m_helper.Activated += delegate(object sender, EventArgs e) {
				//TODO: implemetn ListControl
				if(SelectedValueChanged != null)
					SelectedValueChanged(sender,e);
				//OnSelectedIndexChanged(e);
				//OnSelectedValueChanged(e);
			};	
			
		}
		internal override void CreateHelper ()
		{m_helper = new ComboBoxMouseView();
			m_helper.Host = this;
			m_helper.Activated += delegate(object sender, EventArgs e) {
				//TODO: implemetn ListControl
				//if(SelectedValueChanged != null)
					SelectedValueChanged(sender,e);
				//OnSelectedIndexChanged(e);
				//OnSelectedValueChanged(e);
			};	
		}
		public event EventHandler SelectedValueChanged;
		
		[Obsolete("Not Implemented.", false)]
		[DefaultValue (ComboBoxStyle.DropDown)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[MWFCategory("Appearance")]
		public ComboBoxStyle DropDownStyle {get;set;}
		public Color BackColor {get;set;}
		private string displayMember;
		public string DisplayMember {get{return displayMember; }set{displayMember = value; _dataSource.DisplayMember = value; }}
		private string valueMember;
		public string ValueMember {get{return valueMember; }set{ valueMember = value; _dataSource.ValueMember = value;}}
		ComboBoxDataSource _dataSource = new ComboBoxDataSource();
		
		[DefaultValue ((string)null)]
		[AttributeProvider (typeof (IListSource))]
		[RefreshProperties (RefreshProperties.Repaint)]
		[MWFCategory("Data")]
		public new object DataSource {
			get {
				return _dataSource.dataArray;
			}
			set {
				m_helper.UsesDataSource = true;
				_dataSource = new ComboBoxDataSource(value){DisplayMember = displayMember};
				m_helper.DataSource = _dataSource;
			}
		}
		

		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		[Bindable(true)]
		public object SelectedItem
		{
			get{return _dataSource.dataArray[m_helper.SelectedIndex];}
			set{m_helper.SelectItem(_dataSource.IndexOfItem(m_helper,value));}
		}
		/*
		public override int SelectedIndex {
			get { return m_helper.SelectedIndex; }
			//set { m_helper.SelectedIndex = value;}
		}
		*/
		
		public bool FormattingEnabled {get;set;}
		
		public object SelectedValue
		{
			get{return _dataSource.GetSelectedValue(m_helper);}
			set {_dataSource.SetSelectedValue(m_helper,value);}
		}
		
		
		[Bindable (true)]
		[Localizable (true)]
		public override string Text {
			get { return m_helper.StringValue; }
			set { m_helper.StringValue = value; }
		}
		
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
		[Localizable (true)]
		[Editor ("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + Consts.AssemblySystem_Design, typeof (System.Drawing.Design.UITypeEditor))]
		[MergableProperty (false)]
		[MWFCategory("Data")]
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

