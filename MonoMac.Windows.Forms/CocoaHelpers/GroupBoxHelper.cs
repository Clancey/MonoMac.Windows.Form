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
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Drawing;

#if MAC64
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
using CGFloat = System.Double;
#else
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
using NSPoint = System.Drawing.PointF;
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using CGFloat = System.Single;
#endif

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
				var insideFrame = (this.ContentView as FlippedView).Frame.Location;
				return new PointF((float)(insideFrame.X + this.ContentViewMargins.Width),
				                  (float)(insideFrame.Y+this.ContentViewMargins.Height));
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

