using System;
using System.Drawing;
namespace System.Windows.Forms
{
	public partial class ColumnHeader
	{
		private int width = 75;
		
		#region Private Methods
		

		internal void CalcColumnHeader ()
		{
			if (text_alignment == HorizontalAlignment.Center)
				format.Alignment = StringAlignment.Center;
			else if (text_alignment == HorizontalAlignment.Right)
				format.Alignment = StringAlignment.Far;
			else
				format.Alignment = StringAlignment.Near;
			format.LineAlignment = StringAlignment.Center;
			format.Trimming = StringTrimming.EllipsisCharacter;
			// text is wrappable only in LargeIcon and SmallIcon views
			format.FormatFlags = StringFormatFlags.NoWrap;
			
			column_rect.Height = 25;
			/*
			if (owner != null)
				column_rect.Height = ThemeEngine.Current.ListViewGetHeaderHeight (owner, owner.Font);
			else
				column_rect.Height = ThemeEngine.Current.ListViewGetHeaderHeight (null, ThemeEngine.Current.DefaultFont);
			 */
			column_rect.Width = 0;

			if (width >= 0) // manual width
				column_rect.Width = width;
			else if (Index != -1) { // automatic width, either -1 or -2
				// try to expand if we are the last column
				bool expand_to_right = Index == owner.Columns.Count - 1 && width == -2;
				Rectangle visible_area = owner.ClientRectangle;

				column_rect.Width = owner.GetChildColumnSize (Index).Width;
				width = column_rect.Width;

				// expand only if we have free space to the right
				if (expand_to_right && column_rect.X + column_rect.Width < visible_area.Width) {
					width = visible_area.Width - column_rect.X;
					if (owner.v_scroll.Visible)
						width -= owner.v_scroll.Width;

					column_rect.Width = width;
				}
			}
		}
		#endregion
	}
}

