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
// Copyright (c) 2004 Novell, Inc.
//
// Authors:
//	Peter Bartok	pbartok@novell.com
//

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms {
	[Designer ("System.Windows.Forms.Design.ScrollableControlDesigner, " + Consts.AssemblySystem_Design, "System.ComponentModel.Design.IDesigner")]
#if NET_2_0
	[ClassInterface (ClassInterfaceType.AutoDispatch)]
	[ComVisible (true)]
#endif
	public partial class ScrollableControl : Control {
		#region Local Variables
		private bool			force_hscroll_visible;
		private bool			force_vscroll_visible;
		private bool			auto_scroll;
		private Size			auto_scroll_margin;
		private Size			auto_scroll_min_size;
		private Point			scroll_position;
		private DockPaddingEdges	dock_padding;
		internal Size			canvas_size;
		private Rectangle		display_rectangle;
		private Control			old_parent;

		#endregion	// Local Variables

		[TypeConverter(typeof(ScrollableControl.DockPaddingEdgesConverter))]
		#region Subclass DockPaddingEdges
		public class DockPaddingEdges : ICloneable
		{
			private Control	owner;
			
#if NET_2_0
			internal DockPaddingEdges (Control owner)
			{
				this.owner = owner;
			}

			#region DockPaddingEdges Public Instance Properties
			[RefreshProperties (RefreshProperties.All)]
			public int All {
				get { return owner.Padding.All; }
				set { owner.Padding = new Padding (value); }
			}

			[RefreshProperties (RefreshProperties.All)]
			public int Bottom {
				get { return owner.Padding.Bottom; }
				set { owner.Padding = new Padding (Left, Top, Right, value); }
			}

			[RefreshProperties (RefreshProperties.All)]
			public int Left {
				get { return owner.Padding.Left; }
				set { owner.Padding = new Padding (value, Top, Right, Bottom); }
			}

			[RefreshProperties (RefreshProperties.All)]
			public int Right {
				get { return owner.Padding.Right; }
				set { owner.Padding = new Padding (Left, Top, value, Bottom); }
			}

			[RefreshProperties (RefreshProperties.All)]
			public int Top {
				get { return owner.Padding.Top; }
				set { owner.Padding = new Padding (Left, value, Right, Bottom); }
			}
			#endregion	// DockPaddingEdges Public Instance Properties

			// Public Instance Methods
			public override bool Equals (object other)
			{
				if (!(other is DockPaddingEdges)) {
					return false;
				}

				if ((this.All == ((DockPaddingEdges)other).All) && (this.Left == ((DockPaddingEdges)other).Left) &&
					(this.Right == ((DockPaddingEdges)other).Right) && (this.Top == ((DockPaddingEdges)other).Top) &&
					(this.Bottom == ((DockPaddingEdges)other).Bottom)) {
					return true;
				}

				return false;
			}

			public override int GetHashCode ()
			{
				return All * Top * Bottom * Right * Left;
			}

			public override string ToString ()
			{
				return "All = " + All.ToString () + " Top = " + Top.ToString () + " Left = " + Left.ToString () + " Bottom = " + Bottom.ToString () + " Right = " + Right.ToString ();
			}

			internal void Scale (float dx, float dy)
			{
				Left = (int)(Left * dx);
				Right = (int)(Right * dx);
				Top = (int)(Top * dy);
				Bottom = (int)(Bottom * dy);
			}

			object ICloneable.Clone ()
			{
				return new DockPaddingEdges (owner);
			}
#else
			#region DockPaddingEdges Local Variables
			private int	all;
			private int	left;
			private int	right;
			private int	top;
			private int	bottom;
			#endregion	// DockPaddingEdges Local Variables

			#region DockPaddingEdges Constructor
			internal DockPaddingEdges(Control owner) {
				all = 0;
				left = 0;
				right = 0;
				top = 0;
				bottom = 0;
				this.owner = owner;
			}
			#endregion	// DockPaddingEdges Constructor

			#region DockPaddingEdges Public Instance Properties
			[RefreshProperties(RefreshProperties.All)]
			public int All {
				get {
					return all;
				}

				set {
					all = value;
					left = value;
					right = value;
					top = value;
					bottom = value;

					owner.PerformLayout();
				}
			}

			[RefreshProperties(RefreshProperties.All)]
			public int Bottom {
				get {
					return bottom;
				}

				set {
					bottom = value;
					all = 0;

					owner.PerformLayout();
				}
			}

			[RefreshProperties(RefreshProperties.All)]
			public int Left {
				get {
					return left;
				}

				set {
					left=value;
					all = 0;

					owner.PerformLayout();
				}
			}

			[RefreshProperties(RefreshProperties.All)]
			public int Right {
				get {
					return right;
				}

				set {
					right=value;
					all = 0;

					owner.PerformLayout();
				}
			}

			[RefreshProperties(RefreshProperties.All)]
			public int Top {
				get {
					return top;
				}

				set {
					top=value;
					all = 0;

					owner.PerformLayout();
				}
			}
			#endregion	// DockPaddingEdges Public Instance Properties

			// Public Instance Methods
			public override bool Equals(object other) {
				if (! (other is DockPaddingEdges)) {
					return false;
				}

				if (	(this.all == ((DockPaddingEdges)other).all) && (this.left == ((DockPaddingEdges)other).left) &&
					(this.right == ((DockPaddingEdges)other).right) && (this.top == ((DockPaddingEdges)other).top) && 
					(this.bottom == ((DockPaddingEdges)other).bottom)) {
					return true;
				}

				return false;
			}

			public override int GetHashCode() {
				return all*top*bottom*right*left;
			}

			public override string ToString() {
				return "All = "+all.ToString()+" Top = "+top.ToString()+" Left = "+left.ToString()+" Bottom = "+bottom.ToString()+" Right = "+right.ToString();
			}

			internal void Scale(float dx, float dy) {
				left = (int) (left * dx);
				right = (int) (right * dx);
				top = (int) (top * dy);
				bottom = (int) (bottom * dy);
			}

			object ICloneable.Clone() {
				DockPaddingEdges padding_edge;

				padding_edge=new DockPaddingEdges(owner);

				padding_edge.all=all;
				padding_edge.left=left;
				padding_edge.right=right;
				padding_edge.top=top;
				padding_edge.bottom=bottom;

				return padding_edge;
			}
#endif
		}
		#endregion	// Subclass DockPaddingEdges

		#region Subclass DockPaddingEdgesConverter
		public class DockPaddingEdgesConverter : System.ComponentModel.TypeConverter {
			// Public Constructors
			public DockPaddingEdgesConverter() {
			}

			// Public Instance Methods
			public override PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, Attribute[] attributes) {
				return TypeDescriptor.GetProperties(typeof(DockPaddingEdges), attributes);
			}

			public override bool GetPropertiesSupported(System.ComponentModel.ITypeDescriptorContext context) {
				return true;
			}
		}
		#endregion	// Subclass DockPaddingEdgesConverter

		#region Public Constructors

		void VisibleChangedHandler (object sender, EventArgs e)
		{
			Recalculate (false);
		}

		void ParentChangedHandler (object sender, EventArgs e)
		{
			
			if (old_parent == Parent)
				return;
				
			if (old_parent != null) {
				old_parent.SizeChanged -= new EventHandler (Parent_SizeChanged);
#if NET_2_0				
				old_parent.PaddingChanged -= new EventHandler (Parent_PaddingChanged);
#endif
			}
			
			if (Parent != null) {
				Parent.SizeChanged += new EventHandler (Parent_SizeChanged);
#if NET_2_0
				Parent.PaddingChanged += new EventHandler (Parent_PaddingChanged);
#endif
			}
			
			old_parent = Parent;
		}
		#endregion	// Public Constructors

		#region Protected Static Fields
		protected const int ScrollStateAutoScrolling	= 1;
		protected const int ScrollStateFullDrag		= 16;
		protected const int ScrollStateHScrollVisible	= 2;
		protected const int ScrollStateUserHasScrolled	= 8;
		protected const int ScrollStateVScrollVisible	= 4;
		#endregion	// Protected Static Fields

		#region Public Instance Properties
		[DefaultValue(false)]
		[Localizable(true)]
		[MWFCategory("Layout")]
		public virtual bool AutoScroll {
			get {
				return	auto_scroll;
			}

			set {
				if (auto_scroll != value) {
					auto_scroll = value;
					PerformLayout (this, "AutoScroll");
				}
			}
		}

		[Localizable(true)]
		[MWFCategory("Layout")]
		public Size AutoScrollMargin {
			get {
				return auto_scroll_margin;
			}

			set {
				if (value.Width < 0) {
					throw new ArgumentException("Width is assigned less than 0", "value.Width");
				}

				if (value.Height < 0) {
					throw new ArgumentException("Height is assigned less than 0", "value.Height");
				}

				auto_scroll_margin = value;
			}
		}

		internal bool ShouldSerializeAutoScrollMargin ()
		{
			return this.AutoScrollMargin != new Size (0, 0);
		}

		[Localizable(true)]
		[MWFCategory("Layout")]
		public Size AutoScrollMinSize {
			get {
				return auto_scroll_min_size;
			}

			set {
				if (value != auto_scroll_min_size) {
					auto_scroll_min_size = value;
					AutoScroll = true;
					PerformLayout (this, "AutoScrollMinSize");
				}
			}
		}

		internal bool ShouldSerializeAutoScrollMinSize ()
		{
			return this.AutoScrollMinSize != new Size (0, 0);
		}


		[MWFCategory("Layout")]
#if NET_2_0
		[Browsable (false)]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#else
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Localizable(true)]
#endif
		public DockPaddingEdges DockPadding {
			get {
				if (dock_padding == null)
					CreateDockPadding ();

				return dock_padding;
			}
		}

		#endregion	// Public Instance Properties

		#region Public Instance Methods

		public void SetAutoScrollMargin(int x, int y) {
			if (x < 0) {
				x = 0;
			}

			if (y < 0) {
				y = 0;
			}

			auto_scroll_margin = new Size(x, y);
			Recalculate (false);
		}
		#endregion	// Public Instance Methods

		#region Protected Instance Methods
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void AdjustFormScrollbars(bool displayScrollbars) {
			Recalculate (false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected bool GetScrollState(int bit) {
			// Internal MS
			return false;
		}
		

#if NET_2_0
		protected override void ScaleControl (SizeF factor, BoundsSpecified specified)
		{
			base.ScaleControl (factor, specified);
		}
#endif

		protected void SetDisplayRectLocation(int x, int y) {
			// This method is weird. MS documents that the scrollbars are not
			// updated. We need to move stuff, but leave the scrollbars as is

			if (x > 0) {
				x = 0;
			}

			if (y > 0) {
				y = 0;
			}

			ScrollWindow(scroll_position.X - x , scroll_position.Y - y);
		}

		protected void SetScrollState(int bit, bool value) {
			//throw new NotImplementedException();
		}
		
		#endregion	// Protected Instance Methods

		#region Internal & Private Methods


		// Normally DockPadding is created lazyly, as observed in the test cases, but some children
		// may need to have it always.
		internal void CreateDockPadding ()
		{
			if (dock_padding == null)
				dock_padding = new DockPaddingEdges (this);
		}

		private void Recalculate (object sender, EventArgs e) {
			Recalculate (true);
		}

#if NET_2_0
		private void HandleScrollEvent (object sender, ScrollEventArgs args)
		{
			OnScroll (args);
		}
#endif

	
		#endregion	// Internal & Private Methods

#if NET_2_0
		static object OnScrollEvent = new object ();
		
		protected virtual void OnScroll (ScrollEventArgs se)
		{
			ScrollEventHandler eh = (ScrollEventHandler) (Events [OnScrollEvent]);
			if (eh != null)
				eh (this, se);
		}

		protected override void OnPaddingChanged (EventArgs e)
		{
			base.OnPaddingChanged (e);
		}
		
		protected override void OnPaintBackground (PaintEventArgs e)
		{
			base.OnPaintBackground (e);
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected override void OnRightToLeftChanged (EventArgs e)
		{
			base.OnRightToLeftChanged (e);
		}

		public event ScrollEventHandler Scroll {
			add { Events.AddHandler (OnScrollEvent, value); }
			remove { Events.RemoveHandler (OnScrollEvent, value); }
		}
#endif
	}
}
