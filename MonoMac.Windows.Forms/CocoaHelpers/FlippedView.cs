using System;
using MonoMac.AppKit;

namespace System.Windows.Forms
{
	internal class FlippedView : NSView
	{
		public override bool IsFlipped {
			get {
				return true;
			}
		}
	}
}

