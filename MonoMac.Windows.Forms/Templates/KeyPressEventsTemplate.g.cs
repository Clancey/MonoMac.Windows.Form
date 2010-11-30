using System;
using System.Collections;
using System.Linq;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	public partial class TextBox 
	{	
		public override void KeyDown (NSEvent theEvent)
		{
			base.KeyDown (theEvent);
			if(OnKeyDown != null)
				OnKeyDown(this,new KeyEventArgs(theEvent));
		}
		public override void KeyUp (NSEvent theEvent)
		{
			base.KeyUp (theEvent);
            if (OnKeyUp != null)
                OnKeyUp(this, new KeyEventArgs(theEvent));
			if(OnKeyPress != null)
				OnKeyPress(this, new KeyPressEventArgs(theEvent.Characters.ToCharArray()[0]));
		}
		
	    public KeyPressEventHandler OnKeyPress { get; set; }
        public KeyEventHandler OnKeyUp { get; set; }
        public KeyEventHandler OnKeyDown { get; set; }
	}
	
	
	public partial class ComboBox 
	{	
		public override void KeyDown (NSEvent theEvent)
		{
			base.KeyDown (theEvent);
			if(OnKeyDown != null)
				OnKeyDown(this,new KeyEventArgs(theEvent));
		}
		public override void KeyUp (NSEvent theEvent)
		{
			base.KeyUp (theEvent);
            if (OnKeyUp != null)
                OnKeyUp(this, new KeyEventArgs(theEvent));
			if(OnKeyPress != null)
				OnKeyPress(this, new KeyPressEventArgs(theEvent.Characters.ToCharArray()[0]));
		}
		
	    public KeyPressEventHandler OnKeyPress { get; set; }
        public KeyEventHandler OnKeyUp { get; set; }
        public KeyEventHandler OnKeyDown { get; set; }
	}
	
	
}


