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
namespace System.Windows.Forms
{
	public static class MiscExtenders
	{
		public static Keys ToKeys (this NSEventModifierMask mask)
		{
			switch (mask) {
			//case NSEventModifierMask.AlphaShiftKeyMask:
			//	return Keys.None;
			case NSEventModifierMask.Alternate:
				return Keys.Alt;
			case NSEventModifierMask.Command:
				return Keys.LWin | Keys.RWin;
			case NSEventModifierMask.Control:
				return Keys.Control;
			//case NSEventModifierMask.FunctionKeyMask:
			//	return Keys.None;
			case NSEventModifierMask.Help:
				return Keys.Help;
			case NSEventModifierMask.Shift:
				return Keys.Shift;
			}
			return Keys.None;
		}
	}
}

