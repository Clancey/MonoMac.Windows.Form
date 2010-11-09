using System;
using MonoMac.AppKit;
namespace MonoMac.Windows.Forms.Tests
{

	class MainClass
	{
		static void Main (string[] args)
		{
			NSApplication.Init ();
			NSApplication.Main (args);
		}
	}
}

