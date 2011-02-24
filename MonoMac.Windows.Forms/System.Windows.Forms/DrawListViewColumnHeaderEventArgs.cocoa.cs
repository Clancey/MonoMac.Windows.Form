using System;
using System.Drawing;
namespace System.Windows.Forms
{
	public partial class DrawListViewColumnHeaderEventArgs
	{
		
        #region Methods

        public void DrawBackground ()
        {
		// Always draw a non-pushed button
		//ThemeEngine.Current.CPDrawButton (graphics, bounds, ButtonState.Normal);
        }

        public void DrawText ()
        {
		DrawText (TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
        }

        public void DrawText (TextFormatFlags flags)
        {
		// Text adjustments
		Rectangle text_bounds = new Rectangle (bounds.X + 8, bounds.Y, bounds.Width - 13, bounds.Height);
		//TextRenderer.DrawText (graphics, header.Text, font, text_bounds, foreColor, flags);
        }

        #endregion Methods
	}
}

