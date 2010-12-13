using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public interface IViewHelper
	{
		NSCursor Cursor {get;set;}
		Control Host {get;set;}
	}
}

