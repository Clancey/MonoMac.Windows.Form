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
//    limitations under the License.
using System;
using System.ComponentModel;
namespace System.Windows.Forms
{
	public partial class FileDialog
	{
		
		internal bool BMultiSelect;
		internal bool ReadOnlyChecked;
		internal bool ShowReadOnly;
				
		[DefaultValue("")]
		[System.ComponentModel.Localizable(true)]
		public string Filter {
			get {
				return filter;
			}
			
			set {
				if (value == null) {
					filter = "";
				} else {
				
				UpdateFilters ();
			}
		}
		
	}		
		
		private void CleanupOnClose ()
		{
			WriteConfigValues ();
			
			//Mime.CleanFileCache ();
			
			disable_form_closed_event = true;
		}	
		
		protected override bool RunDialog (IntPtr hWndOwner)
		{
			//TODO: make show
			
			return true;
		}
		
		
		private void SelectFilter ()
		{
			/*
			int filter_to_select = (filterIndex - 1);


			do_not_call_OnSelectedIndexChangedFileTypeComboBox = true;
			fileTypeComboBox.BeginUpdate ();
			fileTypeComboBox.SelectedIndex = filter_to_select;
			fileTypeComboBox.EndUpdate ();
			do_not_call_OnSelectedIndexChangedFileTypeComboBox = false;
			*/
		}
		
		
		private void SetFileAndDirectory (string fname)
		{
			
		}
		
		void OnClickOpenSaveButton (object sender, EventArgs e)
		{
			// for filenames typed or selected by user, enable check for 
			// illegal characters in filename(s)
			checkForIllegalChars = true;


			CancelEventArgs cancelEventArgs = new CancelEventArgs ();

			OnFileOk (cancelEventArgs);

			if (cancelEventArgs.Cancel)
				return;
				
			CleanupOnClose ();
			DialogResult = DialogResult.OK;
		}
		
		void OnClickCancelButton (object sender, EventArgs e)
		{
			//if (restoreDirectory)
			//	mwfFileView.CurrentFolder = restoreDirectoryString;

			CleanupOnClose ();
			
			DialogResult = DialogResult.Cancel;
		}
		
		
		
		void OnSelectedIndexChangedFileTypeComboBox (object sender, EventArgs e)
		{
			//if (do_not_call_OnSelectedIndexChangedFileTypeComboBox) {
			//	do_not_call_OnSelectedIndexChangedFileTypeComboBox = false;
			//	return;
			//}

			UpdateRecentFiles ();

			//mwfFileView.FilterIndex = fileTypeComboBox.SelectedIndex + 1;
		}
		
		void OnSelectedFileChangedFileView (object sender, EventArgs e)
		{
			//fileNameComboBox.Text = mwfFileView.CurrentFSEntry.Name;
		}
		
		void OnSelectedFilesChangedFileView (object sender, EventArgs e)
		{
			//string selectedFiles = mwfFileView.SelectedFilesString;
			//if (selectedFiles != null && selectedFiles.Length != 0)
			//	fileNameComboBox.Text = selectedFiles;
		}
			
		void OnCheckCheckChanged (object sender, EventArgs e)
		{
			//ReadOnlyChecked = readonlyCheckBox.Checked;
		}
		

		private void OnColumnClickFileView (object sender, ColumnClickEventArgs e)
		{
			
		}
		
		
		private void UpdateFilters ()
		{
			//if (fileFilter == null)
			//	fileFilter = new FileFilter ();
			
			//ArrayList filters = fileFilter.FilterArrayList;
			/*
			fileTypeComboBox.BeginUpdate ();
			
			fileTypeComboBox.Items.Clear ();
			
			foreach (FilterStruct fs in filters) {
				fileTypeComboBox.Items.Add (fs.filterName);
			}
			
			fileTypeComboBox.EndUpdate ();
			
			mwfFileView.FilterArrayList = filters;
			*/
		}

		private void UpdateRecentFiles ()
		{
			/*
			fileNameComboBox.Items.Clear ();
			if (configFileNames != null) {
				foreach (string configFileName in configFileNames) {
					if (configFileName == null || configFileName.Trim ().Length == 0)
						continue;
					// add no more than 10 items
					if (fileNameComboBox.Items.Count >= MaxFileNameItems)
						break;
					fileNameComboBox.Items.Add (configFileName);
				}
			}
			*/
		}
		
		private void ResizeAndRelocateForHelpOrReadOnly ()
		{

		}
		
		
		private void WriteConfigValues ()
		{
			
		}
		private void ReadConfigValues ()
		{
			/*
			lastFolder = (string)MWFConfig.GetValue (filedialog_string, lastfolder_string);
			
			if (lastFolder != null && lastFolder.IndexOf ("://") == -1) {
				if (!Directory.Exists (lastFolder)) {
					lastFolder = MWFVFS.DesktopPrefix;
				}
			}
			
			if (InitialDirectory.Length > 0 && Directory.Exists (InitialDirectory))
				lastFolder = InitialDirectory;
			else
				if (lastFolder == null || lastFolder.Length == 0)
					lastFolder = Environment.CurrentDirectory;
			
			if (RestoreDirectory)
				restoreDirectoryString = lastFolder;
				*/
		}
		
		/// <summary>
		/// 
		/// </summary>
		
	}
}

