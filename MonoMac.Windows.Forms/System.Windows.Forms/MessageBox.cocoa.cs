using System;
using System.Drawing;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
		
	internal
	enum AlertType {
		Default		= 1,
		Error		= 2,
		Question	= 3,
		Warning		= 4,
		Information	= 5
	}
	
		#region Private MessageBoxForm class
		internal class MessageBoxForm : NSAlert
		{
			#region MessageBoxFrom Local Variables
			const int space_border = 10;
			const int button_width = 86;
			const int button_height = 23;
			const int button_space = 5;
			const int space_image_text= 10;
		
			MessageBoxIcon Icon;
			string Text;
			string			msgbox_text;
			bool			size_known	= false;
			Icon			icon_image;
			RectangleF		text_rect;
			MessageBoxButtons	msgbox_buttons;
			MessageBoxDefaultButton	msgbox_default;
			bool			buttons_placed	= false;
			int			button_left;
			Button[]		buttons = new Button[4];
			bool                    show_help;
			string help_file_path;
			string help_keyword;
			HelpNavigator help_navigator;
			object help_param;
			AlertType		alert_type;
			#endregion	// MessageBoxFrom Local Variables
		
			#region MessageBoxForm Constructors
			public MessageBoxForm (IWin32Window owner, string text, string caption,
					       MessageBoxButtons buttons, MessageBoxIcon icon,
					       bool displayHelpButton)
			{
				show_help = displayHelpButton;
			
				Icon = icon;
				switch (icon) {
					case MessageBoxIcon.None: {
						icon_image = null;
						alert_type = AlertType.Default;
						break;
					}

					case MessageBoxIcon.Error: {		// Same as MessageBoxIcon.Hand and MessageBoxIcon.Stop
						icon_image = SystemIcons.Error;
						alert_type = AlertType.Error;
						break;
					}

					case MessageBoxIcon.Question: {
 						icon_image = SystemIcons.Question;
						alert_type = AlertType.Question;
						break;
					}

					case MessageBoxIcon.Asterisk: {		// Same as MessageBoxIcon.Information
						icon_image = SystemIcons.Information;
						alert_type = AlertType.Information;
						break;
					}

					case MessageBoxIcon.Warning: {		// Same as MessageBoxIcon.Exclamation:
						icon_image = SystemIcons.Warning;
						alert_type = AlertType.Warning;
						break;
					}
				}

				msgbox_text = text;
				msgbox_buttons = buttons;
				msgbox_default = MessageBoxDefaultButton.Button1;

				this.Text = caption;
			}

			public MessageBoxForm (IWin32Window owner, string text, string caption,
					MessageBoxButtons buttons, MessageBoxIcon icon,
					MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton)
				: this (owner, text, caption, buttons, icon, displayHelpButton)
			{
				msgbox_default = defaultButton;
			}

			public MessageBoxForm (IWin32Window owner, string text, string caption,
					       MessageBoxButtons buttons, MessageBoxIcon icon)
				: this (owner, text, caption, buttons, icon, false)
			{
			}
			#endregion	// MessageBoxForm Constructors

			#region MessageBoxForm Methods
			public void SetHelpData (string file_path, string keyword, HelpNavigator navigator, object param)
			{
				help_file_path = file_path;
				help_keyword = keyword;
				help_navigator = navigator;
				help_param = param;
			}
			
			internal string HelpFilePath {
				get { return help_file_path; }
			}
			
			internal string HelpKeyword {
				get { return help_keyword; }
			}
			
			internal HelpNavigator HelpNavigator {
				get { return help_navigator; }
			}
			
			internal object HelpParam {
				get { return help_param; }
			}
			
			public DialogResult RunDialog ()
			{
				MessageText = Text;
				InformativeText = msgbox_text;
				SetupButtons(msgbox_buttons);
				SetupIcon(Icon);
				var result = GetResult(this.RunModal (),msgbox_buttons);
				return result;
			}
			

			#endregion	// MessageBoxForm Methods

			#region Functions for Adding buttons
			public static DialogResult GetResult(int result,MessageBoxButtons buttons)
			{
				switch(buttons)
				{
				case MessageBoxButtons.OK:
					return DialogResult.OK;
				case MessageBoxButtons.OKCancel :
					if(result > 1000)
						return DialogResult.OK;
					return DialogResult.Cancel;
				case MessageBoxButtons.AbortRetryIgnore:
					if(result == 1000)
						return DialogResult.Ignore;
					else if (result == 1001)
						return DialogResult.Retry;
					return DialogResult.Abort;
				case MessageBoxButtons.RetryCancel:
					if(result == 1000)
						return DialogResult.Cancel;
					return DialogResult.Retry;
				case MessageBoxButtons.YesNo:
					if(result == 1000)
						return DialogResult.No;
					return DialogResult.Yes;
				case MessageBoxButtons.YesNoCancel:
					if(result == 1000)
						return DialogResult.Cancel;
					else if (result == 1001)
						return DialogResult.No;
					else return DialogResult.Yes;
				default :
					return DialogResult.Cancel;
					
				}
			}
			
			public void SetupIcon(MessageBoxIcon icon)
			{
				switch(icon)
				{
				case MessageBoxIcon.Error:
					this.AlertStyle = NSAlertStyle.Critical;
					break;
				case MessageBoxIcon.Warning :
					this.AlertStyle = NSAlertStyle.Warning;
					break;
				default :
					this.AlertStyle = NSAlertStyle.Informational;
					break;
				}
			}
			
			public void SetupButtons(MessageBoxButtons buttons)
			{
				switch (buttons)
				{
				case MessageBoxButtons.OK:
						return;
				case MessageBoxButtons.OKCancel:
					this.AddButton("Cancel");
					this.AddButton("OK");
					break;
				case MessageBoxButtons.AbortRetryIgnore :
					this.AddButton("Ignore");
					this.AddButton("Retry");
					this.AddButton("Abort");
					break;
				case MessageBoxButtons.RetryCancel:
					this.AddButton("Cancel");
					this.AddButton("Retry");
					break;
				case MessageBoxButtons.YesNo:
					this.AddButton("No");
					this.AddButton("Yes");
					break;
				case MessageBoxButtons.YesNoCancel:
					this.AddButton("Cancel");
					this.AddButton("No");
					this.AddButton("Yes");
					break;
					
				}
			}
			
			#endregion


#if NET_2_0
		
			#region UIA Framework: Methods, Properties and Events

			internal string UIAMessage {
				get { return msgbox_text; }
			}

			internal Rectangle UIAMessageRectangle {
				get { 
					return new Rectangle ((int) text_rect.X,
					                      (int) text_rect.Y, 
					                      (int) text_rect.Width, 
					                      (int) text_rect.Height); 
				}
			}

			internal Rectangle UIAIconRectangle {
				get { 
					return new Rectangle (space_border, 
					                      space_border, 
							      icon_image == null ? -1 : icon_image.Width, 
							      icon_image == null ? -1 : icon_image.Height);
				}
			}

			#endregion

#endif
		}
		#endregion	// Private MessageBoxForm class
	
}

