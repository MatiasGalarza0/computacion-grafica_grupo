using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using GeoLab2D.Matematica;
using GeoLab2D.Figuras;
namespace GeoLab2D.Interfaz
{
    public partial class FormPrincipal : Form
    {
        private Figura figura;
        private ComboBox tipo,region;
        private NumericUpDown dimension,puntas,interior,cx,cy,angulo,tx,ty,giro,sx,sy,ax,ay,bx,by,px,py,ux,uy,vx,vy;
        private TextBox propiedades,resultadoPuntos,resultadoVectores;
        private CheckBox mostrarPuntos;
        private Vector2 a,b,p,q;
        private bool analisisVisible;
        public FormPrincipal()
        {
            InitializeComponent(); ConstruirControles();
            lienzo.Paint+=(s,e)=>Dibujar(e.Graphics);
            lienzo.MouseClick+=(s,e)=> { px.Value=Math.Max(px.Minimum,Math.Min(px.Maximum,e.X-400)); py.Value=Math.Max(py.Minimum,Math.Min(py.Maximum,300-e.Y)); AnalizarPuntos(); };
            Crear();
        }
        private FlowLayoutPanel Pagina(string nombre)
        {
            var pagina=new TabPage(nombre); var f=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.TopDown,WrapContents=false,AutoScroll=true,Padding=new Padding(10)};
            pagina.Controls.Add(f); pestañas.TabPages.Add(pagina); return f;
        }
        private void Texto(FlowLayoutPanel f,string texto) { f.Controls.Add(new Label{Text=texto,AutoSize=false,Width=290,Height=42}); }
        private NumericUpDown Numero(FlowLayoutPanel f,string titulo,decimal valor,decimal min,decimal max,int decimales=2)
        {
            var fila=new Panel{Width=290,Height=30}; var label=new Label{Text=titulo,Location=new Point(0,5),Width=175};
            var n=new NumericUpDown{Location=new Point(177,1),Width=106,Minimum=min,Maximum=max,DecimalPlaces=decimales,Value=valor,Increment=decimales==0?1:0.5M};
            fila.Controls.Add(label); fila.Controls.Add(n); f.Controls.Add(fila); return n;
        }
        private Button Boton(FlowLayoutPanel f,string texto,Action accion)
        {
            var boton=new Button{Text=texto,Width=287,Height=32,BackColor=Color.White,FlatStyle=FlatStyle.Flat};
            boton.Click+=(s,e)=>Seguro(accion); f.Controls.Add(boton); return boton;
        }
        private TextBox Salida(FlowLayoutPanel f,int alto)
        {
            var t=new TextBox{Multiline=true,ReadOnly=true,Width=287,Height=alto,ScrollBars=ScrollBars.Vertical,BackColor=Color.White,Font=new Font("Consolas",9)}; f.Controls.Add(t); return t;
        }
        private double D(NumericUpDown n) { return (double)n.Value; }
        private void Seguro(Action accion) { try { accion(); } catch(Exception ex) { MessageBox.Show(this,ex.Message,"Revisar datos",MessageBoxButtons.OK,MessageBoxIcon.Warning); } }
        private void ConstruirControles()
        {
            var f=Pagina("Figuras"); Texto(f,"Elige una figura y modifica sus medidas. Una unidad equivale a un píxel.");
            tipo=new ComboBox{Width=287,DropDownStyle=ComboBoxStyle.DropDownList}; tipo.Items.AddRange(new object[]{"Hexágono de 6 colores","Margarita de 5 pentágonos","Estrella de N puntas"}); tipo.SelectedIndex=0; f.Controls.Add(tipo);
            dimension=Numero(f,"Lado / radio exterior",100,1,400); puntas=Numero(f,"Número de puntas N",5,3,30,0); interior=Numero(f,"Radio interior / exterior",0.45M,0.05M,0.95M);
            cx=Numero(f,"Centro X",0,-10000,10000); cy=Numero(f,"Centro Y",0,-10000,10000); angulo=Numero(f,"Ángulo inicial (grados)",0,-360,360);
            tipo.SelectedIndexChanged+=(s,e)=>{puntas.Enabled=interior.Enabled=tipo.SelectedIndex==2;}; puntas.Enabled=interior.Enabled=false;
            Boton(f,"Crear / restablecer figura",Crear);
            region=new ComboBox{Width=287,DropDownStyle=ComboBoxStyle.DropDownList}; f.Controls.Add(region);
            Boton(f,"Cambiar color de la parte",()=> { using(var dialogo=new ColorDialog()){dialogo.Color=figura.Colores[region.SelectedIndex];if(dialogo.ShowDialog(this)==DialogResult.OK){figura.Colores[region.SelectedIndex]=dialogo.Color;lienzo.Invalidate();}} });
            propiedades=Salida(f,105);
            Boton(f,"Ejecutar pruebas matemáticas",()=>{using(var ventana=new FormResultados("Pruebas matemáticas",Pruebas.Ejecutar()))ventana.ShowDialog(this);});
            f=Pagina("Matrices"); Texto(f,"Las operaciones se acumulan. Rotación y escala se aplican respecto al centro de la figura.");
            tx=Numero(f,"Desplazamiento X",40,-10000,10000); ty=Numero(f,"Desplazamiento Y",20,-10000,10000);
            Boton(f,"Trasladar",()=>Aplicar(Matriz3.Traslacion(D(tx),D(ty))));
            giro=Numero(f,"Rotación (grados)",30,-360,360); Boton(f,"Rotar",()=>Aplicar(Matriz3.EnCentro(Matriz3.Rotacion(D(giro)),figura.Centro)));
            sx=Numero(f,"Escala X",1.2M,0.05M,10); sy=Numero(f,"Escala Y",1.2M,0.05M,10);
            Boton(f,"Escalar",()=>Aplicar(Matriz3.EnCentro(Matriz3.Escala(D(sx),D(sy)),figura.Centro)));
            Texto(f,"Escala X distinta de Y deforma los polígonos regulares. Crear restablece las transformaciones.");
            f=Pagina("Vectores"); Texto(f,"Introduce las componentes de los vectores u y v en coordenadas cartesianas.");
            ux=Numero(f,"u X",3,-10000,10000); uy=Numero(f,"u Y",4,-10000,10000); vx=Numero(f,"v X",1,-10000,10000); vy=Numero(f,"v Y",0,-10000,10000);
            Boton(f,"Calcular vectores",CalcularVectores); resultadoVectores=Salida(f,240);
            f=Pagina("Puntos"); Texto(f,"A → B define el segmento. Haz clic en el lienzo para seleccionar el punto P.");
            ax=Numero(f,"A X",-150,-10000,10000); ay=Numero(f,"A Y",-100,-10000,10000); bx=Numero(f,"B X",150,-10000,10000); by=Numero(f,"B Y",-100,-10000,10000); px=Numero(f,"P X",50,-10000,10000); py=Numero(f,"P Y",50,-10000,10000);
            mostrarPuntos=new CheckBox{Text="Mostrar A, B, P y proyección Q",Checked=true,Width=287}; mostrarPuntos.CheckedChanged+=(s,e)=>lienzo.Invalidate(); f.Controls.Add(mostrarPuntos);
            Boton(f,"Analizar orientación y proyección",AnalizarPuntos); resultadoPuntos=Salida(f,205);
        }
        private void Crear()
        {
            Figura nueva=tipo.SelectedIndex==0?(Figura)new Hexagono(D(dimension)):tipo.SelectedIndex==1?(Figura)new Margarita(D(dimension)):new Estrella((int)puntas.Value,D(dimension),D(interior));
            nueva.Transformar(Matriz3.Traslacion(D(cx),D(cy))*Matriz3.Rotacion(D(angulo))); figura=nueva;
            region.Items.Clear(); for(int i=0;i<figura.Piezas.Count;i++)region.Items.Add("Parte "+(i+1));region.SelectedIndex=0;
            Actualizar();
        }
        private void Aplicar(Matriz3 matriz) { figura.Transformar(matriz); Actualizar(); }
        private void Actualizar()
        {
            propiedades.Text=string.Format("Área: {0:0.###} u²\r\nPerímetro: {1:0.###} u\r\nCentro: {2}\r\nPartes: {3}",figura.Area,figura.Perimetro,figura.Centro,figura.Piezas.Count);
            estado.Text="Origen: centro del lienzo | X → derecha | Y → arriba | Lienzo: 800 × 600 px";
            if(figura.Contornos.SelectMany(x=>x).Any(v=>Math.Abs(v.X)>400||Math.Abs(v.Y)>300)) estado.Text+=" | Figura parcialmente fuera del lienzo";
            if(analisisVisible)AnalizarPuntos(); lienzo.Invalidate();
        }
        private void CalcularVectores()
        {
            Vector2 u=new Vector2(D(ux),D(uy)),v=new Vector2(D(vx),D(vy)); double producto=Vector2.PuntoProducto(u,v);
            string direccionU=u.Magnitud==0?"Indefinida":u.Direccion.ToString("0.###")+"°",direccionV=v.Magnitud==0?"Indefinida":v.Direccion.ToString("0.###")+"°";
            resultadoVectores.Text=string.Format("|u| = {0:0.###}\r\n|v| = {1:0.###}\r\nDirección u: {2}\r\nDirección v: {3}\r\nu · v = {4:0.###}\r\ndet(u,v) = {5:0.###}",u.Magnitud,v.Magnitud,direccionU,direccionV,producto,Vector2.Determinante(u,v));
        }
        private void AnalizarPuntos()
        {
            a=new Vector2(D(ax),D(ay));b=new Vector2(D(bx),D(by));p=new Vector2(D(px),D(py));double t;q=Geometria.Proyeccion(a,b,p,out t);analisisVisible=true;
            resultadoPuntos.Text=string.Format("{0}\r\ndet(AB, AP): {1:0.###}\r\nQ: {2}\r\nt limitado: {3:0.###}\r\nDistancia P–Q: {4:0.###}\r\nLongitud AB: {5:0.###}\r\nP en figura: {6}",Geometria.Orientacion(a,b,p),Vector2.Determinante(b-a,p-a),q,t,(p-q).Magnitud,(b-a).Magnitud,figura.Contiene(p));
            lienzo.Invalidate();
        }
        private PointF Pantalla(Vector2 v) { return new PointF((float)(400+v.X),(float)(300-v.Y)); }
        private void Dibujar(Graphics g)
        {
            g.Clear(Color.White);g.SmoothingMode=SmoothingMode.AntiAlias;
            using(var cuadricula=new Pen(Color.FromArgb(235,238,242)))
            { for(int x=0;x<=800;x+=25)g.DrawLine(cuadricula,x,0,x,600);for(int y=0;y<=600;y+=25)g.DrawLine(cuadricula,0,y,800,y); }
            using(var eje=new Pen(Color.Silver)){g.DrawLine(eje,400,0,400,600);g.DrawLine(eje,0,300,800,300);}
            using(var letra=new Font("Segoe UI",8))using(var tinta=new SolidBrush(Color.DimGray))
            { g.DrawString("X",letra,tinta,780,303);g.DrawString("Y",letra,tinta,405,5);g.DrawString("0",letra,tinta,403,303);for(int x=-300;x<=300;x+=100)if(x!=0)g.DrawString(x.ToString(),letra,tinta,x+400,303); }
            if(figura!=null)
            {
                var partes=figura.Piezas;
                for(int i=0;i<partes.Count;i++)using(var brocha=new SolidBrush(figura.Colores[i]))using(var pluma=new Pen(Color.FromArgb(55,60,68),1.6F))
                { PointF[] puntos=partes[i].Select(Pantalla).ToArray();g.FillPolygon(brocha,puntos);g.DrawPolygon(pluma,puntos); }
            }
            if(analisisVisible&&mostrarPuntos.Checked)
            {
                using(var segmento=new Pen(Color.FromArgb(20,70,120),2))g.DrawLine(segmento,Pantalla(a),Pantalla(b));
                using(var proyeccion=new Pen(Color.Firebrick,2)){proyeccion.DashStyle=DashStyle.Dash;g.DrawLine(proyeccion,Pantalla(p),Pantalla(q));}
                Marcar(g,a,"A",Color.Navy);Marcar(g,b,"B",Color.Navy);Marcar(g,p,"P",Color.Firebrick);Marcar(g,q,"Q",Color.DarkGreen);
            }
        }
        private void Marcar(Graphics g,Vector2 punto,string nombre,Color color)
        {
            var s=Pantalla(punto);using(var brocha=new SolidBrush(color))using(var fuente=new Font("Segoe UI",10,FontStyle.Bold))
            {g.FillEllipse(brocha,s.X-4,s.Y-4,8,8);g.DrawString(nombre,fuente,brocha,s.X+6,s.Y-20);}
        }
    }
}
