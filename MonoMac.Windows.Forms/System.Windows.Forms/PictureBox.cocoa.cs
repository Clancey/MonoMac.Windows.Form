using System;
using System.Drawing;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public partial class PictureBox
	{
		protected override Size DefaultSize {
			get { return new Size(50,50); }
		}

		protected override void OnPaint (PaintEventArgs pe)
		{
			//TODO: Switch to using NSImageView
			pe.Graphics.DrawImage(Image,Location);
			base.OnPaint (pe);
		}
	}
}

