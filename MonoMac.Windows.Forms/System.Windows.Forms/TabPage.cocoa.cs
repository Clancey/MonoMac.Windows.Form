using System;
using MonoMac.AppKit;
using System.Drawing;

namespace System.Windows.Forms
{
	public partial class TabPage : Panel
	{
		internal TabViewItemHelper m_helper;
		
		protected override void CreateHandle ()
		{
			m_helper = new TabViewItemHelper(this);
			m_view = m_helper.View;
		}
		
		public static implicit operator NSTabViewItem (TabPage tabPage)
		{
			return tabPage.m_helper;
		}
		internal override void PaintControlBackground (PaintEventArgs pevent)
		{
			if(m_helper.TabView.Selected != m_helper)
				return;
			base.PaintControlBackground(pevent);
		}
	}
}

