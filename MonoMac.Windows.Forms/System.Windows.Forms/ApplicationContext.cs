using System;
using AppKit;
using System.Drawing;
namespace System.Windows.Forms
{

	[Foundation.Register("AppDelegate")]
	public class ApplicationContext : NSApplicationDelegate
	{
		Form main_form;
		static Func<Form> mainFormFunc;
		public ApplicationContext () : base()
		{
			
		}
		public ApplicationContext (Func<Form> mainForm) : base()
		{
			mainFormFunc = mainForm;
		}

		public override void FinishedLaunching (Foundation.NSObject notification)
		{
			if (mainFormFunc != null)
				main_form = mainFormFunc ();
			main_form.m_helper.MakeKeyAndOrderFront (this);
			main_form.m_helper.DidChangeScreen += delegate(object sender, EventArgs e) { main_form.m_helper.Display (); };
		}

		public ApplicationContext (Form mainForm)
		{
			MainForm = mainForm;
			// Use  property to get event handling setup
		}

		~ApplicationContext ()
		{
			this.Dispose (false);
		}


		public Form MainForm {
			get { return main_form; }

			set {
				if (main_form != value)
				{
					// Catch when the form is destroyed so we can fire OnMainFormClosed
					main_form = value;
				}
			}
		}
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			MainForm = null;
		}
		public override void WillTerminate (Foundation.NSNotification notification)
		{
			OnMainFormClosed (notification.Object, new EventArgs ());
			//base.WillTerminate (notification);
		}
		protected virtual void OnMainFormClosed (object sender, EventArgs e)
		{
			
		}
		public event EventHandler ThreadExit;
	}
}
