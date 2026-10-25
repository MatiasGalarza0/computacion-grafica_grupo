using System.Drawing;
using System.Windows.Forms;
namespace GeoLab2D.Interfaz
{
    public class FormResultados : Form
    {
        public FormResultados(string titulo, string texto)
        {
            Text=titulo; Size=new Size(720,530); StartPosition=FormStartPosition.CenterParent;
            var salida=new TextBox { Multiline=true, ReadOnly=true, Dock=DockStyle.Fill, ScrollBars=ScrollBars.Both, WordWrap=false, Font=new Font("Consolas",10), Text=texto };
            Controls.Add(salida);
        }
    }
}
