using System;
using System.Drawing;
using System.ComponentModel;
using MonoMac.AppKit;
using System.Runtime.InteropServices;
using System.Collections;
namespace System.Windows.Forms
{
	public partial class TabControl : Control
	{
		internal TabViewHelper m_helper;
		#region Public Constructors
		public TabControl ()
		{
			tab_pages = new TabPageCollection (this);
			SetStyle (ControlStyles.UserPaint, false);
			padding = new System.Drawing.Point(5,5);//ThemeEngine.Current.TabControlDefaultPadding;

			//MouseDown += new MouseEventHandler (MouseDownHandler);
			//MouseLeave += new EventHandler (OnMouseLeave);
			//MouseMove += new MouseEventHandler (OnMouseMove);
			//MouseUp += new MouseEventHandler (MouseUpHandler);
			SizeChanged += new EventHandler (SizeChangedHandler);
		}

		#endregion	// Public Constructors
		
		
		protected override void CreateHandle ()
		{
			base.CreateHandle ();
			
			tab_pages = new TabPageCollection (this);
			m_helper = new TabViewHelper(this);
			m_view = m_helper;
			selected_index = (selected_index >= TabCount ? (TabCount > 0 ? 0 : -1) : selected_index);

			if (TabCount > 0) {
				if (selected_index > -1)
					this.SelectedTab.SetVisible(true);
				else
					tab_pages[0].SetVisible(true);
			}
		}

		internal void ResizeTabPages()
		{
			
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Color BackColor {
			get { return base.BackColor; }
			set { /* nothing happens on set on MS */ }
		}
		
		public override Rectangle DisplayRectangle {
			get {
				return base.DisplayRectangle;
			}
		}
		
		private int MinimumTabWidth {
			get {
				return 100;
			}
		}

		private Size TabSpacing {
			get {
				return new Size(5,0);
			}
		}
		
		internal override void OnPaintInternal (PaintEventArgs pe)
		{
			if (GetStyle (ControlStyles.UserPaint))
				return;

			//Draw (pe.Graphics, pe.ClipRectangle);
			//pe.Handled = true;
		}
		
		
		[DefaultValue(-1)]
		[Browsable(false)]
		public int SelectedIndex {
			get { return selected_index; }
			set {

				if (value < -1) {
					throw new ArgumentOutOfRangeException ("SelectedIndex", "Value of '" + value + "' is valid for 'SelectedIndex'. " +
						"'SelectedIndex' must be greater than or equal to -1.");
				}
				if (!this.IsHandleCreated) {
					if (selected_index != value) {
						selected_index = value;
					}
					return;
				}

				if (value >= TabCount) {
					if (value != selected_index)
						OnSelectedIndexChanged (EventArgs.Empty);
					return;
				}

				if (value == selected_index) {
					if (selected_index > -1)
						Invalidate(GetTabRect (selected_index));
					return;
				}

				TabControlCancelEventArgs ret = new TabControlCancelEventArgs (SelectedTab, selected_index, false, TabControlAction.Deselecting);
				OnDeselecting (ret);
				if (ret.Cancel)
					return;

				Focus ();
				int old_index = selected_index;
				int new_index = value;

				selected_index = new_index;

				ret = new TabControlCancelEventArgs (SelectedTab, selected_index, false, TabControlAction.Selecting);
				OnSelecting (ret);
				if (ret.Cancel) {
					selected_index = old_index;
					return;
				}

				SuspendLayout ();

				if (new_index != -1) {
					m_helper.SelectAt(new_index);
				}

				OnSelectedIndexChanged (EventArgs.Empty);

				ResumeLayout ();
			}
		}
		
		
		

		#region Class TaControl.ControlCollection
		[ComVisible (false)]
		public new class ControlCollection : System.Windows.Forms.Control.ControlCollection {

			private TabControl owner;

			public ControlCollection (TabControl owner) : base (owner)
			{
				this.owner = owner;
			}

			public override void Add (Control value)
			{
				TabPage page = value as TabPage;
				if (page == null)
					throw new ArgumentException ("Cannot add " +
						value.GetType ().Name + " to TabControl. " +
						"Only TabPages can be directly added to TabControls.");

				page.SetVisible (false);
				base.Add (value);
				owner.m_helper.Add(page);
				if (owner.TabCount == 1 && owner.selected_index < 0)
					owner.SelectedIndex = 0;
				owner.Redraw ();
			}

			public override void Remove (Control value)
			{
				bool change_index = false;
				
				TabPage page = value as TabPage;
				if (page != null && owner.Controls.Contains (page)) {
					int index = owner.IndexForTabPage (page);
					if (index < owner.SelectedIndex || owner.SelectedIndex == Count - 1)
						change_index = true;
				}
				base.Remove (value);
				owner.m_helper.Remove(page);				
				// We don't want to raise SelectedIndexChanged until after we
				// have removed from the collection, so TabCount will be
				// correct for the user.
				if (change_index && Count > 0) {
					// Clear the selected index internally, to avoid trying to access the previous
					// selected tab when setting the new one - this is what .net seems to do
					int prev_selected_index = owner.SelectedIndex;
					owner.selected_index = -1;

					owner.SelectedIndex = --prev_selected_index;
					owner.Invalidate ();
				} else if (change_index) {
					owner.selected_index = -1;
					owner.OnSelectedIndexChanged (EventArgs.Empty);
					owner.Invalidate ();
				} else
					owner.Redraw ();
			}
		}
		#endregion	// Class TabControl.ControlCollection
		
		#region Class TabPage.TabPageCollection
		public class TabPageCollection	: IList, ICollection, IEnumerable {

			private TabControl owner;

			public TabPageCollection (TabControl owner)
			{
				if (owner == null)
					throw new ArgumentNullException ("Value cannot be null.");
				this.owner = owner;
			}

			[Browsable(false)]
			public int Count {
				get
				{
					return (int)owner.m_helper.Count;
				}
			}

			public bool IsReadOnly {
				get { return false; }
			}

			public virtual TabPage this [int index] {
				get {
					return (TabPage)((TabViewItemHelper)owner.m_helper.Items[index]).Host;
				}
				set {
					owner.SetTab (index, value);
				}
			}
			public virtual TabPage this [string key] {
				get {
					if (string.IsNullOrEmpty (key))
						return null;

					int index = this.IndexOfKey (key);
					if (index < 0 || index >= this.Count)
						return null;
					
					return this[index];
				}
			}

			internal int this[TabPage tabPage] {
				get {
					if (tabPage == null)
						return -1;

					for (int i = 0; i < this.Count; i++)
						if (this[i].Equals (tabPage))
							return i;

					return -1;
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

			object IList.this [int index] {
				get {
					return owner.GetTab (index);
				}
				set {
					owner.SetTab (index, (TabPage) value);
				}
			}

			public void Add (TabPage value)
			{
				if (value == null)
					throw new ArgumentNullException ("Value cannot be null.");
				owner.Controls.Add (value);
			}

			public void Add (string text)
			{
				TabPage page = new TabPage (text);
				this.Add (page);
			}

			public void Add (string key, string text)
			{
				TabPage page = new TabPage (text);
				page.Name = key;
				this.Add (page);
			}

			public void Add (string key, string text, int imageIndex)
			{
				TabPage page = new TabPage (text);
				page.Name = key;
				page.ImageIndex = imageIndex;
				this.Add (page);
			}

			// .Net sets the ImageKey, but does not show the image when this is used
			public void Add (string key, string text, string imageKey)
			{
				TabPage page = new TabPage (text);
				page.Name = key;
				page.ImageKey = imageKey;
				this.Add (page);
			}

			public void AddRange (TabPage [] pages)
			{
				if (pages == null)
					throw new ArgumentNullException ("Value cannot be null.");
				owner.Controls.AddRange (pages);
			}

			public virtual void Clear ()
			{
				owner.Controls.Clear ();
				owner.Invalidate ();
			}

			public bool Contains (TabPage page)
			{
				if (page == null)
					throw new ArgumentNullException ("Value cannot be null.");
				return owner.Controls.Contains (page);
			}

			public virtual bool ContainsKey (string key)
			{
				int index = this.IndexOfKey (key);
				return (index >= 0 && index < this.Count);
			}

			public IEnumerator GetEnumerator ()
			{
				return owner.Controls.GetEnumerator ();
			}

			public int IndexOf (TabPage page)
			{
				return owner.Controls.IndexOf (page);
			}

			public virtual int IndexOfKey(string key)
			{
				if (string.IsNullOrEmpty (key))
					return -1;

				for (int i = 0; i < this.Count; i++) {
					if (string.Compare (this[i].Name, key, true, 
						System.Globalization.CultureInfo.InvariantCulture) == 0) {
						return i;
					}
				}

				return -1;
			}

			public void Remove (TabPage value)
			{
				owner.Controls.Remove (value);
				owner.Invalidate ();
			}

			public void RemoveAt (int index)
			{
				owner.Controls.RemoveAt (index);
				owner.Invalidate ();
			}

			public virtual void RemoveByKey (string key)
			{
				int index = this.IndexOfKey (key);
				if (index >= 0 && index < this.Count)
					this.RemoveAt (index);
			}

			void ICollection.CopyTo (Array dest, int index)
			{
				owner.Controls.CopyTo (dest, index);
			}

			int IList.Add (object value)
			{
				TabPage page = value as TabPage;
				if (value == null)
					throw new ArgumentException ("value");
				owner.Controls.Add (page);
				return owner.Controls.IndexOf (page);
			}

			bool IList.Contains (object page)
			{
				TabPage tabPage = page as TabPage;
				if (tabPage == null)
					return false;
				return Contains (tabPage);
			}

			int IList.IndexOf (object page)
			{
				TabPage tabPage = page as TabPage;
				if (tabPage == null)
					return -1;
				return IndexOf (tabPage);
			}

			void IList.Insert (int index, object tabPage)
			{
				throw new NotSupportedException ();
			}

			public void Insert (int index, string text)
			{
				owner.InsertTab (index, new TabPage (text));
			}
			
			public void Insert (int index, TabPage tabPage)
			{
				owner.InsertTab (index, tabPage);
			}

			public void Insert (int index, string key, string text)
			{
				TabPage page = new TabPage(text);
				page.Name = key;
				owner.InsertTab (index, page);
			}

			public void Insert (int index, string key, string text, int imageIndex) 
			{
				TabPage page = new TabPage(text);
				page.Name = key;
				owner.InsertTab (index, page);
				page.ImageIndex = imageIndex;
			}

			public void Insert (int index, string key, string text, string imageKey) 
			{
				TabPage page = new TabPage(text);
				page.Name = key;
				owner.InsertTab (index, page);
				page.ImageKey = imageKey;
			}
			void IList.Remove (object value)
			{
				TabPage page = value as TabPage;
				if (page == null)
					return;
				Remove ((TabPage) value);
			}
		}
		#endregion	// Class TabPage.TabPageCollection
	}
}

