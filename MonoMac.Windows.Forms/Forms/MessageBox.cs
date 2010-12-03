using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public partial class MessageBox : NSAlert
	{
		public static DialogResult Show (string Text)
		{
			var alert = new MessageBox ();
			alert.MessageText = Text;
			alert.RunModal ();
			return alert.result;
		}
		public static DialogResult Show (object sender, string Text, string Title,MessageBoxButtons buttons,MessageBoxIcon icon)
		{
			var alert = new MessageBox ();
			alert.MessageText = Title;
			alert.InformativeText = Text;
			alert.SetupButtons(buttons);
			alert.SetupIcon(icon);
			var result = GetResult(alert.RunModal (),buttons);
			return result;
		}
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
		
		public MessageBox ()
		{
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
		public DialogResult result = DialogResult.OK;
	}
}

