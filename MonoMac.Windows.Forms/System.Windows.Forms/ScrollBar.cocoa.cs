
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Runtime.InteropServices;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public partial class ScrollBar : Control
	{
		internal NSScroller m_helper {get;set;}
				
		public ScrollBar ()
		{
			position = 0;
			minimum = 0;
			maximum = 100;
			large_change = 10;
			small_change = 1;

			
			base.TabStop = false;

			SetStyle (ControlStyles.UserPaint | ControlStyles.StandardClick
#if NET_2_0
				| ControlStyles.UseTextForAccessibility
#endif
				, false);
		}
		
		
		#region Public Properties
		protected override ImeMode DefaultImeMode
		{
			get { return ImeMode.Disable; }
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Browsable (false)]
		public override Font Font
		{
			get { return base.Font; }
			set {
				if (base.Font.Equals (value))
					return;

				base.Font = value;
			}
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Browsable (false)]
		public override Color ForeColor
		{
			get { return base.ForeColor; }
			set {
				if (base.ForeColor == value)
					return;

				base.ForeColor = value;
				Refresh ();
			}
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Browsable (false)]
		public new ImeMode ImeMode
		{
			get { return base.ImeMode; }
			set {
				if (base.ImeMode == value)
					return;

				base.ImeMode = value;
			}
		}

		[DefaultValue (10)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[MWFDescription("Scroll amount when clicking in the scroll area"), MWFCategory("Behaviour")]
		public int LargeChange {
			get { return Math.Min (large_change, maximum - minimum + 1); }
			set {
				if (value < 0)
					throw new ArgumentOutOfRangeException ("LargeChange", string.Format ("Value '{0}' must be greater than or equal to 0.", value));

				if (large_change != value) {
					large_change = value;

					// thumb area depends on large change value,
					// so we need to recalculate it.
					CalcThumbArea ();
					//UpdatePos (Value, true);
					//InvalidateDirty ();

					// UIA Framework: Generate UIA Event to indicate LargeChange change
					OnUIAValueChanged (new ScrollEventArgs (ScrollEventType.LargeIncrement, value));
				}
			}
		}

		[DefaultValue (100)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[MWFDescription("Highest value for scrollbar"), MWFCategory("Behaviour")]
		public int Maximum {
			get { return maximum; }
			set {
				if (maximum == value)
					return;

				maximum = value;

				// UIA Framework: Generate UIA Event to indicate Maximum change
				OnUIAValueChanged (new ScrollEventArgs (ScrollEventType.Last, value));

				if (maximum < minimum)
					minimum = maximum;
				if (Value > maximum)
					Value = maximum;
					
				// thumb area depends on maximum value,
				// so we need to recalculate it.
				CalcThumbArea ();
				//UpdatePos (Value, true);
				//InvalidateDirty ();
			}
		}

		internal void SetValues (int maximum, int large_change)
		{
			SetValues (-1, maximum, -1, large_change);
		}

		internal void SetValues (int minimum, int maximum, int small_change, int large_change)
		{
			bool update = false;

			if (-1 != minimum && this.minimum != minimum) {
				this.minimum = minimum;

				if (minimum > this.maximum)
					this.maximum = minimum;
				update = true;

				// change the position if it is out of range now
				position = Math.Max (position, minimum);
			}

			if (-1 != maximum && this.maximum != maximum) {
				this.maximum = maximum;

				if (maximum < this.minimum)
					this.minimum = maximum;
				update = true;

				// change the position if it is out of range now
				position = Math.Min (position, maximum);
			}

			if (-1 != small_change && this.small_change != small_change) {
				this.small_change = small_change;
			}

			if (this.large_change != large_change) {
				this.large_change = large_change;
				update = true;
			}

			if (update) {
				CalcThumbArea ();
				//UpdatePos (Value, true);
				//InvalidateDirty ();
			}
		}

		[DefaultValue (0)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[MWFDescription("Smallest value for scrollbar"), MWFCategory("Behaviour")]
		public int Minimum {
			get { return minimum; }
			set {
				if (minimum == value)
					return;

				minimum = value;

				// UIA Framework: Generate UIA Event to indicate Minimum change
				OnUIAValueChanged (new ScrollEventArgs (ScrollEventType.First, value));

				if (minimum > maximum)
					maximum = minimum;

				// thumb area depends on minimum value,
				// so we need to recalculate it.
				CalcThumbArea ();
				//UpdatePos (Value, true);
				//InvalidateDirty ();
			}
		}

		[DefaultValue (1)]
		[MWFDescription("Scroll amount when clicking scroll arrows"), MWFCategory("Behaviour")]
		public int SmallChange {
			get { return small_change > LargeChange ? LargeChange : small_change; }
			set {
				if ( value < 0 )
					throw new ArgumentOutOfRangeException ("SmallChange", string.Format ("Value '{0}' must be greater than or equal to 0.", value));

				if (small_change != value) {
					small_change = value;
					//UpdatePos (Value, true);
					//InvalidateDirty ();

					// UIA Framework: Generate UIA Event to indicate SmallChange change
					OnUIAValueChanged (new ScrollEventArgs (ScrollEventType.SmallIncrement, value));
				}
			}
		}

		[DefaultValue (false)]
		public new bool TabStop {
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		[Bindable (false)]
		[Browsable (false)]
		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
		public override string Text {
			 get { return base.Text;  }
			 set { base.Text = value; }
		}

		[Bindable(true)]
		[DefaultValue (0)]
		[MWFDescription("Current value for scrollbar"), MWFCategory("Behaviour")]
		public int Value {
			get { return position; }
			set {
				if ( value < minimum || value > maximum )
					throw new ArgumentOutOfRangeException ("Value", string.Format ("'{0}' is not a valid value for 'Value'. 'Value' should be between 'Minimum' and 'Maximum'", value));

				if (position != value){
					position = value;

					OnValueChanged (EventArgs.Empty);

					if (IsHandleCreated) {
						Rectangle thumb_rect = thumb_pos;

						//UpdateThumbPos ((vert ? thumb_area.Y : thumb_area.X) + (int)(((float)(position - minimum)) * pixel_per_pos), false, false);

						//MoveThumb (thumb_rect, vert ? thumb_pos.Y : thumb_pos.X);
					}
				}
			}
		}

		#endregion //Public Properties
		
		#region Private Methods
		
		private void CalcThumbArea ()
		{
			int lchange = use_manual_thumb_size ? manual_thumb_size : LargeChange;

			// Thumb area
			if (vert) {

				thumb_area.Height = Height - scrollbutton_height -  scrollbutton_height;
				thumb_area.X = 0;
				thumb_area.Y = scrollbutton_height;
				thumb_area.Width = Width;

				if (Height < thumb_notshown_size)
					thumb_size = 0;
				else {
					double per =  ((double) lchange / (double)((1 + maximum - minimum)));
					thumb_size = 1 + (int) (thumb_area.Height * per);

					if (thumb_size < thumb_min_size)
						thumb_size = thumb_min_size;
						
					// Give the user something to drag if LargeChange is zero
					if (LargeChange == 0)
						thumb_size = 17;
				}

				pixel_per_pos = ((float)(thumb_area.Height - thumb_size) / (float) ((maximum - minimum - lchange) + 1));

			} else	{

				thumb_area.Y = 0;
				thumb_area.X = scrollbutton_width;
				thumb_area.Height = Height;
				thumb_area.Width = Width - scrollbutton_width -  scrollbutton_width;

				if (Width < thumb_notshown_size)
					thumb_size = 0;
				else {
					double per =  ((double) lchange / (double)((1 + maximum - minimum)));
					thumb_size = 1 + (int) (thumb_area.Width * per);

					if (thumb_size < thumb_min_size)
						thumb_size = thumb_min_size;
						
					// Give the user something to drag if LargeChange is zero
					if (LargeChange == 0)
						thumb_size = 17;
				}

				pixel_per_pos = ((float)(thumb_area.Width - thumb_size) / (float) ((maximum - minimum - lchange) + 1));
			}
		}
		
		#endregion
		
		
		#region UIA Framework Section: Events, Methods and Properties.

		//NOTE:
		//	We are using Reflection to add/remove internal events.
		//	Class ScrollBarButtonInvokePatternInvokeEvent uses the events.
		//
    		// Types used to generate UIA InvokedEvent
		// * args.Type = ScrollEventType.LargeIncrement. Space between Thumb and bottom/right Button
		// * args.Type = ScrollEventType.LargeDecrement. Space between Thumb and top/left Button
		// * args.Type = ScrollEventType.SmallIncrement. Small increment UIA Button (bottom/right Button)
    		// * args.Type = ScrollEventType.SmallDecrement. Small decrement UIA Button (top/left Button)
		// Types used to generate RangeValue-related events
		// * args.Type = ScrollEventType.LargeIncrement. LargeChange event
		// * args.Type = ScrollEventType.Last. Maximum event
		// * args.Type = ScrollEventType.First. Minimum event
		// * args.Type = ScrollEventType.SmallIncrement. SmallChange event
		static object UIAScrollEvent = new object ();
		static object UIAValueChangeEvent = new object ();

		internal event ScrollEventHandler UIAScroll {
			add { Events.AddHandler (UIAScrollEvent, value); }
			remove { Events.RemoveHandler (UIAScrollEvent, value); }
		}

		internal event ScrollEventHandler UIAValueChanged {
			add { Events.AddHandler (UIAValueChangeEvent, value); }
			remove { Events.RemoveHandler (UIAValueChangeEvent, value); }
		}

		internal void OnUIAScroll (ScrollEventArgs args)
		{
			ScrollEventHandler eh = (ScrollEventHandler) Events [UIAScrollEvent];
			if (eh != null)
				eh (this, args);
		}

		internal void OnUIAValueChanged (ScrollEventArgs args)
		{
			ScrollEventHandler eh = (ScrollEventHandler) Events [UIAValueChangeEvent];
			if (eh != null)
				eh (this, args);
		}

		
		internal Rectangle UIAThumbArea {
			get { return thumb_area; }
		}

		internal Rectangle UIAThumbPosition {
			get { return thumb_pos; }
		}

		#endregion UIA Framework Section: Events, Methods and Properties.

	}
}

