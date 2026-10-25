using System;
using System.Windows.Forms;
using GeoLab2D.Interfaz;
namespace GeoLab2D
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false); Application.Run(new FormPrincipal());
        }
    }
}
