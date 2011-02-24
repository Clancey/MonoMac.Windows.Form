using System;
using System.Drawing;
namespace System.Windows.Forms
{
	public partial class DrawListViewSubItemEventArgs
	{
        #region Public Methods

        public void DrawBackground ()
        {
		graphics.FillRectangle ( new SolidBrush (subItem.BackColor), bounds);
        }

        public void DrawFocusRectangle (Rectangle bounds)
        {
		if ((itemState & ListViewItemStates.Focused) != 0) {
			Rectangle rect = new Rectangle (bounds.X + 1, bounds.Y + 1, bounds.Width - 1, bounds.Height - 1);
			//ThemeEngine.Current.CPDrawFocusRectangle (graphics, rect, subItem.ForeColor, subItem.BackColor);
		}
        }

        public void DrawText ()
        {
		DrawText (TextFormatFlags.EndEllipsis | TextFormatFlags.HorizontalCenter);
        }

        public void DrawText (TextFormatFlags flags)
        {
		// Text adjustments
		Rectangle text_bounds = new Rectangle (bounds.X + 8, bounds.Y, bounds.Width - 13, bounds.Height);
		//TextRenderer.DrawText (graphics, subItem.Text, subItem.Font, text_bounds, subItem.ForeColor, flags);
        }

        #endregion Public Methods
	}
}

