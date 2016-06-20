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
namespace System.Windows.Forms
{
	public static class NSViewExtender
	{
		public static void BringSubviewToFront(this NSView superView, NSView theView)
		{
			if(theView == null || !superView.Subviews.Contains(theView))
				return;
			theView.RemoveFromSuperview();
			superView.AddSubview(theView);
		}
		
		public static void BringToFront(this NSView theView)
		{
			if(theView.Superview == null)
				return;
			theView.Superview.BringSubviewToFront(theView);
		}
	}
}

