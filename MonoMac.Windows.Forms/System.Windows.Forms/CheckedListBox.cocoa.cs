using System;
using MonoMac.Foundation;
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

