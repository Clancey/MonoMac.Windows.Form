using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public partial class ToolStripButton
	{
		public static implicit operator NSMenuItem (ToolStripButton btn)
		{
			var item = new NSMenuItem(btn.Text ,delegate(object sender, EventArgs e) {
				btn.OnClick(e);	
			});
			item.Image = btn.Image.ToNSImage();
			return item;
		}
			
		protected override void OnPaint (System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint (e);
		}
	}
}

