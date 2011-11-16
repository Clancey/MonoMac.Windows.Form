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
			if (!((TextBoxBase)Host.Host).Multiline && insertString.ToString () == "\n")
				return ;
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
			var chars = theEvent.Characters.ToCharArray();
			var thekey = chars[0];
			var keyEvent = new KeyPressEventArgs(thekey);
			if(thekey != (char)NSKey.LeftArrow 
			   && thekey != (char)NSKey.RightArrow 
			   && thekey != (char)NSKey.UpArrow 
			   && thekey != (char)NSKey.DownArrow
			   && thekey != (char)NSKey.Tab
			   && thekey != (char)NSKey.Insert
			   && thekey != (char)NSKey.Delete
			   && thekey != (char)NSKey.ForwardDelete
			   && char.IsControl(thekey)
			   && thekey != (char)NSKey.Home
			   && thekey != (char)NSKey.End
			   && thekey != (char)NSKey.PageUp
			   && thekey != (char)NSKey.PageDown
			   && thekey != (char)NSKey.F1
			   && thekey != (char)NSKey.F2
			   && thekey != (char)NSKey.Option) {
				Host.Host.onKeyPress(keyEvent);
				if(!keyEvent.Handled)
					base.KeyDown (theEvent);
			}
			else
				base.KeyDown(theEvent);
		}
		
		private int lastKeyCount = 0;
		public override void FlagsChanged (NSEvent theEvent)
		{
			var theKey = (NSEventModifierMask)Enum.ToObject(typeof(NSEventModifierMask),(uint)theEvent.ModifierFlags  & 0xFFFF0000);
			
			int count = Util.NumberOfSetBits((int)theKey) ;
			//Console.WriteLine(count);
			if(theKey == 0 || lastKeyCount > count){
			   Host.Host.onKeyUp(new KeyEventArgs(theEvent));
				//Console.WriteLine("keyUp");
			}
			else {
				Host.Host.onKeyDown(new KeyEventArgs(theEvent));
				//Console.WriteLine("keyDown");
			}
			lastKeyCount = count;
			base.FlagsChanged (theEvent);
		}
	}
	
	
}

