// 
//  Copyright 2011  James Clancey
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.using System;
using System.Drawing;
using AppKit;
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

