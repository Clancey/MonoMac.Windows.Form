using System;
using System.Drawing;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public partial class ScrollableControl
	{	
		internal NSScrollView  m_helper;
		internal override NSView c_helper {
			get {
				return m_helper;
			}
			set {
				m_helper = value as NSScrollView;
			}
		}
		
		private void Recalculate (bool doLayout) {
			if (!IsHandleCreated) {
				return;
			}

			Size canvas = canvas_size;
			Size client = ClientSize;

			canvas.Width += auto_scroll_margin.Width;
			canvas.Height += auto_scroll_margin.Height;

			int right_edge = client.Width;
			int bottom_edge = client.Height;
			int prev_right_edge;
			int prev_bottom_edge;

			bool hscroll_visible;
			bool vscroll_visible;

			do {
				prev_right_edge = right_edge;
				prev_bottom_edge = bottom_edge;

				if ((force_hscroll_visible || (canvas.Width > right_edge && auto_scroll)) && client.Width > 0) {
					hscroll_visible = true;
					bottom_edge = client.Height - SystemInformation.HorizontalScrollBarHeight;
				} else {
					hscroll_visible = false;
					bottom_edge = client.Height;
				}

				if ((force_vscroll_visible || (canvas.Height > bottom_edge && auto_scroll)) && client.Height > 0) {
					vscroll_visible = true;
					right_edge = client.Width - SystemInformation.VerticalScrollBarWidth;
				} else {
					vscroll_visible = false;
					right_edge = client.Width;
				}

			} while (right_edge != prev_right_edge || bottom_edge != prev_bottom_edge);

			if (right_edge < 0) right_edge = 0;
			if (bottom_edge < 0) bottom_edge = 0;

			Rectangle hscroll_bounds;
			Rectangle vscroll_bounds;

			hscroll_bounds = new Rectangle (0, client.Height - SystemInformation.HorizontalScrollBarHeight,
							ClientRectangle.Width, SystemInformation.HorizontalScrollBarHeight);
			vscroll_bounds = new Rectangle (client.Width - SystemInformation.VerticalScrollBarWidth, 0,
							SystemInformation.VerticalScrollBarWidth, ClientRectangle.Height);

			/* the ScrollWindow calls here are needed
			 * because (this explanation sucks):
			 * 
			 * when we transition from having a scrollbar to
			 * not having one, we won't receive a scrollbar
			 * moved (value changed) event, so we need to
			 * manually scroll the canvas.
			 * 
			 * if you can fix this without requiring the
			 * ScrollWindow calls, pdb and toshok will each
			 * pay you $5.
			*/

			if (!vscrollbar.Visible) {
				vscrollbar.Value = 0;
			}
			if (!hscrollbar.Visible) {
				hscrollbar.Value = 0;
			}

			/* Manually setting the size of the thumb should be done before
			 * the other assignments */
			if (hscroll_visible) {
				hscrollbar.manual_thumb_size = right_edge;
				hscrollbar.LargeChange = right_edge;
				hscrollbar.SmallChange = 5;
				hscrollbar.Maximum = canvas.Width - 1;
			} else {
				if (hscrollbar != null && hscrollbar.VisibleInternal) {
					ScrollWindow (- scroll_position.X, 0);
				}
				scroll_position.X = 0;
			}

			if (vscroll_visible) {
				vscrollbar.manual_thumb_size = bottom_edge;
				vscrollbar.LargeChange = bottom_edge;
				vscrollbar.SmallChange = 5;
				vscrollbar.Maximum = canvas.Height - 1;
			} else {
				if (vscrollbar != null && vscrollbar.VisibleInternal) {
					ScrollWindow (0, - scroll_position.Y);
				}
				scroll_position.Y = 0;
			}

			if (hscroll_visible && vscroll_visible) {
				hscroll_bounds.Width -= SystemInformation.VerticalScrollBarWidth;
				vscroll_bounds.Height -= SystemInformation.HorizontalScrollBarHeight;

				sizegrip.Bounds = new Rectangle (hscroll_bounds.Right,
								 vscroll_bounds.Bottom,
								 SystemInformation.VerticalScrollBarWidth,
								 SystemInformation.HorizontalScrollBarHeight);
			}
			
			SuspendLayout ();

			hscrollbar.SetBoundsInternal (hscroll_bounds.X, hscroll_bounds.Y, hscroll_bounds.Width, hscroll_bounds.Height, BoundsSpecified.None);
			hscrollbar.Visible = hscroll_visible;
			if (hscrollbar.Visible)
				hscrollbar.c_helper.Superview.BringToFront();

			vscrollbar.SetBoundsInternal (vscroll_bounds.X, vscroll_bounds.Y, vscroll_bounds.Width, vscroll_bounds.Height, BoundsSpecified.None);
			vscrollbar.Visible = vscroll_visible;
			if (vscrollbar.Visible)
				vscrollbar.c_helper.Superview.BringToFront();

			UpdateSizeGripVisible ();

			ResumeLayout (doLayout);
			
			// We should now scroll the active control into view, 
			// the funny part is that ScrollableControl does not have 
			// the concept of active control.
			ContainerControl container = this as ContainerControl;
			if (container != null && container.ActiveControl != null) {
				ScrollControlIntoView (container.ActiveControl);
			}
		}
		
		public ScrollableControl() {
			m_helper = new NSScrollView();
			SetStyle(ControlStyles.ContainerControl, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, false);

			auto_scroll = false;
			force_hscroll_visible = false;
			force_vscroll_visible = false;
			auto_scroll_margin = new Size(0, 0) ;
			auto_scroll_min_size = new Size(0, 0);
			scroll_position = new Point(0, 0);
			SizeChanged +=new EventHandler(Recalculate);
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
		
		#region Internal and Private Methods
		private void ScrollWindow(int XOffset, int YOffset) {
			int	num_of_children;

			if (XOffset == 0 && YOffset == 0) {
				return;
			}

			SuspendLayout();

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
			rect.Location = new Point(XOffset,YOffset);
			m_helper.ScrollRectToVisible(rect);
			ResumeLayout(false);
		}
		#endregion
	}
}

