using System;
using System.Drawing;
namespace System.Windows.Forms
{
	public abstract partial class UpDownBase: ContainerControl
	{
		
		#region Private & Internal Methods
		#endregion Private & Internal Methods
		
		#region Private Methods
		internal override void OnPaintInternal (PaintEventArgs e)
		{
			e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
		}
		
		internal void layoutControl()
		{
			spnSpinner.Height = this.Height;	
			//TODO: fix this bug, had to add 20 to make up for the shrunken size 
			txtView.Height = this.Height * 2;
			txtView.Width = this.Width - spnSpinner.Width;
			
			if(_UpDownAlign == LeftRightAlignment.Left)
			{
				txtView.Location = new Point(spnSpinner.Width,0);
				spnSpinner.Location = new Point(0,0); 
			}
			else
			{
				txtView.Location = new Point(0,0);
				spnSpinner.Location = new Point(txtView.Width,0);
			}
		}
		
		internal override void UpdateBounds ()
		{
			base.UpdateBounds ();
			layoutControl();
		}

		#endregion	// Private Methods

	}
}

