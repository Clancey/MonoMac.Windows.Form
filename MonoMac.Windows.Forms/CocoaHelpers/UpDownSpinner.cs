using System;
using System.Drawing;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	internal sealed class UpDownSpinner : Control
	{
		#region Local Variables
		private UpDownBase owner;
		internal UpDownSpinnerHelper m_helper;
		#endregion
		
		#region Constructors
		public UpDownSpinner (UpDownBase owner)
		{
			this.owner = owner;	
		}
		#endregion
		
		protected override void CreateHandle ()
		{	
      		m_helper = new UpDownSpinnerHelper();
			m_view =  m_helper;
			m_helper.Host = this;
			m_helper.MinValue = -1;
			m_helper.MaxValue = 1;
			m_helper.IntValue = 0;
			m_helper.Increment = 1;			
			m_helper.Activated += delegate(object sender, EventArgs e) {
				if(m_helper.IntValue == 1)
					owner.UpButton();
				else if (m_helper.IntValue == -1)
					owner.DownButton();
				m_helper.IntValue = 0;
			};
		}
	}
}

