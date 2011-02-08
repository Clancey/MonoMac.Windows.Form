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
//	Jordi Mas i Hernandez, jordi@ximian.com
//	Mike Kestner  <mkestner@novell.com>
//	Daniel Nauck    (dna(at)mono-project(dot)de)

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
	[DefaultProperty("Items")]
	[DefaultEvent("SelectedIndexChanged")]
	[Designer ("System.Windows.Forms.Design.ComboBoxDesigner, " + Consts.AssemblySystem_Design, "System.ComponentModel.Design.IDesigner")]
	[DefaultBindingProperty ("Text")]
	[ClassInterface (ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	public partial class ComboBox : ListControl
	{
		private DrawMode draw_mode = DrawMode.Normal;
		private ComboBoxStyle dropdown_style;
		private int dropdown_width = -1;
		private int selected_index = -1;
		private ObjectCollection items;
		private bool suspend_ctrlupdate;
		private int maxdrop_items = 8;
		private bool integral_height = true;
		private bool sorted;
		private int max_length;
		private bool process_textchanged_event = true;
		private bool process_texchanged_autoscroll = true;
		private bool item_height_specified;
		private int item_height;
		private int requested_height = -1;
		private Hashtable item_heights;
		private bool show_dropdown_button;
		private ButtonState button_state = ButtonState.Normal;
		private bool dropped_down;
		private Rectangle text_area;
		private Rectangle button_area;
		private Rectangle listbox_area;
		private const int button_width = 16;
		bool drop_down_button_entered;
		private AutoCompleteStringCollection auto_complete_custom_source = null;
		private AutoCompleteMode auto_complete_mode = AutoCompleteMode.None;
		private AutoCompleteSource auto_complete_source = AutoCompleteSource.None;
		private FlatStyle flat_style;
		private int drop_down_height;
		const int default_drop_down_height = 106;

		#region events
		
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
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

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event EventHandler DoubleClick
		{
			add { base.DoubleClick += value; }
			remove { base.DoubleClick -= value; }
		}

		static object DrawItemEvent = new object ();
		static object DropDownEvent = new object ();
		static object DropDownStyleChangedEvent = new object ();
		static object MeasureItemEvent = new object ();
		static object SelectedIndexChangedEvent = new object ();
		static object SelectionChangeCommittedEvent = new object ();
		static object DropDownClosedEvent = new object ();
		static object TextUpdateEvent = new object ();

		public event DrawItemEventHandler DrawItem {
			add { Events.AddHandler (DrawItemEvent, value); }
			remove { Events.RemoveHandler (DrawItemEvent, value); }
		}

		public event EventHandler DropDown {
			add { Events.AddHandler (DropDownEvent, value); }
			remove { Events.RemoveHandler (DropDownEvent, value); }
		}
		public event EventHandler DropDownClosed
		{
			add { Events.AddHandler (DropDownClosedEvent, value); }
			remove { Events.RemoveHandler (DropDownClosedEvent, value); }
		}

		public event EventHandler DropDownStyleChanged {
			add { Events.AddHandler (DropDownStyleChangedEvent, value); }
			remove { Events.RemoveHandler (DropDownStyleChangedEvent, value); }
		}

		public event MeasureItemEventHandler MeasureItem {
			add { Events.AddHandler (MeasureItemEvent, value); }
			remove { Events.RemoveHandler (MeasureItemEvent, value); }
		}
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event EventHandler PaddingChanged
		{
			add { base.PaddingChanged += value; }
			remove { base.PaddingChanged -= value; }
		}
		
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public new event PaintEventHandler Paint {
			add { base.Paint += value; }
			remove { base.Paint -= value; }
		}
		
		public event EventHandler SelectedIndexChanged {
			add { Events.AddHandler (SelectedIndexChangedEvent, value); }
			remove { Events.RemoveHandler (SelectedIndexChangedEvent, value); }
		}

		public event EventHandler SelectionChangeCommitted {
			add { Events.AddHandler (SelectionChangeCommittedEvent, value); }
			remove { Events.RemoveHandler (SelectionChangeCommittedEvent, value); }
		}
		public event EventHandler TextUpdate
		{
			add { Events.AddHandler (TextUpdateEvent, value); }
			remove { Events.RemoveHandler (TextUpdateEvent, value); }
		}

		#endregion Events

		#region Public Properties
		[MonoTODO("AutoCompletion algorithm is currently not implemented.")]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
		[Browsable (true)]
		[EditorBrowsable (EditorBrowsableState.Always)]
		[Localizable (true)]
		[Editor ("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + Consts.AssemblySystem_Design,
			 "System.Drawing.Design.UITypeEditor, " + Consts.AssemblySystem_Drawing)]
		public AutoCompleteStringCollection AutoCompleteCustomSource { 
			get {
				if(auto_complete_custom_source == null) {
					auto_complete_custom_source = new AutoCompleteStringCollection ();
					auto_complete_custom_source.CollectionChanged += new CollectionChangeEventHandler (OnAutoCompleteCustomSourceChanged);
				}
				return auto_complete_custom_source;
			}
			set {
				if(auto_complete_custom_source == value)
					return;

				if(auto_complete_custom_source != null) //remove eventhandler from old collection
					auto_complete_custom_source.CollectionChanged -= new CollectionChangeEventHandler (OnAutoCompleteCustomSourceChanged);

				auto_complete_custom_source = value;

				if(auto_complete_custom_source != null)
					auto_complete_custom_source.CollectionChanged += new CollectionChangeEventHandler (OnAutoCompleteCustomSourceChanged);

				SetTextBoxAutoCompleteData ();
			}
		}

		[MonoTODO("AutoCompletion algorithm is currently not implemented.")]
		[Browsable (true)]
		[EditorBrowsable (EditorBrowsableState.Always)]
		[DefaultValue (AutoCompleteMode.None)]
		public AutoCompleteMode AutoCompleteMode {
			get { return auto_complete_mode; }
			set {
				if(auto_complete_mode == value)
					return;

				if((value < AutoCompleteMode.None) || (value > AutoCompleteMode.SuggestAppend))
					throw new InvalidEnumArgumentException (Locale.GetText ("Enum argument value '{0}' is not valid for AutoCompleteMode", value));

				auto_complete_mode = value;
				SetTextBoxAutoCompleteData ();
			}
		}

		[MonoTODO("AutoCompletion algorithm is currently not implemented.")]
		[Browsable (true)]
		[EditorBrowsable (EditorBrowsableState.Always)]
		[DefaultValue (AutoCompleteSource.None)]
		public AutoCompleteSource AutoCompleteSource {
			get { return auto_complete_source; }
			set {
				if(auto_complete_source == value)
					return;

				if(!Enum.IsDefined (typeof (AutoCompleteSource), value))
					throw new InvalidEnumArgumentException (Locale.GetText ("Enum argument value '{0}' is not valid for AutoCompleteSource", value));

				auto_complete_source = value;
				SetTextBoxAutoCompleteData ();
			}
		}


		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public override Image BackgroundImage {
			get { return base.BackgroundImage; }
			set {
				if (base.BackgroundImage == value)
					return;
 				base.BackgroundImage = value;
				Refresh ();
			}
		}

		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout {
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}
		
		protected override Size DefaultSize {
			get { return new Size (121, 21); }
		}

		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue (DrawMode.Normal)]
		[MWFCategory("Behavior")]
		public DrawMode DrawMode {
			get { return draw_mode; }
			set {
				if (!Enum.IsDefined (typeof (DrawMode), value))
					throw new InvalidEnumArgumentException (string.Format("Enum argument value '{0}' is not valid for DrawMode", value));

				if (draw_mode == value)
					return;

				if (draw_mode == DrawMode.OwnerDrawVariable)
					item_heights = null;
				draw_mode = value;
				if (draw_mode == DrawMode.OwnerDrawVariable)
					item_heights = new Hashtable ();
				Refresh ();
			}
		}

		[Browsable (true)]
		[DefaultValue (106)]
		[EditorBrowsable (EditorBrowsableState.Always)]
		[MWFCategory("Behavior")]
		public int DropDownHeight {
			get {
				return drop_down_height;
			}
			set {
				if (value < 1)
					throw new ArgumentOutOfRangeException ("DropDownHeight", "DropDownHeight must be greater than 0.");
					
				if (value == drop_down_height)
					return;

				drop_down_height = value;
				IntegralHeight = false;
			}
		}

		[MWFCategory("Behavior")]
		public int DropDownWidth {
			get { 
				if (dropdown_width == -1)
					return Width;
					
				return dropdown_width; 
			}
			set {
				if (dropdown_width == value)
					return;
					
				if (value < 1)
					throw new ArgumentOutOfRangeException ("DropDownWidth",
						"The DropDownWidth value is less than one.");

				dropdown_width = value;
			}
		}

		[DefaultValue (FlatStyle.Standard)]
		[Localizable (true)]
		[MWFCategory("Appearance")]
		public FlatStyle FlatStyle {
			get { return flat_style; }
			set {
				if (!Enum.IsDefined (typeof (FlatStyle), value))
					throw new InvalidEnumArgumentException ("FlatStyle", (int) value, typeof (FlatStyle));
				
				flat_style = value;
				LayoutComboBox ();
				Invalidate ();
			}
		}

		public override bool Focused {
			get { return base.Focused; }
		}

		public override Color ForeColor {
			get { return base.ForeColor; }
			set {
				if (base.ForeColor == value)
					return;
				base.ForeColor = value;
				Refresh ();
			}
		}

		[DefaultValue (true)]
		[Localizable (true)]
		[MWFCategory("Behavior")]
		public bool IntegralHeight {
			get { return integral_height; }
			set {
				if (integral_height == value)
					return;
				integral_height = value;
				UpdateComboBoxBounds ();
				Refresh ();
			}
		}

		[DefaultValue (8)]
		[Localizable (true)]
		[MWFCategory("Behavior")]
		public int MaxDropDownItems {
			get { return maxdrop_items; }
			set {
				if (maxdrop_items == value)
					return;
				maxdrop_items = value;
			}
		}

		public override Size MaximumSize {
			get { return base.MaximumSize; }
			set {
				base.MaximumSize = new Size (value.Width, 0);
			}
		}

		[DefaultValue (0)]
		[Localizable (true)]
		[MWFCategory("Behavior")]
		public int MaxLength {
			get { return max_length; }
			set {
				if (max_length == value)
					return;

				max_length = value;
				
				if (dropdown_style != ComboBoxStyle.DropDownList) {
					if (value < 0) {
						value = 0;
					}
					textbox_ctrl.MaxLength = value;
				}
			}
		}

		public override Size MinimumSize {
			get { return base.MinimumSize; }
			set {
				base.MinimumSize = new Size (value.Width, 0);
			}
		}
		
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Browsable (false)]
		public new Padding Padding  {
			get { return base.Padding; }
			set { base.Padding = value; }
		}

		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		[Browsable (false)]
		public int PreferredHeight {
			get { return Font.Height + 8; }
		}

		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public override int SelectedIndex {
			get { return selected_index; }
			set {
				SetSelectedIndex (value, false);
			}
		}
		
		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public string SelectedText {
			get {
				if (dropdown_style == ComboBoxStyle.DropDownList)
					return string.Empty;
					
				string retval = textbox_ctrl.SelectedText;
				
				return retval;
			}
			set {
				if (dropdown_style == ComboBoxStyle.DropDownList)
					return;
				textbox_ctrl.SelectedText = value;
			}
		}

		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public int SelectionLength {
			get {
				if (dropdown_style == ComboBoxStyle.DropDownList) 
					return 0;
				
				int result = textbox_ctrl.SelectionLength;
				return result == -1 ? 0 : result;
			}
			set {
				if (dropdown_style == ComboBoxStyle.DropDownList) 
					return;
				if (textbox_ctrl.SelectionLength == value)
					return;
				textbox_ctrl.SelectionLength = value;
			}
		}

		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public int SelectionStart {
			get { 
				if (dropdown_style == ComboBoxStyle.DropDownList) 
					return 0;
				return textbox_ctrl.SelectionStart;
			}
			set {
				if (dropdown_style == ComboBoxStyle.DropDownList) 
					return;
				if (textbox_ctrl.SelectionStart == value)
					return;
				textbox_ctrl.SelectionStart = value;
			}
		}

		[DefaultValue (false)]
		[MWFCategory("Behavior")]
		public bool Sorted {
			get { return sorted; }
			set {
				if (sorted == value)
					return;
				sorted = value;
				SelectedIndex = -1;
				if (sorted) {
					Items.Sort ();
					LayoutComboBox ();
				}
			}
		}

		#endregion Public Properties

		#region Internal Properties
		internal Rectangle ButtonArea {
			get { return button_area; }
		}

		internal Rectangle TextArea {
			get { return text_area; }
		}
		#endregion

		#region UIA Framework Properties


		#endregion UIA Framework Properties

		#region Public Methods
		[Obsolete ("This method has been deprecated")]
		protected virtual void AddItemsCore (object[] value)
		{
			
		}

		public void BeginUpdate ()
		{
			suspend_ctrlupdate = true;
		}


		public void EndUpdate ()
		{
			suspend_ctrlupdate = false;
			UpdatedItems ();
			Refresh ();
		}

		public int FindString (string s)
		{
			return FindString (s, -1);
		}

		public int FindString (string s, int startIndex)
		{
			if (s == null || Items.Count == 0) 
				return -1;

			if (startIndex < -1 || startIndex >= Items.Count)
				throw new ArgumentOutOfRangeException ("startIndex");

			int i = startIndex;
			if (i == (Items.Count - 1))
				i = -1;
			do {
				i++;
				if (string.Compare (s, 0, GetItemText (Items [i]), 0, s.Length, true) == 0)
					return i;
				if (i == (Items.Count - 1))
					i = -1;
			} while (i != startIndex);

			return -1;
		}

		public int FindStringExact (string s)
		{
			return FindStringExact (s, -1);
		}

		public int FindStringExact (string s, int startIndex)
		{
			return FindStringExact (s, startIndex, true);
		}

		private int FindStringExact (string s, int startIndex, bool ignoreCase)
		{
			if (s == null || Items.Count == 0) 
				return -1;

			if (startIndex < -1 || startIndex >= Items.Count)
				throw new ArgumentOutOfRangeException ("startIndex");

			int i = startIndex;
			if (i == (Items.Count - 1))
				i = -1;
			do {
				i++;
				if (string.Compare (s, GetItemText (Items [i]), ignoreCase, CultureInfo.CurrentCulture) == 0)
					return i;
				if (i == (Items.Count - 1))
					i = -1;
			} while (i != startIndex);

			return -1;
		}


		protected override bool IsInputKey (Keys keyData)
		{
			switch (keyData & ~Keys.Modifiers) {
			case Keys.Up:
			case Keys.Down:
			case Keys.Left:
			case Keys.Right:
			case Keys.PageUp:
			case Keys.PageDown:
			case Keys.Home:
			case Keys.End:
				return true;
			
			default:
				return false;
			}
		}

		protected override void OnBackColorChanged (EventArgs e)
		{
			base.OnBackColorChanged (e);

			if (textbox_ctrl != null)
				textbox_ctrl.BackColor = BackColor;
		}

		protected override void OnDataSourceChanged (EventArgs e)
		{
			base.OnDataSourceChanged (e);
			BindDataItems ();
			
			if (DataSource == null || DataManager == null) {
				SelectedIndex = -1;
			} 
			else {
				SelectedIndex = DataManager.Position;
			}
		}

		protected override void OnDisplayMemberChanged (EventArgs e)
		{
			base.OnDisplayMemberChanged (e);

			if (DataManager == null)
				return;

			SelectedIndex = DataManager.Position;

			if (selected_index != -1 && DropDownStyle != ComboBoxStyle.DropDownList)
				SetControlText (GetItemText (Items [selected_index]), true);

			if (!IsHandleCreated)
				return;

			Invalidate ();
		}

		protected virtual void OnDrawItem (DrawItemEventArgs e)
		{
			DrawItemEventHandler eh = (DrawItemEventHandler)(Events [DrawItemEvent]);
			if (eh != null)
				eh (this, e);
		}


		protected virtual void OnDropDown (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [DropDownEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnDropDownClosed (EventArgs e)
		{
			EventHandler eh = (EventHandler) Events [DropDownClosedEvent];
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnDropDownStyleChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [DropDownStyleChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected override void OnFontChanged (EventArgs e)
		{
			base.OnFontChanged (e);

			if (textbox_ctrl != null)
				textbox_ctrl.Font = Font;
			
			if (!item_height_specified)
				item_height = Font.Height + 2;

			if (IntegralHeight)
				UpdateComboBoxBounds ();

			LayoutComboBox ();
		}

		protected override void OnHandleDestroyed (EventArgs e)
		{
			base.OnHandleDestroyed (e);
		}

		protected virtual void OnMeasureItem (MeasureItemEventArgs e)
		{
			MeasureItemEventHandler eh = (MeasureItemEventHandler)(Events [MeasureItemEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected override void OnParentBackColorChanged (EventArgs e)
		{
			base.OnParentBackColorChanged (e);
		}

		protected override void OnSelectedIndexChanged (EventArgs e)
		{
			base.OnSelectedIndexChanged (e);

			EventHandler eh = (EventHandler)(Events [SelectedIndexChangedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected virtual void OnSelectedItemChanged (EventArgs e)
		{
		}

		protected override void OnSelectedValueChanged (EventArgs e)
		{
			base.OnSelectedValueChanged (e);
		}

		protected virtual void OnSelectionChangeCommitted (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [SelectionChangeCommittedEvent]);
			if (eh != null)
				eh (this, e);
		}

		protected override void RefreshItem (int index)
		{
			if (index < 0 || index >= Items.Count)
				throw new ArgumentOutOfRangeException ("index");
				
			if (draw_mode == DrawMode.OwnerDrawVariable)
				item_heights.Remove (Items [index]);
		}

		protected override void RefreshItems ()
		{
			for (int i = 0; i < Items.Count; i++) {
				RefreshItem (i);
			}

			LayoutComboBox ();
			Refresh ();

			if (selected_index != -1 && DropDownStyle != ComboBoxStyle.DropDownList)
				SetControlText (GetItemText (Items [selected_index]), false);
		}

		public override void ResetText ()
		{
			Text = String.Empty;
		}
		
		protected override bool ProcessKeyEventArgs (ref Message m)
		{
			return base.ProcessKeyEventArgs (ref m);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected override void OnKeyDown (KeyEventArgs e)
		{
			base.OnKeyDown (e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected override void OnValidating (CancelEventArgs e)
		{
			base.OnValidating (e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected override void OnTextChanged (EventArgs e)
		{
			base.OnTextChanged (e);
		}

		protected virtual void OnTextUpdate (EventArgs e)
		{
			EventHandler eh = (EventHandler) Events [TextUpdateEvent];
			if (eh != null)
				eh (this, e);
		}
		protected override void OnMouseLeave (EventArgs e)
		{
			if (flat_style == FlatStyle.Popup)
				Invalidate ();
			base.OnMouseLeave (e);
		}
		
		protected override void OnMouseEnter (EventArgs e)
		{
			if (flat_style == FlatStyle.Popup)
				Invalidate ();
			base.OnMouseEnter (e);
		}

		protected override void ScaleControl (SizeF factor, BoundsSpecified specified)
		{
			base.ScaleControl (factor, specified);
		}

		protected override void SetItemCore (int index, object value)
		{
			if (index < 0 || index >= Items.Count)
				return;

			Items[index] = value;
		}

		protected override void SetItemsCore (IList value)
		{
			BeginUpdate ();
			try {
				Items.Clear ();
				Items.AddRange (value);
			} finally {
				EndUpdate ();
			}
		}

		public override string ToString ()
		{
			return base.ToString () + ", Items.Count:" + Items.Count;
		}


		#endregion Public Methods

		#region Private Methods
		void OnAutoCompleteCustomSourceChanged(object sender, CollectionChangeEventArgs e) {
			if(auto_complete_source == AutoCompleteSource.CustomSource) {
				//FIXME: handle add, remove and refresh events in AutoComplete algorithm.
			}
		}
		
		private int FindStringCaseInsensitive (string search)
		{
			if (search.Length == 0) {
				return -1;
			}
			
			for (int i = 0; i < Items.Count; i++) 
			{
				if (String.Compare (GetItemText (Items[i]), 0, search, 0, search.Length, true) == 0)
					return i;
			}

			return -1;
		}

		// Search in the list for the substring, starting the search at the list 
		// position specified, the search wraps thus covering all the list.
		internal int FindStringCaseInsensitive (string search, int start_index)
		{
			if (search.Length == 0) {
				return -1;
			}
			// Accept from first item to after last item. i.e. all cases of (SelectedIndex+1).
			if (start_index < 0 || start_index > Items.Count)
				throw new ArgumentOutOfRangeException("start_index");

			for (int i = 0; i < Items.Count; i++) {
				int index = (i + start_index) % Items.Count;
				if (String.Compare (GetItemText (Items [index]), 0, search, 0, search.Length, true) == 0)
					return index;
			}

			return -1;
		}

		internal override bool IsInputCharInternal (char charCode)
		{
			return true;
		}



		// If no item is currently selected, and an item is found matching the text 
		// in the textbox, then selected that item.  Otherwise the item at the given 
		// index is selected.
		private void FindMatchOrSetIndex(int index)
		{
			int match = -1;
			if (SelectedIndex == -1 && Text.Length != 0)
				match = FindStringCaseInsensitive(Text);
			if (match != -1)
				SetSelectedIndex (match, true);
			else
				SetSelectedIndex (index, true);
		}
		
		void OnMouseDownCB (object sender, MouseEventArgs e)
		{
			Rectangle area;
			if (DropDownStyle == ComboBoxStyle.DropDownList)
				area = ClientRectangle;
			else
				area = button_area;

			if (area.Contains (e.X, e.Y)) {
				if (Items.Count > 0)
					DropDownListBox ();
				else {
					button_state = ButtonState.Pushed;
					OnDropDown (EventArgs.Empty);
				}
				
				Invalidate (button_area);
				Update ();
			}
			Capture = true;
		}

		internal override void OnPaintInternal (PaintEventArgs pevent)
		{
			if (suspend_ctrlupdate)
				return;

			Draw (ClientRectangle, pevent.Graphics);
		}
		
		private void OnTextBoxClick (object sender, EventArgs e)
		{
			OnClick (e);
		}
		
		internal void SetControlText (string s, bool suppressTextChanged)
		{
			SetControlText (s, suppressTextChanged, false);
		}

		#endregion Private Methods



	}
}
