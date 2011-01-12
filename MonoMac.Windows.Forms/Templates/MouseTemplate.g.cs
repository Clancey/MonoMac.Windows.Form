using System;
using System.Collections;
using System.Linq;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	public partial class ButtonHelper : NSButton , IViewHelper
	{
		public Control Host {get;set;}
		public NSCursor Cursor {get;set;}
		public override void ResetCursorRects ()
		{
			base.ResetCursorRects ();
			if(Cursor == null)
				Cursor = NSCursor.ArrowCursor;
			this.AddCursorRectcursor(this.Bounds,Cursor);
		}
		public event EventHandler viewDidMoveToSuperview;
		public override void ViewDidMoveToSuperview ()
		{
			base.ViewDidMoveToSuperview ();
			if(viewDidMoveToSuperview != null)
				viewDidMoveToSuperview(this,new EventArgs());
		}
		public override void MouseUp (NSEvent theEvent)
		{
			base.MouseUp (theEvent);
			FireMouseUp(theEvent);
		}		
		public virtual void FireMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseDown (NSEvent theEvent)
		{
			base.MouseDown (theEvent);
			FireMouseDown (theEvent);
		}
		public virtual void FireMouseDown(NSEvent theEvent)
		{
				
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			base.MouseMoved (theEvent);
			FireMouseMoved (theEvent);
		}
		public virtual void FireMouseMoved(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDown (NSEvent theEvent)
		{
			base.RightMouseDown (theEvent);
			FireRightMouseDown (theEvent);
		}
		public virtual void FireRightMouseDown(NSEvent theEvent)
		{
			
		}
		public override void RightMouseUp (NSEvent theEvent)
		{
			base.RightMouseUp (theEvent);
			FireRightMouseUp(theEvent);
		}
		public virtual void FireRightMouseUp(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDragged (NSEvent theEvent)
		{
			base.RightMouseDragged (theEvent);
			RightMouseDragged (theEvent);
		}
		public virtual void FireRightMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			base.MouseDragged (theEvent);
			FireMouseDragged (theEvent);
		}
		public virtual void FireMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseDown (NSEvent theEvent)
		{
			base.OtherMouseDown (theEvent);
			FireOtherMouseDown (theEvent);
		}
		public virtual void FireOtherMouseDown(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseUp (NSEvent theEvent)
		{
			base.OtherMouseUp (theEvent);
			FireOtherMouseUp (theEvent);
		}
		public virtual void FireOtherMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseEntered (NSEvent theEvent)
		{
			base.MouseEntered (theEvent);
			FireMouseEntered(theEvent);
		}
		public virtual void FireMouseEntered(NSEvent theEvent)
		{
	
		}
		public override void ScrollWheel (NSEvent theEvent)
		{
			base.ScrollWheel (theEvent);
			FireScrollWheel (theEvent);
		}
		public virtual void FireScrollWheel(NSEvent theEvent)
		{
			
		}
		public override void MouseExited (NSEvent theEvent)
		{
			base.MouseExited (theEvent);
			FireMouseExited (theEvent);
		}
		public virtual void FireMouseExited (NSEvent theEvent)
		{
			
		}
	}


	
	public partial class TextBoxMouseView : NSTextField , IViewHelper
	{
		public Control Host {get;set;}
		public NSCursor Cursor {get;set;}
		public override void ResetCursorRects ()
		{
			base.ResetCursorRects ();
			if(Cursor == null)
				Cursor = NSCursor.ArrowCursor;
			this.AddCursorRectcursor(this.Bounds,Cursor);
		}
		public event EventHandler viewDidMoveToSuperview;
		public override void ViewDidMoveToSuperview ()
		{
			base.ViewDidMoveToSuperview ();
			if(viewDidMoveToSuperview != null)
				viewDidMoveToSuperview(this,new EventArgs());
		}
		public override void MouseUp (NSEvent theEvent)
		{
			base.MouseUp (theEvent);
			FireMouseUp(theEvent);
		}		
		public virtual void FireMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseDown (NSEvent theEvent)
		{
			base.MouseDown (theEvent);
			FireMouseDown (theEvent);
		}
		public virtual void FireMouseDown(NSEvent theEvent)
		{
				
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			base.MouseMoved (theEvent);
			FireMouseMoved (theEvent);
		}
		public virtual void FireMouseMoved(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDown (NSEvent theEvent)
		{
			base.RightMouseDown (theEvent);
			FireRightMouseDown (theEvent);
		}
		public virtual void FireRightMouseDown(NSEvent theEvent)
		{
			
		}
		public override void RightMouseUp (NSEvent theEvent)
		{
			base.RightMouseUp (theEvent);
			FireRightMouseUp(theEvent);
		}
		public virtual void FireRightMouseUp(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDragged (NSEvent theEvent)
		{
			base.RightMouseDragged (theEvent);
			RightMouseDragged (theEvent);
		}
		public virtual void FireRightMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			base.MouseDragged (theEvent);
			FireMouseDragged (theEvent);
		}
		public virtual void FireMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseDown (NSEvent theEvent)
		{
			base.OtherMouseDown (theEvent);
			FireOtherMouseDown (theEvent);
		}
		public virtual void FireOtherMouseDown(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseUp (NSEvent theEvent)
		{
			base.OtherMouseUp (theEvent);
			FireOtherMouseUp (theEvent);
		}
		public virtual void FireOtherMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseEntered (NSEvent theEvent)
		{
			base.MouseEntered (theEvent);
			FireMouseEntered(theEvent);
		}
		public virtual void FireMouseEntered(NSEvent theEvent)
		{
	
		}
		public override void ScrollWheel (NSEvent theEvent)
		{
			base.ScrollWheel (theEvent);
			FireScrollWheel (theEvent);
		}
		public virtual void FireScrollWheel(NSEvent theEvent)
		{
			
		}
		public override void MouseExited (NSEvent theEvent)
		{
			base.MouseExited (theEvent);
			FireMouseExited (theEvent);
		}
		public virtual void FireMouseExited (NSEvent theEvent)
		{
			
		}
	}


	
	public partial class ComboBoxMouseView : NSComboBox , IViewHelper
	{
		public Control Host {get;set;}
		public NSCursor Cursor {get;set;}
		public override void ResetCursorRects ()
		{
			base.ResetCursorRects ();
			if(Cursor == null)
				Cursor = NSCursor.ArrowCursor;
			this.AddCursorRectcursor(this.Bounds,Cursor);
		}
		public event EventHandler viewDidMoveToSuperview;
		public override void ViewDidMoveToSuperview ()
		{
			base.ViewDidMoveToSuperview ();
			if(viewDidMoveToSuperview != null)
				viewDidMoveToSuperview(this,new EventArgs());
		}
		public override void MouseUp (NSEvent theEvent)
		{
			base.MouseUp (theEvent);
			FireMouseUp(theEvent);
		}		
		public virtual void FireMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseDown (NSEvent theEvent)
		{
			base.MouseDown (theEvent);
			FireMouseDown (theEvent);
		}
		public virtual void FireMouseDown(NSEvent theEvent)
		{
				
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			base.MouseMoved (theEvent);
			FireMouseMoved (theEvent);
		}
		public virtual void FireMouseMoved(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDown (NSEvent theEvent)
		{
			base.RightMouseDown (theEvent);
			FireRightMouseDown (theEvent);
		}
		public virtual void FireRightMouseDown(NSEvent theEvent)
		{
			
		}
		public override void RightMouseUp (NSEvent theEvent)
		{
			base.RightMouseUp (theEvent);
			FireRightMouseUp(theEvent);
		}
		public virtual void FireRightMouseUp(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDragged (NSEvent theEvent)
		{
			base.RightMouseDragged (theEvent);
			RightMouseDragged (theEvent);
		}
		public virtual void FireRightMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			base.MouseDragged (theEvent);
			FireMouseDragged (theEvent);
		}
		public virtual void FireMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseDown (NSEvent theEvent)
		{
			base.OtherMouseDown (theEvent);
			FireOtherMouseDown (theEvent);
		}
		public virtual void FireOtherMouseDown(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseUp (NSEvent theEvent)
		{
			base.OtherMouseUp (theEvent);
			FireOtherMouseUp (theEvent);
		}
		public virtual void FireOtherMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseEntered (NSEvent theEvent)
		{
			base.MouseEntered (theEvent);
			FireMouseEntered(theEvent);
		}
		public virtual void FireMouseEntered(NSEvent theEvent)
		{
	
		}
		public override void ScrollWheel (NSEvent theEvent)
		{
			base.ScrollWheel (theEvent);
			FireScrollWheel (theEvent);
		}
		public virtual void FireScrollWheel(NSEvent theEvent)
		{
			
		}
		public override void MouseExited (NSEvent theEvent)
		{
			base.MouseExited (theEvent);
			FireMouseExited (theEvent);
		}
		public virtual void FireMouseExited (NSEvent theEvent)
		{
			
		}
	}


	
	public partial class ListBoxMouseView : NSScrollView , IViewHelper
	{
		public Control Host {get;set;}
		public NSCursor Cursor {get;set;}
		public override void ResetCursorRects ()
		{
			base.ResetCursorRects ();
			if(Cursor == null)
				Cursor = NSCursor.ArrowCursor;
			this.AddCursorRectcursor(this.Bounds,Cursor);
		}
		public event EventHandler viewDidMoveToSuperview;
		public override void ViewDidMoveToSuperview ()
		{
			base.ViewDidMoveToSuperview ();
			if(viewDidMoveToSuperview != null)
				viewDidMoveToSuperview(this,new EventArgs());
		}
		public override void MouseUp (NSEvent theEvent)
		{
			base.MouseUp (theEvent);
			FireMouseUp(theEvent);
		}		
		public virtual void FireMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseDown (NSEvent theEvent)
		{
			base.MouseDown (theEvent);
			FireMouseDown (theEvent);
		}
		public virtual void FireMouseDown(NSEvent theEvent)
		{
				
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			base.MouseMoved (theEvent);
			FireMouseMoved (theEvent);
		}
		public virtual void FireMouseMoved(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDown (NSEvent theEvent)
		{
			base.RightMouseDown (theEvent);
			FireRightMouseDown (theEvent);
		}
		public virtual void FireRightMouseDown(NSEvent theEvent)
		{
			
		}
		public override void RightMouseUp (NSEvent theEvent)
		{
			base.RightMouseUp (theEvent);
			FireRightMouseUp(theEvent);
		}
		public virtual void FireRightMouseUp(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDragged (NSEvent theEvent)
		{
			base.RightMouseDragged (theEvent);
			RightMouseDragged (theEvent);
		}
		public virtual void FireRightMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			base.MouseDragged (theEvent);
			FireMouseDragged (theEvent);
		}
		public virtual void FireMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseDown (NSEvent theEvent)
		{
			base.OtherMouseDown (theEvent);
			FireOtherMouseDown (theEvent);
		}
		public virtual void FireOtherMouseDown(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseUp (NSEvent theEvent)
		{
			base.OtherMouseUp (theEvent);
			FireOtherMouseUp (theEvent);
		}
		public virtual void FireOtherMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseEntered (NSEvent theEvent)
		{
			base.MouseEntered (theEvent);
			FireMouseEntered(theEvent);
		}
		public virtual void FireMouseEntered(NSEvent theEvent)
		{
	
		}
		public override void ScrollWheel (NSEvent theEvent)
		{
			base.ScrollWheel (theEvent);
			FireScrollWheel (theEvent);
		}
		public virtual void FireScrollWheel(NSEvent theEvent)
		{
			
		}
		public override void MouseExited (NSEvent theEvent)
		{
			base.MouseExited (theEvent);
			FireMouseExited (theEvent);
		}
		public virtual void FireMouseExited (NSEvent theEvent)
		{
			
		}
	}


	
	public partial class TrackBarMouseView : NSSlider , IViewHelper
	{
		public Control Host {get;set;}
		public NSCursor Cursor {get;set;}
		public override void ResetCursorRects ()
		{
			base.ResetCursorRects ();
			if(Cursor == null)
				Cursor = NSCursor.ArrowCursor;
			this.AddCursorRectcursor(this.Bounds,Cursor);
		}
		public event EventHandler viewDidMoveToSuperview;
		public override void ViewDidMoveToSuperview ()
		{
			base.ViewDidMoveToSuperview ();
			if(viewDidMoveToSuperview != null)
				viewDidMoveToSuperview(this,new EventArgs());
		}
		public override void MouseUp (NSEvent theEvent)
		{
			base.MouseUp (theEvent);
			FireMouseUp(theEvent);
		}		
		public virtual void FireMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseDown (NSEvent theEvent)
		{
			base.MouseDown (theEvent);
			FireMouseDown (theEvent);
		}
		public virtual void FireMouseDown(NSEvent theEvent)
		{
				
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			base.MouseMoved (theEvent);
			FireMouseMoved (theEvent);
		}
		public virtual void FireMouseMoved(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDown (NSEvent theEvent)
		{
			base.RightMouseDown (theEvent);
			FireRightMouseDown (theEvent);
		}
		public virtual void FireRightMouseDown(NSEvent theEvent)
		{
			
		}
		public override void RightMouseUp (NSEvent theEvent)
		{
			base.RightMouseUp (theEvent);
			FireRightMouseUp(theEvent);
		}
		public virtual void FireRightMouseUp(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDragged (NSEvent theEvent)
		{
			base.RightMouseDragged (theEvent);
			RightMouseDragged (theEvent);
		}
		public virtual void FireRightMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			base.MouseDragged (theEvent);
			FireMouseDragged (theEvent);
		}
		public virtual void FireMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseDown (NSEvent theEvent)
		{
			base.OtherMouseDown (theEvent);
			FireOtherMouseDown (theEvent);
		}
		public virtual void FireOtherMouseDown(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseUp (NSEvent theEvent)
		{
			base.OtherMouseUp (theEvent);
			FireOtherMouseUp (theEvent);
		}
		public virtual void FireOtherMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseEntered (NSEvent theEvent)
		{
			base.MouseEntered (theEvent);
			FireMouseEntered(theEvent);
		}
		public virtual void FireMouseEntered(NSEvent theEvent)
		{
	
		}
		public override void ScrollWheel (NSEvent theEvent)
		{
			base.ScrollWheel (theEvent);
			FireScrollWheel (theEvent);
		}
		public virtual void FireScrollWheel(NSEvent theEvent)
		{
			
		}
		public override void MouseExited (NSEvent theEvent)
		{
			base.MouseExited (theEvent);
			FireMouseExited (theEvent);
		}
		public virtual void FireMouseExited (NSEvent theEvent)
		{
			
		}
	}


	
	public partial class UserControlMouseView : NSControl , IViewHelper
	{
		public Control Host {get;set;}
		public NSCursor Cursor {get;set;}
		public override void ResetCursorRects ()
		{
			base.ResetCursorRects ();
			if(Cursor == null)
				Cursor = NSCursor.ArrowCursor;
			this.AddCursorRectcursor(this.Bounds,Cursor);
		}
		public event EventHandler viewDidMoveToSuperview;
		public override void ViewDidMoveToSuperview ()
		{
			base.ViewDidMoveToSuperview ();
			if(viewDidMoveToSuperview != null)
				viewDidMoveToSuperview(this,new EventArgs());
		}
		public override void MouseUp (NSEvent theEvent)
		{
			base.MouseUp (theEvent);
			FireMouseUp(theEvent);
		}		
		public virtual void FireMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseDown (NSEvent theEvent)
		{
			base.MouseDown (theEvent);
			FireMouseDown (theEvent);
		}
		public virtual void FireMouseDown(NSEvent theEvent)
		{
				
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			base.MouseMoved (theEvent);
			FireMouseMoved (theEvent);
		}
		public virtual void FireMouseMoved(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDown (NSEvent theEvent)
		{
			base.RightMouseDown (theEvent);
			FireRightMouseDown (theEvent);
		}
		public virtual void FireRightMouseDown(NSEvent theEvent)
		{
			
		}
		public override void RightMouseUp (NSEvent theEvent)
		{
			base.RightMouseUp (theEvent);
			FireRightMouseUp(theEvent);
		}
		public virtual void FireRightMouseUp(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDragged (NSEvent theEvent)
		{
			base.RightMouseDragged (theEvent);
			RightMouseDragged (theEvent);
		}
		public virtual void FireRightMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			base.MouseDragged (theEvent);
			FireMouseDragged (theEvent);
		}
		public virtual void FireMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseDown (NSEvent theEvent)
		{
			base.OtherMouseDown (theEvent);
			FireOtherMouseDown (theEvent);
		}
		public virtual void FireOtherMouseDown(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseUp (NSEvent theEvent)
		{
			base.OtherMouseUp (theEvent);
			FireOtherMouseUp (theEvent);
		}
		public virtual void FireOtherMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseEntered (NSEvent theEvent)
		{
			base.MouseEntered (theEvent);
			FireMouseEntered(theEvent);
		}
		public virtual void FireMouseEntered(NSEvent theEvent)
		{
	
		}
		public override void ScrollWheel (NSEvent theEvent)
		{
			base.ScrollWheel (theEvent);
			FireScrollWheel (theEvent);
		}
		public virtual void FireScrollWheel(NSEvent theEvent)
		{
			
		}
		public override void MouseExited (NSEvent theEvent)
		{
			base.MouseExited (theEvent);
			FireMouseExited (theEvent);
		}
		public virtual void FireMouseExited (NSEvent theEvent)
		{
			
		}
	}


	
	public partial class PanelMouseView : NSView , IViewHelper
	{
		public Control Host {get;set;}
		public NSCursor Cursor {get;set;}
		public override RectangleF Frame {
			get {
				return base.Frame;
			}
			set {
				this.RemoveTrackingArea((Host as Panel).trackingArea);
				(Host as Panel).trackingArea = new NSTrackingArea(value,(NSTrackingAreaOptions.MouseEnteredAndExited |
			                                                             NSTrackingAreaOptions.MouseMoved |
			                                                             NSTrackingAreaOptions.ActiveInKeyWindow), this,new NSDictionary());
				this.AddTrackingArea((Host as Panel).trackingArea);
				this.UpdateTrackingAreas();
				base.Frame = value;
			}
		}
		public override void ResetCursorRects ()
		{
			base.ResetCursorRects ();
			if(Cursor == null)
				Cursor = NSCursor.ArrowCursor;
			this.AddCursorRectcursor(this.Bounds,Cursor);
		}
		public event EventHandler viewDidMoveToSuperview;
		public override void ViewDidMoveToSuperview ()
		{
			base.ViewDidMoveToSuperview ();
			if(viewDidMoveToSuperview != null)
				viewDidMoveToSuperview(this,new EventArgs());
		}
		public override void MouseUp (NSEvent theEvent)
		{
			base.MouseUp (theEvent);
			FireMouseUp(theEvent);
		}		
		public virtual void FireMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseDown (NSEvent theEvent)
		{
			base.MouseDown (theEvent);
			FireMouseDown (theEvent);
		}
		public virtual void FireMouseDown(NSEvent theEvent)
		{
				
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			PointF point = theEvent.LocationInWindow;
			base.MouseMoved (theEvent);
			this.Host.FireMouseMove (Host, new MouseEventArgs (MouseButtons.Left, theEvent.ClickCount, (int)point.X, (int)point.Y, 0));

		}
		public virtual void FireMouseMoved(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDown (NSEvent theEvent)
		{
			base.RightMouseDown (theEvent);
			FireRightMouseDown (theEvent);
		}
		public virtual void FireRightMouseDown(NSEvent theEvent)
		{
			
		}
		public override void RightMouseUp (NSEvent theEvent)
		{
			base.RightMouseUp (theEvent);
			FireRightMouseUp(theEvent);
		}
		public virtual void FireRightMouseUp(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDragged (NSEvent theEvent)
		{
			base.RightMouseDragged (theEvent);
			RightMouseDragged (theEvent);
		}
		public virtual void FireRightMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			base.MouseDragged (theEvent);
			FireMouseDragged (theEvent);
		}
		public virtual void FireMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseDown (NSEvent theEvent)
		{
			base.OtherMouseDown (theEvent);
			FireOtherMouseDown (theEvent);
		}
		public virtual void FireOtherMouseDown(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseUp (NSEvent theEvent)
		{
			base.OtherMouseUp (theEvent);
			FireOtherMouseUp (theEvent);
		}
		public virtual void FireOtherMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseEntered (NSEvent theEvent)
		{
			base.MouseEntered (theEvent);
			FireMouseEntered(theEvent);
		}
		public virtual void FireMouseEntered(NSEvent theEvent)
		{
	
		}
		public override void ScrollWheel (NSEvent theEvent)
		{
			base.ScrollWheel (theEvent);
			FireScrollWheel (theEvent);
		}
		public virtual void FireScrollWheel(NSEvent theEvent)
		{
			
		}
		public override void MouseExited (NSEvent theEvent)
		{
			base.MouseExited (theEvent);
			FireMouseExited (theEvent);
		}
		public virtual void FireMouseExited (NSEvent theEvent)
		{
			
		}
	}


	
}


