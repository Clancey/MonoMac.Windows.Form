using System;
using NUnit.Framework;
using MonoMac.AppKit;
namespace MonoMac.Windows.Forms.Tests
{
	[TestFixture]
	public class Test
	{
		bool isFirstRun = true;
		[TestFixtureSetUp]
		public void init()
		{
			if(isFirstRun)
			{
				NSApplication.Init();
				isFirstRun = false;
			}
		}
	}
}

