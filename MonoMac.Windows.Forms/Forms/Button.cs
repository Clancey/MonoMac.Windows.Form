using System;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
using System.Drawing;
namespace System.Windows.Forms
{
	
	[MonoMac.Foundation.Register("Button")]
	public partial class Button : NSButton, IControl
	{
		public Button () : base()
		{
			this.BezelStyle = NSBezelStyle.Rounded;
			
			this.Activated += delegate(object sender, EventArgs e) {
				if (Click != null)
					Click (sender, e);
			};
			this.Frame = new System.Drawing.RectangleF (0, 0, 100, 25);
			//this.Font = NSFont.FromFontName("Arial",11);
			
		}
		public string Text {
			get { return this.Title; }
			set {
				this.Title = value;
				resize();
			}
		}

		public int TabIndex {
			get { return Tag; }
			set { Tag = value; }
		}
		#region Events
		[Export("buttonAction:")]
		public EventHandler Click { get; set; }


        public KeyEventHandler OnKeyDown { get; set; }
        public KeyPressEventHandler OnKeyPress { get; set; }
        public KeyEventHandler OnKeyUp { get; set; }
		
		#endregion

		public Color BackColor {
			get {
				if(Cell.BackgroundColor == null)
					return Color.Transparent;
				Cell.BackgroundColor = Cell.BackgroundColor.ColorUsingColorSpaceName(NSColorSpace.CalibratedRGB);
				return Color.FromArgb( (int)Cell.BackgroundColor.AlphaComponent
				                      ,(int)Cell.BackgroundColor.RedComponent
				                      ,(int)Cell.BackgroundColor.GreenComponent
				                      ,(int)Cell.BackgroundColor.BlueComponent());
			}
			set { Cell.BackgroundColor = NSColor.FromCalibratedRGBA(value.R
			                                                   ,value.G
			                                                   ,value.B
			                                                   ,value.A).ColorUsingColorSpaceName(NSColorSpace.CalibratedRGB);
			}
		}
		
		public DialogResult DialogResult {get;set;}

		private bool autoSize;
		public bool AutoSize{get{ return autoSize;}set {autoSize = value; resize();}}
		private void resize ()
		{
			if (!AutoSize)
				return;
			this.SizeToFit ();
		}

	}
}

