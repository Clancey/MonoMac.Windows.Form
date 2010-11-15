using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public partial class Label : TextBox
	{
		public Label ()
		{
			this.Selectable = false;
			this.Editable = false;
			//this.Font = NSFont.FromFontName ("Arial", 10);
			this.Bordered = false;
		}
		public bool AutoSize { get; set; }

		public override string Text {
			get { return base.Text; }
			set {
				base.Text = value;
				resize ();
			}
		}
		public override bool AcceptsFirstResponder ()
		{
			return false;
		}
		private void resize ()
		{
			if (!AutoSize)
				return;
			this.SizeToFit ();
		}
	}
}

