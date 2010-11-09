using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using System.Windows.Forms;

namespace MonomacTestConversion
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		NSWindow window;
	NSTextField text;
	MyView view;
		Form theform;
		MainWindowController mainWindow;
		public AppDelegate ()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
				view = new MyView (new RectangleF (10, 10, 200, 200));

		text = new NSTextField (new RectangleF (44, 32, 232, 31)) {
			StringValue = "Hello Mono Mac!"
		};
			/*
		window = new NSWindow (new RectangleF (50, 50, 400, 400), (NSWindowStyle) (1 | (1 << 1) | (1 << 2) | (1 << 3)), 0, false);
		window.ContentView.AddSubview (text);
		window.ContentView.AddSubview (view);
			window.MakeKeyAndOrderFront(this);
			*/
			theform = new Form();
			theform.Show();
			mainWindow = new MainWindowController();
			mainWindow.ma

		}
	}
}

