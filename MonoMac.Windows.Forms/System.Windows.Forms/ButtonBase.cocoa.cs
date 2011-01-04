using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public abstract partial class ButtonBase
	{
		internal ButtonHelper m_helper;
		internal override  NSView c_helper {
			get {
				return m_helper;
			}
			set {
				m_helper = value as ButtonHelper;
			}
		}
		
		
		#region Public Constructors
		protected ButtonBase() : base()
		{
			m_helper = new ButtonHelper();
			m_helper.Host = this;
			
			
			text_format	= new StringFormat();
			text_format.Alignment = StringAlignment.Center;
			text_format.LineAlignment = StringAlignment.Center;
			text_format.HotkeyPrefix = HotkeyPrefix.Show;
			text_format.FormatFlags |= StringFormatFlags.LineLimit;
			
			text_format_flags = TextFormatFlags.HorizontalCenter;
			text_format_flags |= TextFormatFlags.VerticalCenter;
			text_format_flags |= TextFormatFlags.TextBoxControl;

			SetStyle (ControlStyles.ResizeRedraw | 
				ControlStyles.Opaque | 
				ControlStyles.UserMouse | 
				ControlStyles.SupportsTransparentBackColor | 
				ControlStyles.CacheText |
				ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle (ControlStyles.StandardClick, false);
		}
		#endregion	// Public Constructors
		
		
		[SettingsBindable (true)]
		[Editor ("System.ComponentModel.Design.MultilineStringEditor, " + Consts.AssemblySystem_Design,
			 "System.Drawing.Design.UITypeEditor, " + Consts.AssemblySystem_Drawing)]
		public override string Text {
			get { return m_helper.Title; }
			set {
				m_helper.Title = value;
				resize();
			}
		}
		
		
		
		[Localizable(true)]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		[MWFDescription("Alignment for button text"), MWFCategory("Appearance")]
		public virtual ContentAlignment TextAlign {
			get { return text_alignment; }
			set {
				if (text_alignment != value) {
					text_alignment = value;

					text_format_flags &= ~TextFormatFlags.Bottom;
					text_format_flags &= ~TextFormatFlags.Top;
					text_format_flags &= ~TextFormatFlags.Left;
					text_format_flags &= ~TextFormatFlags.Right;
					text_format_flags &= ~TextFormatFlags.HorizontalCenter;
					text_format_flags &= ~TextFormatFlags.VerticalCenter;
					
					switch (text_alignment) {
						case ContentAlignment.TopLeft:
							text_format.Alignment=StringAlignment.Near;
							text_format.LineAlignment=StringAlignment.Near;
							break;

						case ContentAlignment.TopCenter:
							text_format.Alignment=StringAlignment.Center;
							text_format.LineAlignment=StringAlignment.Near;
							text_format_flags |= TextFormatFlags.HorizontalCenter;
							break;

						case ContentAlignment.TopRight:
							text_format.Alignment=StringAlignment.Far;
							text_format.LineAlignment=StringAlignment.Near;
							text_format_flags |= TextFormatFlags.Right;
							break;

						case ContentAlignment.MiddleLeft:
							text_format.Alignment=StringAlignment.Near;
							text_format.LineAlignment=StringAlignment.Center;
							text_format_flags |= TextFormatFlags.VerticalCenter;
							break;

						case ContentAlignment.MiddleCenter:
							text_format.Alignment=StringAlignment.Center;
							text_format.LineAlignment=StringAlignment.Center;
							text_format_flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
							break;

						case ContentAlignment.MiddleRight:
							text_format.Alignment=StringAlignment.Far;
							text_format.LineAlignment=StringAlignment.Center;
							text_format_flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
							break;

						case ContentAlignment.BottomLeft:
							text_format.Alignment=StringAlignment.Near;
							text_format.LineAlignment=StringAlignment.Far;
							text_format_flags |= TextFormatFlags.Bottom;
							break;

						case ContentAlignment.BottomCenter:
							text_format.Alignment=StringAlignment.Center;
							text_format.LineAlignment=StringAlignment.Far;
							text_format_flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom;
							break;

						case ContentAlignment.BottomRight:
							text_format.Alignment=StringAlignment.Far;
							text_format.LineAlignment=StringAlignment.Far;
							text_format_flags |= TextFormatFlags.Bottom | TextFormatFlags.Right;
							break;
					}
					
					Invalidate();
				}
			}
		}
		
		internal void resize ()
		{
			if (!AutoSize)
				return;
			m_helper.SizeToFit ();
		}
	}
}

