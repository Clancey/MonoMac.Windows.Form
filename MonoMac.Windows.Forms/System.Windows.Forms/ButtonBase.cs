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
// Copyright (c) 2004-2006 Novell, Inc.
//
// Authors:
//	Peter Bartok	pbartok@novell.com
//

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace System.Windows.Forms {
#if NET_2_0
	[ClassInterface (ClassInterfaceType.AutoDispatch)]
	[ComVisible (true)]
	[Designer ("System.Windows.Forms.Design.ButtonBaseDesigner, " + Consts.AssemblySystem_Design,
		   "System.ComponentModel.Design.IDesigner")]
#endif
	public abstract partial class ButtonBase : Control
	{
		#region Local Variables
		internal Image			image;
		private ContentAlignment	image_alignment;
		internal ContentAlignment	text_alignment;
		private bool			is_default;
		internal bool			is_pressed;
//		private bool			enter_state;
		internal StringFormat		text_format;
		internal bool 			paint_as_acceptbutton;
		
		// Properties are 2.0, but variables used in 1.1 for common drawing code
		private bool			auto_ellipsis;
#if NET_2_0		
		private string			image_key;
#endif
		private bool			use_mnemonic;
		private bool			use_visual_style_back_color;
		#endregion	// Local Variables


		#region Public Properties
#if NET_2_0
		[Browsable (true)]
		[EditorBrowsable (EditorBrowsableState.Always)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Visible)]
		[MWFCategory("Layout")]
		public override bool AutoSize {
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}

		public override Color BackColor {
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}
#endif

		[Localizable(true)]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		[MWFDescription("Sets the alignment of the image to be displayed on button face"), MWFCategory("Appearance")]
		public ContentAlignment ImageAlign {
			get { return image_alignment; }
			set {
				if (image_alignment != value) {
					image_alignment = value;
					Invalidate ();
				}
			}
		}


		[MWFCategory("Appearance")]
#if NET_2_0
		public
#else
		internal
#endif
		bool UseVisualStyleBackColor {
			get { return use_visual_style_back_color; }
			set {
				if (use_visual_style_back_color != value) {
					use_visual_style_back_color = value;
					Invalidate ();
				}
			}
		}
		#endregion	// Public Instance Properties

		#region Protected Properties

		protected internal bool IsDefault {
			get { return is_default; }
			set {
				if (is_default != value) {
					is_default = value;
					Invalidate ();
				}
			}
		}
		#endregion	// Public Instance Properties
		
		#region Protected Methods

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
		}

		protected override void OnEnabledChanged (EventArgs e)
		{
			base.OnEnabledChanged (e);
		}

		protected override void OnGotFocus (EventArgs e)
		{
			Invalidate ();
			base.OnGotFocus (e);
		}

		protected override void OnKeyDown (KeyEventArgs kevent)
		{
			if (kevent.KeyData == Keys.Space) {
				is_pressed = true;
				Invalidate ();
				kevent.Handled = true;
			}
			
			base.OnKeyDown (kevent);
		}

		protected override void OnKeyUp (KeyEventArgs kevent)
		{
			if (kevent.KeyData == Keys.Space) {
				is_pressed = false;
				Invalidate ();
				OnClick (EventArgs.Empty);
				kevent.Handled = true;
			}
			
			base.OnKeyUp (kevent);
		}

		protected override void OnLostFocus (EventArgs e)
		{
			Invalidate ();
			base.OnLostFocus (e);
		}

		protected override void OnMouseDown (MouseEventArgs mevent)
		{
			if ((mevent.Button & MouseButtons.Left) != 0) {
				is_pressed = true;
				Invalidate ();
			}

			base.OnMouseDown (mevent);
		}

		protected override void OnMouseEnter (EventArgs eventargs)
		{
			is_entered = true;
			Invalidate ();
			base.OnMouseEnter (eventargs);
		}

		protected override void OnMouseLeave (EventArgs eventargs)
		{
			is_entered = false;
			Invalidate ();
			base.OnMouseLeave (eventargs);
		}

		protected override void OnMouseMove (MouseEventArgs mevent) {
			bool inside = false;
			bool redraw = false;

			if (ClientRectangle.Contains (mevent.Location))
				inside = true;

			// If the button was pressed and we leave, release the button press and vice versa
			if ((mevent.Button & MouseButtons.Left) != 0) {
				if (this.Capture && (inside != is_pressed)) {
					is_pressed = inside;
					redraw = true;
				}
			}

			if (is_entered != inside) {
				is_entered = inside;
				redraw = true;
			}

			if (redraw)
				Invalidate ();

			base.OnMouseMove (mevent);
		}


		protected override void OnPaint (PaintEventArgs pevent)
		{
			//Draw (pevent);
			base.OnPaint (pevent);
		}

		protected override void OnParentChanged (EventArgs e)
		{
			base.OnParentChanged (e);
		}

		protected override void OnTextChanged (EventArgs e)
		{
			Invalidate ();
			base.OnTextChanged (e);
		}

		protected override void OnVisibleChanged (EventArgs e)
		{
			if (!Visible) {
				is_pressed = false;
				is_entered = false;
			}
			
			base.OnVisibleChanged (e);
		}

		protected void ResetFlagsandPaint ()
		{
			// Nothing to do; MS internal
			// Should we do Invalidate (); ?
		}
		#endregion	// Public Instance Properties

		#region	Public Events
#if NET_2_0
		[Browsable (true)]
		[EditorBrowsable (EditorBrowsableState.Always)]
		public new event EventHandler AutoSizeChanged {
			add { base.AutoSizeChanged += value; }
			remove { base.AutoSizeChanged -= value; }
		}
#endif

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event EventHandler ImeModeChanged {
			add { base.ImeModeChanged += value; }
			remove { base.ImeModeChanged -= value; }
		}
		#endregion	// Events

		#region Internal Properties
		internal bool Pressed {
			get { return this.is_pressed; }
		}
		
		#endregion
		
		#region Internal Methods
		internal virtual void HaveDoubleClick ()
		{
			// override me
		}

		internal override void OnPaintBackgroundInternal (PaintEventArgs e)
		{
			base.OnPaintBackground (e);
		}
		#endregion	// Internal Methods

	}
}
