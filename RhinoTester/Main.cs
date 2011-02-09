using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Rhino.UI;
using System.Collections.Generic;
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
			this.ClientSize = new Size(500,400);
			button = new Button();
			button.Location = new Point(10,10);
			button.Size = new Size(100,23);
			button.Text = "Click Me";
			string returnString;
			Color refColor = Color.Blue;
			Double refDbl = Double.Epsilon;
			button.Click += delegate {
				//Dialogs.ShowEditBox("Title","Message","Default",false,out returnString);
				//Dialogs.ShowComboListBox("Title","The Message", new List<string>{"string1","string2","string3"});
				//TODO:FIX
				//Dialogs.ShowColorDialog(ref refColor);
				//Dialogs.ShowListBox("Title","The Message",new List<string>{"string1","string2","string3"});
				//TODO:FIX
				//Dialogs.ShowMessageBox("Message","Title");
				Dialogs.ShowNumberBox("Title","Message",ref refDbl);
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

