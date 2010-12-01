using System;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	//TODO: Create
	public class CheckedListBox : ListBox
	{
		public CheckedListBox ()
		{
			colString = new NSString("CheckedListBox");
		}
		public override void SetupColumn ()
		{
			column = new DataGridViewCheckBoxColumn(colString);
			
			column.DataCell.Editable = false;
			tableView.AddColumn(column);
		}
	}
}

