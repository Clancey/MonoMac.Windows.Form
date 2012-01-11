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
//
//  Alexander Olk	alex.olk@googlemail.com
//  Gert Driesen (drieseng@users.sourceforge.net)
//  Eric Petit (surfzoid2002@yahoo.fr)
//
// TODO:
// Keyboard shortcuts (DEL, F5, F2)
// ??

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Text;
using System.Threading;
using System.Xml;

namespace System.Windows.Forms
{
	#region FileDialog
	[DefaultProperty ("FileName")]
	[DefaultEvent ("FileOk")]
	public abstract partial class FileDialog : CommonDialog
	{
		protected static readonly object EventFileOk = new object ();
		private static int MaxFileNameItems = 10;
		
		internal enum FileDialogType
		{
			OpenFileDialog,
			SaveFileDialog
		}
		
		
		private bool addExtension = true;
		private bool auto_upgrade_enable = true;
		private bool checkFileExists;
		private bool checkPathExists = true;
		private FileDialogCustomPlacesCollection custom_places;
		private string defaultExt;
		private bool dereferenceLinks = true;
		private string[] fileNames;
		private bool checkForIllegalChars = true;
		private string filter = "";
		private int filterIndex = 1;
		private string initialDirectory;
		private bool restoreDirectory;
		private bool showHelp;
		private string title;
		private bool supportMultiDottedExtensions;
		private bool validateNames = true;
		private bool showReadOnly;
		private bool readOnlyChecked;
		private bool disable_form_closed_event;

		internal FileDialogType fileDialogType;
		
		internal FileDialog ()
		{
			/*
			openSaveButton.Click += new EventHandler (OnClickOpenSaveButton);
			cancelButton.Click += new EventHandler (OnClickCancelButton);
			helpButton.Click += new EventHandler (OnClickHelpButton);
			
			smallButtonToolBar.ButtonClick += new ToolBarButtonClickEventHandler (OnClickSmallButtonToolBar);
			
			fileTypeComboBox.SelectedIndexChanged += new EventHandler (OnSelectedIndexChangedFileTypeComboBox);
			
			mwfFileView.SelectedFileChanged += new EventHandler (OnSelectedFileChangedFileView);
			mwfFileView.ForceDialogEnd += new EventHandler (OnForceDialogEndFileView);
			mwfFileView.SelectedFilesChanged += new EventHandler (OnSelectedFilesChangedFileView);
			mwfFileView.ColumnClick += new ColumnClickEventHandler(OnColumnClickFileView);
			
			dirComboBox.DirectoryChanged += new EventHandler (OnDirectoryChangedDirComboBox);
			popupButtonPanel.DirectoryChanged += new EventHandler (OnDirectoryChangedPopupButtonPanel);

			readonlyCheckBox.CheckedChanged += new EventHandler (OnCheckCheckChanged);
			form.FormClosed += new FormClosedEventHandler (OnFileDialogFormClosed);
			custom_places = new FileDialogCustomPlacesCollection ();
			*/
			custom_places = new FileDialogCustomPlacesCollection ();
		}
		
		[DefaultValue(true)]
		public bool AddExtension {
			get {
				return addExtension;
			}
			
			set {
				addExtension = value;
			}
		}
		
		[MonoTODO ("Stub, value not respected")]
		[DefaultValue (true)]
		public bool AutoUpgradeEnabled {
			get { return auto_upgrade_enable; }
			set { auto_upgrade_enable = value; }
		}

		[DefaultValue(false)]
		public virtual bool CheckFileExists {
			get {
				return checkFileExists;
			}
			
			set {
				checkFileExists = value;
			}
		}
		
		[DefaultValue(true)]
		public bool CheckPathExists {
			get {
				return checkPathExists;
			}
			
			set {
				checkPathExists = value;
			}
		}
		
		[MonoTODO ("Stub, collection not used")]
		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public FileDialogCustomPlacesCollection CustomPlaces {
			get { return custom_places; }
		}

		[DefaultValue("")]
		public string DefaultExt {
			get {
				if (defaultExt == null)
					return string.Empty;
				return defaultExt;
			}
			set {
				if (value != null && value.Length > 0) {
					// remove leading dot
					if (value [0] == '.')
						value = value.Substring (1);
				}
				defaultExt = value;
			}
		}
		
		// in MS.NET it doesn't make a difference if
		// DerefenceLinks is true or false
		// if the selected file is a link FileDialog
		// always returns the link
		[DefaultValue(true)]
		public bool DereferenceLinks {
			get {
				return dereferenceLinks;
			}
			
			set {
				dereferenceLinks = value;
			}
		}
		
		[DefaultValue("")]
		public string FileName {
			get {
				if (fileNames == null || fileNames.Length == 0)
					return string.Empty;

				if (fileNames [0].Length == 0)
					return string.Empty;

				// skip check for illegal characters if the filename was set
				// through FileDialog API
				if (!checkForIllegalChars)
					return fileNames [0];

				// ensure filename contains only valid characters
				Path.GetFullPath (fileNames [0]);
				// but return filename as is
				return fileNames [0];

			}
			
			set {
				if (value != null) {
					fileNames = new string [1] { value };
				} else {
					fileNames = new string [0];
				}

				// skip check for illegal characters if the filename was set
				// through FileDialog API
				checkForIllegalChars = false;
			}
		}
		
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string[] FileNames {
			get {
				if (fileNames == null || fileNames.Length == 0) {
					return new string [0];
				}
				
				string[] new_filenames = new string [fileNames.Length];
				fileNames.CopyTo (new_filenames, 0);

				// skip check for illegal characters if the filename was set
				// through FileDialog API
				if (!checkForIllegalChars)
					return new_filenames;

				foreach (string fileName in new_filenames) {
					// ensure filename contains only valid characters
					Path.GetFullPath (fileName);
				}
				return new_filenames;
			}
		}

		[DefaultValue(1)]
		public int FilterIndex {
			get {
				return filterIndex;
			}
			set {
				filterIndex = value;
			}
		}
		
		[DefaultValue("")]
		public string InitialDirectory {
			get {
				if (initialDirectory == null)
					return string.Empty;
				return initialDirectory;
			}
			set {
				initialDirectory = value;
			}
		}
		
		[DefaultValue(false)]
		public bool RestoreDirectory {
			get {
				return restoreDirectory;
			}
			
			set {
				restoreDirectory = value;
			}
		}
		
		[DefaultValue(false)]
		public bool ShowHelp {
			get {
				return showHelp;
			}
			
			set {
				showHelp = value;
				ResizeAndRelocateForHelpOrReadOnly ();
			}
		}
		
		[DefaultValue(false)]
		public bool SupportMultiDottedExtensions {
			get {
				return supportMultiDottedExtensions;
			}

			set {
				supportMultiDottedExtensions = value;
			}
		}

		[DefaultValue("")]
		[Localizable(true)]
		public string Title {
			get {
				if (title == null)
					return string.Empty;
				return title;
			}
			set {
				title = value;
			}
		}
		
		// this one is a hard one ;)
		// Win32 filename:
		// - up to MAX_PATH characters (windef.h) = 260
		// - no trailing dots or spaces
		// - case preserving
		// - etc...
		// NTFS/Posix filename:
		// - up to 32,768 Unicode characters
		// - trailing periods or spaces
		// - case sensitive
		// - etc...
		[DefaultValue(true)]
		public bool ValidateNames {
			get {
				return validateNames;
			}
			
			set {
				validateNames = value;
			}
		}
		
		public override void Reset ()
		{
			addExtension = true;
			checkFileExists = false;
			checkPathExists = true;
			DefaultExt = null;
			dereferenceLinks = true;
			FileName = null;
			Filter = String.Empty;
			FilterIndex = 1;
			InitialDirectory = null;
			restoreDirectory = false;
			SupportMultiDottedExtensions = false;
			ShowHelp = false;
			Title = null;
			validateNames = true;
			
			UpdateFilters ();
		}
		
		public override string ToString ()
		{
			return String.Format("{0}: Title: {1}, FileName: {2}", base.ToString (), Title, FileName);
		}
		
		public event CancelEventHandler FileOk {
			add { Events.AddHandler (EventFileOk, value); }
			remove { Events.RemoveHandler (EventFileOk, value); }
		}
		
		// This is just for internal use with MSs version, so it doesn't need to be implemented
		// as it can't really be accessed anyways
		protected int Options {
			get { return -1; }
		}

		internal virtual string DialogTitle {
			get {
				return Title;
			}
		}

		[MonoTODO ("Not implemented, will throw NotImplementedException")]
		protected override IntPtr HookProc (IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
		{
			throw new NotImplementedException ();
		}
		
		protected void OnFileOk (CancelEventArgs e)
		{	
			CancelEventHandler fo = (CancelEventHandler) Events [EventFileOk];
			if (fo != null)
				fo (this, e);
		}
		

		bool AddFilterExtension (string fileName)
		{
			if (fileDialogType == FileDialogType.OpenFileDialog) {
				if (DefaultExt.Length == 0)
					return true;

				if (checkFileExists) {
					// if CheckFileExists is true, only add filter extension if
					// file with DefaultExt does not exist
					string fullFileName = fileName + "." + DefaultExt;
					return !File.Exists (fullFileName);
				} else {
					// if CheckFileExists is false, only add filter extension
					// if specified file does not exist
					return !File.Exists (fileName);
				}
			}

			return true;
		}

		string GetExtension (string fileName)
		{
			string filter_extension = String.Empty;


			return filter_extension;
		}
		
		void OnClickHelpButton (object sender, EventArgs e)
		{
			OnHelpRequest (e);
		}
		
		void OnForceDialogEndFileView (object sender, EventArgs e)
		{
			OnClickOpenSaveButton (this, EventArgs.Empty);
		}
		
		void OnDirectoryChangedDirComboBox (object sender, EventArgs e)
		{
			//mwfFileView.ChangeDirectory (sender, dirComboBox.CurrentFolder, CustomFilter);
		}
		
		void OnDirectoryChangedPopupButtonPanel (object sender, EventArgs e)
		{
			//mwfFileView.ChangeDirectory (sender, popupButtonPanel.CurrentFolder, CustomFilter);
		}

		void OnFileDialogFormClosed (object sender, FormClosedEventArgs e)
		{
			HandleFormClosedEvent (sender);
		}

		void HandleFormClosedEvent (object sender)
		{
			if (!disable_form_closed_event)
				OnClickCancelButton (sender, EventArgs.Empty);
			
			disable_form_closed_event = false;
		}

		class FileNamesTokenizer
		{
			public FileNamesTokenizer (string text, bool allowMultiple)
			{
				_text = text;
				_position = 0;
				_tokenType = TokenType.BOF;
				_allowMultiple = allowMultiple;
			}

			public TokenType CurrentToken {
				get { return _tokenType; }
			}

			public string TokenText {
				get { return _tokenText; }
			}

			public bool AllowMultiple {
				get { return _allowMultiple; }
			}

			private int ReadChar ()
			{
				if (_position < _text.Length) {
					return _text [_position++];
				} else {
					return -1;
				}
			}

			private int PeekChar ()
			{
				if (_position < _text.Length) {
					return _text [_position];
				} else {
					return -1;
				}
			}

			private void SkipWhitespaceAndQuotes ()
			{
				int ch;

				while ((ch = PeekChar ()) != -1) {
					if ((char) ch != '"' && !char.IsWhiteSpace ((char) ch))
						break;
					ReadChar ();
				}
			}

			public void GetNextFile ()
			{
				if (_tokenType == TokenType.EOF)
					throw new Exception ("");

				int ch;

				SkipWhitespaceAndQuotes ();

				if (PeekChar () == -1) {
					_tokenType = TokenType.EOF;
					return;
				}

				_tokenType = TokenType.FileName;
				StringBuilder sb = new StringBuilder ();

				while ((ch = PeekChar ()) != -1) {
					if ((char) ch == '"') {
						ReadChar ();
						if (AllowMultiple)
							break;
						int pos = _position;

						SkipWhitespaceAndQuotes ();
						if (PeekChar () == -1) {
							break;
						}
						_position = ++pos;
						sb.Append ((char) ch);
					} else {
						sb.Append ((char) ReadChar ());
					}
				}

				_tokenText = sb.ToString ();
			}

			private readonly bool _allowMultiple;
			private int _position;
			private readonly string _text;
			private TokenType _tokenType;
			private string _tokenText;
		}

		internal enum TokenType
		{
			BOF,
			EOF,
			FileName,
		}
	}
	#endregion



}
