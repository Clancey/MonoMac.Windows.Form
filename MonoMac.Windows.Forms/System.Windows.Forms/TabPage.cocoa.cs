using System;
using MonoMac.AppKit;

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
	}
}

