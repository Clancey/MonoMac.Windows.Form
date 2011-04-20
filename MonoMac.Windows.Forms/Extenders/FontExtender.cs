using System;
using System.Drawing;
using MonoMac.AppKit;
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

