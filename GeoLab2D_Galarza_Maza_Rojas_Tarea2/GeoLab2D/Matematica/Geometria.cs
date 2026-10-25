using System;
using System.Collections.Generic;
namespace GeoLab2D.Matematica
{
    public static class Geometria
    {
        public const double Epsilon = 1e-7;
        public static List<Vector2> Regular(int n, double radio, Vector2 centro, double angulo)
        {
            if(n<3 || radio<=0) throw new ArgumentException("El polígono requiere al menos 3 lados y radio positivo.");
            List<Vector2> puntos = new List<Vector2>();
            for(int i=0;i<n;i++) { double t=(angulo+360.0*i/n)*Math.PI/180; puntos.Add(centro+new Vector2(radio*Math.Cos(t),radio*Math.Sin(t))); }
            return puntos;
        }
        public static double Area(IList<Vector2> p)
        {
            double suma=0; for(int i=0;i<p.Count;i++) suma+=Vector2.Determinante(p[i],p[(i+1)%p.Count]); return Math.Abs(suma)/2;
        }
        public static double Perimetro(IList<Vector2> p)
        {
            double suma=0; for(int i=0;i<p.Count;i++) suma+=(p[(i+1)%p.Count]-p[i]).Magnitud; return suma;
        }
        public static Vector2 Proyeccion(Vector2 a, Vector2 b, Vector2 p, out double t)
        {
            Vector2 v=b-a; double d=Vector2.PuntoProducto(v,v);
            if(d<=Epsilon*Epsilon) { t=0; return a; }
            t=Math.Max(0,Math.Min(1,Vector2.PuntoProducto(p-a,v)/d)); return a+v*t;
        }
        public static string Orientacion(Vector2 a, Vector2 b, Vector2 p)
        {
            Vector2 v=b-a;
            if(v.Magnitud<=Epsilon) return "Indefinida: A y B coinciden";
            double d=Vector2.Determinante(v,p-a);
            if(Math.Abs(d)<=Epsilon*v.Magnitud) return "Sobre la recta";
            return d>0 ? "Izquierda de A hacia B" : "Derecha de A hacia B";
        }
        public static string Pertenencia(IList<Vector2> poligono, Vector2 p)
        {
            bool dentro=false;
            for(int i=0,j=poligono.Count-1;i<poligono.Count;j=i++)
            {
                Vector2 a=poligono[j],b=poligono[i]; double t;
                if((Proyeccion(a,b,p,out t)-p).Magnitud<=Epsilon) return "Borde";
                if((a.Y>p.Y)!=(b.Y>p.Y) && p.X<(b.X-a.X)*(p.Y-a.Y)/(b.Y-a.Y)+a.X) dentro=!dentro;
            }
            return dentro ? "Dentro" : "Fuera";
        }
    }
}
