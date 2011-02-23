using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Linq;
using System.Collections.Generic;
namespace System.Windows.Forms
{
	public partial class ComboBox : ListControl
	{
		internal ComboBoxHelper m_helper;
		internal override NSView c_helper {
			get { return m_helper; }
			set { m_helper = value as ComboBoxHelper; }
		}

		internal override void DrawItemInternal (DrawItemEventArgs e)
		{
			OnDrawItem (e);
		}
		private ComboBoxHelper textbox_ctrl {
			get { return m_helper; }
		}
		private NSMenu listbox_ctrl {
			get { return m_helper.Menu; }
		}

		protected override void CreateHandle ()
		{
			m_helper = new ComboBoxHelper ();
			m_view = m_helper;
			m_helper.Host = this;
			m_helper.Activated += delegate(object sender, EventArgs e) {
				//TODO: implement ListControl
				if (SelectedValueChanged != null)
					SelectedValueChanged (sender, e);
				//OnSelectedIndexChanged(e);
				//OnSelectedValueChanged(e);
			};
			_dataSource = new ComboBoxDataSource (this);
		}

		internal ComboBoxHelper UIATextBox {
			get { return m_helper; }
		}

		internal ComboBoxHelper UIAComboListBox {
			get { return m_helper; }
		}

		public event EventHandler SelectedValueChanged;


		public ComboBox ()
		{
			items = new ObjectCollection (this);
			DropDownStyle = ComboBoxStyle.DropDown;
			item_height = FontHeight + 2;
			//background_color = ThemeEngine.Current.ColorWindow;
			border_style = BorderStyle.None;
			
			drop_down_height = default_drop_down_height;
			flat_style = FlatStyle.Standard;
			
		}
		/* Events */
		//MouseDown += new MouseEventHandler (OnMouseDownCB);
		//MouseUp += new MouseEventHandler (OnMouseUpCB);
		//MouseMove += new MouseEventHandler (OnMouseMoveCB);
		//MouseWheel += new MouseEventHandler (OnMouseWheelCB);
		//MouseEnter += new EventHandler (OnMouseEnter);
		//MouseLeave += new EventHandler (OnMouseLeave);
		//KeyDown +=new KeyEventHandler(OnKeyDownCB);


/*
		MouseEventArgs TranslateMouseEventArgs (MouseEventArgs args)
		{
			Point loc = PointToClient (Control.MousePosition);
			return new MouseEventArgs (args.Button, args.Clicks, loc.X, loc.Y, args.Delta);
		}
		*/		
		
		
		
				private void OnTextChangedEdit (object sender, EventArgs e)
		{
			if (process_textchanged_event == false)
				return;
			
			int item = FindStringCaseInsensitive (textbox_ctrl.StringValue);
			
			if (item == -1)
			{
				// Setting base.Text below will raise this event
				// if we found something
				OnTextChanged (EventArgs.Empty);
				return;
			}
			
			base.Text = textbox_ctrl.StringValue;
		}



		internal void SetControlText (string s, bool suppressTextChanged, bool supressAutoScroll)
		{
			if (suppressTextChanged)
				process_textchanged_event = false;
			if (supressAutoScroll)
				process_texchanged_autoscroll = false;
			
			textbox_ctrl.StringValue = s;
			//textbox_ctrl.SelectAll ();
			process_textchanged_event = true;
			process_texchanged_autoscroll = true;
		}

		#region Public Members

		[Obsolete("Not Implemented.", false)]
		[DefaultValue(ComboBoxStyle.DropDown)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[MWFCategory("Appearance")]
		public ComboBoxStyle DropDownStyle { get; set; }
		public Color BackColor { get; set; }
		private string displayMember;
		public string DisplayMember {
			get { return displayMember; }
			set { displayMember = value; }
		}
		private string valueMember;
		public string ValueMember {
			get { return valueMember; }
			set { valueMember = value; }
		}
		ComboBoxDataSource _dataSource;



		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool DroppedDown {
			get {
				if (dropdown_style == ComboBoxStyle.Simple)
					return true;
				
				return dropped_down;
			}
			set {
				if (dropdown_style == ComboBoxStyle.Simple || dropped_down == value)
					return;
				
				if (value)
					DropDownListBox ();
				else
					//TODO: make hide
					//m_helper.Menu.IsTornOff
					m_helper.PerformClick (this);
			}
		}


		[Localizable(true)]
		[MWFCategory("Behavior")]
		public int ItemHeight {
			get {
				if (item_height == -1)
				{
					item_height = (int)m_helper.ItemHeight;
				}
				return item_height;
			}
			set {
				if (value < 1)
					throw new ArgumentOutOfRangeException ("ItemHeight", "The item height value is less than one.");
				
				item_height_specified = true;
				item_height = value;
				if (IntegralHeight)
					UpdateComboBoxBounds ();
				LayoutComboBox ();
				Refresh ();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Bindable(true)]
		public object SelectedItem {
			get { return Items[m_helper.SelectedIndex]; }
			set { m_helper.SelectItem (_dataSource.IndexOfItem (m_helper, value)); }
		}
		/*
		public override int SelectedIndex {
			get { return m_helper.SelectedIndex; }
			//set { m_helper.SelectedIndex = value;}
		}
		*/

		public bool FormattingEnabled { get; set; }

		public object SelectedValue {
			get { return _dataSource.GetSelectedValue (m_helper); }
			set { _dataSource.SetSelectedValue (m_helper, value); }
		}


		[Bindable(true)]
		[Localizable(true)]
		public override string Text {
			get { return m_helper.StringValue; }
			set { m_helper.StringValue = value; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Localizable(true)]
		[Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + Consts.AssemblySystem_Design, typeof(System.Drawing.Design.UITypeEditor))]
		[MergableProperty(false)]
		[MWFCategory("Data")]
		public ComboBox.ObjectCollection Items {
			get { return items; }
		}

		//TODO:
		void SetTextBoxAutoCompleteData ()
		{
			if (textbox_ctrl == null)
				return;
			
		}

		#endregion

		#region public Methods

		//TODO:
		public int GetItemHeight (int index)
		{
			if (DrawMode == DrawMode.OwnerDrawVariable && IsHandleCreated)
			{
				
				if (index < 0 || index >= Items.Count)
					throw new ArgumentOutOfRangeException ("The item height value is less than zero");
				
				object item = Items[index];
				if (item_heights.Contains (item))
					return (int)item_heights[item];
				
				//	MeasureItemEventArgs args = new MeasureItemEventArgs (DeviceContext, index, ItemHeight);
				//	OnMeasureItem (args);
				//	item_heights [item] = args.ItemHeight;
				return this.Height;
				//return args.ItemHeight;
			}
			
			return ItemHeight;
		}

		//TODO:
		internal void HandleDrawItem (DrawItemEventArgs e)
		{
			// Only raise OnDrawItem if we are in an OwnerDraw mode
			switch (DrawMode)
			{
			case DrawMode.OwnerDrawFixed:
			case DrawMode.OwnerDrawVariable:
				OnDrawItem (e);
				break;
			default:
				//	ThemeEngine.Current.DrawComboBoxItem (this, e);
				break;
			}
		}

		protected override void OnForeColorChanged (EventArgs e)
		{
			base.OnForeColorChanged (e);
			if (textbox_ctrl != null)
				textbox_ctrl.TextColor = ForeColor.ToNSColor ();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected override void OnGotFocus (EventArgs e)
		{
			if (dropdown_style == ComboBoxStyle.DropDownList)
			{
				// We draw DDL styles manually, so they require a
				// refresh to have their selection drawn
				Invalidate ();
			}
			
			if (textbox_ctrl != null)
			{
				
				textbox_ctrl.Selectable = false;
				textbox_ctrl.SelectText (new NSString (textbox_ctrl.StringValue));
			}
			
			base.OnGotFocus (e);
		}



		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected override void OnLostFocus (EventArgs e)
		{
			if (dropdown_style == ComboBoxStyle.DropDownList)
			{
				// We draw DDL styles manually, so they require a
				// refresh to have their selection drawn
				Invalidate ();
			}
			
			if (listbox_ctrl != null && dropped_down)
			{
				//TODO:
				//listbox_ctrl.HideWindow ();
			}
			
			if (textbox_ctrl != null)
			{
				textbox_ctrl.Selectable = true;
				textbox_ctrl.SelectionLength = 0;
				//TODO:
				//textbox_ctrl.HideAutoCompleteList ();
			}
			
			base.OnLostFocus (e);
		}



		protected override void OnHandleCreated (EventArgs e)
		{
			base.OnHandleCreated (e);
			
			SetBoundsInternal (Left, Top, Width, PreferredHeight, BoundsSpecified.None);
			
			//if (textbox_ctrl != null)
			//	Controls.AddImplicit (textbox_ctrl);
			
			LayoutComboBox ();
			UpdateComboBoxBounds ();
		}



		protected override void OnKeyPress (KeyPressEventArgs e)
		{
			if (dropdown_style == ComboBoxStyle.DropDownList)
			{
				int index = FindStringCaseInsensitive (e.KeyChar.ToString (), SelectedIndex + 1);
				if (index != -1)
				{
					SelectedIndex = index;
					if (DroppedDown)
					{
						//Scroll into view
						//TODO
						//if (SelectedIndex >= listbox_ctrl.LastVisibleItem ())
						//	listbox_ctrl.Scroll (SelectedIndex - listbox_ctrl.LastVisibleItem () + 1);
						// Or, selecting an item earlier in the list.
						//if (SelectedIndex < listbox_ctrl.FirstVisibleItem ())
						//	listbox_ctrl.Scroll (SelectedIndex - listbox_ctrl.FirstVisibleItem ());
					}
				}
			}
			
			base.OnKeyPress (e);
		}

		protected override void OnResize (EventArgs e)
		{
			LayoutComboBox ();
			//if (listbox_ctrl != null)
			//	listbox_ctrl.CalcListBoxArea ();
		}

		internal override void SetBoundsInternal (int x, int y, int width, int height, BoundsSpecified specified)
		{
			height = 25;
			base.SetBoundsInternal (x, y, width, height, specified);
		}


		public void Select (int start, int length)
		{
			if (start < 0)
				throw new ArgumentException ("Start cannot be less than zero");
			
			if (length < 0)
				throw new ArgumentException ("length cannot be less than zero");
			
			if (dropdown_style == ComboBoxStyle.DropDownList)
				return;
			
			textbox_ctrl.SelectionStart = start;
			textbox_ctrl.SelectionLength = length;
		}


		public void SelectAll ()
		{
			if (dropdown_style == ComboBoxStyle.DropDownList)
				return;
			
			if (textbox_ctrl != null)
			{
				//textbox_ctrl.ShowSelection = true;
				//TODO
				//textbox_ctrl.SelectAll ();
			}
		}

		#endregion

		#region Private Methods

		private void UpdatedItems ()
		{
			_dataSource = new ComboBoxDataSource (this, Items);
			m_helper.UsesDataSource = true;
			m_helper.DataSource = _dataSource;
			m_helper.ReloadData();
			if (listbox_ctrl != null)
			{
				//	listbox_ctrl.UpdateLastVisibleItem ();
				//	listbox_ctrl.CalcListBoxArea ();
				//	listbox_ctrl.Refresh ();
			}
		}

		void LayoutComboBox ()
		{
		}
		/*TODO
			int border = ThemeEngine.Current.Border3DSize.Width;

			text_area = ClientRectangle;
			text_area.Height = PreferredHeight;
			
			listbox_area = ClientRectangle;
			listbox_area.Y = text_area.Bottom + 3;
			listbox_area.Height -= (text_area.Height + 2);

			Rectangle prev_button_area = button_area;

			if (DropDownStyle == ComboBoxStyle.Simple)
				button_area = Rectangle.Empty;
			else {
				button_area = text_area;
				button_area.X = text_area.Right - button_width - border;
				button_area.Y = text_area.Y + border;
				button_area.Width = button_width;
				button_area.Height = text_area.Height - 2 * border;
				if (flat_style == FlatStyle.Popup || flat_style == FlatStyle.Flat) {
					button_area.Inflate (1, 1);
					button_area.X += 2;
					button_area.Width -= 2;
				}
			}

			if (button_area != prev_button_area) {
				prev_button_area.Y -= border;
				prev_button_area.Width += border;
				prev_button_area.Height += 2 * border;
				Invalidate (prev_button_area);
				Invalidate (button_area);
			}

			if (textbox_ctrl != null) {
				int text_border = border + 1;
				textbox_ctrl.Location = new Point (text_area.X + text_border, text_area.Y + text_border);
				textbox_ctrl.Width = text_area.Width - button_area.Width - text_border * 2;
				textbox_ctrl.Height = text_area.Height - text_border * 2;
			}

			if (listbox_ctrl != null && dropdown_style == ComboBoxStyle.Simple) {
				listbox_ctrl.Location = listbox_area.Location;
				listbox_ctrl.CalcListBoxArea ();
			}
			*/

		void UpdateComboBoxBounds ()
		{
			
		}

		internal void Draw (Rectangle clip, Graphics dc)
		{
			
		}



		internal void DropDownListBox ()
		{
		}
		/*
			DropDownButtonEntered = false;

			if (DropDownStyle == ComboBoxStyle.Simple)
				return;
			
			if (listbox_ctrl == null)
				CreateComboListBox ();

			listbox_ctrl.Location = PointToScreen (new Point (text_area.X, text_area.Y + text_area.Height));

			FindMatchOrSetIndex(SelectedIndex);

			if (textbox_ctrl != null)
				textbox_ctrl.HideAutoCompleteList ();

			if (listbox_ctrl.ShowWindow ())
				dropped_down = true;

			button_state = ButtonState.Pushed;
			if (dropdown_style == ComboBoxStyle.DropDownList)
				Invalidate (text_area);
				*/

		internal void DropDownListBoxFinished ()
		{
		}
		/*
			if (DropDownStyle == ComboBoxStyle.Simple)
				return;
				
			FindMatchOrSetIndex (SelectedIndex);
			button_state = ButtonState.Normal;
			Invalidate (button_area);
			dropped_down = false;
			OnDropDownClosed (EventArgs.Empty);
			 
			 // If the user opens a new form in an event, it will close our dropdown,
			 // so we need a null check here
			 if (listbox_ctrl != null) {
				listbox_ctrl.Dispose ();
				listbox_ctrl = null;
			}
			 // The auto complete list could have been shown after the listbox,
			 // so make sure it's hidden.
			 if (textbox_ctrl != null)
				 textbox_ctrl.HideAutoCompleteList ();
			*/

		internal void RestoreContextMenu ()
		{
			//textbox_ctrl.RestoreContextMenu ();
		}


		void SetSelectedIndex (int value, bool supressAutoScroll)
		{
			if (selected_index == value)
				return;
			
			if (value <= -2 || value >= Items.Count)
				throw new ArgumentOutOfRangeException ("SelectedIndex");
			
			selected_index = value;
			//TODO: Test
			textbox_ctrl.IntValue = value;
			
			OnSelectedValueChanged (EventArgs.Empty);
			OnSelectedIndexChanged (EventArgs.Empty);
			OnSelectedItemChanged (EventArgs.Empty);
		}
		#endregion



		[ListBindableAttribute(false)]
		public class ObjectCollection : IList, ICollection, IEnumerable
		{

			private ComboBox owner;
			internal ArrayList object_items = new ArrayList ();

			#region UIA Framework Events

			//NOTE:
			//	We are using Reflection to add/remove internal events.
			//	Class ListProvider uses the events.
			//
			//Event used to generate UIA StructureChangedEvent
			static object UIACollectionChangedEvent = new object ();

			internal event CollectionChangeEventHandler UIACollectionChanged {
				add { owner.Events.AddHandler (UIACollectionChangedEvent, value); }
				remove { owner.Events.RemoveHandler (UIACollectionChangedEvent, value); }
			}

			internal void OnUIACollectionChangedEvent (CollectionChangeEventArgs args)
			{
				CollectionChangeEventHandler eh = (CollectionChangeEventHandler)owner.Events[UIACollectionChangedEvent];
				if (eh != null)
					eh (owner, args);
			}

			#endregion UIA Framework Events

			public ObjectCollection (ComboBox owner)
			{
				this.owner = owner;
			}

			#region Public Properties
			public int Count {
				get { return object_items.Count; }
			}

			public bool IsReadOnly {
				get { return false; }
			}

			[Browsable(false)]
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
			public virtual object this[int index] {
				get {
					if (index < 0 || index >= Count)
						throw new ArgumentOutOfRangeException ("index");
					
					return object_items[index];
				}
				set {
					if (index < 0 || index >= Count)
						throw new ArgumentOutOfRangeException ("index");
					if (value == null)
						throw new ArgumentNullException ("value");
					
					//UIA Framework event: Item Removed
					OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Remove, object_items[index]));
					
					object_items[index] = value;
					
					//UIA Framework event: Item Added
					OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, value));
					
					//if (owner.listbox_ctrl != null)
					//	owner.listbox_ctrl.InvalidateItem (index);
					if (index == owner.SelectedIndex)
					{
						if (owner.textbox_ctrl == null)
							owner.Refresh ();
						else
							owner.textbox_ctrl.SelectedText = value.ToString ();
					}
				}
			}

			bool ICollection.IsSynchronized {
				get { return false; }
			}

			object ICollection.SyncRoot {
				get { return this; }
			}

			bool IList.IsFixedSize {
				get { return false; }
			}

			#endregion Public Properties

			#region Public Methods
			public int Add (object item)
			{
				int idx;
				
				idx = AddItem (item, false);
				owner.UpdatedItems ();
				return idx;
			}

			public void AddRange (object[] items)
			{
				if (items == null)
					throw new ArgumentNullException ("items");
				
				foreach (object mi in items)
					AddItem (mi, true);
				
				if (owner.sorted)
					Sort ();
				
				owner.UpdatedItems ();
			}

			public void Clear ()
			{
				owner.selected_index = -1;
				object_items.Clear ();
				owner.UpdatedItems ();
				owner.Refresh ();
				
				//UIA Framework event: Items list cleared
				OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Refresh, null));
			}

			public bool Contains (object value)
			{
				if (value == null)
					throw new ArgumentNullException ("value");
				
				return object_items.Contains (value);
			}

			public void CopyTo (object[] destination, int arrayIndex)
			{
				object_items.CopyTo (destination, arrayIndex);
			}

			void ICollection.CopyTo (Array destination, int index)
			{
				object_items.CopyTo (destination, index);
			}

			public IEnumerator GetEnumerator ()
			{
				return object_items.GetEnumerator ();
			}

			int IList.Add (object item)
			{
				return Add (item);
			}

			public int IndexOf (object value)
			{
				if (value == null)
					throw new ArgumentNullException ("value");
				
				return object_items.IndexOf (value);
			}

			public void Insert (int index, object item)
			{
				if (index < 0 || index > Count)
					throw new ArgumentOutOfRangeException ("index");
				if (item == null)
					throw new ArgumentNullException ("item");
				
				owner.BeginUpdate ();
				
				if (owner.Sorted)
					AddItem (item, false);
				else
				{
					object_items.Insert (index, item);
					//UIA Framework event: Item added
					OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, item));
				}
				
				owner.EndUpdate ();
				// Calls UpdatedItems
			}

			public void Remove (object value)
			{
				if (value == null)
					return;
				
				if (IndexOf (value) == owner.SelectedIndex)
					owner.SelectedIndex = -1;
				
				RemoveAt (IndexOf (value));
			}

			public void RemoveAt (int index)
			{
				if (index < 0 || index >= Count)
					throw new ArgumentOutOfRangeException ("index");
				
				if (index == owner.SelectedIndex)
					owner.SelectedIndex = -1;
				
				object removed = object_items[index];
				
				object_items.RemoveAt (index);
				owner.UpdatedItems ();
				
				//UIA Framework event: Item removed
				OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Remove, removed));
			}
			#endregion Public Methods

			#region Private Methods
			private int AddItem (object item, bool suspend)
			{
				// suspend means do not sort as we put new items in, we will do a
				// big sort at the end
				if (item == null)
					throw new ArgumentNullException ("item");
				
				if (owner.Sorted && !suspend)
				{
					int index = 0;
					foreach (object o in object_items)
					{
						if (String.Compare (item.ToString (), o.ToString ()) < 0)
						{
							object_items.Insert (index, item);
							
							// If we added the new item before the selectedindex
							// bump the selectedindex by one, behavior differs if
							// Handle has not been created.
							if (index <= owner.selected_index && owner.IsHandleCreated)
								owner.selected_index++;
							
							//UIA Framework event: Item added
							OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, item));
							
							return index;
						}
						index++;
					}
				}
				object_items.Add (item);
				
				//UIA Framework event: Item added
				OnUIACollectionChangedEvent (new CollectionChangeEventArgs (CollectionChangeAction.Add, item));
				
				return object_items.Count - 1;
			}

			internal void AddRange (IList items)
			{
				foreach (object mi in items)
					AddItem (mi, false);
				
				if (owner.sorted)
					Sort ();
				
				owner.UpdatedItems ();
			}

			internal void Sort ()
			{
				// If the objects the user put here don't have their own comparer,
				// use one that compares based on the object's ToString
				if (object_items.Count > 0 && object_items[0] is IComparer)
					object_items.Sort ();
				else
					object_items.Sort (new ObjectComparer (owner));
			}

			private class ObjectComparer : IComparer
			{
				private ListControl owner;

				public ObjectComparer (ListControl owner)
				{
					this.owner = owner;
				}

				#region IComparer Members
				public int Compare (object x, object y)
				{
					return string.Compare (owner.GetItemText (x), owner.GetItemText (y));
				}
				#endregion
			}
			#endregion Private Methods
		}

		internal class ComboBoxDataSource : NSComboBoxDataSource
		{
			//public ObjectCollection dataArray;
			private ComboBox cbox;
			private object dataObject;
			public ComboBoxDataSource (ComboBox comboBox, object theObject)
			{
				dataObject = theObject;
				cbox = comboBox;
				if (theObject is IList)
				{
					//dataArray = new ObjectCollection (comboBox);
					//dataArray.AddRange ((theObject as IList).Cast<object> ().ToArray ());
				}
			}
			public ComboBoxDataSource (ComboBox comboBox)
			{
				cbox = comboBox;
				//dataArray = new ObjectCollection (comboBox);
			}
			public ComboBoxDataSource (ComboBox comboBox, ObjectCollection Items)
			{
				cbox = comboBox;
				//dataArray = Items;
			}
			private NSString returnString;
			public override NSObject ObjectValueForItem (NSComboBox comboBox, int index)
			{
				if (cbox == null)
					return null;
				if (string.IsNullOrEmpty (cbox.DisplayMember))
					returnString = new NSString (cbox.Items[index].ToString ());
				else
					returnString = new NSString (Util.GetPropertyStringValue (cbox.Items[index], cbox.DisplayMember));
				return returnString;
				
			}

			public override int ItemCount (NSComboBox comboBox)
			{
				if (cbox == null)
					return 0;
				return cbox.Items.Count;
			}

			public object GetSelectedValue (NSComboBox comboBox)
			{
				var DisplayMember = cbox.DisplayMember;
				var ValueMember = cbox.ValueMember;
				object l = cbox.Items[comboBox.SelectedIndex];
				if (!string.IsNullOrEmpty (DisplayMember))
				{
					//Use Display Property if they didnt set ValueMember
					var valueMember = string.IsNullOrEmpty (ValueMember) ? DisplayMember : ValueMember;
					return Util.GetPropertyValue (l, valueMember);
				}

				
				else
				{
					return l.ToString ();
				}
			}


			public void SetSelectedValue (NSComboBox combobox, object value)
			{
				var DisplayMember = cbox.DisplayMember;
				var ValueMember = cbox.ValueMember;
				if (string.IsNullOrEmpty (DisplayMember))
				{
					combobox.SelectItem (cbox.Items.IndexOf (value));
					return;
				}
				combobox.SelectItem (cbox.Items.IndexOf (value));
				
			}

			public override int IndexOfItem (NSComboBox comboBox, string value)
			{
				return cbox.Items.IndexOf (value);
			}
			public int IndexOfItem (NSComboBox ComboBox, object value)
			{
				return cbox.Items.IndexOf (value);
			}
		}
	}
}

