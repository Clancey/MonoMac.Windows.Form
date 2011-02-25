using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public static  class MiscExtenders
	{
        public static Keys ToKeys(this NSEventModifierMask mask)
        {
			switch(mask){
			//case NSEventModifierMask.AlphaShiftKeyMask:
			//	return Keys.None;
			case NSEventModifierMask.AlternateKeyMask:
				return Keys.Alt;
			case NSEventModifierMask.CommandKeyMask:
				return Keys.LWin | Keys.RWin;
			case NSEventModifierMask.ControlKeyMask:
				return Keys.Control;				
			//case NSEventModifierMask.FunctionKeyMask:
			//	return Keys.None;
			case NSEventModifierMask.HelpKeyMask:
				return Keys.Help;
			case NSEventModifierMask.ShiftKeyMask:
				return Keys.Shift;
			}
			return Keys.None;
        }
	}
}

