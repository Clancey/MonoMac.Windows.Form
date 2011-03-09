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
//	Peter Bartok	pbartok@novell.com
//     Daniel Nauck    (dna(at)mono-project(dot)de)
//

// NOT COMPLETE

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Windows.Forms {

	[ComVisible(true)]
	[ClassInterface (ClassInterfaceType.AutoDispatch)]
	[Designer ("System.Windows.Forms.Design.TextBoxDesigner, " + Consts.AssemblySystem_Design, "System.ComponentModel.Design.IDesigner")]
	public partial class TextBox : TextBoxBase {
		#region Variables
		private bool use_system_password_char;
		private AutoCompleteStringCollection auto_complete_custom_source;
		private AutoCompleteMode auto_complete_mode = AutoCompleteMode.None;
		private AutoCompleteSource auto_complete_source = AutoCompleteSource.None;
		private AutoCompleteListBox auto_complete_listbox;
		private string auto_complete_original_text;
		private int auto_complete_selected_index = -1;
		private List<string> auto_complete_matches;
		private ComboBox auto_complete_cb_source;
		#endregion	// Variables

		#region Private & Internal Methods


		private void TextBox_MouseWheel (object o, MouseEventArgs args)
		{
			if (auto_complete_listbox == null || !auto_complete_listbox.Visible)
				return;

			int lines = args.Delta / 120;
			auto_complete_listbox.Scroll (-lines);
		}

		internal void HideAutoCompleteList ()
		{
			if (auto_complete_listbox != null)
				auto_complete_listbox.HideListBox (false);
		}

		internal bool IsAutoCompleteAvailable {
			get {
				if (auto_complete_source == AutoCompleteSource.None || auto_complete_mode == AutoCompleteMode.None)
					return false;

				// We only support CustomSource by now, as well as an internal custom source used by ComboBox
				if (auto_complete_source != AutoCompleteSource.CustomSource)
					return false;
				IList custom_source = auto_complete_cb_source == null ? auto_complete_custom_source : (IList)auto_complete_cb_source.Items;
				if (custom_source == null || custom_source.Count == 0)
					return false;

				return true;
			}
		}

		internal ComboBox AutoCompleteInternalSource {
			get {
				return auto_complete_cb_source;
			}
			set {
				auto_complete_cb_source = value;
			}
		}

		internal bool CanNavigateAutoCompleteList {
			get {
				if (auto_complete_mode == AutoCompleteMode.None)
					return false;
				if (auto_complete_matches == null || auto_complete_matches.Count == 0)
					return false;

				bool suggest_window_visible = auto_complete_listbox != null && auto_complete_listbox.Visible;
				if (auto_complete_mode == AutoCompleteMode.Suggest && !suggest_window_visible)
					return false;

				return true;
			}
		}

		void AppendAutoCompleteMatch (int index)
		{
			Text = auto_complete_original_text + auto_complete_matches [index].Substring (auto_complete_original_text.Length);
			SelectionStart = auto_complete_original_text.Length;
			SelectionLength = auto_complete_matches [index].Length - auto_complete_original_text.Length;
		}

		// this is called when the user selects a value from the autocomplete list
		// *with* the mouse
		internal virtual void OnAutoCompleteValueSelected (EventArgs args)
		{
		}

		internal override Color ChangeBackColor (Color backColor)
		{
			if (backColor == Color.Empty) {
				if (!ReadOnly)
					backColor = SystemColors.Window;

				backcolor_set = false;
			}

			return backColor;
		}

		void OnAutoCompleteCustomSourceChanged(object sender, CollectionChangeEventArgs e) {
			if(auto_complete_source == AutoCompleteSource.CustomSource) {
				//FIXME: handle add, remove and refresh events in AutoComplete algorithm.
			}
		}
		#endregion	// Private & Internal Methods

		#region Public Instance Properties
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
			}
		}

		[MonoTODO("AutoCompletion algorithm is currently not implemented.")]
		[Browsable (true)]
		[EditorBrowsable (EditorBrowsableState.Always)]
		[DefaultValue (AutoCompleteSource.None)]
		[TypeConverter (typeof (TextBoxAutoCompleteSourceConverter))]
		public AutoCompleteSource AutoCompleteSource {
			get { return auto_complete_source; }
			set {
				if(auto_complete_source == value)
					return;

				if(!Enum.IsDefined (typeof (AutoCompleteSource), value))
					throw new InvalidEnumArgumentException (Locale.GetText ("Enum argument value '{0}' is not valid for AutoCompleteSource", value));

				auto_complete_source = value;
			}
		}
		
		[DefaultValue(false)]
		[MWFCategory("Behavior")]
		public bool AcceptsReturn {
			get {
				return accepts_return;
			}

			set {
				if (value != accepts_return) {
					accepts_return = value;
				}
			}
		}

		[DefaultValue(CharacterCasing.Normal)]
		[MWFCategory("Behavior")]
		public CharacterCasing CharacterCasing {
			get {
				return character_casing;
			}

			set {
				if (value != character_casing) {
					character_casing = value;
				}
			}
		}
		
		#endregion	// Public Instance Properties

		#region Protected Instance Methods
		protected override CreateParams CreateParams {
			get {
				return base.CreateParams;
			}
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
		}

		protected override bool IsInputKey (Keys keyData)
		{
			return base.IsInputKey (keyData);
		}

		protected override void OnGotFocus (EventArgs e)
		{
			base.OnGotFocus (e);
			if (selection_length == -1 && !has_been_focused)
				SelectAllNoScroll ();
			has_been_focused = true;
		}

		protected override void OnHandleCreated (EventArgs e)
		{
			base.OnHandleCreated (e);
		}

		protected virtual void OnTextAlignChanged (EventArgs e)
		{
			EventHandler eh = (EventHandler)(Events [TextAlignChangedEvent]);
			if (eh != null)
				eh (this, e);
		}
		#endregion	// Protected Instance Methods

		#region Events
		static object TextAlignChangedEvent = new object ();

		public event EventHandler TextAlignChanged {
			add { Events.AddHandler (TextAlignChangedEvent, value); }
			remove { Events.RemoveHandler (TextAlignChangedEvent, value); }
		}
		#endregion	// Events

		#region Private Methods
		
		private void undo_Click(object sender, EventArgs e) {
			Undo();
		}

		private void cut_Click(object sender, EventArgs e) {
			Cut();
		}

		private void copy_Click(object sender, EventArgs e) {
			Copy();
		}

		private void paste_Click(object sender, EventArgs e) {
			Paste();
		}

		private void delete_Click(object sender, EventArgs e) {
			SelectedText = string.Empty;
		}

		private void select_all_Click(object sender, EventArgs e) {
			SelectAll();
		}
		#endregion	// Private Methods

		protected override void OnBackColorChanged (EventArgs e)
		{
			base.OnBackColorChanged (e);
		}
		
		protected override void OnFontChanged (EventArgs e)
		{
			base.OnFontChanged (e);
		}

		protected override void OnHandleDestroyed (EventArgs e)
		{
			base.OnHandleDestroyed (e);
		}

		class AutoCompleteListBox : Control
		{
			TextBox owner;
			VScrollBar vscroll;
			int top_item;
			int last_item;
			internal int page_size;
			int item_height;
			int highlighted_index = -1;
			bool user_defined_size;
			bool resizing;
			Rectangle resizer_bounds;

			const int DefaultDropDownItems = 7;

			public AutoCompleteListBox (TextBox tb)
			{
				owner = tb;
				item_height = FontHeight + 2;

				vscroll = new VScrollBar ();
				vscroll.ValueChanged += VScrollValueChanged;
				Controls.Add (vscroll);

				is_visible = false;
				InternalBorderStyle = BorderStyle.FixedSingle;
			}

			public int HighlightedIndex {
				get {
					return highlighted_index;
				}
				set {
					if (value == highlighted_index)
						return;

					if (highlighted_index != -1)
						Invalidate (GetItemBounds (highlighted_index));
					highlighted_index = value;
					if (highlighted_index != -1)
						Invalidate (GetItemBounds (highlighted_index));

					if (highlighted_index != -1)
						EnsureVisible (highlighted_index);
				}
			}

			public void Scroll (int lines)
			{
				int max = vscroll.Maximum - page_size + 1;
				int val = vscroll.Value + lines;
				if (val > max)
					val = max;
				else if (val < vscroll.Minimum)
					val = vscroll.Minimum;

				vscroll.Value = val;
			}

			public void EnsureVisible (int index)
			{
				if (index < top_item) {
					vscroll.Value = index;
				} else {
					int max = vscroll.Maximum - page_size + 1;
					int rows = Height / item_height;
					if (index > top_item + rows - 1) {
						index = index - rows + 1;
						vscroll.Value = index > max ? max : index;
					}
				}
			}

			internal override bool ActivateOnShow {
				get {
					return false;
				}
			}

			void VScrollValueChanged (object o, EventArgs args)
			{
				if (top_item == vscroll.Value)
					return;

				top_item = vscroll.Value;
				last_item = GetLastVisibleItem ();
				Invalidate ();
			}

			int GetLastVisibleItem ()
			{
				int top_y = Height;

				for (int i = top_item; i < owner.auto_complete_matches.Count; i++) {
					int pos = i - top_item; // relative to visible area
					if ((pos * item_height) + item_height >= top_y)
						return i;
				}

				return owner.auto_complete_matches.Count - 1;
			}

			Rectangle GetItemBounds (int index)
			{
				int pos = index - top_item;
				Rectangle bounds = new Rectangle (0, pos * item_height, Width, item_height);
				if (vscroll.Visible)
					bounds.Width -= vscroll.Width;

				return bounds;
			}

			int GetItemAt (Point loc)
			{
				if (loc.Y > (last_item - top_item) * item_height + item_height)
					return -1;

				int retval = loc.Y / item_height;
				retval += top_item;

				return retval;
			}

			void LayoutListBox ()
			{
				int total_height = owner.auto_complete_matches.Count * item_height;
				page_size = Math.Max (Height / item_height, 1);
				last_item = GetLastVisibleItem ();

				if (Height < total_height) {
					vscroll.Visible = true;
					vscroll.Maximum = owner.auto_complete_matches.Count - 1;
					vscroll.LargeChange = page_size;
					vscroll.Location = new Point (Width - vscroll.Width, 0);
					vscroll.Height = Height - item_height;
				} else
					vscroll.Visible = false;

				resizer_bounds = new Rectangle (Width - item_height, Height - item_height,
						item_height, item_height);
			}

			public void HideListBox (bool set_text)
			{
				if (set_text)
					owner.Text = owner.auto_complete_matches [HighlightedIndex];

				Capture = false;
				Hide ();
			}

			protected override void OnResize (EventArgs args)
			{
				base.OnResize (args);

				LayoutListBox ();
				Refresh ();
			}

			protected override void OnMouseDown (MouseEventArgs args)
			{
				base.OnMouseDown (args);

				if (!resizer_bounds.Contains (args.Location))
					return;

				user_defined_size = true;
				resizing = true;
				Capture = true;
			}

		}
	}
	
	internal class TextBoxAutoCompleteSourceConverter : EnumConverter
	{
		public TextBoxAutoCompleteSourceConverter(Type type)
			: base(type)
		{ }

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			StandardValuesCollection stdv = base.GetStandardValues(context);
			AutoCompleteSource[] arr = new AutoCompleteSource[stdv.Count];
			stdv.CopyTo(arr, 0);
			AutoCompleteSource[] arr2 = Array.FindAll(arr, delegate (AutoCompleteSource value) {
				// No "ListItems" in a TextBox.
				return value != AutoCompleteSource.ListItems;
			});
			return new StandardValuesCollection(arr2);
		}
	}
}
