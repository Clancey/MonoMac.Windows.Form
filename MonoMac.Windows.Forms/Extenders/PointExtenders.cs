using System;
using System.Drawing;
namespace System.Windows.Forms
{
	public static class PointExtenders
	{
		public static PointF Subtract(this PointF orgPoint, PointF point)
        {
			var x = orgPoint.X - point.X;
			var y = orgPoint.Y - point.Y;
			return new PointF(x,y);
        }
		public static PointF Add(this PointF orgPoint, PointF point)
        {
			var x = orgPoint.X + point.X;
			var y = orgPoint.Y + point.Y;
			return new PointF(x,y);
        }
		
		public static PointF Add(this PointF orgPoint, Size size)
        {
			var x = orgPoint.X + size.Width;
			var y = orgPoint.Y + size.Height;
			return new PointF(x,y);
        }
		
		public static PointF Add(this PointF orgPoint, SizeF size)
        {
			var x = orgPoint.X + size.Width;
			var y = orgPoint.Y + size.Height;
			return new PointF(x,y);
        }
		
		public static Point Add(this Point orgPoint, Point point)
        {
			var x = orgPoint.X + point.X;
			var y = orgPoint.Y + point.Y;
			return new Point(x,y);
        }
		public static Point Subtract(this Point orgPoint, Point point)
        {
			var x = orgPoint.X - point.X;
			var y = orgPoint.Y - point.Y;
			return new Point(x,y);
        }
		
		public static Point Subtract(this Point orgPoint, PointF point)
        {
			var pointF = Point.Round(point);
			var x = orgPoint.X - pointF.X;
			var y = orgPoint.Y - pointF.Y;
			return new Point(x,y);
        }
	}
}

