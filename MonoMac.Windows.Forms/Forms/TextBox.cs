using System;
using MonoMac.AppKit;
using System.Drawing;
namespace System.Windows.Forms
{
	[MonoMac.Foundation.Register("TextBox")]
	public partial class TextBox : NSTextField, IControl
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
		
		#region Events
		
		public override void KeyDown (NSEvent theEvent)
		{
			base.KeyDown (theEvent);
			if(OnKeyDown != null)
				OnKeyDown(this,new KeyEventArgs(theEvent));
		}
		public override void KeyUp (NSEvent theEvent)
		{
			base.KeyUp (theEvent);
            if (OnKeyUp != null)
                OnKeyUp(this, new KeyEventArgs(theEvent));
			if(OnKeyPress != null)
				OnKeyPress(this, new KeyPressEventArgs(theEvent.Characters.ToCharArray()[0]));
		}
		
	    public KeyPressEventHandler OnKeyPress { get; set; }
        public KeyEventHandler OnKeyUp { get; set; }
        public KeyEventHandler OnKeyDown { get; set; }

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
		
		public Color BackColor {
			get {
				return Color.FromArgb( (int)BackgroundColor.AlphaComponent
				                      ,(int)BackgroundColor.RedComponent
				                      ,(int)BackgroundColor.GreenComponent
				                      ,(int)BackgroundColor.BlueComponent());
			}
			set { BackgroundColor = NSColor.FromCalibratedRGBA(value.R
			                                                   ,value.G
			                                                   ,value.B
			                                                   ,value.A);
			}
		}
		public RightToLeft RightToLeft{
			get{ 
				switch(this.BaseWritingDirection)
				{
					case NSWritingDirection.LeftToRight: return RightToLeft.No;
					case NSWritingDirection.RightToLeft : return RightToLeft.Yes;
					case NSWritingDirection.Natural : return RightToLeft.No;
					default : return RightToLeft.No;
				}
			}
			set {
				switch(value)
				{
					case RightToLeft.Inherit : BaseWritingDirection = NSWritingDirection.Embedding;	break;
					case RightToLeft.No: BaseWritingDirection = NSWritingDirection.LeftToRight; break ;
					case RightToLeft.Yes: BaseWritingDirection = NSWritingDirection.RightToLeft ; break;
				}
			}
		}
	

	}
}

