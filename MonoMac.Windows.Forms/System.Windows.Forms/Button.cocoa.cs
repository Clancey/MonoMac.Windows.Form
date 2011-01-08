using System;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
using System.Drawing;
using System.Collections.Generic;
namespace System.Windows.Forms
{
	
	[MonoMac.Foundation.Register("Button")]
	public partial class Button 
	{
		#region Public Constructors
		public Button () : base ()
		{
			dialog_result = DialogResult.None;
			SetStyle (ControlStyles.StandardDoubleClick, false);
			
		}
		
		internal override void CreateHelper ()
		{
			m_helper = new ButtonHelper();
			m_helper.Host = this;
			m_helper.BezelStyle = NSBezelStyle.Rounded;
			
			m_helper.Activated += delegate(object sender, EventArgs e) {
					OnClick(e);
			};
			m_helper.Frame = new System.Drawing.RectangleF (0, 0, 100, 25);
		}
		#endregion	// Public Constructors
		
		[Obsolete("Not Implemented.", false)]
		public bool UseVisualStyleBackColor {get;set;}

		public Color BackColor {
			get {return m_helper.Cell.BackgroundColor.ToColor();	}
			set { m_helper.Cell.BackgroundColor = value.ToNSColor();}
		}
		
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
		internal override bool enabled
		{
			get{ return m_helper.Enabled;}
			set{ m_helper.Enabled = value;
				base.enabled = value;
			}
		}

	}
}

