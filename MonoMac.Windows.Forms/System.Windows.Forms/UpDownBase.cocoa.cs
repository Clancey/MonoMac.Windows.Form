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

