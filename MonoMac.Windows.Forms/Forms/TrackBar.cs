using System;
using MonoMac.AppKit;
using System.Drawing;
namespace System.Windows.Forms
{
	public partial class TrackBar : NSSlider
	{
		public TrackBar () : base ()
		{
			this.Frame = new RectangleF(0,0,100,25);
			this.Activated += delegate(object sender, EventArgs e) {
				if(Scroll != null)
					Scroll(sender,e);
			};
		}
		
		public int Value
		{
			get{return this.IntValue;}
			set {this.IntValue = value;}
		}
		public int Maximum 
		{
			get { return (int)this.MaxValue;}
			set{this.MaxValue = (double)value;
				setTickMarks();}
		}
		public int Minimum 
		{
			get { return (int)this.MinValue;}
			set{this.MinValue = (double)value;
				setTickMarks();}
		}
		private int tickFrequency = 1;
		public int TickFrequency
		{
			get { return tickFrequency;}
			set { 
				tickFrequency = value;
			}
		}
		private void setTickMarks()
		{
			var count = (Maximum - Minimum) / tickFrequency;
			this.TickMarksCount = count +  1;
		}
		public int LargeChange
		{
			get;set;}
		public int SmallChange {get;set;}
		
		public virtual Color BackColor{get;set;}
		
		public virtual EventHandler Scroll {get;set;}
	}
}

