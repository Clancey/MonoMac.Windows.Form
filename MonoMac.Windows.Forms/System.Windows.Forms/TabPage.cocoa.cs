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
using AppKit;
using System.Drawing;

namespace System.Windows.Forms
{
	public partial class TabPage : Panel
	{
		internal TabViewItemHelper m_helper;
		
		protected override void CreateHandle ()
		{
			m_helper = new TabViewItemHelper(this);
			m_view = m_helper.View;
		}
		
		public static implicit operator NSTabViewItem (TabPage tabPage)
		{
			return tabPage.m_helper;
		}
		internal override void PaintControlBackground (PaintEventArgs pevent)
		{
			if(m_helper.TabView.Selected != m_helper)
				return;
			base.PaintControlBackground(pevent);
		}
	}
}

