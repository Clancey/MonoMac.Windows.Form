using System;
using System.Drawing;
namespace System.Windows.Forms
{
	public interface IControl
	{
		SizeF Size{get;set;}
		PointF Location {get;set;}
		bool Visible {get;set;}
		
		#region Events

		//TODO: Key Down does not fire from monomac yet.
	    KeyEventHandler OnKeyDown { get; set; }

        KeyPressEventHandler OnKeyPress { get; set; }

        KeyEventHandler OnKeyUp { get; set; }
		#endregion
	}
}

