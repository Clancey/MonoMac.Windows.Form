using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	
	[MonoMac.Foundation.Register("Form")]
	public partial class Application : NSApplication
	{
		public Application ()
		{
			
		}
		public static void Run(Func<Form> mainForm)
		{
			
			Run (new ApplicationContext (mainForm));
		}
		
		public static void Run ()
		{
			Run (new ApplicationContext ());
		}

		public static void Run (Form mainForm)
		{
			Run (new ApplicationContext (mainForm));
		}

		public static void Run (NSWindow mainForm)
		{
			NSApplication.Init ();
			NSApplication.Main (new string[]{});
		}
		
		public static void Run (ApplicationContext context)
		{
			NSApplication.Init ();
			NSApplication.Main (new string[]{});
			
		}
	}
}

