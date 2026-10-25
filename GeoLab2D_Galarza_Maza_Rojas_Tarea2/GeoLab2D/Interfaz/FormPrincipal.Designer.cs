using System.Drawing;
using System.Windows.Forms;
namespace GeoLab2D.Interfaz
{
    partial class FormPrincipal
    {
        private System.ComponentModel.IContainer components = null;
        private TabControl pestañas;
        private Panel desplazamiento;
        private Lienzo lienzo;
        private Label estado;
        protected override void Dispose(bool disposing) { if(disposing && components!=null)components.Dispose(); base.Dispose(disposing); }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pestañas=new TabControl(); desplazamiento=new Panel(); lienzo=new Lienzo(); estado=new Label();
            SuspendLayout();
            AutoScaleMode=AutoScaleMode.None;
            Text="GeoLab 2D — Diseño y análisis geométrico";
            Font=new Font("Segoe UI",9F); BackColor=Color.FromArgb(241,243,246);
            ClientSize=new Size(1190,740); MinimumSize=new Size(930,680); StartPosition=FormStartPosition.CenterScreen;
            pestañas.Dock=DockStyle.Left; pestañas.Width=340;
            desplazamiento.Dock=DockStyle.Fill; desplazamiento.AutoScroll=true; desplazamiento.BackColor=Color.FromArgb(218,223,230);
            lienzo.Location=new Point(12,12); lienzo.Size=new Size(800,600); lienzo.BackColor=Color.White;
            desplazamiento.Controls.Add(lienzo);
            estado.Dock=DockStyle.Bottom; estado.Height=45; estado.Padding=new Padding(12,8,0,0); estado.BackColor=Color.FromArgb(226,234,242);
            Controls.Add(desplazamiento); Controls.Add(pestañas); Controls.Add(estado);
            ResumeLayout(false);
        }
    }
}
