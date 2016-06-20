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
	internal class GroupBoxHelper : NSBox, IViewHelper
	{
		public GroupBoxHelper (Control parent)
		{
			Host = parent;
			this.ContentView = new FlippedView();
		}
		
		public PointF OffSet
		{
			get {
				var parentFram = this.Frame;
				var insideFrame = (this.ContentView as FlippedView).Frame.Location.Add(this.ContentViewMargins);
				return insideFrame;
				
			}
		}

		#region IViewHelper implementation
		public NSCursor Cursor { get; set; }

		public Control Host { get; set; }
		#endregion
		#region IViewHelper implementation
		public void FontChanged ()
		{
			this.TitleFont = Host.Font.ToNsFont();
		}
		
		#endregion
	}
}

