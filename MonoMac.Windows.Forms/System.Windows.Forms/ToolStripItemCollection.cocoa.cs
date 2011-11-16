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
			value.Index = index;
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

