using System;
using MonoMac.AppKit;
using System.Drawing;
namespace System.Windows.Forms
{
	[MonoMac.Foundation.Register("TextBox")]
	public partial class TextBox : NSTextField
	{
		public TextBox ()
		{
			this.Selectable = true;
			this.Editable = true;
		}

		public virtual string Text {
			get { return this.StringValue; }
			set { this.StringValue = value; }
		}

		public string Name { get; set; }

		public int TabIndex {
			get { return Tag; }
			set { Tag = value; }
		}
		public AnchorStyles Anchor { get; set; }

		//TODO: consolidate
		public SizeF Size {
			get { return this.Frame.Size; }
			set { this.Frame = new RectangleF (this.Frame.Location, value); }
		}

		public PointF Location {
			get { return this.Frame.Location; }
			set { this.Frame = new RectangleF (value, this.Frame.Size); }
		}
	}
}

