using System;
using System.Linq;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace System.Windows.Forms
{
	public partial class DataGridViewCheckBoxColumn : DataGridViewButtonColumn
	{
		public DataGridViewCheckBoxColumn () : base ()
		{
			this.DataCell = new DataGridViewCheckBoxCell();
		}		
		public DataGridViewCheckBoxColumn (NSObject identifier) : base (identifier)
		{
			this.DataCell = new DataGridViewCheckBoxCell();
		}
	}
}

