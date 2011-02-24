using System;
using System.Drawing;
namespace System.Windows.Forms
{
	public partial class DrawListViewItemEventArgs
	{
		
		#region Public Methods

        public void DrawBackground ()
        {
			graphics.FillRectangle (new SolidBrush(item.BackColor), bounds);
        }

        public void DrawFocusRectangle ()
        {
		//if ((state & ListViewItemStates.Focused) != 0)
		//	ThemeEngine.Current.CPDrawFocusRectangle (graphics, bounds, item.ListView.ForeColor, item.ListView.BackColor);
        }

        public void DrawText ()
        {
		DrawText (TextFormatFlags.Default);
        }

        public void DrawText (TextFormatFlags flags)
        {
		//TextRenderer.DrawText (graphics, item.Text, item.Font, bounds, item.ForeColor, flags);
        }

        #endregion Public Methods
	}
}

