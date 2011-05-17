// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2006 Alexander Olk
//
// Authors:
//	Alexander Olk (alex.olk@googlemail.com)
//	Gert Driesen (drieseng@users.sourceforge.net)
//
//

using System;
using System.Drawing;
using System.ComponentModel;
using System.Resources;
using System.IO;
using System.Collections;

namespace System.Windows.Forms {
	[DefaultEvent ("HelpRequest")]
	[DefaultProperty ("SelectedPath")]
	[Designer ("System.Windows.Forms.Design.FolderBrowserDialogDesigner, " + Consts.AssemblySystem_Design, "System.ComponentModel.Design.IDesigner")]
	public sealed partial class FolderBrowserDialog : CommonDialog
	{
		#region Local Variables
		private Environment.SpecialFolder rootFolder = Environment.SpecialFolder.Desktop;
		private string selectedPath = string.Empty;
		private bool showNewFolderButton = true;
		
		private Label descriptionLabel;
		private Button cancelButton;
		private Button okButton;
		private Button newFolderButton;
		private ContextMenu folderBrowserTreeViewContextMenu;
		private MenuItem newFolderMenuItem;
		
		private string old_selectedPath = string.Empty;
		
		private readonly string folderbrowserdialog_string = "FolderBrowserDialog";
		private readonly string width_string = "Width";
		private readonly string height_string = "Height";
		private readonly string x_string = "X";
		private readonly string y_string = "Y";
		#endregion	// Local Variables
		
		
		#region Public Instance Properties
		[Browsable(true)]
		[DefaultValue("")]
		[Localizable(true)]
		public string Description {
			set {
				descriptionLabel.Text = value;
			}
			
			get {
				return descriptionLabel.Text;
			}
		}
		
		[Browsable(true)]
		[DefaultValue(Environment.SpecialFolder.Desktop)]
		[Localizable(false)]
		[TypeConverter (typeof (SpecialFolderEnumConverter))]
		public Environment.SpecialFolder RootFolder {
			set {
				int v = (int)value;

				Type enumType = typeof (Environment.SpecialFolder);
				if (!Enum.IsDefined (enumType, v))
					throw new InvalidEnumArgumentException (
						"value", v, enumType);
				
				rootFolder = value;
			}
			get {
				return rootFolder;
			}
		}
		
		[Browsable(true)]
		[DefaultValue("")]
		[Editor("System.Windows.Forms.Design.SelectedPathEditor, " + Consts.AssemblySystem_Design, typeof(System.Drawing.Design.UITypeEditor))]
		[Localizable(true)]
		public string SelectedPath {
			set {
				if (value == null)
					value = string.Empty;
				selectedPath = value;
				old_selectedPath = value;
			}
			get {
				return selectedPath;
			}
		}
		
		[Browsable(true)]
		[DefaultValue(true)]
		[Localizable(false)]
		public bool ShowNewFolderButton {
			set {
				if (value != showNewFolderButton) {
					newFolderButton.Visible = value;
					showNewFolderButton = value;
				}
			}
			
			get {
				return showNewFolderButton;
			}
		}
		#endregion	// Public Instance Properties
		
		#region Public Instance Methods
		public override void Reset ()
		{
			Description = string.Empty;
			RootFolder = Environment.SpecialFolder.Desktop;
			selectedPath = string.Empty;
			ShowNewFolderButton = true;
		}
		
	
		#endregion	// Public Instance Methods
		
		#region Internal Methods
			

		void OnClickNewFolderButton (object sender, EventArgs e)
		{
			//folderBrowserTreeView.CreateNewFolder ();
		}


		
		#endregion	// Internal Methods
		
		#region Events
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler HelpRequest {
			add { base.HelpRequest += value; }
			remove { base.HelpRequest -= value; }
		}
		#endregion
		
	}
	
	internal class SpecialFolderEnumConverter : TypeConverter
	{
		public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if ((value == null) || !(value is String))
				return base.ConvertFrom (context, culture, value);

			return Enum.Parse (typeof (Environment.SpecialFolder), (string)value, true);
		}

		public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if ((value == null) || !(value is Environment.SpecialFolder) || (destinationType != typeof (string)))
				return base.ConvertTo (context, culture, value, destinationType);
				
			return ((Environment.SpecialFolder)value).ToString ();
		}
	}
}
