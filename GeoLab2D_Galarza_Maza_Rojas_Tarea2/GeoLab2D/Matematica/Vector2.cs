using System;
using System.Drawing;
namespace GeoLab2D.Matematica
{
    public struct Vector2
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public Vector2(double x, double y) { X = x; Y = y; }
        public double Magnitud { get { return Math.Sqrt(X * X + Y * Y); } }
        public double Direccion { get { return Math.Atan2(Y, X) * 180 / Math.PI; } }
        public PointF Punto { get { return new PointF((float)X, (float)Y); } }
        public static Vector2 operator +(Vector2 a, Vector2 b) { return new Vector2(a.X + b.X, a.Y + b.Y); }
        public static Vector2 operator -(Vector2 a, Vector2 b) { return new Vector2(a.X - b.X, a.Y - b.Y); }
        public static Vector2 operator *(Vector2 a, double k) { return new Vector2(a.X * k, a.Y * k); }
        public static double PuntoProducto(Vector2 a, Vector2 b) { return a.X * b.X + a.Y * b.Y; }
        public static double Determinante(Vector2 a, Vector2 b) { return a.X * b.Y - a.Y * b.X; }
        public override string ToString() { return string.Format("({0:0.###}, {1:0.###})", X, Y); }
    }
}
