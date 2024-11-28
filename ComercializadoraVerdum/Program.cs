using ComercializadoraVerdum;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            int numeroInstancias = configuration.GetValue<int>("ConnectionStrings:NumeroInstancias");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            List<FrmHome> instanciasFrmHome = new List<FrmHome>();

            for (int i = 0; i < numeroInstancias; i++)
            {
                FrmHome frmHome = new FrmHome();
                instanciasFrmHome.Add(frmHome);
                
                frmHome.Show();
            }

            Application.Run();
        }
    }
}
