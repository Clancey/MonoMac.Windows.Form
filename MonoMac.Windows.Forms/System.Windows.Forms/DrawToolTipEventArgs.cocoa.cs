using System;
using System.Drawing;
namespace System.Windows.Forms
{
	
#if NET_2_0
	public
#endif
	partial class DrawToolTipEventArgs
	{

		public void DrawBackground ()
		{
			graphics.FillRectangle (new SolidBrush (back_color), bounds);
		}
				
		public void DrawBorder ()
		{
			//ControlPaint.DrawBorder (graphics, bounds, SystemColors.WindowFrame, ButtonBorderStyle.Solid);
		}
		

		public void DrawText (TextFormatFlags flags)
		{
		//	TextRenderer.DrawTextInternal (graphics, tooltip_text, font, bounds, fore_color, flags, false);
		}
	}
}

