using System;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
using System.Drawing;
using System.Collections.Generic;

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
	public partial class Button 
	{
 		#region Public Constructors
		public Button () : base ()
		{
			SetStyle (ControlStyles.StandardDoubleClick, false);			
		}
		
		protected override void CreateHandle ()
		{
			ButtonHelper helper = new ButtonHelper();
      		m_view = helper;
			helper.Font = Font.ToNsFont();
			helper.Host = this;
			helper.BezelStyle = NSBezelStyle.Rounded;
			
			helper.Activated += delegate(object sender, EventArgs e) {
					OnClick(e);
			};
			helper.Frame = new NSRect(0, 0, 100, 25);
			helper.ScaleUnitSquareToSize(Util.ScaleSize);
		}
		#endregion	// Public Constructors
		
		[Obsolete("Not Implemented.", false)]
		public bool UseVisualStyleBackColor {get;set;}

		public Color BackColor {
			get {
        ButtonHelper bh = m_view as ButtonHelper;
        if( bh != null )
          return bh.Cell.BackgroundColor.ToColor();
        return background_color;
      }
			set {
        ButtonHelper bh = m_view as ButtonHelper;
        if( bh != null )
          bh.Cell.BackgroundColor = value.ToNSColor();
        background_color = value;
      }
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
			get{
        ButtonHelper bh = m_view as ButtonHelper;
        if( bh != null )
          enabled = bh.Enabled;
        return enabled;
      }
			set{
        enabled = value;
        ButtonHelper bh = m_view as ButtonHelper;
        if( bh != null )
          bh.Enabled = value;
			}
		}
	}
}

