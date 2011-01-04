using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public partial class CheckBox 
	{
		
		#region Public Constructors
		public CheckBox() {
			appearance = Appearance.Normal;
			auto_check = true;
			check_alignment = ContentAlignment.MiddleLeft;
			TextAlign = ContentAlignment.MiddleLeft;
			SetStyle(ControlStyles.StandardDoubleClick, false);
			SetAutoSizeMode (AutoSizeMode.GrowAndShrink);			
			m_helper.SetButtonType(NSButtonType.Switch);
		}
		#endregion	// Public Constructors
	
		
		[Bindable(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(false)]
		[SettingsBindable (true)]
		public bool Checked {
			get {
				if (CheckState != CheckState.Unchecked) {
					return true;
				}
				return false;
			}

			set {
				if (value && (CheckState != CheckState.Checked)) {
					CheckState = CheckState.Checked;
					OnCheckedChanged(EventArgs.Empty);
				} else if (!value && (CheckState != CheckState.Unchecked)) {
					CheckState = CheckState.Unchecked;
					OnCheckedChanged(EventArgs.Empty);
				}
			}
		}
		
		[DefaultValue(CheckState.Unchecked)]
		[RefreshProperties(RefreshProperties.All)]
		[Bindable(true)]
		public CheckState CheckState {
			get {
				switch (m_helper.State)
				{
					case NSCellStateValue.On: return CheckState.Checked;
					case NSCellStateValue.Off : return CheckState.Unchecked;
					case NSCellStateValue.Mixed : return CheckState.Indeterminate;
				}
				return CheckState.Unchecked;
			}

			set {
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
				if (value != check_state) {
					bool	was_checked = (check_state != CheckState.Unchecked);

					check_state = value;

					if (was_checked != (check_state != CheckState.Unchecked)) {
						OnCheckedChanged(EventArgs.Empty);
					}

					OnCheckStateChanged(EventArgs.Empty);
				}
			}
		}
		
		
		[DefaultValue(false)]
		public bool ThreeState
		{
			get{ return m_helper.AllowsMixedState;}
			set { m_helper.AllowsMixedState = value;
				 three_state = value;}
		}
	}
}

