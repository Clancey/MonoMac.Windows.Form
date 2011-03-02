using System;
using MonoMac.AppKit;
using System.Drawing;
namespace System.Windows.Forms
{
	internal partial class ViewHelper : NSView, IViewHelper
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
		
		
		public override bool IsFlipped {
			get { return true; }
		}
		
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
			PointF point = this.ConvertPointFromView (theEvent.LocationInWindow, null);
			
			var button = (MouseButtons)theEvent.ButtonNumber;
			this.Host.FireMouseUp (Host, new MouseEventArgs (button, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			base.MouseUp (theEvent);
		}
		public override void MouseDown (NSEvent theEvent)
		{
			PointF point = this.ConvertPointFromView (theEvent.LocationInWindow, null);
			this.Host.FireMouseDown (Host, new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			base.MouseDown (theEvent);
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			PointF point = this.ConvertPointFromView (theEvent.LocationInWindow, null);
			this.Host.FireMouseMove (Host, new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			
			base.MouseDragged (theEvent);
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			PointF point = theEvent.LocationInWindow;
			this.Host.FireMouseMove (Host, new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));
			//base.MouseMoved (theEvent);
		}
	}
}

