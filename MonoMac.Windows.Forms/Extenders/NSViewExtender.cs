using System;
using MonoMac.AppKit;
using System.Linq;
namespace System.Windows.Forms
{
	public static class NSViewExtender
	{
		public static void BringSubviewToFront(this NSView superView, NSView theView)
		{
			if(theView == null || !superView.Subviews.Contains(theView))
				return;
			theView.RemoveFromSuperview();
			superView.AddSubview(theView);
		}
		
		public static void BringToFront(this NSView theView)
		{
			if(theView.Superview == null)
				return;
			theView.Superview.BringSubviewToFront(theView);
		}
	}
}

