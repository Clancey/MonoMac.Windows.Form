using System;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public partial class Label 
	{
		/*
		public Label () : base()
		{
			m_helper.TextView.Selectable = false;
			m_helper.TextView.Editable = false;
			//this.Font = NSFont.FromFontName ("Arial", 10);
			//m_helper.Bordered = false;
		}
		/*
		public override bool AcceptsFirstResponder ()
		{
			return false;
		}
		private void resize ()
		{
			if (!AutoSize)
				return;
			//m_helper.SizeToFit ();
		}
		*/
		
		internal TextBoxMouseView m_helper;
				
		protected override void CreateHandle ()
		{
      		m_helper = new TextBoxMouseView();
			m_view = m_helper;
			m_helper.Host = this;
			m_helper.Editable = false;
			m_helper.DrawsBackground = false;
			//m_helper.viewDidMoveToSuperview += delegate(object sender, EventArgs e) {
			//	ViewDidMoveToSuperview();
			//};
		}
		
		
		
		public Label () :base ()
		{
			// Defaults in the Spec
			autosize = false;
			TabStop = false;
			string_format = new StringFormat();
			string_format.FormatFlags = StringFormatFlags.LineLimit;
			TextAlign = ContentAlignment.TopLeft;
			image = null;
			UseMnemonic = true;
			image_list = null;
			image_align = ContentAlignment.MiddleCenter;
			SetUseMnemonic (UseMnemonic);
			flat_style = FlatStyle.Standard;

			SetStyle (ControlStyles.Selectable, false);
			SetStyle (ControlStyles.ResizeRedraw | 
				ControlStyles.UserPaint | 
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.SupportsTransparentBackColor
				| ControlStyles.OptimizedDoubleBuffer
				, true);
			
			HandleCreated += new EventHandler (OnHandleCreatedLB);
		}
		
		#region Public Properties
				
		[SettingsBindable (true)]
		[Editor ("System.ComponentModel.Design.MultilineStringEditor, " + Consts.AssemblySystem_Design,
			 typeof (System.Drawing.Design.UITypeEditor))]
		public override string Text {
			get { return base.Text; }
			set { 
				m_helper.Value = value;
				base.Text = value;
			}
		}
		
		[DefaultValue(ContentAlignment.TopLeft)]
		[Localizable(true)]
		public virtual ContentAlignment TextAlign {
			get { return text_align; }

			set {
				if (!Enum.IsDefined (typeof (ContentAlignment), value))
					throw new InvalidEnumArgumentException (string.Format("Enum argument value '{0}' is not valid for ContentAlignment", value));

				if (text_align != value) {
					text_align = value;
					switch (value) {
					case ContentAlignment.BottomLeft:
						string_format.LineAlignment = StringAlignment.Far;
						string_format.Alignment = StringAlignment.Near;
						m_helper.Alignment = NSTextAlignment.Left;
						break;
					case ContentAlignment.BottomCenter:
						string_format.LineAlignment = StringAlignment.Far;
						string_format.Alignment = StringAlignment.Center;
						m_helper.Alignment = NSTextAlignment.Center;
						break;
					case ContentAlignment.BottomRight:
						string_format.LineAlignment = StringAlignment.Far;
						string_format.Alignment = StringAlignment.Far;
						m_helper.Alignment = NSTextAlignment.Right;
						break;
					case ContentAlignment.TopLeft:
						string_format.LineAlignment = StringAlignment.Near;
						string_format.Alignment = StringAlignment.Near;
						m_helper.Alignment = NSTextAlignment.Left;
						break;
					case ContentAlignment.TopCenter:
						string_format.LineAlignment = StringAlignment.Near;
						string_format.Alignment = StringAlignment.Center;
						m_helper.Alignment = NSTextAlignment.Center;
						break;
					case ContentAlignment.TopRight:
						string_format.LineAlignment = StringAlignment.Near;
						string_format.Alignment = StringAlignment.Far;
						m_helper.Alignment = NSTextAlignment.Right;
						break;
					case ContentAlignment.MiddleLeft:
						string_format.LineAlignment = StringAlignment.Center;
						string_format.Alignment = StringAlignment.Near;
						m_helper.Alignment = NSTextAlignment.Left;
						break;
					case ContentAlignment.MiddleRight:
						string_format.LineAlignment = StringAlignment.Center;
						string_format.Alignment = StringAlignment.Far;
						m_helper.Alignment = NSTextAlignment.Right;
						break;
					case ContentAlignment.MiddleCenter:
						string_format.LineAlignment = StringAlignment.Center;
						string_format.Alignment = StringAlignment.Center;
						m_helper.Alignment = NSTextAlignment.Center;
						break;
					default:
						break;
					}
					OnTextAlignChanged (EventArgs.Empty);
					Invalidate ();
				}
			}
		}
		
		internal virtual Size InternalGetPreferredSize (Size proposed)
		{
			Size size;

			if (Text == string.Empty) {
				size = new Size (0, Font.Height);
			} else {
				var txt = new NSText();
				txt.Value = Text;
				txt.Font = Font.ToNsFont();
				txt.SizeToFit();
				size = Size.Round(txt.Frame.Size);
			}

#if NET_2_0
			size.Width += Padding.Horizontal;
			size.Height += Padding.Vertical;
			
			if (!use_compatible_text_rendering)
				return size;
#else
				size.Height = Font.Height;
#endif

			if (border_style == BorderStyle.None)
				size.Height += 3;
			else
				size.Height += 6;
			
			return size;
		}
		
		protected internal void DrawImage (Graphics g, Image image, Rectangle r, ContentAlignment align)
		{
 			if (image == null || g == null)
				return;

			Rectangle rcImageClip = CalcImageRenderBounds (image, r, align);

			//if (Enabled)
				g.DrawImage (image, rcImageClip.X, rcImageClip.Y, rcImageClip.Width, rcImageClip.Height);
			//else
			//	ControlPaint.DrawImageDisabled (g, image, rcImageClip.X, rcImageClip.Y, BackColor);
		}
		
		protected override void OnPaint (PaintEventArgs e)
		{
			base.OnPaint(e);
		}
		
		
		#endregion
		
		

		private void CalcAutoSize ()
		{
			m_helper.TextContainer.TextView.AutoresizingMask = NSViewResizingMask.WidthSizable;
			var frame = m_helper.TextContainer.TextView.Frame;
			m_helper.HorizontallyResizable = true;
			m_helper.TextContainer.TextView.Frame = new RectangleF(frame.Location,new SizeF(9999,frame.Height));
			m_helper.Frame = new RectangleF(m_helper.Frame.Location, m_helper.TextContainer.TextView.Frame.Size);
		}
	}
}

