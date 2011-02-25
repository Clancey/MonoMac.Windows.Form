using System;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Drawing;
namespace System.Windows.Forms
{
	internal partial class TextBoxHelper : NSScrollView, IViewHelper
	{
		public Control Host { get; set; }
		internal TextBoxBase HostTb {
			get { return (TextBoxBase)Host; }
		}
		public NSCursor Cursor { get; set; }

		public TextView TextView;
		internal TextBoxHelper ()
		{
			this.AutohidesScrollers = true;
			this.BorderType = NSBorderType.BezelBorder;
			this.HasVerticalScroller = false;
			this.HasHorizontalScroller = true;
			TextView = new TextView ();
			TextView.Host = this;
			TextView.TextContainerInset = new SizeF (5f, 5f);
			TextView.AutoresizingMask = (NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable);
			TextView.TextContainer.ContainerSize = new SizeF (float.MaxValue, float.MaxValue);
			this.DocumentView = TextView;
			
			//TextView.EnclosingScrollView.HasHorizontalScroller = true;
		}

		public void UpdateTextView ()
		{
			TextView.VerticallyResizable = HostTb.Multiline;
			TextView.TextContainer.WidthTracksTextView = HostTb.WordWrap;
			TextView.HorizontallyResizable = !HostTb.WordWrap;
		}

		public string SelectedText {
			get { return TextView.Value.Substring (TextView.SelectedRange.Location, TextView.SelectedRange.Length); }
			set {
				var index = TextView.Value.IndexOf (value);
				if (index == -1)
					return;
				TextView.SelectedRange = new NSRange (index, value.Length);
			}
		}

		public override Drawing.RectangleF Frame {
			get { return base.Frame; }
			set {
				TextView.MinSize = value.Size;
				base.Frame = value;
			}
		}
		
	}

	internal class TextView : NSTextView
	{
		public TextBoxHelper Host;
		internal TextView ()
		{
			
		}

		public override void InsertText (NSObject insertString)
		{
			if (((TextBoxBase)Host.Host).Multiline || insertString.ToString () != "\n")
				base.InsertText (insertString);
		}
		
		public void SetSingleLine()
		{
			if(this.Value.Contains("\n"))
				this.Value = this.Value.Replace("\n","");	
		}
		
		public override string Value {
			get {
				return base.Value;
			}
			set {
				base.Value = value;
				
			if(!((TextBoxBase)Host.Host).Multiline)
				SetSingleLine();
			}
		}
		
		public override void KeyUp (NSEvent theEvent)
		{
			base.KeyUp (theEvent);
		}
		public override void KeyDown (NSEvent theEvent)
		{
			var keyEvent = new KeyPressEventArgs(theEvent.Characters.ToCharArray()[0]);
			Host.Host.onKeyPress(keyEvent);
			if(!keyEvent.Handled)
				base.KeyDown (theEvent);
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
	}
	
	
}

