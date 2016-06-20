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
//    limitations under the License.using System;
using System.Drawing;
using AppKit;
namespace System.Windows.Forms
{
	public static class ColorExtenders
	{
		public static Color ToColor (this NSColor clr)
		{
			if (clr == null)
				return Color.Transparent;
			clr = clr.UsingColorSpace (NSColorSpace.CalibratedRGB);
			return Color.FromArgb ((int)clr.AlphaComponent, (int)clr.RedComponent, (int)clr.GreenComponent, (int)clr.BlueComponent);
		}
		public static NSColor ToNSColor (this Color clr)
		{
			return NSColor.FromCalibratedRgba (clr.R, clr.G, clr.B, clr.A).UsingColorSpace (NSColorSpace.CalibratedRGB);
		}
	}
}


