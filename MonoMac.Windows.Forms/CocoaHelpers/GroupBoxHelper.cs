using System;
using MonoMac.AppKit;
using System.Drawing;
namespace System.Windows.Forms
{
	internal class GroupBoxHelper : NSBox, IViewHelper
	{
		public GroupBoxHelper (Control parent)
		{
			Host = parent;
			this.ContentView = new FlippedView();
		}
		
		public PointF OffSet
		{
			get {
				var parentFram = this.Frame;
				var insideFrame = (this.ContentView as FlippedView).Frame.Location.Add(this.ContentViewMargins);
				return insideFrame;
				
			}
		}

		#region IViewHelper implementation
		public NSCursor Cursor { get; set; }

		public Control Host { get; set; }
		#endregion
		#region IViewHelper implementation
		public void FontChanged ()
		{
			this.TitleFont = Host.Font.ToNsFont();
		}
		
		#endregion
	}
}

