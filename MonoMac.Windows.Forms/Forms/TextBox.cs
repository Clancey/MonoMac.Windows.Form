using System;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	[MonoMac.Foundation.Register("TextBox")]
	public partial class TextBox : Control // : TextBoxMouseView, IControl
		//NSTextField
	{
		internal TextBoxMouseView m_helper;
		internal override NSView c_helper {
			get {
				return m_helper;
			}
			set {
				m_helper = value as TextBoxMouseView;
			}
		}
		public TextBox ()
		{
			m_helper =  new TextBoxMouseView();
			m_helper.Selectable = true;
			m_helper.Editable = true;
			Multiline = true;
			//this.BackgroundColor = NSColor.Clear;
		}

		public virtual string Text {
			get { return m_helper.StringValue; }
			set { m_helper.StringValue = value; }
		}
		
		public string[] Lines
		{
			get{return m_helper.StringValue.Split(new char[] { '\n' });}
			set{m_helper.StringValue = "";
				foreach(var val in value)
				{ m_helper.StringValue+= val + "\n";}
			}
		}
		private DockStyle dock = DockStyle.None;
		public DockStyle Dock {
			get{return dock;}
			set{dock = value ;
				//TODO resize;
			}
		}
		/*
		public override void ViewDidMoveToSuperview ()
		{
			base.ViewDidMoveToSuperview ();
			
			if(Dock ==  DockStyle.Fill)
				this.Size =  this.Superview.Bounds.Size;
			setScrollBars();
		}
		*/
		
		private ScrollBars _scrollBars = ScrollBars.None;
		public ScrollBars ScrollBars { 
			get{return _scrollBars;}
			set
			{
				_scrollBars = value;
				setScrollBars();
			}
		}
		
		private void setScrollBars()
		{
			if(m_helper.EnclosingScrollView == null)
				return;
			switch(_scrollBars)
			{
			case ScrollBars.Both:
				m_helper.EnclosingScrollView.HasVerticalScroller = true;
				m_helper.EnclosingScrollView.HasHorizontalScroller = true;
				return;
			case ScrollBars.Horizontal:
				m_helper.EnclosingScrollView.HasVerticalScroller = false;
				m_helper.EnclosingScrollView.HasHorizontalScroller = true;
				return;
			case ScrollBars.Vertical:
				m_helper.EnclosingScrollView.HasVerticalScroller = true;
				m_helper.EnclosingScrollView.HasHorizontalScroller = false;
				return;
			case ScrollBars.None:
				m_helper.EnclosingScrollView.HasVerticalScroller = false;
				m_helper.EnclosingScrollView.HasHorizontalScroller = false;
				return;
			}
		}
		
		public bool Multiline{get;set;}
		
		#region Events
		


		#endregion

		//TODO: consolidate
			
		//TODO: make it work
		public float Left{get;set;}
		public float Right {get { if (m_helper.Superview == null) return -1;return  (m_helper.Frame.X + this.Width) - m_helper.Superview.Frame.Width ;}}
		
		public float Top{ get{ return this.Location.Y;}
			set { this.Location = new PointF(this.Location.Y,value);}
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
					m_helper.Alignment  = (uint)NSTextAlignment.Left;
					break;
				case HorizontalAlignment.Center:
					m_helper.Alignment  = (uint)NSTextAlignment.Center;
					break;
				case HorizontalAlignment.Right:
					m_helper.Alignment  = (uint)NSTextAlignment.Right;
					break;
				default : m_helper.Alignment = (uint) NSTextAlignment.Left;
					break;
				}
					
			}
		}
		
		public Color BackColor {
			get {return m_helper.BackgroundColor.ToColor();}
			set { m_helper.BackgroundColor = value.ToNSColor();
			}
		}
		
		public RightToLeft RightToLeft{
			get{ 
				switch(m_helper.BaseWritingDirection)
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
					case RightToLeft.Inherit : m_helper.BaseWritingDirection = NSWritingDirection.Embedding;	break;
					case RightToLeft.No: m_helper.BaseWritingDirection = NSWritingDirection.LeftToRight; break ;
					case RightToLeft.Yes: m_helper.BaseWritingDirection = NSWritingDirection.RightToLeft ; break;
				}
			}
		}
	

	}
}

