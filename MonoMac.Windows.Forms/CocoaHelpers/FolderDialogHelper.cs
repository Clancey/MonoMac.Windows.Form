using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	internal class FolderDialogHelper : FileDialogHelper
	{
		public FolderDialogHelper(CommonDialog host) : base(host)
		{
			this.CanChooseFiles = false;
			this.CanChooseDirectories = true;
		}
	
	}
}