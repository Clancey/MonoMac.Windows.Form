using System;
using System.Drawing;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
    public static class ColorExtenders
    {
        public static Color ToColor(this NSColor clr)
        {
			if (clr == null)
				return Color.Transparent;
			clr.UsingColorSpace(NSColorSpace.CalibratedRGB);
			return Color.FromArgb( (int)clr.AlphaComponent
				                      ,(int)clr.RedComponent
				                      ,(int)clr.GreenComponent
				                      ,(int)clr.BlueComponent);
        }
        public static NSColor ToNSColor(this Color clr)
        {
			 return NSColor.FromCalibratedRgba(clr.R
                                               ,clr.G
                                               ,clr.B
                                               ,clr.A).UsingColorSpace(NSColorSpace.CalibratedRGB);
        }
    }   
}


