using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Drawing.Text;

namespace ToolStripDemo
{
	public class ToolStripDemo : Form
	{
		[STAThread]
		static void Main ()
		{
		        //Application.EnableVisualStyles ();
		        Application.Run (delegate {return new ToolStripDemo ();});
		}

		private ToolStrip ts;
		private TextBox rtb;
		private string image_path;
		
		public ToolStripDemo ()
		{
			this.Text = "ToolStrip Notepad Sample";
			this.Size = new Size (750, 450);
			this.StartPosition = FormStartPosition.CenterScreen;

			image_path = Path.Combine (Path.GetDirectoryName (Application.ExecutablePath), "images");

			rtb = new TextBox ();
			rtb.Multiline = true;
			rtb.Dock = DockStyle.Fill;
			rtb.BorderStyle = BorderStyle.FixedSingle;
			//rtb.MouseUp += new MouseEventHandler (rtb_MouseUp);
			this.Controls.Add (rtb);

			ts = new ToolStrip ();
			this.Controls.Add (ts);

			Image image1 = Image.FromFile (Path.Combine (image_path, "document-new.png"));
			ToolStripButton tb1 = new ToolStripButton ("&New Document", image1, delegate
			                                           {
				
			});
			tb1.DisplayStyle = ToolStripItemDisplayStyle.Image;
			ts.Items.Add (tb1);

			Image image2 = Image.FromFile (Path.Combine (image_path, "document-open.png"));
			ToolStripButton tb2 = new ToolStripButton ("&Open Document", image2);
			tb2.DisplayStyle = ToolStripItemDisplayStyle.Image;
			ts.Items.Add (tb2);

			Image image3 = Image.FromFile (Path.Combine (image_path, "document-save.png"));
			ToolStripButton tb3 = new ToolStripButton ("&Save Document", image3);
			tb3.DisplayStyle = ToolStripItemDisplayStyle.Image;
			ts.Items.Add (tb3);

			
		}


		void Exit_Clicked (object sender, EventArgs e)
		{
			this.Close();
		}

	}
}
