
using MonoMac.AppKit;
using MonoMac.Foundation;
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
		internal override NSView c_helper {
			get {
				return m_helper;
			}
			set {
				m_helper = value as NSControl;
			}
		}
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
						var tb = (new NSTextField(){StringValue = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890",Font = this.Font.ToNsFont()});
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
				c.c_helper.BecomeFirstResponder();
			}
		}
		/// 
	}
}

