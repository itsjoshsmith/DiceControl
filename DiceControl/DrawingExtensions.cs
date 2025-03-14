﻿using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceControl
{
  public static class DrawingExtensions
  {
    /// <summary>
    /// Creates a path for drawing the rounded rectangle
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
      int diameter = radius * 2;
      Size size = new Size(diameter, diameter);
      Rectangle arc = new Rectangle(bounds.Location, size);
      GraphicsPath path = new GraphicsPath();

      if (radius == 0)
      {
        path.AddRectangle(bounds);
        return path;
      }

      path.AddArc(arc, 180, 90);
      arc.X = bounds.Right - diameter;
      path.AddArc(arc, 270, 90);
      arc.Y = bounds.Bottom - diameter;
      path.AddArc(arc, 0, 90);
      arc.X = bounds.Left;
      path.AddArc(arc, 90, 90);

      path.CloseFigure();
      return path;
    }

    /// <summary>
    /// Extension: Draws a rounded corner rectangle
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="pen"></param>
    /// <param name="bounds"></param>
    /// <param name="cornerRadius"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
    {
      if (graphics == null)
        throw new ArgumentNullException("graphics");
      if (pen == null)
        throw new ArgumentNullException("pen");

      using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
      {
        graphics.DrawPath(pen, path);
      }
    }

    /// <summary>
    /// /// Extension: Fills a rounded corner rectangle
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="brush"></param>
    /// <param name="bounds"></param>
    /// <param name="cornerRadius"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
    {
      if (graphics == null)
        throw new ArgumentNullException("graphics");
      if (brush == null)
        throw new ArgumentNullException("brush");

      using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
      {
        graphics.FillPath(brush, path);
      }
    }
  }
}
