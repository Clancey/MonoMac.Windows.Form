using System;
using System.Drawing;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public partial class PictureBox
	{
		
		internal NSImageView m_helper;
		protected override void CreateHandle ()
		{
			m_helper = new NSImageView();
      		m_view = m_helper;
			//helper.Host = this;
			//helper.ScaleUnitSquareToSize(Util.ScaleSize);
		}
		
		protected override Size DefaultSize {
			get { return new Size(50,50); }
		}

		protected override void OnPaint (PaintEventArgs pe)
		{
			pe.Graphics.DrawImage(Image,Location);
			base.OnPaint (pe);
		}
	}
}

