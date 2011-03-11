using System;
using System.Drawing;
using System.ComponentModel;
using MonoMac.AppKit;

namespace System.Windows.Forms
{
	public partial class ToolStripItem : Component
	{
		internal int Index;
		protected ToolStripItem (string text, Image image, EventHandler onClick, string name)
		{
			//this.alignment = ToolStripItemAlignment.Left;
			this.anchor = AnchorStyles.Left | AnchorStyles.Top;
			this.auto_size = true;
			this.auto_tool_tip = this.DefaultAutoToolTip;
			this.available = true;
			this.back_color = Color.Empty;
			this.background_image_layout = ImageLayout.Tile;
			this.can_select = true;
			//this.display_style = this.DefaultDisplayStyle;
			this.dock = DockStyle.None;
			this.enabled = true;
			this.fore_color = Color.Empty;
			this.image = image;
			this.image_align = ContentAlignment.MiddleCenter;
			this.image_index = -1;
			this.image_key = string.Empty;
			//this.image_scaling = ToolStripItemImageScaling.SizeToFit;
			this.image_transparent_color = Color.Empty;
			this.margin = this.DefaultMargin;
			//this.merge_action = MergeAction.Append;
			this.merge_index = -1;
			this.name = name;
			//this.overflow = ToolStripItemOverflow.AsNeeded;
			this.padding = this.DefaultPadding;
			//this.placement = ToolStripItemPlacement.None;
			this.right_to_left = RightToLeft.Inherit;
			this.bounds.Size = this.DefaultSize;
			this.text = text;
			this.text_align = ContentAlignment.MiddleCenter;
			//this.text_direction = DefaultTextDirection;
			//this.text_image_relation = TextImageRelation.ImageBeforeText;
			this.visible = true;
			
			this.Click += onClick;
			OnLayout (new LayoutEventArgs (null, string.Empty));
		}
		internal NSToolbarItem m_helper;
		
		[Browsable (false)]
		public Rectangle ContentRectangle {
			get {
				// ToolStripLabels don't have a border
				//if (this is ToolStripLabel || this is ToolStripStatusLabel)
				//	return new Rectangle (0, 0, this.bounds.Width, this.bounds.Height);

				//if (this is ToolStripDropDownButton && (this as ToolStripDropDownButton).ShowDropDownArrow)
				//	return new Rectangle (2, 2, this.bounds.Width - 13, this.bounds.Height - 4);

				return new Rectangle (2, 2, this.bounds.Width - 4, this.bounds.Height - 4);
			}
		}
		/*
		protected override void CreateHandle ()
		{
			//base.CreateHandle();
			m_helper = new NSToolbarItem();
		}
		*/
				
		[Browsable (false)]
		public bool IsOnDropDown {
			get {
				//if (this.parent != null && this.parent is ToolStripDropDown)
				//	return true;

				return false;
			}
		}
		
		[Browsable (false)]
		public bool IsOnOverflow {
			get { return false;}//return this.placement == ToolStripItemPlacement.Overflow; }
		}
		
				
		public void Select ()
		{
			if (!this.is_selected && this.CanSelect) {
				this.is_selected = true;
				
				if (this.Parent != null) {
					//if (this.Visible && this.Parent.Focused && this is ToolStripControlHost)
					//	(this as ToolStripControlHost).Focus ();
						
					this.Invalidate ();
					this.Parent.NotifySelectedChanged (this);
				}
				OnUIASelectionChanged ();
			}
		}
		
		
		protected virtual void OnMouseUp (MouseEventArgs e)
		{
			if (this.Enabled) {
				this.is_pressed = false;
				this.Invalidate ();

				//if (this.IsOnDropDown)
					//if (!(this is ToolStripDropDownItem) || !(this as ToolStripDropDownItem).HasDropDownItems || (this as ToolStripDropDownItem).DropDown.Visible == false) {
					//	if ((this.Parent as ToolStripDropDown).OwnerItem != null)
					//		((this.Parent as ToolStripDropDown).OwnerItem as ToolStripDropDownItem).HideDropDown ();
					//	else
					//		(this.Parent as ToolStripDropDown).Hide ();
					//}
						
				
				MouseEventHandler eh = (MouseEventHandler)(Events [MouseUpEvent]);
				if (eh != null)
					eh (this, e);
			}
		}
		
				
		void OnPaintInternal (PaintEventArgs e)
		{
			// Have the background rendered independently from OnPaint
			//if (this.parent != null)
			//	this.parent.Renderer.DrawItemBackground (new ToolStripItemRenderEventArgs (e.Graphics, this));

			OnPaint (e);
		}
		
		
		protected virtual void OnParentChanged (ToolStrip oldParent, ToolStrip newParent)
		{
			//this.text_size = TextRenderer.MeasureText (this.Text == null ? string.Empty : this.text, this.Font, Size.Empty, TextFormatFlags.HidePrefix);
			
			if (oldParent != null)
				oldParent.PerformLayout ();
				
			if (newParent != null)
				newParent.PerformLayout ();
		}
		
		internal void CalculateAutoSize ()
		{
			//m_helper.Image = NSImage.FromObject(image);
		}

		internal virtual Size CalculatePreferredSize (Size constrainingSize)
		{
			if (!this.auto_size)
				return this.explicit_size;
				
			Size preferred_size = this.DefaultSize;


			return preferred_size;
		}
		
				
		internal void CalculateTextAndImageRectangles (Rectangle contentRectangle, out Rectangle text_rect, out Rectangle image_rect)
		{
		
		}

		
		internal Size GetImageSize ()
		{
			// Get the actual size of our internal image -or-
			// Get the ImageList.ImageSize if we are using ImageLists
			//if (this.image_scaling == ToolStripItemImageScaling.None) {
			if (this.image != null)	{
					return image.Size;
					
				if (this.image_index >= 0 || !string.IsNullOrEmpty (this.image_key))
					if (this.owner != null && this.owner.ImageList != null)
						return this.owner.ImageList.ImageSize;
			}
			 else {
				// If we have an image and a parent, return ImageScalingSize
				if (this.Parent == null)
					return Size.Empty;
					
				if (this.image != null)
					return this.Parent.ImageScalingSize;

				if (this.image_index >= 0 || !string.IsNullOrEmpty (this.image_key))
					if (this.owner != null && this.owner.ImageList != null)
						return this.Parent.ImageScalingSize;
			}
			
			return Size.Empty;
		}
		///
	}
}

