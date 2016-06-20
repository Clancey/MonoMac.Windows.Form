using System;
using System.Linq;
using AppKit;
using Foundation;

namespace System.Windows.Forms
{
	public partial class DataGridViewButtonColumn : NSTableColumn
	{
		public DataGridViewButtonColumn () : base ()
		{
			this.DataCell = new DataGridViewButtonCell();
		}
		public DataGridViewButtonColumn (NSObject identifier) : base (identifier)
		{
			this.DataCell = new DataGridViewButtonCell();
		}
	}
}

