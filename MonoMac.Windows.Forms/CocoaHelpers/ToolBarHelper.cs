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
using System.Linq;
using System.Collections.Generic;
using System.Collections;
namespace System.Windows.Forms
{
	public class ToolBarHelper : NSMenu, IViewHelper
	{
		public ToolStrip ToolStrip;
		public ToolBarHelper(ToolStrip parent)
		{
			Host = parent;
			
			//this.AllowsUserCustomization = true;
			//this.DisplayMode = NSToolbarDisplayMode.IconAndLabel;
		}
		
	

		#region IViewHelper implementation
		public NSCursor Cursor {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public Control Host {
			get {return ToolStrip;}
			set {ToolStrip = (ToolStrip)value;	}
		}
		#endregion
		#region IViewHelper implementation
		public void FontChanged ()
		{
			
		}
		
		#endregion
}
}

