using System;
using MonoMac.AppKit;
using System.Drawing;
namespace System.Windows.Forms
{
	public static class ImageExtenders
	{
		public static NSImage ToNSImage(this Image image)
        {
			//TODO: Rewrite!
			image.Save("tempimage");
			return new NSImage("tempimage");
        }
	}
}

