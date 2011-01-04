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
// Copyright (c) 2004-2005 Novell, Inc.
//
// Authors:
//	Dennis Hayes	dennish@raytek.com
//	Peter Bartok	pbartok@novell.com
//

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms {
	[DefaultProperty("Checked")]
	[DefaultEvent("CheckedChanged")]
	[ComVisible (true)]
	[ClassInterface (ClassInterfaceType.AutoDispatch)]
	[DefaultBindingProperty ("CheckState")]
	[ToolboxItem ("System.Windows.Forms.Design.AutoSizeToolboxItem," + Consts.AssemblySystem_Design)]
	public partial class CheckBox : ButtonBase {
		#region Local Variables
		internal Appearance		appearance;
		internal bool			auto_check;
		internal ContentAlignment	check_alignment;
		internal CheckState		check_state;
		internal bool			three_state;
		#endregion	// Local Variables

		#region	Internal Methods

		internal override void HaveDoubleClick() {
			if (DoubleClick != null) DoubleClick(this, EventArgs.Empty);
		}
		#endregion	// Internal Methods

		#region Public Instance Properties
		[DefaultValue(Appearance.Normal)]
		[Localizable(true)]
		public Appearance Appearance {
			get {
				return appearance;
			}

			set {
				if (value != appearance) {
					appearance = value;
					OnAppearanceChanged (EventArgs.Empty);

					if (Parent != null)
						Parent.PerformLayout (this, "Appearance");
					Invalidate();
				}
			}
		}

		[DefaultValue(true)]
		public bool AutoCheck {
			get {
				return auto_check;
			}

			set {
				auto_check = value;
			}
		}

		[Bindable(true)]
		[Localizable(true)]
		[DefaultValue(ContentAlignment.MiddleLeft)]
		public ContentAlignment CheckAlign {
			get {
				return check_alignment;
			}

			set {
				if (value != check_alignment) {
					check_alignment = value;
					if (Parent != null)
						Parent.PerformLayout (this, "CheckAlign");
					Invalidate();
				}
			}
		}

		[DefaultValue(ContentAlignment.MiddleLeft)]
		[Localizable(true)]
		public override ContentAlignment TextAlign {
			get { return base.TextAlign; }
			set { base.TextAlign = value; }
		}
		
		#endregion	// Public Instance Properties

		#region Protected Instance Properties
		protected override Size DefaultSize {
			get {
				return new Size(104, 24);
			}
		}
		#endregion	// Protected Instance Properties

		#region Public Instance Methods
		public override string ToString() {
			return base.ToString() + ", CheckState: " + (int)check_state;
		}
		#endregion	// Public Instance Methods

		#region Protected Instance Methods
		protected virtual void OnAppearanceChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [AppearanceChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnCheckedChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [CheckedChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnCheckStateChanged(EventArgs e) {
			EventHandler eh = (EventHandler)(Events [CheckStateChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected override void OnClick(EventArgs e) {
			if (auto_check) {
				switch(check_state) {
					case CheckState.Unchecked: {
						if (three_state) {
							CheckState = CheckState.Indeterminate;
						} else {
							CheckState = CheckState.Checked;
						}
						break;
					}

					case CheckState.Indeterminate: {
						CheckState = CheckState.Checked;
						break;
					}

					case CheckState.Checked: {
						CheckState = CheckState.Unchecked;
						break;
					}
				}
			}
			
			base.OnClick (e);
		}

		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated (e);
		}

		protected override void OnKeyDown (KeyEventArgs e)
		{
			base.OnKeyDown (e);
		}

		protected override void OnMouseUp(MouseEventArgs mevent) {
			base.OnMouseUp (mevent);
		}
		#endregion	// Protected Instance Methods

		#region Events
		static object AppearanceChangedEvent = new object ();
		static object CheckedChangedEvent = new object ();
		static object CheckStateChangedEvent = new object ();

		public event EventHandler AppearanceChanged {
			add { Events.AddHandler (AppearanceChangedEvent, value); }
			remove { Events.RemoveHandler (AppearanceChangedEvent, value); }
		}

		public event EventHandler CheckedChanged {
			add { Events.AddHandler (CheckedChangedEvent, value); }
			remove { Events.RemoveHandler (CheckedChangedEvent, value); }
		}

		public event EventHandler CheckStateChanged {
			add { Events.AddHandler (CheckStateChangedEvent, value); }
			remove { Events.RemoveHandler (CheckStateChangedEvent, value); }
		}
		
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event MouseEventHandler MouseDoubleClick {
			add { base.MouseDoubleClick += value; }
			remove { base.MouseDoubleClick -= value; }
		}
		#endregion	// Events

		#region Events
		// XXX have a look at this and determine if it
		// manipulates base.DoubleClick, and see if
		// HaveDoubleClick can just call OnDoubleClick.
		[Browsable(false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event EventHandler DoubleClick;
		#endregion	// Events
	}
}
