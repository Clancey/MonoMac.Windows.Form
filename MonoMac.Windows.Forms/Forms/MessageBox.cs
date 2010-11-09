using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public partial class MessageBox : NSAlert
	{
		public static DialogResult Show (string Text)
		{
			var alert = new NSAlert ();
			alert.MessageText = Text;
			alert.RunModal ();
			return DialogResult.OK;
		}

		public MessageBox ()
		{
		}
	}
}

