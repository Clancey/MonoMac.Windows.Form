using System;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
using System.Drawing;
using System.Collections.Generic;
namespace System.Windows.Forms
{
	
	[MonoMac.Foundation.Register("Button")]
	public partial class Button : Control //: ButtonMouseView, IControl
	{
		internal ButtonHelper m_helper;
		internal override  NSView c_helper {
			get {
				return m_helper;
			}
			set {
				m_helper = value as ButtonHelper;
			}
		}
		public Button () : base()
		{
			m_helper = new ButtonHelper();
			m_helper.Host = this;
			m_helper.BezelStyle = NSBezelStyle.Rounded;
			
			m_helper.Activated += delegate(object sender, EventArgs e) {
				if (Click != null)
					Click (sender, e);
			};
			m_helper.Frame = new System.Drawing.RectangleF (0, 0, 100, 25);
			//this.Font = NSFont.FromFontName("Arial",11);
			
		}
		public string Text {
			get { return m_helper.Title; }
			set {
				m_helper.Title = value;
				resize();
			}
		}

		#region Events
		//[Export("buttonAction:")]
		public event EventHandler Click;

		
		#endregion
		
		[Obsolete("Not Implemented.", false)]
		public bool UseVisualStyleBackColor {get;set;}

		public Color BackColor {
			get {return m_helper.Cell.BackgroundColor.ToColor();	}
			set { m_helper.Cell.BackgroundColor = value.ToNSColor();}
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
			m_helper.SizeToFit ();
		}

	}
}

