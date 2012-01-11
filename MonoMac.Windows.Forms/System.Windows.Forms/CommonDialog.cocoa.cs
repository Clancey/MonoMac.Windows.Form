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
//    limitations under the License.
using System;
namespace System.Windows.Forms
{
	public partial class CommonDialog
	{
		internal FileDialogHelper m_helper;
		
		protected virtual void CreateHandle ()
		{
			m_helper = new FileDialogHelper (this);
			//m_view = m_helper;
			m_helper.Title = "Browse For Folder";
			//m_helper.
		}
		
		public DialogResult DialogResult {get;set;}
		
		
		public DialogResult ShowDialog (IWin32Window owner)
		{
			//TODO:
			m_helper.Show();
			return DialogResult;
		}
	}
}

