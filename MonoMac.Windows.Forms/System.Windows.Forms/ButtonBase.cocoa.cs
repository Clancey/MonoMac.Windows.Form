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
		
		internal void resize ()
		{
			if (!AutoSize)
				return;
			m_helper.SizeToFit ();
		}
	}
}

