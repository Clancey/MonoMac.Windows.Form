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

