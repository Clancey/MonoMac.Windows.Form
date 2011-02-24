using System;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	[MonoMac.Foundation.Register("TextBox")]
	public partial class TextBox : TextBoxBase // : TextBoxMouseView, IControl
		//NSTextField
	{
		/*
		internal override void CreateHelper ()
		{			
			m_helper =  new TextBoxHelper();
			m_helper.Host = this;
			m_helper.TextView.Selectable = true;
			m_helper.TextView.Editable = true;
			Multiline = false;			
			m_helper.TextView.VerticallyResizable = false;
			m_helper.TextView.HorizontallyResizable = true;
			m_helper.TextView.AutoresizingMask = (NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable );
			m_helper.TextView.TextContainer.ContainerSize = new SizeF(float.MaxValue,float.MaxValue);
			m_helper.TextView.TextContainer.WidthTracksTextView = false;
			//m_helper.viewDidMoveToSuperview += delegate(object sender, EventArgs e) {
			//	ViewDidMoveToSuperview();
			//};
		}
		*/
		
		internal override void UpdateBounds ()
		{
			m_helper.TextView.MinSize = bounds.Size;
			base.UpdateBounds ();
		}
		public virtual string Text {
			get { return m_helper.TextView.Value; }
			set { m_helper.TextView.Value = value; 
			if(!Multiline)
				m_helper.TextView.SetSingleLine();}
		}
		
		public string[] Lines
		{
			get{return m_helper.TextView.Value.Split(new char[] { '\n' });}
			set{m_helper.TextView.Value = "";
				foreach(var val in value)
				{ m_helper.TextView.Value += val + "\n";}
			}
		}
		private DockStyle dock = DockStyle.None;
		public DockStyle Dock {
			get{return dock;}
			set{dock = value ;
				//TODO resize;
			}
		}
		
		public void ViewDidMoveToSuperview ()
		{
			
			if(Dock ==  DockStyle.Fill)
				this.Size =  Size.Round(m_helper.Superview.Bounds.Size);
			setScrollBars();
		}
	
		
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
		
		public int Top{ get{ return (int)this.Location.Y;}
			set { this.Location = new Point((int)this.Location.Y,value);}
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
					m_helper.TextView.Alignment  = NSTextAlignment.Left;
					break;
				case HorizontalAlignment.Center:
					m_helper.TextView.Alignment  = NSTextAlignment.Center;
					break;
				case HorizontalAlignment.Right:
					m_helper.TextView.Alignment  = NSTextAlignment.Right;
					break;
				default : m_helper.TextView.Alignment = NSTextAlignment.Left;
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
				switch(m_helper.TextView.BaseWritingDirection)
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
					case RightToLeft.Inherit : m_helper.TextView.BaseWritingDirection = NSWritingDirection.Embedding;	break;
					case RightToLeft.No: m_helper.TextView.BaseWritingDirection = NSWritingDirection.LeftToRight; break ;
					case RightToLeft.Yes: m_helper.TextView.BaseWritingDirection = NSWritingDirection.RightToLeft ; break;
				}
			}
		}
	

	}
}

