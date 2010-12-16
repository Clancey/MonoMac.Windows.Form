using System;
using System.Drawing;
using MonoMac.AppKit;
using System.Linq;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Collections;

namespace System.Windows.Forms
{
	public partial class Control : Component , IBindableComponent, IWin32Window
									//, IDropTarget, IBounds, ISynchronizeInvoke
	{
		internal virtual NSView c_helper { get; set; }
		#region constructors
		public Control ()
		{
			initialize ();
		}

		internal void initialize ()
		{
			c_helper = new NSControl ();
			MaximumSize = DefaultMaximumSize;
			MinimumSize = DefaultMinimumSize;
		}

		public static implicit operator NSView (Control control)
		{
			return control.c_helper;
		}

		#endregion
		
		

		#region Public Static Properties

		public static Font DefaultFont {
			get {//TODO:
				return null;
			}
		}

		public static Color DefaultForeColor {
			get {//TODO:
				return null;
			}
		}

		public static Keys ModifierKeys {
			get { //TODO
				return null;
			}
		}

		public static MouseButtons MouseButtons {
			get { //TODO:
				return null;
			}
		}

		public static Point MousePosition {
			get {//TODO:
				return Point.Empty;
			}
		}
		
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		[Browsable (false)]
		[MonoTODO ("Stub, value is not used")]
		public static bool CheckForIllegalCrossThreadCalls 
		{
			get {
				return verify_thread_handle;
			}

			set {
				verify_thread_handle = value;
			}
		}
		#endregion	// Public Static Properties
		
		#region Internal Members
		
		#endregion
		
		#region members

		
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle Bounds {
			get { return Rectangle.Round (c_helper.Bounds); }
			set { setBounds(value.Left, value.Top, value.Width, value.Height, BoundsSpecified.All);
				//c_helper.Bounds = value;
			}
		}
		
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanFocus {
			get { return c_helper.AcceptsFirstResponder (); }
		}

		
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle ClientRectangle {
			get { return Rectangle.Round (c_helper.Frame); }
		}
		
		
		internal virtual Size client_size {
			get { return new Size ((int)Size.Width, (int)Size.Height); }
			set { Size = value;
		
				//this.SetClientSizeCore(value.Width, value.Height);
				this.OnClientSizeChanged (EventArgs.Empty);	
			}
		}
		
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool ContainsFocus {
			get { return c_helper == c_helper.Window.FirstResponder || c_helper.Subviews.Where (x => x == c_helper.Window.FirstResponder).Count () > 0; }
		}

		
		internal virtual ControlCollection child_controls {
			get {
				if (controls == null)
					controls = new ControlCollection (c_helper);
				return controls;
			}
		}

		public static Color DefaultBackColor {
			get { return Color.Gray; }
		}

		
		bool is_enabled
		{
			get { 
					bool state = true;
				if (c_helper is NSControl)
					state  = (c_helper as NSControl).Cell.State;
				if(!state)
					return false;
			}
		}
		
		[DispId(-514)]
		[Localizable(true)]
		[MWFCategory("Behavior")]
		public bool Enabled {
			get {
				if (!is_enabled) {
					return false;
				}

				if (parent != null) {
					return parent.Enabled;
				}

				return true;
			}

			set {
				if (this.is_enabled == value)
					return;

				bool old_value = is_enabled;

				is_enabled = value;

				if (!value)
					UpdateCursor ();

				if (old_value != value && !value && this.has_focus)
					SelectNextControl(this, true, true, true, true);

				OnEnabledChanged (EventArgs.Empty);
			}
		}
		
		
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool Focused {
			get { return c_helper == c_helper.Window.FirstResponder; }
		}

		internal virtual NSFont font { get; set; }
		

		[DispId(-512)]
		[AmbientValue(null)]
		[Localizable(true)]
		[MWFCategory("Appearance")]
		public virtual System.Drawing.Font Font {
			get {
				if (font == null)
					return new System.Drawing.Font ("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				return new System.Drawing.Font (font.FontName, font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}

			set { font = MonoMac.AppKit.NSFont.FromFontName (value.Name, value.Size); }
		}

		internal virtual Point location {
			get { return Point.Round (c_helper.Frame.Location); }
			set { c_helper.Frame = new RectangleF (value, c_helper.Frame.Size); }
		}
		
		/*
		public Point Location {
			get { return location; }
			set { location = value; }
		}
		*/
		
		public Control parent
		{
			get {
				if (c_helper.Superview == null)
					return null;
				if (c_helper is IViewHelper)
					return (c_helper as IViewHelper).Host;
				return null;
			}
			set { c_helper.RemoveFromSuperview (); }
		}
		

		internal virtual Size size {
			get { return Size.Round (c_helper.Frame.Size); }
			set { c_helper.Frame = new RectangleF (c_helper.Frame.Location, value); }
		}

		
		/*
		public bool Visible {
			get { return c_helper.Hidden; }
			set { c_helper.Hidden = value; }
		}
		*/

		#endregion
		
		
		public virtual void Refresh()
		{
			c_helper.Display();
		}
		public void PerformLayout()
		{
			//TODO:
		}

		public void Invalidate()
		{
			if(c_helper != null)
				c_helper.NeedsDisplay = true;
		}
		
		// This method exists so controls overriding OnPaintBackground can have default background painting done
		internal virtual void PaintControlBackground (PaintEventArgs pevent) {
	
			if (background_image == null) {
					Rectangle paintRect = new Rectangle(pevent.ClipRectangle.X, pevent.ClipRectangle.Y, pevent.ClipRectangle.Width, pevent.ClipRectangle.Height);
					Brush pen =  new SolidBrush(BackColor);//ThemeEngine.Current.ResPool.GetSolidBrush(BackColor);
					pevent.Graphics.FillRectangle(pen, paintRect);
				
				return;
			}

			DrawBackgroundImage (pevent.Graphics);
		}

		void DrawBackgroundImage (Graphics g) {
			Rectangle drawing_rectangle = new Rectangle ();
			g.FillRectangle (new SolidBrush (BackColor), ClientRectangle);
				
			switch (backgroundimage_layout)
			{
			case ImageLayout.Tile:
				using (TextureBrush b = new TextureBrush (background_image, WrapMode.Tile)) {
					g.FillRectangle (b, ClientRectangle);
				}
				return;
			case ImageLayout.Center:
				drawing_rectangle.Location = new Point (ClientSize.Width / 2 - background_image.Width / 2, ClientSize.Height / 2 - background_image.Height / 2);
				drawing_rectangle.Size = background_image.Size;
				break;
			case ImageLayout.None:
				drawing_rectangle.Location = Point.Empty;
				drawing_rectangle.Size = background_image.Size;
				break;
			case ImageLayout.Stretch:
				drawing_rectangle = ClientRectangle;
				break;
			case ImageLayout.Zoom:
				drawing_rectangle = ClientRectangle;
				if ((float)background_image.Width / (float)background_image.Height < (float)drawing_rectangle.Width / (float) drawing_rectangle.Height) {
					drawing_rectangle.Width = (int) (background_image.Width * ((float)drawing_rectangle.Height / (float)background_image.Height));
					drawing_rectangle.X = (ClientRectangle.Width - drawing_rectangle.Width) / 2;
				} else {
					drawing_rectangle.Height = (int) (background_image.Height * ((float)drawing_rectangle.Width / (float)background_image.Width));
					drawing_rectangle.Y = (ClientRectangle.Height - drawing_rectangle.Height) / 2;
				}
				break;
			default:
				return;
			}

			g.DrawImage (background_image, drawing_rectangle);

/*
			using (TextureBrush b = new TextureBrush (background_image, WrapMode.Tile)) {
				g.FillRectangle (b, ClientRectangle);
			}
			*/

		}
		
		
		/// <summary>
		///Private class 
		/// </summary>
		
		#region Public Classes


		[ListBindable (false)]
#if NET_2_0
		[ComVisible (false)]
		public class ControlCollection : Layout.ArrangedElementCollection, IList, ICollection, ICloneable, IEnumerable {
#else
		[DesignerSerializer("System.Windows.Forms.Design.ControlCollectionCodeDomSerializer, " + Consts.AssemblySystem_Design, "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + Consts.AssemblySystem_Design)]
		public class ControlCollection : IList, ICollection, ICloneable, IEnumerable {
#endif
			#region ControlCollection Local Variables
#if !NET_2_0
			ArrayList list;
#endif
			ArrayList impl_list;
			Control [] all_controls;
			Control owner;
			#endregion // ControlCollection Local Variables

			#region ControlCollection Public Constructor
			public ControlCollection (Control owner)
			{
				this.owner = owner;
#if !NET_2_0
				this.list = new ArrayList();
#endif
			}
			#endregion

			#region ControlCollection Public Instance Properties

#if !NET_2_0
			public int Count {
				get { return list.Count; }
			}

			public bool IsReadOnly {
				get {
					return list.IsReadOnly;
				}
			}
#endif

#if NET_2_0
			public Control Owner {
				get { return this.owner; }
			}
			
			public virtual Control this[string key] {
				get { 
					int index = IndexOfKey (key);
					
					if (index >= 0)
						return this[index];
						
					return null;
				}
			}
			
			new
#endif
			public virtual Control this[int index] {
				get {
					if (index < 0 || index >= list.Count) {
						throw new ArgumentOutOfRangeException("index", index, "ControlCollection does not have that many controls");
					}
					return (Control)list[index];
				}
				
				
			}

			#endregion // ControlCollection Public Instance Properties
			
			#region ControlCollection Instance Methods

			public virtual void Add (Control value)
			{
				if (value == null)
					return;

				Form form_value = value as Form;
				Form form_owner = owner as Form;
				bool owner_permits_toplevels = (owner is MdiClient) || (form_owner != null && form_owner.IsMdiContainer);
				bool child_is_toplevel = value.GetTopLevel();
				bool child_is_mdichild = form_value != null && form_value.IsMdiChild;

				if (child_is_toplevel && !(owner_permits_toplevels && child_is_mdichild))
					throw new ArgumentException("Cannot add a top level control to a control.", "value");
				
				if (child_is_mdichild && form_value.MdiParent != null && form_value.MdiParent != owner && form_value.MdiParent != owner.Parent) {
					throw new ArgumentException ("Form cannot be added to the Controls collection that has a valid MDI parent.", "value");
				}
				
				value.recalculate_distances = true;
				
				if (Contains (value)) {
					owner.PerformLayout();
					return;
				}

				if (value.tab_index == -1) {
					int	end;
					int	index;
					int	use;

					use = 0;
					end = owner.child_controls.Count;
					for (int i = 0; i < end; i++) {
						index = owner.child_controls[i].tab_index;
						if (index >= use) {
							use = index + 1;
						}
					}
					value.tab_index = use;
				}

				if (value.parent != null) {
					value.parent.Controls.Remove(value);
				}

				all_controls = null;
				list.Add (value);

				value.ChangeParent(owner);

				value.InitLayout();

				if (owner.Visible)
					owner.UpdateChildrenZOrder();
				owner.PerformLayout(value, "Parent");
				owner.OnControlAdded(new ControlEventArgs(value));
			}
			
			internal void AddToList (Control c)
			{
				all_controls = null;
				list.Add (c);
			}

			internal virtual void AddImplicit (Control control)
			{
				if (impl_list == null)
					impl_list = new ArrayList ();

				if (AllContains (control)) {
					owner.PerformLayout ();
					return;
				}

				if (control.parent != null) {
					control.parent.Controls.Remove(control);
				}

				all_controls = null;
				impl_list.Add (control);

				control.ChangeParent (owner);
				control.InitLayout ();
				if (owner.Visible)
					owner.UpdateChildrenZOrder ();
				
				// If we are adding a new control that isn't
				// visible, don't trigger a layout
				if (control.VisibleInternal)
					owner.PerformLayout (control, "Parent");
			}
#if NET_2_0
			[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
#endif
			public virtual void AddRange (Control[] controls)
			{
				if (controls == null)
					throw new ArgumentNullException ("controls");

				owner.SuspendLayout ();

				try {
					for (int i = 0; i < controls.Length; i++) 
						Add (controls[i]);
				} finally {
					owner.ResumeLayout ();
				}
			}

			internal virtual void AddRangeImplicit (Control [] controls)
			{
				if (controls == null)
					throw new ArgumentNullException ("controls");

				owner.SuspendLayout ();

				try {
					for (int i = 0; i < controls.Length; i++)
						AddImplicit (controls [i]);
				} finally {
					owner.ResumeLayout (false);
				}
			}

#if NET_2_0
			new
#endif
			public virtual void Clear ()
			{
				all_controls = null;

				// MS sends remove events in reverse order
				while (list.Count > 0) {
					Remove((Control)list[list.Count - 1]);
				}
			}

			internal virtual void ClearImplicit ()
			{
				if (impl_list == null)
					return;
				all_controls = null;
				impl_list.Clear ();
			}

			public bool Contains (Control control)
			{
				return list.Contains (control);
			}

			internal bool ImplicitContains (Control value) {
				if (impl_list == null)
					return false;

				return impl_list.Contains (value);
			}

			internal bool AllContains (Control value) {
				return Contains (value) || ImplicitContains (value);
			}

#if NET_2_0
			public virtual bool ContainsKey (string key)
			{
				return IndexOfKey (key) >= 0;
			}
#endif

#if !NET_2_0
			public void CopyTo (Array dest, int index)
			{
				list.CopyTo (dest, index);
			}

			public override bool Equals (object other)
			{
				if (other is ControlCollection && (((ControlCollection)other).owner==this.owner)) {
					return(true);
				} else {
					return(false);
				}
			}
#endif

#if NET_2_0
			// LAMESPEC: MSDN says AE, MS implementation throws ANE
			public Control[] Find (string key, bool searchAllChildren)
			{
				if (string.IsNullOrEmpty (key))
					throw new ArgumentNullException ("key");
					
				ArrayList al = new ArrayList ();
				
				foreach (Control c in list) {
					if (c.Name.Equals (key, StringComparison.CurrentCultureIgnoreCase))
						al.Add (c);
						
					if (searchAllChildren)
						al.AddRange (c.Controls.Find (key, true));
				}
				
				return (Control[])al.ToArray (typeof (Control));
			}
#endif

			public int GetChildIndex(Control child) {
				return GetChildIndex(child, false);
			}

#if NET_2_0
			public virtual int
#else
			public int
#endif
			GetChildIndex(Control child, bool throwException) {
				int index;

				index=list.IndexOf(child);

				if (index==-1 && throwException) {
					throw new ArgumentException("Not a child control", "child");
				}
				return index;
			}

#if NET_2_0
			public override IEnumerator
#else
			public IEnumerator
#endif
			GetEnumerator () {
				return new ControlCollectionEnumerator (list);
			}

			internal IEnumerator GetAllEnumerator () {
				Control [] res = GetAllControls ();
				return res.GetEnumerator ();
			}

			internal ArrayList ImplicitControls {
				get { return impl_list; }
			}
			
			internal Control [] GetAllControls () {
				if (all_controls != null)
					return all_controls;

				if (impl_list == null) {
					all_controls = (Control []) list.ToArray (typeof (Control));
					return all_controls;
				}
				
				all_controls = new Control [list.Count + impl_list.Count];
				impl_list.CopyTo (all_controls);
				list.CopyTo (all_controls, impl_list.Count);

				return all_controls;
			}

#if !NET_2_0
			public override int GetHashCode ()
			{
				return base.GetHashCode ();
			}
#endif

			public int IndexOf (Control control)
			{
				return list.IndexOf (control);
			}

#if NET_2_0
			public virtual int IndexOfKey (string key)
			{
				if (string.IsNullOrEmpty (key))
					return -1;
					
				for (int i = 0; i < list.Count; i++)
					if (((Control)list[i]).Name.Equals (key, StringComparison.CurrentCultureIgnoreCase))
						return i;
						
				return -1;
			}
#endif

			public virtual void Remove (Control value)
			{
				if (value == null)
					return;

				all_controls = null;
				list.Remove(value);

				owner.PerformLayout(value, "Parent");
				owner.OnControlRemoved(new ControlEventArgs(value));

				ContainerControl container = owner.InternalGetContainerControl ();
				if (container != null) { 
					// Inform any container controls about the loss of a child control
					// so that they can update their active control
					container.ChildControlRemoved (value);
				}

				value.ChangeParent(null);

				owner.UpdateChildrenZOrder();
			}

			internal virtual void RemoveImplicit (Control control)
			{
				if (impl_list != null) {
					all_controls = null;
					impl_list.Remove (control);
					owner.PerformLayout (control, "Parent");
					owner.OnControlRemoved (new ControlEventArgs (control));
				}
				control.ChangeParent (null);
				owner.UpdateChildrenZOrder ();
			}

			public void RemoveAt (int index)
			{
				if (index < 0 || index >= list.Count)
					throw new ArgumentOutOfRangeException("index", index, "ControlCollection does not have that many controls");

				Remove ((Control) list [index]);
			}

#if NET_2_0
			public virtual void RemoveByKey (string key)
			{
				int index = IndexOfKey (key);
				
				if (index >= 0)
					RemoveAt (index);
			}
#endif

#if NET_2_0
			public virtual void
#else
			public void
#endif
			SetChildIndex(Control child, int newIndex)
			{
				if (child == null)
					throw new ArgumentNullException ("child");

				int	old_index;

				old_index=list.IndexOf(child);
				if (old_index==-1) {
					throw new ArgumentException("Not a child control", "child");
				}

				if (old_index==newIndex) {
					return;
				}

				all_controls = null;
				list.RemoveAt(old_index);

				if (newIndex>list.Count) {
					list.Add(child);
				} else {
					list.Insert(newIndex, child);
				}
				child.UpdateZOrder();
				owner.PerformLayout();
			}

			#endregion // ControlCollection Private Instance Methods

			#region ControlCollection Interface Properties

#if !NET_2_0
			object IList.this [int index] {
				get {
					if (index<0 || index>=list.Count) {
						throw new ArgumentOutOfRangeException("index", index, "ControlCollection does not have that many controls");
					}
					return this[index];
				}

				set {
					if (!(value is Control)) {
						throw new ArgumentException("Object of type Control required", "value");
					}

					all_controls = null;
					Control ctrl = (Control) value;
					list[index]= ctrl;

					ctrl.ChangeParent(owner);

					ctrl.InitLayout();

					owner.UpdateChildrenZOrder();
					owner.PerformLayout(ctrl, "Parent");
				}
			}

			bool IList.IsFixedSize {
				get {
					return false;
				}
			}

			bool ICollection.IsSynchronized {
				get {
					return list.IsSynchronized;
				}
			}

			object ICollection.SyncRoot {
				get {
					return list.SyncRoot;
				}
			}
#endif

			#endregion // ControlCollection Interface Properties

			#region ControlCollection Interface Methods

			int IList.Add (object control)
			{
				if (!(control is Control))
					throw new ArgumentException ("Object of type Control required", "control");

				if (control == null)
					throw new ArgumentException ("control", "Cannot add null controls");

				this.Add ((Control)control);
				return this.IndexOf ((Control)control);
			}

#if !NET_2_0
			bool IList.Contains (object control)
			{
				if (!(control is Control))
					throw new ArgumentException ("Object of type Control required", "control");

				return this.Contains ((Control) control);
			}

			int IList.IndexOf (object control)
			{
				if (!(control is Control))
					throw new ArgumentException ("Object of type Control  required", "control");

				return this.IndexOf ((Control) control);
			}

			void IList.Insert (int index, object value)
			{
				if (!(value is Control))
					throw new ArgumentException("Object of type Control required", "value");

				all_controls = null;
				list.Insert (index, value);
			}
#endif

			void IList.Remove (object control)
			{
				if (!(control is Control))
					throw new ArgumentException ("Object of type Control required", "control");

				this.Remove ((Control)control);
			}

			Object ICloneable.Clone ()
			{
				ControlCollection clone = new ControlCollection (this.owner);
				clone.list = (ArrayList)list.Clone ();		// FIXME: Do we need this?
				return clone;
			}

			#endregion // ControlCollection Interface Methods
		
			internal class ControlCollectionEnumerator : IEnumerator
			{
				private ArrayList list;
				int position = -1;
				
				public ControlCollectionEnumerator (ArrayList collection)
				{
					list = collection;
				}
				
				#region IEnumerator Members
				public object Current {
					get {
						try {
							return list[position];
						} catch (IndexOutOfRangeException) {
							throw new InvalidOperationException ();
						}
					}
				}

				public bool MoveNext ()
				{
					position++;
					return (position < list.Count);
				}

				public void Reset ()
				{
					position = -1;
				}

				#endregion
			}
		}
		#endregion	// ControlCollection Class

	}
}

