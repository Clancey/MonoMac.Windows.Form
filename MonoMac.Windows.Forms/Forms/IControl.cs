using System;
using System.Drawing;
namespace System.Windows.Forms
{
	public interface IControl
	{
		SizeF Size{get;set;}
		PointF Location {get;set;}
		
		#region Events


	    KeyEventHandler OnKeyDown { get; set; }

        KeyPressEventHandler OnKeyPress { get; set; }

        KeyEventHandler OnKeyUp { get; set; }
		#endregion
	}
}

