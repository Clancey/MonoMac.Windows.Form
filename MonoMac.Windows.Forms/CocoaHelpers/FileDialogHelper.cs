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
using AppKit;
using System.Threading;
namespace System.Windows.Forms
{
	internal class FileDialogHelper : NSOpenPanel, IViewHelper
	{
		CommonDialog Host;
		public event EventHandler CancelEvent; 
		public FileDialogHelper (CommonDialog host)
		{
			Host = host;
			this.CanChooseFiles = true;
			this.CanChooseDirectories = false;
		}
		public override void Cancel (Foundation.NSObject sender)
		{
			if(CancelEvent != null)
				CancelEvent(this,null);
			base.Cancel (sender);
		}
		
		public void Show()
		{
			
			bool isComplete = false;
			this.RunModal();
			Host.DialogResult = DialogResult.OK;
			this.SelectionDidChange += delegate(object sender, EventArgs e) {
				if(Host is FileDialog)
				{
					FileDialog fileDialog = (FileDialog)Host;
					fileDialog.FileName = this.Url.Path;
				}
			};
			this.CancelEvent += delegate {
				Host.DialogResult = DialogResult.Cancel;
			};
			Console.WriteLine("complete");
			
		}
	

		#region IViewHelper implementation
		void IViewHelper.FontChanged ()
		{
			throw new NotImplementedException ();
		}

		NSCursor IViewHelper.Cursor {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		Control IViewHelper.Host {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}
		#endregion
	}
}

