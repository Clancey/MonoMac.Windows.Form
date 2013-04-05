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
using System.Drawing;
namespace System.Windows.Forms
{
	internal partial class ViewHelper : FlippedView, IViewHelper
	{
		public ViewHelper (Control parent)
		{
			Host = parent;
			this.AutoresizingMask = (NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable);
		}
		
		public override void ViewDidMoveToWindow ()
		{
			base.ViewDidMoveToWindow ();
		}
		
		public Control Host {get;set;}
		public NSCursor Cursor {get;set;}
		
		public override void KeyDown (NSEvent theEvent)
		{
			base.KeyDown (theEvent);
		}
		public override void KeyUp (NSEvent theEvent)
		{
			base.KeyUp (theEvent);
		}
		private int lastKeyCount = 0;
		public override void FlagsChanged (NSEvent theEvent)
		{
			var theKey = (NSEventModifierMask)Enum.ToObject(typeof(NSEventModifierMask),(uint)theEvent.ModifierFlags  & 0xFFFF0000);
			
			int count = Util.NumberOfSetBits((int)theKey) ;
			//Console.WriteLine(count);
			if(theKey == 0 || lastKeyCount > count){
				Host.onKeyUp(new KeyEventArgs(theEvent));
				//Console.WriteLine("keyUp");
			}
			else {
				Host.onKeyDown(new KeyEventArgs(theEvent));
				//Console.WriteLine("keyDown");
			}
			lastKeyCount = count;
			base.FlagsChanged (theEvent);
		}
		
		public override void MouseUp (NSEvent theEvent)
		{
			var point = this.ConvertPointFromView (theEvent.LocationInWindow, null);
			
			var button = (MouseButtons)theEvent.ButtonNumber;
			this.Host.FireMouseUp (Host, new MouseEventArgs (button, (int)theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			base.MouseUp (theEvent);
		}
		public override void MouseDown (NSEvent theEvent)
		{
			var point = this.ConvertPointFromView (theEvent.LocationInWindow, null);
			this.Host.FireMouseDown (Host, new MouseEventArgs (MouseButtons.Left, (int)theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			base.MouseDown (theEvent);
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			var point = this.ConvertPointFromView (theEvent.LocationInWindow, null);
			this.Host.FireMouseMove (Host, new MouseEventArgs (MouseButtons.Left, (int)theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			
			base.MouseDragged (theEvent);
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			var point = theEvent.LocationInWindow;
			this.Host.FireMouseMove (Host, new MouseEventArgs (MouseButtons.Left, (int)theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			//base.MouseMoved (theEvent);
		}
	}
}

