using System;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
using System.Drawing;
namespace System.Windows.Forms
{
	
	[MonoMac.Foundation.Register("Button")]
	public partial class Button : NSButton
	{
		public Button () : base()
		{
			this.BezelStyle = NSBezelStyle.Rounded;
			
			this.Activated += delegate(object sender, EventArgs e) {
				if (Clicked != null)
					Clicked (sender, e);
			};
			this.Frame = new System.Drawing.RectangleF (0, 0, 0, 0);
			this.Font = NSFont.FromFontName("Arial",11);
			
		}
		public string Text {
			get { return this.Title; }
			set {
				this.Title = value;
				this.SizeToFit ();
			}
		}

		[Export("buttonAction:")]
		public EventHandler Clicked { get; set; }

		public string Name { get; set; }

		public int TabIndex {
			get { return Tag; }
			set { Tag = value; }
		}

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

