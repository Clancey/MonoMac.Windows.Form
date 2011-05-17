using System;
using System.Drawing;
using MonoMac.AppKit;
using System.ComponentModel;
namespace System.Windows.Forms
{
	public partial class ScrollableControl
	{
		internal NSScrollView m_helper;

		private void Recalculate (bool doLayout)
		{
			//TODO: make work
		}

		protected override void CreateHandle ()
		{
			m_helper = new NSScrollView ();
			if (m_view == null)
				m_view = m_helper;
		}

		public ScrollableControl ()
		{
			SetStyle (ControlStyles.ContainerControl, true);
			SetStyle (ControlStyles.AllPaintingInWmPaint, false);
			
			auto_scroll = false;
			force_hscroll_visible = false;
			force_vscroll_visible = false;
			auto_scroll_margin = new Size (0, 0);
			auto_scroll_min_size = new Size (0, 0);
			scroll_position = new Point (0, 0);
			SizeChanged += new EventHandler (Recalculate);
			VisibleChanged += new EventHandler (VisibleChangedHandler);
			LocationChanged += new EventHandler (LocationChangedHandler);
			ParentChanged += new EventHandler (ParentChangedHandler);
			//HandleCreated += new EventHandler (AddScrollbars);
			
			//CreateScrollbars ();
		}

		void LocationChangedHandler (object sender, EventArgs e)
		{
			//UpdateSizeGripVisible ();
		}

		void Parent_PaddingChanged (object sender, EventArgs e)
		{
			//UpdateSizeGripVisible ();
		}

		void Parent_SizeChanged (object sender, EventArgs e)
		{
			//UpdateSizeGripVisible ();
		}

		#region Public Instance Properties


		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Point AutoScrollPosition {
			get { return DisplayRectangle.Location; }

			set {
				if (value != AutoScrollPosition) {
					m_helper.ScrollPoint (value);
				}
			}
		}

		public override Rectangle DisplayRectangle {
			get {
				if (auto_scroll) {
					display_rectangle = Rectangle.Round (m_helper.VisibleRect ());
				} else {
					display_rectangle = base.DisplayRectangle;
				}
				
				if (dock_padding != null) {
					display_rectangle.X += dock_padding.Left;
					display_rectangle.Y += dock_padding.Top;
					display_rectangle.Width -= dock_padding.Left + dock_padding.Right;
					display_rectangle.Height -= dock_padding.Top + dock_padding.Bottom;
				}
				
				return display_rectangle;
			}
		}
		#endregion

		#region Public Instance Methods

		public void ScrollControlIntoView (Control activeControl)
		{
			int corner_x;
			int corner_y;
			
			Rectangle within = new Rectangle ();
			within.Size = ClientSize;
			
			if (!AutoScroll || (!m_helper.HasHorizontalRuler && !m_helper.HasVerticalRuler)) {
				return;
			}
			
			if (!Contains (activeControl)) {
				return;
			}
			
			m_helper.ScrollRectToVisible (new RectangleF (activeControl.Location, activeControl.Size));
		}

		#endregion

		#region Protected Instance Method


		protected bool HScroll {
			get { return m_helper.HasHorizontalRuler; }

			set { m_helper.HasHorizontalRuler = value; }
		}

		protected bool VScroll {
			get { return m_helper.HasVerticalRuler; }

			set { m_helper.HasVerticalRuler = value; }
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected override void OnLayout (LayoutEventArgs levent)
		{
			CalculateCanvasSize (true);
			
			AdjustFormScrollbars (AutoScroll);
			// Dunno what the logic is. Passing AutoScroll seems to match MS behaviour
			base.OnLayout (levent);
		}



		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected override void OnVisibleChanged (EventArgs e)
		{
			if (Visible) {
				//UpdateChildrenZOrder ();
				PerformLayout (this, "Visible");
			}
			base.OnVisibleChanged (e);
		}


		protected virtual Point ScrollToControl (Control activeControl)
		{
			int corner_x;
			int corner_y;
			
			Rectangle within = new Rectangle ();
			within.Size = ClientSize;
			
			
			// If the control is above the top or the left, move it down and right until it aligns 
			// with the top/left.
			// If the control is below the bottom or to the right, move it up/left until it aligns
			// with the bottom/right, but do never move it further than the top/left side.
			int x_diff = 0, y_diff = 0;
			
			if (activeControl.Top <= 0 || activeControl.Height >= within.Height)
				y_diff = -activeControl.Top; else if (activeControl.Bottom > within.Height)
				y_diff = within.Height - activeControl.Bottom;
			
			if (activeControl.Left <= 0 || activeControl.Width >= within.Width)
				x_diff = -activeControl.Left; else if (activeControl.Right > within.Width)
				x_diff = within.Width - activeControl.Right;
			
			corner_x = AutoScrollPosition.X + x_diff;
			corner_y = AutoScrollPosition.Y + y_diff;
			
			return new Point (corner_x, corner_y);
		}

		#endregion

		#region Internal and Private Methods

		internal virtual void CalculateCanvasSize (bool canOverride)
		{
			if(m_helper == null)
				return;
			Control child;
			int num_of_children;
			int width;
			int height;
			int extra_width;
			int extra_height;
			
			num_of_children = Controls.Count;
			width = 0;
			height = 0;
			
			extra_width = m_helper.HorizontalScroller == null ? 0 : m_helper.HorizontalScroller.IntValue;
			extra_height = m_helper.VerticalScroller == null ? 0 : m_helper.VerticalScroller.IntValue;
			if (dock_padding != null) {
				extra_width += dock_padding.Right;
				extra_height += dock_padding.Bottom;
			}
			
			for (int i = 0; i < num_of_children; i++) {
				child = Controls[i];
				if (child.Dock == DockStyle.Right) {
					extra_width += child.Width;
				} else if (child.Dock == DockStyle.Bottom) {
					extra_height += child.Height;
				}
			}
			
			if (!auto_scroll_min_size.IsEmpty) {
				width = auto_scroll_min_size.Width;
				height = auto_scroll_min_size.Height;
			}
			
			for (int i = 0; i < num_of_children; i++) {
				child = Controls[i];
				
				switch (child.Dock) {
				case DockStyle.Left:
					
					{
						if ((child.Right + extra_width) > width) {
							width = child.Right + extra_width;
						}
						continue;
					}

				
				case DockStyle.Top:
					
					{
						if ((child.Bottom + extra_height) > height) {
							height = child.Bottom + extra_height;
						}
						continue;
					}

				
				case DockStyle.Fill:
				case DockStyle.Right:
				case DockStyle.Bottom:
					
					{
						continue;
					}

				default:
					
					
					{
						AnchorStyles anchor;
						
						anchor = child.Anchor;
						
						if (((anchor & AnchorStyles.Left) != 0) && ((anchor & AnchorStyles.Right) == 0)) {
							if ((child.Right + extra_width) > width) {
								width = child.Right + extra_width;
							}
						}
						
						if (((anchor & AnchorStyles.Top) != 0) || ((anchor & AnchorStyles.Bottom) == 0)) {
							if ((child.Bottom + extra_height) > height) {
								height = child.Bottom + extra_height;
							}
						}
						continue;
					}

				}
			}
			
			canvas_size.Width = width;
			canvas_size.Height = height;
		}

		private void ScrollWindow (int XOffset, int YOffset)
		{
			int num_of_children;
			
			if (XOffset == 0 && YOffset == 0) {
				return;
			}
			
			SuspendLayout ();
			
			num_of_children = Controls.Count;
			
			for (int i = 0; i < num_of_children; i++) {
				Controls[i].Location = new Point (Controls[i].Left - XOffset, Controls[i].Top - YOffset);
				//Controls[i].Left -= XOffset;
				//Controls[i].Top -= YOffset;
				// Is this faster? Controls[i].Location -= new Size(XOffset, YOffset);
			}
			
			scroll_position.X += XOffset;
			scroll_position.Y += YOffset;
			var rect = ClientRectangle;
			rect.Location = new Point (XOffset, YOffset);
			m_helper.ScrollRectToVisible (rect);
			ResumeLayout (false);
		}
		#endregion
	}
}

