using System;
using System.Drawing;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public partial class CheckBox : Button
	{
		public CheckBox () : base()
		{
			this.SetButtonType(NSButtonType.Switch);
			
		}
		
		public bool Checked 
		{
			get { return this.State == NSCellStateValue.On;}
			set { this.State = value ? NSCellStateValue.On : NSCellStateValue.Off;}
		}
		public CheckState CheckState
		{
			get {
				switch (this.State)
				{
					case NSCellStateValue.On: return CheckState.Checked;
					case NSCellStateValue.Off : return CheckState.Unchecked;
					case NSCellStateValue.Mixed : return CheckState.Indeterminate;
				}
				return CheckState.Unchecked;
			}
			set{
				switch(value){
				case System.Windows.Forms.CheckState.Checked :
					this.State = NSCellStateValue.On;
					break;
				case System.Windows.Forms.CheckState.Unchecked:
					this.State = NSCellStateValue.Off;
					break;
				case System.Windows.Forms.CheckState.Indeterminate : 
					this.State = NSCellStateValue.Mixed;
					break;
				}
			}
		}
		
		public bool ThreeState
		{
			get{ return this.AllowsMixedState;}
			set { this.AllowsMixedState = value;}
		}
	}
}

