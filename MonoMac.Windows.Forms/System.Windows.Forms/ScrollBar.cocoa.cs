using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public partial class ScrollBar : Control
	{
		internal NSScroller m_helper {get;set;}
				
		public ScrollBar ()
		{
			position = 0;
			minimum = 0;
			maximum = 100;
			large_change = 10;
			small_change = 1;

			
			base.TabStop = false;

			SetStyle (ControlStyles.UserPaint | ControlStyles.StandardClick
#if NET_2_0
				| ControlStyles.UseTextForAccessibility
#endif
				, false);
		}
	}
}

