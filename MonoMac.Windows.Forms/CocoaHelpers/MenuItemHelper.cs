using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public class MenuItemHelper : NSMenuItem
	{
		MenuItem Host;
		public MenuItemHelper (MenuItem host) : base ()
		{
			Host = host;
		}
		
		public override bool IsSeparatorItem {
			get {
				return Host.Separator || Host.BarBreak || Host.Break;
			}
		}
	}
}

