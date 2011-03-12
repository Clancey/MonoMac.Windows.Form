using System;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	internal partial class ComboBoxHelper : NSComboBox, IViewHelper
	{
		#region private members
		int maxLength = -1;
		
		#endregion
		
		#region Public Member
		
		public int MaxLength
		{
			get { return maxLength;}
			set { maxLength = value;}
			
		}
		
		public string SelectedText
		{
			get{ return this.StringValue;}
			set{ this.StringValue = value;}
		}
		//TODO: Make Work
		public int SelectionLength
		{
			get{return this.SelectedText.Length;}
			set { 
				if(this.SelectedText.Length == value)
					return;
			}
		}
		
		public int SelectionStart{get;set;}
		
		//TODO: make work
		private AutoCompleteMode autoCompleteMode;
		public AutoCompleteMode AutoCompleteMode
		{
			get{return autoCompleteMode;}
			set{autoCompleteMode = value;}
			
		}
		
		#endregion
		
		#region Public Methods
		public override void KeyUp (NSEvent theEvent)
		{
			if(maxLength > -1 &&  this.StringValue.Length > maxLength)
				this.StringValue = this.StringValue.Substring(0,maxLength);
			base.KeyUp (theEvent);
		}
		
		#region IViewHelper implementation
		public NSCursor Cursor {get;set;}

		public Control Host {get;set;}
		#endregion
		#endregion
	}
}

