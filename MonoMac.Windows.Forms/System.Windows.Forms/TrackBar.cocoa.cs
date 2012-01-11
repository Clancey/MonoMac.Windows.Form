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
	public partial class TrackBar : Control
	{
		// TrackBarMouseView
		internal TrackBarMouseView m_helper;

		public TrackBar () : base()
		{
		}
		protected override void CreateHandle ()
		{
			m_helper = new TrackBarMouseView ();
			m_view = m_helper;
			m_helper.Host = this;
			m_helper.Frame = new RectangleF (0, 0, 100, 25);
			m_helper.Activated += delegate(object sender, EventArgs e) {
				if (Scroll != null)
					Scroll (sender, e);
			};
		}

		public int Value {
			get { return m_helper.IntValue; }
			set { m_helper.IntValue = value; }
		}
		public int Maximum {
			get { return (int)m_helper.MaxValue; }
			set {
				m_helper.MaxValue = (double)value;
				setTickMarks ();
			}
		}
		public int Minimum {
			get { return (int)m_helper.MinValue; }
			set {
				m_helper.MinValue = (double)value;
				setTickMarks ();
			}
		}
		private int tickFrequency = 1;
		public int TickFrequency {
			get { return tickFrequency; }
			set { tickFrequency = value; }
		}
		private void setTickMarks ()
		{
			var count = (Maximum - Minimum) / tickFrequency;
			m_helper.TickMarksCount = count + 1;
		}
		public int LargeChange { get; set; }
		public int SmallChange { get; set; }

		public virtual Color BackColor { get; set; }

		public virtual EventHandler Scroll { get; set; }
	}
}

