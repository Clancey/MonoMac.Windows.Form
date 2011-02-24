using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace System.Windows.Forms
{
	public partial class TextBoxBase : Control
	{
		internal TextBoxHelper m_helper;
		
		protected override void CreateHandle ()
		{	
      		m_helper = new TextBoxHelper();
			m_view =  m_helper;
			m_helper.Host = this;
			m_helper.TextView.Selectable = true;
			m_helper.TextView.Editable = true;
			Multiline = false;			
			m_helper.TextView.VerticallyResizable = false;
			m_helper.TextView.HorizontallyResizable = true;
			m_helper.TextView.AutoresizingMask = (NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable );
			m_helper.TextView.TextContainer.ContainerSize = new SizeF(float.MaxValue,float.MaxValue);
			m_helper.TextView.TextContainer.WidthTracksTextView = false;
			
			
			//m_helper.ScaleUnitSquareToSize(Util.ScaleSize);
			//m_helper.viewDidMoveToSuperview += delegate(object sender, EventArgs e) {
			//	ViewDidMoveToSuperview();
			//};
		}
		
		/*
		internal override void CreateHelper ()
		{			
			m_helper =  new TextBoxMouseView();
			m_helper.Host = this;
			m_helper.Selectable = true;
			m_helper.Editable = true;
			Multiline = false;	
			m_helper.viewDidMoveToSuperview += delegate(object sender, EventArgs e) {
				ViewDidMoveToSuperview();
			};
		}
		*/
		
		
		#region Internal Constructor
		// Constructor will go when complete, only for testing - pdb
		internal TextBoxBase () : base()
		{
			alignment = HorizontalAlignment.Left;
			accepts_return = false;
			accepts_tab = false;
			auto_size = true;
			actual_border_style = BorderStyle.Fixed3D;
			character_casing = CharacterCasing.Normal;
			hide_selection = true;
			max_length = short.MaxValue;
			password_char = '\0';
			read_only = false;
			word_wrap = true;
			richtext = false;
			show_selection = false;
			enable_links = false;
			list_links = new ArrayList ();
			show_caret_w_selection = (this is TextBox);
			//document.CaretMoved += new EventHandler(CaretMoved);
			click_last = DateTime.Now;

			//MouseDown += new MouseEventHandler(TextBoxBase_MouseDown);
			//MouseUp += new MouseEventHandler(TextBoxBase_MouseUp);
			//MouseMove += new MouseEventHandler(TextBoxBase_MouseMove);
			SizeChanged += new EventHandler(TextBoxBase_SizeChanged);
			//FontChanged += new EventHandler(TextBoxBase_FontOrColorChanged);
			//ForeColorChanged += new EventHandler(TextBoxBase_FontOrColorChanged);
			//MouseWheel += new MouseEventHandler(TextBoxBase_MouseWheel);
			RightToLeftChanged += new EventHandler (TextBoxBase_RightToLeftChanged);

			SuspendLayout ();
			ResumeLayout ();
			
			SetStyle(ControlStyles.UserPaint | ControlStyles.StandardClick, false);
			SetStyle(ControlStyles.UseTextForAccessibility, false);
			
			base.SetAutoSizeMode (AutoSizeMode.GrowAndShrink);

			canvas_width = ClientSize.Width;
			canvas_height = ClientSize.Height;

		}
		#endregion	// Internal Constructor
		
		#region Private Instance Properties
		private bool multiline;
		
		#endregion
		
		#region Public Instance Properties
		[DefaultValue(BorderStyle.Fixed3D)]
		[DispId(-504)]
		[MWFCategory("Appearance")]
		public BorderStyle BorderStyle {
			get { return actual_border_style; }
			set {
				if (value == actual_border_style)
					return;

				if (actual_border_style != BorderStyle.Fixed3D || value != BorderStyle.Fixed3D)
					Invalidate ();

				actual_border_style = value;

				if (value != BorderStyle.Fixed3D)
					value = BorderStyle.None;

				OnBorderStyleChanged(EventArgs.Empty);
			}
		}
		
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanUndo {
			get {
				//TODO: make work
				return true;
			}
		}
		
		[DefaultValue(true)]
		[MWFCategory("Behavior")]
		public bool HideSelection {
			get {
				return hide_selection;
			}

			set {
				if (value != hide_selection) {
					hide_selection = value;
					OnHideSelectionChanged(EventArgs.Empty);
				}
				// TODO: Make Work
				//document.selection_visible = !hide_selection;
				//document.InvalidateSelectionArea();
			}
		}
		
		[MergableProperty (false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Editor("System.Windows.Forms.Design.StringArrayEditor, " + Consts.AssemblySystem_Design, typeof(System.Drawing.Design.UITypeEditor))]
		[Localizable(true)]
		[MWFCategory("Appearance")]		
		public string[] Lines
		{
			get{return m_helper.TextView.Value.Split(new char[] { '\n' });}
			set{m_helper.TextView.Value = "";
				foreach(var val in value)
				{ m_helper.TextView.Value += val + "\n";}
			}
		}

		[DefaultValue(false)]
		[Localizable(true)]
		[RefreshProperties(RefreshProperties.All)]
		[MWFCategory("Behavior")]
		public virtual bool Multiline {
			get {
				return multiline;
			}

			set {
				if (value != multiline) {
					multiline = value;

					if (this is TextBox)
						SetStyle (ControlStyles.FixedHeight, !value);

					// SetBoundsCore overrides the Height for multiline if it needs to,
					// so we don't need to worry about it here.
					//SetBoundsCore (Left, Top, Width, ExplicitBounds.Height, BoundsSpecified.None);
					
					if (Parent != null)
						Parent.PerformLayout ();

					OnMultilineChanged(EventArgs.Empty);
				}
				m_helper.UpdateTextView();

				if (IsHandleCreated)
					CalculateDocument ();
			}
		}
		
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string SelectedText {
			get {
				return m_helper.SelectedText;
			}

			set {
				if (value == null)
					value = String.Empty;
				
				selection_length = value.Length;
				m_helper.SelectedText = value;

				ScrollToCaret();
				OnTextChanged(EventArgs.Empty);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int SelectionLength {
			get {
				int res = m_helper.TextView.SelectedRange.Length;

				return res;
			}

			set {
				if (value < 0) {
					string msg = String.Format ("'{0}' is not a valid value for 'SelectionLength'", value);
					throw new ArgumentOutOfRangeException ("SelectionLength", msg);
				}

				if (value != 0) {

					selection_length = value;
					m_helper.TextView.SelectedRange = new NSRange(m_helper.TextView.SelectedRange.Location,value);
				} else {
					selection_length = -1;
					m_helper.TextView.SelectedRange = new NSRange(0,0);
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectionStart {
			get {
				return m_helper.TextView.SelectedRange.Location;
			}

			set {
				if (value < 0) {
					string msg = String.Format ("'{0}' is not a valid value for 'SelectionStart'", value);
					throw new ArgumentOutOfRangeException ("SelectionStart", msg);
				}

				// If SelectionStart has been used, we don't highlight on focus
				has_been_focused = true;				
				m_helper.TextView.SelectedRange = new NSRange(value,m_helper.TextView.SelectedRange.Length);
				ScrollToCaret ();
			}
		}
		
		
		[Editor ("System.ComponentModel.Design.MultilineStringEditor, " + Consts.AssemblySystem_Design,
			 "System.Drawing.Design.UITypeEditor, " + Consts.AssemblySystem_Drawing)]
		[Localizable(true)]
		public override string Text {
			get {
					return m_helper.TextView.Value;
			}

			set {
				// reset to force a select all next time the box gets focus
				has_been_focused = false;

				if (value == Text)
					return;

				m_helper.TextView.Value = string.Empty;
				if ((value != null) && (value != "")) {
				m_helper.TextView.Value = value;
				} else {
					if (IsHandleCreated) {
						CalculateDocument ();
					}
				}

				ScrollToCaret ();

				OnTextChanged(EventArgs.Empty);
			}
		}

		[Browsable(false)]
		public virtual int TextLength {
			get {
				return Text.Length;
			}
		}

		[DefaultValue(true)]
		[Localizable(true)]
		[MWFCategory("Behavior")]
		public bool WordWrap {
			get {
				return word_wrap;
			}

			set {
				if (value != word_wrap) {
					if (multiline) {
						word_wrap = value;
					}
					CalculateDocument ();
				}
			}
		}
		
		#endregion
		
		#region Private Methods
		

		internal void CalculateDocument()
		{
			m_helper.UpdateTextView();

			Invalidate();
		}
		
		internal bool EnableLinks {
			get { return enable_links; }
			set {
				enable_links = value;

				m_helper.TextView.AutomaticLinkDetectionEnabled = value;
			}
		}
		
		private void FixupHeight ()
		{
			if (!richtext) {
				if (!multiline) {
					if (PreferredHeight != ClientSize.Height) {
						ClientSize = new Size (ClientSize.Width, PreferredHeight);
					}
				}
			}
		}
		internal bool ShowSelection {
			get {
				if (show_selection || !hide_selection) {
					return true;
				}

				return has_focus;
			}

			set {
				if (show_selection == value)
					return;

				show_selection = value;
				// Currently InvalidateSelectionArea is commented out so do a full invalidate
			}
		}
		
		//TODO: Implement
		internal int TopMargin {
			get {
				return 0;
			}
			set {
				return;
			}
		}
		
		
		internal override void PaintControlBackground (PaintEventArgs pevent)
		{
			base.PaintControlBackground (pevent);
		}
		#endregion
		#region Public Instance Methods
		public void AppendText (string text)
		{
			// Save some cycles and only check the Text if we are one line
			bool is_empty = Lines.Length == 1 && Text == String.Empty; 
			
			m_helper.TextView.Value += text;
			
			if (!is_empty)
				ScrollToCaret ();

			//
			// Avoid the initial focus selecting all when append text is used
			//
			has_been_focused = true;

			Modified = false;
			OnTextChanged(EventArgs.Empty);
		}
		
		

		public void ClearUndo ()
		{
			//TODO:
		}
		

		public void Copy ()
		{
			m_helper.TextView.Copy(this);
		}

		public void Cut ()
		{
			m_helper.TextView.Cut(this);

			Modified = true;
			OnTextChanged (EventArgs.Empty);
		}

		public void Paste ()
		{
			m_helper.TextView.Paste(this);
		}
		//TODO:
		public void ScrollToCaret ()
		{
			//if (IsHandleCreated)
			//	CaretMoved (this, EventArgs.Empty);
		}
		
		public void Select(int start, int length)
		{
			m_helper.TextView.SelectedRange = new NSRange(start,length);
		}

		public void SelectAll ()
		{
			m_helper.TextView.SelectAll(this);
		}

		internal void SelectAllNoScroll ()
		{
			m_helper.TextView.SelectAll(this);
		}
		
		//TODO:
		[MonoInternalNote ("Deleting is classed as Typing, instead of its own Undo event")]
		public void Undo ()
		{
			//if (m_helper.TextView.undo) {
			//	Modified = true;
			//	OnTextChanged (EventArgs.Empty);
			//}
		}
		
		
		internal virtual char GetCharFromPositionInternal (Point p)
		{
			var index = m_helper.TextView.CharacterIndex(p);
			if(index == -1)
				return char.MinValue;
			return m_helper.TextView.Value[(int)index];
		}
		
		
		public virtual int GetCharIndexFromPosition (Point pt)
		{
			return (int)m_helper.TextView.CharacterIndex(pt);
			                 
		}
		//TODO:
		public virtual Point GetPositionFromCharIndex (int index)
		{
			return Point.Empty;
		}
		
		public int GetFirstCharIndexFromLine (int lineNumber)
		{
			var lines = Lines;
			if(lines.Length <= lineNumber)
				return -1;
			return Text.IndexOf(lines[lineNumber]);
		}
		//TODO:
		public int GetFirstCharIndexOfCurrentLine ()
		{
			return -1;
			//return document.LineTagToCharIndex (document.caret.line, 0);
		}
		
		#endregion
		
		
		#region Protected Instance Methods
		
		protected override bool IsInputKey (Keys keyData)
		{
			if ((keyData & Keys.Alt) != 0)
				return base.IsInputKey(keyData);

			switch (keyData & Keys.KeyCode) {
				case Keys.Enter: {
					return (accepts_return && multiline);
				}

				case Keys.Tab: {
					if (accepts_tab && multiline)
						if ((keyData & Keys.Control) == 0)
							return true;
					return false;
				}

				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
				case Keys.Down:
				case Keys.PageUp:
				case Keys.PageDown:
				case Keys.Home:
				case Keys.End: {
					return true;
				}
			}
			return false;
		}
		
		
		protected override void OnFontChanged (EventArgs e)
		{
			base.OnFontChanged (e);

			if (auto_size && !multiline) {
				if (PreferredHeight != ClientSize.Height) {
					Height = PreferredHeight;
				}
			}
		}
		
		#endregion
		
		public virtual int GetLineFromCharIndex (int index)
		{
			//TODO:
			return -1;
		}
	}
}

