﻿namespace PhysicalDrag
{
	partial class frmDragTest
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			this.SuspendLayout ();
			// 
			// frmDragTest
			// 
			//this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			//this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Size = new System.Drawing.Size (528, 679);
			//this.MaximizeBox = false;
			//this.MinimizeBox = false;
			this.Name = "frmDragTest";
			//this.ShowIcon = false;
			this.Text = "Physical drag";
			this.Load += new System.EventHandler (this.frmDragTest_Load);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler (this.frmDragTest_MouseUp);
			this.Paint += new System.Windows.Forms.PaintEventHandler (this.frmDragTest_Paint);
			this.SizeChanged += new System.EventHandler (this.frmDragTest_SizeChanged);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler (this.frmDragTest_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler (this.frmDragTest_MouseMove);
			this.ResumeLayout (false);
			
		}
		
		#endregion
	}
}

