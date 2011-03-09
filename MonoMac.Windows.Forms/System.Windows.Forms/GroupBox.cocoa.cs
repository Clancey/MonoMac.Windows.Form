using System;
using System.Drawing;
using MonoMac.AppKit;
using System.ComponentModel;
namespace System.Windows.Forms
{
	public partial class GroupBox
	{
		internal GroupBoxHelper m_helper;
		
		protected override void CreateHandle ()
		{
			m_helper = new GroupBoxHelper(this);
      		m_view = m_helper;
		}
		
		
		protected override Size DefaultSize {
			get { return new Size(100,100);}
		}
		
		[Localizable(true)]
		public override string Text {
			get { return base.Text; }
			set {
				if (base.Text == value)
					return;
				m_helper.Title = value;
				base.Text = value;
				Refresh ();
			}
		}
		protected override void OnPaint (PaintEventArgs e)
		{
			//ThemeEngine.Current.DrawGroupBox (e.Graphics, ClientRectangle, this);
			base.OnPaint(e);
		}
	}
}

