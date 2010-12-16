using System;
using System.Drawing;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public partial class CheckBox : Button
	{
		public CheckBox () : base()
		{
			m_helper.SetButtonType(NSButtonType.Switch);
			
		}
		
		public bool Checked 
		{
			get { return m_helper.State == NSCellStateValue.On;}
			set { m_helper.State = value ? NSCellStateValue.On : NSCellStateValue.Off;}
		}
		public CheckState CheckState
		{
			get {
				switch (m_helper.State)
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
					m_helper.State = NSCellStateValue.On;
					break;
				case System.Windows.Forms.CheckState.Unchecked:
					m_helper.State = NSCellStateValue.Off;
					break;
				case System.Windows.Forms.CheckState.Indeterminate : 
					m_helper.State = NSCellStateValue.Mixed;
					break;
				}
			}
		}
		
		public bool ThreeState
		{
			get{ return m_helper.AllowsMixedState;}
			set { m_helper.AllowsMixedState = value;}
		}
	}
}

