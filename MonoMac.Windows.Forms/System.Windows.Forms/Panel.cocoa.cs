using System;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	public partial class Panel : Control // : PanelMouseView
		//NSView
	{
		internal PanelMouseView m_helper;
		
		internal override NSView c_helper {
			get {
				return m_helper;
			}
			set {
				m_helper = (PanelMouseView)value;
			}
		}
		
		public Panel ()
		{
			BackColor = Color.DarkGray;
		}
		internal NSTrackingArea trackingArea;
		internal override void CreateHelper ()
		{
			m_helper = new PanelMouseView();
			m_helper.Host = this;
			trackingArea = new NSTrackingArea(m_helper.Frame,(NSTrackingAreaOptions.MouseEnteredAndExited |
			                                                             NSTrackingAreaOptions.MouseMoved |
			                                                             NSTrackingAreaOptions.ActiveInKeyWindow), m_helper,new NSDictionary());
			m_helper.AddTrackingArea( trackingArea);
		}
		#region Setup
		#endregion
		
		#region Simple Public Properties
		
		#endregion
	}
}

