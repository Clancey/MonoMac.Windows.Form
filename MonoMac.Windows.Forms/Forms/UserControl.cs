using System;
using System.Collections;
using System.Linq;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	public partial class UserControl : NSControl , IControl
	{		
		public string Text {get;set;}
		public Color BackColor {get;set;}

	}
}

