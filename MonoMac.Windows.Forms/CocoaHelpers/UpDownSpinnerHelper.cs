using System;
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

