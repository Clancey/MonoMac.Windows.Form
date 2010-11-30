using System;
using MonoMac.AppKit;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
namespace System.Windows.Forms
{
	public class View : NSView
	{

		public override bool IsFlipped {
			get { return true; }
		}
		
	}

	[MonoMac.Foundation.Register("Form")]
	public partial class Form : NSWindow
	{
		private controls theControls;
		public Form () : base(new RectangleF (50, 50, 400, 400), (NSWindowStyle)(1 | (1 << 1) | (1 << 2) | (1 << 3)), NSBackingStore.Buffered, false)
		{
			theControls = new controls (this);
			ContentView = new View ();
			
		}
		public override NSResponder NextResponder {
			get { return base.NextResponder; }
			set { base.NextResponder = value; }
		}
		public override bool RespondsToSelector (MonoMac.ObjCRuntime.Selector sel)
		{
			return base.RespondsToSelector (sel);
		}

		public override void AwakeFromNib ()
		{
			
		}
		public void Show ()
		{
			this.MakeKeyAndOrderFront (this);
			//Controls.SetTab ();
		}
		public void Close()
		{
			if(NSApplication.SharedApplication.ModalWindow == this)
				NSApplication.SharedApplication.StopModal();
			this.PerformClose(this);
		}
		
		public void ShowDialog()
		{
			
			//this.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.BeginSheet(this,NSApplication.SharedApplication.MainWindow);
			NSApplication.SharedApplication.RunModalForWindow(this);
			NSApplication.SharedApplication.EndSheet(this);
			this.OrderOut(this);
			//NSApplication.SharedApplication.BeginSheet(this,NSApplication.SharedApplication.MainWindow);
		}
		
		
		/*
		public SizeF ClientSize {
			get { return this.Frame.Size; }
			set { this.SetFrame (new RectangleF (this.Frame.Location, value), true, true); }
		}
		*/

		public controls Controls {
			get {
				if (theControls == null)
					theControls = new controls (this);
				return theControls;
			}
		}
		/*
		public string Name {
			get { return this.FrameAutosaveName; }
			set { this.FrameAutosaveName = value; }
		}
		*/
		public string Text {
			get { return this.Title; }
			set { this.Title = value; }
		}
		public object components { get; set; }
		public void SuspendLayout ()
		{
			
		}
		public void ResumeLayout (bool action)
		{
			
		}
		public void PerformLayout ()
		{
			
		}
		public override void BecomeKeyWindow ()
		{
			base.BecomeKeyWindow ();
			if(Load != null)
				Load(this,new EventArgs());
		}
		public EventHandler Load {get;set;}
		
		#region From Template
			public string Name { get; set; }
		public SizeF Size {
			get { return this.Frame.Size; }
			set { this.SetFrame( new RectangleF (this.Frame.Location, value),true); }
		}

		public PointF Location {
			get { return this.Frame.Location; }
			set { this.SetFrame(new RectangleF (value, this.Frame.Size),true); }
		}
		/*
		public Rectangle ClientRectangle
		{
			get{ return Rectangle.Round(this.ContentView.Bounds);}
			set{ this.Bounds = value;}
		}
		*/
		public SizeF ClientSize
		{
			get {return this.Frame.Size;}
			set {this.SetFrame( new RectangleF(this.Frame.Location, value),true);}
		}
		
		public float Width
		{
			get{return this.Size.Width;}
			set {this.Size = new SizeF(value,this.Size.Height);}
		}
		
		public float Height
		{
			get{return this.Size.Height;}
			set {this.Size = new SizeF(this.Size.Width,value);}
		}
		
		public BorderStyle BorderStyle{get;set;}

		/*
		public bool Focused
		{
			get {
				if(this.CurrentEditor == null)
					return false;
				return this.Window.FirstResponder == this.CurrentEditor;}
		}
		*/
		
		public virtual void Refresh()
		{
			this.Display();
		}
		/*
		public virtual void Invalidate(Region region)
		{
			var rect = region.GetBounds(Graphics.FromHwnd (this.Handle));
			this.SetNeedsDisplayInRect(rect);
		}
		
		public virtual void Invalidate()
		{
			this.SetNeedsDisplay();	
		}
		
		public new System.Drawing.Font Font
		{
			get {
				return new System.Drawing.Font(base.Font.FontName, base.Font.PointSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			}
			set {
				base.Font = MonoMac.AppKit.NSFont.FromFontName(value.Name,value.Size);
				
			}
		}
		*/
		
		public EventHandler AutoSizeChanged{get;set;}
		public EventHandler AutoValidateChanged {get;set;}
		public MouseEventHandler MouseDown {get;set;}
		public MouseEventHandler MouseUp {get;set;}
		public EventHandler GotFocus {get;set;}
		public MouseEventHandler MouseMove {get;set;}
		public MouseEventHandler MouseDoubleClick{get;set;}
		public EventHandler SizeChanged {get;set;}
		
				
		
		#endregion
				
		public class controls
		{
			private Form theForm;
			public controls (Form form)
			{
				theForm = form;
			}


			public void Add (NSView item)
			{
				theForm.ContentView.AddSubview (item);
				SetTab ();
			}

			public void Clear ()
			{
				foreach (var view in theForm.ContentView.Subviews)
				{
					view.RemoveFromSuperview ();
				}
			}

			public bool Contains (NSView item)
			{
				return theForm.ContentView.Subviews.Contains (item);
			}

			public void CopyTo (NSView[] array, int arrayIndex)
			{
				theForm.ContentView.Subviews.CopyTo (array, arrayIndex);
			}

			public bool Remove (NSView item)
			{
				item.RemoveFromSuperview ();
				SetTab ();
				return true;
				
			}

			public int Count {
				get { return theForm.ContentView.Subviews.Count (); }
			}

			public bool IsReadOnly {
				get { return false; }
			}

			public int IndexOf (NSView item)
			{
				return theForm.ContentView.Subviews.ToList ().IndexOf (item);
				
			}

			public void Insert (int index, NSView item)
			{
				if (index == 0)
				{
					theForm.ContentView.AddSubview (item, NSWindowOrderingMode.Below, null);
					return;
				}
				
				var rowBelow = theForm.ContentView.Subviews[index - 1];
				theForm.ContentView.AddSubview (item, NSWindowOrderingMode.Below, rowBelow);
				SetTab ();
			}

			public void RemoveAt (int index)
			{
				theForm.ContentView.Subviews[index].RemoveFromSuperview ();
				SetTab ();
			}

			public NSView this[int index] {
				get { return theForm.ContentView.Subviews[index]; }
					
				set { theForm.ContentView.Subviews[index] = value; }
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

