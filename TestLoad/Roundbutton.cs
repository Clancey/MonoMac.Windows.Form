using System;
using System.Windows.Forms;
using System.Drawing;
namespace TestLoad
{
	public class RoundButton : UserControl {

   		public Color backgroundColor = Color.Blue;
	    protected override void OnPaint(PaintEventArgs e) {
	
		
	      Graphics graphics = e.Graphics;
	
	      int penWidth = 4;
	      Pen pen = new Pen(Color.Black, 4);
	
	      int fontHeight = 10;
	      Font font = new Font("Arial", fontHeight);
	
	      SolidBrush brush = new SolidBrush(backgroundColor);
	      graphics.FillEllipse(brush, 0, 0, Width, Height);
	      SolidBrush textBrush = new SolidBrush(Color.Black);
	
	      graphics.DrawEllipse(pen, (int) penWidth/2, 
	        (int) penWidth/2, Width - penWidth, Height - penWidth);
	
	      graphics.DrawString(Text, font, textBrush, penWidth, 
	        Height / 2 - fontHeight);
	
	    }
	}
}