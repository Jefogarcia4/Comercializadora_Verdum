using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComercializadoraVerdum
{

    public partial class IngresoPagos : Form
    {
        public decimal ValorEfectivo { get; private set; } = 0;
        public decimal ValorTransferencia { get; private set; } = 0;
        public IngresoPagos()
        {
            this.Icon = new Icon("Images/verdum-logo-icono.ico");
            InitializeComponent();
        }

        private void btnIngresoPagos_Click(object sender, EventArgs e)
        {
            decimal efectivo = 0;
            decimal transferencia = 0;

            if (!string.IsNullOrWhiteSpace(txtefectivo.Text))
            {
                if (!decimal.TryParse(txtefectivo.Text, out efectivo))
                {
                    MessageBox.Show("El valor ingresado en 'Efectivo' no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(txttransferencia.Text))
            {
                if (!decimal.TryParse(txttransferencia.Text, out transferencia))
                {
                    MessageBox.Show("El valor ingresado en 'Transferencia' no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Asignar los valores obtenidos
            ValorEfectivo = efectivo;
            ValorTransferencia = transferencia;

            // Cerrar el formulario y retornar el resultado
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
