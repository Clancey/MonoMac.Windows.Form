using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public partial class Label : TextBoxBase
	{
		public Label () : base()
		{
			m_helper.TextView.Selectable = false;
			m_helper.TextView.Editable = false;
			//this.Font = NSFont.FromFontName ("Arial", 10);
			//m_helper.Bordered = false;
		}
		/*
		public override bool AcceptsFirstResponder ()
		{
			return false;
		}
		*/
		private void resize ()
		{
			if (!AutoSize)
				return;
			//m_helper.SizeToFit ();
		}
	}
}

