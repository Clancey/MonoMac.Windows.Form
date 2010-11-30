using System;
namespace TestLoad
{
	public class ComboBoxItems
	{
		public ComboBoxItems ()
		{			
		}
		public ComboBoxItems (string display,int theValue)
		{		
			Display = display;
			Value = theValue;
		}
		public string Display {get;set;}
		public int Value {get;set;}
	}
}

