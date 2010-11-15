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
			this.Frame = new System.Drawing.RectangleF (0, 0, 0, 0);
			//this.Font = NSFont.FromFontName("Arial",11);
			
		}
		public string Text {
			get { return this.Title; }
			set {
				this.Title = value;
				resize();
			}
		}

		public string Name { get; set; }

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

		//TODO: consolidate
		public SizeF Size {
			get { return this.Frame.Size; }
			set { this.Frame = new RectangleF (this.Frame.Location, value); }
		}

		public PointF Location {
			get { return this.Frame.Location; }
			set { this.Frame = new RectangleF (value, this.Frame.Size); }
		}
		
		public new System.Drawing.Font Font
		{
			get {
				return new System.Drawing.Font(base.Font.FontName, base.Font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}
			set {
				base.Font = MonoMac.AppKit.NSFont.FromFontName(value.Name,value.Size);
				
			}
		}
		
		public bool Visible{
			get{ return Hidden;}
			set {Hidden = value;}
		}
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

