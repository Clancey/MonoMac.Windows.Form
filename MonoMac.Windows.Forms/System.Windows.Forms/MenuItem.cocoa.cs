using System;
using AppKit;
namespace System.Windows.Forms
{
	public partial class MenuItem
	{
		internal MenuItemHelper helper;
		public static implicit operator NSMenuItem (MenuItem menuItem)
		{
			return menuItem.helper;
		}
		
		void CreateHandle ()
		{
			helper = new MenuItemHelper(this);
			helper.Activated += delegate(object sender, EventArgs e) {
				OnClick(e);
			};
		}
		
		private void CommonConstructor (string text)
		{
			CreateHandle();
			defaut_item = false;
			separator = false;
			break_ = false;
			bar_break = false;
			checked_ = false;
			radiocheck = false;
			enabled = true;
			showshortcut = true;
			visible = true;
			ownerdraw = false;
			menubar = false;
			menuheight = 0;
			xtab = 0;
			index = -1;
			mnemonic = '\0';
			menuid = -1;
			mergeorder = 0;
			mergetype = MenuMerge.Add;
			Text = text;	// Text can change separator status
			helper.Title = text;
		}
	}
}

