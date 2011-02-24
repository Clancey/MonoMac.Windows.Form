using System;
using System.Drawing;
namespace System.Windows.Forms
{
	public partial class ListView
	{
		
		#region Public Constructors
		public ListView ()
		{
			background_color = Color.White;//ThemeEngine.Current.ColorWindow;
			groups = new ListViewGroupCollection (this);
			items = new ListViewItemCollection (this);
			items.Changed += new CollectionChangedHandler (OnItemsChanged);
			checked_indices = new CheckedIndexCollection (this);
			checked_items = new CheckedListViewItemCollection (this);
			columns = new ColumnHeaderCollection (this);
			foreground_color = SystemColors.WindowText;
			selected_indices = new SelectedIndexCollection (this);
			selected_items = new SelectedListViewItemCollection (this);
			items_location = new Point [16];
			items_matrix_location = new ItemMatrixLocation [16];
			reordered_items_indices = new int [16];
			item_tooltip = new ToolTip ();
			item_tooltip.Active = false;
			insertion_mark = new ListViewInsertionMark (this);

			InternalBorderStyle = BorderStyle.Fixed3D;

			header_control = new HeaderControl (this);
			header_control.Visible = false;
			Controls.AddImplicit (header_control);

			item_control = new ItemControl (this);
			Controls.AddImplicit (item_control);

			h_marker = v_marker = 0;
			keysearch_tickcnt = 0;

			// scroll bars are disabled initially
			
			h_scroll.Visible = false;
			h_scroll.ValueChanged += new EventHandler(HorizontalScroller);
			v_scroll.Visible = false;
			v_scroll.ValueChanged += new EventHandler(VerticalScroller);

			// event handlers
			base.KeyDown += new KeyEventHandler(ListView_KeyDown);
			SizeChanged += new EventHandler (ListView_SizeChanged);
			GotFocus += new EventHandler (FocusChanged);
			LostFocus += new EventHandler (FocusChanged);
			MouseWheel += new MouseEventHandler(ListView_MouseWheel);
			MouseEnter += new EventHandler (ListView_MouseEnter);
			Invalidated += new InvalidateEventHandler (ListView_Invalidated);

			BackgroundImageTiled = false;

			this.SetStyle (ControlStyles.UserPaint | ControlStyles.StandardClick
				| ControlStyles.UseTextForAccessibility
				, false);
		}
		#endregion	// Public Constructors
		
		#region Private Internal Properties
		internal Size CheckBoxSize {
			get {
				if (this.check_boxes) {
					if (this.state_image_list != null)
						return this.state_image_list.ImageSize;
					else
						return new Size(25,25);//ThemeEngine.Current.ListViewCheckBoxSize;
				}
				return Size.Empty;
			}
		}
		
		
		internal bool UsingGroups {
			get {
				return show_groups && groups.Count > 0 && view != View.List;// && 	Application.VisualStylesEnabled;
			}
		}

		
		#endregion
		
		
		#region	 Protected Properties
		protected override Size DefaultSize {
			get { return ThemeEngine.Current.ListViewDefaultSize; }
		}
		#endregion
		
		
		public override Color BackColor {
			get {
				if (background_color.IsEmpty)
					return Color.White;
				else
					return background_color;
			}
			set { 
				background_color = value;
				item_control.BackColor = value;
			}
		}
		

		public override Color ForeColor {
			get {
				if (foreground_color.IsEmpty)
					return Color.Black;
				else
					return foreground_color;
			}
			set { foreground_color = value; }
		}

	}
}

