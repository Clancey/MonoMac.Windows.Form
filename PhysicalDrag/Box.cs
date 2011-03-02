using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PhysicalDrag
{
  public class Box
  {
    /// <summary>
    /// corner points
    /// </summary>
    private PointF[] m_C = new PointF[4];
    private GraphicsPath m_outline;

    #region constructors
    public Box() { }
    public Box(Point nA, Point nB, Point nC, Point nD)
    {
      Set(nA, nB, nC, nD);
    }
    public Box(PointF nA, PointF nB, PointF nC, PointF nD)
    {
      Set(nA, nB, nC, nD);
    }
    #endregion

    #region shape properties
    public PointF A { get { return m_C[0]; } }
    public PointF B { get { return m_C[1]; } }
    public PointF C { get { return m_C[2]; } }
    public PointF D { get { return m_C[3]; } }

    /// <summary>
    /// Center of mass
    /// </summary>
    public PointF Center
    {
      get
      {
        return new PointF(0.5f * (A.X + C.X),
                          0.5f * (A.Y + C.Y));
      }
    }

    /// <summary>
    /// Test for inclusion of a point.
    /// </summary>
    /// <param name="pt">Point to test</param>
    public bool Contains(Point pt)
    {
      return m_outline.IsVisible(pt);
    }

    /// <summary>
    /// Override all corners.
    /// </summary>
    public void Set(PointF nA, PointF nB, PointF nC, PointF nD)
    {
      m_C[0] = nA;
      m_C[1] = nB;
      m_C[2] = nC;
      m_C[3] = nD;

      RecomputeOutline();
    }

    private void RecomputeOutline()
    {
      m_outline = new GraphicsPath();
      m_outline.AddLine(A, B);
      m_outline.AddLine(B, C);
      m_outline.AddLine(C, D);
      m_outline.CloseFigure();
    }
    #endregion

    #region physical simulations
    /// <summary>
    /// Perform a single drag operation.
    /// </summary>
    /// <param name="from">From point.</param>
    /// <param name="to">To point</param>
    public void Drag(Point from, Point to)
    {
      PointF M = Center;

      double angle_before = Math.Atan2(from.Y - M.Y, from.X - M.X);
      double angle_after = Math.Atan2(to.Y - M.Y, to.X - M.X);
      double angle_diff = 180.0 * ((angle_after - angle_before) / Math.PI);

      if (angle_diff > 180.0) { angle_diff -= 360.0; }
      else if (angle_diff < -180.0) { angle_diff += 360.0; }

      float dx = to.X - from.X;
      float dy = to.Y - from.Y;

      float dd = Distance(M, from);

      if (dd < 10.0F)
      {
        angle_diff = 0.0f;
      }
      else
      {
        float radius = 0.5f * Distance(A, C);
        dd = (dd*dd) / (radius * radius);

        angle_diff *= dd;
      }

      //apply translation
      Matrix T0 = new Matrix();
      T0.Translate(dx, dy);
      T0.TransformPoints(m_C);

      //apply rotation
      Matrix T1 = new Matrix();
      T1.RotateAt((float)(angle_diff), to);
      T1.TransformPoints(m_C);

      RecomputeOutline();
    }

    /// <summary>
    /// Distance squared between two points.
    /// </summary>
    private float Distance(PointF A, PointF B)
    {
      return (float)Math.Sqrt((A.X - B.X) * (A.X - B.X) + (A.Y - B.Y) * (A.Y - B.Y));
    }
    #endregion

    /// <summary>
    /// Render the object in a Graphics context.
    /// </summary>
    /// <param name="G">Graphics object to draw with.</param>
    /// <param name="bg">Background color.</param>
    /// <param name="fg">Outline color.</param>
    public void Render(Graphics G, Color bg, Color fg)
    {
      if (m_outline == null) { return; }

      Pen pen = new Pen(fg, 2.0f);
      Brush brush = new SolidBrush(bg);

      G.FillPath(brush, m_outline);
      G.DrawPath(pen, m_outline);

      PointF c = Center;
	  G.DrawString("Box",new Font(FontFamily.GenericSerif,10f,GraphicsUnit.Pixel),new SolidBrush(Color.Blue),c);

      G.DrawLine(pen, c.X - 3, c.Y - 3, c.X + 3, c.Y + 3);
      G.DrawLine(pen, c.X - 3, c.Y + 3, c.X + 3, c.Y - 3);

      pen.Dispose();
      brush.Dispose();
    }
  }
}