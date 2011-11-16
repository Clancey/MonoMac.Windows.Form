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
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	internal class UpDownSpinnerHelper: NSStepper, IViewHelper
	{
		public UpDownSpinnerHelper ()
		{
		}
		public override void DidChange (MonoMac.Foundation.NSKeyValueChange changeKind, MonoMac.Foundation.NSIndexSet indexes, MonoMac.Foundation.NSString forKey)
		{
			base.DidChange (changeKind, indexes, forKey);
		}
		public override void DidChangeValue (string forKey)
		{
			base.DidChangeValue (forKey);
		}
		public override void DidChange (MonoMac.Foundation.NSString forKey, MonoMac.Foundation.NSKeyValueSetMutationKind mutationKind, MonoMac.Foundation.NSSet objects)
		{
			base.DidChange (forKey, mutationKind, objects);
		}
		public override void WillChangeValue (string forKey)
		{
			base.WillChangeValue (forKey);
		}
		#region IViewHelper implementation
		public void FontChanged ()
		{
			this.Font = Host.Font.ToNsFont();
		}
		
		#endregion
		#region IViewHelper implementation
		public NSCursor Cursor { get;set;}
		
		
		public Control Host {get;set;}
		
		#endregion
	}
}

