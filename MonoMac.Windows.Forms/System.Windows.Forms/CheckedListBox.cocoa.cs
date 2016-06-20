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
using Foundation;
namespace System.Windows.Forms
{
	//TODO: Create
	public partial  class CheckedListBox : ListBox
	{
		/*
		public CheckedListBox ()
		{
			colString = new NSString("CheckedListBox");
		}
		*/
		
		public override void SetupColumn ()
		{
			column = new DataGridViewCheckBoxColumn(colString);
			
			column.DataCell.Editable = false;
			tableView.AddColumn(column);
		}
		
		

		private void UpdateCollections ()
		{
			if(CheckedItems != null)
				CheckedItems.Refresh ();
			if(CheckedIndices != null)
				CheckedIndices.Refresh ();
		}
	}
}

