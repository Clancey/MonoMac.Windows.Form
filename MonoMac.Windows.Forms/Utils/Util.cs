using System;
using System.Linq;
using System.Reflection;
using System.Drawing;
using MonoMac.CoreGraphics;
using System.Runtime.InteropServices;
using MonoMac.AppKit;

namespace System.Windows.Forms
{
	public static  class Util
	{
		public static string GetPropertyStringValue(object inObject, string propertyName)
		{
			PropertyInfo[] props = inObject.GetType().GetProperties();
			PropertyInfo prop = props.Select(p => p).Where(p =>  p.Name == propertyName).FirstOrDefault();
				if (prop != null)
					return prop.GetValue(inObject,null).ToString();
			return "";
		}
		
		public static object GetPropertyValue(object inObject, string propertyName)
		{
			PropertyInfo[] props = inObject.GetType().GetProperties();
			PropertyInfo prop = props.Select(p => p).Where(p =>  p.Name == propertyName).FirstOrDefault();
				if (prop != null)
					return prop.GetValue(inObject,null).ToString();
			return null;
		}
	
	
		[DllImport ("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextTranslateCTM (IntPtr context, float tx, float ty);
		[DllImport ("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
		internal static extern void CGContextScaleCTM (IntPtr context, float x, float y);
		
		public static void FlipDrawing(Rectangle rect)
		{
			var ctx = NSGraphicsContext.CurrentContext.GraphicsPort;
			CGContextTranslateCTM (ctx.Handle, (float)rect.X, (float)rect.Height);
			CGContextScaleCTM(ctx.Handle,1.0f,-1.0f);
		}
	}
}

