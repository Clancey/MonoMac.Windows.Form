using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
//using TestLibrary;

namespace RhinoTester
{
	class MainClass : Form
	{
		static void Main (string[] args)
		{
			Application.Run (delegate() { return new MainClass(); });
		}
		Button button;
		public MainClass()
		{
			this.ClientSize = new Size(100,50);
			button = new Button();
			button.Location = new Point(0,0);
			button.Size = new Size(100,25);
			button.Text = "Click Me";
			string returnString;
			
				
			button.Click += delegate {
				Rhino.UI.Dialogs.ShowEditBox("Title","Message","Default",false,out returnString);
			};
			this.Controls.Add(button);
		}
		
	    public class ButtonForm : Form
	    {
	        public ButtonForm()
	        {
	            this.ClientSize = new Size(300,100);
	            Button okBtn = new Button();
	            okBtn.Size = new Size(100,25);
	            okBtn.Location = new Point(0,0);
	            okBtn.DialogResult = DialogResult.OK;
	            this.Controls.Add(okBtn);
	        }
	    }
	}
}

