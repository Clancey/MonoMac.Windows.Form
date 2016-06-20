using System;
using AppKit;
using System.Runtime.InteropServices;
namespace System.Windows.Forms
{
	public class DataGridViewCheckBoxCell : DataGridViewButtonCell
	{
		public DataGridViewCheckBoxCell ()
		{
			Init();
		}
		/*
		public DataGridViewCheckBoxCell(IntPtr handle) : base(handle)
		{
			Init();
		}
		*/
		void Init()
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
	}
}

