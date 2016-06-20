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
//    limitations under the License.
using AppKit;
using Foundation;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
	public partial class ContainerControl
	{
		internal NSControl m_helper;
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public SizeF CurrentAutoScaleDimensions {
			get {
				switch(auto_scale_mode) {
					case AutoScaleMode.Dpi:
					//TODO
					//return new SizeF (Hwnd.GraphicsContext.DpiX, Hwnd.GraphicsContext.DpiY);
						return new SizeF();

				case AutoScaleMode.Font:
						var tb = new NSTextField();
						tb.StringValue = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
						tb.Font = this.Font.ToNsFont();
						tb.SizeToFit();
						Size s = Size.Round(tb.Frame.Size);
						int width = (int)Math.Round ((float)s.Width / 62f);
						
						return new SizeF (width, s.Height);
				}

				return auto_scale_dimensions;
			}
		}
		

		internal void SendControlFocus (Control c)
		{
			if (c.IsHandleCreated) {
				c.NSViewForControl.BecomeFirstResponder();
			}
		}
		/// 
	}
}

