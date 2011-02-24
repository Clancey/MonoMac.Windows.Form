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

		public MainClass()
		{
			// 11 Feb 2011 S. Baer
			// Define different UI tests by order of importance
			this.ClientSize = new Size(500,400);
			int i=0;
			AddTest(i++, "ShowMessageBox - works", ShowMessageBox);
			AddTest(i++, "ShowEditBox - works", ShowEditBox);
			AddTest(i++, "ShowNumberBox - works", ShowNumberBox);
			AddTest(i++, "ShowListBox - works", ShowListBox);
			AddTest(i++, "ShowComboListBox - works", ShowComboListBox);
			// Not really important, we have this wrapped by other means
			AddTest(i++, "ShowColorDialog", ShowColorDialog);
		}
		
		void AddTest(int i, string text, System.EventHandler click_event )
		{
			Button button = new Button();
			button.Location = new Point(10,i*25+10);
			button.Size = new Size(200,23);
			button.Text = text;
			button.Click += click_event;
 			this.Controls.Add(button);
		}
			
		void ShowEditBox(object sender, EventArgs e)
		{
			string returnString;
			var dr = Dialogs.ShowEditBox("Edit Box Title", "Message", "Default Text", false, out returnString);
			//if( !string.IsNullOrEmpty(returnString) )
			MessageBox.Show(returnString,"Dialog Result : " +  dr.ToString());
		}
		
		void ShowComboListBox(object sender, EventArgs e)
		{
			var result = Dialogs.ShowComboListBox("Title","The Message", new List<string>{"string1","string2","string3"}) ?? "";
			if(!string.IsNullOrEmpty(result.ToString()))
				MessageBox.Show("Selection : " + result.ToString());
		}
		void ShowListBox(object sender, EventArgs e)
		{
			var result = Dialogs.ShowListBox("Title","The Message", new List<string>{"string1","string2","string3"}) ?? "";
			if(!string.IsNullOrEmpty(result.ToString()))
				MessageBox.Show("Selection : " + result.ToString());
		}
		void ShowColorDialog(object sender, EventArgs e)
		{
			Color refColor = Color.Blue;
		    Dialogs.ShowColorDialog(ref refColor);
		}
		void ShowMessageBox(object sender, EventArgs e)
		{
			Rectangle[] rects = Dialogs.StringBoxRects();
			string s = "";
			for( int i=0; i<rects.Length; i++ )
			{
				s += string.Format("{0}\n",rects[i]);
			}
			Dialogs.ShowMessageBox(s, "String Box Rects");
		}
		void ShowNumberBox(object sender, EventArgs e)
		{
			double refDbl = 32;
			Dialogs.ShowNumberBox("Title","Message",ref refDbl);
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

