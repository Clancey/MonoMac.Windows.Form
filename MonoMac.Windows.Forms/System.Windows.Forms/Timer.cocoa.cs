using System;
using System.Threading;
using System.ComponentModel;
using MonoMac.AppKit;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	public partial class Timer
	{
		internal NSTimer m_helper;

		public Timer ()
		{
			throw new NotImplementedException ("Timer not implemented yet");
			//m_helper = new NSTimer();
			enabled = false;
		}
		
		[DefaultValue (false)]
		public virtual bool Enabled {
			get {
				return enabled;
			}
			set {
				if (value != enabled) {
					enabled = value;
					if (value) {
						// Use AddTicks so we get some rounding
						expires = DateTime.UtcNow.AddMilliseconds (interval > Minimum ? interval : Minimum);

						thread = Thread.CurrentThread;
						m_helper = NSTimer.CreateRepeatingTimer(new TimeSpan(0,0,0,0,Interval),NSTimerFire);
					} else {
						m_helper.Invalidate();
						thread = null;
					}
				}
			}
		}
		
		[Export("NSTimerFire")]
		void NSTimerFire()
		{
			FireTick();
		}
		
		
		[DefaultValue (100)]
		public int Interval {
			get {
				return interval;
			}
			set {
				if (value <= 0)
					throw new ArgumentOutOfRangeException ("Interval", string.Format ("'{0}' is not a valid value for Interval. Interval must be greater than 0.", value));

				if (interval == value) {
					return;
				}
				
				interval = value;
								
				// Use AddTicks so we get some rounding
				expires = DateTime.UtcNow.AddMilliseconds (interval > Minimum ? interval : Minimum);
									
				if (enabled == true) {	
					m_helper.Invalidate();
					m_helper = NSTimer.CreateRepeatingTimer(new TimeSpan(0,0,0,0,Interval),NSTimerFire);
				}
			}
		}
	}
}

