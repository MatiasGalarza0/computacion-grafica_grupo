using System;
namespace GeoLab2D.Matematica
{
    public class Matriz3
    {
        private readonly double[,] datos;
        public double this[int i, int j] { get { return datos[i,j]; } }
        private Matriz3(double[,] valores) { datos = valores; }
        public static Matriz3 Identidad() { return new Matriz3(new double[,] {{1,0,0},{0,1,0},{0,0,1}}); }
        public static Matriz3 Traslacion(double x, double y) { return new Matriz3(new double[,] {{1,0,x},{0,1,y},{0,0,1}}); }
        public static Matriz3 Escala(double x, double y) { return new Matriz3(new double[,] {{x,0,0},{0,y,0},{0,0,1}}); }
        public static Matriz3 Rotacion(double grados)
        {
            double r = grados * Math.PI / 180, c = Math.Cos(r), s = Math.Sin(r);
            return new Matriz3(new double[,] {{c,-s,0},{s,c,0},{0,0,1}});
        }
        public static Matriz3 operator *(Matriz3 a, Matriz3 b)
        {
            double[,] d = new double[3,3];
            for (int i=0;i<3;i++) for(int j=0;j<3;j++) for(int k=0;k<3;k++) d[i,j]+=a[i,k]*b[k,j];
            return new Matriz3(d);
        }
        public Vector2 Aplicar(Vector2 p) { return new Vector2(datos[0,0]*p.X+datos[0,1]*p.Y+datos[0,2], datos[1,0]*p.X+datos[1,1]*p.Y+datos[1,2]); }
        public static Matriz3 EnCentro(Matriz3 operacion, Vector2 c) { return Traslacion(c.X,c.Y)*operacion*Traslacion(-c.X,-c.Y); }
        public override string ToString()
        {
            return string.Format("[{0,8:0.###} {1,8:0.###} {2,8:0.###}]\r\n[{3,8:0.###} {4,8:0.###} {5,8:0.###}]\r\n[       0        0        1]",datos[0,0],datos[0,1],datos[0,2],datos[1,0],datos[1,1],datos[1,2]);
        }
    }
}
