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
//	Jackson Harper (jackson@ximian.com)


using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;
//using System.Windows.Forms.Theming;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms {
	[ComVisibleAttribute (true)]
	[ClassInterfaceAttribute (ClassInterfaceType.AutoDispatch)]
	[DefaultEvent("SelectedIndexChanged")]
	[DefaultProperty("TabPages")]
	[Designer("System.Windows.Forms.Design.TabControlDesigner, " + Consts.AssemblySystem_Design, "System.ComponentModel.Design.IDesigner")]
	public partial class TabControl : Control {
		#region Fields
		private int selected_index = -1;
		private TabAlignment alignment;
		private TabAppearance appearance;
		private TabDrawMode draw_mode;
		private bool multiline;
		private ImageList image_list;
		private Size item_size = Size.Empty;
		private bool item_size_manual;
		private Point padding;
		private int row_count = 0;
		private bool hottrack;
		private TabPageCollection tab_pages;
		private bool show_tool_tips;
		private TabSizeMode size_mode;
		private bool show_slider = false;
		private PushButtonState right_slider_state = PushButtonState.Normal;
		private PushButtonState left_slider_state = PushButtonState.Normal;
		private int slider_pos = 0;
		TabPage entered_tab_page;
		bool mouse_down_on_a_tab_page;
		//ToolTip tooltip;
		//ToolTip.TipState tooltip_state = ToolTip.TipState.Down;
		//Timer tooltip_timer;

		private bool rightToLeftLayout;
		#endregion	// Fields

		#region UIA Framework Events
		static object UIAHorizontallyScrollableChangedEvent = new object ();

		internal event EventHandler UIAHorizontallyScrollableChanged {
			add { Events.AddHandler (UIAHorizontallyScrollableChangedEvent, value); }
			remove { Events.RemoveHandler (UIAHorizontallyScrollableChangedEvent, value); }
		}

		internal void OnUIAHorizontallyScrollableChanged (EventArgs e)
		{
			EventHandler eh
				= (EventHandler) Events [UIAHorizontallyScrollableChangedEvent];
			if (eh != null)
				eh (this, e);
		}

		static object UIAHorizontallyScrolledEvent = new object ();

		internal event EventHandler UIAHorizontallyScrolled {
			add { Events.AddHandler (UIAHorizontallyScrolledEvent, value); }
			remove { Events.RemoveHandler (UIAHorizontallyScrolledEvent, value); }
		}

		internal void OnUIAHorizontallyScrolled (EventArgs e)
		{
			EventHandler eh
				= (EventHandler) Events [UIAHorizontallyScrolledEvent];
			if (eh != null)
				eh (this, e);
		}
		#endregion


		#region Public Instance Properties
		[DefaultValue(TabAlignment.Top)]
		[Localizable(true)]
		[RefreshProperties(RefreshProperties.All)]
		public TabAlignment Alignment {
			get { return alignment; }
			set {
				if (alignment == value)
					return;
				alignment = value;
				if (alignment == TabAlignment.Left || alignment == TabAlignment.Right)
					multiline = true;
				Redraw ();
			}
		}

		[DefaultValue(TabAppearance.Normal)]
		[Localizable(true)]
		public TabAppearance Appearance {
			get { return appearance; }
			set {
				if (appearance == value)
					return;
				appearance = value;
				Redraw ();
			}
		}



		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Image BackgroundImage {
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout {
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}
		

		[EditorBrowsable (EditorBrowsableState.Never)]
		protected override bool DoubleBuffered {
			get { return base.DoubleBuffered; }
			set { base.DoubleBuffered = value; }
		}

		[DefaultValue(TabDrawMode.Normal)]
		public TabDrawMode DrawMode {
			get { return draw_mode; }
			set {
				if (draw_mode == value)
					return;
				draw_mode = value;
				Redraw ();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}

		[DefaultValue(false)]
		public bool HotTrack {
			get { return hottrack; }
			set {
				if (hottrack == value)
					return;
				hottrack = value;
				Redraw ();
			}
		}

		[RefreshProperties (RefreshProperties.Repaint)]
		[DefaultValue(null)]
		public ImageList ImageList {
			get { return image_list; }
			set { 
				image_list = value; 
				Redraw ();
			}
		}

		[Localizable(true)]
		public Size ItemSize {
			get {
				if (item_size_manual)
					return item_size;

				if (!IsHandleCreated)
					return Size.Empty;

				Size size = item_size;
				if (SizeMode != TabSizeMode.Fixed) {
					size.Width += padding.X * 2;
					size.Height += padding.Y * 2;
				}

				if (tab_pages.Count == 0)
					size.Width = 0;

				return size;
			}
			set {
				if (value.Height < 0 || value.Width < 0)
					throw new ArgumentException ("'" + value + "' is not a valid value for 'ItemSize'.");
				item_size = value;
				item_size_manual = true;
				Redraw ();
			}
		}

		[DefaultValue(false)]
		public bool Multiline {
			get { return multiline; }
			set {
				if (multiline == value)
					return;
				multiline = value;
				if (!multiline && alignment == TabAlignment.Left || alignment == TabAlignment.Right)
					alignment = TabAlignment.Top;
				Redraw ();
			}
		}

		[Localizable(true)]
		public
		new
		Point Padding {
			get { return padding; }
			set {
				if (value.X < 0 || value.Y < 0)
					throw new ArgumentException ("'" + value + "' is not a valid value for 'Padding'.");
				if (padding == value)
					return;
				padding = value;
				Redraw ();
			}

		}

		[MonoTODO ("RTL not supported")]
		[Localizable (true)]
		[DefaultValue (false)]
		public virtual bool RightToLeftLayout {
			get { return this.rightToLeftLayout; }
			set {
				if (value != this.rightToLeftLayout) {
					this.rightToLeftLayout = value;
					this.OnRightToLeftLayoutChanged (EventArgs.Empty);
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int RowCount {
			get { return row_count; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TabPage SelectedTab {
			get {
				if (selected_index == -1)
					return null;
				return tab_pages [selected_index];
			}
			set {
				int index = IndexForTabPage (value);
				if (index == selected_index)
					return;
				SelectedIndex = index;
			}
		}

		[DefaultValue(false)]
		[Localizable(true)]
		public bool ShowToolTips {
			get { return show_tool_tips; }
			set {
				if (show_tool_tips == value)
					return;
				show_tool_tips = value;
			}
		}

		[DefaultValue(TabSizeMode.Normal)]
		[RefreshProperties(RefreshProperties.Repaint)]
		public TabSizeMode SizeMode {
			get { return size_mode; }
			set {
				if (size_mode == value)
					return;
				size_mode = value;
				Redraw ();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TabCount {
			get {
				return tab_pages.Count;
			}
		}

		[Editor ("System.Windows.Forms.Design.TabPageCollectionEditor, " + Consts.AssemblySystem_Design, typeof (System.Drawing.Design.UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MergableProperty(false)]
		public TabPageCollection TabPages {
			get { return tab_pages; }
		}

		[Browsable(false)]
		[Bindable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		#endregion	// Public Instance Properties

		#region Internal Properties
		internal bool ShowSlider {
			get { return show_slider; }
			set {
				show_slider = value;

				// UIA Framework Event: HorizontallyScrollable Changed
				OnUIAHorizontallyScrollableChanged (EventArgs.Empty);
			}
		}
		
		#endregion	// Internal Properties

		#region Protected Instance Properties
		protected override CreateParams CreateParams {
			get {
				CreateParams c = base.CreateParams;
				return c;
			}
		}

		protected override Size DefaultSize {
			get { return new Size (200, 100); }  
		}

		#endregion	// Protected Instance Properties

		#region Public Instance Methods
		public Rectangle GetTabRect (int index)
		{
			TabPage page = GetTab (index);
			return page.TabBounds;
		}

		public Control GetControl (int index)
		{
			return GetTab (index);
		}

		public void SelectTab (TabPage tabPage)
		{
			if (tabPage == null)
				throw new ArgumentNullException ("tabPage");

			SelectTab (this.tab_pages [tabPage]);
		}

		public void SelectTab (string tabPageName)
		{
			if (tabPageName == null)
				throw new ArgumentNullException ("tabPageName");

			SelectTab (this.tab_pages [tabPageName]);
		}

		public void SelectTab (int index)
		{
			if (index < 0 || index > this.tab_pages.Count - 1)
				throw new ArgumentOutOfRangeException ("index");
				
			SelectedIndex = index;
		}

		public void DeselectTab (TabPage tabPage)
		{
			if (tabPage == null)
				throw new ArgumentNullException ("tabPage");

			DeselectTab (this.tab_pages [tabPage]);
		}

		public void DeselectTab (string tabPageName)
		{
			if (tabPageName == null)
				throw new ArgumentNullException ("tabPageName");

			DeselectTab (this.tab_pages [tabPageName]);
		}

		public void DeselectTab (int index)
		{
			if (index == SelectedIndex) {
				if (index >= 0 && index < this.tab_pages.Count - 1)
					SelectedIndex = ++index;
				else
					SelectedIndex = 0;
			}
		}


		public override string ToString ()
		{
			string res = String.Concat (base.ToString (),
					", TabPages.Count: ",
					TabCount);
			if (TabCount > 0)
				res = String.Concat (res, ", TabPages[0]: ",
						TabPages [0]);
			return res;
		}

		#endregion	// Public Instance Methods

		#region Protected Instance Methods

		#region Handles
		protected override Control.ControlCollection CreateControlsInstance ()
		{
			return new TabControl.ControlCollection (this);
		}

		protected override void OnHandleCreated (EventArgs e)
		{
			base.OnHandleCreated (e);
		}

		protected override void OnHandleDestroyed (EventArgs e)
		{
			base.OnHandleDestroyed (e);
		}

		protected override void Dispose (bool disposing)
		{
			//CloseToolTip ();
			base.Dispose (disposing);
		}

		#endregion

		#region Events
		protected virtual void OnDrawItem (DrawItemEventArgs e)
		{
			if (DrawMode != TabDrawMode.OwnerDrawFixed)
				return;

			DrawItemEventHandler eh = (DrawItemEventHandler)(Events [DrawItemEvent]);
			if (eh != null)
				eh (this, e);
		}

		internal void OnDrawItemInternal (DrawItemEventArgs e)
		{
			OnDrawItem (e);
		}

		protected override void OnFontChanged (EventArgs e)
		{
			base.OnFontChanged (e);
			ResizeTabPages ();
		}

		protected override void OnResize (EventArgs e)
		{
			base.OnResize (e);
		}

		protected override void OnStyleChanged (EventArgs e)
		{
			base.OnStyleChanged (e);
		}

		protected virtual void OnSelectedIndexChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler) (Events[SelectedIndexChangedEvent]);
			if (eh != null)
				eh (this, e);
		}



		protected override void OnEnter (EventArgs e)
		{
			base.OnEnter (e);
			if (SelectedTab != null)
				SelectedTab.FireEnter ();
		}

		protected override void OnLeave (EventArgs e)
		{
			if (SelectedTab != null)
				SelectedTab.FireLeave ();
			base.OnLeave (e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected virtual void OnRightToLeftLayoutChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler) (Events[RightToLeftLayoutChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		protected override void ScaleCore (float dx, float dy)
		{
			base.ScaleCore (dx, dy);
		}

		protected virtual void OnDeselecting (TabControlCancelEventArgs e)
		{
			TabControlCancelEventHandler eh = (TabControlCancelEventHandler) (Events[DeselectingEvent]);
			if (eh != null)
				eh (this, e);

			if (!e.Cancel)
				OnDeselected (new TabControlEventArgs (SelectedTab, selected_index, TabControlAction.Deselected));
		}

		protected virtual void OnDeselected (TabControlEventArgs e)
		{
			TabControlEventHandler eh = (TabControlEventHandler) (Events[DeselectedEvent]);
			if (eh != null)
				eh (this, e);

			if (this.SelectedTab != null)
				this.SelectedTab.FireLeave ();
		}

		protected virtual void OnSelecting (TabControlCancelEventArgs e)
		{
			TabControlCancelEventHandler eh = (TabControlCancelEventHandler) (Events[SelectingEvent]);
			if (eh != null)
				eh (this, e);

			if (!e.Cancel)
				OnSelected (new TabControlEventArgs (SelectedTab, selected_index, TabControlAction.Selected));
		}

		protected virtual void OnSelected (TabControlEventArgs e)
		{
			TabControlEventHandler eh = (TabControlEventHandler) (Events[SelectedEvent]);
			if (eh != null)
				eh (this, e);

			if (this.SelectedTab != null)
				this.SelectedTab.FireEnter ();
		}

		#endregion

		#region Keys

		protected override void OnKeyDown (KeyEventArgs ke)
		{
			base.OnKeyDown (ke);
			if (ke.Handled)
				return;

			if (ke.KeyCode == Keys.Tab && (ke.KeyData & Keys.Control) != 0) {
				if ((ke.KeyData & Keys.Shift) == 0)
					SelectedIndex = (SelectedIndex + 1) % TabCount;
				else
					SelectedIndex = (SelectedIndex + TabCount - 1) % TabCount;
				ke.Handled = true;
			} else if (ke.KeyCode == Keys.Home) {
				SelectedIndex = 0;
				ke.Handled = true;
			} else if (ke.KeyCode == Keys.End) {
				SelectedIndex = TabCount - 1;
				ke.Handled = true;
			} else if (NavigateTabs (ke.KeyCode))
				ke.Handled = true;
		}

		protected override bool IsInputKey (Keys keyData)
		{
			switch (keyData & Keys.KeyCode) {
			case Keys.Home:
			case Keys.End:
			case Keys.Left:
			case Keys.Right:
			case Keys.Up:
			case Keys.Down:
				return true;
			}
			return base.IsInputKey (keyData);
		}
		
		private bool NavigateTabs (Keys keycode)
		{
			bool move_left = false;
			bool move_right = false;
			
			if (alignment == TabAlignment.Bottom || alignment == TabAlignment.Top) {
				if (keycode == Keys.Left)
					move_left = true;
				else if (keycode == Keys.Right)
					move_right = true;
			} else {
				if (keycode == Keys.Up)
					move_left = true;
				else if (keycode == Keys.Down)
					move_right = true;
			}
				
			if (move_left) {
				if (SelectedIndex > 0) {
					SelectedIndex--;
					return true;
				}
			}
			
			if (move_right) {
				if (SelectedIndex < TabCount - 1) {
					SelectedIndex++;
					return true;
				}
			}
			
			return false;
		}
		#endregion

		#region Pages Collection
		protected void RemoveAll ()
		{
			Controls.Clear ();
		}

		protected virtual object [] GetItems ()
		{
			TabPage [] pages = new TabPage [Controls.Count];
			Controls.CopyTo (pages, 0);
			return pages;
		}

		protected virtual object [] GetItems (Type baseType)
		{
			object[] pages = (object[])Array.CreateInstance (baseType, Controls.Count);
			Controls.CopyTo (pages, 0);
			return pages;
		}
		#endregion

		protected void UpdateTabSelection (bool updateFocus)
		{
			ResizeTabPages ();
		}

		protected string GetToolTipText (object item)
		{
			TabPage page = (TabPage) item;
			return page.ToolTipText;
		}
		#endregion	// Protected Instance Methods

		#region Internal & Private Methods
		private bool CanScrollRight {
			get {
				return (slider_pos < TabCount - 1);
			}
		}

		private bool CanScrollLeft {
			get { return slider_pos > 0; }
		}

		static PushButtonState GetScrollButtonState (Rectangle scrollButtonArea, Point cursorLocation)
		{
			return scrollButtonArea.Contains (cursorLocation) ? PushButtonState.Hot : PushButtonState.Normal;
		}

		private void SizeChangedHandler (object sender, EventArgs e)
		{
			Redraw ();
		}

		internal int IndexForTabPage (TabPage page)
		{
			for (int i = 0; i < tab_pages.Count; i++) {
				if (page == tab_pages [i])
					return i;
			}
			return -1;
		}

		private int BottomRow {
			get { return 1; }
		}

		private int Direction
		{
			get {
				return 1;
			}
		}

		private void DropRow (int row)
		{
			if (Appearance != TabAppearance.Normal)
				return;

			int bottom = BottomRow;
			int direction = Direction;

			foreach (TabPage page in TabPages) {
				if (page.Row == row) {
					page.Row = bottom;
				} else if (direction == 1 && page.Row < row) {
					page.Row += direction;
				} else if (direction == -1 && page.Row > row) {
					page.Row += direction;
				}
			}
		}
		
		private void FillRow (int start, int end, int amount, Size spacing, bool vertical) 
		{
			if (vertical)
				FillRowV (start, end, amount, spacing);
			else
				FillRow (start, end, amount, spacing);
		}

		private void FillRow (int start, int end, int amount, Size spacing)
		{
			int xpos = TabPages [start].TabBounds.Left;
			for (int i = start; i <= end; i++) {
				TabPage page = TabPages [i];
				int left = xpos;
				int width = (i == end ? Width - left - 3 : page.TabBounds.Width + amount);

				page.TabBounds = new Rectangle (left, page.TabBounds.Top,
						width, page.TabBounds.Height);
				xpos = page.TabBounds.Right + 1 + spacing.Width;
			}
		}

		private void FillRowV (int start, int end, int amount, Size spacing)
		{
			int ypos = TabPages [start].TabBounds.Top;
			for (int i = start; i <= end; i++) {
				TabPage page = TabPages [i];
				int top = ypos;
				int height = (i == end ? Height - top - 5 : page.TabBounds.Height + amount);

				page.TabBounds = new Rectangle (page.TabBounds.Left, top,
						page.TabBounds.Width, height);
				ypos = page.TabBounds.Bottom + 1;
			}
		}

		private TabPage GetTab (int index)
		{
			return Controls [index] as TabPage;
		}

		private void SetTab (int index, TabPage value)
		{
			if (!tab_pages.Contains (value)) {
				this.Controls.Add (value);
			}
			this.Controls.RemoveAt (index);
			this.Controls.SetChildIndex (value, index);
			Redraw ();
		}
		private void InsertTab (int index, TabPage value)
		{
			if (!tab_pages.Contains (value)) {
				this.Controls.Add (value);
			}
			this.Controls.SetChildIndex (value, index);
			Redraw ();
		}
		internal void Redraw ()
		{
			if (!IsHandleCreated)
				return;

			ResizeTabPages ();
			Refresh ();
		}

		private int MeasureStringWidth (Graphics graphics, string text, Font font) 
		{
			if (text == String.Empty)
				return 0;
			StringFormat format = new StringFormat();
			RectangleF rect = new RectangleF(0, 0, 1000, 1000);
			CharacterRange[] ranges = { new CharacterRange(0, text.Length) };
			Region[] regions = new Region[1];

			format.SetMeasurableCharacterRanges(ranges);
			format.FormatFlags = StringFormatFlags.NoClip;
			format.FormatFlags |= StringFormatFlags.NoWrap;
			regions = graphics.MeasureCharacterRanges(text + "I", font, rect, format);
			rect = regions[0].GetBounds(graphics);

			return (int)(rect.Width);
		}

		#endregion	// Internal & Private Methods

		#region Events
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler BackColorChanged {
			add { base.BackColorChanged += value; }
			remove { base.BackColorChanged -= value; }
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler BackgroundImageChanged {
			add { base.BackgroundImageChanged += value; }
			remove { base.BackgroundImageChanged -= value; }
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event EventHandler BackgroundImageLayoutChanged
		{
			add { base.BackgroundImageLayoutChanged += value; }
			remove { base.BackgroundImageLayoutChanged -= value; }
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler ForeColorChanged {
			add { base.ForeColorChanged += value; }
			remove { base.ForeColorChanged -= value; }
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new event PaintEventHandler Paint {
			add { base.Paint += value; }
			remove { base.Paint -= value; }
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler TextChanged {
			add { base.TextChanged += value; }
			remove { base.TextChanged -= value; }
		}

		static object DrawItemEvent = new object ();
		static object SelectedIndexChangedEvent = new object ();

		public event DrawItemEventHandler DrawItem {
			add { Events.AddHandler (DrawItemEvent, value); }
			remove { Events.RemoveHandler (DrawItemEvent, value); }
		}

		public event EventHandler SelectedIndexChanged {
			add { Events.AddHandler (SelectedIndexChangedEvent, value); }
			remove { Events.RemoveHandler (SelectedIndexChangedEvent, value); }
		}
		
		static object SelectedEvent = new object ();
		
		public event TabControlEventHandler Selected {
			add { Events.AddHandler (SelectedEvent, value); }
			remove { Events.RemoveHandler (SelectedEvent, value); }
		}

		static object DeselectedEvent = new object ();

		public event TabControlEventHandler Deselected
		{
			add { Events.AddHandler (DeselectedEvent, value); }
			remove { Events.RemoveHandler (DeselectedEvent, value); }
		}

		static object SelectingEvent = new object ();

		public event TabControlCancelEventHandler Selecting
		{
			add { Events.AddHandler (SelectingEvent, value); }
			remove { Events.RemoveHandler (SelectingEvent, value); }
		}

		static object DeselectingEvent = new object ();

		public event TabControlCancelEventHandler Deselecting
		{
			add { Events.AddHandler (DeselectingEvent, value); }
			remove { Events.RemoveHandler (DeselectingEvent, value); }
		}

		static object RightToLeftLayoutChangedEvent = new object ();
		public event EventHandler RightToLeftLayoutChanged
		{
			add { Events.AddHandler (RightToLeftLayoutChangedEvent, value); }
			remove { Events.RemoveHandler (RightToLeftLayoutChangedEvent, value); }
		}
		#endregion	// Events


	}
}


