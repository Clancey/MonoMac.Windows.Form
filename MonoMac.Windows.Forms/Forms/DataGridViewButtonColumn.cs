using System;
using System.Linq;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace System.Windows.Forms
{
	public class DataGridViewButtonColumn : NSTableColumn
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

