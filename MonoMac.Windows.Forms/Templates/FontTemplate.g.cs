using System;
using System.Collections;
using System.Linq;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	internal partial class UserControl 
	{	
		public new System.Drawing.Font Font
		{
			get {
				if(base.Font == null)
					return new System.Drawing.Font ("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				return new System.Drawing.Font(base.Font.FontName, base.Font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}
			set {
				base.Font = MonoMac.AppKit.NSFont.FromFontName(value.Name,value.Size);
				
			}
		}
	}
	
	
	internal partial class ButtonHelper 
	{	
		public new System.Drawing.Font Font
		{
			get {
				if(base.Font == null)
					return new System.Drawing.Font ("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				return new System.Drawing.Font(base.Font.FontName, base.Font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}
			set {
				base.Font = MonoMac.AppKit.NSFont.FromFontName(value.Name,value.Size);
				
			}
		}
	}
	
	
	internal partial class TextBoxMouseView 
	{	
		public new System.Drawing.Font Font
		{
			get {
				if(base.Font == null)
					return new System.Drawing.Font ("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				return new System.Drawing.Font(base.Font.FontName, base.Font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}
			set {
				base.Font = MonoMac.AppKit.NSFont.FromFontName(value.Name,value.Size);
				
			}
		}
	}
	
	
	internal partial class ComboBoxMouseView 
	{	
		public new System.Drawing.Font Font
		{
			get {
				if(base.Font == null)
					return new System.Drawing.Font ("Arial", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				return new System.Drawing.Font(base.Font.FontName, base.Font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}
			set {
				base.Font = MonoMac.AppKit.NSFont.FromFontName(value.Name,value.Size);
				
			}
		}
	}
	
	
}


