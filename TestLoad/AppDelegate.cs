using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace TestLoad
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		NSWindow window;
		MainWindowController mainWindowController;

		public AppDelegate ()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
			window = new NSWindow(new RectangleF (50, 50, 400, 400), (NSWindowStyle) (1 | (1 << 1) | (1 << 2) | (1 << 3)), NSBackingStore.Buffered, false);
			window.MakeKeyAndOrderFront(this);
			//mainWindowController = new MainWindowController ();
			//mainWindowController.Window.MakeKeyAndOrderFront (this);
		}
	}
}

