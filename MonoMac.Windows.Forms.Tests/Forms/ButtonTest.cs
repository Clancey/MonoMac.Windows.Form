using System;
using System.Windows.Forms;
using NUnit;
using NUnit.Framework;
using MonoMac.AppKit;
using System.Drawing;
namespace MonoMac.Windows.Forms.Tests
{
	[TestFixture]
	public class ButtonTest : Test
	{
		[Test]
		public void text_should_set_title()
		{
			var theString = "Test String";
			var button = new Button{Text = theString} ;
			Assert.AreEqual(theString,button.Title);
		}
		
		[Test]
		public void size_will_set_frame_size()
		{
			var size = new SizeF(100,100);
			var button = new Button {Size = size};
			Assert.AreEqual(size,button.Frame.Size);
			
		}
		
		[Test]
		public void location_will_set_frame_location()
		{
			var location = new PointF(100,100);
			var button = new Button {Location = location};
			Assert.AreEqual(location,button.Frame.Location);
			
		}
	}
}

