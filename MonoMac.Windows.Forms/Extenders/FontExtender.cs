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
	public static class FontExtender
	{
		public static Font ToFont (this NSFont font)
		{
			if (font == null)
				return new System.Drawing.Font ("Arial", 9.9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			return new System.Drawing.Font (font.FontName, font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			
		}
		public static NSFont ToNsFont (this Font font)
		{
			if (font == null)
				return NSFont.SystemFontOfSize (NSFont.SystemFontSize);
			NSFontManager fontManager = NSFontManager.SharedFontManager;
			NSFont theFont = fontManager.FontWithFamily (font.Name, getFontTraits (font), 0, font.Size);
			return theFont;
			
		}
		private static NSFontTraitMask getFontTraits (Font font)
		{
			if (font.Bold && font.Italic)
				return NSFontTraitMask.Bold | NSFontTraitMask.Italic;
			if (font.Bold)
				return NSFontTraitMask.Bold;
			if (font.Italic)
				return NSFontTraitMask.Italic;
			return NSFontTraitMask.Unbold;
		}
	}
}

