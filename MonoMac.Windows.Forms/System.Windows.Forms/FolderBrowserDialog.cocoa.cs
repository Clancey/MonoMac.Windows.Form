using System;
using System.Drawing;
namespace System.Windows.Forms
{
	public partial class FolderBrowserDialog : CommonDialog
	{
		public FolderBrowserDialog ()
		{
			CreateHandle();
					
			okButton.Click += new EventHandler (OnClickOKButton);
			cancelButton.Click += new EventHandler (OnClickCancelButton);
			newFolderButton.Click += new EventHandler (OnClickNewFolderButton);

			//form.VisibleChanged += new EventHandler (OnFormVisibleChanged);
			
			RootFolder = rootFolder;
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
		

	}
}

