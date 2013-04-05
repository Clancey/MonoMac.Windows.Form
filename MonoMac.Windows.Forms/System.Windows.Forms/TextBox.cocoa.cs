// 
//  Copyright 2011  James Clancey
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.using System;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;

#if MAC64
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
using CGFloat = System.Double;
#else
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
using NSPoint = System.Drawing.PointF;
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using CGFloat = System.Single;
#endif

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
			m_helper.TextView.MinSize = new NSSize(bounds.Size);
			base.UpdateBounds ();
		}
		public override string Text {
			get { return m_helper.TextView.Value; }
			set { m_helper.TextView.Value = value; 
			if(!Multiline)
				m_helper.TextView.SetSingleLine();}
		}
		
		
		public void ViewDidMoveToSuperview ()
		{
			var sz = m_helper.Superview.Bounds.Size;
			int width = (int)sz.Width;
			int height = (int)sz.Height;
			if (Dock == DockStyle.Fill)
				this.Size = new Size (width, height);
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
		
		#region Events
		


		#endregion

		//TODO: consolidate
			
		//TODO: make it work
		public float Left
		{
			get;
			set;
		}

		public float Right 
		{
			get 
			{
				if (m_helper.Superview == null) return -1;
				return (float)((m_helper.Frame.X + this.Width) - m_helper.Superview.Frame.Width);
			}
		}
		
		public int Top
		{
			get { return (int)this.Location.Y;}
			set { this.Location = new Point((int)this.Location.Y,value);}
		}
		
		public void Clear()
		{
			this.Text = string.Empty;	
		}

		public HorizontalAlignment TextAlign
		{
			get
			{
				switch(m_helper.TextView.Alignment)
				{
				case NSTextAlignment.Left:
					return HorizontalAlignment.Left;
				case NSTextAlignment.Center:
					return HorizontalAlignment.Center;
				case NSTextAlignment.Right:
					return HorizontalAlignment.Right;
				default : return HorizontalAlignment.Left;
					break;
				}
			}
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
				case MonoMac.AppKit.NSWritingDirection.LeftToRight:
					return RightToLeft.No;
				case MonoMac.AppKit.NSWritingDirection.RightToLeft:
					return RightToLeft.Yes;
				case MonoMac.AppKit.NSWritingDirection.Natural :
					return RightToLeft.No;
				default :
					return RightToLeft.No;
				}
			}
			set {
				switch(value)
				{
				case RightToLeft.Inherit:
					m_helper.TextView.BaseWritingDirection = MonoMac.AppKit.NSWritingDirection.Embedding;
					break;
				case RightToLeft.No:
					m_helper.TextView.BaseWritingDirection = MonoMac.AppKit.NSWritingDirection.LeftToRight;
					break;
				case RightToLeft.Yes:
					m_helper.TextView.BaseWritingDirection = MonoMac.AppKit.NSWritingDirection.RightToLeft;
					break;
				}
			}
		}
		
		public void Paste (string text)
		{
			m_helper.SelectedText = text;
			//document.ReplaceSelection (CaseAdjust (text), false);

			//ScrollToCaret();
			OnTextChanged(EventArgs.Empty);
		}
	}
}

