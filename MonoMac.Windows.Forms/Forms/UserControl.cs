using System;
using System.Collections;
using System.Linq;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	public partial class UserControl : UserControlMouseView , IControl
	{		
		public string Text {get;set;}
		
		public override bool IsFlipped {
			get {
				return true;
			}
		}

	}
}

