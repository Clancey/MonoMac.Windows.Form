using System;
using System.Collections;
using System.Linq;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;
namespace System.Windows.Forms
{
	public partial class UserControl 
	{

		private controls theControls;		
		public controls Controls {
			get {
				if (theControls == null)
					theControls = new controls (this);
				return theControls;
			}
		}
		
		public class controls
		{
			private NSView theView;
			public controls (Form form)
			{
				theView = form.ContentView;
			}
			public controls (NSView view)
			{
				theView = view;
			}


			public void Add (NSView item)
			{
				theView.AddSubview (item);
				SetTab ();
			}

			public void Clear ()
			{
				foreach (var view in theView.Subviews)
				{
					view.RemoveFromSuperview ();
				}
			}

			public bool Contains (NSView item)
			{
				return theView.Subviews.Contains (item);
			}

			public void CopyTo (NSView[] array, int arrayIndex)
			{
				theView.Subviews.CopyTo (array, arrayIndex);
			}

			public bool Remove (NSView item)
			{
				item.RemoveFromSuperview ();
				SetTab ();
				return true;
				
			}

			public int Count {
				get { return theView.Subviews.Count (); }
			}

			public bool IsReadOnly {
				get { return false; }
			}

			public int IndexOf (NSView item)
			{
				return theView.Subviews.ToList ().IndexOf (item);
				
			}

			public void Insert (int index, NSView item)
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
				get { return theView.Subviews[index]; }
					
				set { theView.Subviews[index] = value; }
			}
			//TODO: Make it work, It doesn't work as is
			public void SetTab ()
			{
				/*
				var controls = theForm.ContentView.Subviews.OrderBy (x => x.Tag).ToList ();
				for (int i = 0; i < controls.Count - 1; i++)
				{
					var firstControl = controls[i];
					var nextControl = controls[i + 1];
					firstControl.NextResponder = nextControl;
				}
				*/
				
			}
		}
	}

}


