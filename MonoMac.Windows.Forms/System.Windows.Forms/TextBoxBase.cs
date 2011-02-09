// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:c
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
//	Peter Bartok	pbartok@novell.com
//
//

// NOT COMPLETE
#undef Debug
#undef DebugClick

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace System.Windows.Forms
{
	[ComVisible (true)]
	[DefaultBindingProperty ("Text")]
	[ClassInterface (ClassInterfaceType.AutoDispatch)]
	[DefaultEvent("TextChanged")]
	[Designer("System.Windows.Forms.Design.TextBoxBaseDesigner, " + Consts.AssemblySystem_Design)]
	public abstract partial class TextBoxBase : Control
	{
		#region Local Variables
		internal HorizontalAlignment	alignment;
		internal bool			accepts_tab;
		internal bool			accepts_return;
		internal bool			auto_size;
		internal bool			backcolor_set;
		internal CharacterCasing	character_casing;
		internal bool			hide_selection;
		int				max_length;
		internal bool			modified;
		internal char			password_char;
		internal bool			read_only;
		internal bool			word_wrap;
		internal int			caret_pos;		// position on the line our cursor is in (can be 0 = beginning of line)
		internal Timer			scroll_timer;
		internal bool			richtext;
		internal bool			show_selection;		// set to true to always show selection, even if no focus is set
		internal ArrayList		list_links;		// currently showing links
		private bool			enable_links;		// whether links are enabled
		
		internal bool has_been_focused;

		internal int			selection_length = -1;	// set to the user-specified selection length, or -1 if none
		internal bool show_caret_w_selection;  // TextBox shows the caret when the selection is visible
		internal int			canvas_width;
		internal int			canvas_height;
		static internal int		track_width = 2;	//
		static internal int		track_border = 5;	//
		internal DateTime		click_last;
		internal int			click_point_x;
		internal int 			click_point_y;
		internal BorderStyle actual_border_style;
		internal bool shortcuts_enabled = true;
		#if Debug
		internal static bool	draw_lines = false;
		#endif

		#endregion	// Local Variables


		#region Private and Internal Methods
		internal string CaseAdjust (string s)
		{
			if (character_casing == CharacterCasing.Normal)
				return s;
			if (character_casing == CharacterCasing.Lower)
				return s.ToLower();
			return s.ToUpper();
		}

		internal override Size GetPreferredSizeCore (Size proposedSize)
		{
			return new Size (Width, Height);
		}

		internal override void HandleClick (int clicks, MouseEventArgs me)
		{
			// MS seems to fire the click event in spite of the styles they set
			bool click_set = GetStyle (ControlStyles.StandardClick);
			bool doubleclick_set = GetStyle (ControlStyles.StandardDoubleClick);

			// so explicitly set them to true first
			SetStyle (ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);

			base.HandleClick (clicks, me);

			// then revert to our previous state
			if (!click_set)
				SetStyle (ControlStyles.StandardClick, false);
			if (!doubleclick_set)
				SetStyle (ControlStyles.StandardDoubleClick, false);
		}

		#endregion	// Private and Internal Methods

		#region Public Instance Properties
		[DefaultValue(false)]
		[MWFCategory("Behavior")]
		public bool AcceptsTab {
			get {
				return accepts_tab;
			}

			set {
				if (value != accepts_tab) {
					accepts_tab = value;
					OnAcceptsTabChanged(EventArgs.Empty);
				}
			}
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[DefaultValue(true)]
		[Localizable(true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[MWFCategory("Behavior")]
		public override bool AutoSize {
			get {
				return auto_size;
			}

			set {
				if (value != auto_size) {
					auto_size = value;
					if (auto_size) {
						if (PreferredHeight != ClientSize.Height) {
							ClientSize = new Size(ClientSize.Width, PreferredHeight);
						}
					}
				}
			}
		}

		[DispId(-501)]
		public override System.Drawing.Color BackColor {
			get {
				return base.BackColor;
			}
			set {
				backcolor_set = true;
				base.BackColor = ChangeBackColor (value);
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override System.Drawing.Image BackgroundImage {
			get {
				return base.BackgroundImage;
			}
			set {
				base.BackgroundImage = value;
			}
		}


		[DispId(-513)]
		public override System.Drawing.Color ForeColor {
			get {
				return base.ForeColor;
			}
			set {
				base.ForeColor = value;
			}
		}

		[DefaultValue(32767)]
		[Localizable(true)]
		[MWFCategory("Behavior")]
		public virtual int MaxLength {
			get {
				if (max_length == (int.MaxValue - 1)) {	// We don't distinguish between single and multi-line limits
					return 0;
				}
				return max_length;
			}

			set {
				if (value != max_length) {
					if (value == 0)
						value = int.MaxValue - 1;

					max_length = value;
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Modified {
			get {
				return modified;
			}

			set {
				if (value != modified) {
					modified = value;
					OnModifiedChanged(EventArgs.Empty);
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int PreferredHeight {
			get {
				if (BorderStyle != BorderStyle.None)
					return Font.Height + 7;

				// usually in borderless mode the top margin is 0, but
				// try to access it, in case it was set manually, as ToolStrip* controls do
				return Font.Height + TopMargin;
			}
		}

		[RefreshProperties (RefreshProperties.Repaint)]	
		[DefaultValue(false)]
		[MWFCategory("Behavior")]
		public bool ReadOnly {
			get {
				return read_only;
			}

			set {
				if (value != read_only) {
					read_only = value;
					if (!backcolor_set) {
						if (read_only)
							background_color = SystemColors.Control;
						else
							background_color = SystemColors.Window;
					}
					OnReadOnlyChanged(EventArgs.Empty);
					Invalidate ();
				}
			}
		}

		[DefaultValue (true)]
		public virtual bool ShortcutsEnabled {
			get { return shortcuts_enabled; }
			set { shortcuts_enabled = value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout {
			get { return base.BackgroundImageLayout; } 
			set { base.BackgroundImageLayout = value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public new Padding Padding {
			get { return base.Padding; }
			set { base.Padding = value; }
		}
		
		#endregion	// Public Instance Properties

		#region Protected Instance Properties
		protected override bool CanEnableIme {
			get {
				if (ReadOnly || password_char != '\0')
					return false;
					
				return true;
			}
		}

		protected override System.Drawing.Size DefaultSize {
			get {
				return new Size(100, 20);
			}
		}

		// Currently our double buffering breaks our scrolling, so don't let people enable this
		[EditorBrowsable (EditorBrowsableState.Never)]
		protected override bool DoubleBuffered {
			get { return false; }
			set { }
		}

		#endregion	// Protected Instance Properties

		#region Public Instance Methods

		public void Clear ()
		{
			Modified = false;
			Text = string.Empty;
		}


		public override string ToString ()
		{
			return String.Concat (base.ToString (), ", Text: ", Text);
		}

		public void DeselectAll ()
		{
			SelectionLength = 0;
		}

		public virtual char GetCharFromPosition (Point pt)
		{
			return GetCharFromPositionInternal (pt);
		}

		#endregion	// Public Instance Methods

		#region Protected Instance Methods
		
		protected virtual void OnAcceptsTabChanged(EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [AcceptsTabChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnBorderStyleChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [BorderStyleChangedEvent]);
			if (eh != null)
				eh (this, e);
		}


		protected override void OnHandleCreated (EventArgs e)
		{
			base.OnHandleCreated (e);
			FixupHeight ();
		}

		protected override void OnHandleDestroyed (EventArgs e)
		{
			base.OnHandleDestroyed (e);
		}

		protected virtual void OnHideSelectionChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [HideSelectionChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnModifiedChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [ModifiedChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnMultilineChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [MultilineChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected override void OnPaddingChanged (EventArgs e)
		{
			base.OnPaddingChanged (e);
		}

		protected virtual void OnReadOnlyChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [ReadOnlyChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected override bool ProcessCmdKey (ref Message msg, Keys keyData)
		{
			return base.ProcessCmdKey (ref msg, keyData);
		}
		protected override bool ProcessDialogKey (Keys keyData)
		{
			// The user can use Ctrl-Tab or Ctrl-Shift-Tab to move control focus
			// instead of inserting a Tab.  However, the focus-moving-tab-stuffs
			// doesn't work if Ctrl is pushed, so we remove it before sending it.
			if (accepts_tab && (keyData & (Keys.Control | Keys.Tab)) == (Keys.Control | Keys.Tab))
				keyData ^= Keys.Control;
				
			return base.ProcessDialogKey(keyData);
		}


		internal virtual void RaiseSelectionChanged ()
		{
			// Do nothing, overridden in RTB
		}

		#endregion	// Protected Instance Methods

		#region Events
		static object AcceptsTabChangedEvent = new object ();
		static object AutoSizeChangedEvent = new object ();
		static object BorderStyleChangedEvent = new object ();
		static object HideSelectionChangedEvent = new object ();
		static object ModifiedChangedEvent = new object ();
		static object MultilineChangedEvent = new object ();
		static object ReadOnlyChangedEvent = new object ();
		static object HScrolledEvent = new object ();
		static object VScrolledEvent = new object ();

		public event EventHandler AcceptsTabChanged {
			add { Events.AddHandler (AcceptsTabChangedEvent, value); }
			remove { Events.RemoveHandler (AcceptsTabChangedEvent, value); }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event EventHandler AutoSizeChanged {
			add { Events.AddHandler (AutoSizeChangedEvent, value); }
			remove { Events.RemoveHandler (AutoSizeChangedEvent, value); }
		}

		public event EventHandler BorderStyleChanged {
			add { Events.AddHandler (BorderStyleChangedEvent, value); }
			remove { Events.RemoveHandler (BorderStyleChangedEvent, value); }
		}

		public event EventHandler HideSelectionChanged {
			add { Events.AddHandler (HideSelectionChangedEvent, value); }
			remove { Events.RemoveHandler (HideSelectionChangedEvent, value); }
		}

		public event EventHandler ModifiedChanged {
			add { Events.AddHandler (ModifiedChangedEvent, value); }
			remove { Events.RemoveHandler (ModifiedChangedEvent, value); }
		}

		public event EventHandler MultilineChanged {
			add { Events.AddHandler (MultilineChangedEvent, value); }
			remove { Events.RemoveHandler (MultilineChangedEvent, value); }
		}

		public event EventHandler ReadOnlyChanged {
			add { Events.AddHandler (ReadOnlyChangedEvent, value); }
			remove { Events.RemoveHandler (ReadOnlyChangedEvent, value); }
		}

		internal event EventHandler HScrolled {
			add { Events.AddHandler (HScrolledEvent, value); }
			remove { Events.RemoveHandler (HScrolledEvent, value); }
		}

		internal event EventHandler VScrolled {
			add { Events.AddHandler (VScrolledEvent, value); }
			remove { Events.RemoveHandler (VScrolledEvent, value); }
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler BackgroundImageChanged {
			add { base.BackgroundImageChanged += value; }
			remove { base.BackgroundImageChanged -= value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event EventHandler BackgroundImageLayoutChanged {
			add { base.BackgroundImageLayoutChanged += value; }
			remove { base.BackgroundImageLayoutChanged -= value; }
		}

		[Browsable (true)]
		[EditorBrowsable (EditorBrowsableState.Always)]
		public new event MouseEventHandler MouseClick {
			add { base.MouseClick += value; }
			remove { base.MouseClick -= value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public new event EventHandler PaddingChanged {
			add { base.PaddingChanged += value; }
			remove { base.PaddingChanged -= value; }
		}

		[Browsable (true)]
		[EditorBrowsable (EditorBrowsableState.Always)]
		public new event EventHandler Click {
			add { base.Click += value; }
			remove { base.Click -= value; }
		}

		// XXX should this not manipulate base.Paint?
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new event PaintEventHandler Paint;
		#endregion	// Events

		#region Private Methods

		

		internal override bool ScaleChildrenInternal {
			get { return false; }
		}

		internal RightToLeft GetInheritedRtoL ()
		{
			for (Control c = this; c != null; c = c.Parent)
				if (c.RightToLeft != RightToLeft.Inherit)
					return c.RightToLeft;
			return RightToLeft.No;
		}

		private void TextBoxBase_SizeChanged (object sender, EventArgs e)
		{
			if (IsHandleCreated)
				CalculateDocument ();
		}

		private void TextBoxBase_RightToLeftChanged (object o, EventArgs e)
		{
			if (IsHandleCreated)
				CalculateDocument ();
		}


		internal virtual Color ChangeBackColor (Color backColor)
		{
			return backColor;
		}

		internal override bool IsInputCharInternal (char charCode)
		{
			return true;
		}
		#endregion	// Private Methods
		// This is called just before OnTextChanged is called.
		internal virtual void OnTextUpdate ()
		{
		}
		
		protected override void OnTextChanged (EventArgs e)
		{
			base.OnTextChanged (e);
		}

		protected override void OnMouseUp (MouseEventArgs mevent)
		{
			base.OnMouseUp (mevent);
		}
	}
}
