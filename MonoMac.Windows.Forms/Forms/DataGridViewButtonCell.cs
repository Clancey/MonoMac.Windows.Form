using System;
using System.Linq;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	public class DataGridViewButtonCell : NSButtonCell
	{
		public DataGridViewButtonCell ()
		{
			this.BezelStyle = NSBezelStyle.Rounded;
			this.Activated += delegate(object sender, EventArgs e) {
				if (Click != null)
					Click (sender, e);
			};
		}
		public string Text {
			get { return this.Title; }
			set {
				this.Title = value;
			}
		}

		public int TabIndex {
			get { return Tag; }
			set { Tag = value; }
		}
		#region Events
		[Export("buttonAction:")]
		public EventHandler Click { get; set; }

		
		#endregion

		public Color BackColor {
			get {return this.BackgroundColor.ToColor();	}
			set { BackgroundColor = value.ToNSColor();}
		}

	}
}

