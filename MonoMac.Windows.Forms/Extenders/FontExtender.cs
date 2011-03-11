using System;
using System.Drawing;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public static class FontExtender
	{
		public static Font ToFont(this NSFont font)
        {
				if(font == null)
					return new System.Drawing.Font ("Arial",  9.9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				return new System.Drawing.Font(font.FontName, font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

        }
        public static NSFont ToNsFont(this Font font)
        {
			return MonoMac.AppKit.NSFont.FromFontName(font.Name,font.Size);
        }
	}
}

