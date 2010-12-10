using System;
using System.Drawing;
using MonoMac.AppKit;
using System.Linq;

namespace System.Windows.Forms
{
	public class Control
	{
		internal virtual NSView c_helper { get; set; }

		public Control ()
		{
			c_helper = new NSView ();
		}

		public static implicit operator NSView (Control control)
		{
			return control.c_helper;
		}
		public AnchorStyles Anchor {get;set;}
		
		public virtual Color BackColor { get; set; }

		public int Bottom {
			get {
				if (c_helper.Superview == null)
					return 0;
				return (int)(c_helper.Superview.Frame.Height - (this.Height + this.Location.Y));
			}
		}

		public virtual Rectangle Bounds {
			get { return Rectangle.Round (c_helper.Bounds); }
			set { c_helper.Bounds = value; }
		}

		public virtual Rectangle ClientRectangle {
			get { return Rectangle.Round (c_helper.Frame); }
		}

		public virtual Size ClientSize {
			get { return new Size ((int)Size.Width, (int)Size.Height); }
			set { Size = value; }
		}

		public virtual bool ContainsFocus {
			get { return c_helper == c_helper.Window.FirstResponder || c_helper.Subviews.Where (x => x == c_helper.Window.FirstResponder).Count () > 0; }
		}


		internal ControlCollection theControls;
		public virtual ControlCollection Controls {
			get {
				if (theControls == null)
					theControls = new ControlCollection (c_helper);
				return theControls;
			}
		}

		public virtual Color DefaultBackColor {
			get { return Color.Gray; }
		}

		public virtual Font Font { get; set; }
		protected int FontHeight { get; set; }

		public virtual bool Enabled {
			get { return c_helper.AcceptsFirstResponder (); }
		}
		//TODO: Setter

		public virtual bool Focused {
			get { return c_helper == c_helper.Window.FirstResponder; }
		}

		/*
		public virtual new System.Drawing.Font Font {
			get {
				if (m_helper.Font == null)
					return new System.Drawing.Font ("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				return new System.Drawing.Font (m_helper.Font.FontName, m_helper.Font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}
				
			set { m_helper.Font = MonoMac.AppKit.NSFont.FromFontName (value.Name, value.Size); }
		}
		protected int FontHeight {
			get { return Font.Height; }
			//set { Font.Height = value;}
		}
		 
		 */
		
		//TODO: set
		private Color foreColor = Color.Gray;
		public virtual Color ForeColor
		{
			get { return foreColor; }
			set { foreColor = value;}
		}
		
		public virtual IntPtr Handle {
			get { return c_helper.Handle; }
		}
		
		public bool HasChildren { 
			get { return Controls.Count > 0; }
		}
		

		public virtual float Height {
			get { return this.Size.Height; }
			set { this.Size = new SizeF (this.Size.Width, value); }
		}		
		
		public virtual PointF Location {
			get { return c_helper.Frame.Location; }
			set { c_helper.Frame = new RectangleF (value, c_helper.Frame.Size); }
		}
		
		public string Name { get; set; }

		public virtual SizeF Size {
			get { return c_helper.Frame.Size; }
			set { c_helper.Frame = new RectangleF (c_helper.Frame.Location, value); }
		}		
		
		[Obsolete("Not Implemented.", false)]
		public int TabIndex {get;set;}		
		
		public virtual float Width {
			get { return this.Size.Width; }
			set { this.Size = new SizeF (value, this.Size.Height); }
		}

		public bool Visible {
			get { return c_helper.Hidden; }
			set { c_helper.Hidden = value; }
		}
	}
}

