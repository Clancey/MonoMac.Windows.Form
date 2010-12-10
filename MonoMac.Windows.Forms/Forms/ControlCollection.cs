using System;
using System.Linq;
using MonoMac.AppKit;
namespace System.Windows.Forms
{
	public class ControlCollection
	{
		private NSView theView;
		public ControlCollection (NSView view)
		{
			theView = view;
		}


		public void Add (Control item)
		{
			theView.AddSubview (item);
			SetTab ();
		}
		public void AddRange (Control[] InControls)
		{
			foreach (var view in InControls)
			{
				theView.AddSubview (view);
			}
		}

		public void Clear ()
		{
			foreach (var view in theView.Subviews)
			{
				view.RemoveFromSuperview ();
			}
		}

		public bool Contains (Control item)
		{
			return theView.Subviews.Contains (item);
		}

		public void CopyTo (Control[] array, int arrayIndex)
		{
			theView.Subviews.CopyTo (array, arrayIndex);
		}

		public bool Remove (Control item)
		{
			item.c_helper.RemoveFromSuperview ();
			SetTab ();
			return true;
			
		}

		public int Count {
			get { return theView.Subviews.Count (); }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public int IndexOf (Control item)
		{
			return theView.Subviews.ToList ().IndexOf (item);
			
		}

		public void Insert (int index, Control item)
		{
			if (index == 0)
			{
				theView.AddSubview (item, NSWindowOrderingMode.Below, null);
				return;
			}
			
			var rowBelow = theView.Subviews[index - 1];
			theView.AddSubview (item, NSWindowOrderingMode.Below, rowBelow);
			SetTab ();
		}

		public void RemoveAt (int index)
		{
			theView.Subviews[index].RemoveFromSuperview ();
			SetTab ();
		}

		public NSView this[int index] {
			get { return theView.Subviews[index];}

			set { theView.Subviews[index] = value; }
		}
		//TODO: Make it work, It doesn't work as is
		public void SetTab ()
		{
		}
	}
}

