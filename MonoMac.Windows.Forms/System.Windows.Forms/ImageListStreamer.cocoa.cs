using System.IO;
using System.Drawing;
using System.Collections;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
namespace System.Windows.Forms
{
	public partial class ImageListStreamer
	{
		public void GetObjectData (SerializationInfo si, StreamingContext context)
		{
			MemoryStream stream = new MemoryStream ();
			BinaryWriter writer = new BinaryWriter (stream);
			writer.Write (header);

			Image [] images = (imageCollection != null) ? imageCollection.ToArray () : this.images;
			int cols = 4;
			int rows = images.Length / cols;
			if (images.Length % cols > 0)
				++rows;

			writer.Write ((ushort) images.Length);
			writer.Write ((ushort) images.Length);
			writer.Write ((ushort) 0x4);
			writer.Write ((ushort) (images [0].Width));
			writer.Write ((ushort) (images [0].Height));
			writer.Write (0xFFFFFFFF); //BackColor.ToArgb ()); //FIXME: should set the right one here.
			writer.Write ((ushort) 0x1009);
			for (int i = 0; i < 4; i++)
				writer.Write ((short) -1);

			Bitmap main = new Bitmap (cols * ImageSize.Width, rows * ImageSize.Height);
			using (Graphics g = Graphics.FromImage (main)) {
				g.FillRectangle (new SolidBrush(BackColor), 0, 0,
						main.Width, main.Height);
				for (int i = 0; i < images.Length; i++) {
					g.DrawImage (images [i], (i % cols) * ImageSize.Width,
							(i / cols) * ImageSize.Height);
				}
			}

			MemoryStream tmp = new MemoryStream ();
			main.Save (tmp, ImageFormat.Bmp);
			tmp.WriteTo (stream);

			Bitmap mask = Get1bppMask (main);
			main.Dispose ();
			main = null;

			tmp = new MemoryStream ();
			mask.Save (tmp, ImageFormat.Bmp);
			tmp.WriteTo (stream);
			mask.Dispose ();

			stream = GetRLEStream (stream, 4);
			si.AddValue ("Data", stream.ToArray (), typeof (byte []));
		}
	}
}

