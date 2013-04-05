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

#if MAC64
using NSInteger = System.Int64;
using NSUInteger = System.UInt64;
using CGFloat = System.Double;
#else
using NSInteger = System.Int32;
using NSUInteger = System.UInt32;
using NSPoint = System.Drawing.PointF;
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using CGFloat = System.Single;
#endif

namespace System.Windows.Forms
{
	static class Util
	{
		public static Rectangle NSRectToRectangle(NSRect r)
		{
#if MAC64
			int x = (int)(r.X+0.5);
			int y = (int)(r.Y+0.5);
			int width = (int)(r.Width+0.5);
			int height = (int)(r.Height+0.5);
			return new Rectangle(x,y,width,height);
#else
			return Rectangle.Round(r);
#endif
		}

		public static RectangleF NSRectToRectangleF(NSRect r)
		{
#if MAC64
			float x = (float)(r.X);
			float y = (float)(r.Y);
			float width = (float)(r.Width);
			float height = (float)(r.Height);
			return new RectangleF(x,y,width,height);
#else
			return r;
#endif
		}

		public static NSRect RectangleToNSRect(Rectangle r)
		{
#if MAC64
			return new NSRect(r);
#else
			return new NSRect(r.X, r.Y, r.Width, r.Height);
#endif
		}

		public static Size NSSizeToSize(NSSize s)
		{
#if MAC64
			int width = (int)(s.Width+0.5);
			int height = (int)(s.Height+0.5);
			return new Size(width,height);
#else
			return Size.Round(s);
#endif
		}

		public static NSSize SizeToNSSize(Size s)
		{
#if MAC64
			return new NSSize (s.Width, s.Height);
#else
			return new NSSize (s);
#endif
		}

		public static Point NSPointToPoint(NSPoint p)
		{
#if MAC64
			int x = (int)(p.X+0.5);
			int y = (int)(p.Y+0.5);
			return new Point(x,y);
#else
			return Point.Round(p);
#endif
		}

		public static NSPoint PointToNSPoint(Point p)
		{
#if MAC64
			return new NSPoint(p.X, p.Y);
#else
			return new PointF(p.X, p.Y);
#endif
		}

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
			/*
			NSTextStorage textStorage = new NSTextStorage();
			textStorage.
			NSTextContainer textContainer = new NSTextContainer(size);
			NSLayoutManager layoutManager = new NSLayoutManager();
			*/
			var tv = new NSText { Font = font.ToNsFont (), Value = inString };
			tv.SizeToFit ();
			float w = (float)tv.Frame.Size.Width;
			float h = (float)tv.Frame.Size.Height;
			return new SizeF (w, h);
		}

		public static NSSize ScaleSize = new NSSize (.92f, .92f);
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

