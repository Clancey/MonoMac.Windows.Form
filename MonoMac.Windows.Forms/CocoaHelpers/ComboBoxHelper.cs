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
using System.Drawing;
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
		public new Font Font
		{
			get { return base.Font.ToFont();}
			set{ base.Font = value.ToNsFont();}
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

