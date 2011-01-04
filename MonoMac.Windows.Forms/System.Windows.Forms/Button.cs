//
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
//	Peter Bartok	pbartok@novell.com
//

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace System.Windows.Forms {
	[ClassInterface (ClassInterfaceType.AutoDispatch)]
	[ComVisible (true)]
	[Designer ("System.Windows.Forms.Design.ButtonBaseDesigner, " + Consts.AssemblySystem_Design,
		   "System.ComponentModel.Design.IDesigner")]
	public partial class Button : ButtonBase, IButtonControl {
		#region Local variables
		DialogResult	dialog_result;
		#endregion	// Local variables

		#region Public Properties
		[Browsable (true)]
		[Localizable (true)]
		[DefaultValue (AutoSizeMode.GrowOnly)]
		[MWFCategory("Layout")]
		public AutoSizeMode AutoSizeMode {
			get { return base.GetAutoSizeMode (); }
			set { base.SetAutoSizeMode (value); }
		}

		[DefaultValue (DialogResult.None)]
		[MWFCategory("Behavior")]
		public virtual DialogResult DialogResult {	// IButtonControl
			get { return dialog_result; }
			set { dialog_result = value; }
		}
		#endregion	// Public Properties

		#region Public Methods
		public virtual void NotifyDefault (bool value)	// IButtonControl
		{	
			this.IsDefault = value;
		}

		public void PerformClick ()			// IButtonControl
		{			
			if (CanSelect)
				OnClick (EventArgs.Empty);
		}

		public override string ToString ()
		{
			return base.ToString () + ", Text: " + this.Text;
		}
		#endregion	// Public Methods

		#region	Protected Methods
		protected override void OnClick (EventArgs e)
		{
			if (dialog_result != DialogResult.None) {
				Form p = FindForm ();

				if (p != null)
					p.DialogResult = dialog_result;
			}
			
			base.OnClick (e);
		}

		protected override void OnFontChanged (EventArgs e)
		{
			base.OnFontChanged (e);
		}
		
		protected override void OnMouseEnter (EventArgs e)
		{
			base.OnMouseEnter (e);
		}
		
		protected override void OnMouseLeave (EventArgs e)
		{
			base.OnMouseLeave (e);
		}

		protected override void OnMouseUp (MouseEventArgs mevent)
		{
			base.OnMouseUp (mevent);
		}

		protected override void OnTextChanged (EventArgs e)
		{
			base.OnTextChanged (e);
		}
		#endregion	// Protected Methods

		#region Events
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public new event EventHandler DoubleClick {
			add { base.DoubleClick += value; }
			remove { base.DoubleClick -= value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public new event MouseEventHandler MouseDoubleClick {
			add { base.MouseDoubleClick += value; }
			remove { base.MouseDoubleClick -= value; }
		}
		#endregion	// Events
	}
}
