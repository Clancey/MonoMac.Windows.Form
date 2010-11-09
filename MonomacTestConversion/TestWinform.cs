using System;
using System.Drawing;
using System.Windows.Forms;
using MonoMac.AppKit;
class MyForm : Form
{
	public Button button1;
	public MyForm()
	{
	//Text to be Displayed in the Caption-Title Bar
	this.Text = "Winform Built with monomac"; 
		button1 = new Button();
		this.Controls.Add(button1);
		
	}
	public static void Main(string[] args)
	{
		Application.Run(delegate(){
			return new MyForm();
		});
	}
}

