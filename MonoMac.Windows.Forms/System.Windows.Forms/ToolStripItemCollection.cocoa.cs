using System;

namespace System.Windows.Forms
{
	public partial class ToolStripItemCollection
	{
		
		public int Add (ToolStripItem value)
		{
			if (value == null)
				throw new ArgumentNullException ("value");
			
			if (Contains (value))
				return IndexOf (value);

			value.InternalOwner = owner;
				
			//if (value is ToolStripMenuItem && (value as ToolStripMenuItem).ShortcutKeys != Keys.None)
			//	ToolStripManager.AddToolStripMenuItem ((ToolStripMenuItem)value);
				
			int index = base.Add (value);
			
			if (this.internal_created)
				owner.OnItemAdded (new ToolStripItemEventArgs (value));
				
			return index;
		}
		
		public void Insert (int index, ToolStripItem value)
		{
			if (value == null)
				throw new ArgumentNullException ("value");

			//if (value is ToolStripMenuItem && (value as ToolStripMenuItem).ShortcutKeys != Keys.None)
			//	ToolStripManager.AddToolStripMenuItem ((ToolStripMenuItem)value);

			if (value.Owner != null)
				value.Owner.Items.Remove (value);
				
			base.Insert (index, value);
			
			if (internal_created) {
				value.InternalOwner = owner;
				owner.OnItemAdded (new ToolStripItemEventArgs (value));
			}
			
			if (owner.Created)
				owner.PerformLayout ();
		}
	}
}

