using System;
using MonoMac.AppKit;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	[MonoMac.Foundation.Register("Form")]
	public partial class Application : NSApplication
	{
		static ApplicationContext context;
		
		[Export("NSZombieEnabled=YES")]
		public Application ()
		{
			
		}
		
		public static void Run(Func<Form> mainForm)
		{
			context = new ApplicationContext (mainForm);
			Run (context);
		}
		
		public static void Run ()
		{
			context = new ApplicationContext();
			Run (context);
		}

		public static void Run (Form mainForm)
		{
			context = new ApplicationContext (mainForm);
			Run (context);
		}

		public static void Run (NSWindow mainForm)
		{
			NSApplication.Init ();
			NSApplication.InitDrawingBridge();
			NSApplication.Main (new string[]{});
		}
		
		public static void Run (ApplicationContext context)
		{
			NSApplication.Init ();
			NSApplication.InitDrawingBridge();
			NSApplication.Main (new string[]{});
		}
	}
}

