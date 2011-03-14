using System;
using System.Drawing;
using System.ComponentModel;
using MonoMac.AppKit;

namespace System.Windows.Forms
{
	public partial class ToolStrip : ScrollableControl
	{
		internal ToolBarHelper m_helper;
		public ToolStrip (params ToolStripItem[] items) : base ()
		{
			SetStyle (ControlStyles.AllPaintingInWmPaint, true);
			SetStyle (ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle (ControlStyles.Selectable, false);
			SetStyle (ControlStyles.SupportsTransparentBackColor, true);

			this.SuspendLayout ();
			
			this.items = new ToolStripItemCollection (this, items, true);
			this.allow_merge = true;
			base.AutoSize = true;
			this.SetAutoSizeMode (AutoSizeMode.GrowAndShrink);
			this.back_color = Control.DefaultBackColor;
			this.can_overflow = true;
			base.CausesValidation = false;
			//this.default_drop_down_direction = ToolStripDropDownDirection.BelowRight;
			this.displayed_items = new ToolStripItemCollection (this, null, true);
			this.Dock = this.DefaultDock;
			//base.Font = new Font ("Tahoma", 8.25f);
			this.fore_color = Control.DefaultForeColor;
			this.grip_margin = this.DefaultGripMargin;
			//this.grip_style = ToolStripGripStyle.Visible;
			this.image_scaling_size = new Size (16, 16);
			//this.layout_style = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.orientation = Orientation.Horizontal;
			//if (!(this is ToolStripDropDown))
			//	this.overflow_button = new ToolStripOverflowButton (this);
			//this.renderer = null;
			//this.render_mode = ToolStripRenderMode.ManagerRenderMode;
			this.show_item_tool_tips = this.DefaultShowItemToolTips;
			base.TabStop = false;
			//this.text_direction = ToolStripTextDirection.Horizontal;
			this.ResumeLayout ();
			
			// Register with the ToolStripManager
			//ToolStripManager.AddToolStrip (this);
		}
		
		protected override void CreateHandle ()
		{
			base.CreateHandle();
			m_helper = new ToolBarHelper(this);
		}
		
		public static implicit operator NSToolbar (ToolStrip toolstrip)
		{
			return toolstrip.m_helper;
		}
		
		[DefaultValue (DockStyle.Top)]
		public override DockStyle Dock {
			get { return base.Dock; }
			set {
				if (base.Dock != value) {
					base.Dock = value;
					
					switch (value) {
						case DockStyle.Top:
						case DockStyle.Bottom:
						case DockStyle.None:
							//this.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
							break;
						case DockStyle.Left:
						case DockStyle.Right:
							//this.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
							break;
					}
				}
			}
		}
		
		protected internal virtual ToolStripItem CreateDefaultItem (string text, Image image, EventHandler onClick)
		{
			//if (text == "-")
			//	return new ToolStripSeparator ();

			//if (this is ToolStripDropDown)
			//	return new ToolStripMenuItem (text, image, onClick);
				
			return new ToolStripButton (text, image, onClick);
		}
		
		protected override void Dispose (bool disposing)
		{
			if (!IsDisposed) {
				//CloseToolTip (null);
				// ToolStripItem.Dispose modifes the collection,
				// so we iterate it in reverse order
				for (int i = Items.Count - 1; i >= 0; i--)
					Items [i].Dispose ();
					
				//if (this.overflow_button != null && this.overflow_button.drop_down != null)
				//	this.overflow_button.drop_down.Dispose ();

				//ToolStripManager.RemoveToolStrip (this);
				base.Dispose (disposing);
			}
		}
		
		
		protected internal virtual void OnItemAdded (ToolStripItemEventArgs e)
		{
			if (e.Item.InternalVisible)
				e.Item.Available = true;
				
			m_helper.InsertItem(e.Item.Text,0);
			//e.Item.SetPlacement (ToolStripItemPlacement.Main);
			
			if (this.Created)
				this.PerformLayout ();
			
			ToolStripItemEventHandler eh = (ToolStripItemEventHandler)(Events [ItemAddedEvent]);
			if (eh != null)
				eh (this, e);
		}
		
		protected virtual void OnItemClicked (ToolStripItemClickedEventArgs e)
		{
			//if (this.KeyboardActive)
			//	ToolStripManager.SetActiveToolStrip (null, false);
			
			ToolStripItemClickedEventHandler eh = (ToolStripItemClickedEventHandler)(Events [ItemClickedEvent]);
			if (eh != null)
				eh (this, e);
		}
		
		
		protected override void OnMouseDown (MouseEventArgs mea)
		{
			/*
			if (mouse_currently_over != null)
			{
				ToolStripItem focused = GetCurrentlyFocusedItem ();

				if (focused != null && focused != mouse_currently_over)
					this.FocusInternal (true);

				//if (this is MenuStrip && !menu_selected) {
				//	(this as MenuStrip).FireMenuActivate ();
				//	menu_selected = true;				
				//}
					
				mouse_currently_over.FireEvent (mea, ToolStripItemEventType.MouseDown);
				
				if (this is MenuStrip && mouse_currently_over is ToolStripMenuItem && !(mouse_currently_over as ToolStripMenuItem).HasDropDownItems)
					return;
			} else {
				this.HideMenus (true, ToolStripDropDownCloseReason.AppClicked);
			}
			
			if (this is MenuStrip)
				this.Capture = false;
			*/
			base.OnMouseDown (mea);
		}
		
		
		protected override void OnMouseMove (MouseEventArgs mea)
		{
			/*
			ToolStripItem tsi;
			// Find the item we are now 
			//if (this.overflow_button != null && this.overflow_button.Visible && this.overflow_button.Bounds.Contains (mea.Location))
			//	tsi = this.overflow_button;
			//else
				tsi = this.GetItemAt (mea.X, mea.Y);

			if (tsi != null) {
				// If we were already hovering on this item, just send a mouse move
				if (tsi == mouse_currently_over) 
					tsi.FireEvent (mea, ToolStripItemEventType.MouseMove);
				else {
					// If we were over a different item, fire a mouse leave on it
					if (mouse_currently_over != null) {
						MouseLeftItem (tsi);
						mouse_currently_over.FireEvent (mea, ToolStripItemEventType.MouseLeave);
					}
					
					// Set the new item we are currently over
					mouse_currently_over = tsi;
					
					// Fire mouse enter and mouse move
					tsi.FireEvent (mea, ToolStripItemEventType.MouseEnter);
					MouseEnteredItem (tsi);
					tsi.FireEvent (mea, ToolStripItemEventType.MouseMove);

					// If we're over something with a drop down, show it
					if (menu_selected && mouse_currently_over.Enabled && mouse_currently_over is ToolStripDropDownItem && (mouse_currently_over as ToolStripDropDownItem).HasDropDownItems)
						(mouse_currently_over as ToolStripDropDownItem).ShowDropDown ();
				}
			} else {
				// We're not over anything now, just fire the mouse leave on what we used to be over
				if (mouse_currently_over != null) {
					MouseLeftItem (tsi);
					mouse_currently_over.FireEvent (mea, ToolStripItemEventType.MouseLeave);
					mouse_currently_over = null;
				}
			}
			*/
			base.OnMouseMove (mea);
		}
		
		protected override void OnMouseLeave (EventArgs e)
		{

			base.OnMouseLeave (e);
		}

		protected override void OnMouseUp (MouseEventArgs mea)
		{
			/*
			// If we're currently over an item (set in MouseMove)
			if (mouse_currently_over != null && !(mouse_currently_over is ToolStripControlHost) && mouse_currently_over.Enabled) {
				// Fire our ItemClicked event
				OnItemClicked (new ToolStripItemClickedEventArgs (mouse_currently_over));
					
				// Fire the item's MouseUp event
				if (mouse_currently_over != null)
					mouse_currently_over.FireEvent (mea, ToolStripItemEventType.MouseUp);

				// The event handler may have blocked until the mouse moved off of the ToolStripItem
				if (mouse_currently_over == null)
					return;
			}
			 */
			base.OnMouseUp (mea);
		}
		
		
		protected override void OnPaint (PaintEventArgs e)
		{
			base.OnPaint (e);
			/*
			// Draw the grip
			this.OnPaintGrip (e);

			// Make each item draw itself
			for (int i = 0; i < displayed_items.Count; i++) {
				ToolStripItem tsi = displayed_items[i];
				
				if (tsi.Visible) {
					e.Graphics.TranslateTransform (tsi.Bounds.Left, tsi.Bounds.Top);
					tsi.FireEvent (e, ToolStripItemEventType.Paint);
					e.Graphics.ResetTransform ();
				}
			}

			// Paint the Overflow button if it's visible
			if (this.overflow_button != null && this.overflow_button.Visible) {
				e.Graphics.TranslateTransform (this.overflow_button.Bounds.Left, this.overflow_button.Bounds.Top);
				this.overflow_button.FireEvent (e, ToolStripItemEventType.Paint);
				e.Graphics.ResetTransform ();
			}

			Rectangle affected_bounds = new Rectangle (Point.Empty, this.Size);

			ToolStripRenderEventArgs pevent = new ToolStripRenderEventArgs (e.Graphics, this, affected_bounds, Color.Empty);
			pevent.InternalConnectedArea = CalculateConnectedArea ();

			this.Renderer.DrawToolStripBorder (pevent);
			*/
		}
		
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected override void OnPaintBackground (PaintEventArgs e)
		{
			base.OnPaintBackground (e);

			//Rectangle affected_bounds = new Rectangle (Point.Empty, this.Size);
			//ToolStripRenderEventArgs tsrea = new ToolStripRenderEventArgs (e.Graphics, this, affected_bounds, SystemColors.Control);
			
			//this.Renderer.DrawToolStripBackground (tsrea);
		}
		
		protected override void OnLayout (LayoutEventArgs e)
		{
			base.OnLayout (e);

			//this.SetDisplayedItems ();
			this.OnLayoutCompleted (EventArgs.Empty);
			this.Invalidate ();
		}
				
		protected internal static void SetItemParent (ToolStripItem item, ToolStrip parent)
		{
			if (item.Owner != null) {
				item.Owner.Items.RemoveNoOwnerOrLayout (item);

				//if (item.Owner is ToolStripOverflow)
				//	(item.Owner as ToolStripOverflow).ParentToolStrip.Items.RemoveNoOwnerOrLayout (item);
			}
			
			parent.Items.AddNoOwnerOrLayout (item);
			item.Parent = parent;
		}
		internal virtual void Dismiss (ToolStripDropDownCloseReason reason)
		{
			// Release our stranglehold on the keyboard
			//this.KeyboardActive = false;
			
			// Set our drop down flag to false;
			this.menu_selected = false;
			
			// Make sure all of our items are deselected and repainted
			foreach (ToolStripItem tsi in this.Items)
				tsi.Dismiss (reason);
				
			// We probably need to redraw for mnemonic underlines
			this.Invalidate ();
		}
		
		internal virtual Size GetToolStripPreferredSize (Size proposedSize)
		{
			return proposedSize;
			
		}
		
		internal void HideMenus (bool release, ToolStripDropDownCloseReason reason)
		{
			//if (this is MenuStrip && release && menu_selected)
			//	(this as MenuStrip).FireMenuDeactivate ();
				
			if (release)
				menu_selected = false;
				
			NotifySelectedChanged (null);
		}
		
		internal void NotifySelectedChanged (ToolStripItem tsi)
		{
			//foreach (ToolStripItem tsi2 in this.DisplayedItems)
				//if (tsi != tsi2)
					//if (tsi2 is ToolStripDropDownItem)
					//	(tsi2 as ToolStripDropDownItem).HideDropDown (ToolStripDropDownCloseReason.Keyboard);

			//if (this.OverflowButton != null) {
			//	ToolStripItemCollection tsic = this.OverflowButton.DropDown.DisplayedItems;
				
				//foreach (ToolStripItem tsi2 in tsic)
				//	if (tsi != tsi2)
				//		if (tsi2 is ToolStripDropDownItem)
				//			(tsi2 as ToolStripDropDownItem).HideDropDown (ToolStripDropDownCloseReason.Keyboard);
			
				//this.OverflowButton.HideDropDown ();
			//}
			
			foreach (ToolStripItem tsi2 in this.Items)
				if (tsi != tsi2)
					tsi2.Dismiss (ToolStripDropDownCloseReason.Keyboard);
		}
				
		protected override void OnVisibleChanged (EventArgs e)
		{
			//if (!Visible)
			//	CloseToolTip (null);

			base.OnVisibleChanged (e);
		}
		///
	}
}

