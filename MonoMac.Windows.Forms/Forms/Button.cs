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
		//[Export("buttonAction:")]
		public event EventHandler Click;

		
		#endregion

		public Color BackColor {
			get {return Cell.BackgroundColor.ToColor();	}
			set { Cell.BackgroundColor = value.ToNSColor();}
		}
		
		public DialogResult DialogResult {get;set;}
		/*
		public override void MouseUp (NSEvent theEvent)
		{
			
			PointF point = theEvent.LocationInWindow;
			

			if(MouseUp != null)
				MouseUp(this, new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			base.MouseUp (theEvent);
		}
		*/

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

