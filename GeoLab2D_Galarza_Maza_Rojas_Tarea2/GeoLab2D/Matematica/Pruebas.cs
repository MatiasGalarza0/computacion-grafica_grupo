using System;
using System.Collections.Generic;
using System.Text;
using GeoLab2D.Figuras;
namespace GeoLab2D.Matematica
{
    public static class Pruebas
    {
        public static int Fallos { get; private set; }
        public static string Ejecutar()
        {
            Fallos=0;int total=0;var salida=new StringBuilder();
            Action<string,bool> verificar=(nombre,ok)=>{total++;if(!ok)Fallos++;salida.AppendLine((ok?"OK    ":"FALLO ")+nombre);};
            Func<double,double,bool> cerca=(x,y)=>Math.Abs(x-y)<1e-6;
            var u=new Vector2(3,4);var v=new Vector2(1,0);
            verificar("Magnitud (3,4) = 5",cerca(u.Magnitud,5));
            verificar("Dirección (0,1) = 90°",cerca(new Vector2(0,1).Direccion,90));
            verificar("Producto (3,4) · (1,0) = 3",cerca(Vector2.PuntoProducto(u,v),3));
            verificar("Determinante (3,4),(1,0) = -4",cerca(Vector2.Determinante(u,v),-4));
            var h=new Hexagono(10);
            verificar("Hexágono lado 10: área = 150√3",cerca(h.Area,150*Math.Sqrt(3)));
            verificar("Hexágono lado 10: perímetro = 60",cerca(h.Perimetro,60));
            verificar("Hexágono: seis partes",h.Piezas.Count==6);
            verificar("Triángulos equiláteros de lado 10",h.Piezas.TrueForAll(p=>cerca((p[0]-p[1]).Magnitud,10)&&cerca((p[1]-p[2]).Magnitud,10)&&cerca((p[2]-p[0]).Magnitud,10)));
            var flor=new Margarita(10);
            verificar("Margarita: cinco pentágonos",flor.Contornos.Count==5&&flor.Contornos.TrueForAll(p=>p.Count==5));
            verificar("Margarita: área = 25s²/(4tan36°)",cerca(flor.Area,25*100/(4*Math.Tan(Math.PI/5))));
            verificar("Margarita: perímetro = 25s",cerca(flor.Perimetro,250));
            verificar("Margarita: centro vacío",flor.Contiene(new Vector2(0,0))=="Fuera");
            var estrella=new Estrella(5,10,0.4);
            verificar("Estrella: diez vértices para cinco puntas",estrella.Contornos[0].Count==10);
            verificar("Estrella: área = N R r sen(π/N)",cerca(estrella.Area,5*10*4*Math.Sin(Math.PI/5)));
            verificar("Estrella: perímetro analítico",cerca(estrella.Perimetro,10*Math.Sqrt(116-80*Math.Cos(Math.PI/5))));
            var cuadrado=new List<Vector2>{new Vector2(0,0),new Vector2(4,0),new Vector2(4,4),new Vector2(0,4)};
            verificar("Punto interior",Geometria.Pertenencia(cuadrado,new Vector2(2,2))=="Dentro");
            verificar("Punto exterior",Geometria.Pertenencia(cuadrado,new Vector2(5,2))=="Fuera");
            verificar("Punto en borde",Geometria.Pertenencia(cuadrado,new Vector2(0,2))=="Borde");
            verificar("Punto en vértice",Geometria.Pertenencia(cuadrado,new Vector2(4,4))=="Borde");
            var a=new Vector2(0,0);var b=new Vector2(4,0);double t;
            var q=Geometria.Proyeccion(a,b,new Vector2(2,3),out t);
            verificar("Proyección interior Q=(2,0), t=0.5",cerca(q.X,2)&&cerca(q.Y,0)&&cerca(t,0.5));
            q=Geometria.Proyeccion(a,b,new Vector2(8,3),out t);verificar("Proyección fuera: extremo B",cerca(q.X,4)&&cerca(t,1));
            q=Geometria.Proyeccion(a,b,new Vector2(-2,3),out t);verificar("Proyección fuera: extremo A",cerca(q.X,0)&&cerca(t,0));
            q=Geometria.Proyeccion(a,a,u,out t);verificar("Segmento degenerado Q=A",cerca(q.Magnitud,0));
            verificar("Orientación izquierda",Geometria.Orientacion(a,b,new Vector2(2,3)).StartsWith("Izquierda"));
            verificar("Orientación derecha al invertir A y B",Geometria.Orientacion(b,a,new Vector2(2,3)).StartsWith("Derecha"));
            verificar("Colinealidad",Geometria.Orientacion(a,b,new Vector2(7,0))=="Sobre la recta");
            verificar("Orientación A=B indefinida",Geometria.Orientacion(a,a,u).StartsWith("Indefinida"));
            q=Matriz3.Rotacion(90).Aplicar(new Vector2(1,0));verificar("Rotación 90°: (1,0) → (0,1)",cerca(q.X,0)&&cerca(q.Y,1));
            q=(Matriz3.Traslacion(5,2)*Matriz3.Escala(2,3)).Aplicar(new Vector2(1,1));verificar("Orden T·S: (1,1) → (7,5)",cerca(q.X,7)&&cerca(q.Y,5));
            double area=h.Area,perimetro=h.Perimetro;
            h.Transformar(Matriz3.Traslacion(30,-20));h.Transformar(Matriz3.EnCentro(Matriz3.Rotacion(37),h.Centro));
            verificar("Traslación y rotación conservan área",cerca(h.Area,area));verificar("Traslación y rotación conservan perímetro",cerca(h.Perimetro,perimetro));
            verificar("Rotación local conserva centro",cerca(h.Centro.X,30)&&cerca(h.Centro.Y,-20));
            h.Transformar(Matriz3.EnCentro(Matriz3.Escala(2,2),h.Centro));
            verificar("Escala 2: área ×4",cerca(h.Area,area*4));verificar("Escala 2: perímetro ×2",cerca(h.Perimetro,perimetro*2));
            h.Transformar(Matriz3.EnCentro(Matriz3.Escala(2,3),h.Centro));verificar("Escala no uniforme: área ×6",cerca(h.Area,area*24));
            bool rechaza=false;try{new Hexagono(0);}catch(ArgumentException){rechaza=true;}verificar("Rechaza lado cero",rechaza);
            rechaza=false;try{new Estrella(3,10,0.8);}catch(ArgumentException){rechaza=true;}verificar("Rechaza estrella sin concavidad",rechaza);
            salida.AppendLine();salida.AppendLine(string.Format("Resultado: {0}/{1} correctas. Fallos: {2}.",total-Fallos,total,Fallos));return salida.ToString();
        }
    }
}
