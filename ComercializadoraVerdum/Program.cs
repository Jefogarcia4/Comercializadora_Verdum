using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComercializadoraVerdum
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            int numeroInstancias = 1;

            List<FrmHome> instanciasFrmHome = new List<FrmHome>();

            for (int i = 0; i < numeroInstancias; i++)
            {
                Historial historial = new Historial();
                FrmHome frmHome = new FrmHome(historial);
                instanciasFrmHome.Add(frmHome);

                frmHome.Show();
            }

            Application.Run();
        }
    }

}
