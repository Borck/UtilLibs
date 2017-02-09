using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Utils.Numerics {
  public static class GeometryExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(this Size size, int width, int height) {
      return (size.Width==width)&&(size.Height==height);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point ToPoint(this PointF point, Size norm) {
      return new Point((int)(point.X*norm.Width), (int)(point.Y*norm.Height));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point ToPoint(this PointF point, int normWidth, int normHeight) {
      return new Point((int)(point.X*normWidth), (int)(point.Y*normHeight));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point ToPoint(this PointF point, double normWidth, double normHeight) {
      return new Point((int)(point.X*normWidth), (int)(point.Y*normHeight));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Scale(this PointF point, float scaleX, float scaleY) {
      return new PointF(point.X*scaleX, point.Y*scaleY);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Scale(this PointF point, double scaleX, double scaleY) {
      return new PointF((float)(point.X*scaleX), (float)(point.Y*scaleY));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point Round(this PointF point) {
      return new Point((int)Math.Round(point.X), (int)Math.Round(point.Y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point Truncate(this PointF point) {
      return new Point((int)point.X, (int)point.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point Ceiling(this PointF point) {
      return new Point((int)Math.Ceiling(point.X), (int)Math.Ceiling(point.Y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF ToPointF(this Point point, Size norm) {
      return new PointF(
        (float)(point.X/norm.Width),
        (float)(point.Y/norm.Height));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF ToPointF(this Point point, int normWidth, int normHeight) {
      return new PointF((float)point.X/normWidth, (float)point.Y/normHeight);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size ToSize(this SizeF size, Size norm) {
      return new Size((int)(size.Width*norm.Width), (int)(size.Height*norm.Height));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size ToSize(this SizeF size, int normWidth, int normHeight) {
      return new Size((int)(size.Width*normWidth), (int)(size.Height*normHeight));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size ToSize(this SizeF size, double normWidth, double normHeight) {
      return new Size((int)(size.Width*normWidth), (int)(size.Height*normHeight));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size Round(this SizeF size) {
      return new Size((int)(size.Width+.5), (int)(size.Height+.5));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size Truncate(this SizeF size) {
      return new Size((int)size.Width, (int)size.Height);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size Ceiling(this SizeF size) {
      return new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF Scale(this SizeF size, float scaleX, float scaleY) {
      return new SizeF(size.Width*scaleX, size.Height*scaleY);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF Scale(this SizeF size, double scaleX, double scaleY) {
      return new SizeF((float)(size.Width*scaleX), (float)(size.Height*scaleY));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF ToSizeF(this Size size, Size norm) {
      return new SizeF(
        (float)(size.Width/norm.Width),
        (float)(size.Height/norm.Height));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SizeF ToSizeF(this Size size, int normWidth, int normHeight) {
      return new SizeF((float)size.Width/normWidth, (float)size.Height/normHeight);
    }
  }
}