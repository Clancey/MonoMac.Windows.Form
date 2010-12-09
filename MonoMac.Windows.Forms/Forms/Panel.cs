using System;
using MonoMac.AppKit;
using System.Drawing;
namespace System.Windows.Forms
{
	public partial class Panel : PanelMouseView
		//NSView
	{
		public Panel ()
		{
			BackColor = Color.DarkGray;
		}
		#region Setup
		public override bool IsFlipped {
			get {
				return true;
			}
		} 
		#endregion
		
		#region Simple Public Properties
		public Color BackColor {get;set;}
		
		#endregion
	}
}

