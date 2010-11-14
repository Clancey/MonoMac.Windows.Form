using System;
using System.Drawing;
using MonoMac.AppKit;

namespace System.Windows.Forms
{
	public class Control: NSView, IControl
	{
        public SizeF Size
        {
            get { return this.Frame.Size; }
            set { this.Frame = new RectangleF(this.Frame.Location, value); }
        }

        public PointF Location
        {
            get { return this.Frame.Location; }
            set { this.Frame = new RectangleF(value, this.Frame.Size); }
        }
		/*
		public Color BackColor 
		{
			get 
			{
				return Color.FromArgb( BackgroundColor.AlphaComponent
				                      ,BackgroundColor.RedComponent
				                      ,BackgroundColor.GreenComponent
				                      ,BackgroundColor.BlueComponent());
			}
			set 
			{ 
				BackgroundColor = NSColor.FromCalibratedRGBA(value.R ,value.G ,value.B ,value.A);
			}
		}
		*/

        #region Key Events

        public KeyEventHandler OnKeyDown { get; set; }
        public override void KeyDown(NSEvent theEvent)
        {
            base.KeyDown(theEvent);
            if (OnKeyDown != null)
                OnKeyDown(this, new KeyEventArgs(theEvent));
        }

        public KeyEventHandler OnKeyUp { get; set; }
        public override void KeyUp(NSEvent theEvent)
        {
            base.KeyUp(theEvent);
            if (OnKeyUp != null)
                OnKeyUp(this, new KeyEventArgs(theEvent));
        }

	    public KeyPressEventHandler OnKeyPress
	    {
	        get { throw new NotImplementedException(); }
	        set { throw new NotImplementedException(); }
	    }
        #endregion
    }
}

