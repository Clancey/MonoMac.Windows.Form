using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PhysicalDrag
{
	public partial class frmDragTest : Form
	{
		#region loading routines
		public frmDragTest ()
		{
			InitializeComponent ();
			//this.SetStyle(ControlStyles.ResizeRedraw, true);
			//this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			//this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
		}

		private void frmDragTest_Load (object sender, EventArgs e)
		{
			m_boxes = new List<Box> (4);
			m_boxes.Add (new Box (new Point (20, 20), new Point (150, 20), new Point (150, 30), new Point (20, 30)));
			m_boxes.Add (new Box (new Point (20, 40), new Point (150, 40), new Point (150, 80), new Point (20, 80)));
			m_boxes.Add (new Box (new Point (20, 100), new Point (150, 100), new Point (150, 200), new Point (20, 200)));
			m_boxes.Add (new Box (new Point (20, 220), new Point (150, 220), new Point (150, 500), new Point (20, 500)));
			
			Regen();
		}
		#endregion

		#region box logic
		private List<Box> m_boxes;

		private bool m_dragging = false;
		private int m_drag_index;
		private Point m_drag_start;

		private void frmDragTest_MouseDown (object sender, MouseEventArgs e)
		{
			
			//	MessageBox.Show (this, "Test Message", "Paint Exception", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			
			for (int i = 0; i < m_boxes.Count; i++)
			{
				if (m_boxes[i].Contains (e.Location))
				{
					m_dragging = true;
					m_drag_index = i;
					m_drag_start = e.Location;
				}
			}
			
		}
		private void frmDragTest_MouseMove (object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			if (!m_dragging)
			{
				return;
			}
			
			m_boxes[m_drag_index].Drag (m_drag_start, e.Location);
			m_drag_start = e.Location;
			
			Regen();
			//this.Refresh();
		}
		private void frmDragTest_MouseUp (object sender, MouseEventArgs e)
		{
			m_dragging = false;
		}
		#endregion

		#region drawing routines
		private BufferedGraphicsContext m_buffer;
		private BufferedGraphics m_graphics;

		private bool CreateBuffer ()
		{
			//Destroy the graphics object if it is not NULL
			if (m_graphics != null)
			{
				m_graphics.Dispose ();
				m_graphics = null;
			}
			
			//Check for client area validity
			if (ClientSize.Height < 2)
			{
				return false;
			}
			if (ClientSize.Width < 2)
			{
				return false;
			}
			
			//Initialize and allocate the Buffer
			m_buffer = BufferedGraphicsManager.Current;
			m_graphics = m_buffer.Allocate (CreateGraphics (), ClientRectangle);
			
			return true;
		}
		private void Regen ()
		{
			if (m_graphics != null)
			{
				if (m_graphics.Graphics.VisibleClipBounds != ClientRectangle)
				{
					CreateBuffer ();
				}
			}
			
			if (m_buffer == null)
			{
				CreateBuffer ();
			}
			if (m_buffer == null)
			{
				return;
			}
			if (m_graphics == null)
			{
				return;
			}
			
			Graphics G = m_graphics.Graphics;
			if (G == null)
			{
				return;
			}
			
			//G.Clear(this.BackColor);
			G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			if (m_boxes != null)
			{
				foreach (Box box in m_boxes)
				{
					box.Render (G, Color.YellowGreen, Color.DarkGreen);
				}
			}
			
			Refresh ();
		}

		protected override void OnPaint (PaintEventArgs e)
		{
			var G = e.Graphics;
			
		//	G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			if (m_boxes != null)
			{
				foreach (Box box in m_boxes)
				{
					
					box.Render (G, Color.YellowGreen, Color.DarkGreen);
				}
			}
		}
		protected override void OnPaintBackground (PaintEventArgs e)
		{
			//Don't do nuttin'
		}
		private void frmDragTest_SizeChanged (object sender, EventArgs e)
		{
			 Regen();
		}
		private void frmDragTest_Paint (object sender, PaintEventArgs e)
		{
			if (m_buffer == null)
			{
				return;
			}
			if (m_graphics == null)
			{
				return;
			}
			
			try
			{
				m_graphics.Render ();
			}
			catch (Exception ex)
			{
				MessageBox.Show (ex.Message);
				//this, ex.Message, "Paint Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion
	}
}
