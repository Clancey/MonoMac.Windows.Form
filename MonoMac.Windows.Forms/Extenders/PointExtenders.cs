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
using System.Drawing;
namespace System.Windows.Forms
{
	public static class PointExtenders
	{
		public static PointF Subtract (this PointF orgPoint, PointF point)
		{
			var x = orgPoint.X - point.X;
			var y = orgPoint.Y - point.Y;
			return new PointF (x, y);
		}
		public static PointF Add (this PointF orgPoint, PointF point)
		{
			var x = orgPoint.X + point.X;
			var y = orgPoint.Y + point.Y;
			return new PointF (x, y);
		}

		public static PointF Add (this PointF orgPoint, Size size)
		{
			var x = orgPoint.X + size.Width;
			var y = orgPoint.Y + size.Height;
			return new PointF (x, y);
		}

		public static PointF Add (this PointF orgPoint, SizeF size)
		{
			var x = orgPoint.X + size.Width;
			var y = orgPoint.Y + size.Height;
			return new PointF (x, y);
		}

		public static Point Add (this Point orgPoint, Point point)
		{
			var x = orgPoint.X + point.X;
			var y = orgPoint.Y + point.Y;
			return new Point (x, y);
		}
		public static Point Subtract (this Point orgPoint, Point point)
		{
			var x = orgPoint.X - point.X;
			var y = orgPoint.Y - point.Y;
			return new Point (x, y);
		}

		public static Point Subtract (this Point orgPoint, PointF point)
		{
			var pointF = Point.Round (point);
			var x = orgPoint.X - pointF.X;
			var y = orgPoint.Y - pointF.Y;
			return new Point (x, y);
		}
	}
}

