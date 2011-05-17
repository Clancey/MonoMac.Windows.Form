// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2004-2006 Novell, Inc. (http://www.novell.com)
//
// Authors:
//	Peter Bartok	(pbartok@novell.com)
//
//

// NOT COMPLETE

using System.ComponentModel;

namespace System.Windows.Forms {
	[ToolboxItemFilter("System.Windows.Forms")]
	public abstract partial class CommonDialog : System.ComponentModel.Component {
		#region Local Variables
		private object tag;
		#endregion Local Variables

		#region Public Constructors
		public CommonDialog() {
		}
		#endregion Public Constructors

		#region Public Properties
		[Localizable (false)]
		[Bindable (true)]
		[TypeConverter (typeof (StringConverter))]
		[DefaultValue (null)]
		[MWFCategory ("Data")]
		public object Tag {
			get { return this.tag; }
			set { this.tag = value; }
		}
		#endregion

		#region Internal Methods
		internal virtual void InitFormsSize(Form form) {
			// Override this to set a default size for the form
			form.Width = 200;
			form.Height = 200;
		}
		#endregion Internal Methods
	
		#region Public Instance Methods
		public abstract void Reset ();

		public DialogResult ShowDialog() {
			return ShowDialog (null);
		}

		#endregion	// Public Instance Methods

		#region Protected Instance Methods
		protected virtual IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) {
			return IntPtr.Zero;
		}

		protected virtual void OnHelpRequest(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [HelpRequestEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected virtual IntPtr OwnerWndProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) {
			return IntPtr.Zero;
		}

		protected abstract bool RunDialog(IntPtr hwndOwner);
		#endregion	// Protected Instance Methods

		#region Events
		static object HelpRequestEvent = new object ();

		public event EventHandler HelpRequest {
			add { Events.AddHandler (HelpRequestEvent, value); }
			remove { Events.RemoveHandler (HelpRequestEvent, value); }
		}
		#endregion	// Events
	}
}
