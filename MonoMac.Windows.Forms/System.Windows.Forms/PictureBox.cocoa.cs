using System;
using System.Drawing;
using AppKit;
namespace System.Windows.Forms
{
	public partial class PictureBox :Control
	{
		NSImageView m_helper;
		protected override Size DefaultSize {
			get { return new Size(50,50); }
		}
		protected override void CreateHandle ()
		{
			m_helper = new NSImageView();
			m_view = m_helper;
		}
		protected override void OnPaint (PaintEventArgs pe)
		{
			//TODO: Switch to using NSImageView
			//pe.Graphics.DrawImage(Image,new Point(0,0));
			base.OnPaint (pe);
		}
		
		
		private void ChangeImage (Image value, bool from_url)
		{
			StopAnimation ();

			image_from_url = from_url;
			image = value;

			if (IsHandleCreated) {
				m_helper.Image = image.ToNSImage();
				this.Size = image.Size;
				UpdateSize ();
				if (image != null && ImageAnimator.CanAnimate (image)) {
					frame_handler = new EventHandler (OnAnimateImage);
					ImageAnimator.Animate (image, frame_handler);
				}
				if (no_update == 0) {
					Invalidate ();
				}
			}
		}
	}
}

