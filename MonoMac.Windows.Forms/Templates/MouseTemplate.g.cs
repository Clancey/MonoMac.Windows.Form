using System;
using System.Collections;
using System.Linq;
using AppKit;
using System.Drawing;
using Foundation;
namespace System.Windows.Forms
{
	internal partial class ButtonHelper : NSButton , IViewHelper
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
			if(theEvent == null)
				return;
			base.MouseUp (theEvent);
			FireMouseUp(theEvent);
		}		
		public virtual void FireMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseDown (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseDown (theEvent);
			FireMouseDown (theEvent);
		}
		public virtual void FireMouseDown(NSEvent theEvent)
		{
				
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
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
			if(theEvent == null)
				return;
			base.RightMouseUp (theEvent);
			FireRightMouseUp(theEvent);
		}
		public virtual void FireRightMouseUp(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDragged (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.RightMouseDragged (theEvent);
			RightMouseDragged (theEvent);
		}
		public virtual void FireRightMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseDragged (theEvent);
			FireMouseDragged (theEvent);
		}
		public virtual void FireMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseDown (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.OtherMouseDown (theEvent);
			FireOtherMouseDown (theEvent);
		}
		public virtual void FireOtherMouseDown(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseUp (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.OtherMouseUp (theEvent);
			FireOtherMouseUp (theEvent);
		}
		public virtual void FireOtherMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseEntered (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseEntered (theEvent);
			FireMouseEntered(theEvent);
		}
		public virtual void FireMouseEntered(NSEvent theEvent)
		{
	
		}
		public override void ScrollWheel (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.ScrollWheel (theEvent);
			FireScrollWheel (theEvent);
		}
		public virtual void FireScrollWheel(NSEvent theEvent)
		{
			
		}
		public override void MouseExited (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseExited (theEvent);
			FireMouseExited (theEvent);
		}
		public virtual void FireMouseExited (NSEvent theEvent)
		{
			
		}
	}


	
	internal partial class TextBoxMouseView : NSTextView , IViewHelper
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
			if(theEvent == null)
				return;
			base.MouseUp (theEvent);
			FireMouseUp(theEvent);
		}		
		public virtual void FireMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseDown (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseDown (theEvent);
			FireMouseDown (theEvent);
		}
		public virtual void FireMouseDown(NSEvent theEvent)
		{
				
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
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
			if(theEvent == null)
				return;
			base.RightMouseUp (theEvent);
			FireRightMouseUp(theEvent);
		}
		public virtual void FireRightMouseUp(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDragged (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.RightMouseDragged (theEvent);
			RightMouseDragged (theEvent);
		}
		public virtual void FireRightMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseDragged (theEvent);
			FireMouseDragged (theEvent);
		}
		public virtual void FireMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseDown (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.OtherMouseDown (theEvent);
			FireOtherMouseDown (theEvent);
		}
		public virtual void FireOtherMouseDown(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseUp (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.OtherMouseUp (theEvent);
			FireOtherMouseUp (theEvent);
		}
		public virtual void FireOtherMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseEntered (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseEntered (theEvent);
			FireMouseEntered(theEvent);
		}
		public virtual void FireMouseEntered(NSEvent theEvent)
		{
	
		}
		public override void ScrollWheel (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.ScrollWheel (theEvent);
			FireScrollWheel (theEvent);
		}
		public virtual void FireScrollWheel(NSEvent theEvent)
		{
			
		}
		public override void MouseExited (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseExited (theEvent);
			FireMouseExited (theEvent);
		}
		public virtual void FireMouseExited (NSEvent theEvent)
		{
			
		}
	}

	
	internal partial class ListBoxMouseView : NSScrollView , IViewHelper
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
			if(theEvent == null)
				return;
			base.MouseUp (theEvent);
			FireMouseUp(theEvent);
		}		
		public virtual void FireMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseDown (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseDown (theEvent);
			FireMouseDown (theEvent);
		}
		public virtual void FireMouseDown(NSEvent theEvent)
		{
				
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
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
			if(theEvent == null)
				return;
			base.RightMouseUp (theEvent);
			FireRightMouseUp(theEvent);
		}
		public virtual void FireRightMouseUp(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDragged (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.RightMouseDragged (theEvent);
			RightMouseDragged (theEvent);
		}
		public virtual void FireRightMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseDragged (theEvent);
			FireMouseDragged (theEvent);
		}
		public virtual void FireMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseDown (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.OtherMouseDown (theEvent);
			FireOtherMouseDown (theEvent);
		}
		public virtual void FireOtherMouseDown(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseUp (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.OtherMouseUp (theEvent);
			FireOtherMouseUp (theEvent);
		}
		public virtual void FireOtherMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseEntered (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseEntered (theEvent);
			FireMouseEntered(theEvent);
		}
		public virtual void FireMouseEntered(NSEvent theEvent)
		{
	
		}
		public override void ScrollWheel (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.ScrollWheel (theEvent);
			FireScrollWheel (theEvent);
		}
		public virtual void FireScrollWheel(NSEvent theEvent)
		{
			
		}
		public override void MouseExited (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseExited (theEvent);
			FireMouseExited (theEvent);
		}
		public virtual void FireMouseExited (NSEvent theEvent)
		{
			
		}
	}


	
	internal partial class TrackBarMouseView : NSSlider , IViewHelper
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
			if(theEvent == null)
				return;
			base.MouseUp (theEvent);
			FireMouseUp(theEvent);
		}		
		public virtual void FireMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseDown (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseDown (theEvent);
			FireMouseDown (theEvent);
		}
		public virtual void FireMouseDown(NSEvent theEvent)
		{
				
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
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
			if(theEvent == null)
				return;
			base.RightMouseUp (theEvent);
			FireRightMouseUp(theEvent);
		}
		public virtual void FireRightMouseUp(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDragged (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.RightMouseDragged (theEvent);
			RightMouseDragged (theEvent);
		}
		public virtual void FireRightMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseDragged (theEvent);
			FireMouseDragged (theEvent);
		}
		public virtual void FireMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseDown (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.OtherMouseDown (theEvent);
			FireOtherMouseDown (theEvent);
		}
		public virtual void FireOtherMouseDown(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseUp (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.OtherMouseUp (theEvent);
			FireOtherMouseUp (theEvent);
		}
		public virtual void FireOtherMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseEntered (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseEntered (theEvent);
			FireMouseEntered(theEvent);
		}
		public virtual void FireMouseEntered(NSEvent theEvent)
		{
	
		}
		public override void ScrollWheel (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.ScrollWheel (theEvent);
			FireScrollWheel (theEvent);
		}
		public virtual void FireScrollWheel(NSEvent theEvent)
		{
			
		}
		public override void MouseExited (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseExited (theEvent);
			FireMouseExited (theEvent);
		}
		public virtual void FireMouseExited (NSEvent theEvent)
		{
			
		}
	}


	
	internal partial class UserControlMouseView : NSControl , IViewHelper
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
			if(theEvent == null)
				return;
			base.MouseUp (theEvent);
			FireMouseUp(theEvent);
		}		
		public virtual void FireMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseDown (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseDown (theEvent);
			FireMouseDown (theEvent);
		}
		public virtual void FireMouseDown(NSEvent theEvent)
		{
				
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
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
			if(theEvent == null)
				return;
			base.RightMouseUp (theEvent);
			FireRightMouseUp(theEvent);
		}
		public virtual void FireRightMouseUp(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDragged (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.RightMouseDragged (theEvent);
			RightMouseDragged (theEvent);
		}
		public virtual void FireRightMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseDragged (theEvent);
			FireMouseDragged (theEvent);
		}
		public virtual void FireMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseDown (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.OtherMouseDown (theEvent);
			FireOtherMouseDown (theEvent);
		}
		public virtual void FireOtherMouseDown(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseUp (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.OtherMouseUp (theEvent);
			FireOtherMouseUp (theEvent);
		}
		public virtual void FireOtherMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseEntered (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseEntered (theEvent);
			FireMouseEntered(theEvent);
		}
		public virtual void FireMouseEntered(NSEvent theEvent)
		{
	
		}
		public override void ScrollWheel (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.ScrollWheel (theEvent);
			FireScrollWheel (theEvent);
		}
		public virtual void FireScrollWheel(NSEvent theEvent)
		{
			
		}
		public override void MouseExited (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseExited (theEvent);
			FireMouseExited (theEvent);
		}
		public virtual void FireMouseExited (NSEvent theEvent)
		{
			
		}
	}


	
	internal partial class PanelMouseView : NSView , IViewHelper
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
			if(theEvent == null)
				return;
			base.MouseUp (theEvent);
			FireMouseUp(theEvent);
		}		
		public virtual void FireMouseUp(NSEvent theEvent)
		{
			
		}
		public virtual void FireMouseDown(NSEvent theEvent)
		{
				
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
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
			if(theEvent == null)
				return;
			base.RightMouseUp (theEvent);
			FireRightMouseUp(theEvent);
		}
		public virtual void FireRightMouseUp(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDragged (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.RightMouseDragged (theEvent);
			RightMouseDragged (theEvent);
		}
		public virtual void FireRightMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseDragged (theEvent);
			FireMouseDragged (theEvent);
		}
		public virtual void FireMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseDown (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.OtherMouseDown (theEvent);
			FireOtherMouseDown (theEvent);
		}
		public virtual void FireOtherMouseDown(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseUp (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.OtherMouseUp (theEvent);
			FireOtherMouseUp (theEvent);
		}
		public virtual void FireOtherMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseEntered (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseEntered (theEvent);
			FireMouseEntered(theEvent);
		}
		public virtual void FireMouseEntered(NSEvent theEvent)
		{
	
		}
		public override void ScrollWheel (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.ScrollWheel (theEvent);
			FireScrollWheel (theEvent);
		}
		public virtual void FireScrollWheel(NSEvent theEvent)
		{
			
		}
		public override void MouseExited (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseExited (theEvent);
			FireMouseExited (theEvent);
		}
		public virtual void FireMouseExited (NSEvent theEvent)
		{
			
		}
	}


	
	internal partial class TableViewHelper : NSTableView , IViewHelper
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
			if(theEvent == null)
				return;
			base.MouseUp (theEvent);
			FireMouseUp(theEvent);
		}		
		public virtual void FireMouseUp(NSEvent theEvent)
		{
			
		}
		public virtual void FireMouseDown(NSEvent theEvent)
		{
				
		}
		public override void MouseMoved (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
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
			if(theEvent == null)
				return;
			base.RightMouseUp (theEvent);
			FireRightMouseUp(theEvent);
		}
		public virtual void FireRightMouseUp(NSEvent theEvent)
		{
			
		}
		public override void RightMouseDragged (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.RightMouseDragged (theEvent);
			RightMouseDragged (theEvent);
		}
		public virtual void FireRightMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void MouseDragged (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseDragged (theEvent);
			FireMouseDragged (theEvent);
		}
		public virtual void FireMouseDragged(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseDown (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.OtherMouseDown (theEvent);
			FireOtherMouseDown (theEvent);
		}
		public virtual void FireOtherMouseDown(NSEvent theEvent)
		{
			
		}
		public override void OtherMouseUp (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.OtherMouseUp (theEvent);
			FireOtherMouseUp (theEvent);
		}
		public virtual void FireOtherMouseUp(NSEvent theEvent)
		{
			
		}
		public override void MouseEntered (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseEntered (theEvent);
			FireMouseEntered(theEvent);
		}
		public virtual void FireMouseEntered(NSEvent theEvent)
		{
	
		}
		public override void ScrollWheel (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.ScrollWheel (theEvent);
			FireScrollWheel (theEvent);
		}
		public virtual void FireScrollWheel(NSEvent theEvent)
		{
			
		}
		public override void MouseExited (NSEvent theEvent)
		{
			if(theEvent == null)
				return;
			base.MouseExited (theEvent);
			FireMouseExited (theEvent);
		}
		public virtual void FireMouseExited (NSEvent theEvent)
		{
			
		}
		#region IViewHelper implementation
		public void FontChanged ()
		{
			throw new System.NotImplementedException();
		}
		
		#endregion
	}


	
}


