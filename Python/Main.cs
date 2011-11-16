using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using System.Windows.Forms;

namespace Python
{
	class MyForm : Form
	{
		
		public static void Main (string[] args)
		{
			Application.Run (delegate() { return new MyForm (); });
		}
	}
}

