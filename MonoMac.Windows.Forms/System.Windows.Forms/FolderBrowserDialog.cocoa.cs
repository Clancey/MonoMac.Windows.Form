// 
//  Copyright 2011  James Clancey
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.using System;
using System.Drawing;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	public partial class FolderBrowserDialog : CommonDialog
	{
		public FolderBrowserDialog ()
		{
			CreateHandle();
					
			//okButton.Click += new EventHandler (OnClickOKButton);
			//cancelButton.Click += new EventHandler (OnClickCancelButton);
			//newFolderButton.Click += new EventHandler (OnClickNewFolderButton);

			//form.VisibleChanged += new EventHandler (OnFormVisibleChanged);
			
			RootFolder = rootFolder;
		}
		
		protected override void CreateHandle ()
		{
			//base.CreateHandle ();
			m_helper = new FolderDialogHelper(this);
		}
		
		protected override bool RunDialog (IntPtr hWndOwner)
		{
			m_helper.Show();
			
			return true;
		}
		
		void OnClickOKButton (object sender, EventArgs e)
		{
		//	WriteConfigValues ();
			
			DialogResult = DialogResult.OK;
		}
				
		void OnClickCancelButton (object sender, EventArgs e)
		{
			//WriteConfigValues ();
			
			selectedPath = old_selectedPath;
			DialogResult = DialogResult.Cancel;
		}
		
		
		private string selectedPath 
		{
			get {return m_helper.DirectoryUrl.Path;}
			set {m_helper.DirectoryUrl = new NSUrl(value,true);}
		}
		

	}
}

