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
			//this.BackgroundColor = NSColor.Clear;
		}

		public virtual string Text {
			get { return this.StringValue; }
			set { this.StringValue = value; }
		}

		public int TabIndex {
			get { return Tag; }
			set { Tag = value; }
		}
		public AnchorStyles Anchor { get; set; }
		
		#region Events
		


		#endregion

		//TODO: consolidate
			
		//TODO: make it work
		public float Left{get;set;}
		public float Right {get { if (this.Superview == null) return -1;return  (this.Frame.X + this.Width) - this.Superview.Frame.Width ;}}
		
		public float Top{ get{ return this.Location.Y;}
			set { this.Location = new PointF(this.Location.Y,value);}
		}
		
		public void Focus()
		{
			this.BecomeFirstResponder();
		}
		public void Clear()
		{
			this.Text = string.Empty;	
		}
		public HorizontalAlignment TextAlign
		{
			set
			{
				switch(value)
				{
				case HorizontalAlignment.Left:
					this.Alignment  = (uint)NSTextAlignment.Left;
					break;
				case HorizontalAlignment.Center:
					this.Alignment  = (uint)NSTextAlignment.Center;
					break;
				case HorizontalAlignment.Right:
					this.Alignment  = (uint)NSTextAlignment.Right;
					break;
				default : this.Alignment = (uint) NSTextAlignment.Left;
					break;
				}
					
			}
		}
		
		public Color BackColor {
			get {return BackgroundColor.ToColor();}
			set { BackgroundColor = value.ToNSColor();
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

