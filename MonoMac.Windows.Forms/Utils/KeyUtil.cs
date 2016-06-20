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
using System.Linq;
using AppKit;
using System.Collections;
namespace System.Windows.Forms
{
	public static class KeyUtil
	{
		private static IDictionary keyNames;
		private static IDictionary modifiers;
		private static bool initialized;

		private static void Initialize ()
		{
			if (initialized)
				return;
			initialized = true;
			keyNames = new Hashtable ();
			modifiers = new Hashtable ();
			
			keyNames.Add (NSKey.Backslash, Keys.OemBackslash);
			keyNames.Add (NSKey.CapsLock, Keys.Capital);
			keyNames.Add (NSKey.Comma, Keys.Oemcomma);
			keyNames.Add (NSKey.Command, Keys.LWin);
			keyNames.Add (NSKey.Delete, Keys.Back);
			keyNames.Add (NSKey.DownArrow, Keys.Down);
			keyNames.Add (NSKey.Equal, Keys.Oemplus);
			keyNames.Add (NSKey.ForwardDelete, NSKey.Delete);
			keyNames.Add (NSKey.Keypad0, Keys.D0);
			keyNames.Add (NSKey.Keypad1, Keys.D1);
			keyNames.Add (NSKey.Keypad2, Keys.D2);
			keyNames.Add (NSKey.Keypad3, Keys.D3);
			keyNames.Add (NSKey.Keypad4, Keys.D4);
			keyNames.Add (NSKey.Keypad5, Keys.D5);
			keyNames.Add (NSKey.Keypad6, Keys.D6);
			keyNames.Add (NSKey.Keypad7, Keys.D7);
			keyNames.Add (NSKey.Keypad8, Keys.D8);
			keyNames.Add (NSKey.Keypad9, Keys.D9);
			keyNames.Add (NSKey.KeypadDecimal, Keys.Decimal);
			keyNames.Add (NSKey.KeypadDivide, Keys.Divide);
			keyNames.Add (NSKey.KeypadEnter, Keys.Enter);
			keyNames.Add (NSKey.KeypadEquals, Keys.Oemplus);
			keyNames.Add (NSKey.KeypadMinus, Keys.OemMinus);
			keyNames.Add (NSKey.KeypadMultiply, Keys.Multiply);
			keyNames.Add (NSKey.KeypadPlus, Keys.Oemplus | Keys.Shift);
			keyNames.Add (NSKey.LeftArrow, Keys.Left);
			keyNames.Add (NSKey.LeftBracket, Keys.OemOpenBrackets);
			keyNames.Add (NSKey.Minus, Keys.OemMinus);
			keyNames.Add (NSKey.Mute, Keys.VolumeMute);
			keyNames.Add (NSKey.Next, Keys.MediaNextTrack);
			keyNames.Add (NSKey.Option, Keys.Alt);
			keyNames.Add (NSKey.Pause, Keys.MediaPlayPause);
			keyNames.Add (NSKey.Prev, Keys.MediaPreviousTrack);
			keyNames.Add (NSKey.Quote, Keys.OemQuotes);
			keyNames.Add (NSKey.RightArrow, Keys.Right);
			keyNames.Add (NSKey.RightBracket, Keys.OemCloseBrackets);
			keyNames.Add (NSKey.RightControl, Keys.RControlKey);
			keyNames.Add (NSKey.RightOption, Keys.Alt);
			keyNames.Add (NSKey.RightShift, Keys.RShiftKey);
			keyNames.Add (NSKey.ScrollLock, Keys.Scroll);
			keyNames.Add (NSKey.Semicolon, Keys.OemSemicolon);
			keyNames.Add (NSKey.Slash, Keys.OemQuestion);
			keyNames.Add (NSKey.UpArrow, Keys.Up);
			
			// Modifiers
			modifiers.Add ("524576", Keys.Alt);
			modifiers.Add ("65792", Keys.CapsLock);
			modifiers.Add ("524608", Keys.LWin);
			modifiers.Add ("262401", Keys.ControlKey);
			modifiers.Add ("131332", Keys.RShiftKey | Keys.Shift);
			modifiers.Add ("131330", Keys.LShiftKey | Keys.Shift);
			modifiers.Add ("655650", Keys.Shift | Keys.Alt);
		}
			/*
			keyNames.Add("+",Keys.Add);
			keyNames.Add("524576",Keys.Alt);
			keyNames.Add("65792",Keys.CapsLock);
			keyNames.Add("0", Keys.D0);
            keyNames.Add("1", Keys.D1);
            keyNames.Add("2", Keys.D2);
            keyNames.Add("3", Keys.D3);
            keyNames.Add("4", Keys.D4);
            keyNames.Add("5", Keys.D5);
            keyNames.Add("6", Keys.D6);
            keyNames.Add("7", Keys.D7);
            keyNames.Add("8", Keys.D8);
            keyNames.Add("9", Keys.D9);
			keyNames.Add("256",Keys.Back);
			keyNames.Add("." , Keys.Decimal);
			keyNames.Add(NSKey.Delete,Keys.Delete);			
			keyNames.Add(NSKey.Space,Keys.Delete);
			keyNames.Add("/",Keys.Divide);
			keyNames.Add(NSKey.DownArrow,Keys.Down);
			keyNames.Add("\r",Keys.Enter);
			
			keyNames.Add("524608",Keys.LWin);
			keyNames.Add("262401",Keys.ControlKey);
			//keyNames.Add(NSEventModifierMask.FunctionKeyMask,Keys.f
			keyNames.Add(NSEventModifierMask.HelpKeyMask,Keys.Help);
			keyNames.Add("131332",Keys.RShiftKey | Keys.Shift);
			keyNames.Add("131330",Keys.LShiftKey | Keys.Shift);
			keyNames.Add("655650",Keys.Shift | Keys.Alt);
			*/			
			
				public static Keys GetKeys (NSEvent theEvent)
		{
			Initialize ();
			//TODO: Make modifiers work
			var nskey = Enum.ToObject (typeof(NSKey), theEvent.KeyCode);
			var modInt = (uint)theEvent.ModifierFlags & 0xFFFF0000;
			var modifier = ((NSEventModifierMask)Enum.ToObject (typeof(NSEventModifierMask), modInt)).ToKeys ();
			try {
				var key = (Keys)keyNames[nskey];
				return modInt != 0 ? key | modifier : key;
			} catch {
				try {
					// Works if the keys have the same name;
					var key = (Keys)Enum.Parse (typeof(Keys), nskey.ToString ());
					return modInt != 0 ? key | modifier : key;
				} catch {
					// None found
					return modInt != 0 ? modifier : Keys.None;
				}
			}
			
			//Works based on Character
		}
			/*
			//NSKey nskey =   (NSKey)theEvent.KeyCode;
			var foundMod = keyNames[theEvent.ModifierFlags.ToString()];
			Keys mod = foundMod == null ? 0 : (Keys)foundMod;
			
			var foundkey = keyNames[theEvent.Characters];
			Keys key = foundkey == null ? 0 : (Keys)foundkey;
			if (key == 0)
			{
				var keyName =  Enum.GetNames(typeof(Keys)).Where(x=> x == theEvent.Characters.ToUpper()).FirstOrDefault();
				if(string.IsNullOrEmpty(keyName))
				{
					return mod != 0 ? Keys.None | mod : Keys.None;
				}
				var theKey =  (Keys)Enum.Parse(typeof(Keys),keyName);
				return mod != 0 ? theKey | mod : theKey;
			}
			return mod != 0 ? key | mod : key;
			*/			
		
		
			}
}

