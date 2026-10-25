using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GeoLab2D.Matematica;
namespace GeoLab2D.Figuras
{
    public abstract class Figura
    {
        protected readonly List<List<Vector2>> piezas = new List<List<Vector2>>();
        protected readonly List<List<Vector2>> contornos = new List<List<Vector2>>();
        public Color[] Colores { get; private set; }
        public Matriz3 Transformacion { get; private set; }
        public abstract string Nombre { get; }
        protected Figura()
        {
            Transformacion=Matriz3.Identidad();
            Colores=new Color[]{Color.Coral,Color.Gold,Color.MediumSeaGreen,Color.DeepSkyBlue,Color.MediumPurple,Color.HotPink};
        }
        public Vector2 Centro { get { return Transformacion.Aplicar(new Vector2(0,0)); } }
        public List<List<Vector2>> Piezas { get { return piezas.Select(p=>p.Select(Transformacion.Aplicar).ToList()).ToList(); } }
        public List<List<Vector2>> Contornos { get { return contornos.Select(p=>p.Select(Transformacion.Aplicar).ToList()).ToList(); } }
        public double Area { get { return Contornos.Sum(p=>Geometria.Area(p)); } }
        public double Perimetro { get { return Contornos.Sum(p=>Geometria.Perimetro(p)); } }
        public void Transformar(Matriz3 m)
        {
            Matriz3 candidata=m*Transformacion;
            foreach(var p in contornos.SelectMany(x=>x))
            {
                Vector2 q=candidata.Aplicar(p);
                if(double.IsNaN(q.X)||double.IsInfinity(q.X)||double.IsNaN(q.Y)||double.IsInfinity(q.Y)||Math.Abs(q.X)>1000000||Math.Abs(q.Y)>1000000)
                    throw new ArgumentException("La transformación excede el límite de un millón de unidades.");
            }
            foreach(var contorno in contornos)
                for(int i=0;i<contorno.Count;i++)
                    if((candidata.Aplicar(contorno[i])-candidata.Aplicar(contorno[(i+1)%contorno.Count])).Magnitud<0.0001)
                        throw new ArgumentException("La escala vuelve la figura demasiado pequeña. Restablezca la figura.");
            Transformacion=candidata;
        }
        public string Contiene(Vector2 p)
        {
            bool borde=false;
            foreach(var contorno in Contornos) { string r=Geometria.Pertenencia(contorno,p); if(r=="Dentro")return r; if(r=="Borde")borde=true; }
            return borde?"Borde":"Fuera";
        }
    }
    public class Hexagono : Figura
    {
        public override string Nombre { get { return "Hexágono de 6 colores"; } }
        public Hexagono(double lado)
        {
            var vertices=Geometria.Regular(6,lado,new Vector2(0,0),0); contornos.Add(vertices);
            for(int i=0;i<6;i++) piezas.Add(new List<Vector2>{new Vector2(0,0),vertices[i],vertices[(i+1)%6]});
        }
    }
    public class Margarita : Figura
    {
        public override string Nombre { get { return "Margarita de 5 pentágonos"; } }
        public Margarita(double lado)
        {
            if(lado<=0)throw new ArgumentException("El lado debe ser positivo.");
            double radio=lado/(2*Math.Sin(Math.PI/5)), apotema=radio*Math.Cos(Math.PI/5);
            for(int i=0;i<5;i++)
            {
                double grados=90+i*72, t=grados*Math.PI/180;
                Vector2 centro=new Vector2(2*apotema*Math.Cos(t),2*apotema*Math.Sin(t));
                var p=Geometria.Regular(5,radio,centro,grados+180-36); piezas.Add(p); contornos.Add(p);
            }
        }
    }
    public class Estrella : Figura
    {
        public override string Nombre { get { return "Estrella de puntas configurables"; } }
        public Estrella(int puntas, double radio, double proporcion)
        {
            if(puntas<3||puntas>30||radio<=0||proporcion<=0||proporcion>=Math.Cos(Math.PI/puntas)) throw new ArgumentException("Use 3 a 30 puntas, radio positivo y radio interior menor que R·cos(π/N).");
            var p=new List<Vector2>();
            for(int i=0;i<2*puntas;i++) { double t=(90+180.0*i/puntas)*Math.PI/180, r=i%2==0?radio:radio*proporcion; p.Add(new Vector2(r*Math.Cos(t),r*Math.Sin(t))); }
            piezas.Add(p); contornos.Add(p);
        }
    }
}
