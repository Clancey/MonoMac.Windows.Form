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
using System.Reflection;
using System.Drawing;
using MonoMac.CoreGraphics;
using System.Runtime.InteropServices;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace System.Windows.Forms
{
	public static class Util
	{
		public static string GetPropertyStringValue (object inObject, string propertyName)
		{
			PropertyInfo[] props = inObject.GetType ().GetProperties ();
			PropertyInfo prop = props.Select (p => p).Where (p => p.Name == propertyName).FirstOrDefault ();
			if (prop != null)
				return prop.GetValue (inObject, null).ToString ();
			return "";
		}

		public static object GetPropertyValue (object inObject, string propertyName)
		{
			PropertyInfo[] props = inObject.GetType ().GetProperties ();
			PropertyInfo prop = props.Select (p => p).Where (p => p.Name == propertyName).FirstOrDefault ();
			if (prop != null)
				return prop.GetValue (inObject, null).ToString ();
			return null;
		}

		public static int NumberOfSetBits (int i)
		{
			i = i - ((i >> 1) & 0x55555555);
			i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
			return ((i + (i >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
		}

		//, SizeF size)
		public static SizeF MeasureString (string inString, Font font)
		{
			
						/*NSTextStorage textStorage = new NSTextStorage();
			textStorage.
			NSTextContainer textContainer = new NSTextContainer(size);
			NSLayoutManager layoutManager = new NSLayoutManager();
			*/
var tv = new NSText { Font = font.ToNsFont (), Value = inString };
			tv.SizeToFit ();
			return tv.Frame.Size;
		}

		public static SizeF ScaleSize = new SizeF (.92f, .92f);
		//(.92f,.92f);
		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		static internal extern void CGContextTranslateCTM (IntPtr context, float tx, float ty);
		[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		static internal extern void CGContextScaleCTM (IntPtr context, float x, float y);

		public static void FlipDrawing (Rectangle rect)
		{
			var ctx = NSGraphicsContext.CurrentContext.GraphicsPort;
			CGContextTranslateCTM (ctx.Handle, (float)rect.X, (float)rect.Height);
			CGContextScaleCTM (ctx.Handle, 1.0f, -1.0f);
		}
	}
}

